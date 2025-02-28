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

public class DeltaProtocolPartitionEventReceiver
{
    private readonly PartitionEventHandler _eventHandler;
    private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;
    private readonly Dictionary<CompressedMetaPointer, IKeyed> _sharedKeyedMap;
    private readonly DeserializerBuilder _deserializerBuilder;

    public DeltaProtocolPartitionEventReceiver(
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


    public void Receive(IDeltaEvent deltaEvent)
    {
        M1.Event.Partition.IPartitionEvent partitionEvent = deltaEvent switch
        {
            PropertyAdded a => OnPropertyAdded(a),
            PropertyDeleted a => OnPropertyDeleted(a),
            PropertyChanged a => OnPropertyChanged(a),
            ChildAdded a => OnChildAdded(a),
            ChildDeleted a => OnChildDeleted(a),
            ChildReplaced a => OnChildReplaced(a),
            ChildMovedFromOtherContainment a => OnChildMovedFromOtherContainment(a),
            ChildMovedFromOtherContainmentInSameParent a => OnChildMovedFromOtherContainmentInSameParent(a),
            ChildMovedInSameContainment a => OnChildMovedInSameContainment(a),
            AnnotationAdded a => OnAnnotationAdded(a),
            AnnotationDeleted a => OnAnnotationDeleted(a),
            AnnotationMovedFromOtherParent a => OnAnnotationMovedFromOtherParent(a),
            AnnotationMovedInSameParent a => OnAnnotationMovedInSameParent(a),
            ReferenceAdded a => OnReferenceAdded(a),
            ReferenceDeleted a => OnReferenceDeleted(a),
            ReferenceChanged a => OnReferenceChanged(a),
            _ => throw new NotImplementedException(deltaEvent.GetType().Name)
        };

        _eventHandler.Raise(partitionEvent);
    }

    #region Properties

    private PropertyAddedEvent OnPropertyAdded(PropertyAdded @event)
    {
        var parent = ToNode(@event.Property.Parent);
        var property = ToProperty(@event.Property, parent);
        return new PropertyAddedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, @event.NewValue),
            ToEventId(@event)
        );
    }

    private PropertyDeletedEvent OnPropertyDeleted(PropertyDeleted @event)
    {
        var parent = ToNode(@event.Property.Parent);
        var property = ToProperty(@event.Property, parent);
        return new PropertyDeletedEvent(
            parent,
            property,
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToEventId(@event)
        );
    }

    private PropertyChangedEvent OnPropertyChanged(PropertyChanged @event)
    {
        var parent = ToNode(@event.Property.Parent);
        var property = ToProperty(@event.Property, parent);
        return new PropertyChangedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, @event.NewValue),
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToEventId(@event)
        );
    }

    private Property ToProperty(DeltaProperty deltaProperty, IReadableNode node) =>
        ToFeature<Property>(deltaProperty.Property, node);

    private SemanticPropertyValue ToPropertyValue(IReadableNode node, Property property, PropertyValue value) =>
        _deserializerBuilder.Build().VersionSpecifics.ConvertDatatype(node, property, property.Type, value) ??
        throw new InvalidValueException(property, value);

    #endregion

    #region Children

    private ChildAddedEvent OnChildAdded(ChildAdded @event)
    {
        var parent = ToNode(@event.Parent);
        var containment = ToContainment(@event.Containment, parent);
        return new ChildAddedEvent(
            parent,
            Deserialize(@event.NewChild),
            containment,
            @event.Containment.Index,
            ToEventId(@event)
        );
    }

    private ChildDeletedEvent OnChildDeleted(ChildDeleted @event)
    {
        var parent = ToNode(@event.Parent);
        var containment = ToContainment(@event.Containment, parent);
        return new ChildDeletedEvent(
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[@event.Containment.Index],
            parent,
            containment,
            @event.Containment.Index,
            ToEventId(@event)
        );
    }

    private ChildReplacedEvent OnChildReplaced(ChildReplaced @event)
    {
        var parent = ToNode(@event.Parent);
        var containment = ToContainment(@event.Containment, parent);
        return new ChildReplacedEvent(
            Deserialize(@event.NewChild),
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[@event.Containment.Index],
            parent,
            containment,
            @event.Containment.Index,
            ToEventId(@event)
        );
    }

    private ChildMovedFromOtherContainmentEvent OnChildMovedFromOtherContainment(ChildMovedFromOtherContainment @event)
    {
        var movedChild = ToNode(@event.MovedChild);
        var oldParent = ToNode(@event.OldContainment.Parent);
        var newParent = ToNode(@event.NewContainment.Parent);
        var oldContainment = ToContainment(@event.OldContainment, oldParent);
        var newContainment = ToContainment(@event.NewContainment, newParent);
        return new ChildMovedFromOtherContainmentEvent(
            newParent,
            newContainment,
            @event.NewContainment.Index,
            movedChild,
            oldParent,
            oldContainment,
            @event.OldContainment.Index,
            ToEventId(@event)
        );
    }

    private ChildMovedFromOtherContainmentInSameParentEvent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParent @event)
    {
        var parent = ToNode(@event.Parent);
        var movedChild = ToNode(@event.MovedChild);
        var oldContainment = ToContainment(@event.OldContainment, parent);
        var newContainment = ToContainment(@event.NewContainment, parent);
        return new ChildMovedFromOtherContainmentInSameParentEvent(
            newContainment,
            @event.NewIndex,
            movedChild,
            parent,
            oldContainment,
            @event.OldIndex,
            ToEventId(@event)
        );
    }

    private ChildMovedInSameContainmentEvent OnChildMovedInSameContainment(ChildMovedInSameContainment @event)
    {
        var parent = ToNode(@event.Parent);
        var movedChild = ToNode(@event.MovedChild);
        var containment = ToContainment(@event.Containment, parent);
        return new ChildMovedInSameContainmentEvent(
            @event.NewIndex,
            movedChild,
            parent,
            containment,
            @event.OldIndex,
            ToEventId(@event)
        );
    }

    private Containment ToContainment(DeltaContainment deltaContainment, IReadableNode node) =>
        ToContainment(deltaContainment.Containment, node);

    private Containment ToContainment(MetaPointer deltaContainment, IReadableNode node) =>
        ToFeature<Containment>(deltaContainment, node);

    #endregion

    #region Annotations

    private AnnotationAddedEvent OnAnnotationAdded(AnnotationAdded @event)
    {
        var parent = ToNode(@event.Parent);
        return new AnnotationAddedEvent(
            parent,
            Deserialize(@event.NewAnnotation),
            @event.Parent.Index,
            ToEventId(@event)
        );
    }

    private AnnotationDeletedEvent OnAnnotationDeleted(AnnotationDeleted @event)
    {
        var parent = ToNode(@event.Parent);
        var deletedAnnotation = parent.GetAnnotations()[@event.Parent.Index];
        return new AnnotationDeletedEvent(
            deletedAnnotation as IWritableNode ?? throw new InvalidValueException(null, deletedAnnotation),
            parent,
            @event.Parent.Index,
            ToEventId(@event)
        );
    }

    private AnnotationMovedFromOtherParentEvent OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParent @event)
    {
        var oldParent = ToNode(@event.OldParent);
        var newParent = ToNode(@event.NewParent);
        var movedAnnotation = ToNode(@event.MovedAnnotation);
        return new AnnotationMovedFromOtherParentEvent(
            newParent,
            @event.NewParent.Index,
            movedAnnotation,
            oldParent,
            @event.OldParent.Index,
            ToEventId(@event)
        );
    }

    private AnnotationMovedInSameParentEvent OnAnnotationMovedInSameParent(AnnotationMovedInSameParent @event)
    {
        var parent = ToNode(@event.Parent);
        var movedAnnotation = ToNode(@event.MovedAnnotation);
        return new AnnotationMovedInSameParentEvent(
            @event.NewIndex,
            movedAnnotation,
            parent,
            @event.OldIndex,
            ToEventId(@event)
        );
    }

    private IWritableNode ToNode(DeltaAnnotation deltaAnnotation) =>
        ToNode(deltaAnnotation.Parent);

    #endregion

    #region References

    private ReferenceAddedEvent OnReferenceAdded(ReferenceAdded @event)
    {
        var parent = ToNode(@event.Parent);
        var reference = ToReference(@event.Reference, parent);
        return new ReferenceAddedEvent(
            parent,
            reference,
            @event.Reference.Index,
            ToTarget(@event.NewTarget),
            ToEventId(@event)
        );
    }

    private ReferenceDeletedEvent OnReferenceDeleted(ReferenceDeleted @event)
    {
        var parent = ToNode(@event.Parent);
        var reference = ToReference(@event.Reference, parent);
        return new ReferenceDeletedEvent(
            parent,
            reference,
            @event.Reference.Index,
            ToTarget(@event.DeletedTarget),
            ToEventId(@event)
        );
    }

    private ReferenceChangedEvent OnReferenceChanged(ReferenceChanged @event)
    {
        var parent = ToNode(@event.Parent);
        var reference = ToReference(@event.Reference, parent);
        return new ReferenceChangedEvent(
            parent,
            reference,
            @event.Reference.Index,
            ToTarget(@event.NewTarget),
            ToTarget(@event.ReplacedTarget),
            ToEventId(@event)
        );
    }

    private Reference ToReference(DeltaReference deltaReference, IReadableNode node) =>
        ToFeature<Reference>(deltaReference.Reference, node);

    private IReferenceTarget ToTarget(SerializedReferenceTarget serializedTarget)
    {
        IReadableNode? target = null;
        if (serializedTarget.Reference != null &&
            _sharedNodeMap.TryGetValue(serializedTarget.Reference, out var node))
            target = node;

        return new ReferenceTarget(serializedTarget.ResolveInfo, target);
    }

    #endregion


    private static EventId ToEventId(ISingleDeltaEvent @event) =>
        string.Join("_", @event.OriginCommands.Select(c => c.Source));

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