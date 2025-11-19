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
using Core.M3;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Serialization;
using Message;
using Message.Command;

public class NotificationToDeltaCommandMapper
{
    private readonly ICommandIdProvider _commandIdProvider;
    private readonly LionWebVersions _lionWebVersion;
    private readonly ISerializerVersionSpecifics _propertySerializer;

    public NotificationToDeltaCommandMapper(ICommandIdProvider commandIdProvider, LionWebVersions lionWebVersion)
    {
        _commandIdProvider = commandIdProvider;
        _lionWebVersion = lionWebVersion;
        _propertySerializer = ISerializerVersionSpecifics.Create(lionWebVersion);
    }

    public IDeltaCommand Map(INotification notification) =>
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
            ChildMovedFromOtherContainmentInSameParentNotification a =>
                OnChildMovedFromOtherContainmentInSameParent(a),
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
            _ => throw new NotImplementedException(notification.GetType().Name)
        };

    #region Partitions

    private AddPartition OnPartitionAdded(PartitionAddedNotification partitionAddedNotification) =>
        new(
            ToDeltaChunk(partitionAddedNotification.NewPartition),
            ToCommandId(partitionAddedNotification),
            []
        );

    private DeletePartition OnPartitionDeleted(PartitionDeletedNotification partitionDeletedNotification) =>
        new(
            partitionDeletedNotification.DeletedPartition.GetId(),
            ToCommandId(partitionDeletedNotification),
            []
        );

    #endregion

    #region Properties

    private AddProperty OnPropertyAdded(PropertyAddedNotification propertyAddedNotification) =>
        new(
            propertyAddedNotification.Node.GetId(),
            propertyAddedNotification.Property.ToMetaPointer(),
            ToDelta(propertyAddedNotification.Node, propertyAddedNotification.Property,
                propertyAddedNotification.NewValue)!,
            ToCommandId(propertyAddedNotification),
            []
        );

    private DeleteProperty OnPropertyDeleted(PropertyDeletedNotification propertyDeletedNotification) =>
        new(
            propertyDeletedNotification.Node.GetId(),
            propertyDeletedNotification.Property.ToMetaPointer(),
            ToCommandId(propertyDeletedNotification),
            []
        );

    private ChangeProperty OnPropertyChanged(PropertyChangedNotification propertyChangedNotification) =>
        new(
            propertyChangedNotification.Node.GetId(),
            propertyChangedNotification.Property.ToMetaPointer(),
            ToDelta(propertyChangedNotification.Node, propertyChangedNotification.Property,
                propertyChangedNotification.NewValue)!,
            ToCommandId(propertyChangedNotification),
            []
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private AddChild OnChildAdded(ChildAddedNotification childAddedNotification) =>
        new(
            childAddedNotification.Parent.GetId(),
            ToDeltaChunk(childAddedNotification.NewChild),
            childAddedNotification.Containment.ToMetaPointer(),
            childAddedNotification.Index,
            ToCommandId(childAddedNotification),
            []
        );

    private DeleteChild OnChildDeleted(ChildDeletedNotification childDeletedNotification) =>
        new(
            childDeletedNotification.Parent.GetId(),
            childDeletedNotification.Containment.ToMetaPointer(),
            childDeletedNotification.Index,
            childDeletedNotification.DeletedChild.GetId(),
            ToCommandId(childDeletedNotification),
            []
        );

    private ReplaceChild OnChildReplaced(ChildReplacedNotification childReplacedNotification) =>
        new(
            ToDeltaChunk(childReplacedNotification.NewChild),
            childReplacedNotification.Parent.GetId(),
            childReplacedNotification.Containment.ToMetaPointer(),
            childReplacedNotification.Index,
            childReplacedNotification.ReplacedChild.GetId(),
            ToCommandId(childReplacedNotification),
            []
        );

    private MoveChildFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification childMovedNotification) =>
        new(
            childMovedNotification.NewParent.GetId(),
            childMovedNotification.NewContainment.ToMetaPointer(),
            childMovedNotification.NewIndex,
            childMovedNotification.MovedChild.GetId(),
            ToCommandId(childMovedNotification),
            []
        );

    private MoveAndReplaceChildFromOtherContainment
        OnChildMovedAndReplacedFromOtherContainment(
            ChildMovedAndReplacedFromOtherContainmentNotification childMovedAndReplacedNotification) =>
        new(
            childMovedAndReplacedNotification.NewParent.GetId(),
            childMovedAndReplacedNotification.NewContainment.ToMetaPointer(),
            childMovedAndReplacedNotification.NewIndex,
            childMovedAndReplacedNotification.ReplacedChild.GetId(),
            childMovedAndReplacedNotification.MovedChild.GetId(),
            ToCommandId(childMovedAndReplacedNotification),
            []
        );

    private MoveAndReplaceChildFromOtherContainmentInSameParent
        OnChildMovedAndReplacedFromOtherContainmentInSameParent(
            ChildMovedAndReplacedFromOtherContainmentInSameParentNotification childMovedAndReplacedNotification) =>
        new(
            childMovedAndReplacedNotification.NewContainment.ToMetaPointer(),
            childMovedAndReplacedNotification.NewIndex,
            childMovedAndReplacedNotification.ReplacedChild.GetId(),
            childMovedAndReplacedNotification.MovedChild.GetId(),
            ToCommandId(childMovedAndReplacedNotification),
            []
        );

    private MoveChildFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification childMovedNotification) =>
        new(
            childMovedNotification.NewContainment.ToMetaPointer(),
            childMovedNotification.NewIndex,
            childMovedNotification.MovedChild.GetId(),
            ToCommandId(childMovedNotification),
            []
        );

    private MoveChildInSameContainment OnChildMovedInSameContainment(
        ChildMovedInSameContainmentNotification childMovedNotification) =>
        new(
            childMovedNotification.NewIndex,
            childMovedNotification.MovedChild.GetId(),
            ToCommandId(childMovedNotification),
            []
        );

    #endregion

    #region Annotations

    private AddAnnotation OnAnnotationAdded(AnnotationAddedNotification annotationAddedNotification) =>
        new(
            annotationAddedNotification.Parent.GetId(),
            ToDeltaChunk(annotationAddedNotification.NewAnnotation),
            annotationAddedNotification.Index,
            ToCommandId(annotationAddedNotification),
            []
        );

    private DeleteAnnotation OnAnnotationDeleted(AnnotationDeletedNotification annotationDeletedNotification) =>
        new(
            annotationDeletedNotification.Parent.GetId(),
            annotationDeletedNotification.Index,
            annotationDeletedNotification.DeletedAnnotation.GetId(),
            ToCommandId(annotationDeletedNotification),
            []
        );

    private MoveAnnotationFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification annotationMovedNotification) =>
        new(
            annotationMovedNotification.NewParent.GetId(),
            annotationMovedNotification.NewIndex,
            annotationMovedNotification.MovedAnnotation.GetId(),
            ToCommandId(annotationMovedNotification),
            []
        );

    private MoveAnnotationInSameParent OnAnnotationMovedInSameParent(
        AnnotationMovedInSameParentNotification annotationMovedNotification) =>
        new(
            annotationMovedNotification.NewIndex,
            annotationMovedNotification.MovedAnnotation.GetId(),
            ToCommandId(annotationMovedNotification),
            []
        );

    #endregion

    #region References

    private AddReference OnReferenceAdded(ReferenceAddedNotification referenceAddedNotification) =>
        new(
            referenceAddedNotification.Parent.GetId(),
            referenceAddedNotification.Reference.ToMetaPointer(),
            referenceAddedNotification.Index,
            referenceAddedNotification.NewTarget.TargetId,
            referenceAddedNotification.NewTarget.ResolveInfo,
            ToCommandId(referenceAddedNotification),
            []
        );

    private DeleteReference OnReferenceDeleted(ReferenceDeletedNotification referenceDeletedNotification) =>
        new(
            referenceDeletedNotification.Parent.GetId(),
            referenceDeletedNotification.Reference.ToMetaPointer(),
            referenceDeletedNotification.Index,
            referenceDeletedNotification.DeletedTarget.TargetId,
            referenceDeletedNotification.DeletedTarget.ResolveInfo,
            ToCommandId(referenceDeletedNotification),
            []
        );

    private ChangeReference OnReferenceChanged(ReferenceChangedNotification referenceChangedNotification) =>
        new(
            referenceChangedNotification.Parent.GetId(),
            referenceChangedNotification.Reference.ToMetaPointer(),
            referenceChangedNotification.Index,
            referenceChangedNotification.OldTarget.TargetId,
            referenceChangedNotification.OldTarget.ResolveInfo,
            referenceChangedNotification.NewTarget.TargetId,
            referenceChangedNotification.NewTarget.ResolveInfo,
            ToCommandId(referenceChangedNotification),
            []
        );

    #endregion

    private SerializedReferenceTarget ToDelta(IReferenceTarget target) =>
        new SerializedReferenceTarget { Reference = target.TargetId, ResolveInfo = target.ResolveInfo };

    private DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }

    private CommandId ToCommandId(INotification notification) =>
        notification.NotificationId switch
        {
            ParticipationNotificationId pei => pei.CommandId,
            _ => _commandIdProvider.Create()
        };
}