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
using Core.M3;
using Core.Serialization;
using Message;
using Message.Event;

public class PartitionEventToDeltaEventMapper
{
    private readonly IParticipationIdProvider _participationIdProvider;
    private readonly LionWebVersions _lionWebVersion;
    private readonly ISerializerVersionSpecifics _propertySerializer;

    public PartitionEventToDeltaEventMapper(IParticipationIdProvider participationIdProvider, LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        _participationIdProvider = participationIdProvider;
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
            ChildMovedFromOtherContainmentInSameParentEvent a => OnChildMovedFromOtherContainmentInSameParent(a),
            ChildMovedInSameContainmentEvent a => OnChildMovedInSameContainment(a),
            ChildMovedAndReplacedFromOtherContainmentEvent a => OnChildMovedAndReplacedFromOtherContainment(a),
            ChildMovedAndReplacedFromOtherContainmentInSameParentEvent a => OnChildMovedAndReplacedFromOtherContainmentInSameParent(a),
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

    private PropertyAdded OnPropertyAdded(PropertyAddedEvent propertyAddedEvent) =>
        new(
            propertyAddedEvent.Node.GetId(),
            propertyAddedEvent.Property.ToMetaPointer(),
            ToDelta(propertyAddedEvent.Node, propertyAddedEvent.Property, propertyAddedEvent.NewValue)!,
            ToCommandSources(propertyAddedEvent),
            []
        );

    private PropertyDeleted OnPropertyDeleted(PropertyDeletedEvent propertyDeletedEvent) =>
        new(
            propertyDeletedEvent.Node.GetId(),
            propertyDeletedEvent.Property.ToMetaPointer(),
            ToDelta(propertyDeletedEvent.Node, propertyDeletedEvent.Property, propertyDeletedEvent.OldValue)!,
            ToCommandSources(propertyDeletedEvent),
            []
        );

    private PropertyChanged OnPropertyChanged(PropertyChangedEvent propertyChangedEvent) =>
        new(
            propertyChangedEvent.Node.GetId(),
            propertyChangedEvent.Property.ToMetaPointer(),
            ToDelta(propertyChangedEvent.Node, propertyChangedEvent.Property, propertyChangedEvent.NewValue)!,
            ToDelta(propertyChangedEvent.Node, propertyChangedEvent.Property, propertyChangedEvent.OldValue)!,
            ToCommandSources(propertyChangedEvent),
            []
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private ChildAdded OnChildAdded(ChildAddedEvent childAddedEvent) =>
        new(
            childAddedEvent.Parent.GetId(),
            ToDeltaChunk(childAddedEvent.NewChild),
            childAddedEvent.Containment.ToMetaPointer(),
            childAddedEvent.Index,
            ToCommandSources(childAddedEvent),
            []
        );

    private ChildDeleted OnChildDeleted(ChildDeletedEvent childDeletedEvent) =>
        new(
            childDeletedEvent.DeletedChild.GetId(),
            ToDescendants(childDeletedEvent.DeletedChild),
            childDeletedEvent.Parent.GetId(),
            childDeletedEvent.Containment.ToMetaPointer(),
            childDeletedEvent.Index,
            ToCommandSources(childDeletedEvent),
            []
        );

    private ChildReplaced OnChildReplaced(ChildReplacedEvent childReplacedEvent) =>
        new(
            ToDeltaChunk(childReplacedEvent.NewChild),
            childReplacedEvent.ReplacedChild.GetId(),
            ToDescendants(childReplacedEvent.ReplacedChild),
            childReplacedEvent.Parent.GetId(),
            childReplacedEvent.Containment.ToMetaPointer(),
            childReplacedEvent.Index,
            ToCommandSources(childReplacedEvent),
            []
        );

    private ChildMovedFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewParent.GetId(),
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            childMovedEvent.OldParent.GetId(),
            childMovedEvent.OldContainment.ToMetaPointer(),
            childMovedEvent.OldIndex,
            ToCommandSources(childMovedEvent),
            []
        );

    private ChildMovedAndReplacedFromOtherContainment
        OnChildMovedAndReplacedFromOtherContainment(ChildMovedAndReplacedFromOtherContainmentEvent childMovedAndReplacedEvent) =>
        new(
            childMovedAndReplacedEvent.NewParent.GetId(),
            childMovedAndReplacedEvent.NewContainment.ToMetaPointer(),
            childMovedAndReplacedEvent.NewIndex,
            childMovedAndReplacedEvent.MovedChild.GetId(),
            childMovedAndReplacedEvent.OldParent.GetId(),
            childMovedAndReplacedEvent.OldContainment.ToMetaPointer(),
            childMovedAndReplacedEvent.OldIndex,
            childMovedAndReplacedEvent.ReplacedChild.GetId(),
            ToDescendants(childMovedAndReplacedEvent.ReplacedChild),
            ToCommandSources(childMovedAndReplacedEvent), 
            []
            );

    private ChildMovedAndReplacedFromOtherContainmentInSameParent
        OnChildMovedAndReplacedFromOtherContainmentInSameParent(
            ChildMovedAndReplacedFromOtherContainmentInSameParentEvent childMovedAndReplacedEvent) =>
        new(
            childMovedAndReplacedEvent.NewContainment.ToMetaPointer(),
            childMovedAndReplacedEvent.NewIndex,
            childMovedAndReplacedEvent.MovedChild.GetId(),
            childMovedAndReplacedEvent.Parent.GetId(),
            childMovedAndReplacedEvent.OldContainment.ToMetaPointer(),
            childMovedAndReplacedEvent.OldIndex,
            childMovedAndReplacedEvent.ReplacedChild.GetId(),
            ToDescendants(childMovedAndReplacedEvent.ReplacedChild),
            ToCommandSources(childMovedAndReplacedEvent),
            []
        );
    
    private ChildMovedFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            childMovedEvent.Parent.GetId(),
            childMovedEvent.OldContainment.ToMetaPointer(),
            childMovedEvent.OldIndex,
            ToCommandSources(childMovedEvent),
            []
        );

