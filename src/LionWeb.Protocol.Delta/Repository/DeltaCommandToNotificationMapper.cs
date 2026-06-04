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
using Message.Command;
using System.Collections;

public class DeltaCommandToNotificationMapper
{
    private protected readonly SharedNodeMap _sharedNodeMap;
    private readonly SharedKeyedMap _sharedKeyedMap;
    private readonly DeserializerBuilder _deserializerBuilder;

    public DeltaCommandToNotificationMapper(
        SharedNodeMap sharedNodeMap,
        SharedKeyedMap sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _deserializerBuilder = deserializerBuilder;
    }

    public INotification Map(IDeltaCommand command) =>
        command switch
        {
            AddPartition a => OnAddPartition(a),
            DeletePartition a => OnDeletePartition(a),
            AddProperty a => OnAddProperty(a),
            DeleteProperty a => OnDeleteProperty(a),
            ChangeProperty a => OnChangeProperty(a),
            AddChild a => OnAddChild(a),
            DeleteChild a => OnDeleteChild(a),
            ReplaceChild a => OnReplaceChild(a),
            MoveChildFromOtherContainment a => OnMoveChildFromOtherContainment(a),
            MoveChildFromOtherContainmentInSameParent a => OnMoveChildFromOtherContainmentInSameParent(a),
            MoveChildInSameContainment a => OnMoveChildInSameContainment(a),
            MoveAndReplaceChildFromOtherContainment a => OnMoveAndReplaceChildFromOtherContainment(a),
            MoveAndReplaceChildFromOtherContainmentInSameParent a => OnMoveAndReplaceChildFromOtherContainmentInSameParent(a),
            MoveAndReplaceChildInSameContainment a => OnMoveAndReplaceChildInSameContainment(a),
            AddAnnotation a => OnAddAnnotation(a),
            DeleteAnnotation a => OnDeleteAnnotation(a),
            ReplaceAnnotation a => OnReplaceAnnotation(a),
            MoveAnnotationFromOtherParent a => OnMoveAnnotationFromOtherParent(a),
            MoveAnnotationInSameParent a => OnMoveAnnotationInSameParent(a),
            MoveAndReplaceAnnotationFromOtherParent a => OnMoveAndReplaceAnnotationFromOtherParent(a),
            MoveAndReplaceAnnotationInSameParent a => OnMoveAndReplaceAnnotationInSameParent(a),
            AddReference a => OnAddReference(a),
            DeleteReference a => OnDeleteReference(a),
            ChangeReference a => OnChangeReference(a),
            CompositeCommand a => OnComposite(a),
            _ => throw new ArgumentException($"{nameof(DeltaCommandToNotificationMapper)} does not support {command.GetType().Name}!")
        };

    #region Partitions

    private PartitionAddedNotification OnAddPartition(AddPartition addPartitionCommand) =>
        new(
            (IPartitionInstance)Deserialize(addPartitionCommand.NewPartition),
            ToNotificationId(addPartitionCommand)
        );

    private PartitionDeletedNotification OnDeletePartition(DeletePartition deletePartitionCommand) =>
        new(
            (IPartitionInstance)ToNode(deletePartitionCommand.DeletedPartition),
            ToNotificationId(deletePartitionCommand)
        );

    #endregion

    #region Properties

    private PropertyAddedNotification OnAddProperty(AddProperty addPropertyCommand)
    {
        var parent = ToNode(addPropertyCommand.Node);
        var property = ToProperty(addPropertyCommand.Property, parent);
        return new PropertyAddedNotification(
            parent,
            property,
            ToPropertyValue(parent, property, addPropertyCommand.NewValue),
            ToNotificationId(addPropertyCommand)
        );
    }

    private PropertyDeletedNotification OnDeleteProperty(DeleteProperty deletePropertyCommand)
    {
        var parent = ToNode(deletePropertyCommand.Node);
        var property = ToProperty(deletePropertyCommand.Property, parent);
        return new PropertyDeletedNotification(
            parent,
            property,
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToNotificationId(deletePropertyCommand)
        );
    }

