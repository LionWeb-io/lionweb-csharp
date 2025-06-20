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

namespace LionWeb.Core.Serialization;

using Delta;
using Delta.Event;
using M1;
using M1.Event;
using M1.Event.Partition;
using M3;
using ParticipationId = NodeId;
using EventSequenceNumber = long;

public class PartitionEventToDeltaEventMapper
{
    private readonly IParticipationIdProvider _participationIdProvider;
    private readonly IEventSequenceNumberProvider _eventSequenceNumberProvider;
    private readonly LionWebVersions _lionWebVersion;
    private readonly ISerializerVersionSpecifics _propertySerializer;

    public PartitionEventToDeltaEventMapper(IParticipationIdProvider participationIdProvider, IEventSequenceNumberProvider eventSequenceNumberProvider, LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        _participationIdProvider = participationIdProvider;
        _eventSequenceNumberProvider = eventSequenceNumberProvider;
        _propertySerializer = ISerializerVersionSpecifics.Create(lionWebVersion);
    }

    public IDeltaEvent Map(IPartitionEvent partitionEvent) =>
        partitionEvent switch
        {
            PropertyAddedEvent a => OnPropertyAdded(a),
            PropertyDeletedEvent a => OnPropertyDeleted(a),
            PropertyChangedEvent a => OnPropertyChanged(a),
            ChildAddedEvent a => OnChildAdded(a),
            ChildDeletedEvent a => OnChildDeleted(a),
            ChildReplacedEvent a => OnChildReplaced(a),
            ChildMovedFromOtherContainmentEvent a => OnChildMovedFromOtherContainment(a),
            ChildMovedFromOtherContainmentInSameParentEvent a =>
                OnChildMovedFromOtherContainmentInSameParent(a),
            ChildMovedInSameContainmentEvent a => OnChildMovedInSameContainment(a),
            AnnotationAddedEvent a => OnAnnotationAdded(a),
            AnnotationDeletedEvent a => OnAnnotationDeleted(a),
            AnnotationMovedFromOtherParentEvent a => OnAnnotationMovedFromOtherParent(a),
            AnnotationMovedInSameParentEvent a => OnAnnotationMovedInSameParent(a),
            ReferenceAddedEvent a => OnReferenceAdded(a),
            ReferenceDeletedEvent a => OnReferenceDeleted(a),
            ReferenceChangedEvent a => OnReferenceChanged(a),
            _ => throw new NotImplementedException(partitionEvent.GetType().Name)
        };
    
    #region Properties

    private PropertyAdded OnPropertyAdded(PropertyAddedEvent propertyAddedEvent) =>
        new(
            propertyAddedEvent.Node.GetId(),
            propertyAddedEvent.Property.ToMetaPointer(),
            ToDelta(propertyAddedEvent.Node, propertyAddedEvent.Property, propertyAddedEvent.NewValue)!,
            NewEventSequenceNumber(),
            ToCommandSources(propertyAddedEvent),
            null
        );

    private PropertyDeleted OnPropertyDeleted(PropertyDeletedEvent propertyDeletedEvent) =>
        new(
            propertyDeletedEvent.Node.GetId(),
            propertyDeletedEvent.Property.ToMetaPointer(),
            ToDelta(propertyDeletedEvent.Node, propertyDeletedEvent.Property, propertyDeletedEvent.OldValue)!,
            NewEventSequenceNumber(),
            ToCommandSources(propertyDeletedEvent),
            null
        );

    private PropertyChanged OnPropertyChanged(PropertyChangedEvent propertyChangedEvent) =>
        new(
            propertyChangedEvent.Node.GetId(),
            propertyChangedEvent.Property.ToMetaPointer(),
            ToDelta(propertyChangedEvent.Node, propertyChangedEvent.Property, propertyChangedEvent.NewValue)!,
            ToDelta(propertyChangedEvent.Node, propertyChangedEvent.Property, propertyChangedEvent.OldValue)!,
            NewEventSequenceNumber(),
            ToCommandSources(propertyChangedEvent),
            null
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private ChildAdded OnChildAdded(ChildAddedEvent childAddedEvent) =>
        new(
            childAddedEvent.Parent.GetId(),
            childAddedEvent.Containment.ToMetaPointer(),
            childAddedEvent.Index,
            ToDeltaChunk(childAddedEvent.NewChild),
            NewEventSequenceNumber(),
            ToCommandSources(childAddedEvent),
            null
        );

    private ChildDeleted OnChildDeleted(ChildDeletedEvent childDeletedEvent) =>
        new(
            childDeletedEvent.Parent.GetId(),
            childDeletedEvent.Containment.ToMetaPointer(),
            childDeletedEvent.Index,
            ToDeltaChunk(childDeletedEvent.DeletedChild),
            NewEventSequenceNumber(),
            ToCommandSources(childDeletedEvent),
            null
        );

    private ChildReplaced OnChildReplaced(ChildReplacedEvent childReplacedEvent) =>
        new(
            childReplacedEvent.Parent.GetId(),
            childReplacedEvent.Containment.ToMetaPointer(),
            childReplacedEvent.Index,
            ToDeltaChunk(childReplacedEvent.NewChild),
            ToDeltaChunk(childReplacedEvent.ReplacedChild),
            NewEventSequenceNumber(),
            ToCommandSources(childReplacedEvent),
            null
        );

    private ChildMovedFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewParent.GetId(),
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            childMovedEvent.OldParent.GetId(),
            childMovedEvent.OldContainment.ToMetaPointer(),
            childMovedEvent.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(childMovedEvent),
            null
        );

