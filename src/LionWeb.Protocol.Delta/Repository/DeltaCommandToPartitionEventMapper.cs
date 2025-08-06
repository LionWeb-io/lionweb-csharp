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
using Core.M1.Event;
using Core.M1.Event.Partition;
using Core.M2;
using Core.M3;
using Core.Serialization;
using Message;
using Message.Command;
using System.Collections;

public class DeltaCommandToPartitionEventMapper
{
    private readonly SharedNodeMap _sharedNodeMap;
    private readonly Dictionary<CompressedMetaPointer, IKeyed> _sharedKeyedMap;
    private readonly DeserializerBuilder _deserializerBuilder;

    public DeltaCommandToPartitionEventMapper(
        SharedNodeMap sharedNodeMap,
        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _deserializerBuilder = deserializerBuilder;
    }

    public IPartitionEvent Map(ICommand command) =>
        command switch
        {
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
            AddAnnotation a => OnAddAnnotation(a),
            DeleteAnnotation a => OnDeleteAnnotation(a),
            MoveAnnotationFromOtherParent a => OnMoveAnnotationFromOtherParent(a),
            MoveAnnotationInSameParent a => OnMoveAnnotationInSameParent(a),
            AddReference a => OnAddReference(a),
            DeleteReference a => OnDeleteReference(a),
            ChangeReference a => OnChangeReference(a),
            _ => throw new NotImplementedException(command.GetType().Name)
        };

    #region Properties

