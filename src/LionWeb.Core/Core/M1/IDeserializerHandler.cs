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

/// <summary>
/// Callbacks to customize a <see cref="IDeserializer"/>'s behaviour in non-regular situations.
///
/// <para>
/// Each method of this interface is one callback. It should provide all relevant information as parameters.
/// If the method returns non-null, the returned value is used to <i>heal</i> the issue; otherwise, the offender is skipped (if possible).
/// </para>
/// </summary>
public interface IDeserializerHandler
{
    /// <summary>
    /// Cannot find classifier according to <paramref name="classifier"/> for node id <paramref name="id"/>.
    /// </summary>
    /// <param name="classifier">Unknown classifier.</param>
    /// <param name="id">Node id the unknown classifier appeared.</param>
    /// <returns>Replacement classifier to use, or <c>null</c> to skip node <paramref name="id"/>.</returns>
    Classifier? UnknownClassifier(CompressedMetaPointer classifier, ICompressedId id);

    /// <summary>
    /// <paramref name="nodeId"/> is same for <paramref name="existingNode"/> and <paramref name="node"/>. 
    /// </summary>
    /// <param name="nodeId">Duplicate node id.</param>
    /// <param name="existingNode">Previously deserialized node with id <paramref name="nodeId"/>.</param>
    /// <param name="node">Currently deserialized node with id <paramref name="nodeId"/>.</param>
    /// <returns>Replacement node id to use for <paramref name="node"/>, or <c>null</c> to skip <paramref name="node"/>.</returns>
    /// <remarks>For both <paramref name="existingNode"/> and <paramref name="node"/>, only node id and properties are populated -- no other features.</remarks>
    /// <remarks>If returned replacement node id is not unique, deserializer keeps calling this method, might lead to an infinite loop.</remarks>
    NodeId? DuplicateNodeId(ICompressedId nodeId, IReadableNode existingNode, IReadableNode node);

