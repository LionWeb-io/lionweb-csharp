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
using Core.M2;
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
    private readonly SharedNodeMap _sharedNodeMap;
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
            MoveAndReplaceChildFromOtherContainmentInSameParent a =>
                OnMoveAndReplaceChildFromOtherContainmentInSameParent(a),
            AddAnnotation a => OnAddAnnotation(a),
            DeleteAnnotation a => OnDeleteAnnotation(a),
            MoveAnnotationFromOtherParent a => OnMoveAnnotationFromOtherParent(a),
            MoveAnnotationInSameParent a => OnMoveAnnotationInSameParent(a),
            AddReference a => OnAddReference(a),
            DeleteReference a => OnDeleteReference(a),
            ChangeReference a => OnChangeReference(a),
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
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment), containment).ToList()[command.Index],
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
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment), containment).ToList()[command.Index],
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
        var oldParent = (IWritableNode)movedChild.GetParent();
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
        var oldParent = (IWritableNode)movedChild.GetParent();
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
        var parent = (IWritableNode)replacedChild.GetParent();
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
        var parent = (IWritableNode)movedChild.GetParent();
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

    private ChildMovedInSameContainmentNotification OnMoveChildInSameContainment(MoveChildInSameContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var parent = (IWritableNode)movedChild.GetParent();
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
        var deletedAnnotation = parent.GetAnnotations()[deleteAnnotationCommand.Index];
        return new AnnotationDeletedNotification(
            deletedAnnotation as IWritableNode ?? throw new InvalidValueException(null, deletedAnnotation),
            parent,
            deleteAnnotationCommand.Index,
            ToNotificationId(deleteAnnotationCommand)
        );
    }

    private AnnotationMovedFromOtherParentNotification OnMoveAnnotationFromOtherParent(
        MoveAnnotationFromOtherParent moveAnnotationCommand)
    {
        var movedAnnotation = ToNode(moveAnnotationCommand.MovedAnnotation);
        var oldParent = (IWritableNode)movedAnnotation.GetParent();
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
        var parent = (IWritableNode)movedAnnotation.GetParent();
        var oldIndex = parent.GetAnnotations().ToList().IndexOf(movedAnnotation);
        
        return new AnnotationMovedInSameParentNotification(
            moveAnnotationCommand.NewIndex,
            movedAnnotation,
            parent,
            oldIndex,
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
            ToTarget(addReferenceCommand.NewTarget, addReferenceCommand.NewResolveInfo),
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
            ReferenceTarget.FromNode(parent.Get(reference) as IReadableNode),
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
            ToTarget(changeReferenceCommand.NewTarget, changeReferenceCommand.NewResolveInfo),
            ReferenceTarget.FromNode(parent.Get(reference) as IReadableNode),
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

    private static INotificationId ToNotificationId(IDeltaCommand command) =>
        new ParticipationNotificationId(command.InternalParticipationId, command.CommandId);

    private IWritableNode ToNode(TargetNode nodeId)
    {
        if (_sharedNodeMap.TryGetValue(nodeId, out var node))
        {
            if (node is IWritableNode w) 
                return w;
           
            throw new InvalidCastException($"Can not cast {node.GetType().Name} to {nameof(IWritableNode)}");
        }

        throw new InvalidOperationException($"Unknown node with id: {nodeId}");
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
 
        throw new InvalidCastException($"Can not cast nodes in {nodes.GetType().Name} to {nameof(IWritableNode)}");
    }
}