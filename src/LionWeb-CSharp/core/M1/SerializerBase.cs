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

// ReSharper disable SuggestVarOrType_SimpleTypes

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;
using System.Collections;
using Utilities;

public abstract class SerializerBase : ISerializer
{
    private readonly LionWebVersions _lionWebVersion;
    private readonly ILionCoreLanguage _m3;
    private readonly IBuiltInsLanguage _builtIns;

    private readonly HashSet<Language> _usedLanguages = new();

    protected SerializerBase(LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        _m3 = lionWebVersion.GetLionCore();
        _builtIns = lionWebVersion.GetBuiltIns();
    }

    /// <inheritdoc />
    public ISerializerHandler Handler { get; init; } = new SerializerExceptionHandler();

    /// <inheritdoc />
    public IEnumerable<SerializedLanguageReference> UsedLanguages =>
        _usedLanguages.Select(SerializeLanguageReference);


    /// <summary>
    /// Whether we store uncompressed <see cref="IReadableNode.GetId()">node ids</see> and <see cref="MetaPointer">MetaPointers</see> during deserialization.
    /// Uses more memory, but very helpful for debugging. 
    /// </summary>
    public bool StoreUncompressedIds { get; init; } = false;

    public abstract IEnumerable<SerializedNode> Serialize(IEnumerable<IReadableNode> allNodes);

    protected SerializedLanguageReference SerializeLanguageReference(Language language) =>
        new() { Key = language.Key, Version = language.Version };

    protected SerializedProperty SerializeProperty(IReadableNode node, Property property)
    {
        var value = GetValueIfSet(node, property);

        return new SerializedProperty
        {
            Property = property.ToMetaPointer(),
            Value = property.Type switch
            {
                _ when value == null => null,
                PrimitiveType => ConvertPrimitiveType(value),
                Enumeration when value is Enum e => e.LionCoreKey(),
                _ => Handler.UnknownDatatype(node, property, value)
            }
        };
    }

    protected SerializedReferenceTarget SerializeReferenceTarget(IReadableNode? target) =>
        new() { Reference = target?.GetId(), ResolveInfo = target?.GetNodeName() };

    protected object? GetValueIfSet(IReadableNode node, Feature feature) =>
        node.CollectAllSetFeatures().Contains(feature) ? node.Get(feature) : null;

    protected CompressedId Compress(string id) =>
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

        if (featureValues.Count != 0)
            throw new ArgumentException($"remaining features: {featureValues}");

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

    private Classifier ExtractClassifier(IReadableNode node)
    {
        Classifier classifier = node switch
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
        return classifier;
    }

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

    private IEnumerable<SerializedProperty> CollectUnsetProperties(IReadableNode node, ISet<Feature> allFeatures,
        Dictionary<Feature, object?> properties) =>
        allFeatures
            .OfType<Property>()
            .Except(properties.Keys)
            .Select(p => SerializeProperty(node, p, null));

    private IEnumerable<SerializedContainment> CollectUnsetContainments(ISet<Feature> allFeatures,
        Dictionary<Feature, object?> containments) =>
        allFeatures
            .OfType<Containment>()
            .Except(containments.Keys)
            .Select(c => SerializeContainment([], c));

    private IEnumerable<SerializedReference> CollectUnsetReferences(ISet<Feature> allFeatures,
        Dictionary<Feature, object?> references) =>
        allFeatures
            .OfType<Reference>()
            .Except(references.Keys)
            .Select(c => SerializeReference([], c));

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

    protected SerializedContainment SerializeContainment(IEnumerable<IReadableNode> children, Feature containment) =>
        new()
        {
            Containment = containment.ToMetaPointer(), Children = children.Select(child => child.GetId()).ToArray()
        };

    private SerializedReference SerializeReference(KeyValuePair<Feature, object?> pair) =>
        SerializeReference(AsNodes(pair), pair.Key);

    protected SerializedReference SerializeReference(IEnumerable<IReadableNode?> targets, Feature reference) =>
        new()
        {
            Reference = reference.ToMetaPointer(),
            Targets = targets.Where(t => t != null).Select(SerializeReferenceTarget).ToArray()
        };

    private string SerializeAnnotationTarget(IReadableNode annotation) =>
        annotation.GetId();

    protected void RegisterUsedLanguage(Language language)
    {
        if (language.EqualsIdentity(_builtIns))
            return;

        Language? existingLanguage = _usedLanguages.FirstOrDefault(l => l != language && l.EqualsIdentity(language));
        if (existingLanguage == null)
        {
            _usedLanguages.Add(language);
            return;
        }

        Language? altLanguage = Handler.DuplicateUsedLanguage(existingLanguage, language);
        if (altLanguage != null)
            _usedLanguages.Add(altLanguage);
    }
}