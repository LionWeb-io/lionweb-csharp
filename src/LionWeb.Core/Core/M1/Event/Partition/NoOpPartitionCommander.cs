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
using SemanticPropertyValue = object;
using TargetNode = IReadableNode;

public class NoOpPartitionCommander : IPartitionCommander
{
    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier) { }

    public bool CanRaiseChangeClassifier => false;

    public void AddProperty(IWritableNode node, Property property, SemanticPropertyValue newValue) { }

    public bool CanRaiseAddProperty => false;

    public void DeleteProperty(IWritableNode node, Property property, SemanticPropertyValue oldValue) { }

    public bool CanRaiseDeleteProperty => false;

    public void ChangeProperty(IWritableNode node, Property property, SemanticPropertyValue newValue,
        SemanticPropertyValue oldValue)
    {
    }

    public bool CanRaiseChangeProperty => false;

    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index) { }

    public bool CanRaiseAddChild => false;

    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index) { }

    public bool CanRaiseDeleteChild => false;

    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment, Index index)
    {
    }

    public bool CanRaiseReplaceChild => false;

    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex)
    {
    }

    public bool CanRaiseMoveChildFromOtherContainment => false;

    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex)
    {
    }

    public bool CanRaiseMoveChildFromOtherContainmentInSameParent => false;

    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment, Index oldIndex)
    {
    }

    public bool CanRaiseMoveChildInSameContainment => false;

    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index) { }

    public bool CanRaiseAddAnnotation => false;

    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index) { }

    public bool CanRaiseDeleteAnnotation => false;

    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index)
    {
    }

    public bool CanRaiseReplaceAnnotation => false;

    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex)
    {
    }

    public bool CanRaiseMoveAnnotationFromOtherParent => false;

    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex)
    {
    }

    public bool CanRaiseMoveAnnotationInSameParent => false;

    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget) { }

    public bool CanRaiseAddReference => false;

    public void DeleteReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget)
    {
    }

    public bool CanRaiseDeleteReference => false;

    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget)
    {
    }

    public bool CanRaiseChangeReference => false;

    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target)
    {
    }

    public bool CanRaiseMoveEntryFromOtherReference => false;

    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target)
    {
    }

    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent => false;

    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target)
    {
    }

    public bool CanRaiseMoveEntryInSameReference => false;

    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo, TargetNode target)
    {
    }

    public bool CanRaiseAddReferenceResolveInfo => false;

    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, TargetNode target,
        ResolveInfo deletedResolveInfo)
    {
    }

    public bool CanRaiseDeleteReferenceResolveInfo => false;

    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo, TargetNode? target, ResolveInfo replacedResolveInfo)
    {
    }

    public bool CanRaiseChangeReferenceResolveInfo => false;

    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo resolveInfo)
    {
    }

    public bool CanRaiseAddReferenceTarget => false;

    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        TargetNode deletedTarget)
    {
    }

    public bool CanRaiseDeleteReferenceTarget => false;

    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo? resolveInfo, TargetNode oldTarget)
    {
    }

    public bool CanRaiseChangeReferenceTarget => false;
}