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

namespace LionWeb.Core.M2;

using M1;
using M3;
using Serialization;

/// <summary>
/// A deserializer that deserializes serializations of <see cref="Language"/>s.
/// The generic deserializer isn't aware of the LionCore M3-types (and their idiosyncrasies),
/// so that can't be used.
/// </summary>
public partial class LanguageDeserializer : DeserializerBase<IReadableNode>, ILanguageDeserializer
{
    private static readonly M3Language _m3 = M3Language.Instance;

    private readonly Dictionary<CompressedId, SerializedNode> _serializedNodesById = new();

    private readonly DeserializerBuilder _deserializerBuilder = new();

    /// <summary>
    /// Deserializes the given <paramref name="serializationChunk">serialization chunk</paramref> as an iterable collection of <see cref="Language"/>s.
    /// The <paramref name="dependentLanguages">dependent languages</paramref> should contain all languages that are referenced by the top-level
    /// <c>languages</c> property of the serialization chunk.
    /// </summary>
    ///
    /// <param name="serializationChunk">Chunk to deserialize.</param>
    /// <param name="preloadM3Language">Whether <see cref="M3Language"/> should be preloaded. Keep at <c>true</c> unless deserializing meta-languages.</param>
    /// <param name="dependentLanguages">Referred languages.</param>
    /// 
    /// <returns>The deserialization of the language definitions present in the given <paramref name="serializationChunk"/>.</returns>
    public LanguageDeserializer(bool preloadM3Language = true)
    {
        RegisterDependentLanguage(BuiltInsLanguage.Instance);

        if (preloadM3Language)
            RegisterDependentLanguage(_m3);
    }

    /// <inheritdoc cref="ILanguageDeserializerExtensions.Deserialize"/>
    public static IEnumerable<DynamicLanguage> Deserialize(SerializationChunk serializationChunk,
        params Language[] dependentLanguages) =>
        new LanguageDeserializer().Deserialize(serializationChunk, dependentLanguages);

    /// <inheritdoc />
    public override void RegisterInstantiatedLanguage(Language language, INodeFactory factory)
    {
        base.RegisterInstantiatedLanguage(language, factory);
        _deserializerBuilder.WithCustomFactory(language, factory);
    }

    /// <inheritdoc />
    public void RegisterDependentLanguage(Language language)
    {
        _deserializerBuilder.WithLanguage(language);
        RegisterDependentNodes(M1Extensions.Descendants<IKeyed>(language, true));
    }

    #region Process

    /// <inheritdoc />
    public override void Process(SerializedNode serializedNode)
    {
        _serializedNodesById[Compress(serializedNode.Id)] = serializedNode;
        if (!IsLanguageNode(serializedNode))
            return;

        var id = serializedNode.Id;
        var compressedId = Compress(id);
        if (!(IsInDependentNodes(compressedId) && Handler.SkipDeserializingDependentNode(id)))
            _deserializedNodesById[compressedId] = DeserializeMemoized(compressedId);
    }

    private bool IsInDependentNodes(CompressedId compressedId) =>
        _dependentNodesById.ContainsKey(compressedId);

    private IReadableNode DeserializeMemoized(CompressedId compressedId)
    {
        var serializedNode = _serializedNodesById[compressedId];
        if (!_deserializedNodesById.TryGetValue(compressedId, out var node))
        {
            node = CreateNodeWithProperties(serializedNode);
        }

        return node;
    }

