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
    private readonly HashSet<Language> _usedLanguages = new(new LanguageIdentityComparer());
    private readonly ISerializerHandler _handler = new SerializerExceptionHandler();
    private readonly Dictionary<IKeyed, MetaPointer> _metaPointers = [];
    private readonly Dictionary<Classifier, ISet<Feature>> _allFeatures = [];

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

        if (_usedLanguages.Add(language))
        {
            LionWebVersion.AssureCompatible(language);
            _usedLanguages.Add(language);
            return;
        }

        _usedLanguages.TryGetValue(language, out var existingLanguage);
        if (ReferenceEquals(language, existingLanguage))
            return;
        Language? altLanguage = Handler.DuplicateUsedLanguage(existingLanguage!, language);
        if (altLanguage != null)
            _usedLanguages.Add(altLanguage);
    }

    private SerializedNode? SerializeNode(IReadableNode node)
    {
        var id = node.GetId();
        if (!_duplicateIdChecker.IsIdDuplicate(id))
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

        Dictionary<Feature, object?> featureValues = CollectFeatureValues(node, classifier);

        featureValues.RemoveAll(CollectProperties(node, featureValues, out var serializedProperties));

        featureValues.RemoveAll(CollectContainmentsMultiple(featureValues, out var serializedSingleContainments));
        featureValues.RemoveAll(CollectContainmentsSingle(featureValues, out var serializedMultipleContainments));

        featureValues.RemoveAll(CollectReferencesMultiple(featureValues, out var serializedSingleReferences));
        featureValues.RemoveAll(CollectReferencesSingle(featureValues, out var serializedMultipleReferences));

        Debug.Assert(featureValues.Count == 0, $"remaining features: {featureValues}");

        var annotationIds = SerializeAnnotations(node);

        return new()
        {
            Id = node.GetId(),
            Classifier = ToMetaPointer(classifier),
            Properties = serializedProperties.ToArray(),
            Containments = serializedSingleContainments.Concat(serializedMultipleContainments).ToArray(),
            References = serializedSingleReferences.Concat(serializedMultipleReferences).ToArray(),
            Annotations = annotationIds,
            Parent = node.GetParent()?.GetId()
        };
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

    private Dictionary<Feature, object?> CollectFeatureValues(IReadableNode node, Classifier classifier)
    {
        var features = new HashSet<Feature>(_featureComparer);
        features.AddAll(node.CollectAllSetFeatures());
        features.AddAll(AllFeatures(classifier));

        Dictionary<Feature, object?> featureValues = [];
        foreach (Feature feature in features)
        {
            if (node.TryGetRaw(feature, out var value))
            {
                if (SerializeEmptyFeatures ||  value is not null and not IList { Count: 0 })
                    featureValues[feature] = value;
            } else if (SerializeEmptyFeatures)
            {
                featureValues[feature] = null;
            }
        }

        return featureValues;
    }

    private ISet<Feature> AllFeatures(Classifier classifier)
    {
        if (!_allFeatures.TryGetValue(classifier, out var result))
        {
            result = classifier.AllFeatures();
            _allFeatures[classifier] = result;
        }
        return result;
    }

    #region Properties

    private List<Feature> CollectProperties(IReadableNode node, Dictionary<Feature, object?> featureValues, out List<SerializedProperty> serializedProperties)
    {
        List<Feature> result = [];
        serializedProperties = [];
        foreach (var pair in featureValues)
        {
            if (pair.Value is string
                || (pair.Value is not null && pair.Value.GetType().IsValueType)
                || pair is { Key: Property, Value: null })
            {
                result.Add(pair.Key);
                serializedProperties.Add(SerializeProperty(node, pair.Key, pair.Value));
            }
        }
        return result;
    }

    private SerializedProperty SerializeProperty(IReadableNode node, Feature property, object? value)
    {
        RegisterUsedLanguage(property.GetLanguage());
        return _versionSpecifics.SerializeProperty(node, property, value);
    }

    #endregion

    #region Containments

    private List<Feature> CollectContainmentsSingle(Dictionary<Feature, object?> featureValues, out List<SerializedContainment> serializedContainments)
    {
        List<Feature> result = [];
        serializedContainments = [];
        foreach (var pair in featureValues)
        {
            if (pair.Value is IReadableNode || pair is { Key: Containment, Value: null })
            {
                result.Add(pair.Key);
                serializedContainments.Add(SerializeSingleContainment(pair.Value as IReadableNode, pair.Key));
            }
        }
        return result;
    }

    private SerializedContainment SerializeSingleContainment(IReadableNode? child, Feature containment)
    {
        RegisterUsedLanguage(containment.GetLanguage());
        return new SerializedContainment
        {
            Containment = ToMetaPointer(containment), Children = child is not null ? [child.GetId()] : []
        };
    }

    private List<Feature> CollectContainmentsMultiple(Dictionary<Feature, object?> featureValues, out List<SerializedContainment> serializedContainments)
    {
        List<Feature> result = [];
        serializedContainments = [];
        foreach (var pair in featureValues)
        {
            if (pair.Value is IReadOnlyList<IReadableNode> children)
            {
                result.Add(pair.Key);
                serializedContainments.Add(SerializeMultipleContainment(children, pair.Key));
            }
        }
        return result;
    }

    private SerializedContainment SerializeMultipleContainment(IEnumerable<IReadableNode> children, Feature containment)
    {
        RegisterUsedLanguage(containment.GetLanguage());
        return new SerializedContainment
        {
            Containment = ToMetaPointer(containment), Children = children.Select(child => child.GetId()).ToArray()
        };
    }

    #endregion

    #region References

    private List<Feature> CollectReferencesSingle(Dictionary<Feature, object?> featureValues, out List<SerializedReference> serializedReferences)
    {
        List<Feature> result = [];
        serializedReferences = [];
        foreach (var pair in featureValues)
        {
            if (pair.Value is ReferenceTarget || pair is { Key: Reference, Value: null })
            {
                result.Add(pair.Key);
                serializedReferences.Add(SerializeSingleReference(pair.Value as ReferenceTarget, pair.Key));
            }
        }
        return result;
    }

    private SerializedReference SerializeSingleReference(ReferenceTarget? target, Feature reference)
    {
        RegisterUsedLanguage(reference.GetLanguage());
        return new SerializedReference
        {
            Reference = ToMetaPointer(reference), Targets = target is not null ? [SerializeReferenceTarget(target)] : []
        };
    }

    private List<Feature> CollectReferencesMultiple(Dictionary<Feature, object?> featureValues, out List<SerializedReference> serializedReferences)
    {
        List<Feature> result = [];
        serializedReferences = [];
        foreach (var pair in featureValues)
        {
            if (pair.Value is IReadOnlyList<ReferenceTarget> targets)
            {
                result.Add(pair.Key);
                serializedReferences.Add(SerializeMultipleReference(targets, pair.Key));
            }
        }
        return result;
    }

    private SerializedReference SerializeMultipleReference(IEnumerable<ReferenceTarget> targets, Feature reference)
    {
        RegisterUsedLanguage(reference.GetLanguage());
        return new SerializedReference
        {
            Reference = ToMetaPointer(reference), Targets = targets.Select(SerializeReferenceTarget).ToArray()
        };
    }

    private SerializedReferenceTarget SerializeReferenceTarget(ReferenceTarget target)
    {
        if (target.Target is not IKeyed k)
            return new SerializedReferenceTarget
            {
                Reference = target.Target?.GetId() ?? target.TargetId, ResolveInfo = target.ResolveInfo
            };

        var hostingLanguage = k.GetLanguage();

        var referenceTarget = PersistLionCoreReferenceTargetIds ? target.TargetId : null;

        return hostingLanguage.Key switch
        {
            ILionCoreLanguage.LanguageKey => new SerializedReferenceTarget
            {
                Reference = referenceTarget,
                ResolveInfo = ConcatResolveInfo(target.Target, ILionCoreLanguage.ResolveInfoPrefix)
            },
            IBuiltInsLanguage.LanguageKey => new SerializedReferenceTarget
            {
                Reference = referenceTarget,
                ResolveInfo = ConcatResolveInfo(target.Target, IBuiltInsLanguage.ResolveInfoPrefix)
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

    private NodeId[] SerializeAnnotations(IReadableNode node)
    {
        var annotations = node.GetAnnotationsRaw();
        NodeId[] annotationIds = new NodeId[annotations.Count];
        for (var i = 0; i < annotations.Count; i++)
        {
            annotationIds[i] = SerializeAnnotationTarget(annotations[i]);
        }

        return annotationIds;
    }

    private static NodeId SerializeAnnotationTarget(IReadableNode annotation) =>
        annotation.GetId();

    private static SerializedLanguageReference SerializeLanguageReference(Language language) =>
        new() { Key = language.Key, Version = language.Version };

    #region Helpers

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

    private MetaPointer ToMetaPointer(IKeyed keyed)
    {
        if (_metaPointers.TryGetValue(keyed, out var result))
            return result;

        result = keyed.ToMetaPointer();
        _metaPointers[keyed] = result;
        return result;
    }

    #endregion
}

internal static class CollectionExtensions
{
    public static void AddAll(this HashSet<Feature> set, IEnumerable<Feature> newEntries)
    {
        foreach (var newEntry in newEntries)
        {
            set.Add(newEntry);
        }
    }

    public static void RemoveAll(this Dictionary<Feature, object?> dictionary, IEnumerable<Feature> keysToRemove)
    {
        foreach (var feature in keysToRemove)
        {
            dictionary.Remove(feature);
        }
    }
}