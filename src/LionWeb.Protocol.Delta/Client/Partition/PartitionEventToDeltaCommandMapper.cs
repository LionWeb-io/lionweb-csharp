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

namespace LionWeb.Protocol.Delta.Client.Partition;

using Core;
using Core.M1.Event.Partition;
using Core.Serialization;
using Message.Command;

public class PartitionEventToDeltaCommandMapper(ICommandIdProvider commandIdProvider, LionWebVersions lionWebVersion)
    : EventToDeltaCommandMapperBase(commandIdProvider, lionWebVersion)
{
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

    private AddProperty OnPropertyAdded(PropertyAddedEvent propertyAddedEvent) =>
        new(
            propertyAddedEvent.Node.GetId(),
            propertyAddedEvent.Property.ToMetaPointer(),
            ToDelta(propertyAddedEvent.Node, propertyAddedEvent.Property, propertyAddedEvent.NewValue)!,
            ToCommandId(propertyAddedEvent),
            []
        );

    private DeleteProperty OnPropertyDeleted(PropertyDeletedEvent propertyDeletedEvent) =>
        new(
            propertyDeletedEvent.Node.GetId(),
            propertyDeletedEvent.Property.ToMetaPointer(),
            ToCommandId(propertyDeletedEvent),
            []
        );

    private ChangeProperty OnPropertyChanged(PropertyChangedEvent propertyChangedEvent) =>
        new(
            propertyChangedEvent.Node.GetId(),
            propertyChangedEvent.Property.ToMetaPointer(),
            ToDelta(propertyChangedEvent.Node, propertyChangedEvent.Property, propertyChangedEvent.NewValue)!,
            ToCommandId(propertyChangedEvent),
            []
        );

    #endregion

    #region Children

    private AddChild OnChildAdded(ChildAddedEvent childAddedEvent) =>
        new(
            childAddedEvent.Parent.GetId(),
            ToDeltaChunk(childAddedEvent.NewChild),
            childAddedEvent.Containment.ToMetaPointer(),
            childAddedEvent.Index,
            ToCommandId(childAddedEvent),
            []
        );

    private DeleteChild OnChildDeleted(ChildDeletedEvent childDeletedEvent) =>
        new(
            childDeletedEvent.Parent.GetId(),
            childDeletedEvent.Containment.ToMetaPointer(),
            childDeletedEvent.Index,
            childDeletedEvent.DeletedChild.GetId(),
            ToCommandId(childDeletedEvent),
            []
        );

    private ReplaceChild OnChildReplaced(ChildReplacedEvent childReplacedEvent) =>
        new(
            ToDeltaChunk(childReplacedEvent.NewChild),
            childReplacedEvent.Parent.GetId(),
            childReplacedEvent.Containment.ToMetaPointer(),
            childReplacedEvent.Index,
            childReplacedEvent.ReplacedChild.GetId(),
            ToCommandId(childReplacedEvent),
            []
        );

    private MoveChildFromOtherContainment
        OnChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewParent.GetId(),
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            ToCommandId(childMovedEvent),
            []
        );

    private MoveAndReplaceChildFromOtherContainment 
        OnChildMovedAndReplacedFromOtherContainment(ChildMovedAndReplacedFromOtherContainmentEvent childMovedAndReplacedEvent) => 
        new(
            childMovedAndReplacedEvent.NewParent.GetId(),
            childMovedAndReplacedEvent.NewContainment.ToMetaPointer(),
            childMovedAndReplacedEvent.NewIndex,
            childMovedAndReplacedEvent.ReplacedChild.GetId(),
            childMovedAndReplacedEvent.MovedChild.GetId(),
            ToCommandId(childMovedAndReplacedEvent),
            []
        );

    private MoveAndReplaceChildFromOtherContainmentInSameParent
        OnChildMovedAndReplacedFromOtherContainmentInSameParent(
            ChildMovedAndReplacedFromOtherContainmentInSameParentEvent childMovedAndReplacedEvent) =>
        new(
            childMovedAndReplacedEvent.NewContainment.ToMetaPointer(),
            childMovedAndReplacedEvent.NewIndex,
            childMovedAndReplacedEvent.ReplacedChild.GetId(),
            childMovedAndReplacedEvent.MovedChild.GetId(),
            ToCommandId(childMovedAndReplacedEvent),
            []
        );
    
    private MoveChildFromOtherContainmentInSameParent OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewContainment.ToMetaPointer(),
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            ToCommandId(childMovedEvent),
            []
        );

    private MoveChildInSameContainment OnChildMovedInSameContainment(ChildMovedInSameContainmentEvent childMovedEvent) =>
        new(
            childMovedEvent.NewIndex,
            childMovedEvent.MovedChild.GetId(),
            ToCommandId(childMovedEvent),
            []
        );

    #endregion

    #region Annotations

    private AddAnnotation OnAnnotationAdded(AnnotationAddedEvent annotationAddedEvent) =>
        new(
            annotationAddedEvent.Parent.GetId(),
            ToDeltaChunk(annotationAddedEvent.NewAnnotation),
            annotationAddedEvent.Index,
            ToCommandId(annotationAddedEvent),
            []
        );

    private DeleteAnnotation OnAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        new(
            annotationDeletedEvent.Parent.GetId(),
            annotationDeletedEvent.Index,
            annotationDeletedEvent.DeletedAnnotation.GetId(),
            ToCommandId(annotationDeletedEvent),
            []
        );

    private MoveAnnotationFromOtherParent
        OnAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewParent.GetId(),
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            ToCommandId(annotationMovedEvent),
            []
        );

    private MoveAnnotationInSameParent OnAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent annotationMovedEvent) =>
        new(
            annotationMovedEvent.NewIndex,
            annotationMovedEvent.MovedAnnotation.GetId(),
            ToCommandId(annotationMovedEvent),
            []
        );

    #endregion

    #region References

    private AddReference OnReferenceAdded(ReferenceAddedEvent referenceAddedEvent) =>
        new(
            referenceAddedEvent.Parent.GetId(),
            referenceAddedEvent.Reference.ToMetaPointer(),
            referenceAddedEvent.Index,
            referenceAddedEvent.NewTarget.Reference?.GetId(),
            referenceAddedEvent.NewTarget.ResolveInfo,
            ToCommandId(referenceAddedEvent),
            []
        );

    private DeleteReference OnReferenceDeleted(ReferenceDeletedEvent referenceDeletedEvent) =>
        new(
            referenceDeletedEvent.Parent.GetId(),
            referenceDeletedEvent.Reference.ToMetaPointer(),
            referenceDeletedEvent.Index,
            referenceDeletedEvent.DeletedTarget.Reference?.GetId(),
            referenceDeletedEvent.DeletedTarget.ResolveInfo,
            ToCommandId(referenceDeletedEvent),
            []
        );

    private ChangeReference OnReferenceChanged(ReferenceChangedEvent referenceChangedEvent) =>
        new(
            referenceChangedEvent.Parent.GetId(),
            referenceChangedEvent.Reference.ToMetaPointer(),
            referenceChangedEvent.Index,
            referenceChangedEvent.OldTarget.Reference?.GetId(),
            referenceChangedEvent.OldTarget.ResolveInfo,
            referenceChangedEvent.NewTarget.Reference?.GetId(),
            referenceChangedEvent.NewTarget.ResolveInfo,
            ToCommandId(referenceChangedEvent),
            []
        );

    #endregion
}