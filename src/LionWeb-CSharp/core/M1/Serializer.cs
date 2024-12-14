// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;
using System.Collections;
using System.Diagnostics;
using Utilities;

/// <inheritdoc cref="ISerializer"/>
public class Serializer : ISerializer
{
    private readonly DuplicateIdChecker _duplicateIdChecker = new();
    private ILionCoreLanguage _m3;
    private IBuiltInsLanguage _builtIns;
    private readonly HashSet<Language> _usedLanguages = new();

    /// <inheritdoc cref="ISerializer"/>
    public Serializer(LionWebVersions lionWebVersion)
    {
        LionWebVersion = lionWebVersion;
        _m3 = lionWebVersion.LionCore;
        _builtIns = lionWebVersion.BuiltIns;
    }

    /// <inheritdoc />
    public ISerializerHandler Handler { get; init; } = new SerializerExceptionHandler();

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; }

    /// <inheritdoc />
    public IEnumerable<SerializedLanguageReference> UsedLanguages =>
        _usedLanguages.Select(SerializeLanguageReference);

    /// <summary>
    /// Whether we store uncompressed <see cref="IReadableNode.GetId()">node ids</see> and <see cref="MetaPointer">MetaPointers</see> during deserialization.
    /// Uses more memory, but very helpful for debugging. 
    /// </summary>
    public bool StoreUncompressedIds { get; init; } = false;

    /// <inheritdoc />
    public IEnumerable<SerializedNode> Serialize(IEnumerable<IReadableNode> allNodes)
    {
        foreach (var node in allNodes)
        {
            RegisterUsedLanguage(node.GetClassifier().GetLanguage());
            var result = SerializeNode(node);
            if (result != null)
                yield return result;
        }
    }

    private SerializedNode? SerializeNode(IReadableNode node)
    {
        var id = node.GetId();
        if (_duplicateIdChecker.IsIdDuplicate(Compress(id)))
        {
            Handler.DuplicateNodeId(node);
            return null;
        }

        return SerializeSimpleNode(node);
    }

    private SerializedLanguageReference SerializeLanguageReference(Language language) =>
        new() { Key = language.Key, Version = language.Version };

    private SerializedReferenceTarget SerializeReferenceTarget(IReadableNode target)
    {
        if (target is not IKeyed k)
            return new SerializedReferenceTarget { Reference = target.GetId(), ResolveInfo = target.GetNodeName() };

        var hostingLanguage = M1Extensions
            .Ancestors(k, [], true)
            .OfType<Language>()
            .FirstOrDefault();

        return hostingLanguage?.Key switch
        {
            ILionCoreLanguage.LanguageKey => new SerializedReferenceTarget
            {
                Reference = null, ResolveInfo = ConcatResolveInfo(target, ILionCoreLanguage.ResolveInfoPrefix),
            },
            IBuiltInsLanguage.LanguageKey => new SerializedReferenceTarget
            {
                Reference = null, ResolveInfo = ConcatResolveInfo(target, IBuiltInsLanguage.ResolveInfoPrefix),
            },
            _ => new SerializedReferenceTarget { Reference = target.GetId(), ResolveInfo = target.GetNodeName() }
        };
    }

    private static string ConcatResolveInfo(IReadableNode target, string prefix) =>
        prefix + target switch
        {
            Feature f => f.GetFeatureClassifier().Name,
            _ => ""
        } + ((IKeyed)target).Name;

    private CompressedId Compress(string id) =>
        CompressedId.Create(id, StoreUncompressedIds);

    /// <summary>
    /// Serializes the given <paramref name="value">runtime value</paramref> as a string,
    /// conforming to the LionWeb JSON serialization format.
    /// 
    /// <em>Note!</em> No exception is thrown when the given runtime value doesn't correspond to a primitive type defined here.
    /// Instead, the runtime value is simply coerced to a string using its <c>ToString</c> method.
    /// </summary>
    private string? ConvertPrimitiveType(object? value) => value switch
    {
        null => null,
        bool boolean => boolean ? "true" : "false",
        string @string => @string,
        _ => value.ToString()
    };

    /// <remarks>
    /// Features with `string` or _value type_ values are treated as property.
    /// Remaining features with single or IEnumerable&lt;IReadableNode> values with all values' parents == node are treated as containment.
    /// Remaining features with single or IEnumerable&lt;IReadableNode> values are treated as reference.
    /// If any feature still remaining, throws exception.
    /// Only annotations are treated as annotations.
    /// </remarks>
    protected SerializedNode SerializeSimpleNode(IReadableNode node)
    {
        Classifier classifier = ExtractClassifier(node);

        var featureValues = node
            .CollectAllSetFeatures()
            .ToDictionary(f => f, node.Get);

        Dictionary<Feature, object?> properties = CollectProperties(featureValues);
        RemoveFromFeatureValues(properties);

        Dictionary<Feature, object?> containments = CollectContainments(node, featureValues);
        RemoveFromFeatureValues(containments);

        Dictionary<Feature, object?> references = CollectReferences(featureValues);
        RemoveFromFeatureValues(references);

        Debug.Assert(featureValues.Count == 0, $"remaining features: {featureValues}");

        var allFeatures = classifier.AllFeatures();

        return new()
        {
            Id = node.GetId(),
            Classifier = classifier.ToMetaPointer(),
            Properties = properties.Select(pair => SerializeProperty(node, pair.Key, pair.Value))
                .Concat(CollectUnsetProperties(node, allFeatures, properties))
                .ToArray(),
            Containments = containments.Select(SerializeContainment)
                .Concat(CollectUnsetContainments(allFeatures, containments))
                .ToArray(),
            References = references.Select(SerializeReference)
                .Concat(CollectUnsetReferences(allFeatures, references))
                .ToArray(),
            Annotations = node.GetAnnotations().Select(SerializeAnnotationTarget)
                .ToArray(),
            Parent = node.GetParent()?.GetId()
        };

        void RemoveFromFeatureValues(Dictionary<Feature, object?> dictionary)
        {
            foreach (var key in dictionary.Keys)
            {
                featureValues.Remove(key);
            }
        }
    }

    private Classifier ExtractClassifier(IReadableNode node) => node switch
    {
        Language => _m3.Language,
        Annotation => _m3.Annotation,
        Concept => _m3.Concept,
        Interface => _m3.Interface,
        Enumeration => _m3.Enumeration,
        PrimitiveType => _m3.PrimitiveType,
        EnumerationLiteral => _m3.EnumerationLiteral,
        Property => _m3.Property,
        Containment => _m3.Containment,
        Reference => _m3.Reference,
        _ => node.GetClassifier()
    };

    private Dictionary<Feature, object?> CollectProperties(Dictionary<Feature, object?> featureValues) =>
        featureValues
            .Where(pair => pair.Value is string
                           || (pair.Value is not null && pair.Value.GetType().IsValueType)
                           || pair is { Key: Property, Value: null }
            )
            .ToDictionary();

    private Dictionary<Feature, object?> CollectContainments(IReadableNode node,
        Dictionary<Feature, object?> featureValues)
    {
        var containments = featureValues
            .Where(pair =>
                pair.Key is Containment
                || (pair.Value is IReadableNode n && ReferenceEquals(n.GetParent(), node))
                || (pair.Value is IEnumerable<IReadableNode> en && en.All(c => ReferenceEquals(c.GetParent(), node)))
                || (pair.Value is IEnumerable e &&
                    e.Cast<IReadableNode>().All(c => ReferenceEquals(c.GetParent(), node)))
            )
            .ToDictionary();
        return containments;
    }

    private Dictionary<Feature, object?> CollectReferences(Dictionary<Feature, object?> featureValues)
    {
        var references = featureValues
            .Where(pair => pair.Key is Reference
                           || pair.Value is IReadableNode
                           || (pair.Value is IEnumerable e && M2Extensions.AreAll<IReadableNode>(e))
            )
            .ToDictionary();
        return references;
    }

    private class FeatureComparer : IEqualityComparer<Feature>
    {
        public bool Equals(Feature? x, Feature? y) =>
            EqualityExtensions.Equals(x, y) || (IsLionCore(x) && IsLionCore(y) && x?.Key == y?.Key);

        public int GetHashCode(Feature obj) =>
            IsLionCore(obj) ? obj.Key.GetHashCode() : obj.GetHashCodeIdentity();

        private bool IsLionCore(Feature? feature) =>
            feature?.GetLanguage().Key is ILionCoreLanguage.LanguageKey or IBuiltInsLanguage.LanguageKey;
    }

    private IEnumerable<SerializedProperty> CollectUnsetProperties(IReadableNode node, ISet<Feature> allFeatures,
        Dictionary<Feature, object?> properties) =>
        allFeatures
            .OfType<Property>()
            .Except(properties.Keys, new FeatureComparer())
            .Select(p => SerializeProperty(node, p, null));

    private IEnumerable<SerializedContainment> CollectUnsetContainments(ISet<Feature> allFeatures,
        Dictionary<Feature, object?> containments) =>
        allFeatures
            .OfType<Containment>()
            .Except(containments.Keys, new FeatureComparer())
            .Select<Feature, SerializedContainment>(c => SerializeContainment([], c));

    private IEnumerable<SerializedReference> CollectUnsetReferences(ISet<Feature> allFeatures,
        Dictionary<Feature, object?> references) =>
        allFeatures
            .OfType<Reference>()
            .Except(references.Keys, new FeatureComparer())
            .Select<Feature, SerializedReference>(c => SerializeReference([], c));

    private IEnumerable<IReadableNode> AsNodes(KeyValuePair<Feature, object?> pair) =>
        pair.Value != null ? M2Extensions.AsNodes<IReadableNode>(pair.Value) : [];

    private SerializedProperty SerializeProperty(IReadableNode node, Feature feature, object? value)
    {
        if (value is not null && feature is Property { Type: Enumeration enumeration })
            RegisterUsedLanguage(enumeration.GetLanguage());

        return new SerializedProperty
        {
            Property = feature.ToMetaPointer(),
            Value = value switch
            {
                null => null,
                Enum e => e.LionCoreKey(),
                int or bool or string => ConvertPrimitiveType(value),
                _ => Handler.UnknownDatatype(node, feature, value)
            }
        };
    }

    private SerializedContainment SerializeContainment(KeyValuePair<Feature, object?> pair) =>
        SerializeContainment(AsNodes(pair), pair.Key);

    private SerializedContainment SerializeContainment(IEnumerable<IReadableNode> children, Feature containment) =>
        new()
        {
            Containment = containment.ToMetaPointer(), Children = children.Select(child => child.GetId()).ToArray()
        };

    private SerializedReference SerializeReference(KeyValuePair<Feature, object?> pair) =>
        SerializeReference(AsNodes(pair), pair.Key);

    private SerializedReference SerializeReference(IEnumerable<IReadableNode?> targets, Feature reference) =>
        new()
        {
            Reference = reference.ToMetaPointer(),
            Targets = targets.Where(t => t != null).Select(SerializeReferenceTarget!).ToArray()
        };

    private string SerializeAnnotationTarget(IReadableNode annotation) =>
        annotation.GetId();

    private void RegisterUsedLanguage(Language language)
    {
        if (language.EqualsIdentity(_builtIns))
            return;

        Language? existingLanguage = _usedLanguages.FirstOrDefault(l => l != language && l.EqualsIdentity(language));
        if (existingLanguage == null)
        {
            LionWebVersion.AssureCompatible(language);
            _usedLanguages.Add(language);
            return;
        }

        Language? altLanguage = Handler.DuplicateUsedLanguage(existingLanguage, language);
        if (altLanguage != null)
            _usedLanguages.Add(altLanguage);
    }
}