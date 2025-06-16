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
using M2;
using M3;
using TargetNode = NodeId;
using SemanticPropertyValue = object;

public class DeltaCommandToPartitionEventMapper
{
    private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;
    private readonly Dictionary<CompressedMetaPointer, IKeyed> _sharedKeyedMap;
    private readonly DeserializerBuilder _deserializerBuilder;

    public DeltaCommandToPartitionEventMapper(
        Dictionary<NodeId, IReadableNode> sharedNodeMap,
        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _deserializerBuilder = deserializerBuilder;
    }

    public IPartitionEvent Map(IDeltaCommand command) =>
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
        var parent = ToNode(addPropertyEvent.Parent);
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
        var parent = ToNode(deletePropertyEvent.Parent);
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
        var parent = ToNode(changePropertyEvent.Parent);
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

    private ChildAddedEvent OnAddChild(AddChild addChildEvent)
    {
        var parent = ToNode(addChildEvent.Parent);
        var containment = ToContainment(addChildEvent.Containment, parent);
        return new ChildAddedEvent(
            parent,
            Deserialize(addChildEvent.NewChild),
            containment,
            addChildEvent.Index,
            ToEventId(addChildEvent)
        );
    }

    private ChildDeletedEvent OnDeleteChild(DeleteChild deleteChildEvent)
    {
        var parent = ToNode(deleteChildEvent.Parent);
        var containment = ToContainment(deleteChildEvent.Containment, parent);
        return new ChildDeletedEvent(
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[deleteChildEvent.Index],
            parent,
            containment,
            deleteChildEvent.Index,
            ToEventId(deleteChildEvent)
        );
    }

    private ChildReplacedEvent OnReplaceChild(ReplaceChild replaceChildEvent)
    {
        var parent = ToNode(replaceChildEvent.Parent);
        var containment = ToContainment(replaceChildEvent.Containment, parent);
        return new ChildReplacedEvent(
            Deserialize(replaceChildEvent.NewChild),
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[replaceChildEvent.Index],
            parent,
            containment,
            replaceChildEvent.Index,
            ToEventId(replaceChildEvent)
        );
    }

    private ChildMovedFromOtherContainmentEvent OnMoveChildFromOtherContainment(MoveChildFromOtherContainment moveChildEvent)
    {
        var movedChild = ToNode(moveChildEvent.MovedChild);
        var oldParent = (IWritableNode)movedChild.GetParent();
        var newParent = ToNode(moveChildEvent.NewParent);
        var oldContainment = oldParent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(moveChildEvent.NewContainment, newParent);
        return new ChildMovedFromOtherContainmentEvent(
            newParent,
            newContainment,
            moveChildEvent.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            0, // TODO FIXME
            ToEventId(moveChildEvent)
        );
    }

    private ChildMovedFromOtherContainmentInSameParentEvent OnMoveChildFromOtherContainmentInSameParent(
        MoveChildFromOtherContainmentInSameParent moveChildEvent)
    {
        var movedChild = ToNode(moveChildEvent.MovedChild);
        var parent = (IWritableNode)movedChild.GetParent();
        var oldContainment = parent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(moveChildEvent.NewContainment, parent);
        return new ChildMovedFromOtherContainmentInSameParentEvent(
            newContainment,
            moveChildEvent.NewIndex,
            movedChild,
            parent,
            oldContainment,
            0, // TODO FIXME
            ToEventId(moveChildEvent)
        );
    }

    private ChildMovedInSameContainmentEvent OnMoveChildInSameContainment(MoveChildInSameContainment moveChildEvent)
    {
        var movedChild = ToNode(moveChildEvent.MovedChild);
        var parent = (IWritableNode)movedChild.GetParent();
        var containment = parent.GetContainmentOf(movedChild);
        return new ChildMovedInSameContainmentEvent(
            moveChildEvent.NewIndex,
            movedChild,
            parent,
            containment,
            0, // TODO FIXME
            ToEventId(moveChildEvent)
        );
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
            ToTarget(addReferenceEvent.NewTarget),
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
            ToTarget(changeReferenceEvent.NewTarget),
            new ReferenceTarget(null, parent.Get(reference) as IReadableNode),
            ToEventId(changeReferenceEvent)
        );
    }

    private Reference ToReference(MetaPointer deltaReference, IReadableNode node) =>
        ToFeature<Reference>(deltaReference, node);

    private IReferenceTarget ToTarget(SerializedReferenceTarget serializedTarget)
    {
        IReadableNode? target = null;
        if (serializedTarget.Reference != null &&
            _sharedNodeMap.TryGetValue(serializedTarget.Reference, out var node))
            target = node;

        return new ReferenceTarget(serializedTarget.ResolveInfo, target);
    }

    #endregion


    private static IEventId ToEventId(ISingleDeltaCommand command) =>
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