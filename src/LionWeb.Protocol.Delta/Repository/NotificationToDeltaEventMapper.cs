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
    private readonly IEventSequenceNumberProvider _eventSequenceNumberProvider;
    private readonly ISerializerVersionSpecifics _propertySerializer;

    public NotificationToDeltaEventMapper(
        IParticipationIdProvider participationIdProvider,
        LionWebVersions lionWebVersion,
        IEventSequenceNumberProvider eventSequenceNumberProvider
    )
    {
        _lionWebVersion = lionWebVersion;
        _eventSequenceNumberProvider = eventSequenceNumberProvider;
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
            ChildMovedFromContainmentInOtherParentNotification a => OnChildMovedFromContainmentInOtherParent(a),
            ChildMovedFromOtherContainmentInSameParentNotification a => OnChildMovedFromOtherContainmentInSameParent(a),
            ChildMovedInSameContainmentNotification a => OnChildMovedInSameContainment(a),
            ChildMovedAndReplacedFromContainmentInOtherParentNotification a => OnChildMovedAndReplacedFromContainmentInOtherParent(a),
            ChildMovedAndReplacedFromOtherContainmentInSameParentNotification a => OnChildMovedAndReplacedFromOtherContainmentInSameParent(a),
            ChildMovedAndReplacedInSameContainmentNotification a => OnChildMovedAndReplacedInSameContainment(a),
            AnnotationAddedNotification a => OnAnnotationAdded(a),
            AnnotationDeletedNotification a => OnAnnotationDeleted(a),
            AnnotationReplacedNotification a => OnAnnotationReplaced(a),
            AnnotationMovedFromOtherParentNotification a => OnAnnotationMovedFromOtherParent(a),
            AnnotationMovedInSameParentNotification a => OnAnnotationMovedInSameParent(a),
            AnnotationMovedAndReplacedFromOtherParentNotification a => OnAnnotationMovedAndReplacedFromOtherParent(a),
            AnnotationMovedAndReplacedInSameParentNotification a => OnAnnotationMovedAndReplacedInSameParent(a),
            ReferenceAddedNotification a => OnReferenceAdded(a),
            ReferenceDeletedNotification a => OnReferenceDeleted(a),
            ReferenceChangedNotification a => OnReferenceChanged(a),
            CompositeNotification a => OnComposite(a),
            _ => throw new ArgumentException($"{nameof(NotificationToDeltaEventMapper)} does not support {notification.GetType().Name}!")
        };

    #region Partitions

    private PartitionAdded OnPartitionAdded(PartitionAddedNotification partitionAddedNotification) =>
        new(
            ToDeltaChunk(partitionAddedNotification.FrozenNewPartition ?? partitionAddedNotification.NewPartition),
            partitionAddedNotification.NewPartition.GetId(),
            ToCommandSources(partitionAddedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private PartitionDeleted OnPartitionDeleted(PartitionDeletedNotification partitionDeletedNotification) =>
        new(
            partitionDeletedNotification.DeletedPartition.GetId(),
            ToDescendants(partitionDeletedNotification.DeletedPartition),
            ToCommandSources(partitionDeletedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

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
        ) { SequenceNumber = NextEventSequenceNumber() };

    private PropertyDeleted OnPropertyDeleted(PropertyDeletedNotification propertyDeletedNotification) =>
        new(
            propertyDeletedNotification.Node.GetId(),
            propertyDeletedNotification.Property.ToMetaPointer(),
            ToDelta(propertyDeletedNotification.Node, propertyDeletedNotification.Property,
                propertyDeletedNotification.OldValue)!,
            ToCommandSources(propertyDeletedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

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
        ) { SequenceNumber = NextEventSequenceNumber() };

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private ChildAdded OnChildAdded(ChildAddedNotification childAddedNotification) =>
        new(
            childAddedNotification.Parent.GetId(),
            ToDeltaChunk(childAddedNotification.FrozenNewChild ?? childAddedNotification.NewChild),
            childAddedNotification.Containment.ToMetaPointer(),
            childAddedNotification.Index,
            ToCommandSources(childAddedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private ChildDeleted OnChildDeleted(ChildDeletedNotification childDeletedNotification) =>
        new(
            childDeletedNotification.DeletedChild.GetId(),
            ToDescendants(childDeletedNotification.DeletedChild),
            childDeletedNotification.Parent.GetId(),
            childDeletedNotification.Containment.ToMetaPointer(),
            childDeletedNotification.Index,
            ToCommandSources(childDeletedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private ChildReplaced OnChildReplaced(ChildReplacedNotification childReplacedNotification) =>
        new(
            ToDeltaChunk(childReplacedNotification.FrozenNewChild ?? childReplacedNotification.NewChild),
            childReplacedNotification.ReplacedChild.GetId(),
            ToDescendants(childReplacedNotification.ReplacedChild),
            childReplacedNotification.Parent.GetId(),
            childReplacedNotification.Containment.ToMetaPointer(),
            childReplacedNotification.Index,
            ToCommandSources(childReplacedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private ChildMovedFromContainmentInOtherParent
        OnChildMovedFromContainmentInOtherParent(ChildMovedFromContainmentInOtherParentNotification childMovedNotification) =>
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
        ) { SequenceNumber = NextEventSequenceNumber() };

    private ChildMovedAndReplacedFromContainmentInOtherParent
        OnChildMovedAndReplacedFromContainmentInOtherParent(
            ChildMovedAndReplacedFromContainmentInOtherParentNotification childMovedAndReplacedNotification) =>
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
        ) { SequenceNumber = NextEventSequenceNumber() };

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
        ) { SequenceNumber = NextEventSequenceNumber() };

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
        ) { SequenceNumber = NextEventSequenceNumber() };

    private ChildMovedAndReplacedInSameContainment OnChildMovedAndReplacedInSameContainment(
        ChildMovedAndReplacedInSameContainmentNotification childMovedNotification) =>
        new(
            childMovedNotification.MovedChild.GetId(),
            childMovedNotification.Parent.GetId(),
            childMovedNotification.Containment.ToMetaPointer(),
            childMovedNotification.OldIndex,
            childMovedNotification.IndexOffset,
            childMovedNotification.ReplacedChild.GetId(),
            ToDescendants(childMovedNotification.ReplacedChild),
            ToCommandSources(childMovedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private ChildMovedInSameContainment OnChildMovedInSameContainment(
        ChildMovedInSameContainmentNotification childMovedNotification) =>
        new(
            childMovedNotification.MovedChild.GetId(),
            childMovedNotification.Parent.GetId(),
            childMovedNotification.Containment.ToMetaPointer(),
            childMovedNotification.OldIndex,
            childMovedNotification.IndexOffset,
            ToCommandSources(childMovedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    #endregion

    #region Annotations

    private AnnotationAdded OnAnnotationAdded(AnnotationAddedNotification annotationAddedNotification) =>
        new(
            annotationAddedNotification.Parent.GetId(),
            ToDeltaChunk(annotationAddedNotification.FrozenNewAnnotation ?? annotationAddedNotification.NewAnnotation),
            annotationAddedNotification.Index,
            ToCommandSources(annotationAddedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private AnnotationDeleted OnAnnotationDeleted(AnnotationDeletedNotification annotationDeletedNotification) =>
        new(
            annotationDeletedNotification.DeletedAnnotation.GetId(),
            ToDescendants(annotationDeletedNotification.DeletedAnnotation),
            annotationDeletedNotification.Parent.GetId(),
            annotationDeletedNotification.Index,
            ToCommandSources(annotationDeletedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private AnnotationReplaced OnAnnotationReplaced(AnnotationReplacedNotification annotationReplacedNotification) =>
        new(
            ToDeltaChunk(annotationReplacedNotification.FrozenNewAnnotation ?? annotationReplacedNotification.NewAnnotation),
            annotationReplacedNotification.ReplacedAnnotation.GetId(),
            ToDescendants(annotationReplacedNotification.ReplacedAnnotation),
            annotationReplacedNotification.Parent.GetId(),
            annotationReplacedNotification.Index,
            ToCommandSources(annotationReplacedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

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
        ) { SequenceNumber = NextEventSequenceNumber() };

    private AnnotationMovedInSameParent OnAnnotationMovedInSameParent(
        AnnotationMovedInSameParentNotification annotationMovedNotification) =>
        new(
            annotationMovedNotification.MovedAnnotation.GetId(),
            annotationMovedNotification.Parent.GetId(),
            annotationMovedNotification.OldIndex,
            annotationMovedNotification.IndexOffset,
            ToCommandSources(annotationMovedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private AnnotationMovedAndReplacedFromOtherParent
        OnAnnotationMovedAndReplacedFromOtherParent(AnnotationMovedAndReplacedFromOtherParentNotification annotationMovedNotification) =>
        new(
            annotationMovedNotification.NewParent.GetId(),
            annotationMovedNotification.NewIndex,
            annotationMovedNotification.MovedAnnotation.GetId(),
            annotationMovedNotification.OldParent.GetId(),
            annotationMovedNotification.OldIndex,
            annotationMovedNotification.ReplacedAnnotation.GetId(),
            ToDescendants(annotationMovedNotification.ReplacedAnnotation),
            ToCommandSources(annotationMovedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

    private AnnotationMovedAndReplacedInSameParent OnAnnotationMovedAndReplacedInSameParent(
        AnnotationMovedAndReplacedInSameParentNotification annotationMovedNotification) =>
        new(
            annotationMovedNotification.MovedAnnotation.GetId(),
            annotationMovedNotification.Parent.GetId(),
            annotationMovedNotification.OldIndex,
            annotationMovedNotification.IndexOffset,
            annotationMovedNotification.ReplacedAnnotation.GetId(),
            ToDescendants(annotationMovedNotification.ReplacedAnnotation),
            ToCommandSources(annotationMovedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

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
        ) { SequenceNumber = NextEventSequenceNumber() };

    private ReferenceDeleted OnReferenceDeleted(ReferenceDeletedNotification referenceDeletedNotification) =>
        new(
            referenceDeletedNotification.Parent.GetId(),
            referenceDeletedNotification.Reference.ToMetaPointer(),
            referenceDeletedNotification.Index,
            referenceDeletedNotification.DeletedTarget.TargetId,
            referenceDeletedNotification.DeletedTarget.ResolveInfo,
            ToCommandSources(referenceDeletedNotification),
            []
        ) { SequenceNumber = NextEventSequenceNumber() };

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
        ) { SequenceNumber = NextEventSequenceNumber() };

    #endregion

    private CompositeEvent OnComposite(CompositeNotification compositeNotification)
    {
        var eventSequenceNumber = NextEventSequenceNumber();
        return new CompositeEvent(
            [.. compositeNotification.Parts.Select(notification => (INonContinuedDeltaEvent)Map(notification))],
            ToCommandSources(compositeNotification),
            []
        ) { SequenceNumber = eventSequenceNumber };
    }

    private DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }

    private TargetNode[] ToDescendants(IReadableNode node) =>
        M1Extensions.Descendants(node, false, true).Select(n => n.GetId()).ToArray();

    private CommandSource[] ToCommandSources(INotification notification) => 
        [new(notification.NotificationId.ParticipationId ?? _participationIdProvider.Create(), notification.NotificationId.CommandId)];

    private EventSequenceNumber NextEventSequenceNumber() =>
        _eventSequenceNumberProvider.Next();
}