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

namespace LionWeb.Core.M1.Event.Partition;

using M3;

/// Forwards <see cref="IPartitionCommander"/> commands to <see cref="IPartitionPublisher"/> events.
public class PartitionEventHandler : EventHandlerBase<IPartitionEvent>, IPartitionPublisher, IPartitionCommander
{
    /// <inheritdoc cref="PartitionEventHandler"/>
    /// <param name="sender">Optional sender of the events.</param>
    public PartitionEventHandler(object? sender) : base(sender)
    {
    }

    /// <inheritdoc />
    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ClassifierChangedArgs(node, newClassifier, oldClassifier, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeClassifier => CanRaise(typeof(IPartitionPublisher.ClassifierChangedArgs));

    /// <inheritdoc />
    public void AddProperty(IWritableNode node, Property property, object newValue, EventId? eventId = null)
    {
        Raise(new IPartitionPublisher.PropertyAddedArgs(node, property, newValue, eventId ?? CreateEventId()));
    }

    /// <inheritdoc />
    public bool CanRaiseAddProperty => CanRaise(typeof(IPartitionPublisher.PropertyAddedArgs));

    /// <inheritdoc />
    public void DeleteProperty(IWritableNode node, Property property, object oldValue, EventId? eventId = null) =>
        Raise( new IPartitionPublisher.PropertyDeletedArgs(node, property, oldValue, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteProperty => CanRaise(typeof(IPartitionPublisher.PropertyDeletedArgs));

    /// <inheritdoc />
    public void ChangeProperty(IWritableNode node, Property property, object newValue, object oldValue,
        EventId? eventId = null)
    {
        Raise(new IPartitionPublisher.PropertyChangedArgs(node, property, newValue, oldValue, eventId ?? CreateEventId()));
    }

    /// <inheritdoc />
    public bool CanRaiseChangeProperty => CanRaise(typeof(IPartitionPublisher.PropertyChangedArgs));

    /// <inheritdoc />
    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ChildAddedArgs(parent, newChild, containment, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddChild => CanRaise(typeof(IPartitionPublisher.ChildAddedArgs));

    /// <inheritdoc />
    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ChildDeletedArgs(deletedChild, parent, containment, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteChild => CanRaise(typeof(IPartitionPublisher.ChildDeletedArgs));

    /// <inheritdoc />
    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment,
        Index index, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ChildReplacedArgs(newChild, replacedChild, parent, containment, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseReplaceChild => CanRaise(typeof(IPartitionPublisher.ChildReplacedArgs));

    /// <inheritdoc />
    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ChildMovedFromOtherContainmentArgs(newParent, newContainment, newIndex, movedChild, oldParent, oldContainment, oldIndex,
                eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveChildFromOtherContainment => CanRaise(typeof(IPartitionPublisher.ChildMovedFromOtherContainmentArgs));

    /// <inheritdoc />
    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ChildMovedFromOtherContainmentInSameParentArgs(newContainment, newIndex, movedChild, parent, oldContainment, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveChildFromOtherContainmentInSameParent => CanRaise(typeof(IPartitionPublisher.ChildMovedFromOtherContainmentInSameParentArgs));

    /// <inheritdoc />
    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment,
        Index oldIndex, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ChildMovedInSameContainmentArgs(newIndex, movedChild, parent, containment, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveChildInSameContainment => CanRaise(typeof(IPartitionPublisher.ChildMovedInSameContainmentArgs));

    /// <inheritdoc />
    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.AnnotationAddedArgs(parent, newAnnotation, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddAnnotation => CanRaise(typeof(IPartitionPublisher.AnnotationAddedArgs));

    /// <inheritdoc />
    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.AnnotationDeletedArgs(deletedAnnotation, parent, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteAnnotation => CanRaise(typeof(IPartitionPublisher.AnnotationDeletedArgs));

    /// <inheritdoc />
    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.AnnotationReplacedArgs(newAnnotation, replacedAnnotation, parent, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseReplaceAnnotation => CanRaise(typeof(IPartitionPublisher.AnnotationReplacedArgs));

    /// <inheritdoc />
    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.AnnotationMovedFromOtherParentArgs(newParent, newIndex, movedAnnotation, oldParent, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveAnnotationFromOtherParent => CanRaise(typeof(IPartitionPublisher.AnnotationMovedFromOtherParentArgs));

    /// <inheritdoc />
    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.AnnotationMovedInSameParentArgs(newIndex, movedAnnotation, parent, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveAnnotationInSameParent => CanRaise(typeof(IPartitionPublisher.AnnotationMovedInSameParentArgs));

    /// <inheritdoc />
    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ReferenceAddedArgs(parent, reference, index, newTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddReference => CanRaise(typeof(IPartitionPublisher.ReferenceAddedArgs));

    /// <inheritdoc />
    public void DeleteReference(IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ReferenceDeletedArgs(parent, reference, index, deletedTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteReference => CanRaise(typeof(IPartitionPublisher.ReferenceDeletedArgs));

    /// <inheritdoc />
    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.ReferenceChangedArgs(parent, reference, index, newTarget, replacedTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeReference => CanRaise(typeof(IPartitionPublisher.ReferenceChangedArgs));

    /// <inheritdoc />
    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target,
        EventId? eventId = null) =>
        Raise(new IPartitionPublisher.EntryMovedFromOtherReferenceArgs(newParent, newReference, newIndex, oldParent, oldReference, oldIndex, target,
                eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryFromOtherReference => CanRaise(typeof(IPartitionPublisher.EntryMovedFromOtherReferenceArgs));

    /// <inheritdoc />
    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.EntryMovedFromOtherReferenceInSameParentArgs(parent, newReference, newIndex, oldReference, oldIndex, target, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent => CanRaise(typeof(IPartitionPublisher.EntryMovedFromOtherReferenceInSameParentArgs));

    /// <inheritdoc />
    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target, EventId? eventId = null) =>
        Raise(new IPartitionPublisher.EntryMovedInSameReferenceArgs(parent, reference, newIndex, oldIndex, target, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryInSameReference => CanRaise(typeof(IPartitionPublisher.EntryMovedInSameReferenceArgs));

    /// <inheritdoc />
    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode target, EventId? eventId = null) =>
        Raise(
            new IPartitionPublisher.ReferenceResolveInfoAddedArgs(parent, reference, index, newResolveInfo, target, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddReferenceResolveInfo => CanRaise(typeof(IPartitionPublisher.ReferenceResolveInfoAddedArgs));

    /// <inheritdoc />
    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, IReadableNode target,
        ResolveInfo deletedResolveInfo, EventId? eventId = null) =>
        Raise(
            new IPartitionPublisher.ReferenceResolveInfoDeletedArgs(parent, reference, index, target, deletedResolveInfo, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteReferenceResolveInfo => CanRaise(typeof(IPartitionPublisher.ReferenceResolveInfoDeletedArgs));

    /// <inheritdoc />
    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode? target, ResolveInfo replacedResolveInfo, EventId? eventId = null) =>
        Raise(
            new IPartitionPublisher.ReferenceResolveInfoChangedArgs(parent, reference, index, newResolveInfo, target, replacedResolveInfo, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeReferenceResolveInfo => CanRaise(typeof(IPartitionPublisher.ReferenceResolveInfoChangedArgs));

    /// <inheritdoc />
    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo resolveInfo, EventId? eventId = null) =>
        Raise(
            new IPartitionPublisher.ReferenceTargetAddedArgs(parent, reference, index, newTarget, resolveInfo, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddReferenceTarget => CanRaise(typeof(IPartitionPublisher.ReferenceTargetAddedArgs));

    /// <inheritdoc />
    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        IReadableNode deletedTarget, EventId? eventId = null) =>
        Raise(
            new IPartitionPublisher.ReferenceTargetDeletedArgs(parent, reference, index, resolveInfo, deletedTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteReferenceTarget => CanRaise(typeof(IPartitionPublisher.ReferenceTargetDeletedArgs));

    /// <inheritdoc />
    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo? resolveInfo, IReadableNode oldTarget, EventId? eventId = null) =>
        Raise(
            new IPartitionPublisher.ReferenceTargetChangedArgs(parent, reference, index, newTarget, resolveInfo, oldTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeReferenceTarget => CanRaise(typeof(IPartitionPublisher.ReferenceTargetChangedArgs));
}