    private DynamicIKeyed CreateNodeWithProperties(SerializedNode serializedNode)
    {
        var serializedPropertiesByKey = serializedNode.Properties.ToDictionary(
            serializedProperty => Compress(serializedProperty.Property),
            serializedProperty => serializedProperty.Value
        );
        var id = serializedNode.Id;
        string key = LookupString(_m3.IKeyed_key);
        string name = LookupString(BuiltInsLanguage.Instance.INamed_name);

        return serializedNode.Classifier switch
        {
            var s when s.Matches(_m3.Annotation) => new DynamicAnnotation(id, null) { Key = key, Name = name },
            var s when s.Matches(_m3.Concept) => new DynamicConcept(id, null)
            {
                Key = key,
                Name = name,
                Abstract = LookupBool(_m3.Concept_abstract),
                Partition = LookupBool(_m3.Concept_partition)
            },
            var s when s.Matches(_m3.Containment) => new DynamicContainment(id, null)
            {
                Key = key,
                Name = name,
                Optional = LookupBool(_m3.Feature_optional),
                Multiple = LookupBool(_m3.Link_multiple)
            },
            var s when s.Matches(_m3.Enumeration) => new DynamicEnumeration(id, null) { Key = key, Name = name },
            var s when s.Matches(_m3.EnumerationLiteral) => new DynamicEnumerationLiteral(id, null)
            {
                Key = key, Name = name
            },
            var s when s.Matches(_m3.Interface) => new DynamicInterface(id, null) { Key = key, Name = name },
            var s when s.Matches(_m3.Language) => new DynamicLanguage(id)
            {
                Key = key, Name = name, Version = LookupString(_m3.Language_version)
            },
            var s when s.Matches(_m3.PrimitiveType) => new DynamicPrimitiveType(id, null) { Key = key, Name = name },
            var s when s.Matches(_m3.Property) => new DynamicProperty(id, null)
            {
                Key = key, Name = name, Optional = LookupBool(_m3.Feature_optional)
            },
            var s when s.Matches(_m3.Reference) => new DynamicReference(id, null)
            {
                Key = key,
                Name = name,
                Optional = LookupBool(_m3.Feature_optional),
                Multiple = LookupBool(_m3.Link_multiple)
            },
            _ => throw new UnsupportedClassifierException(serializedNode.Classifier)
        };

        bool LookupBool(Property property)
        {
            var compressedMetaPointer = Compress(property.ToMetaPointer());
            if (serializedPropertiesByKey.TryGetValue(compressedMetaPointer, out var value))
                return value == "true";

            var result = Handler.UnknownDatatype(id, property, null);
            return result as bool? ?? throw new InvalidValueException(property, result);
        }

        string LookupString(Property property)
        {
            var compressedMetaPointer = Compress(property.ToMetaPointer());
            if (serializedPropertiesByKey.TryGetValue(compressedMetaPointer, out var s) && s != null)
                return s;

            var result = Handler.UnknownDatatype(id, property, null);
            return result as string ?? throw new InvalidValueException(property, result);
        }
    }

    #endregion

    private static bool IsLanguageNode(SerializedNode serializedNode) =>
        serializedNode.Classifier.Language == _m3.Key;
}

internal class AnnotationDeserializerHandler(IDeserializerHandler @delegate) : IDeserializerHandler
{
    public INode? UnknownParent(CompressedId parentId, INode node) =>
        null;

    public Classifier? UnknownClassifier(string id, MetaPointer metaPointer) =>
        @delegate.UnknownClassifier(id, metaPointer);

    public Feature? UnknownFeature(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        IReadableNode node) =>
        @delegate.UnknownFeature(classifier, compressedMetaPointer, node);

    public TFeature? InvalidFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        INode node) where TFeature : class, Feature =>
        @delegate.InvalidFeature<TFeature>(classifier, compressedMetaPointer, node);

    public INode? UnknownChild(CompressedId childId, IWritableNode node) =>
        @delegate.UnknownChild(childId, node);

    public IReadableNode? UnknownReference(CompressedId targetId, string? resolveInfo, IWritableNode node) =>
        @delegate.UnknownReference(targetId, resolveInfo, node);

    public INode? UnknownAnnotation(CompressedId annotationId, INode node) =>
        @delegate.UnknownAnnotation(annotationId, node);

    public INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
        @delegate.InvalidAnnotation(annotation, node);

    public Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key) =>
        @delegate.UnknownEnumerationLiteral(nodeId, enumeration, key);

    public object? UnknownDatatype(string nodeId, Property property, string? value) =>
        @delegate.UnknownDatatype(nodeId, property, value);

    public bool SkipDeserializingDependentNode(string id) =>
        @delegate.SkipDeserializingDependentNode(id);

    public TFeature? InvalidFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
        IReadableNode node) where TFeature : class, Feature =>
        @delegate.InvalidFeature<TFeature>(classifier, compressedMetaPointer, node);

    public void InvalidContainment(IReadableNode node) =>
        @delegate.InvalidContainment(node);

    public void InvalidReference(IReadableNode node) =>
        @delegate.InvalidReference(node);

    public IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId) =>
        @delegate.InvalidAnnotationParent(annotation, parentId);
}