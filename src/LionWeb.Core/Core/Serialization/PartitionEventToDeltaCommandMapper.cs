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

    private AddProperty OnPropertyAdded(PropertyAddedEvent propertyAddedEvent) =>
        new(
            propertyAddedEvent.Node.GetId(),
            propertyAddedEvent.Property.ToMetaPointer(),
            ToDelta(propertyAddedEvent.Node, propertyAddedEvent.Property, propertyAddedEvent.NewValue)!,
            ToCommandId(propertyAddedEvent),
            null
        );

    private DeleteProperty OnPropertyDeleted(PropertyDeletedEvent propertyDeletedEvent) =>
        new(
            propertyDeletedEvent.Node.GetId(),
            propertyDeletedEvent.Property.ToMetaPointer(),
            ToCommandId(propertyDeletedEvent),
            null
        );

    private ChangeProperty OnPropertyChanged(PropertyChangedEvent propertyChangedEvent) =>
        new(
            propertyChangedEvent.Node.GetId(),
            propertyChangedEvent.Property.ToMetaPointer(),
            ToDelta(propertyChangedEvent.Node, propertyChangedEvent.Property, propertyChangedEvent.NewValue)!,
            ToCommandId(propertyChangedEvent),
            null
        );

    private PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    #endregion

    #region Children

    private AddChild OnChildAdded(ChildAddedEvent childAddedEvent) =>
        new(
            childAddedEvent.Parent.GetId(),
            childAddedEvent.Containment.ToMetaPointer(),
            childAddedEvent.Index,
            ToDeltaChunk(childAddedEvent.NewChild),
            ToCommandId(childAddedEvent),
            null
        );

    private DeleteChild OnChildDeleted(ChildDeletedEvent childDeletedEvent) =>
        new(
            childDeletedEvent.Parent.GetId(),
            childDeletedEvent.Containment.ToMetaPointer(),
            childDeletedEvent.Index,
            ToCommandId(childDeletedEvent),
            null
        );

    private ReplaceChild OnChildReplaced(ChildReplacedEvent childReplacedEvent) =>
        new(
            childReplacedEvent.Parent.GetId(),
            childReplacedEvent.Containment.ToMetaPointer(),
            childReplacedEvent.Index,
            ToDeltaChunk(childReplacedEvent.NewChild),
            ToCommandId(childReplacedEvent),
            null
        );

    private MoveChildFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewParent.GetId(),
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            ToCommandId(childMovedEvent),
            null
        );

    private MoveChildFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            ToCommandId(childMovedEvent),
            null
        );

    private MoveChildInSameContainment OnChildMovedInSameContainment(ChildMovedInSameContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            ToCommandId(childMovedEvent),
            null
        );

    #endregion

    #region Annotations

    private AddAnnotation OnAnnotationAdded(AnnotationAddedEvent annotationAddedEvent) =>
        new(
            annotationAddedEvent.Parent.GetId(),
            annotationAddedEvent.Index,
            ToDeltaChunk(annotationAddedEvent.NewAnnotation),
            ToCommandId(annotationAddedEvent),
            null
        );

    private DeleteAnnotation OnAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        new(
            annotationDeletedEvent.Parent.GetId(),
            annotationDeletedEvent.Index,
            ToCommandId(annotationDeletedEvent),
            null
        );

    private MoveAnnotationFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewParent.GetId(),
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            ToCommandId(annotationMovedEvent),
            null
        );

    private MoveAnnotationInSameParent OnAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            ToCommandId(annotationMovedEvent),
            null
        );

    #endregion

    #region References

    private AddReference OnReferenceAdded(ReferenceAddedEvent referenceAddedEvent) =>
        new(
            referenceAddedEvent.Parent.GetId(),
            referenceAddedEvent.Reference.ToMetaPointer(),
            referenceAddedEvent.Index,
            ToDelta(referenceAddedEvent.NewTarget),
            ToCommandId(referenceAddedEvent),
            null
        );

    private DeleteReference OnReferenceDeleted(ReferenceDeletedEvent referenceDeletedEvent) =>
        new(
            referenceDeletedEvent.Parent.GetId(),
            referenceDeletedEvent.Reference.ToMetaPointer(),
            referenceDeletedEvent.Index,
            ToCommandId(referenceDeletedEvent),
            null
        );

    private ChangeReference OnReferenceChanged(ReferenceChangedEvent referenceChangedEvent) =>
        new(
            referenceChangedEvent.Parent.GetId(),
            referenceChangedEvent.Reference.ToMetaPointer(),
            referenceChangedEvent.Index,
            ToDelta(referenceChangedEvent.NewTarget),
            ToCommandId(referenceChangedEvent),
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