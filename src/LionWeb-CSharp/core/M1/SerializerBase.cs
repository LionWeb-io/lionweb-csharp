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
using Utilities;

public abstract class SerializerBase : ISerializer
{
    private readonly HashSet<Language> _usedLanguages = new();

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
        new() { Reference = target?.GetId(), ResolveInfo = target.GetNodeName() };

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

    protected SerializedNode SerializeSimpleNode(IReadableNode node)
    {
        Classifier classifier = node switch
        {
            Language => M3Language.Instance.Language,
            Annotation => M3Language.Instance.Annotation,
            Concept => M3Language.Instance.Concept,
            Interface => M3Language.Instance.Interface,
            Enumeration => M3Language.Instance.Enumeration,
            PrimitiveType => M3Language.Instance.PrimitiveType,
            EnumerationLiteral => M3Language.Instance.EnumerationLiteral,
            Property => M3Language.Instance.Property,
            Containment => M3Language.Instance.Containment,
            Reference => M3Language.Instance.Reference,
            _ => node.GetClassifier()
        };

        return new()
        {
            Id = node.GetId(),
            Classifier = classifier.ToMetaPointer(),
            Properties = classifier.AllFeatures().OfType<Property>()
                .Select(property => SerializeProperty(node, property)).ToArray(),
            Containments = classifier.AllFeatures().OfType<Containment>()
                .Select<Containment, SerializedContainment>(containment =>
                    SerializeContainmentsOf(node, containment)).ToArray(),
            References = classifier.AllFeatures().OfType<Reference>()
                .Select<Reference, SerializedReference>(reference => SerializeReferencesOf(node, reference))
                .ToArray(),
            Annotations = node.GetAnnotations()
                .Select(annotation => annotation.GetId()).ToArray(),
            Parent = node.GetParent()?.GetId()
        };
    }

    private SerializedContainment SerializeContainmentsOf(IReadableNode node, Containment containment)
    {
        var value = GetValueIfSet(node, containment);
        return SerializeContainment(value != null ? containment.AsNodes<IReadableNode>(value) : [], containment);
    }

    private SerializedReference SerializeReferencesOf(IReadableNode node, Reference reference)
    {
        var value = GetValueIfSet(node, reference);
        return SerializeReference(value != null ? reference.AsNodes<IReadableNode>(value) : [], reference);
    }

    protected SerializedContainment SerializeContainment(
        IEnumerable<IReadableNode> children,
        Containment containment) =>
        new()
        {
            Containment = containment.ToMetaPointer(),
            Children = children.Select((child) => child.GetId()).ToArray()
        };

    protected SerializedReference SerializeReference(IEnumerable<IReadableNode?> targets, Reference reference) =>
        new()
        {
            Reference = reference.ToMetaPointer(),
            Targets = targets.Where(t => t != null).Select(SerializeReferenceTarget).ToArray()
        };

    protected void RegisterUsedLanguage(IReadableNode node)
    {
        Language language = node.GetClassifier().GetLanguage();
        if (language.Key == BuiltInsLanguage.LionCoreBuiltInsIdAndKey)
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