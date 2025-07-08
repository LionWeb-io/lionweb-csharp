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

namespace LionWeb.Protocol.Delta.Client;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Partition;
using Core.M2;
using Core.M3;
using Core.Serialization;
using Message;
using Message.Event;

public class DeltaEventToPartitionEventMapper
{
    private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;
    private readonly Dictionary<CompressedMetaPointer, IKeyed> _sharedKeyedMap;
    private readonly DeserializerBuilder _deserializerBuilder;

    public DeltaEventToPartitionEventMapper(
        Dictionary<NodeId, IReadableNode> sharedNodeMap,
        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _deserializerBuilder = deserializerBuilder;
    }

    public IPartitionEvent Map(IDeltaEvent deltaEvent) =>
        deltaEvent switch
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

    #region Properties

    private PropertyAddedEvent OnPropertyAdded(PropertyAdded propertyAddedEvent)
    {
        var parent = ToNode(propertyAddedEvent.Node);
        var property = ToProperty(propertyAddedEvent.Property, parent);
        return new PropertyAddedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, propertyAddedEvent.NewValue),
            ToEventId(propertyAddedEvent)
        );
    }

    private PropertyDeletedEvent OnPropertyDeleted(PropertyDeleted propertyDeletedEvent)
    {
        var parent = ToNode(propertyDeletedEvent.Node);
        var property = ToProperty(propertyDeletedEvent.Property, parent);
        return new PropertyDeletedEvent(
            parent,
            property,
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToEventId(propertyDeletedEvent)
        );
    }

    private PropertyChangedEvent OnPropertyChanged(PropertyChanged propertyChangedEvent)
    {
        var parent = ToNode(propertyChangedEvent.Node);
        var property = ToProperty(propertyChangedEvent.Property, parent);
        return new PropertyChangedEvent(
            parent,
            property,
            ToPropertyValue(parent, property, propertyChangedEvent.NewValue),
            parent.Get(property) ?? throw new UnsetFeatureException(property),
            ToEventId(propertyChangedEvent)
        );
    }

    private Property ToProperty(MetaPointer deltaProperty, IReadableNode node) =>
        ToFeature<Property>(deltaProperty, node);

    private SemanticPropertyValue ToPropertyValue(IReadableNode node, Property property, PropertyValue value) =>
        _deserializerBuilder.Build().VersionSpecifics.ConvertDatatype(node, property, property.Type, value) ??
        throw new InvalidValueException(property, value);

    #endregion

    #region Children

    private ChildAddedEvent OnChildAdded(ChildAdded childAddedEvent)
    {
        var parent = ToNode(childAddedEvent.Parent);
        var containment = ToContainment(childAddedEvent.Containment, parent);
        return new ChildAddedEvent(
            parent,
            Deserialize(childAddedEvent.NewChild),
            containment,
            childAddedEvent.Index,
            ToEventId(childAddedEvent)
        );
    }

    private ChildDeletedEvent OnChildDeleted(ChildDeleted childDeletedEvent)
    {
        var parent = ToNode(childDeletedEvent.Parent);
        var containment = ToContainment(childDeletedEvent.Containment, parent);
        return new ChildDeletedEvent(
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[childDeletedEvent.Index],
            parent,
            containment,
            childDeletedEvent.Index,
            ToEventId(childDeletedEvent)
        );
    }

    private ChildReplacedEvent OnChildReplaced(ChildReplaced childReplacedEvent)
    {
        var parent = ToNode(childReplacedEvent.Parent);
        var containment = ToContainment(childReplacedEvent.Containment, parent);
        return new ChildReplacedEvent(
            Deserialize(childReplacedEvent.NewChild),
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList()[childReplacedEvent.Index],
            parent,
            containment,
            childReplacedEvent.Index,
            ToEventId(childReplacedEvent)
        );
    }

    private ChildMovedFromOtherContainmentEvent OnChildMovedFromOtherContainment(ChildMovedFromOtherContainment childMovedEvent)
    {
        var movedChild = ToNode(childMovedEvent.MovedChild);
        var oldParent = ToNode(childMovedEvent.OldParent);
        var newParent = ToNode(childMovedEvent.NewParent);
        var oldContainment = ToContainment(childMovedEvent.OldContainment, oldParent);
        var newContainment = ToContainment(childMovedEvent.NewContainment, newParent);
        return new ChildMovedFromOtherContainmentEvent(
            newParent,
            newContainment,
            childMovedEvent.NewIndex,
            movedChild,
            oldParent,
            oldContainment,
            childMovedEvent.OldIndex,
            ToEventId(childMovedEvent)
        );
    }

    private ChildMovedFromOtherContainmentInSameParentEvent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParent childMovedEvent)
    {
        var parent = ToNode(childMovedEvent.Parent);
        var movedChild = ToNode(childMovedEvent.MovedChild);
        var oldContainment = ToContainment(childMovedEvent.OldContainment, parent);
        var newContainment = ToContainment(childMovedEvent.NewContainment, parent);
        return new ChildMovedFromOtherContainmentInSameParentEvent(
            newContainment,
            childMovedEvent.NewIndex,
            movedChild,
            parent,
            oldContainment,
            childMovedEvent.OldIndex,
            ToEventId(childMovedEvent)
        );
    }

    private ChildMovedInSameContainmentEvent OnChildMovedInSameContainment(ChildMovedInSameContainment childMovedEvent)
    {
        var parent = ToNode(childMovedEvent.Parent);
        var movedChild = ToNode(childMovedEvent.MovedChild);
        var containment = ToContainment(childMovedEvent.Containment, parent);
        return new ChildMovedInSameContainmentEvent(
            childMovedEvent.NewIndex,
            movedChild,
            parent,
            containment,
            childMovedEvent.OldIndex,
            ToEventId(childMovedEvent)
        );
    }

    private Containment ToContainment(MetaPointer deltaContainment, IReadableNode node) =>
        ToFeature<Containment>(deltaContainment, node);

    #endregion

    #region Annotations

    private AnnotationAddedEvent OnAnnotationAdded(AnnotationAdded annotationAddedEvent)
    {
        var parent = ToNode(annotationAddedEvent.Parent);
        return new AnnotationAddedEvent(
            parent,
            Deserialize(annotationAddedEvent.NewAnnotation),
            annotationAddedEvent.Index,
            ToEventId(annotationAddedEvent)
        );
    }

    private AnnotationDeletedEvent OnAnnotationDeleted(AnnotationDeleted annotationDeletedEvent)
    {
        var parent = ToNode(annotationDeletedEvent.Parent);
        var deletedAnnotation = parent.GetAnnotations()[annotationDeletedEvent.Index];
        return new AnnotationDeletedEvent(
            deletedAnnotation as IWritableNode ?? throw new InvalidValueException(null, deletedAnnotation),
            parent,
            annotationDeletedEvent.Index,
            ToEventId(annotationDeletedEvent)
        );
    }

    private AnnotationMovedFromOtherParentEvent OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParent annotationMovedEvent)
    {
        var oldParent = ToNode(annotationMovedEvent.OldParent);
        var newParent = ToNode(annotationMovedEvent.NewParent);
        var movedAnnotation = ToNode(annotationMovedEvent.MovedAnnotation);
        return new AnnotationMovedFromOtherParentEvent(
            newParent,
            annotationMovedEvent.NewIndex,
            movedAnnotation,
            oldParent,
            annotationMovedEvent.OldIndex,
            ToEventId(annotationMovedEvent)
        );
    }

    private AnnotationMovedInSameParentEvent OnAnnotationMovedInSameParent(AnnotationMovedInSameParent annotationMovedEvent)
    {
        var parent = ToNode(annotationMovedEvent.Parent);
        var movedAnnotation = ToNode(annotationMovedEvent.MovedAnnotation);
        return new AnnotationMovedInSameParentEvent(
            annotationMovedEvent.NewIndex,
            movedAnnotation,
            parent,
            annotationMovedEvent.OldIndex,
            ToEventId(annotationMovedEvent)
        );
    }

    #endregion

    #region References

    private ReferenceAddedEvent OnReferenceAdded(ReferenceAdded referenceAddedEvent)
    {
        var parent = ToNode(referenceAddedEvent.Parent);
        var reference = ToReference(referenceAddedEvent.Reference, parent);
        return new ReferenceAddedEvent(
            parent,
            reference,
            referenceAddedEvent.Index,
            ToTarget(referenceAddedEvent.NewTarget, referenceAddedEvent.NewResolveInfo),
            ToEventId(referenceAddedEvent)
        );
    }

    private ReferenceDeletedEvent OnReferenceDeleted(ReferenceDeleted referenceDeletedEvent)
    {
        var parent = ToNode(referenceDeletedEvent.Parent);
        var reference = ToReference(referenceDeletedEvent.Reference, parent);
        return new ReferenceDeletedEvent(
            parent,
            reference,
            referenceDeletedEvent.Index,
            ToTarget(referenceDeletedEvent.DeletedTarget, referenceDeletedEvent.DeletedResolveInfo),
            ToEventId(referenceDeletedEvent)
        );
    }

    private ReferenceChangedEvent OnReferenceChanged(ReferenceChanged referenceChangedEvent)
    {
        var parent = ToNode(referenceChangedEvent.Parent);
        var reference = ToReference(referenceChangedEvent.Reference, parent);
        return new ReferenceChangedEvent(
            parent,
            reference,
            referenceChangedEvent.Index,
            ToTarget(referenceChangedEvent.NewTarget,  referenceChangedEvent.NewResolveInfo),
            ToTarget(referenceChangedEvent.OldTarget,  referenceChangedEvent.OldResolveInfo),
            ToEventId(referenceChangedEvent)
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


    private static IEventId ToEventId(IDeltaEvent deltaEvent) =>
        new ParticipationEventId(deltaEvent.InternalParticipationId,
            string.Join("_", deltaEvent.OriginCommands.Select(c => c.CommandId)));

    private IWritableNode ToNode(TargetNode nodeId)
    {
        if (_sharedNodeMap.TryGetValue(nodeId, out var node) && node is IWritableNode w)
            return w;

        // TODO change to correct exception 
        throw new NotImplementedException(nodeId);
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