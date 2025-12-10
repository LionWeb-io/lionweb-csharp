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

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1;
using Core.M3;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Serialization;
using Message;
using Message.Event;

public class NotificationToDeltaEventMapper
{
    private readonly IParticipationIdProvider _participationIdProvider;
    private readonly LionWebVersions _lionWebVersion;
    private readonly ISerializerVersionSpecifics _propertySerializer;

    public NotificationToDeltaEventMapper(IParticipationIdProvider participationIdProvider,
        LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        _participationIdProvider = participationIdProvider;
        _propertySerializer = ISerializerVersionSpecifics.Create(lionWebVersion);
    }

    public IDeltaEvent Map(INotification notification) =>
        notification switch
        {
            PartitionAddedNotification a => OnPartitionAdded(a),
            PartitionDeletedNotification a => OnPartitionDeleted(a),
            PropertyAddedNotification a => OnPropertyAdded(a),
            PropertyDeletedNotification a => OnPropertyDeleted(a),
            PropertyChangedNotification a => OnPropertyChanged(a),
            ChildAddedNotification a => OnChildAdded(a),
            ChildDeletedNotification a => OnChildDeleted(a),
            ChildReplacedNotification a => OnChildReplaced(a),
            ChildMovedFromOtherContainmentNotification a => OnChildMovedFromOtherContainment(a),
            ChildMovedFromOtherContainmentInSameParentNotification a => OnChildMovedFromOtherContainmentInSameParent(a),
            ChildMovedInSameContainmentNotification a => OnChildMovedInSameContainment(a),
            ChildMovedAndReplacedFromOtherContainmentNotification a => OnChildMovedAndReplacedFromOtherContainment(a),
            ChildMovedAndReplacedFromOtherContainmentInSameParentNotification a =>
                OnChildMovedAndReplacedFromOtherContainmentInSameParent(a),
            AnnotationAddedNotification a => OnAnnotationAdded(a),
            AnnotationDeletedNotification a => OnAnnotationDeleted(a),
            AnnotationMovedFromOtherParentNotification a => OnAnnotationMovedFromOtherParent(a),
            AnnotationMovedInSameParentNotification a => OnAnnotationMovedInSameParent(a),
            ReferenceAddedNotification a => OnReferenceAdded(a),
            ReferenceDeletedNotification a => OnReferenceDeleted(a),
            ReferenceChangedNotification a => OnReferenceChanged(a),
            _ => throw new ArgumentException($"{nameof(NotificationToDeltaEventMapper)} does not support {notification.GetType().Name}!")
        };

    #region Partitions

    private PartitionAdded OnPartitionAdded(PartitionAddedNotification partitionAddedNotification) =>
        new(
            ToDeltaChunk(partitionAddedNotification.NewPartition),
            partitionAddedNotification.NewPartition.GetId(),
            ToCommandSources(partitionAddedNotification),
            []
        );

    private PartitionDeleted OnPartitionDeleted(PartitionDeletedNotification partitionDeletedNotification) =>
        new(
            partitionDeletedNotification.DeletedPartition.GetId(),
            ToDescendants(partitionDeletedNotification.DeletedPartition),
            ToCommandSources(partitionDeletedNotification),
            []
        );

    #endregion

    #region Properties

    private PropertyAdded OnPropertyAdded(PropertyAddedNotification propertyAddedNotification) =>
        new(
            propertyAddedNotification.Node.GetId(),
            propertyAddedNotification.Property.ToMetaPointer(),
            ToDelta(propertyAddedNotification.Node, propertyAddedNotification.Property,
                propertyAddedNotification.NewValue)!,
            ToCommandSources(propertyAddedNotification),
            []
        );

    private PropertyDeleted OnPropertyDeleted(PropertyDeletedNotification propertyDeletedNotification) =>
        new(
            propertyDeletedNotification.Node.GetId(),
            propertyDeletedNotification.Property.ToMetaPointer(),
            ToDelta(propertyDeletedNotification.Node, propertyDeletedNotification.Property,
                propertyDeletedNotification.OldValue)!,
            ToCommandSources(propertyDeletedNotification),
            []
        );

