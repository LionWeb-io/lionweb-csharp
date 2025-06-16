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

    private PropertyAdded OnPropertyAdded(PropertyAddedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToDelta(@event.Node, @event.Property, @event.NewValue)!,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private PropertyDeleted OnPropertyDeleted(PropertyDeletedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToDelta(@event.Node, @event.Property, @event.OldValue)!,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private PropertyChanged OnPropertyChanged(PropertyChangedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToDelta(@event.Node, @event.Property, @event.NewValue)!,
            ToDelta(@event.Node, @event.Property, @event.OldValue)!,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private ChildAdded OnChildAdded(ChildAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToDeltaChunk(@event.NewChild),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private ChildDeleted OnChildDeleted(ChildDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToDeltaChunk(@event.DeletedChild),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private ChildReplaced OnChildReplaced(ChildReplacedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToDeltaChunk(@event.NewChild),
            ToDeltaChunk(@event.ReplacedChild),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private ChildMovedFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent @event) =>
        new(
            @event.NewParent.GetId(),
            @event.NewContainment.ToMetaPointer(),
            @event.NewIndex,
            @event.MovedChild.GetId(),
            @event.OldParent.GetId(),
            @event.OldContainment.ToMetaPointer(),
            @event.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private ChildMovedFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentEvent @event) =>
        new(
            @event.NewContainment.ToMetaPointer(),
            @event.NewIndex,
            @event.MovedChild.GetId(),
            @event.Parent.GetId(),
            @event.OldContainment.ToMetaPointer(),
            @event.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private ChildMovedInSameContainment OnChildMovedInSameContainment(ChildMovedInSameContainmentEvent @event) =>
        new(
            @event.NewIndex,
            @event.MovedChild.GetId(),
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    #endregion

    #region Annotations

    private AnnotationAdded OnAnnotationAdded(AnnotationAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Index,
            ToDeltaChunk(@event.NewAnnotation),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private AnnotationDeleted OnAnnotationDeleted(AnnotationDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Index,
            ToDeltaChunk(@event.DeletedAnnotation),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private AnnotationMovedFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent @event) =>
        new(
            @event.NewParent.GetId(),
            @event.NewIndex,
            @event.MovedAnnotation.GetId(),
            @event.OldParent.GetId(),
            @event.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private AnnotationMovedInSameParent OnAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent @event) =>
        new(
            @event.NewIndex,
            @event.MovedAnnotation.GetId(),
            @event.Parent.GetId(),
            @event.OldIndex,
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    #endregion

    #region References

    private ReferenceAdded OnReferenceAdded(ReferenceAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToDelta(@event.NewTarget),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private ReferenceDeleted OnReferenceDeleted(ReferenceDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToDelta(@event.DeletedTarget),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
            null
        );

    private ReferenceChanged OnReferenceChanged(ReferenceChangedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToDelta(@event.NewTarget),
            ToDelta(@event.ReplacedTarget),
            NewEventSequenceNumber(),
            ToCommandSources(@event),
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