    private PropertyChangedNotification OnChangeProperty(ChangeProperty changePropertyCommand)
    {
        var parent = ToNode(changePropertyCommand.Node);
        var property = ToProperty(changePropertyCommand.Property, parent);
        return new PropertyChangedNotification(
            parent,
            property,
            ToPropertyValue(parent, property, changePropertyCommand.NewValue),
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToNotificationId(changePropertyCommand)
        );
    }

    private Property ToProperty(MetaPointer deltaProperty, IReadableNode node) =>
        ToFeature<Property>(deltaProperty, node);

    private SemanticPropertyValue ToPropertyValue(IReadableNode node, Property property, PropertyValue value) =>
        _deserializerBuilder.Build().VersionSpecifics.ConvertDatatype(node, property, property.Type, value) ??
        throw new InvalidValueException(property, value);

    #endregion

    #region Children

    private ChildAddedNotification OnAddChild(AddChild command)
    {
        var parent = ToNode(command.Parent);
        var containment = ToContainment(command.Containment, parent);

        return new ChildAddedNotification(
            parent,
            Deserialize(command.NewChild),
            containment,
            command.Index,
            ToNotificationId(command)
        );
    }

    private ChildDeletedNotification OnDeleteChild(DeleteChild command)
    {
        var parent = ToNode(command.Parent);
        var containment = ToContainment(command.Containment, parent);

        return new ChildDeletedNotification(
            ToNode(command.DeletedChild),
            parent,
            containment,
            command.Index,
            ToNotificationId(command)
        );
    }

    private ChildReplacedNotification OnReplaceChild(ReplaceChild command)
    {
        var parent = ToNode(command.Parent);
        var containment = ToContainment(command.Containment, parent);
        return new ChildReplacedNotification(
            Deserialize(command.NewChild),
            ToNode(command.ReplacedChild),
            parent,
            containment,
            command.Index,
            ToNotificationId(command)
        );
    }

    private ChildMovedFromOtherContainmentNotification OnMoveChildFromOtherContainment(
        MoveChildFromOtherContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var oldParent = (IWritableNode?)movedChild.GetParent();
        if (oldParent is null)
            throw new LionWebMappingException(nameof(command.MovedChild), movedChild);
        var newParent = ToNode(command.NewParent);
        var oldContainment = oldParent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(command.NewContainment, newParent);
        var oldIndex = GetChildIndex(oldParent, oldContainment, movedChild);

        return new ChildMovedFromOtherContainmentNotification(
            newParent,
            newContainment,
            command.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            oldIndex,
            ToNotificationId(command)
        );
    }

    private ChildMovedAndReplacedFromOtherContainmentNotification OnMoveAndReplaceChildFromOtherContainment(
        MoveAndReplaceChildFromOtherContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var oldParent = (IWritableNode?)movedChild.GetParent();
        if (oldParent is null)
            throw new LionWebMappingException(nameof(command.MovedChild), movedChild);
        var oldContainment = oldParent.GetContainmentOf(movedChild);
        var newParent = ToNode(command.NewParent);
        var newContainment = ToContainment(command.NewContainment, newParent);
        var replacedChild = ToNode(command.ReplacedChild);
        var oldIndex = GetChildIndex(oldParent, oldContainment, movedChild);

        return new ChildMovedAndReplacedFromOtherContainmentNotification(
            newParent,
            newContainment,
            command.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            oldIndex,
            replacedChild,
            ToNotificationId(command)
        );
    }

    private ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        OnMoveAndReplaceChildFromOtherContainmentInSameParent(
            MoveAndReplaceChildFromOtherContainmentInSameParent command)
    {
        var movedChild = ToNode(command.MovedChild);
        var replacedChild = ToNode(command.ReplacedChild);
        var parent = (IWritableNode?)replacedChild.GetParent();
        if (parent is null)
            throw new LionWebMappingException(nameof(command.ReplacedChild), replacedChild);
        var newContainment = ToContainment(command.NewContainment, parent);
        var oldContainment = parent.GetContainmentOf(movedChild);
        var oldIndex = GetChildIndex(parent, oldContainment, movedChild);

        return new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(
            newContainment,
            command.NewIndex,
            movedChild,
            parent,
            oldContainment,
            oldIndex,
            replacedChild,
            ToNotificationId(command)
        );
    }

