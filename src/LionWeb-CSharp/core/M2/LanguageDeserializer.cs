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
    public LanguageDeserializer(IDeserializerVersionSpecifics versionSpecifics) :
        base(versionSpecifics)
    {
        versionSpecifics.RegisterBuiltins(this);
        RegisterDependentLanguage(_m3);
    }

    /// <inheritdoc />
    public override void RegisterInstantiatedLanguage(Language language, INodeFactory? factory = null)
    {
        base.RegisterInstantiatedLanguage(language, factory);
        _deserializerBuilder.WithCustomFactory(language, factory ?? language.GetFactory());
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

    private bool IsLanguageNode(SerializedNode serializedNode) =>
        serializedNode.Classifier.Language == _m3.Key;
}

// TODO still needed?
internal class AnnotationDeserializerHandler(IDeserializerHandler @delegate) : IDeserializerHandler
{
    public Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id) =>
        @delegate.UnknownClassifier(classifier, id);

    public string? DuplicateNodeId(CompressedId nodeId, IReadableNode existingNode, IReadableNode node) =>
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

    public IWritableNode? InvalidAnnotation(IReadableNode annotation, IReadableNode? node) =>
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