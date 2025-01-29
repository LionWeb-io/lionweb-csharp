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

    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier) =>
        TargetDelegate.ChangeClassifier(node, newClassifier, oldClassifier);

    public bool CanRaiseChangeClassifier => TargetDelegate.CanRaiseChangeClassifier;

    public void AddProperty(IWritableNode node, Property property, Object newValue) =>
        TargetDelegate.AddProperty(node, property, newValue);

    public bool CanRaiseAddProperty => TargetDelegate.CanRaiseAddProperty;

    public void DeleteProperty(IWritableNode node, Property property, Object oldValue) =>
        TargetDelegate.DeleteProperty(node, property, oldValue);

    public bool CanRaiseDeleteProperty => TargetDelegate.CanRaiseDeleteProperty;

    public void ChangeProperty(IWritableNode node, Property property, Object newValue,
        Object oldValue) =>
        TargetDelegate.ChangeProperty(node, property, newValue, oldValue);

    public bool CanRaiseChangeProperty => TargetDelegate.CanRaiseChangeProperty;

    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index) =>
        TargetDelegate.AddChild(parent, newChild, containment, index);

    public bool CanRaiseAddChild => TargetDelegate.CanRaiseAddChild;

    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index) =>
        TargetDelegate.DeleteChild(deletedChild, parent, containment, index);

    public bool CanRaiseDeleteChild => TargetDelegate.CanRaiseDeleteChild;

    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment,
        Index index) =>
        TargetDelegate.ReplaceChild(newChild, replacedChild, parent, containment, index);

    public bool CanRaiseReplaceChild => TargetDelegate.CanRaiseReplaceChild;

    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex) =>
        TargetDelegate.MoveChildFromOtherContainment(newParent, newContainment, newIndex, movedChild, oldParent,
            oldContainment, oldIndex);

    public bool CanRaiseMoveChildFromOtherContainment => TargetDelegate.CanRaiseMoveChildFromOtherContainment;

    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex) =>
        TargetDelegate.MoveChildFromOtherContainmentInSameParent(newContainment, newIndex, movedChild, parent,
            oldContainment, oldIndex);

    public bool CanRaiseMoveChildFromOtherContainmentInSameParent =>
        TargetDelegate.CanRaiseMoveChildFromOtherContainmentInSameParent;

    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment,
        Index oldIndex) =>
        TargetDelegate.MoveChildInSameContainment(newIndex, movedChild, parent, containment, oldIndex);

    public bool CanRaiseMoveChildInSameContainment => TargetDelegate.CanRaiseMoveChildInSameContainment;

    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index) =>
        TargetDelegate.AddAnnotation(parent, newAnnotation, index);

    public bool CanRaiseAddAnnotation => TargetDelegate.CanRaiseAddAnnotation;

    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index) =>
        TargetDelegate.DeleteAnnotation(deletedAnnotation, parent, index);

    public bool CanRaiseDeleteAnnotation => TargetDelegate.CanRaiseDeleteAnnotation;

    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index) =>
        TargetDelegate.ReplaceAnnotation(newAnnotation, replacedAnnotation, parent, index);

    public bool CanRaiseReplaceAnnotation => TargetDelegate.CanRaiseReplaceAnnotation;

    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex) =>
        TargetDelegate.MoveAnnotationFromOtherParent(newParent, newIndex, movedAnnotation, oldParent, oldIndex);

    public bool CanRaiseMoveAnnotationFromOtherParent => TargetDelegate.CanRaiseMoveAnnotationFromOtherParent;

    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex) => TargetDelegate.MoveAnnotationInSameParent(newIndex, movedAnnotation, parent, oldIndex);

    public bool CanRaiseMoveAnnotationInSameParent => TargetDelegate.CanRaiseMoveAnnotationInSameParent;

    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget) =>
        TargetDelegate.AddReference(parent, reference, index, newTarget);

    public bool CanRaiseAddReference => TargetDelegate.CanRaiseAddReference;

    public void DeleteReference(IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget) => TargetDelegate.DeleteReference(parent, reference, index, deletedTarget);

    public bool CanRaiseDeleteReference => TargetDelegate.CanRaiseDeleteReference;

    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget) =>
        TargetDelegate.ChangeReference(parent, reference, index, newTarget, replacedTarget);

    public bool CanRaiseChangeReference => TargetDelegate.CanRaiseChangeReference;

    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        TargetDelegate.MoveEntryFromOtherReference(newParent, newReference, newIndex, oldParent, oldReference, oldIndex,
            target);

    public bool CanRaiseMoveEntryFromOtherReference => TargetDelegate.CanRaiseMoveEntryFromOtherReference;

    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        TargetDelegate.MoveEntryFromOtherReferenceInSameParent(parent, newReference, newIndex, oldReference, oldIndex,
            target);

    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent =>
        TargetDelegate.CanRaiseMoveEntryFromOtherReferenceInSameParent;

    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target) =>
        TargetDelegate.MoveEntryInSameReference(parent, reference, oldIndex, newIndex, target);

    public bool CanRaiseMoveEntryInSameReference => TargetDelegate.CanRaiseMoveEntryInSameReference;

    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode target) =>
        TargetDelegate.AddReferenceResolveInfo(parent, reference, index, newResolveInfo, target);

    public bool CanRaiseAddReferenceResolveInfo => TargetDelegate.CanRaiseAddReferenceResolveInfo;

    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, IReadableNode target,
        ResolveInfo deletedResolveInfo) =>
        TargetDelegate.DeleteReferenceResolveInfo(parent, reference, index, target, deletedResolveInfo);

    public bool CanRaiseDeleteReferenceResolveInfo => TargetDelegate.CanRaiseDeleteReferenceResolveInfo;

    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        IReadableNode? target, ResolveInfo replacedResolveInfo) =>
        TargetDelegate.ChangeReferenceResolveInfo(parent, reference, index, newResolveInfo, target,
            replacedResolveInfo);

    public bool CanRaiseChangeReferenceResolveInfo => TargetDelegate.CanRaiseChangeReferenceResolveInfo;

    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo resolveInfo) =>
        TargetDelegate.AddReferenceTarget(parent, reference, index, newTarget, resolveInfo);

    public bool CanRaiseAddReferenceTarget => TargetDelegate.CanRaiseAddReferenceTarget;

    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        IReadableNode deletedTarget) =>
        TargetDelegate.DeleteReferenceTarget(parent, reference, index, resolveInfo, deletedTarget);

    public bool CanRaiseDeleteReferenceTarget => TargetDelegate.CanRaiseDeleteReferenceTarget;

    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo? resolveInfo, IReadableNode oldTarget) =>
        TargetDelegate.ChangeReferenceTarget(parent, reference, index, newTarget, resolveInfo, oldTarget);

    public bool CanRaiseChangeReferenceTarget => TargetDelegate.CanRaiseChangeReferenceTarget;
}