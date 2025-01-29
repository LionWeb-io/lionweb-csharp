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

public class CallSensitivePartitionCommander : IPartitionCommander, IOverridableCommander<IPartitionCommander>
{
    private readonly IPartitionCommander _defaultDelegate;
    private readonly AsyncLocal<IPartitionCommander?> _delegate = new();

    public IPartitionCommander? Delegate
    {
        get => _delegate.Value;
        set => _delegate.Value = value;
    }

    public CallSensitivePartitionCommander(IPartitionCommander defaultDelegate)
    {
        _defaultDelegate = defaultDelegate;
    }

    private IPartitionCommander TargetDelegate => Delegate ?? _defaultDelegate;

    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier,
        EventId? eventId = null) =>
        TargetDelegate.ChangeClassifier(node, newClassifier, oldClassifier, eventId);

    public bool CanRaiseChangeClassifier => TargetDelegate.CanRaiseChangeClassifier;

    public void AddProperty(IWritableNode node, Property property, object newValue, EventId? eventId = null) =>
        TargetDelegate.AddProperty(node, property, newValue, eventId);

    public bool CanRaiseAddProperty => TargetDelegate.CanRaiseAddProperty;

    public void DeleteProperty(IWritableNode node, Property property, object oldValue, EventId? eventId = null) =>
        TargetDelegate.DeleteProperty(node, property, oldValue, eventId);

    public bool CanRaiseDeleteProperty => TargetDelegate.CanRaiseDeleteProperty;

    public void ChangeProperty(IWritableNode node, Property property, object newValue,
        object oldValue, EventId? eventId = null) =>
        TargetDelegate.ChangeProperty(node, property, newValue, oldValue, eventId);

    public bool CanRaiseChangeProperty => TargetDelegate.CanRaiseChangeProperty;

    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index,
        EventId? eventId = null) =>
        TargetDelegate.AddChild(parent, newChild, containment, index, eventId);

    public bool CanRaiseAddChild => TargetDelegate.CanRaiseAddChild;

    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index,
        EventId? eventId = null) =>
        TargetDelegate.DeleteChild(deletedChild, parent, containment, index, eventId);

    public bool CanRaiseDeleteChild => TargetDelegate.CanRaiseDeleteChild;

    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment,
        Index index, EventId? eventId = null) =>
        TargetDelegate.ReplaceChild(newChild, replacedChild, parent, containment, index, eventId);

    public bool CanRaiseReplaceChild => TargetDelegate.CanRaiseReplaceChild;

    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex,
        EventId? eventId = null) =>
        TargetDelegate.MoveChildFromOtherContainment(newParent, newContainment, newIndex, movedChild, oldParent,
            oldContainment, oldIndex, eventId);

    public bool CanRaiseMoveChildFromOtherContainment => TargetDelegate.CanRaiseMoveChildFromOtherContainment;

    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex, EventId? eventId = null) =>
        TargetDelegate.MoveChildFromOtherContainmentInSameParent(newContainment, newIndex, movedChild, parent,
            oldContainment, oldIndex, eventId);

    public bool CanRaiseMoveChildFromOtherContainmentInSameParent =>
        TargetDelegate.CanRaiseMoveChildFromOtherContainmentInSameParent;

    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment,
        Index oldIndex, EventId? eventId = null) =>
        TargetDelegate.MoveChildInSameContainment(newIndex, movedChild, parent, containment, oldIndex, eventId);

    public bool CanRaiseMoveChildInSameContainment => TargetDelegate.CanRaiseMoveChildInSameContainment;

    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index,
        EventId? eventId = null) =>
        TargetDelegate.AddAnnotation(parent, newAnnotation, index, eventId);

    public bool CanRaiseAddAnnotation => TargetDelegate.CanRaiseAddAnnotation;

    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index,
        EventId? eventId = null) =>
        TargetDelegate.DeleteAnnotation(deletedAnnotation, parent, index, eventId);

    public bool CanRaiseDeleteAnnotation => TargetDelegate.CanRaiseDeleteAnnotation;

    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index, EventId? eventId = null) =>
        TargetDelegate.ReplaceAnnotation(newAnnotation, replacedAnnotation, parent, index, eventId);

    public bool CanRaiseReplaceAnnotation => TargetDelegate.CanRaiseReplaceAnnotation;

    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex, EventId? eventId = null) =>
        TargetDelegate.MoveAnnotationFromOtherParent(newParent, newIndex, movedAnnotation, oldParent, oldIndex,
            eventId);

    public bool CanRaiseMoveAnnotationFromOtherParent => TargetDelegate.CanRaiseMoveAnnotationFromOtherParent;

    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex, EventId? eventId = null) =>
        TargetDelegate.MoveAnnotationInSameParent(newIndex, movedAnnotation, parent, oldIndex, eventId);

    public bool CanRaiseMoveAnnotationInSameParent => TargetDelegate.CanRaiseMoveAnnotationInSameParent;

    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        EventId? eventId = null) =>
        TargetDelegate.AddReference(parent, reference, index, newTarget, eventId);

    public bool CanRaiseAddReference => TargetDelegate.CanRaiseAddReference;

    public void DeleteReference(IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget, EventId? eventId = null) =>
        TargetDelegate.DeleteReference(parent, reference, index, deletedTarget, eventId);

    public bool CanRaiseDeleteReference => TargetDelegate.CanRaiseDeleteReference;

    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget, EventId? eventId = null) =>
        TargetDelegate.ChangeReference(parent, reference, index, newTarget, replacedTarget, eventId);

    public bool CanRaiseChangeReference => TargetDelegate.CanRaiseChangeReference;

    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target,
        EventId? eventId = null) =>
        TargetDelegate.MoveEntryFromOtherReference(newParent, newReference, newIndex, oldParent, oldReference, oldIndex,
            target, eventId);

    public bool CanRaiseMoveEntryFromOtherReference => TargetDelegate.CanRaiseMoveEntryFromOtherReference;

    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target, EventId? eventId = null) =>
        TargetDelegate.MoveEntryFromOtherReferenceInSameParent(parent, newReference, newIndex, oldReference, oldIndex,
            target, eventId);

    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent =>
        TargetDelegate.CanRaiseMoveEntryFromOtherReferenceInSameParent;

    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target, EventId? eventId = null) =>
        TargetDelegate.MoveEntryInSameReference(parent, reference, oldIndex, newIndex, target, eventId);

    public bool CanRaiseMoveEntryInSameReference => TargetDelegate.CanRaiseMoveEntryInSameReference;

    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode target, EventId? eventId = null) =>
        TargetDelegate.AddReferenceResolveInfo(parent, reference, index, newResolveInfo, target, eventId);

    public bool CanRaiseAddReferenceResolveInfo => TargetDelegate.CanRaiseAddReferenceResolveInfo;

    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, IReadableNode target,
        ResolveInfo deletedResolveInfo, EventId? eventId = null) =>
        TargetDelegate.DeleteReferenceResolveInfo(parent, reference, index, target, deletedResolveInfo, eventId);

    public bool CanRaiseDeleteReferenceResolveInfo => TargetDelegate.CanRaiseDeleteReferenceResolveInfo;

    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode? target, ResolveInfo replacedResolveInfo, EventId? eventId = null) =>
        TargetDelegate.ChangeReferenceResolveInfo(parent, reference, index, newResolveInfo, target,
            replacedResolveInfo, eventId);

    public bool CanRaiseChangeReferenceResolveInfo => TargetDelegate.CanRaiseChangeReferenceResolveInfo;

    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo resolveInfo, EventId? eventId = null) =>
        TargetDelegate.AddReferenceTarget(parent, reference, index, newTarget, resolveInfo, eventId);

    public bool CanRaiseAddReferenceTarget => TargetDelegate.CanRaiseAddReferenceTarget;

    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        IReadableNode deletedTarget, EventId? eventId = null) =>
        TargetDelegate.DeleteReferenceTarget(parent, reference, index, resolveInfo, deletedTarget, eventId);

    public bool CanRaiseDeleteReferenceTarget => TargetDelegate.CanRaiseDeleteReferenceTarget;

    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo? resolveInfo, IReadableNode oldTarget, EventId? eventId = null) =>
        TargetDelegate.ChangeReferenceTarget(parent, reference, index, newTarget, resolveInfo, oldTarget, eventId);

    public bool CanRaiseChangeReferenceTarget => TargetDelegate.CanRaiseChangeReferenceTarget;

    public EventId CreateEventId() => TargetDelegate.CreateEventId();
}