    private PropertyAddedEvent OnAddProperty(AddProperty addPropertyEvent)
    {
        var parent = ToNode(addPropertyEvent.Node);
        var property = ToProperty(addPropertyEvent.Property, parent);
        return new PropertyAddedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, addPropertyEvent.NewValue),
            ToEventId(addPropertyEvent)
        );
    }

    private PropertyDeletedEvent OnDeleteProperty(DeleteProperty deletePropertyEvent)
    {
        var parent = ToNode(deletePropertyEvent.Node);
        var property = ToProperty(deletePropertyEvent.Property, parent);
        return new PropertyDeletedEvent(
            parent,
            property,
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToEventId(deletePropertyEvent)
        );
    }

    private PropertyChangedEvent OnChangeProperty(ChangeProperty changePropertyEvent)
    {
        var parent = ToNode(changePropertyEvent.Node);
        var property = ToProperty(changePropertyEvent.Property, parent);
        return new PropertyChangedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, changePropertyEvent.NewValue),
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToEventId(changePropertyEvent)
        );
    }

    private Property ToProperty(MetaPointer deltaProperty, IReadableNode node) =>
        ToFeature<Property>(deltaProperty, node);

    private SemanticPropertyValue ToPropertyValue(IReadableNode node, Property property, PropertyValue value) =>
        _deserializerBuilder.Build().VersionSpecifics.ConvertDatatype(node, property, property.Type, value) ??
        throw new InvalidValueException(property, value);

    #endregion

    #region Children

    private ChildAddedEvent OnAddChild(AddChild command)
    {
        var parent = ToNode(command.Parent);
        var containment = ToContainment(command.Containment, parent);

        return new ChildAddedEvent(
            parent,
            Deserialize(command.NewChild),
            containment,
            command.Index,
            ToEventId(command)
        );
    }

    private ChildDeletedEvent OnDeleteChild(DeleteChild command)
    {
        var parent = ToNode(command.Parent);
        var containment = ToContainment(command.Containment, parent);

        return new ChildDeletedEvent(
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[command.Index],
            parent,
            containment,
            command.Index,
            ToEventId(command)
        );
    }

    private ChildReplacedEvent OnReplaceChild(ReplaceChild command)
    {
        var parent = ToNode(command.Parent);
        var containment = ToContainment(command.Containment, parent);
        return new ChildReplacedEvent(
            Deserialize(command.NewChild),
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[command.Index],
            parent,
            containment,
            command.Index,
            ToEventId(command)
        );
    }

    private ChildMovedFromOtherContainmentEvent OnMoveChildFromOtherContainment(MoveChildFromOtherContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var oldParent = (IWritableNode)movedChild.GetParent();
        var newParent = ToNode(command.NewParent);
        var oldContainment = oldParent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(command.NewContainment, newParent);
        var oldIndex = GetChildIndex(oldParent, oldContainment, movedChild);

        return new ChildMovedFromOtherContainmentEvent(
            newParent,
            newContainment,
            command.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            oldIndex,
            ToEventId(command)
        );
    }

    private ChildMovedAndReplacedFromOtherContainmentEvent OnMoveAndReplaceChildFromOtherContainment(MoveAndReplaceChildFromOtherContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var oldParent = (IWritableNode)movedChild.GetParent();
        var oldContainment = oldParent.GetContainmentOf(movedChild);
        var newParent = ToNode(command.NewParent);
        var newContainment = ToContainment(command.NewContainment, newParent);
        var replacedChild = ToNode(command.ReplacedChild);
        var oldIndex = GetChildIndex(oldParent, oldContainment, movedChild);

        return new ChildMovedAndReplacedFromOtherContainmentEvent(
            newParent,
            newContainment,
            command.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            oldIndex,
            replacedChild,
            ToEventId(command)
        );
    }
    
    private ChildMovedAndReplacedFromOtherContainmentInSameParentEvent 
        OnMoveAndReplaceChildFromOtherContainmentInSameParent(MoveAndReplaceChildFromOtherContainmentInSameParent command)
    {
        var movedChild = ToNode(command.MovedChild);
        var replacedChild = ToNode(command.ReplacedChild);
        var parent = (IWritableNode)replacedChild.GetParent();
        var newContainment = ToContainment(command.NewContainment, parent);
        var oldContainment = parent.GetContainmentOf(movedChild);
        var oldIndex = GetChildIndex(parent, oldContainment, movedChild);
        
        return new ChildMovedAndReplacedFromOtherContainmentInSameParentEvent(
            newContainment,
            command.NewIndex,
            movedChild,
            parent,
            oldContainment,
            oldIndex,
            replacedChild,
            ToEventId(command)
            );
    }

    private ChildMovedFromOtherContainmentInSameParentEvent OnMoveChildFromOtherContainmentInSameParent(
        MoveChildFromOtherContainmentInSameParent command)
    {
        var movedChild = ToNode(command.MovedChild);
        var parent = (IWritableNode)movedChild.GetParent();
        var oldContainment = parent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(command.NewContainment, parent);
        var oldIndex = GetChildIndex(parent, oldContainment, movedChild);

        return new ChildMovedFromOtherContainmentInSameParentEvent(
            newContainment,
            command.NewIndex,
            movedChild,
            parent,
            oldContainment,
            oldIndex,
            ToEventId(command)
        );
    }

    private ChildMovedInSameContainmentEvent OnMoveChildInSameContainment(MoveChildInSameContainment command)
    {
        var movedChild = ToNode(command.MovedChild);
        var parent = (IWritableNode)movedChild.GetParent();
        var containment = parent.GetContainmentOf(movedChild);
        var oldIndex = GetChildIndex(parent, containment, movedChild);

        return new ChildMovedInSameContainmentEvent(
            command.NewIndex,
            movedChild,
            parent,
            containment,
            oldIndex,
            ToEventId(command)
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

    private AnnotationAddedEvent OnAddAnnotation(AddAnnotation addAnnotationEvent)
    {
        var parent = ToNode(addAnnotationEvent.Parent);
        return new AnnotationAddedEvent(
            parent,
            Deserialize(addAnnotationEvent.NewAnnotation),
            addAnnotationEvent.Index,
            ToEventId(addAnnotationEvent)
        );
    }

    private AnnotationDeletedEvent OnDeleteAnnotation(DeleteAnnotation deleteAnnotationEvent)
    {
        var parent = ToNode(deleteAnnotationEvent.Parent);
        var deletedAnnotation = parent.GetAnnotations()[deleteAnnotationEvent.Index];
        return new AnnotationDeletedEvent(
            deletedAnnotation as IWritableNode ?? throw new InvalidValueException(null, deletedAnnotation),
            parent,
            deleteAnnotationEvent.Index,
            ToEventId(deleteAnnotationEvent)
        );
    }

    private AnnotationMovedFromOtherParentEvent OnMoveAnnotationFromOtherParent(MoveAnnotationFromOtherParent moveAnnotationEvent)
    {
        var movedAnnotation = ToNode(moveAnnotationEvent.MovedAnnotation);
        var oldParent = (IWritableNode)movedAnnotation.GetParent();
        var newParent = ToNode(moveAnnotationEvent.NewParent);
        return new AnnotationMovedFromOtherParentEvent(
            newParent,
            moveAnnotationEvent.NewIndex,
            movedAnnotation,
            oldParent,
            0, // TODO FIXME
            ToEventId(moveAnnotationEvent)
        );
    }

    private AnnotationMovedInSameParentEvent OnMoveAnnotationInSameParent(MoveAnnotationInSameParent moveAnnotationEvent)
    {
        var movedAnnotation = ToNode(moveAnnotationEvent.MovedAnnotation);
        var parent = (IWritableNode)movedAnnotation.GetParent();
        return new AnnotationMovedInSameParentEvent(
            moveAnnotationEvent.NewIndex,
            movedAnnotation,
            parent,
            0, // TODO FIXME
            ToEventId(moveAnnotationEvent)
        );
    }

    #endregion

    #region References

    private ReferenceAddedEvent OnAddReference(AddReference addReferenceEvent)
    {
        var parent = ToNode(addReferenceEvent.Parent);
        var reference = ToReference(addReferenceEvent.Reference, parent);
        return new ReferenceAddedEvent(
            parent,
            reference,
            addReferenceEvent.Index,
            ToTarget(addReferenceEvent.NewTarget, addReferenceEvent.NewResolveInfo),
            ToEventId(addReferenceEvent)
        );
    }

    private ReferenceDeletedEvent OnDeleteReference(DeleteReference deleteReferenceEvent)
    {
        var parent = ToNode(deleteReferenceEvent.Parent);
        var reference = ToReference(deleteReferenceEvent.Reference, parent);
        return new ReferenceDeletedEvent(
            parent,
            reference,
            deleteReferenceEvent.Index,
            new ReferenceTarget(null, parent.Get(reference) as IReadableNode),
            ToEventId(deleteReferenceEvent)
        );
    }

    private ReferenceChangedEvent OnChangeReference(ChangeReference changeReferenceEvent)
    {
        var parent = ToNode(changeReferenceEvent.Parent);
        var reference = ToReference(changeReferenceEvent.Reference, parent);
        return new ReferenceChangedEvent(
            parent,
            reference,
            changeReferenceEvent.Index,
            ToTarget(changeReferenceEvent.NewTarget, changeReferenceEvent.NewResolveInfo),
            new ReferenceTarget(null, parent.Get(reference) as IReadableNode),
            ToEventId(changeReferenceEvent)
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

        return new ReferenceTarget(resolveInfo, target);
    }

    #endregion


    private static IEventId ToEventId(ICommand command) =>
        new ParticipationEventId(command.InternalParticipationId, command.CommandId);

    private IWritableNode ToNode(TargetNode nodeId)
    {
        if (_sharedNodeMap.TryGetValue(nodeId, out var node) && node is IWritableNode w)
            return w;

        // TODO change to correct exception 
        throw new NotImplementedException();
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