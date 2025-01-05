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

namespace LionWeb.Core.M1;

using M3;
using Index = int;
using ResolveInfo = string;
using TargetNode = IReadableNode;
using PropertyValue = object;

public interface IForestListener
{
    record NewPartitionArgs(IReadableNode newPartition);

    event EventHandler<NewPartitionArgs> NewPartition;

    record PartitionDeletedArgs(IReadableNode deletedPartition);

    event EventHandler<PartitionDeletedArgs> PartitionDeleted;
}

public interface IForestCommander
{
    void AddPartition(IReadableNode newPartition);
    // bool CanRaiseAddPartition();

    void DeletePartition(IReadableNode deletedPartition);
    // bool CanRaiseDeletePartition();
}

public interface IReferenceTarget
{
    ResolveInfo? ResolveInfo { get; }
    TargetNode? Reference { get; }
}

public readonly record struct ReferenceTarget(ResolveInfo? ResolveInfo, TargetNode? Reference) : IReferenceTarget;

public interface IPartitionListener
{
    #region Nodes

    record ClassifierChangedArgs(IWritableNode node, Classifier newClassifier, Classifier oldClassifier);

    event EventHandler<ClassifierChangedArgs> ClassifierChanged;

    #endregion

    #region Properties

    record PropertyAddedArgs(IWritableNode node, Property property, PropertyValue newValue);

    event EventHandler<PropertyAddedArgs> PropertyAdded;

    record PropertyDeletedArgs(IWritableNode node, Property property, PropertyValue oldValue);

    event EventHandler<PropertyDeletedArgs> PropertyDeleted;

    record PropertyChangedArgs(IWritableNode node, Property property, PropertyValue newValue, PropertyValue oldValue);

    event EventHandler<PropertyChangedArgs> PropertyChanged;

    #endregion

    #region Children

    record ChildAddedArgs(IWritableNode parent, IWritableNode newChild, Containment containment, Index index);

    event EventHandler<ChildAddedArgs> ChildAdded;

    record ChildDeletedArgs(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index);

    event EventHandler<ChildDeletedArgs> ChildDeleted;

    record ChildReplacedArgs(
        IWritableNode newChild,
        IWritableNode replacedChild,
        IWritableNode parent,
        Containment containment,
        Index index);

    event EventHandler<ChildReplacedArgs> ChildReplaced;

    record ChildMovedFromOtherContainmentArgs(
        IWritableNode newParent,
        Containment newContainment,
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode oldParent,
        Containment oldContainment,
        Index oldIndex);

    event EventHandler<ChildMovedFromOtherContainmentArgs> ChildMovedFromOtherContainment;

    record ChildMovedFromOtherContainmentInSameParentArgs(
        Containment newContainment,
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent,
        Containment oldContainment,
        Index oldIndex);

    event EventHandler<ChildMovedFromOtherContainmentInSameParentArgs> ChildMovedFromOtherContainmentInSameParent;

    record ChildMovedInSameContainmentArgs(
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent,
        Containment containment,
        Index oldIndex);

    event EventHandler<ChildMovedInSameContainmentArgs> ChildMovedInSameContainment;

    #endregion

    #region Annotations

    record AnnotationAddedArgs(IWritableNode parent, IWritableNode newAnnotation, Index index);

    event EventHandler<AnnotationAddedArgs> AnnotationAdded;

    record AnnotationDeletedArgs(IWritableNode deletedAnnotation, IWritableNode parent, Index index);

    event EventHandler<AnnotationDeletedArgs> AnnotationDeleted;

    record AnnotationReplacedArgs(
        IWritableNode newAnnotation,
        IWritableNode replacedAnnotation,
        IWritableNode parent,
        Index index);

    event EventHandler<AnnotationReplacedArgs> AnnotationReplaced;

    record AnnotationMovedFromOtherParentArgs(
        IWritableNode newParent,
        Index newIndex,
        IWritableNode movedAnnotation,
        IWritableNode oldParent,
        Index oldIndex);

    event EventHandler<AnnotationMovedFromOtherParentArgs> AnnotationMovedFromOtherParent;

    record AnnotationMovedInSameParentArgs(
        Index newIndex,
        IWritableNode movedAnnotation,
        IWritableNode parent,
        Index oldIndex);

    event EventHandler<AnnotationMovedInSameParentArgs> AnnotationMovedInSameParent;

    #endregion

    #region References

    record ReferenceAddedArgs(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget);

    event EventHandler<ReferenceAddedArgs> ReferenceAdded;

    record ReferenceDeletedArgs(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget);

    event EventHandler<ReferenceDeletedArgs> ReferenceDeleted;

    record ReferenceChangedArgs(
        IWritableNode parent,
        Reference reference,
        Index index,
        IReferenceTarget newTarget,
        IReferenceTarget replacedTarget);

    event EventHandler<ReferenceChangedArgs> ReferenceChanged;

    record EntryMovedFromOtherReferenceArgs(
        IWritableNode newParent,
        Reference newReference,
        Index newIndex,
        IWritableNode oldParent,
        Reference oldReference,
        Index oldIndex,
        IReferenceTarget target);

