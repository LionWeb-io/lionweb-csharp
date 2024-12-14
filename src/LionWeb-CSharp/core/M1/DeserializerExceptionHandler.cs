// Copyright 2024 TRUMPF Laser SE and other contributors
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
using Serialization;

/// Throws some variant of <see cref="LionWebExceptionBase"/> for any callback.
public class DeserializerExceptionHandler : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id) =>
        throw new UnsupportedClassifierException(classifier, $"On node with id={id}: ");

    /// <inheritdoc />
    public virtual string? DuplicateNodeId(CompressedId nodeId, IReadableNode existingNode, IReadableNode node) =>
        throw new DeserializerException($"Duplicate node with id={existingNode.GetId()}");

    /// <inheritdoc />
    public virtual T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages)
        where T : class, IKeyed =>
        // TODO think about correct handling
        // throw new DeserializerException($"Unknown meta-pointer {metaPointer}");
        null;

    #region features

    /// <inheritdoc />
    public virtual Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IReadableNode node) where TFeature : class, Feature =>
        throw new UnknownFeatureException(classifier, feature, $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IReadableNode node) where TFeature : class, Feature =>
        throw new UnknownFeatureException(classifier, feature, $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IReadableNode node)
        where T : class, IReadableNode =>
        throw new InvalidValueException(link, value);

    /// <inheritdoc />
    public virtual IWritableNode? InvalidAnnotation(IReadableNode annotation, IReadableNode? node) =>
        throw new DeserializerException(
            $"On node with id={node?.GetId()}: unsuitable annotation {annotation}");

    /// <inheritdoc />
    public virtual IWritableNode? CircularContainment(IReadableNode containedNode, IReadableNode parent) =>
        throw new DeserializerException(
            $"On node with id={parent.GetId()}: adding {containedNode.GetId()} as child/annotation would result in circular containment.");

    /// <inheritdoc />
    public virtual bool DuplicateContainment(IReadableNode containedNode, IReadableNode newParent,
        IReadableNode existingParent) =>
        throw new DeserializerException(
            $"On node with id={containedNode.GetId()}: already has parent {existingParent.GetId()}, but also child/annotation of {newParent.GetId()}.");

    #endregion

    #region properties

    /// <inheritdoc />
    public virtual Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration,
        Feature property, IReadableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: unknown enumeration literal for enumeration {enumeration} with key {key}");

    /// <inheritdoc />
    public virtual object? UnknownDatatype(Feature property, string? value, IReadableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: unknown property type {property /*.Type*/} with value {value}");

    /// <inheritdoc />
    public virtual object? InvalidPropertyValue<TValue>(string? value, Feature property, CompressedId nodeId) =>
        throw new DeserializerException(
            $"On node with id={nodeId}: invalid property value '{value}' for property {property}");

    #endregion

    #region unresolveable nodes

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableChild(CompressedId childId, Feature containment, IReadableNode node) =>
        throw new DeserializerException($"On node with id={node.GetId()}: couldn't find child with id={childId}");

    /// <inheritdoc />
    public virtual IReadableNode? UnresolvableReferenceTarget(CompressedId? targetId,
        string? resolveInfo,
        Feature reference,
        IReadableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find reference target with id={targetId}");

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableAnnotation(CompressedId annotationId, IReadableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find annotation with id={annotationId}");

    #endregion

    #region language deserializer

    /// <inheritdoc />
    public virtual void InvalidContainment(IReadableNode node) =>
        throw new UnsupportedClassifierException(node.GetClassifier().ToMetaPointer(),
            $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual void InvalidReference(IReadableNode node) =>
        throw new UnsupportedClassifierException(node.GetClassifier().ToMetaPointer(),
            $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual void InvalidAnnotationParent(IReadableNode annotation, IReadableNode? parent) =>
        throw new DeserializerException(
            $"Cannot attach annotation {annotation} to its parent with id={parent?.GetId()}.");

    /// <inheritdoc />
    public virtual bool SkipDeserializingDependentNode(CompressedId id) =>
        throw new DeserializerException(
            $"Skip deserializing {id} because dependentLanguages contains node with same id");

    #endregion
}

/// Something went wrong during deserialization.
public class DeserializerException(string? message) : LionWebExceptionBase(message);