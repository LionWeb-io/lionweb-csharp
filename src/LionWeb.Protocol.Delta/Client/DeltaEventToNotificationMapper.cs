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

namespace LionWeb.Protocol.Delta.Client;

using Core;
using Core.M1;
using Core.M2;
using Core.M3;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Serialization;
using Message;
using Message.Event;
using System.Diagnostics.CodeAnalysis;

public class DeltaEventToNotificationMapper
{
    private readonly SharedNodeMap _sharedNodeMap;
    private readonly SharedKeyedMap _sharedKeyedMap;
    private readonly DeserializerBuilder _deserializerBuilder;

    public DeltaEventToNotificationMapper(
        SharedNodeMap sharedNodeMap,
        SharedKeyedMap sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _deserializerBuilder = deserializerBuilder;
    }

    public INotification Map(IDeltaEvent deltaEvent) =>
        deltaEvent switch
        {
            PartitionAdded a => OnPartitionAdded(a),
            PartitionDeleted a => OnPartitionDeleted(a),
            PropertyAdded a => OnPropertyAdded(a),
            PropertyDeleted a => OnPropertyDeleted(a),
            PropertyChanged a => OnPropertyChanged(a),
            ChildAdded a => OnChildAdded(a),
            ChildDeleted a => OnChildDeleted(a),
            ChildReplaced a => OnChildReplaced(a),
            ChildMovedFromOtherContainment a => OnChildMovedFromOtherContainment(a),
            ChildMovedFromOtherContainmentInSameParent a => OnChildMovedFromOtherContainmentInSameParent(a),
            ChildMovedInSameContainment a => OnChildMovedInSameContainment(a),
            ChildMovedAndReplacedFromOtherContainment a => OnChildMovedAndReplacedFromOtherContainment(a),
            ChildMovedAndReplacedFromOtherContainmentInSameParent a =>
                OnChildMovedAndReplacedFromOtherContainmentInSameParent(a),
            AnnotationAdded a => OnAnnotationAdded(a),
            AnnotationDeleted a => OnAnnotationDeleted(a),
            AnnotationMovedFromOtherParent a => OnAnnotationMovedFromOtherParent(a),
            AnnotationMovedInSameParent a => OnAnnotationMovedInSameParent(a),
            ReferenceAdded a => OnReferenceAdded(a),
            ReferenceDeleted a => OnReferenceDeleted(a),
            ReferenceChanged a => OnReferenceChanged(a),
            _ => throw new ArgumentException($"{nameof(DeltaEventToNotificationMapper)} does not support {deltaEvent.GetType().Name}!")
        };

    #region Partitions

    private PartitionAddedNotification OnPartitionAdded(PartitionAdded partitionAdded) =>
        new(
            (IPartitionInstance)Deserialize(partitionAdded.NewPartition),
            ToNotificationId(partitionAdded)
        );

    private PartitionDeletedNotification OnPartitionDeleted(PartitionDeleted partitionDeleted)
    {
        TryToNode(partitionDeleted.DeletedPartition, out var deletedNode);
        if (deletedNode is null or IPartitionInstance)
            return new PartitionDeletedNotification((IPartitionInstance)deletedNode!, ToNotificationId(partitionDeleted));

        throw new DeltaException(DeltaErrorCode.InvalidNodeType.AsError(partitionDeleted.OriginCommands, partitionDeleted.ProtocolMessages, deletedNode));
    }

    #endregion

    #region Properties

    private PropertyAddedNotification OnPropertyAdded(PropertyAdded propertyAddedEvent)
    {
        var parent = ToNode(propertyAddedEvent.Node);
        var property = ToProperty(propertyAddedEvent.Property, parent);
        return new PropertyAddedNotification(
            parent,
            property,
            ToPropertyValue(parent, property, propertyAddedEvent.NewValue),
            ToNotificationId(propertyAddedEvent)
        );
    }

    private PropertyDeletedNotification OnPropertyDeleted(PropertyDeleted propertyDeletedEvent)
    {
        var parent = ToNode(propertyDeletedEvent.Node);
        var property = ToProperty(propertyDeletedEvent.Property, parent);
        return new PropertyDeletedNotification(
            parent,
            property,
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToNotificationId(propertyDeletedEvent)
        );
    }

