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

/// Logs and ignores any kind of callback.
public class DeserializerIgnoringHandler : IDeserializerHandler
{
    /// <inheritdoc />
    public virtual Classifier? UnknownClassifier(CompressedMetaPointer classifier, ICompressedId id)
    {
        LogMessage($"On node with id={id}: couldn't find specified classifier {classifier} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public NodeId? DuplicateNodeId(ICompressedId nodeId, IReadableNode existingNode, IReadableNode node)
    {
        LogMessage($"Duplicate node with id={existingNode.GetId()}");
        return null;
    }

    /// <inheritdoc />
    public virtual T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages)
        where T : class, IKeyed
    {
        LogMessage($"Unknown meta-pointer {metaPointer}");
        return null;
    }

    #region features

    /// <inheritdoc />
    public virtual Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IReadableNode node) where TFeature : class, Feature
    {
        LogMessage(
            $"On node with id={node.GetId()}: couldn't find specified feature {feature} - leaving this feature unset.");
        return null;
    }

    /// <inheritdoc />
    public List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IReadableNode node) where T : class, IReadableNode
    {
        LogMessage($"On node with id={node.GetId()}: invalid link value {value} for link {link} - skipping");
        return null;
    }

    /// <inheritdoc />
    public Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature,
        Classifier classifier,
        IReadableNode node) where TFeature : class, Feature
    {
        LogMessage($"On node with id={node.GetId()}: wrong type of feature {feature} - leaving this feature unset.");
        return null;
    }

    /// <inheritdoc />
    public IWritableNode? InvalidAnnotation(IReadableNode annotation, IReadableNode? node)
    {
        LogMessage($"On node with id={node?.GetId()}: unsuitable annotation {annotation} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public IWritableNode? CircularContainment(IReadableNode containedNode, IReadableNode parent)
    {
        LogMessage(
            $"On node with id={parent.GetId()}: adding {containedNode.GetId()} as child/annotation would result in circular containment - skipping");
        return null;
    }

    /// <inheritdoc />
    public bool DuplicateContainment(IReadableNode containedNode, IReadableNode newParent,
        IReadableNode existingParent)
    {
        LogMessage(
            $"On node with id={containedNode.GetId()}: already has parent {existingParent.GetId()}, but also child/annotation of {newParent.GetId()} - keeping it in place.");
        return false;
    }

    #endregion

    #region properties

    /// <inheritdoc />
    public Enum? UnknownEnumerationLiteral(MetaPointerKey key, Enumeration enumeration, Feature property, IReadableNode node)
    {
        LogMessage(
            $"On node with id={node.GetId()}: unknown enumeration literal for enumeration {enumeration} with key {key} - skipping");
        return null;
    }

    /// <inheritdoc />
    public Field? UnknownField(MetaPointerKey key, StructuredDataType structuredDataType, Feature property, IReadableNode node)
    {
        LogMessage(
            $"On node with id={node.GetId()}: unknown field for structured datatype {structuredDataType} with key {key} = skipping");
        return null;
    }

    /// <inheritdoc />
    public object? UnknownDatatype(PropertyValue? value, LanguageEntity datatype, Feature property, IReadableNode node)
    {
        LogMessage(
            $"On node with id={node.GetId()}: unknown datatype {property /*.Type*/} with value {value} - skipping");
        return null;
    }

    /// <inheritdoc />
    public object? InvalidPropertyValue<TValue>(PropertyValue? value, Feature property, ICompressedId nodeId)
    {
        LogMessage($"On node with id={nodeId}: invalid property value {value} for property {property} - skipping");
        return null;
    }

    #endregion

    #region unresolveable nodes

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableChild(ICompressedId childId, Feature containment, IReadableNode node)
    {
        LogMessage($"On node with id={node.GetId()}: couldn't find child with id={childId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual ReferenceDescriptor? UnresolvableReferenceTarget(ICompressedId? targetId,
        ResolveInfo? resolveInfo,
        Feature reference,
        IReadableNode node)
    {
        LogMessage($"On node with id={node.GetId()}: couldn't find reference target with id={targetId} - skipping.");
        return null;
    }

    /// <inheritdoc />
    public virtual IWritableNode? UnresolvableAnnotation(ICompressedId annotationId, IReadableNode node)
    {
        LogMessage($"On node with id={node.GetId()}: couldn't find annotation with id={annotationId} - skipping.");
        return null;
    }

    #endregion

    /// <inheritdoc />
    public void InvalidReference(IReadableNode node) =>
        LogMessage($"installing references in node of meta-concept {node.GetType().Name} not implemented");


    /// <inheritdoc />
    public bool SkipDeserializingDependentNode(ICompressedId id)
    {
        LogMessage($"Skip deserializing {id} because dependent nodes contains node with same id");
        return true;
    }

    /// Writes <paramref name="message"/> to the appropriate logging facility.
    protected virtual void LogMessage(string message) =>
        Console.WriteLine(message);
}