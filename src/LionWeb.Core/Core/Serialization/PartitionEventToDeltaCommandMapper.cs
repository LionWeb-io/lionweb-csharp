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

public class PartitionEventToDeltaCommandMapper
{
    private readonly ICommandIdProvider _commandIdProvider;
    private readonly LionWebVersions _lionWebVersion;
    private readonly ISerializerVersionSpecifics _propertySerializer;

    public PartitionEventToDeltaCommandMapper(ICommandIdProvider commandIdProvider, LionWebVersions lionWebVersion)
    {
        _commandIdProvider = commandIdProvider;
        _lionWebVersion = lionWebVersion;
        _propertySerializer = ISerializerVersionSpecifics.Create(lionWebVersion);
    }

    public IDeltaCommand Map(IPartitionEvent partitionEvent) =>
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

    private AddProperty OnPropertyAdded(PropertyAddedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToDelta(@event.Node, @event.Property, @event.NewValue)!,
            ToCommandId(@event),
            null
        );

    private DeleteProperty OnPropertyDeleted(PropertyDeletedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToCommandId(@event),
            null
        );

    private ChangeProperty OnPropertyChanged(PropertyChangedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToDelta(@event.Node, @event.Property, @event.NewValue)!,
            ToCommandId(@event),
            null
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private AddChild OnChildAdded(ChildAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToDeltaChunk(@event.NewChild),
            ToCommandId(@event),
            null
        );

    private DeleteChild OnChildDeleted(ChildDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToCommandId(@event),
            null
        );

    private ReplaceChild OnChildReplaced(ChildReplacedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToDeltaChunk(@event.NewChild),
            ToCommandId(@event),
            null
        );

    private MoveChildFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent @event) =>
        new(
            @event.NewParent.GetId(),
            @event.NewContainment.ToMetaPointer(),
            @event.NewIndex,
            @event.MovedChild.GetId(),
            ToCommandId(@event),
            null
        );

    private MoveChildFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentEvent @event) =>
        new(
            @event.NewContainment.ToMetaPointer(),
            @event.NewIndex,
            @event.MovedChild.GetId(),
            ToCommandId(@event),
            null
        );

    private MoveChildInSameContainment OnChildMovedInSameContainment(ChildMovedInSameContainmentEvent @event) =>
        new(
            @event.NewIndex,
            @event.MovedChild.GetId(),
            ToCommandId(@event),
            null
        );

    #endregion

    #region Annotations

    private AddAnnotation OnAnnotationAdded(AnnotationAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Index,
            ToDeltaChunk(@event.NewAnnotation),
            ToCommandId(@event),
            null
        );

    private DeleteAnnotation OnAnnotationDeleted(AnnotationDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Index,
            ToCommandId(@event),
            null
        );

    private MoveAnnotationFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent @event) =>
        new(
            @event.NewParent.GetId(),
            @event.NewIndex,
            @event.MovedAnnotation.GetId(),
            ToCommandId(@event),
            null
        );

    private MoveAnnotationInSameParent OnAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent @event) =>
        new(
            @event.NewIndex,
            @event.MovedAnnotation.GetId(),
            ToCommandId(@event),
            null
        );

    #endregion

    #region References

    private AddReference OnReferenceAdded(ReferenceAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToDelta(@event.NewTarget),
            ToCommandId(@event),
            null
        );

    private DeleteReference OnReferenceDeleted(ReferenceDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToCommandId(@event),
            null
        );

    private ChangeReference OnReferenceChanged(ReferenceChangedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToDelta(@event.NewTarget),
            ToCommandId(@event),
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

    private CommandId ToCommandId(IEvent @event) =>
        @event.EventId switch
        {
            ParticipationEventId pei => pei.CommandId,
            _ => _commandIdProvider.Create()
        };
}

public interface ICommandIdProvider
{
    CommandId Create();
}