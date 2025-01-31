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
    public event EventHandler<IPartitionPublisher.ClassifierChangedArgs> ClassifierChanged
    {
        add => _classifierChanged.Add(value);
        remove => _classifierChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ClassifierChangedArgs> _classifierChanged = new();

    /// <inheritdoc />
    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier,
        EventId? eventId = null) =>
        _classifierChanged.Invoke(_sender, new(node, newClassifier, oldClassifier, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeClassifier => _classifierChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.PropertyAddedArgs> PropertyAdded
    {
        add => _propertyAdded.Add(value);
        remove => _propertyAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.PropertyAddedArgs> _propertyAdded = new();

    /// <inheritdoc />
    public void AddProperty(IWritableNode node, Property property, object newValue, EventId? eventId = null)
    {
        var args = new IPartitionPublisher.PropertyAddedArgs(node, property, newValue, eventId ?? CreateEventId());
        _propertyAdded.Invoke(_sender, args);
        Raise(args);
    }

    /// <inheritdoc />
    public bool CanRaiseAddProperty => _propertyAdded.HasSubscribers || CanRaise(typeof(IPartitionPublisher.PropertyAddedArgs));

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.PropertyDeletedArgs> PropertyDeleted
    {
        add => _propertyDeleted.Add(value);
        remove => _propertyDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.PropertyDeletedArgs> _propertyDeleted = new();

    /// <inheritdoc />
    public void DeleteProperty(IWritableNode node, Property property, object oldValue, EventId? eventId = null) =>
        _propertyDeleted.Invoke(_sender, new(node, property, oldValue, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteProperty => _propertyDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.PropertyChangedArgs> PropertyChanged
    {
        add => _propertyChanged.Add(value);
        remove => _propertyChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.PropertyChangedArgs> _propertyChanged = new();

    /// <inheritdoc />
    public void ChangeProperty(IWritableNode node, Property property, object newValue, object oldValue,
        EventId? eventId = null)
    {
        var args = new IPartitionPublisher.PropertyChangedArgs(node, property, newValue, oldValue, eventId ?? CreateEventId());
        _propertyChanged.Invoke(_sender, args);
        Raise(args);
    }

    /// <inheritdoc />
    public bool CanRaiseChangeProperty => _propertyChanged.HasSubscribers || CanRaise(typeof(IPartitionPublisher.PropertyChangedArgs));

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ChildAddedArgs> ChildAdded
    {
        add => _childAdded.Add(value);
        remove => _childAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ChildAddedArgs> _childAdded = new();

    /// <inheritdoc />
    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index,
        EventId? eventId = null) =>
        _childAdded.Invoke(_sender, new(parent, newChild, containment, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddChild => _childAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ChildDeletedArgs> ChildDeleted
    {
        add => _childDeleted.Add(value);
        remove => _childDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ChildDeletedArgs> _childDeleted = new();

    /// <inheritdoc />
    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index,
        EventId? eventId = null) =>
        _childDeleted.Invoke(_sender, new(deletedChild, parent, containment, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteChild => _childDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ChildReplacedArgs> ChildReplaced
    {
        add => _childReplaced.Add(value);
        remove => _childReplaced.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ChildReplacedArgs> _childReplaced = new();

    /// <inheritdoc />
    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment,
        Index index, EventId? eventId = null) =>
        _childReplaced.Invoke(_sender,
            new(newChild, replacedChild, parent, containment, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseReplaceChild => _childReplaced.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ChildMovedFromOtherContainmentArgs> ChildMovedFromOtherContainment
    {
        add => _childMovedFromOtherContainment.Add(value);
        remove => _childMovedFromOtherContainment.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ChildMovedFromOtherContainmentArgs>
        _childMovedFromOtherContainment = new();

    /// <inheritdoc />
    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex,
        EventId? eventId = null) =>
        _childMovedFromOtherContainment.Invoke(_sender,
            new(newParent, newContainment, newIndex, movedChild, oldParent, oldContainment, oldIndex,
                eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveChildFromOtherContainment => _childMovedFromOtherContainment.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ChildMovedFromOtherContainmentInSameParentArgs>
        ChildMovedFromOtherContainmentInSameParent
        {
            add => _childMovedFromOtherContainmentInSameParent.Add(value);
            remove => _childMovedFromOtherContainmentInSameParent.Remove(value);
        }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ChildMovedFromOtherContainmentInSameParentArgs>
        _childMovedFromOtherContainmentInSameParent = new();

    /// <inheritdoc />
    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex, EventId? eventId = null) =>
        _childMovedFromOtherContainmentInSameParent.Invoke(_sender,
            new(newContainment, newIndex, movedChild, parent, oldContainment, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveChildFromOtherContainmentInSameParent =>
        _childMovedFromOtherContainmentInSameParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ChildMovedInSameContainmentArgs> ChildMovedInSameContainment
    {
        add => _childMovedInSameContainment.Add(value);
        remove => _childMovedInSameContainment.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ChildMovedInSameContainmentArgs>
        _childMovedInSameContainment = new();

    /// <inheritdoc />
    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment,
        Index oldIndex, EventId? eventId = null) =>
        _childMovedInSameContainment.Invoke(_sender,
            new(newIndex, movedChild, parent, containment, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveChildInSameContainment => _childMovedInSameContainment.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.AnnotationAddedArgs> AnnotationAdded
    {
        add => _annotationAdded.Add(value);
        remove => _annotationAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.AnnotationAddedArgs> _annotationAdded = new();

    /// <inheritdoc />
    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index,
        EventId? eventId = null) =>
        _annotationAdded.Invoke(_sender, new(parent, newAnnotation, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddAnnotation => _annotationAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.AnnotationDeletedArgs> AnnotationDeleted
    {
        add => _annotationDeleted.Add(value);
        remove => _annotationDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.AnnotationDeletedArgs> _annotationDeleted = new();

    /// <inheritdoc />
    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index,
        EventId? eventId = null) =>
        _annotationDeleted.Invoke(_sender, new(deletedAnnotation, parent, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteAnnotation => _annotationDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.AnnotationReplacedArgs> AnnotationReplaced
    {
        add => _annotationReplaced.Add(value);
        remove => _annotationReplaced.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.AnnotationReplacedArgs> _annotationReplaced = new();

    /// <inheritdoc />
    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index, EventId? eventId = null) =>
        _annotationReplaced.Invoke(_sender,
            new(newAnnotation, replacedAnnotation, parent, index, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseReplaceAnnotation => _annotationReplaced.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.AnnotationMovedFromOtherParentArgs> AnnotationMovedFromOtherParent
    {
        add => _annotationMovedFromOtherParent.Add(value);
        remove => _annotationMovedFromOtherParent.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.AnnotationMovedFromOtherParentArgs>
        _annotationMovedFromOtherParent = new();

    /// <inheritdoc />
    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex, EventId? eventId = null) =>
        _annotationMovedFromOtherParent.Invoke(_sender,
            new(newParent, newIndex, movedAnnotation, oldParent, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveAnnotationFromOtherParent => _annotationMovedFromOtherParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.AnnotationMovedInSameParentArgs> AnnotationMovedInSameParent
    {
        add => _annotationMovedInSameParent.Add(value);
        remove => _annotationMovedInSameParent.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.AnnotationMovedInSameParentArgs>
        _annotationMovedInSameParent = new();

    /// <inheritdoc />
    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex, EventId? eventId = null) =>
        _annotationMovedInSameParent.Invoke(_sender,
            new(newIndex, movedAnnotation, parent, oldIndex, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveAnnotationInSameParent => _annotationMovedInSameParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceAddedArgs> ReferenceAdded
    {
        add => _referenceAdded.Add(value);
        remove => _referenceAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceAddedArgs> _referenceAdded = new();

    /// <inheritdoc />
    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        EventId? eventId = null) =>
        _referenceAdded.Invoke(_sender, new(parent, reference, index, newTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddReference => _referenceAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceDeletedArgs> ReferenceDeleted
    {
        add => _referenceDeleted.Add(value);
        remove => _referenceDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceDeletedArgs> _referenceDeleted = new();

    /// <inheritdoc />
    public void DeleteReference(IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget, EventId? eventId = null) =>
        _referenceDeleted.Invoke(_sender, new(parent, reference, index, deletedTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteReference => _referenceDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceChangedArgs> ReferenceChanged
    {
        add => _referenceChanged.Add(value);
        remove => _referenceChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceChangedArgs> _referenceChanged = new();

    /// <inheritdoc />
    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget, EventId? eventId = null) =>
        _referenceChanged.Invoke(_sender,
            new(parent, reference, index, newTarget, replacedTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeReference => _referenceChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.EntryMovedFromOtherReferenceArgs> EntryMovedFromOtherReference
    {
        add => _entryMovedFromOtherReference.Add(value);
        remove => _entryMovedFromOtherReference.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.EntryMovedFromOtherReferenceArgs>
        _entryMovedFromOtherReference = new();

    /// <inheritdoc />
    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target,
        EventId? eventId = null) =>
        _entryMovedFromOtherReference.Invoke(_sender,
            new(newParent, newReference, newIndex, oldParent, oldReference, oldIndex, target,
                eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryFromOtherReference => _entryMovedFromOtherReference.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.EntryMovedFromOtherReferenceInSameParentArgs>
        EntryMovedFromOtherReferenceInSameParent
        {
            add => _entryMovedFromOtherReferenceInSameParent.Add(value);
            remove => _entryMovedFromOtherReferenceInSameParent.Remove(value);
        }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.EntryMovedFromOtherReferenceInSameParentArgs>
        _entryMovedFromOtherReferenceInSameParent = new();

    /// <inheritdoc />
    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target, EventId? eventId = null) =>
        _entryMovedFromOtherReferenceInSameParent.Invoke(_sender,
            new(parent, newReference, newIndex, oldReference, oldIndex, target, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent =>
        _entryMovedFromOtherReferenceInSameParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.EntryMovedInSameReferenceArgs> EntryMovedInSameReference
    {
        add => _entryMovedInSameReference.Add(value);
        remove => _entryMovedInSameReference.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.EntryMovedInSameReferenceArgs>
        _entryMovedInSameReference =
            new();

    /// <inheritdoc />
    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target, EventId? eventId = null) =>
        _entryMovedInSameReference.Invoke(_sender,
            new(parent, reference, newIndex, oldIndex, target, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryInSameReference => _entryMovedInSameReference.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceResolveInfoAddedArgs> ReferenceResolveInfoAdded
    {
        add => _referenceResolveInfoAdded.Add(value);
        remove => _referenceResolveInfoAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceResolveInfoAddedArgs>
        _referenceResolveInfoAdded =
            new();

    /// <inheritdoc />
    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode target, EventId? eventId = null) =>
        _referenceResolveInfoAdded.Invoke(_sender,
            new(parent, reference, index, newResolveInfo, target, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddReferenceResolveInfo => _referenceResolveInfoAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceResolveInfoDeletedArgs> ReferenceResolveInfoDeleted
    {
        add => _referenceResolveInfoDeleted.Add(value);
        remove => _referenceResolveInfoDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceResolveInfoDeletedArgs>
        _referenceResolveInfoDeleted = new();

    /// <inheritdoc />
    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, IReadableNode target,
        ResolveInfo deletedResolveInfo, EventId? eventId = null) =>
        _referenceResolveInfoDeleted.Invoke(_sender,
            new(parent, reference, index, target, deletedResolveInfo, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteReferenceResolveInfo => _referenceResolveInfoDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceResolveInfoChangedArgs> ReferenceResolveInfoChanged
    {
        add => _referenceResolveInfoChanged.Add(value);
        remove => _referenceResolveInfoChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceResolveInfoChangedArgs>
        _referenceResolveInfoChanged = new();

    /// <inheritdoc />
    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode? target, ResolveInfo replacedResolveInfo, EventId? eventId = null) =>
        _referenceResolveInfoChanged.Invoke(_sender,
            new(parent, reference, index, newResolveInfo, target, replacedResolveInfo, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeReferenceResolveInfo => _referenceResolveInfoChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceTargetAddedArgs> ReferenceTargetAdded
    {
        add => _referenceTargetAdded.Add(value);
        remove => _referenceTargetAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceTargetAddedArgs>
        _referenceTargetAdded = new();

    /// <inheritdoc />
    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo resolveInfo, EventId? eventId = null) =>
        _referenceTargetAdded.Invoke(_sender,
            new(parent, reference, index, newTarget, resolveInfo, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddReferenceTarget => _referenceTargetAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceTargetDeletedArgs> ReferenceTargetDeleted
    {
        add => _referenceTargetDeleted.Add(value);
        remove => _referenceTargetDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceTargetDeletedArgs>
        _referenceTargetDeleted = new();

    /// <inheritdoc />
    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        IReadableNode deletedTarget, EventId? eventId = null) =>
        _referenceTargetDeleted.Invoke(_sender,
            new(parent, reference, index, resolveInfo, deletedTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeleteReferenceTarget => _referenceTargetDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionPublisher.ReferenceTargetChangedArgs> ReferenceTargetChanged
    {
        add => _referenceTargetChanged.Add(value);
        remove => _referenceTargetChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionPublisher.ReferenceTargetChangedArgs>
        _referenceTargetChanged = new();

    /// <inheritdoc />
    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo? resolveInfo, IReadableNode oldTarget, EventId? eventId = null) =>
        _referenceTargetChanged.Invoke(_sender,
            new(parent, reference, index, newTarget, resolveInfo, oldTarget, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseChangeReferenceTarget => _referenceTargetChanged.HasSubscribers;
}