    private PropertyChangedNotification OnPropertyChanged(PropertyChanged propertyChangedEvent)
    {
        var parent = ToNode(propertyChangedEvent.Node);
        var property = ToProperty(propertyChangedEvent.Property, parent);
        return new PropertyChangedNotification(
            parent,
            property,
            ToPropertyValue(parent, property, propertyChangedEvent.NewValue),
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToNotificationId(propertyChangedEvent)
        );
    }

    private Property ToProperty(MetaPointer deltaProperty, IReadableNode node) =>
        ToFeature<Property>(deltaProperty, node);

    private SemanticPropertyValue ToPropertyValue(IReadableNode node, Property property, PropertyValue value) =>
        _deserializerBuilder.Build().VersionSpecifics.ConvertDatatype(node, property, property.Type, value) ??
        throw new InvalidValueException(property, value);

    #endregion

    #region Children

    private ChildAddedNotification OnChildAdded(ChildAdded childAddedEvent)
    {
        var parent = ToNode(childAddedEvent.Parent);
        var containment = ToContainment(childAddedEvent.Containment, parent);
        return new ChildAddedNotification(
            parent,
            Deserialize(childAddedEvent.NewChild),
            containment,
            childAddedEvent.Index,
            ToNotificationId(childAddedEvent)
        );
    }

