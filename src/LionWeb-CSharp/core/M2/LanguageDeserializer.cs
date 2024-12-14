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

/// <inheritdoc cref="ILanguageDeserializer"/>
public partial class LanguageDeserializer : DeserializerBase<IReadableNode>, ILanguageDeserializer
{
    private readonly Dictionary<CompressedId, SerializedNode> _serializedNodesById = new();

    private readonly DeserializerBuilder _deserializerBuilder = new();

    /// <summary>
    /// Deserializes languages based on LionWeb version encoded in <paramref name="versionSpecifics"/>.
    /// </summary>
    /// <param name="versionSpecifics">Version of LionWeb standard to use for deserializing.</param>
    /// <param name="preloadLionCoreLanguage"><c>true</c> if LionCore M3 should be preloaded (defaults to <c>true</c>).</param>
    public LanguageDeserializer(IDeserializerVersionSpecifics versionSpecifics, bool preloadLionCoreLanguage = true) :
        base(versionSpecifics)
    {
        RegisterDependentLanguage(_builtIns);

        if (preloadLionCoreLanguage)
            RegisterDependentLanguage(_m3);
    }
    
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
        RegisterDependentNodes(M1Extensions.Descendants<IKeyed>(language, [], true, true));
    }

    /// <inheritdoc />
    public override void RegisterDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        var nodes = dependentNodes.ToList();
        base.RegisterDependentNodes(nodes);
        _deserializerBuilder.WithDependentNodes(nodes);
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
        if (!IsInDependentNodes(compressedId) || !Handler.SkipDeserializingDependentNode(compressedId))
            _deserializedNodesById[compressedId] = DeserializeMemoized(compressedId);
    }

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
        string name = LookupString(_builtIns.INamed_name);

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
            var s when s.Matches(_m3.Language) => new DynamicLanguage(id, LionWebVersion)
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

            var result = Handler.InvalidPropertyValue<bool>(null, property, Compress(id));
            return result as bool? ?? throw new InvalidValueException(property, result);
        }

        string LookupString(Property property)
        {
            var compressedMetaPointer = Compress(property.ToMetaPointer());
            if (serializedPropertiesByKey.TryGetValue(compressedMetaPointer, out var s) && s != null)
                return s;

            var result = Handler.InvalidPropertyValue<string>(null, property, Compress(id));
            return result as string ?? throw new InvalidValueException(property, result);
        }
    }

    #endregion

    private bool IsLanguageNode(SerializedNode serializedNode) =>
        serializedNode.Classifier.Language == _m3.Key;
}

internal class AnnotationDeserializerHandler(IDeserializerHandler @delegate) : IDeserializerHandler
{
    public IWritableNode? UnresolvableParent(CompressedId parentId, IReadableNode node) =>
        null;

    public Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id) =>
        @delegate.UnknownClassifier(classifier, id);

    public string? DuplicateNodeId(CompressedId nodeId, IReadableNode existingNode, INode node) =>
        @delegate.DuplicateNodeId(nodeId, existingNode, node);

    public T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages) where T : class, IKeyed =>
        @delegate.SelectVersion<T>(metaPointer, languages);

    public Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IReadableNode node)
        where TFeature : class, Feature =>
        @delegate.UnknownFeature<TFeature>(feature, classifier, node);

    public List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IReadableNode node)
        where T : class, IReadableNode =>
        @delegate.InvalidLinkValue(value, link, node);

    public IWritableNode? UnresolvableChild(CompressedId childId, Feature containment, IReadableNode node) =>
        @delegate.UnresolvableChild(childId, containment, node);

    public IReadableNode? UnresolvableReferenceTarget(CompressedId? targetId, string? resolveInfo, Feature reference,
        IReadableNode node) =>
        @delegate.UnresolvableReferenceTarget(targetId, resolveInfo, reference, node);

    public IWritableNode? UnresolvableAnnotation(CompressedId annotationId, IReadableNode node) =>
        @delegate.UnresolvableAnnotation(annotationId, node);

    public IWritableNode? InvalidAnnotation(IReadableNode annotation, IReadableNode node) =>
        @delegate.InvalidAnnotation(annotation, node);

    public IWritableNode? CircularContainment(IReadableNode containedNode, IReadableNode parent) =>
        @delegate.CircularContainment(containedNode, parent);

    public bool DuplicateContainment(IReadableNode containedNode, IReadableNode newParent,
        IReadableNode existingParent) =>
        @delegate.DuplicateContainment(containedNode, newParent, existingParent);

    public Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration, Feature property,
        IReadableNode nodeId) =>
        @delegate.UnknownEnumerationLiteral(key, enumeration, property, nodeId);

    public object? UnknownDatatype(Feature property, string? value, IReadableNode nodeId) =>
        @delegate.UnknownDatatype(property, value, nodeId);

    public object? InvalidPropertyValue<TValue>(string? value, Feature property, CompressedId nodeId) =>
        @delegate.InvalidPropertyValue<TValue>(value, property, nodeId);

    public bool SkipDeserializingDependentNode(CompressedId id) =>
        @delegate.SkipDeserializingDependentNode(id);

    public Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IReadableNode node)
        where TFeature : class, Feature =>
        @delegate.InvalidFeature<TFeature>(feature, classifier, node);

    public void InvalidContainment(IReadableNode node) =>
        @delegate.InvalidContainment(node);

    public void InvalidReference(IReadableNode node) =>
        @delegate.InvalidReference(node);

    public void InvalidAnnotationParent(IReadableNode annotation, IReadableNode? parent) =>
        @delegate.InvalidAnnotationParent(annotation, parent);
}