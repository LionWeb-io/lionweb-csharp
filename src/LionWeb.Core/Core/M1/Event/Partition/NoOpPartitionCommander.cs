// Copyright 2024 TRUMPF Laser SE and other contributors
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

public class NoOpPartitionCommander : EventHandlerBase, IPartitionCommander
{
    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier,
        EventId? eventId = null) { }

    public bool CanRaiseChangeClassifier => false;

    public void AddProperty(IWritableNode node, Property property, object newValue, EventId? eventId = null) { }

    public bool CanRaiseAddProperty => false;

    public void DeleteProperty(IWritableNode node, Property property, object oldValue, EventId? eventId = null) { }

    public bool CanRaiseDeleteProperty => false;

    public void ChangeProperty(IWritableNode node, Property property, object newValue,
        object oldValue, EventId? eventId = null)
    {
    }

    public bool CanRaiseChangeProperty => false;

    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index,
        EventId? eventId = null) { }

    public bool CanRaiseAddChild => false;

    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index,
        EventId? eventId = null) { }

    public bool CanRaiseDeleteChild => false;

    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment, Index index, EventId? eventId = null)
    {
    }

    public bool CanRaiseReplaceChild => false;

    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex,
        EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveChildFromOtherContainment => false;

    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex, EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveChildFromOtherContainmentInSameParent => false;

    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment, Index oldIndex, EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveChildInSameContainment => false;

    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index, EventId? eventId = null) { }

    public bool CanRaiseAddAnnotation => false;

    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index,
        EventId? eventId = null) { }

    public bool CanRaiseDeleteAnnotation => false;

    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index, EventId? eventId = null)
    {
    }

    public bool CanRaiseReplaceAnnotation => false;

    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex, EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveAnnotationFromOtherParent => false;

    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex, EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveAnnotationInSameParent => false;

    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        EventId? eventId = null) { }

    public bool CanRaiseAddReference => false;

    public void DeleteReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget,
        EventId? eventId = null)
    {
    }

    public bool CanRaiseDeleteReference => false;

    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget, EventId? eventId = null)
    {
    }

    public bool CanRaiseChangeReference => false;

    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target,
        EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveEntryFromOtherReference => false;

    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target, EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent => false;

    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target, EventId? eventId = null)
    {
    }

    public bool CanRaiseMoveEntryInSameReference => false;

    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo, IReadableNode target, EventId? eventId = null)
    {
    }

    public bool CanRaiseAddReferenceResolveInfo => false;

    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, IReadableNode target,
        ResolveInfo deletedResolveInfo, EventId? eventId = null)
    {
    }

    public bool CanRaiseDeleteReferenceResolveInfo => false;

    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo, IReadableNode? target, ResolveInfo replacedResolveInfo, EventId? eventId = null)
    {
    }

    public bool CanRaiseChangeReferenceResolveInfo => false;

    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo resolveInfo, EventId? eventId = null)
    {
    }

    public bool CanRaiseAddReferenceTarget => false;

    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        IReadableNode deletedTarget, EventId? eventId = null)
    {
    }

    public bool CanRaiseDeleteReferenceTarget => false;

    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo? resolveInfo, IReadableNode oldTarget, EventId? eventId = null)
    {
    }

    public bool CanRaiseChangeReferenceTarget => false;
}