    private ChildMovedFromOtherContainmentInSameParentNotification OnMoveChildFromOtherContainmentInSameParent(
        MoveChildFromOtherContainmentInSameParent command)
    {
        var movedChild = ToNode(command.MovedChild);
        var parent = (IWritableNode?)movedChild.GetParent();
        if (parent is null)
            throw new LionWebMappingException(nameof(command.MovedChild), movedChild);
        var oldContainment = parent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(command.NewContainment, parent);
        var oldIndex = GetChildIndex(parent, oldContainment, movedChild);

        return new ChildMovedFromOtherContainmentInSameParentNotification(
            newContainment,
            command.NewIndex,
            movedChild,
            parent,
            oldContainment,
            oldIndex,
            ToNotificationId(command)
        );
    }

    private ChildMovedAndReplacedInSameContainmentNotification OnMoveAndReplaceChildInSameContainment(MoveAndReplaceChildInSameContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var parent = (IWritableNode?)movedChild.GetParent();
        if (parent is null)
            throw new LionWebMappingException(nameof(command.MovedChild), movedChild);
        var containment = parent.GetContainmentOf(movedChild);
        var oldIndex = GetChildIndex(parent, containment, movedChild);

        return new ChildMovedAndReplacedInSameContainmentNotification(
            command.NewIndex,
            movedChild,
            parent,
            containment,
            ToNode(command.ReplacedChild),
            oldIndex,
            ToNotificationId(command)
        );
    }

    private ChildMovedInSameContainmentNotification OnMoveChildInSameContainment(MoveChildInSameContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var parent = (IWritableNode?)movedChild.GetParent();
        if (parent is null)
            throw new LionWebMappingException(nameof(command.MovedChild), movedChild);
        var containment = parent.GetContainmentOf(movedChild);
        var oldIndex = GetChildIndex(parent, containment, movedChild);

        return new ChildMovedInSameContainmentNotification(
            command.NewIndex,
            movedChild,
            parent,
            containment,
            oldIndex,
            ToNotificationId(command)
        );
    }

    private Index GetChildIndex(IWritableNode parent, Containment? containment, IWritableNode child)
    {
        Index index;

        if (parent.TryGet(containment, out var existingChildren))
        {
            switch (existingChildren)
            {
                case IList l:
                    {
                        var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                        index = children.IndexOf(child);
                        break;
                    }
                case IWritableNode _:
                    index = 0;
                    break;
                default:
                    throw new InvalidValueException(containment, existingChildren);
            }
        } else
        {
            throw new UnsetFeatureException(containment);
        }

        return index;
    }

    private Containment ToContainment(MetaPointer deltaContainment, IReadableNode node) =>
        ToFeature<Containment>(deltaContainment, node);

    #endregion

    #region Annotations

    private AnnotationAddedNotification OnAddAnnotation(AddAnnotation addAnnotationCommand)
    {
        var parent = ToNode(addAnnotationCommand.Parent);
        return new AnnotationAddedNotification(
            parent,
            Deserialize(addAnnotationCommand.NewAnnotation),
            addAnnotationCommand.Index,
            ToNotificationId(addAnnotationCommand)
        );
    }

    private AnnotationDeletedNotification OnDeleteAnnotation(DeleteAnnotation deleteAnnotationCommand)
    {
        var parent = ToNode(deleteAnnotationCommand.Parent);
        var deletedAnnotation = ToNode(deleteAnnotationCommand.DeletedAnnotation);
        return new AnnotationDeletedNotification(
            deletedAnnotation as IWritableNode ?? throw new InvalidValueException(null, deletedAnnotation),
            parent,
            deleteAnnotationCommand.Index,
            ToNotificationId(deleteAnnotationCommand)
        );
    }

    private AnnotationReplacedNotification OnReplaceAnnotation(ReplaceAnnotation command)
    {
        var parent = ToNode(command.Parent);
        return new AnnotationReplacedNotification(
            Deserialize(command.NewAnnotation),
            ToNode(command.ReplacedAnnotation),
            parent,
            command.Index,
            ToNotificationId(command)
        );
    }

