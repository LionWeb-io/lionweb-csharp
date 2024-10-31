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

public interface IDeserializerHandler
{
    /// <summary>
    /// Cannot find classifier according to <paramref name="classifier"/> for node id <paramref name="id"/>.
    /// </summary>
    /// <param name="classifier">Unknown classifier.</param>
    /// <param name="id">Node id the unknown classifier appeared.</param>
    /// <returns>Replacement classifier to use, or <c>null</c> to skip node <paramref name="id"/>.</returns>
    Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id);

    /// <summary>
    /// <paramref name="nodeId"/> is same for <paramref name="existingNode"/> and <paramref name="node"/>. 
    /// </summary>
    /// <param name="nodeId">Duplicate node id.</param>
    /// <param name="existingNode">Previously deserialized node with id <paramref name="nodeId"/>.</param>
    /// <param name="node">Currently deserialized node with id <paramref name="nodeId"/>.</param>
    /// <returns>Replacement node id to use for <paramref name="node"/>, or <c>null</c> to skip <paramref name="node"/>.</returns>
    /// <remarks>For both <paramref name="existingNode"/> and <paramref name="node"/>, only node id and properties are populated -- no other features.</remarks>
    string? DuplicateNodeId(CompressedId nodeId, IWritableNode existingNode, INode node);

    #region features

    /// <summary>
    /// Cannot find feature according to <paramref name="feature"/>.
    /// </summary>
    /// <param name="classifier">Classifier of <paramref name="node"/> that should contain feature <paramref name="feature"/>.</param>
    /// <param name="feature">Unknown feature.</param>
    /// <param name="node">Partially deserialized node that contains feature <paramref name="feature"/>.</param>
    /// <typeparam name="TFeature">Kind of feature requested.</typeparam>
    /// <returns>Replacement feature to use, or <c>null</c> to skip feature <paramref name="feature"/>.</returns>
    /// <remarks>Return type <see cref="Feature"/> instead of <typeparamref name="TFeature"/>
    /// to support feature kind vs. value mismatch (e.g. string in containment).</remarks>
    Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IWritableNode node)
        where TFeature : class, Feature;

    /// <summary>
    /// Feature found for <paramref name="feature"/> is not a <typeparamref name="TFeature"/>.
    /// </summary>
    /// <param name="feature">Invalid feature not conforming to <typeparamref name="TFeature"/>.</param>
    /// <param name="classifier">Classifier that should contain <paramref name="feature"/>.</param>
    /// <param name="node">Node that contains <paramref name="feature"/>.</param>
    /// <typeparam name="TFeature">Kind of feature requested.</typeparam>
    /// <returns>Replacement feature to use, or <c>null</c> to skip feature <paramref name="feature"/>.</returns>
    /// <remarks>Return type <see cref="Feature"/> instead of <typeparamref name="TFeature"/>
    /// to support feature kind vs. value mismatch (e.g. string in containment).</remarks>
    Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IWritableNode node)
        where TFeature : class, Feature;

    /// <summary>
    /// Cannot put <paramref name="value"/> into <paramref name="link"/>.
    /// Most probably, that's because <paramref name="value"/> is not compatible with <paramref name="link"/>'s
    /// <see cref="Link.Type"/>, <see cref="Link.Multiple"/>, and/or <see cref="Feature.Optional"/>.
    /// </summary>
    /// <param name="value">Invalid value for <paramref name="link"/>.</param>
    /// <param name="link">Link with invalid <paramref name="value"/>.</param>
    /// <param name="node">Node that has link <paramref name="link"/>.</param>
    /// <typeparam name="T">Only needed to allow any kind of <see cref="IReadableNode"/> in <paramref name="value"/> or returned list.</typeparam>
    /// <returns>Replacement value to use, or <c>null</c> to skip link <paramref name="link"/>.</returns>
    List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IWritableNode node) where T : class, IReadableNode;

    /// <summary>
    /// <paramref name="annotation"/> is not a valid annotation for <paramref name="node"/>.
    /// This means <paramref name="annotation"/>'s classifier is not an <see cref="Annotation"/>,
    /// or <paramref name="node"/> is not compatible with the classifier's <see cref="Annotation.Annotates"/>.
    /// </summary>
    /// <param name="annotation">Invalid annotation used for <paramref name="node"/>.</param>
    /// <param name="node">Node that wants to have <paramref name="annotation"/> as annotation.</param>
    /// <returns>Replacement annotation node to use, or <c>null</c> to skip annotation <paramref name="annotation"/>.</returns>
    IWritableNode? InvalidAnnotation(IReadableNode annotation, IWritableNode node);

    /// <summary>
    /// Adding <paramref name="containedNode"/> as containment or annotation to <paramref name="parent"/> would create a containment cycle.
    /// </summary>
    /// <param name="containedNode">Node that's already part of <paramref name="parent"/>'s ancestors, or is equal to <paramref name="parent"/>.</param>
    /// <param name="parent">Node that already has <paramref name="containedNode"/> as ancestor, or is equal to <paramref name="containedNode"/>.</param>
    /// <returns>Replacement to use, or <c>null</c> to skip <paramref name="containedNode"/> as containment / annotation.</returns>
    IWritableNode? CircularContainment(IWritableNode containedNode, IWritableNode parent);

    /// <summary>
    /// <paramref name="containedNode"/> already has parent <paramref name="existingParent"/>, but is about to be added to <paramref name="newParent"/>.
    /// </summary>
    /// <param name="containedNode">Node that already has parent <paramref name="existingParent"/>, but is about to be added to <paramref name="newParent"/>.</param>
    /// <param name="newParent">Already existing parent of <paramref name="containedNode"/>.</param>
    /// <param name="existingParent">Newly requested parent of <paramref name="containedNode"/>.</param>
    /// <returns><c>true</c> if <paramref name="containedNode"/> should be moved to <paramref name="newParent"/>,
    /// <c>false</c> if <paramref name="containedNode"/> should stay at <paramref name="existingParent"/>.</returns>
    bool DuplicateContainment(IWritableNode containedNode, IWritableNode newParent, IReadableNode existingParent);

    #endregion

    #region properties

    /// <summary>
    /// Cannot find enumeration literal with <see cref="IKeyed.Key"/> <paramref name="key"/> in <paramref name="enumeration"/>.
    /// </summary>
    /// <param name="key">Unknown enumeration literal key.</param>
    /// <param name="enumeration">LionWeb enumeration that should contain a literal with key <paramref name="key"/>.</param>
    /// <param name="property">Property in <paramref name="node"/> that contains <paramref name="key"/>.</param>
    /// <param name="node">Node that has <paramref name="property"/> with value <paramref name="key"/>.</param>
    /// <returns>Replacement C# enumeration literal to use, or <c>null</c> to skip property <paramref name="property"/>.</returns>
    Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration, Feature property, IWritableNode node);

    /// <summary>
    /// Cannot process <see cref="Datatype"/> in <paramref name="property"/>.
    /// </summary>
    /// <param name="property">Property with unknown Datatype.</param>
    /// <param name="value">Value of <paramref name="property"/> in <paramref name="node"/>.</param>
    /// <param name="node">Node that has property <paramref name="property"/>.</param>
    /// <returns>Replacement value to use, or <c>null</c> to skip property <paramref name="property"/>.</returns>
    object? UnknownDatatype(Feature property, string? value, IWritableNode node);

    /// <summary>
    /// Cannot put <paramref name="value"/> into <paramref name="property"/>.
    /// Most probably, that's because <paramref name="value"/> is not compatible with <paramref name="property"/>'s <see cref="Property.Type"/>.
    /// </summary>
    /// <param name="value">Invalid value for <paramref name="property"/>.</param>
    /// <param name="property">Property with invalid <paramref name="value"/>.</param>
    /// <param name="nodeId">Node that has property <paramref name="property"/>.</param>
    /// <typeparam name="TValue">Type of value to be used for property <paramref name="property"/> in node <paramref name="nodeId"/>.</typeparam>
    /// <returns>Replacement value to use, or <c>null</c> to skip property <paramref name="property"/>.</returns>
    object? InvalidPropertyValue<TValue>(string? value, Feature property, CompressedId nodeId);

    #endregion

    #region unresolveable nodes

    /// <summary>
    /// Cannot find node with id <paramref name="parentId"/>, mentioned as parent for <paramref name="node"/>.
    /// </summary>
    /// <param name="parentId">Unresolvable parent node id.</param>
    /// <param name="node">Node that mentions <paramref name="parentId"/> as parent.</param>
    /// <returns>Replacement parent node to use, or <c>null</c> to make <paramref name="node"/> a root node.</returns>
    IWritableNode? UnresolvableParent(CompressedId parentId, IWritableNode node);

    /// <summary>
    /// Cannot find node with id <paramref name="childId"/> mentioned as child of <paramref name="node"/> in containment <paramref name="containment"/>. 
    /// </summary>
    /// <param name="childId">Unresolvable child node id.</param>
    /// <param name="containment">Containment that should contain <paramref name="childId"/>.</param>
    /// <param name="node">Node that mentions <paramref name="childId"/> as child.</param>
    /// <returns>Replacement child node to use, or <c>null</c> to skip child <paramref name="childId"/>.</returns>
    IWritableNode? UnresolvableChild(CompressedId childId, Feature containment, IWritableNode node);

    /// <summary>
    /// Cannot find node with id <paramref name="targetId"/> mentioned as reference target in <paramref name="node"/> in reference <paramref name="reference"/>.
    /// </summary>
    /// <param name="targetId">Unresolvable target node id.</param>
    /// <param name="resolveInfo">ResolveInfo of <paramref name="targetId"/>.</param>
    /// <param name="reference">Reference that should contain <paramref name="targetId"/>.</param>
    /// <param name="node">Node that mentions <paramref name="targetId"/> as reference target.</param>
    /// <returns>Replacement reference target node to use, or <c>null</c> to skip reference target <paramref name="targetId"/>.</returns>
    IReadableNode? UnresolvableReferenceTarget(CompressedId? targetId, string? resolveInfo, Feature reference,
        IWritableNode node);

    /// <summary>
    /// Cannot find node with id <paramref name="annotationId"/> mentioned as annotation on <paramref name="node"/>.
    /// </summary>
    /// <param name="annotationId">Unresolvable annotation node id.</param>
    /// <param name="node">Node that mentions <paramref name="annotationId"/> as annotation.</param>
    /// <returns>Replacement annotation node to use, or <c>null</c> to skip annotation node <paramref name="annotationId"/>.</returns>
    IWritableNode? UnresolvableAnnotation(CompressedId annotationId, IWritableNode node);

    #endregion

    #region language deserializer

    /// <summary>
    /// Whether to skip node with id <paramref name="id"/> that appears both in deserialized nodes and dependent nodes.
    /// </summary>
    /// <param name="id">Node id appearing in both deserialized nodes and dependent nodes.</param>
    /// <returns><c>true</c> if we should skip the deserialized node if the same node id appears in dependent nodes;
    /// <c>false</c> if we should still deserialize the node.</returns>
    bool SkipDeserializingDependentNode(CompressedId id);

    /// <summary>
    /// Cannot install containments into <paramref name="node"/>. 
    /// </summary>
    /// <param name="node">Node that cannot receive new containments.</param>
    void InvalidContainment(IReadableNode node);

    /// <summary>
    /// Cannot install references into <paramref name="node"/>. 
    /// </summary>
    /// <param name="node">Node that cannot receive new references.</param>
    void InvalidReference(IReadableNode node);

    /// <summary>
    /// Cannot install annotations into <paramref name="parent"/>.
    /// </summary>
    /// <param name="annotation">Annotation we want to add to <paramref name="parent"/>.</param>
    /// <param name="parent">Node that cannot receive new annotations.</param>
    void InvalidAnnotationParent(IWritableNode annotation, IReadableNode? parent);

    #endregion
}

