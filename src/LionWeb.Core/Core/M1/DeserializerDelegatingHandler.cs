// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

using M3;

/// Delegates all calls to <paramref name="delegateHandler"/>.
public class DeserializerDelegatingHandler(IDeserializerHandler delegateHandler) : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(CompressedMetaPointer classifier, ICompressedId id) =>
        delegateHandler.UnknownClassifier(classifier, id);

    /// <inheritdoc />
    public virtual NodeId? DuplicateNodeId(ICompressedId nodeId, IReadableNode existingNode, IReadableNode node) =>
        delegateHandler.DuplicateNodeId(nodeId, existingNode, node);

    /// <inheritdoc />
    public virtual T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages) where T : class, IKeyed =>
        delegateHandler.SelectVersion<T>(metaPointer, languages);

    /// <inheritdoc />
    public virtual Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IReadableNode node)
        where TFeature : class, Feature => delegateHandler.UnknownFeature<TFeature>(feature, classifier, node);

    /// <inheritdoc />
    public virtual Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IReadableNode node)
        where TFeature : class, Feature => delegateHandler.InvalidFeature<TFeature>(feature, classifier, node);

    /// <inheritdoc />
    public virtual List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IReadableNode node)
        where T : class, IReadableNode => delegateHandler.InvalidLinkValue(value, link, node);

    /// <inheritdoc />
    public virtual IWritableNode? InvalidAnnotation(IReadableNode annotation, IReadableNode? node) =>
        delegateHandler.InvalidAnnotation(annotation, node);

    /// <inheritdoc />
    public virtual IWritableNode? CircularContainment(IReadableNode containedNode, IReadableNode parent) =>
        delegateHandler.CircularContainment(containedNode, parent);

    /// <inheritdoc />
    public virtual bool DuplicateContainment(IReadableNode containedNode, IReadableNode newParent, IReadableNode existingParent)
        => delegateHandler.DuplicateContainment(containedNode, newParent, existingParent);

    /// <inheritdoc />
    public virtual Enum? UnknownEnumerationLiteral(MetaPointerKey key, Enumeration enumeration, Feature property, IReadableNode node) =>
        delegateHandler.UnknownEnumerationLiteral(key, enumeration, property, node);

    /// <inheritdoc />
    public virtual Field?
        UnknownField(MetaPointerKey key, StructuredDataType structuredDataType, Feature property, IReadableNode node) =>
        delegateHandler.UnknownField(key, structuredDataType, property, node);

    /// <inheritdoc />
    public virtual object? UnknownDatatype(PropertyValue? value, LanguageEntity datatype, Feature property, IReadableNode node) =>
        delegateHandler.UnknownDatatype(value, datatype, property, node);

    /// <inheritdoc />
    public virtual object? InvalidPropertyValue<TValue>(PropertyValue? value, Feature property, ICompressedId nodeId) =>
        delegateHandler.InvalidPropertyValue<TValue>(value, property, nodeId);

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableChild(ICompressedId childId, Feature containment, IReadableNode node) =>
        delegateHandler.UnresolvableChild(childId, containment, node);

    /// <inheritdoc />
    public virtual IReadableNode? UnresolvableReferenceTarget(ICompressedId? targetId, ResolveInfo? resolveInfo, Feature reference,
        IReadableNode node) => delegateHandler.UnresolvableReferenceTarget(targetId, resolveInfo, reference, node);

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableAnnotation(ICompressedId annotationId, IReadableNode node) =>
        delegateHandler.UnresolvableAnnotation(annotationId, node);

    /// <inheritdoc />
    public virtual bool SkipDeserializingDependentNode(ICompressedId id) =>
        delegateHandler.SkipDeserializingDependentNode(id);

    /// <inheritdoc />
    public virtual void InvalidReference(IReadableNode node) =>
        delegateHandler.InvalidReference(node);
}