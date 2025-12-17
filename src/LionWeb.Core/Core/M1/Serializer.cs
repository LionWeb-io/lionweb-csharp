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
    private readonly FeatureComparer _featureComparer = new();
    private readonly ILionCoreLanguage _m3;
    private readonly IBuiltInsLanguage _builtIns;
    private readonly ISerializerVersionSpecifics _versionSpecifics;
    private readonly HashSet<Language> _usedLanguages = new();
    private readonly ISerializerHandler _handler = new SerializerExceptionHandler();

    /// <inheritdoc cref="ISerializer"/>
    public Serializer(LionWebVersions lionWebVersion)
    {
        LionWebVersion = lionWebVersion;
        _m3 = lionWebVersion.LionCore;
        _builtIns = lionWebVersion.BuiltIns;
        _versionSpecifics = ISerializerVersionSpecifics.Create(lionWebVersion);
        _versionSpecifics.Initialize(this, _handler);
    }

    /// <inheritdoc />
    public ISerializerHandler Handler
    {
        get => _handler;
        init
        {
            _handler = value;
            _versionSpecifics.Initialize(this, value);
        }
    }

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; }

    /// <inheritdoc />
    public IEnumerable<SerializedLanguageReference> UsedLanguages =>
        _usedLanguages.Select(SerializeLanguageReference);

    /// Whether we store uncompressed <see cref="IReadableNode.GetId()">node ids</see> and <see cref="MetaPointer">MetaPointers</see> during deserialization.
    /// Uses more memory, but very helpful for debugging.
    /// Defaults to <c>false</c>. 
    public CompressedIdConfig CompressedIdConfig { get; init; } = new();

    /// Whether references to LionCore nodes (<see cref="ILionCoreLanguage"/>, <see cref="IBuiltInsLanguage"/>)
    /// should include the target node's id, or only the resolveInfo.
    /// Defaults to false.
    public bool PersistLionCoreReferenceTargetIds { get; init; } = false;

    /// <inheritdoc />
    public bool SerializeEmptyFeatures { get; init; } = true;

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

    internal void RegisterUsedLanguage(Language language)
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

    private SerializedNode? SerializeNode(IReadableNode node)
    {
        var id = node.GetId();
        if (!_duplicateIdChecker.IsIdDuplicate(Compress(id)))
            return SerializeSimpleNode(node);

        Handler.DuplicateNodeId(node);
        return null;
    }

    /// <remarks>
    /// Features with `string` or _value type_ values are treated as property.
    /// Remaining features with single or IEnumerable&lt;IReadableNode> values with all values' parents == node are treated as containment.
    /// Remaining features with single or IEnumerable&lt;IReadableNode> values are treated as reference.
    /// If any feature still remaining, throws exception.
    /// Only annotations are treated as annotations.
    /// </remarks>
    private SerializedNode SerializeSimpleNode(IReadableNode node)
    {
        Classifier classifier = ExtractClassifier(node);

        var featureValuePairs = node.CollectAllSetFeatures()
            .Concat(classifier.AllFeatures())
            .Distinct(_featureComparer)
            .Select(f => (f, node.TryGetRaw(f, out var r) ? r : null));

        if (!SerializeEmptyFeatures)
            featureValuePairs = featureValuePairs.Where(p => p.Item2 is not null and not IList { Count: 0 });

        var featureValues = featureValuePairs.ToDictionary();

        Dictionary<Feature, object?> properties = CollectProperties(featureValues);
        RemoveFromFeatureValues(properties.Keys);

        Dictionary<Feature, IReadOnlyList<IReadableNode>?> containmentsMultiple = CollectContainmentsMultiple(featureValues);
        RemoveFromFeatureValues(containmentsMultiple.Keys);
        Dictionary<Feature, IReadableNode?> containmentsSingle = CollectContainmentsSingle(featureValues);
        RemoveFromFeatureValues(containmentsSingle.Keys);

        Dictionary<Feature, IReadOnlyList<ReferenceTarget>?> referencesMultiple = CollectReferencesMultiple(featureValues);
        RemoveFromFeatureValues(referencesMultiple.Keys);
        Dictionary<Feature, ReferenceTarget?> referencesSingle = CollectReferencesSingle(featureValues);
        RemoveFromFeatureValues(referencesSingle.Keys);

        Debug.Assert(featureValues.Count == 0, $"remaining features: {featureValues}");

        return new()
        {
            Id = node.GetId(),
            Classifier = classifier.ToMetaPointer(),
            Properties = properties.Select(pair => SerializeProperty(node, pair.Key, pair.Value))
                .ToArray(),
            Containments = containmentsSingle
                .Select(p => SerializeContainment(p.Value is not null ? [p.Value] : [], p.Key))
                .Concat(containmentsMultiple.Select(p => SerializeContainment(p.Value ?? [], p.Key)))
                .ToArray(),
            References = referencesSingle.Select(p => SerializeReference(p.Value is not null ? [p.Value] : [], p.Key))
                .Concat(referencesMultiple.Select(p => SerializeReference(p.Value ?? [], p.Key)))
                .ToArray(),
            Annotations = node.GetAnnotations().Select(SerializeAnnotationTarget)
                .ToArray(),
            Parent = node.GetParent()?.GetId()
        };

        void RemoveFromFeatureValues(IEnumerable<Feature> keys)
        {
            foreach (var key in keys)
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

    #region Properties

    private Dictionary<Feature, object?> CollectProperties(Dictionary<Feature, object?> featureValues) =>
        featureValues
            .Where(pair => pair.Value is string
                           || (pair.Value is not null && pair.Value.GetType().IsValueType)
                           || pair is { Key: Property, Value: null }
            )
            .ToDictionary();

    private SerializedProperty SerializeProperty(IReadableNode node, Feature property, object? value)
    {
        RegisterUsedLanguage(property.GetLanguage());
        return _versionSpecifics.SerializeProperty(node, property, value);
    }

    #endregion

    #region Containments

    private Dictionary<Feature, IReadableNode?> CollectContainmentsSingle(Dictionary<Feature, object?> featureValues)
    {
        var containments = featureValues
            .Where(pair => pair.Value is IReadableNode || pair is { Key: Containment, Value: null })
            .ToDictionary(p => p.Key, p => (IReadableNode?)p.Value);
        return containments;
    }

    private Dictionary<Feature, IReadOnlyList<IReadableNode>?> CollectContainmentsMultiple(
        Dictionary<Feature, object?> featureValues)
    {
        var containments = featureValues
            .Where(pair => pair.Value is IReadOnlyList<IReadableNode>)
            .ToDictionary(p => p.Key, p => (IReadOnlyList<IReadableNode>?)p.Value);
        return containments;
    }

    private SerializedContainment SerializeContainment(IEnumerable<IReadableNode> children, Feature containment)
    {
        RegisterUsedLanguage(containment.GetLanguage());
        return new SerializedContainment
        {
            Containment = containment.ToMetaPointer(), Children = children.Select(child => child.GetId()).ToArray()
        };
    }

    #endregion

    #region References

    private Dictionary<Feature, ReferenceTarget?> CollectReferencesSingle(Dictionary<Feature, object?> featureValues)
    {
        var references = featureValues
            .Where(pair => pair.Value is ReferenceTarget || pair is { Key: Reference, Value: null })
            .ToDictionary(p => p.Key, p => (ReferenceTarget?)p.Value);
        return references;
    }

    private Dictionary<Feature, IReadOnlyList<ReferenceTarget>?> CollectReferencesMultiple(
        Dictionary<Feature, object?> featureValues)
    {
        var references = featureValues
            .Where(pair => pair.Value is IReadOnlyList<ReferenceTarget>)
            .ToDictionary(p => p.Key, p => (IReadOnlyList<ReferenceTarget>?)p.Value);
        return references;
    }

    private SerializedReference SerializeReference(IEnumerable<ReferenceTarget> targets, Feature reference)
    {
        RegisterUsedLanguage(reference.GetLanguage());
        return new SerializedReference
        {
            Reference = reference.ToMetaPointer(), Targets = targets.Select(SerializeReferenceTarget).ToArray()
        };
    }

    private SerializedReferenceTarget SerializeReferenceTarget(ReferenceTarget target)
    {
        if (target.Target is not IKeyed k)
            return new SerializedReferenceTarget
            {
                Reference = target.Target?.GetId() ?? target.TargetId, ResolveInfo = target.ResolveInfo
            };

        var hostingLanguage = M1Extensions
            .Ancestors(k, true)
            .OfType<Language>()
            .FirstOrDefault();

        var referenceTarget = PersistLionCoreReferenceTargetIds ? target.TargetId : null;

        return hostingLanguage?.Key switch
        {
            ILionCoreLanguage.LanguageKey => new SerializedReferenceTarget
            {
                Reference = referenceTarget,
                ResolveInfo = ConcatResolveInfo(target.Target, ILionCoreLanguage.ResolveInfoPrefix),
            },
            IBuiltInsLanguage.LanguageKey => new SerializedReferenceTarget
            {
                Reference = referenceTarget,
                ResolveInfo = ConcatResolveInfo(target.Target, IBuiltInsLanguage.ResolveInfoPrefix),
            },
            _ => new SerializedReferenceTarget
            {
                Reference = target.TargetId, ResolveInfo = target.Target.GetNodeName()
            }
        };
    }

    private static ResolveInfo ConcatResolveInfo(IReadableNode target, string prefix) =>
        prefix + target switch
        {
            Feature f => f.GetFeatureClassifier().Name,
            _ => SerializationConstants.Empty
        } + ((IKeyed)target).Name;

    #endregion

    private NodeId SerializeAnnotationTarget(IReadableNode annotation) =>
        annotation.GetId();

    private SerializedLanguageReference SerializeLanguageReference(Language language) =>
        new() { Key = language.Key, Version = language.Version };

    #region Helpers

    private ICompressedId Compress(NodeId id) =>
        ICompressedId.Create(id, CompressedIdConfig);

    /// Compares features with special handling for LionCore features (only compared by key).
    private class FeatureComparer : IEqualityComparer<Feature>
    {
        public bool Equals(Feature? x, Feature? y) =>
            EqualityExtensions.Equals(x, y) || (IsLionCore(x) && IsLionCore(y) && x?.Key == y?.Key);

        public int GetHashCode(Feature obj) =>
            IsLionCore(obj) ? obj.Key.GetHashCode() : obj.GetHashCodeIdentity();

        private bool IsLionCore(Feature? feature) =>
            feature?.GetLanguage().Key is ILionCoreLanguage.LanguageKey or IBuiltInsLanguage.LanguageKey;
    }

    #endregion
}