public class DeserializerExceptionHandler : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id) =>
        throw new UnsupportedClassifierException(classifier, $"On node with id={id}: ");

    /// <inheritdoc />
    public string? DuplicateNodeId(CompressedId nodeId, IWritableNode existingNode, INode node) =>
        throw new DeserializerException($"Duplicate node with id={existingNode.GetId()}");

    /// <inheritdoc />
    public virtual Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IWritableNode node) where TFeature : class, Feature =>
        throw new UnknownFeatureException(classifier, feature, $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IWritableNode node) where TFeature : class, Feature =>
        throw new UnknownFeatureException(classifier, feature, $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IWritableNode node)
        where T : class, IReadableNode =>
        throw new InvalidValueException(link, value);

    /// <inheritdoc />
    public virtual void InvalidContainment(IReadableNode node) =>
        throw new UnsupportedClassifierException(node.GetClassifier().ToMetaPointer(),
            $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual void InvalidReference(IReadableNode node) =>
        throw new UnsupportedClassifierException(node.GetClassifier().ToMetaPointer(),
            $"On node with id={node.GetId()}:");

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableParent(CompressedId parentId, IWritableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find specified parent with id={parentId}");

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableChild(CompressedId childId, Feature containment, IWritableNode node) =>
        throw new DeserializerException($"On node with id={node.GetId()}: couldn't find child with id={childId}");

    /// <inheritdoc />
    public virtual IReadableNode? UnresolvableReferenceTarget(CompressedId? targetId,
        string? resolveInfo,
        Feature reference,
        IWritableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find reference target with id={targetId}");

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableAnnotation(CompressedId annotationId, IWritableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: couldn't find annotation with id={annotationId}");

    /// <inheritdoc />
    public virtual IWritableNode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: unsuitable annotation {annotation}");

    /// <inheritdoc />
    public virtual IWritableNode? CircularContainment(IWritableNode containedNode, IWritableNode parent) =>
        throw new DeserializerException(
            $"On node with id={parent.GetId()}: adding {containedNode.GetId()} as child/annotation would result in circular containment.");

    /// <inheritdoc />
    public bool DuplicateContainment(IWritableNode containedNode, IWritableNode newParent,
        IReadableNode existingParent) =>
        throw new DeserializerException(
            $"On node with id={containedNode.GetId()}: already has parent {existingParent.GetId()}, but also child/annotation of {newParent.GetId()}.");

    /// <inheritdoc />
    public virtual void InvalidAnnotationParent(IWritableNode annotation, IReadableNode? parent) =>
        throw new DeserializerException(
            $"Cannot attach annotation {annotation} to its parent with id={parent?.GetId()}.");

    /// <inheritdoc />
    public virtual Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration,
        Feature property, IWritableNode nodeId) =>
        throw new DeserializerException(
            $"On node with id={nodeId}: unknown enumeration literal for enumeration {enumeration} with key {key}");

    /// <inheritdoc />
    public virtual object? UnknownDatatype(Feature property, string? value, IWritableNode node) =>
        throw new DeserializerException(
            $"On node with id={node.GetId()}: unknown property type {property /*.Type*/} with value {value}");

    /// <inheritdoc />
    public virtual object? InvalidPropertyValue<TValue>(string? value, Feature property, CompressedId nodeId) =>
        throw new DeserializerException(
            $"On node with id={nodeId}: invalid property value {value} for property {property}");

    /// <inheritdoc />
    public virtual bool SkipDeserializingDependentNode(CompressedId id) =>
        throw new DeserializerException(
            $"Skip deserializing {id} because dependentLanguages contains node with same id");
}

internal class DeserializerException(string? message) : LionWebExceptionBase(message);

public class DeserializerIgnoringHandler : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id)
    {
        LogMessage($"On node with id={id}: couldn't find specified classifier {classifier} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public string? DuplicateNodeId(CompressedId nodeId, IWritableNode existingNode, INode node)
    {
        LogMessage($"Duplicate node with id={existingNode.GetId()}");
        return null;
    }

    /// <inheritdoc />
    public virtual Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IWritableNode node) where TFeature : class, Feature
    {
        LogMessage(
            $"On node with id={node.GetId()}: couldn't find specified feature {feature} - leaving this feature unset.");
        return null;
    }

    /// <inheritdoc />
    public List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IWritableNode node) where T : class, IReadableNode
    {
        LogMessage($"On node with id={node.GetId()}: invalid link value {value} for link {link} - skipping");
        return null;
    }

    /// <inheritdoc />
    public void InvalidContainment(IReadableNode node) =>
        LogMessage($"installing containments in node of meta-concept {node.GetType().Name} not implemented");

    /// <inheritdoc />
    public void InvalidReference(IReadableNode node) =>
        LogMessage($"installing references in node of meta-concept {node.GetType().Name} not implemented");

    /// <inheritdoc />
    public Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IWritableNode node) where TFeature : class, Feature
    {
        LogMessage($"On node with id={node.GetId()}: wrong type of feature {feature} - leaving this feature unset.");
        return null;
    }

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableParent(CompressedId parentId, IWritableNode node)
    {
        LogMessage($"On node with id={node.GetId()}: couldn't find specified parent - leaving this node orphaned.");
        return null;
    }

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableChild(CompressedId childId, Feature containment, IWritableNode node)
    {
        LogMessage($"On node with id={node.GetId()}: couldn't find child with id={childId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual IReadableNode? UnresolvableReferenceTarget(CompressedId? targetId,
        string? resolveInfo,
        Feature reference,
        IWritableNode node)
    {
        LogMessage($"On node with id={node.GetId()}: couldn't find reference target with id={targetId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableAnnotation(CompressedId annotationId, IWritableNode node)
    {
        LogMessage($"On node with id={node.GetId()}: couldn't find annotation with id={annotationId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public void InvalidAnnotationParent(IWritableNode annotation, IReadableNode? parent) =>
        LogMessage($"Cannot attach annotation {annotation} to its parent with id={parent?.GetId()}.");

    /// <inheritdoc />
    public IWritableNode? InvalidAnnotation(IReadableNode annotation, IWritableNode node)
    {
        LogMessage($"On node with id={node?.GetId()}: unsuitable annotation {annotation} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public IWritableNode? CircularContainment(IWritableNode containedNode, IWritableNode parent)
    {
        LogMessage(
            $"On node with id={parent.GetId()}: adding {containedNode.GetId()} as child/annotation would result in circular containment - skipping");
        return null;
    }

    /// <inheritdoc />
    public bool DuplicateContainment(IWritableNode containedNode, IWritableNode newParent,
        IReadableNode existingParent)
    {
        LogMessage(
            $"On node with id={containedNode.GetId()}: already has parent {existingParent.GetId()}, but also child/annotation of {newParent.GetId()} - keeping it in place.");
        return false;
    }

    /// <inheritdoc />
    public Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration, Feature property, IWritableNode nodeId)
    {
        LogMessage(
            $"On node with id={nodeId}: unknown enumeration literal for enumeration {enumeration} with key {key} - skipping");
        return null;
    }

    /// <inheritdoc />
    public object? UnknownDatatype(Feature property, string? value, IWritableNode node)
    {
        LogMessage(
            $"On node with id={node.GetId()}: unknown datatype {property /*.Type*/} with value {value} - skipping");
        return null;
    }

    /// <inheritdoc />
    public object? InvalidPropertyValue<TValue>(string? value, Feature property, CompressedId nodeId)
    {
        LogMessage($"On node with id={nodeId}: invalid property value {value} for property {property} - skipping");
        return null;
    }

    /// <inheritdoc />
    public bool SkipDeserializingDependentNode(CompressedId id)
    {
        LogMessage($"Skip deserializing {id} because dependent nodes contains node with same id");
        return true;
    }

    protected virtual void LogMessage(string format) =>
        Console.WriteLine(format);
}