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
using M1.Event.Partition;
using M3;
using CommandId = NodeId;
using SemanticPropertyValue = object;

public class DeltaProtocolPartitionCommandSender
{
    private readonly ICommandIdProvider _commandIdProvider;
    private readonly LionWebVersions _lionWebVersion;
    private readonly ISerializerVersionSpecifics _propertySerializer;

    public event EventHandler<IDeltaCommand>? DeltaCommand;

    public DeltaProtocolPartitionCommandSender(IPartitionPublisher partitionPublisher,
        ICommandIdProvider commandIdProvider, LionWebVersions lionWebVersion)
    {
        partitionPublisher.Subscribe<M1.Event.Partition.IPartitionEvent>(ProcessEvent);
        _commandIdProvider = commandIdProvider;
        _lionWebVersion = lionWebVersion;
        _propertySerializer = ISerializerVersionSpecifics.Create(lionWebVersion);
    }

    private void ProcessEvent(object? sender, M1.Event.Partition.IPartitionEvent? @event)
    {
        if (DeltaCommand == null || @event == null)
            return;

        IDeltaCommand deltaCommand = @event switch
        {
            PropertyAddedEvent a => OnPropertyAdded(sender, a),
            PropertyDeletedEvent a => OnPropertyDeleted(sender, a),
            PropertyChangedEvent a => OnPropertyChanged(sender, a),
            ChildAddedEvent a => OnChildAdded(sender, a),
            ChildDeletedEvent a => OnChildDeleted(sender, a),
            ChildReplacedEvent a => OnChildReplaced(sender, a),
            ChildMovedFromOtherContainmentEvent a => OnChildMovedFromOtherContainment(sender, a),
            ChildMovedFromOtherContainmentInSameParentEvent a =>
                OnChildMovedFromOtherContainmentInSameParent(sender, a),
            ChildMovedInSameContainmentEvent a => OnChildMovedInSameContainment(sender, a),
            AnnotationAddedEvent a => OnAnnotationAdded(sender, a),
            AnnotationDeletedEvent a => OnAnnotationDeleted(sender, a),
            AnnotationMovedFromOtherParentEvent a => OnAnnotationMovedFromOtherParent(sender, a),
            AnnotationMovedInSameParentEvent a => OnAnnotationMovedInSameParent(sender, a),
            ReferenceAddedEvent a => OnReferenceAdded(sender, a),
            ReferenceDeletedEvent a => OnReferenceDeleted(sender, a),
            ReferenceChangedEvent a => OnReferenceChanged(sender, a),
            _ => throw new NotImplementedException(@event.GetType().Name)
        };

        DeltaCommand.Invoke(sender, deltaCommand);
    }

    #region Properties

    private AddProperty OnPropertyAdded(object? sender, PropertyAddedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToDelta(@event.Node, @event.Property, @event.NewValue)!,
            NewCommandId(),
            null
        );

    private DeleteProperty OnPropertyDeleted(object? sender, PropertyDeletedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            NewCommandId(),
            null
        );

    private ChangeProperty OnPropertyChanged(object? sender, PropertyChangedEvent @event) =>
        new(
            @event.Node.GetId(),
            @event.Property.ToMetaPointer(),
            ToDelta(@event.Node, @event.Property, @event.NewValue)!,
            NewCommandId(),
            null
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, SemanticPropertyValue newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private AddChild OnChildAdded(object? sender, ChildAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToDeltaChunk(@event.NewChild),
            NewCommandId(),
            null
        );

    private DeleteChild OnChildDeleted(object? sender, ChildDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            NewCommandId(),
            null
        );

    private ReplaceChild OnChildReplaced(object? sender, ChildReplacedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Containment.ToMetaPointer(),
            @event.Index,
            ToDeltaChunk(@event.NewChild),
            NewCommandId(),
            null
        );

    private MoveChildFromOtherContainment OnChildMovedFromOtherContainment(object? sender,
        ChildMovedFromOtherContainmentEvent @event) =>
        new(
            @event.NewParent.GetId(),
            @event.NewContainment.ToMetaPointer(),
            @event.NewIndex,
            @event.MovedChild.GetId(),
            NewCommandId(),
            null
        );

    private MoveChildFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(object? sender,
        ChildMovedFromOtherContainmentInSameParentEvent @event) =>
        new(
            @event.NewContainment.ToMetaPointer(),
            @event.NewIndex,
            @event.MovedChild.GetId(),
            NewCommandId(),
            null
        );

    private MoveChildInSameContainment OnChildMovedInSameContainment(object? sender,
        ChildMovedInSameContainmentEvent @event) =>
        new(
            @event.NewIndex,
            @event.MovedChild.GetId(),
            NewCommandId(),
            null
        );

    #endregion

    #region Annotations

    private AddAnnotation OnAnnotationAdded(object? sender, AnnotationAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Index,
            ToDeltaChunk(@event.NewAnnotation),
            NewCommandId(),
            null
        );

    private DeleteAnnotation OnAnnotationDeleted(object? sender, AnnotationDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Index,
            NewCommandId(),
            null
        );

    private MoveAnnotationFromOtherParent OnAnnotationMovedFromOtherParent(object? sender,
        AnnotationMovedFromOtherParentEvent @event) =>
        new(
            @event.NewParent.GetId(),
            @event.NewIndex,
            @event.MovedAnnotation.GetId(),
            NewCommandId(),
            null
        );

    private MoveAnnotationInSameParent OnAnnotationMovedInSameParent(object? sender,
        AnnotationMovedInSameParentEvent @event) =>
        new(
            @event.NewIndex,
            @event.MovedAnnotation.GetId(),
            NewCommandId(),
            null
        );

    #endregion

    #region References

    private AddReference OnReferenceAdded(object? sender, ReferenceAddedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToDelta(@event.NewTarget),
            NewCommandId(),
            null
        );

    private DeleteReference OnReferenceDeleted(object? sender, ReferenceDeletedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            NewCommandId(),
            null
        );

    private ChangeReference OnReferenceChanged(object? sender, ReferenceChangedEvent @event) =>
        new(
            @event.Parent.GetId(),
            @event.Reference.ToMetaPointer(),
            @event.Index,
            ToDelta(@event.NewTarget),
            NewCommandId(),
            null
        );

    private SerializedReferenceTarget ToDelta(IReferenceTarget target) =>
        new SerializedReferenceTarget { Reference = target.Reference?.GetId(), ResolveInfo = target.ResolveInfo };

    #endregion

    private CommandId NewCommandId() =>
        _commandIdProvider.Create();

    private DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }
}

public interface ICommandIdProvider
{
    CommandId Create();
}