    /// <summary>
    /// Cannot resolve <paramref name="metaPointer"/>, but know about at least one language
    /// with <see cref="IKeyed.Key"/> <paramref name="metaPointer"/>.<see cref="CompressedMetaPointer.Language"/>.
    /// </summary>
    /// <param name="metaPointer">Unresolvable meta-pointer.</param>
    /// <param name="languages">Languages with same key as <paramref name="metaPointer"/>.</param>
    /// <typeparam name="T">Kind of language element we're looking for.</typeparam>
    /// <returns>Resolved <paramref name="metaPointer"/>, typically from one of <paramref name="languages"/>.</returns>
    T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages) where T : class, IKeyed;

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
    Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IReadableNode node)
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
    Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier, IReadableNode node)
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
    List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IReadableNode node) where T : class, IReadableNode;

    /// <summary>
    /// <paramref name="annotation"/> is not a valid annotation for <paramref name="node"/>.
    /// This means <paramref name="annotation"/>'s classifier is not an <see cref="Annotation"/>,
    /// or <paramref name="node"/> is not compatible with the classifier's <see cref="Annotation.Annotates"/>.
    /// </summary>
    /// <param name="annotation">Invalid annotation used for <paramref name="node"/>.</param>
    /// <param name="node">Node that wants to have <paramref name="annotation"/> as annotation.</param>
    /// <returns>Replacement annotation node to use, or <c>null</c> to skip annotation <paramref name="annotation"/>.</returns>
    IWritableNode? InvalidAnnotation(IReadableNode annotation, IReadableNode? node);

    /// <summary>
    /// Adding <paramref name="containedNode"/> as containment or annotation to <paramref name="parent"/> would create a containment cycle.
    /// </summary>
    /// <param name="containedNode">Node that's already part of <paramref name="parent"/>'s ancestors, or is equal to <paramref name="parent"/>.</param>
    /// <param name="parent">Node that already has <paramref name="containedNode"/> as ancestor, or is equal to <paramref name="containedNode"/>.</param>
    /// <returns>Replacement to use, or <c>null</c> to skip <paramref name="containedNode"/> as containment / annotation.</returns>
    IWritableNode? CircularContainment(IReadableNode containedNode, IReadableNode parent);

    /// <summary>
    /// <paramref name="containedNode"/> already has parent <paramref name="existingParent"/>, but is about to be added to <paramref name="newParent"/>.
    /// </summary>
    /// <param name="containedNode">Node that already has parent <paramref name="existingParent"/>, but is about to be added to <paramref name="newParent"/>.</param>
    /// <param name="newParent">Newly requested parent of <paramref name="containedNode"/>.</param>
    /// <param name="existingParent">Already existing parent of <paramref name="containedNode"/>.</param>
    /// <returns><c>true</c> if <paramref name="containedNode"/> should be moved to <paramref name="newParent"/>,
    /// <c>false</c> if <paramref name="containedNode"/> should stay at <paramref name="existingParent"/>.</returns>
    bool DuplicateContainment(IReadableNode containedNode, IReadableNode newParent, IReadableNode existingParent);

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
    Enum? UnknownEnumerationLiteral(MetaPointerKey key, Enumeration enumeration, Feature property, IReadableNode node);

    /// <summary>
    /// Cannot find field with <see cref="IKeyed.Key"/> <paramref name="key"/> in <paramref name="structuredDataType"/>.
    /// </summary>
    /// <param name="key">Unknown field key.</param>
    /// <param name="structuredDataType">LionWeb structured datatype that should contain a field with key <paramref name="key"/>.</param>
    /// <param name="property">Property in <paramref name="node"/> that contains <paramref name="key"/>.</param>
    /// <param name="node">Node that has <paramref name="property"/> with value <paramref name="key"/>.</param>
    /// <returns>Replacement field to use, or <c>null</c> to skip field <paramref name="key"/>.</returns>
    Field? UnknownField(MetaPointerKey key, StructuredDataType structuredDataType, Feature property, IReadableNode node);

    /// <summary>
    /// Cannot process <paramref name="datatype"/> in <paramref name="property"/>.
    /// </summary>
    /// <param name="value">Value of <paramref name="property"/> in <paramref name="node"/>.</param>
    /// <param name="datatype">Unknown Datatype.</param>
    /// <param name="property">Property with unknown Datatype.</param>
    /// <param name="node">Node that has property <paramref name="property"/>.</param>
    /// <returns>Replacement value to use, or <c>null</c> to skip property <paramref name="property"/>.</returns>
    object? UnknownDatatype(PropertyValue? value, LanguageEntity datatype, Feature property, IReadableNode node);

    /// <summary>
    /// Cannot put <paramref name="value"/> into <paramref name="property"/>.
    /// Most probably, that's because <paramref name="value"/> is not compatible with <paramref name="property"/>'s <see cref="Property.Type"/>.
    /// </summary>
    /// <param name="value">Invalid value for <paramref name="property"/>.</param>
    /// <param name="property">Property with invalid <paramref name="value"/>.</param>
    /// <param name="nodeId">Node that has property <paramref name="property"/>.</param>
    /// <typeparam name="TValue">Type of value to be used for property <paramref name="property"/> in node <paramref name="nodeId"/>.</typeparam>
    /// <returns>Replacement value to use, or <c>null</c> to skip property <paramref name="property"/>.</returns>
    object? InvalidPropertyValue<TValue>(PropertyValue? value, Feature property, ICompressedId nodeId);

    #endregion

    #region unresolveable nodes

    /// <summary>
    /// Cannot find node with id <paramref name="childId"/> mentioned as child of <paramref name="node"/> in containment <paramref name="containment"/>. 
    /// </summary>
    /// <param name="childId">Unresolvable child node id.</param>
    /// <param name="containment">Containment that should contain <paramref name="childId"/>.</param>
    /// <param name="node">Node that mentions <paramref name="childId"/> as child.</param>
    /// <returns>Replacement child node to use, or <c>null</c> to skip child <paramref name="childId"/>.</returns>
    IWritableNode? UnresolvableChild(ICompressedId childId, Feature containment, IReadableNode node);

    /// <summary>
    /// Cannot find node with id <paramref name="targetId"/> mentioned as reference target in <paramref name="node"/> in reference <paramref name="reference"/>.
    /// </summary>
    /// <param name="targetId">Unresolvable target node id.</param>
    /// <param name="resolveInfo">ResolveInfo of <paramref name="targetId"/>.</param>
    /// <param name="reference">Reference that should contain <paramref name="targetId"/>.</param>
    /// <param name="node">Node that mentions <paramref name="targetId"/> as reference target.</param>
    /// <returns>Replacement reference target node to use, or <c>null</c> to skip reference target <paramref name="targetId"/>.</returns>
    IReferenceDescriptor? UnresolvableReferenceTarget(ICompressedId? targetId, ResolveInfo? resolveInfo, Feature reference,
        IReadableNode node);

    /// <summary>
    /// Cannot find node with id <paramref name="annotationId"/> mentioned as annotation on <paramref name="node"/>.
    /// </summary>
    /// <param name="annotationId">Unresolvable annotation node id.</param>
    /// <param name="node">Node that mentions <paramref name="annotationId"/> as annotation.</param>
    /// <returns>Replacement annotation node to use, or <c>null</c> to skip annotation node <paramref name="annotationId"/>.</returns>
    IWritableNode? UnresolvableAnnotation(ICompressedId annotationId, IReadableNode node);

    #endregion

    /// <summary>
    /// Whether to skip node with id <paramref name="id"/> that appears both in deserialized nodes and <see cref="IDeserializer.RegisterDependentNodes">dependent nodes</see>.
    /// </summary>
    /// <param name="id">Node id appearing in both deserialized nodes and dependent nodes.</param>
    /// <returns><c>true</c> if we should skip the deserialized node if the same node id appears in dependent nodes;
    /// <c>false</c> if we should still deserialize the node.</returns>
    bool SkipDeserializingDependentNode(ICompressedId id);

    /// <summary>
    /// Cannot install references into <paramref name="node"/>. 
    /// </summary>
    /// <param name="node">Node that cannot receive new references.</param>
    void InvalidReference(IReadableNode node);
}