    event EventHandler<EntryMovedFromOtherReferenceArgs> EntryMovedFromOtherReference;

    record EntryMovedFromOtherReferenceInSameParentArgs(
        IWritableNode parent,
        Reference newReference,
        Index newIndex,
        Reference oldReference,
        Index oldIndex,
        IReferenceTarget target);

    event EventHandler<EntryMovedFromOtherReferenceInSameParentArgs> EntryMovedFromOtherReferenceInSameParent;

    record EntryMovedInSameReferenceArgs(
        IWritableNode parent,
        Reference reference,
        Index oldIndex,
        Index newIndex,
        IReferenceTarget target);

    event EventHandler<EntryMovedInSameReferenceArgs> EntryMovedInSameReference;

    record ReferenceResolveInfoAddedArgs(
        IWritableNode parent,
        Reference reference,
        Index index,
        ResolveInfo newResolveInfo,
        TargetNode target);

    event EventHandler<ReferenceResolveInfoAddedArgs> ReferenceResolveInfoAdded;

    record ReferenceResolveInfoDeletedArgs(
        IWritableNode parent,
        Reference reference,
        Index index,
        TargetNode target,
        ResolveInfo deletedResolveInfo);

    event EventHandler<ReferenceResolveInfoDeletedArgs> ReferenceResolveInfoDeleted;

    record ReferenceResolveInfoChangedArgs(
        IWritableNode parent,
        Reference reference,
        Index index,
        ResolveInfo newResolveInfo,
        TargetNode? target,
        ResolveInfo replacedResolveInfo);

    event EventHandler<ReferenceResolveInfoChangedArgs> ReferenceResolveInfoChanged;

    record ReferenceTargetAddedArgs(
        IWritableNode parent,
        Reference reference,
        Index index,
        TargetNode newTarget,
        ResolveInfo resolveInfo);

    event EventHandler<ReferenceTargetAddedArgs> ReferenceTargetAdded;

    record ReferenceTargetDeletedArgs(
        IWritableNode parent,
        Reference reference,
        Index index,
        ResolveInfo resolveInfo,
        TargetNode deletedTarget);

    event EventHandler<ReferenceTargetDeletedArgs> ReferenceTargetDeleted;

    record ReferenceTargetChangedArgs(
        IWritableNode parent,
        Reference reference,
        Index index,
        TargetNode newTarget,
        ResolveInfo? resolveInfo,
        TargetNode oldTarget);

    event EventHandler<ReferenceTargetChangedArgs> ReferenceTargetChanged;

    #endregion
}

public interface IPartitionCommander
{
    #region Nodes

    void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier);
    // bool CanRaiseChangeClassifier();

    #endregion

    #region Properties

    void AddProperty(IWritableNode node, Property property, PropertyValue newValue);
    // bool CanRaiseAddProperty();

    void DeleteProperty(IWritableNode node, Property property, PropertyValue oldValue);
    // bool CanRaiseDeleteProperty();

    void ChangeProperty(IWritableNode node, Property property, PropertyValue newValue, PropertyValue oldValue);
    // bool CanRaiseChangeProperty();

    #endregion

    #region Children

    void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index);
    bool CanRaiseAddChild();

    void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index);
    bool CanRaiseDeleteChild();

    void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment, Index index);

    bool CanRaiseReplaceChild();

    void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex);

    bool CanRaiseMoveChildFromOtherContainment();

    void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex, IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex);

    bool CanRaiseMoveChildFromOtherContainmentInSameParent();

    void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment, Index oldIndex);

    bool CanRaiseMoveChildInSameContainment();

    #endregion

    #region Annotations

    void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index);
    // bool CanRaiseAddAnnotation();

    void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index);
    // bool CanRaiseDeleteAnnotation();

    void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index);
    // bool CanRaiseReplaceAnnotation();

    void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex);
    // bool CanRaiseMoveAnnotationFromOtherParent();

    void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex);
    // bool CanRaiseMoveAnnotationInSameParent();

    #endregion

    #region References

    void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget);
    bool CanRaiseAddReference();

    void DeleteReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget);
    bool CanRaiseDeleteReference();

    void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget);

    bool CanRaiseChangeReference();

    void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target);
    // bool CanRaiseMoveEntryFromOtherReference();

    void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target);
    // bool CanRaiseMoveEntryFromOtherReferenceInSameParent();

    void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target);

    bool CanRaiseMoveEntryInSameReference();

    void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        TargetNode target);
    // bool CanRaiseAddReferenceResolveInfo();

    void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, TargetNode target,
        ResolveInfo deletedResolveInfo);
    // bool CanRaiseDeleteReferenceResolveInfo();

    void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        TargetNode? target, ResolveInfo replacedResolveInfo);
    // bool CanRaiseChangeReferenceResolveInfo();

    void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo resolveInfo);
    // bool CanRaiseAddReferenceTarget();

    void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        TargetNode deletedTarget);
    // bool CanRaiseDeleteReferenceTarget();

    void ChangedReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo? resolveInfo, TargetNode oldTarget);
    // bool CanRaiseChangeReferenceTarget();

    #endregion
}