    private AnnotationMovedFromOtherParentNotification OnMoveAnnotationFromOtherParent(
        MoveAnnotationFromOtherParent moveAnnotationCommand)
    {
        var movedAnnotation = ToNode(moveAnnotationCommand.MovedAnnotation);
        var oldParent = (IWritableNode?)movedAnnotation.GetParent();
        if (oldParent is null)
            throw new LionWebMappingException(nameof(moveAnnotationCommand.MovedAnnotation), movedAnnotation);
        var oldIndex = oldParent.GetAnnotations().ToList().IndexOf(movedAnnotation);

        var newParent = ToNode(moveAnnotationCommand.NewParent);
        return new AnnotationMovedFromOtherParentNotification(
            newParent,
            moveAnnotationCommand.NewIndex,
            movedAnnotation,
            oldParent,
            oldIndex,
            ToNotificationId(moveAnnotationCommand)
        );
    }

    private AnnotationMovedInSameParentNotification OnMoveAnnotationInSameParent(
        MoveAnnotationInSameParent moveAnnotationCommand)
    {
        var movedAnnotation = ToNode(moveAnnotationCommand.MovedAnnotation);
        var parent = (IWritableNode?)movedAnnotation.GetParent();
        if (parent is null)
            throw new LionWebMappingException(nameof(moveAnnotationCommand.MovedAnnotation), movedAnnotation);
        var oldIndex = parent.GetAnnotations().ToList().IndexOf(movedAnnotation);

        return new AnnotationMovedInSameParentNotification(
            moveAnnotationCommand.NewIndex,
            movedAnnotation,
            parent,
            oldIndex,
            ToNotificationId(moveAnnotationCommand)
        );
    }

    private AnnotationMovedAndReplacedFromOtherParentNotification OnMoveAndReplaceAnnotationFromOtherParent(
        MoveAndReplaceAnnotationFromOtherParent moveAnnotationCommand)
    {
        var movedAnnotation = ToNode(moveAnnotationCommand.MovedAnnotation);
        var oldParent = (IWritableNode?)movedAnnotation.GetParent();
        if (oldParent is null)
            throw new LionWebMappingException(nameof(moveAnnotationCommand.MovedAnnotation), movedAnnotation);
        var oldIndex = oldParent.GetAnnotations().ToList().IndexOf(movedAnnotation);

        var newParent = ToNode(moveAnnotationCommand.NewParent);
        return new AnnotationMovedAndReplacedFromOtherParentNotification(
            newParent,
            moveAnnotationCommand.NewIndex,
            movedAnnotation,
            oldParent,
            oldIndex,
            ToNode(moveAnnotationCommand.ReplacedAnnotation),
            ToNotificationId(moveAnnotationCommand)
        );
    }

    private AnnotationMovedAndReplacedInSameParentNotification OnMoveAndReplaceAnnotationInSameParent(
        MoveAndReplaceAnnotationInSameParent moveAnnotationCommand)
    {
        var movedAnnotation = ToNode(moveAnnotationCommand.MovedAnnotation);
        var parent = (IWritableNode?)movedAnnotation.GetParent();
        if (parent is null)
            throw new LionWebMappingException(nameof(moveAnnotationCommand.MovedAnnotation), movedAnnotation);
        var oldIndex = parent.GetAnnotations().ToList().IndexOf(movedAnnotation);

        return new AnnotationMovedAndReplacedInSameParentNotification(
            moveAnnotationCommand.NewIndex,
            movedAnnotation,
            parent,
            oldIndex,
            ToNode(moveAnnotationCommand.ReplacedAnnotation),
            ToNotificationId(moveAnnotationCommand)
        );
    }

    #endregion

    #region References

    private ReferenceAddedNotification OnAddReference(AddReference addReferenceCommand)
    {
        var parent = ToNode(addReferenceCommand.Parent);
        var reference = ToReference(addReferenceCommand.Reference, parent);
        return new ReferenceAddedNotification(
            parent,
            reference,
            addReferenceCommand.Index,
            ToTarget(addReferenceCommand.NewReference, addReferenceCommand.NewResolveInfo),
            ToNotificationId(addReferenceCommand)
        );
    }