    private ChildMovedFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            childMovedEvent.Parent.GetId(),
            childMovedEvent.OldContainment.ToMetaPointer(),
            childMovedEvent.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(childMovedEvent),
            null
        );

    private ChildMovedInSameContainment OnChildMovedInSameContainment(ChildMovedInSameContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            childMovedEvent.Parent.GetId(),
            childMovedEvent.Containment.ToMetaPointer(),
            childMovedEvent.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(childMovedEvent),
            null
        );

    #endregion

    #region Annotations

    private AnnotationAdded OnAnnotationAdded(AnnotationAddedEvent annotationAddedEvent) =>
        new(
            annotationAddedEvent.Parent.GetId(),
            annotationAddedEvent.Index,
            ToDeltaChunk(annotationAddedEvent.NewAnnotation),
            NewEventSequenceNumber(),
            ToCommandSources(annotationAddedEvent),
            null
        );

    private AnnotationDeleted OnAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        new(
            annotationDeletedEvent.Parent.GetId(),
            annotationDeletedEvent.Index,
            ToDeltaChunk(annotationDeletedEvent.DeletedAnnotation),
            NewEventSequenceNumber(),
            ToCommandSources(annotationDeletedEvent),
            null
        );

    private AnnotationMovedFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewParent.GetId(),
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            annotationMovedEvent.OldParent.GetId(),
            annotationMovedEvent.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(annotationMovedEvent),
            null
        );

    private AnnotationMovedInSameParent OnAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            annotationMovedEvent.Parent.GetId(),
            annotationMovedEvent.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(annotationMovedEvent),
            null
        );

    #endregion

    #region References

    private ReferenceAdded OnReferenceAdded(ReferenceAddedEvent referenceAddedEvent) =>
        new(
            referenceAddedEvent.Parent.GetId(),
            referenceAddedEvent.Reference.ToMetaPointer(),
            referenceAddedEvent.Index,
            ToDelta(referenceAddedEvent.NewTarget),
            NewEventSequenceNumber(),
            ToCommandSources(referenceAddedEvent),
            null
        );

    private ReferenceDeleted OnReferenceDeleted(ReferenceDeletedEvent referenceDeletedEvent) =>
        new(
            referenceDeletedEvent.Parent.GetId(),
            referenceDeletedEvent.Reference.ToMetaPointer(),
            referenceDeletedEvent.Index,
            ToDelta(referenceDeletedEvent.DeletedTarget),
            NewEventSequenceNumber(),
            ToCommandSources(referenceDeletedEvent),
            null
        );

    private ReferenceChanged OnReferenceChanged(ReferenceChangedEvent referenceChangedEvent) =>
        new(
            referenceChangedEvent.Parent.GetId(),
            referenceChangedEvent.Reference.ToMetaPointer(),
            referenceChangedEvent.Index,
            ToDelta(referenceChangedEvent.NewTarget),
            ToDelta(referenceChangedEvent.ReplacedTarget),
            NewEventSequenceNumber(),
            ToCommandSources(referenceChangedEvent),
            null
        );

    private SerializedReferenceTarget ToDelta(IReferenceTarget target) =>
        new SerializedReferenceTarget { Reference = target.Reference?.GetId(), ResolveInfo = target.ResolveInfo };

    #endregion

    private DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }

    private EventSequenceNumber NewEventSequenceNumber() =>
        _eventSequenceNumberProvider.Create();
    
    private CommandSource[] ToCommandSources(IEvent @event)
    {
        ParticipationId participationId;
        EventId commandId;
        if (@event.EventId is ParticipationEventId pei)
        {
            participationId = pei.ParticipationId;
            commandId = pei.CommandId;
        } else
        {
            participationId = _participationIdProvider.ParticipationId;
            commandId = @event.EventId.ToString();
        }
        return [new CommandSource(participationId, commandId)];
    }
}

public interface IEventSequenceNumberProvider
{
    EventSequenceNumber Create();
}

public interface IParticipationIdProvider
{
    ParticipationId ParticipationId { get; }
}