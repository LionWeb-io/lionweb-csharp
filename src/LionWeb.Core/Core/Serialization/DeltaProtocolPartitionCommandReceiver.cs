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
using M2;
using M3;
using TargetNode = NodeId;
using SemanticPropertyValue = object;

public class DeltaProtocolPartitionCommandReceiver
{
    private readonly PartitionEventHandler _eventHandler;
    private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;
    private readonly Dictionary<CompressedMetaPointer, IKeyed> _sharedKeyedMap;
    private readonly DeserializerBuilder _deserializerBuilder;

    public DeltaProtocolPartitionCommandReceiver(
        PartitionEventHandler eventHandler,
        Dictionary<NodeId, IReadableNode> sharedNodeMap,
        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _eventHandler = eventHandler;
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _deserializerBuilder = deserializerBuilder;
    }


    public void Receive(IDeltaCommand deltaCommand)
    {
        M1.Event.Partition.IPartitionEvent partitionCommand = deltaCommand switch
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
            _ => throw new NotImplementedException(deltaCommand.GetType().Name)
        };
        
        _eventHandler.Raise(partitionCommand);
    }

    #region Properties

    private PropertyAddedEvent OnAddProperty(AddProperty @event)
    {
        var parent = ToNode(@event.Parent);
        var property = ToProperty(@event.Property, parent);
        return new PropertyAddedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, @event.NewValue),
            ToCommandId(@event)
        );
    }

    private PropertyDeletedEvent OnDeleteProperty(DeleteProperty @event)
    {
        var parent = ToNode(@event.Parent);
        var property = ToProperty(@event.Property, parent);
        return new PropertyDeletedEvent(
            parent,
            property,
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToCommandId(@event)
        );
    }

    private PropertyChangedEvent OnChangeProperty(ChangeProperty @event)
    {
        var parent = ToNode(@event.Parent);
        var property = ToProperty(@event.Property, parent);
        return new PropertyChangedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, @event.NewValue),
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToCommandId(@event)
        );
    }

    private Property ToProperty(MetaPointer deltaProperty, IReadableNode node) =>
        ToFeature<Property>(deltaProperty, node);

    private SemanticPropertyValue ToPropertyValue(IReadableNode node, Property property, PropertyValue value) =>
        _deserializerBuilder.Build().VersionSpecifics.ConvertDatatype(node, property, property.Type, value) ??
        throw new InvalidValueException(property, value);

    #endregion

    #region Children

    private ChildAddedEvent OnAddChild(AddChild @event)
    {
        var parent = ToNode(@event.Parent);
        var containment = ToContainment(@event.Containment, parent);
        return new ChildAddedEvent(
            parent,
            Deserialize(@event.NewChild),
            containment,
            @event.Index,
            ToCommandId(@event)
        );
    }

    private ChildDeletedEvent OnDeleteChild(DeleteChild @event)
    {
        var parent = ToNode(@event.Parent);
        var containment = ToContainment(@event.Containment, parent);
        return new ChildDeletedEvent(
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[@event.Index],
            parent,
            containment,
            @event.Index,
            ToCommandId(@event)
        );
    }

    private ChildReplacedEvent OnReplaceChild(ReplaceChild @event)
    {
        var parent = ToNode(@event.Parent);
        var containment = ToContainment(@event.Containment, parent);
        return new ChildReplacedEvent(
            Deserialize(@event.NewChild),
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[@event.Index],
            parent,
            containment,
            @event.Index,
            ToCommandId(@event)
        );
    }

    private ChildMovedFromOtherContainmentEvent OnMoveChildFromOtherContainment(MoveChildFromOtherContainment @event)
    {
        var movedChild = ToNode(@event.MovedChild);
        var oldParent = (IWritableNode) movedChild.GetParent();
        var newParent = ToNode(@event.NewParent);
        var oldContainment = oldParent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(@event.NewContainment, newParent);
        return new ChildMovedFromOtherContainmentEvent(
            newParent,
            newContainment,
            @event.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            0, // TODO FIXME
            ToCommandId(@event)
        );
    }

    private ChildMovedFromOtherContainmentInSameParentEvent OnMoveChildFromOtherContainmentInSameParent(
        MoveChildFromOtherContainmentInSameParent @event)
    {
        var movedChild = ToNode(@event.MovedChild);
        var parent = (IWritableNode) movedChild.GetParent();
        var oldContainment = parent.GetContainmentOf(movedChild);
        var newContainment = ToContainment(@event.NewContainment, parent);
        return new ChildMovedFromOtherContainmentInSameParentEvent(
            newContainment,
            @event.NewIndex,
            movedChild,
            parent,
            oldContainment,
            0, // TODO FIXME
            ToCommandId(@event)
        );
    }

    private ChildMovedInSameContainmentEvent OnMoveChildInSameContainment(MoveChildInSameContainment @event)
    {
        var movedChild = ToNode(@event.MovedChild);
        var parent = (IWritableNode) movedChild.GetParent();
        var containment = parent.GetContainmentOf(movedChild);
        return new ChildMovedInSameContainmentEvent(
            @event.NewIndex,
            movedChild,
            parent,
            containment,
            0, // TODO FIXME
            ToCommandId(@event)
        );
    }

    private Containment ToContainment(MetaPointer deltaContainment, IReadableNode node) =>
        ToFeature<Containment>(deltaContainment, node);

    #endregion

    #region Annotations

    private AnnotationAddedEvent OnAddAnnotation(AddAnnotation @event)
    {
        var parent = ToNode(@event.Parent);
        return new AnnotationAddedEvent(
            parent,
            Deserialize(@event.NewAnnotation),
            @event.Index,
            ToCommandId(@event)
        );
    }

    private AnnotationDeletedEvent OnDeleteAnnotation(DeleteAnnotation @event)
    {
        var parent = ToNode(@event.Parent);
        var deletedAnnotation = parent.GetAnnotations()[@event.Index];
        return new AnnotationDeletedEvent(
            deletedAnnotation as IWritableNode ?? throw new InvalidValueException(null, deletedAnnotation),
            parent,
            @event.Index,
            ToCommandId(@event)
        );
    }

    private AnnotationMovedFromOtherParentEvent OnMoveAnnotationFromOtherParent(MoveAnnotationFromOtherParent @event)
    {
        var movedAnnotation = ToNode(@event.MovedAnnotation);
        var oldParent = (IWritableNode) movedAnnotation.GetParent();
        var newParent = ToNode(@event.NewParent);
        return new AnnotationMovedFromOtherParentEvent(
            newParent,
            @event.NewIndex,
            movedAnnotation,
            oldParent,
            0, // TODO FIXME
            ToCommandId(@event)
        );
    }

    private AnnotationMovedInSameParentEvent OnMoveAnnotationInSameParent(MoveAnnotationInSameParent @event)
    {
        var movedAnnotation = ToNode(@event.MovedAnnotation);
        var parent = (IWritableNode) movedAnnotation.GetParent();
        return new AnnotationMovedInSameParentEvent(
            @event.NewIndex,
            movedAnnotation,
            parent,
            0, // TODO FIXME
            ToCommandId(@event)
        );
    }

    #endregion

    #region References

    private ReferenceAddedEvent OnAddReference(AddReference @event)
    {
        var parent = ToNode(@event.Parent);
        var reference = ToReference(@event.Reference, parent);
        return new ReferenceAddedEvent(
            parent,
            reference,
            @event.Index,
            ToTarget(@event.NewTarget),
            ToCommandId(@event)
        );
    }

    private ReferenceDeletedEvent OnDeleteReference(DeleteReference @event)
    {
        var parent = ToNode(@event.Parent);
        var reference = ToReference(@event.Reference, parent);
        return new ReferenceDeletedEvent(
            parent,
            reference,
            @event.Index,
            new ReferenceTarget(null, parent.Get(reference) as IReadableNode),
            ToCommandId(@event)
        );
    }

    private ReferenceChangedEvent OnChangeReference(ChangeReference @event)
    {
        var parent = ToNode(@event.Parent);
        var reference = ToReference(@event.Reference, parent);
        return new ReferenceChangedEvent(
            parent,
            reference,
            @event.Index,
            ToTarget(@event.NewTarget),
            new ReferenceTarget(null, parent.Get(reference) as IReadableNode),
            ToCommandId(@event)
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


    private static CommandId ToCommandId(ISingleDeltaCommand command) =>
        command.CommandId;

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