    private ChildDeletedNotification OnChildDeleted(ChildDeleted childDeletedEvent)
    {
        var parent = ToNode(childDeletedEvent.Parent);
        var containment = ToContainment(childDeletedEvent.Containment, parent);
        return new ChildDeletedNotification(
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment), containment).ToList()[childDeletedEvent.Index],
            parent,
            containment,
            childDeletedEvent.Index,
            ToNotificationId(childDeletedEvent)
        );
    }

    private ChildReplacedNotification OnChildReplaced(ChildReplaced childReplacedEvent)
    {
        var parent = ToNode(childReplacedEvent.Parent);
        var containment = ToContainment(childReplacedEvent.Containment, parent);
        return new ChildReplacedNotification(
            Deserialize(childReplacedEvent.NewChild),
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment), containment).ToList()[childReplacedEvent.Index],
            parent,
            containment,
            childReplacedEvent.Index,
            ToNotificationId(childReplacedEvent)
        );
    }

    private ChildMovedFromOtherContainmentNotification OnChildMovedFromOtherContainment(
        ChildMovedFromOtherContainment childMovedEvent)
    {
        var movedChild = ToNode(childMovedEvent.MovedChild);
        var oldParent = ToNode(childMovedEvent.OldParent);
        var newParent = ToNode(childMovedEvent.NewParent);
        var oldContainment = ToContainment(childMovedEvent.OldContainment, oldParent);
        var newContainment = ToContainment(childMovedEvent.NewContainment, newParent);
        return new ChildMovedFromOtherContainmentNotification(
            newParent,
            newContainment,
            childMovedEvent.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            childMovedEvent.OldIndex,
            ToNotificationId(childMovedEvent)
        );
    }

    private ChildMovedAndReplacedFromOtherContainmentNotification OnChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainment childMovedAndReplacedEvent)
    {
        var movedChild = ToNode(childMovedAndReplacedEvent.MovedChild);
        var oldParent = (IWritableNode)movedChild.GetParent();
        var oldContainment = oldParent.GetContainmentOf(movedChild);

        var newParent = ToNode(childMovedAndReplacedEvent.NewParent);
        var newContainment = ToContainment(childMovedAndReplacedEvent.NewContainment, newParent);

        var replacedChild = ToNode(childMovedAndReplacedEvent.ReplacedChild);

        return new ChildMovedAndReplacedFromOtherContainmentNotification(
            newParent,
            newContainment,
            childMovedAndReplacedEvent.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            childMovedAndReplacedEvent.OldIndex,
            replacedChild,
            ToNotificationId(childMovedAndReplacedEvent)
        );
    }

    private ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        OnChildMovedAndReplacedFromOtherContainmentInSameParent(
            ChildMovedAndReplacedFromOtherContainmentInSameParent childMovedAndReplaced)
    {
        var parent = ToNode(childMovedAndReplaced.Parent);
        var movedChild = ToNode(childMovedAndReplaced.MovedChild);
        var oldContainment = ToContainment(childMovedAndReplaced.OldContainment, parent);
        var replacedChild = ToNode(childMovedAndReplaced.ReplacedChild);
        var newContainment = ToContainment(childMovedAndReplaced.NewContainment, parent);

        return new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(
            newContainment,
            childMovedAndReplaced.NewIndex,
            movedChild,
            parent,
            oldContainment,
            childMovedAndReplaced.OldIndex,
            replacedChild,
            ToNotificationId(childMovedAndReplaced)
        );
    }

    private ChildMovedFromOtherContainmentInSameParentNotification OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParent childMovedEvent)
    {
        var parent = ToNode(childMovedEvent.Parent);
        var movedChild = ToNode(childMovedEvent.MovedChild);
        var oldContainment = ToContainment(childMovedEvent.OldContainment, parent);
        var newContainment = ToContainment(childMovedEvent.NewContainment, parent);
        return new ChildMovedFromOtherContainmentInSameParentNotification(
            newContainment,
            childMovedEvent.NewIndex,
            movedChild,
            parent,
            oldContainment,
            childMovedEvent.OldIndex,
            ToNotificationId(childMovedEvent)
        );
    }

    private ChildMovedInSameContainmentNotification OnChildMovedInSameContainment(
        ChildMovedInSameContainment childMovedEvent)
    {
        var parent = ToNode(childMovedEvent.Parent);
        var movedChild = ToNode(childMovedEvent.MovedChild);
        var containment = ToContainment(childMovedEvent.Containment, parent);
        return new ChildMovedInSameContainmentNotification(
            childMovedEvent.NewIndex,
            movedChild,
            parent,
            containment,
            childMovedEvent.OldIndex,
            ToNotificationId(childMovedEvent)
        );
    }

    private Containment ToContainment(MetaPointer deltaContainment, IReadableNode node) =>
        ToFeature<Containment>(deltaContainment, node);

    #endregion

    #region Annotations

    private AnnotationAddedNotification OnAnnotationAdded(AnnotationAdded annotationAddedEvent)
    {
        var parent = ToNode(annotationAddedEvent.Parent);
        return new AnnotationAddedNotification(
            parent,
            Deserialize(annotationAddedEvent.NewAnnotation),
            annotationAddedEvent.Index,
            ToNotificationId(annotationAddedEvent)
        );
    }

    private AnnotationDeletedNotification OnAnnotationDeleted(AnnotationDeleted annotationDeletedEvent)
    {
        var parent = ToNode(annotationDeletedEvent.Parent);
        var deletedAnnotation = parent.GetAnnotations()[annotationDeletedEvent.Index];
        return new AnnotationDeletedNotification(
            deletedAnnotation as IWritableNode ?? throw new InvalidValueException(null, deletedAnnotation),
            parent,
            annotationDeletedEvent.Index,
            ToNotificationId(annotationDeletedEvent)
        );
    }

    private AnnotationMovedFromOtherParentNotification OnAnnotationMovedFromOtherParent(
        AnnotationMovedFromOtherParent annotationMovedEvent)
    {
        var oldParent = ToNode(annotationMovedEvent.OldParent);
        var newParent = ToNode(annotationMovedEvent.NewParent);
        var movedAnnotation = ToNode(annotationMovedEvent.MovedAnnotation);
        return new AnnotationMovedFromOtherParentNotification(
            newParent,
            annotationMovedEvent.NewIndex,
            movedAnnotation,
            oldParent,
            annotationMovedEvent.OldIndex,
            ToNotificationId(annotationMovedEvent)
        );
    }

    private AnnotationMovedInSameParentNotification OnAnnotationMovedInSameParent(
        AnnotationMovedInSameParent annotationMovedEvent)
    {
        var parent = ToNode(annotationMovedEvent.Parent);
        var movedAnnotation = ToNode(annotationMovedEvent.MovedAnnotation);
        return new AnnotationMovedInSameParentNotification(
            annotationMovedEvent.NewIndex,
            movedAnnotation,
            parent,
            annotationMovedEvent.OldIndex,
            ToNotificationId(annotationMovedEvent)
        );
    }

    #endregion

    #region References

    private ReferenceAddedNotification OnReferenceAdded(ReferenceAdded referenceAddedEvent)
    {
        var parent = ToNode(referenceAddedEvent.Parent);
        var reference = ToReference(referenceAddedEvent.Reference, parent);
        return new ReferenceAddedNotification(
            parent,
            reference,
            referenceAddedEvent.Index,
            ToTarget(referenceAddedEvent.NewTarget, referenceAddedEvent.NewResolveInfo),
            ToNotificationId(referenceAddedEvent)
        );
    }

    private ReferenceDeletedNotification OnReferenceDeleted(ReferenceDeleted referenceDeletedEvent)
    {
        var parent = ToNode(referenceDeletedEvent.Parent);
        var reference = ToReference(referenceDeletedEvent.Reference, parent);
        return new ReferenceDeletedNotification(
            parent,
            reference,
            referenceDeletedEvent.Index,
            ToTarget(referenceDeletedEvent.DeletedTarget, referenceDeletedEvent.DeletedResolveInfo),
            ToNotificationId(referenceDeletedEvent)
        );
    }

    private ReferenceChangedNotification OnReferenceChanged(ReferenceChanged referenceChangedEvent)
    {
        var parent = ToNode(referenceChangedEvent.Parent);
        var reference = ToReference(referenceChangedEvent.Reference, parent);
        return new ReferenceChangedNotification(
            parent,
            reference,
            referenceChangedEvent.Index,
            ToTarget(referenceChangedEvent.NewTarget, referenceChangedEvent.NewResolveInfo),
            ToTarget(referenceChangedEvent.OldTarget, referenceChangedEvent.OldResolveInfo),
            ToNotificationId(referenceChangedEvent)
        );
    }

    private Reference ToReference(MetaPointer deltaReference, IReadableNode node) =>
        ToFeature<Reference>(deltaReference, node);

    private IReferenceTarget ToTarget(TargetNode? targetNode, ResolveInfo? resolveInfo)
    {
        IReadableNode? target = null;
        if (targetNode != null &&
            _sharedNodeMap.TryGetValue(targetNode, out var node))
            target = node;

        return new ReferenceTarget(resolveInfo, targetNode, target);
    }

    #endregion

    private static INotificationId ToNotificationId(IDeltaEvent deltaEvent) =>
        new ParticipationNotificationId(deltaEvent.InternalParticipationId,
            string.Join("_", deltaEvent.OriginCommands.Select(c => c.CommandId)));

    private IWritableNode ToNode(TargetNode nodeId)
    {
        if (TryToNode(nodeId, out var w))
            return w;

        throw new DeltaException(DeltaErrorCode.UnknownNodeId.AsError(null, null, nodeId));
    }

    private bool TryToNode(TargetNode nodeId, [NotNullWhen(true)] out IWritableNode? node)
    {
        if (_sharedNodeMap.TryGetValue(nodeId, out var n) && n is IWritableNode w)
        {
            node = w;
            return true;
        }

        node = null;
        return false;
    }

    private T ToFeature<T>(MetaPointer deltaReference, IReadableNode node) where T : Feature
    {
        if (_sharedKeyedMap.TryGetValue(Compress(deltaReference), out var e) && e is T c)
            return c;

        throw new UnknownFeatureException(node.GetClassifier(), deltaReference);
    }

    private CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, true);

    private IWritableNode Deserialize(DeltaSerializationChunk deltaChunk)
    {
        var nodes = _deserializerBuilder.Build().Deserialize(deltaChunk.Nodes, _sharedNodeMap.Values);
        if (nodes is [IWritableNode w])
            return w;

        // TODO change to correct exception 
        throw new NotImplementedException();
    }
}