    private PropertyChanged OnPropertyChanged(PropertyChangedNotification propertyChangedNotification) =>
        new(
            propertyChangedNotification.Node.GetId(),
            propertyChangedNotification.Property.ToMetaPointer(),
            ToDelta(propertyChangedNotification.Node, propertyChangedNotification.Property,
                propertyChangedNotification.NewValue)!,
            ToDelta(propertyChangedNotification.Node, propertyChangedNotification.Property,
                propertyChangedNotification.OldValue)!,
            ToCommandSources(propertyChangedNotification),
            []
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private ChildAdded OnChildAdded(ChildAddedNotification childAddedNotification) =>
        new(
            childAddedNotification.Parent.GetId(),
            ToDeltaChunk(childAddedNotification.NewChild),
            childAddedNotification.Containment.ToMetaPointer(),
            childAddedNotification.Index,
            ToCommandSources(childAddedNotification),
            []
        );

    private ChildDeleted OnChildDeleted(ChildDeletedNotification childDeletedNotification) =>
        new(
            childDeletedNotification.DeletedChild.GetId(),
            ToDescendants(childDeletedNotification.DeletedChild),
            childDeletedNotification.Parent.GetId(),
            childDeletedNotification.Containment.ToMetaPointer(),
            childDeletedNotification.Index,
            ToCommandSources(childDeletedNotification),
            []
        );

    private ChildReplaced OnChildReplaced(ChildReplacedNotification childReplacedNotification) =>
        new(
            ToDeltaChunk(childReplacedNotification.NewChild),
            childReplacedNotification.ReplacedChild.GetId(),
            ToDescendants(childReplacedNotification.ReplacedChild),
            childReplacedNotification.Parent.GetId(),
            childReplacedNotification.Containment.ToMetaPointer(),
            childReplacedNotification.Index,
            ToCommandSources(childReplacedNotification),
            []
        );

    private ChildMovedFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification childMovedNotification) =>
        new(
            childMovedNotification.NewParent.GetId(),
            childMovedNotification.NewContainment.ToMetaPointer(),
            childMovedNotification.NewIndex,
            childMovedNotification.MovedChild.GetId(),
            childMovedNotification.OldParent.GetId(),
            childMovedNotification.OldContainment.ToMetaPointer(),
            childMovedNotification.OldIndex,
            ToCommandSources(childMovedNotification),
            []
        );

    private ChildMovedAndReplacedFromOtherContainment
        OnChildMovedAndReplacedFromOtherContainment(
            ChildMovedAndReplacedFromOtherContainmentNotification childMovedAndReplacedNotification) =>
        new(
            childMovedAndReplacedNotification.NewParent.GetId(),
            childMovedAndReplacedNotification.NewContainment.ToMetaPointer(),
            childMovedAndReplacedNotification.NewIndex,
            childMovedAndReplacedNotification.MovedChild.GetId(),
            childMovedAndReplacedNotification.OldParent.GetId(),
            childMovedAndReplacedNotification.OldContainment.ToMetaPointer(),
            childMovedAndReplacedNotification.OldIndex,
            childMovedAndReplacedNotification.ReplacedChild.GetId(),
            ToDescendants(childMovedAndReplacedNotification.ReplacedChild),
            ToCommandSources(childMovedAndReplacedNotification),
            []
        );

    private ChildMovedAndReplacedFromOtherContainmentInSameParent
        OnChildMovedAndReplacedFromOtherContainmentInSameParent(
            ChildMovedAndReplacedFromOtherContainmentInSameParentNotification childMovedAndReplacedNotification) =>
        new(
            childMovedAndReplacedNotification.NewContainment.ToMetaPointer(),
            childMovedAndReplacedNotification.NewIndex,
            childMovedAndReplacedNotification.MovedChild.GetId(),
            childMovedAndReplacedNotification.Parent.GetId(),
            childMovedAndReplacedNotification.OldContainment.ToMetaPointer(),
            childMovedAndReplacedNotification.OldIndex,
            childMovedAndReplacedNotification.ReplacedChild.GetId(),
            ToDescendants(childMovedAndReplacedNotification.ReplacedChild),
            ToCommandSources(childMovedAndReplacedNotification),
            []
        );