    private ReferenceDeletedNotification OnDeleteReference(DeleteReference deleteReferenceCommand)
    {
        var parent = ToNode(deleteReferenceCommand.Parent);
        var reference = ToReference(deleteReferenceCommand.Reference, parent);
        return new ReferenceDeletedNotification(
            parent,
            reference,
            deleteReferenceCommand.Index,
            ToTarget(deleteReferenceCommand.DeletedReference, deleteReferenceCommand.DeletedResolveInfo),
            ToNotificationId(deleteReferenceCommand)
        );
    }

    private ReferenceChangedNotification OnChangeReference(ChangeReference changeReferenceCommand)
    {
        var parent = ToNode(changeReferenceCommand.Parent);
        var reference = ToReference(changeReferenceCommand.Reference, parent);
        return new ReferenceChangedNotification(
            parent,
            reference,
            changeReferenceCommand.Index,
            ToTarget(changeReferenceCommand.NewReference, changeReferenceCommand.NewResolveInfo),
            ToTarget(changeReferenceCommand.OldReference, changeReferenceCommand.OldResolveInfo),
            ToNotificationId(changeReferenceCommand)
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

    private CompositeNotification OnComposite(CompositeCommand compositeCommand)
    {
        var mapper = new InterdependentDeltaCommandToNotificationMapper(_sharedNodeMap, _sharedKeyedMap, _deserializerBuilder);

        return new(
            compositeCommand.Parts.Select(mapper.Map),
            ToNotificationId(compositeCommand)
        );
    }

    private static INotificationId ToNotificationId(IDeltaCommand command) =>
        new ParticipationNotificationId(command.InternalParticipationId, command.CommandId);

    private T ToFeature<T>(MetaPointer deltaReference, IReadableNode node) where T : Feature
    {
        if (_sharedKeyedMap.TryGetValue(deltaReference, out var e) && e is T c)
            return c;

        throw new UnknownFeatureException(node.GetClassifier(), deltaReference);
    }

    private protected virtual IWritableNode ToNode(TargetNode nodeId)
    {
        if (_sharedNodeMap.TryGetValue(nodeId, out var node))
        {
            if (node is IWritableNode w)
                return w;

            throw new UnsupportedNodeTypeException(node, nameof(node));
        }

        throw new InvalidOperationException($"Unknown node with id: {nodeId}");
    }

    private protected virtual IWritableNode Deserialize(DeltaSerializationChunk deltaChunk)
    {
        var nodes = _deserializerBuilder.Build().Deserialize(deltaChunk.Nodes, _sharedNodeMap.Values);

        var node = nodes.FirstOrDefault();
        if (node is not IWritableNode w)
        {
            throw new UnsupportedNodeTypeException(node, nameof(node));
        }

        return w;
    }
}

internal class InterdependentDeltaCommandToNotificationMapper(SharedNodeMap sharedNodeMap, SharedKeyedMap sharedKeyedMap, DeserializerBuilder deserializerBuilder)
    : DeltaCommandToNotificationMapper(sharedNodeMap, sharedKeyedMap, deserializerBuilder)
{
    private readonly SharedNodeMap _interdependentNodeMap = new();

    private protected override IWritableNode ToNode(TargetNode nodeId)
    {
        if (_sharedNodeMap.TryGetValue(nodeId, out var node))
        {
            if (node is IWritableNode w)
                return w;

            throw new UnsupportedNodeTypeException(node, nameof(node));
        }

        if (_interdependentNodeMap.TryGetValue(nodeId, out node))
        {
            if (node is IWritableNode w)
                return w;

            throw new UnsupportedNodeTypeException(node, nameof(node));
        }

        throw new InvalidOperationException($"Unknown node with id: {nodeId}");
    }

    private protected override IWritableNode Deserialize(DeltaSerializationChunk deltaChunk)
    {
        var result = base.Deserialize(deltaChunk);
        _interdependentNodeMap.RegisterNode(result);
        return result;
    }
}