    private ChildMovedInSameContainment OnChildMovedInSameContainment(ChildMovedInSameContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            childMovedEvent.Parent.GetId(),
            childMovedEvent.Containment.ToMetaPointer(),
            childMovedEvent.OldIndex,
            ToCommandSources(childMovedEvent),
            []
        );

    #endregion

    #region Annotations

    private AnnotationAdded OnAnnotationAdded(AnnotationAddedEvent annotationAddedEvent) =>
        new(
            annotationAddedEvent.Parent.GetId(),
            ToDeltaChunk(annotationAddedEvent.NewAnnotation),
            annotationAddedEvent.Index,
            ToCommandSources(annotationAddedEvent),
            []
        );

    private AnnotationDeleted OnAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        new(
            annotationDeletedEvent.DeletedAnnotation.GetId(),
            ToDescendants(annotationDeletedEvent.DeletedAnnotation),
            annotationDeletedEvent.Parent.GetId(),
            annotationDeletedEvent.Index,
            ToCommandSources(annotationDeletedEvent),
            []
        );

    private AnnotationMovedFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewParent.GetId(),
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            annotationMovedEvent.OldParent.GetId(),
            annotationMovedEvent.OldIndex,
            ToCommandSources(annotationMovedEvent),
            []
        );

    private AnnotationMovedInSameParent OnAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            annotationMovedEvent.Parent.GetId(),
            annotationMovedEvent.OldIndex,
            ToCommandSources(annotationMovedEvent),
            []
        );

    #endregion

    #region References

    private ReferenceAdded OnReferenceAdded(ReferenceAddedEvent referenceAddedEvent) =>
        new(
            referenceAddedEvent.Parent.GetId(),
            referenceAddedEvent.Reference.ToMetaPointer(),
            referenceAddedEvent.Index,
            referenceAddedEvent.NewTarget.Reference?.GetId(),
            referenceAddedEvent.NewTarget.ResolveInfo,
            ToCommandSources(referenceAddedEvent),
            []
        );

    private ReferenceDeleted OnReferenceDeleted(ReferenceDeletedEvent referenceDeletedEvent) =>
        new(
            referenceDeletedEvent.Parent.GetId(),
            referenceDeletedEvent.Reference.ToMetaPointer(),
            referenceDeletedEvent.Index,
            referenceDeletedEvent.DeletedTarget.Reference?.GetId(),
            referenceDeletedEvent.DeletedTarget.ResolveInfo,
            ToCommandSources(referenceDeletedEvent),
            []
        );

    private ReferenceChanged OnReferenceChanged(ReferenceChangedEvent referenceChangedEvent) =>
        new(
            referenceChangedEvent.Parent.GetId(),
            referenceChangedEvent.Reference.ToMetaPointer(),
            referenceChangedEvent.Index,
            referenceChangedEvent.NewTarget.Reference?.GetId(),
            referenceChangedEvent.NewTarget.ResolveInfo,
            referenceChangedEvent.OldTarget.Reference?.GetId(),
            referenceChangedEvent.OldTarget.ResolveInfo,
            ToCommandSources(referenceChangedEvent),
            []
        );

    #endregion

    private DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }

    private TargetNode[] ToDescendants(IReadableNode node) =>
        M1Extensions.Descendants(node, false, true).Select(n => n.GetId()).ToArray();

    private CommandSource[] ToCommandSources(IEvent internalEvent)
    {
        ParticipationId participationId;
        EventId commandId;
        if (internalEvent.EventId is ParticipationEventId pei)
        {
            participationId = pei.ParticipationId;
            commandId = pei.CommandId;
        } else
        {
            participationId = _participationIdProvider.ParticipationId;
            commandId = internalEvent.EventId.ToString();
        }
        return [new CommandSource(participationId, commandId)];
    }
}

public interface IParticipationIdProvider
{
    ParticipationId ParticipationId { get; }
}