    private ChildMovedFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification childMovedNotification) =>
        new(
            childMovedNotification.NewContainment.ToMetaPointer(),
            childMovedNotification.NewIndex,
            childMovedNotification.MovedChild.GetId(),
            childMovedNotification.Parent.GetId(),
            childMovedNotification.OldContainment.ToMetaPointer(),
            childMovedNotification.OldIndex,
            ToCommandSources(childMovedNotification),
            []
        );

    private ChildMovedInSameContainment OnChildMovedInSameContainment(
        ChildMovedInSameContainmentNotification childMovedNotification) =>
        new(
            childMovedNotification.NewIndex,
            childMovedNotification.MovedChild.GetId(),
            childMovedNotification.Parent.GetId(),
            childMovedNotification.Containment.ToMetaPointer(),
            childMovedNotification.OldIndex,
            ToCommandSources(childMovedNotification),
            []
        );

    #endregion

    #region Annotations

    private AnnotationAdded OnAnnotationAdded(AnnotationAddedNotification annotationAddedNotification) =>
        new(
            annotationAddedNotification.Parent.GetId(),
            ToDeltaChunk(annotationAddedNotification.NewAnnotation),
            annotationAddedNotification.Index,
            ToCommandSources(annotationAddedNotification),
            []
        );

    private AnnotationDeleted OnAnnotationDeleted(AnnotationDeletedNotification annotationDeletedNotification) =>
        new(
            annotationDeletedNotification.DeletedAnnotation.GetId(),
            ToDescendants(annotationDeletedNotification.DeletedAnnotation),
            annotationDeletedNotification.Parent.GetId(),
            annotationDeletedNotification.Index,
            ToCommandSources(annotationDeletedNotification),
            []
        );

    private AnnotationMovedFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification annotationMovedNotification) =>
        new(
            annotationMovedNotification.NewParent.GetId(),
            annotationMovedNotification.NewIndex,
            annotationMovedNotification.MovedAnnotation.GetId(),
            annotationMovedNotification.OldParent.GetId(),
            annotationMovedNotification.OldIndex,
            ToCommandSources(annotationMovedNotification),
            []
        );

    private AnnotationMovedInSameParent OnAnnotationMovedInSameParent(
        AnnotationMovedInSameParentNotification annotationMovedNotification) =>
        new(
            annotationMovedNotification.NewIndex,
            annotationMovedNotification.MovedAnnotation.GetId(),
            annotationMovedNotification.Parent.GetId(),
            annotationMovedNotification.OldIndex,
            ToCommandSources(annotationMovedNotification),
            []
        );

    #endregion

    #region References

    private ReferenceAdded OnReferenceAdded(ReferenceAddedNotification referenceAddedNotification) =>
        new(
            referenceAddedNotification.Parent.GetId(),
            referenceAddedNotification.Reference.ToMetaPointer(),
            referenceAddedNotification.Index,
            referenceAddedNotification.NewTarget.TargetId,
            referenceAddedNotification.NewTarget.ResolveInfo,
            ToCommandSources(referenceAddedNotification),
            []
        );

    private ReferenceDeleted OnReferenceDeleted(ReferenceDeletedNotification referenceDeletedNotification) =>
        new(
            referenceDeletedNotification.Parent.GetId(),
            referenceDeletedNotification.Reference.ToMetaPointer(),
            referenceDeletedNotification.Index,
            referenceDeletedNotification.DeletedTarget.TargetId,
            referenceDeletedNotification.DeletedTarget.ResolveInfo,
            ToCommandSources(referenceDeletedNotification),
            []
        );

    private ReferenceChanged OnReferenceChanged(ReferenceChangedNotification referenceChangedNotification) =>
        new(
            referenceChangedNotification.Parent.GetId(),
            referenceChangedNotification.Reference.ToMetaPointer(),
            referenceChangedNotification.Index,
            referenceChangedNotification.NewTarget.TargetId,
            referenceChangedNotification.NewTarget.ResolveInfo,
            referenceChangedNotification.OldTarget.TargetId,
            referenceChangedNotification.OldTarget.ResolveInfo,
            ToCommandSources(referenceChangedNotification),
            []
        );

    #endregion

    private DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }

    private TargetNode[] ToDescendants(IReadableNode node) =>
        M1Extensions.Descendants(node, false, true).Select(n => n.GetId()).ToArray();

    private CommandSource[] ToCommandSources(INotification notification)
    {
        ParticipationId participationId;
        EventId commandId;
        if (notification.NotificationId is ParticipationNotificationId pei)
        {
            participationId = pei.ParticipationId;
            commandId = pei.CommandId;
        } else
        {
            participationId = _participationIdProvider.Create();
            commandId = notification.NotificationId.ToString();
        }

        return [new CommandSource(participationId, commandId)];
    }
}