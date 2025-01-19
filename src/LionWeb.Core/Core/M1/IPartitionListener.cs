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
using TargetNode = IReadableNode;
using PropertyValue = object;

/// Provides events about <see cref="INode">nodes</see> and their <see cref="Feature">features</see>.
/// <seealso cref="IPartitionCommander"/>
/// <seealso cref="PartitionEventHandler"/>
public interface IPartitionListener
{
    #region Nodes

    /// <inheritdoc cref="IPartitionListener.ClassifierChanged"/>
    /// <param name="Node"></param>
    /// <param name="NewClassifier"></param>
    /// <param name="OldClassifier"></param>
    record ClassifierChangedArgs(IWritableNode Node, Classifier NewClassifier, Classifier OldClassifier);

    /// <seealso cref="IPartitionCommander.ChangeClassifier"/>
    event EventHandler<ClassifierChangedArgs> ClassifierChanged;

    #endregion

    #region Properties

    /// <inheritdoc cref="IPartitionListener.PropertyAdded"/>
    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="NewValue"></param>
    record PropertyAddedArgs(IWritableNode Node, Property Property, PropertyValue NewValue);

    /// <seealso cref="IPartitionCommander.AddProperty"/>
    event EventHandler<PropertyAddedArgs> PropertyAdded;

    /// <inheritdoc cref="IPartitionListener.PropertyDeleted"/>
    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="OldValue"></param>
    record PropertyDeletedArgs(IWritableNode Node, Property Property, PropertyValue OldValue);

    /// <seealso cref="IPartitionCommander.DeleteProperty"/>
    event EventHandler<PropertyDeletedArgs> PropertyDeleted;

    /// <inheritdoc cref="IPartitionListener.PropertyChanged"/>
    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="NewValue"></param>
    /// <param name="OldValue"></param>
    record PropertyChangedArgs(IWritableNode Node, Property Property, PropertyValue NewValue, PropertyValue OldValue);

    /// <seealso cref="IPartitionCommander.ChangeProperty"/>
    event EventHandler<PropertyChangedArgs> PropertyChanged;

    #endregion

    #region Children

    /// <inheritdoc cref="IPartitionListener.ChildAdded"/>
    /// <param name="Parent"></param>
    /// <param name="NewChild"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    record ChildAddedArgs(IWritableNode Parent, IWritableNode NewChild, Containment Containment, Index Index);

    /// <seealso cref="IPartitionCommander.AddChild"/>
    event EventHandler<ChildAddedArgs> ChildAdded;

    /// <inheritdoc cref="IPartitionListener.ChildDeleted"/>
    /// <param name="DeletedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    record ChildDeletedArgs(IWritableNode DeletedChild, IWritableNode Parent, Containment Containment, Index Index);

    /// <seealso cref="IPartitionCommander.DeleteChild"/>
    event EventHandler<ChildDeletedArgs> ChildDeleted;

    /// <inheritdoc cref="IPartitionListener.ChildReplaced"/>
    /// <param name="NewChild"></param>
    /// <param name="ReplacedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    record ChildReplacedArgs(
        IWritableNode NewChild,
        IWritableNode ReplacedChild,
        IWritableNode Parent,
        Containment Containment,
        Index Index);

    /// <seealso cref="IPartitionCommander.ReplaceChild"/>
    event EventHandler<ChildReplacedArgs> ChildReplaced;

    /// <inheritdoc cref="IPartitionListener.ChildMovedFromOtherContainment"/>
    /// <param name="NewParent"></param>
    /// <param name="NewContainment"></param>
    /// <param name="NewIndex"></param>
    /// <param name="MovedChild"></param>
    /// <param name="OldParent"></param>
    /// <param name="OldContainment"></param>
    /// <param name="OldIndex"></param>
    record ChildMovedFromOtherContainmentArgs(
        IWritableNode NewParent,
        Containment NewContainment,
        Index NewIndex,
        IWritableNode MovedChild,
        IWritableNode OldParent,
        Containment OldContainment,
        Index OldIndex);

    /// <seealso cref="IPartitionCommander.MoveChildFromOtherContainment"/>
    event EventHandler<ChildMovedFromOtherContainmentArgs> ChildMovedFromOtherContainment;

    /// <inheritdoc cref="IPartitionListener.ChildMovedFromOtherContainmentInSameParent"/>
    /// <param name="NewContainment"></param>
    /// <param name="NewIndex"></param>
    /// <param name="MovedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="OldContainment"></param>
    /// <param name="OldIndex"></param>
    record ChildMovedFromOtherContainmentInSameParentArgs(
        Containment NewContainment,
        Index NewIndex,
        IWritableNode MovedChild,
        IWritableNode Parent,
        Containment OldContainment,
        Index OldIndex);

    /// <seealso cref="IPartitionCommander.CanRaiseMoveChildFromOtherContainmentInSameParent"/>
    event EventHandler<ChildMovedFromOtherContainmentInSameParentArgs> ChildMovedFromOtherContainmentInSameParent;

    /// <inheritdoc cref="IPartitionListener.ChildMovedInSameContainment"/>
    /// <param name="NewIndex"></param>
    /// <param name="MovedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="OldIndex"></param>
    record ChildMovedInSameContainmentArgs(
        Index NewIndex,
        IWritableNode MovedChild,
        IWritableNode Parent,
        Containment Containment,
        Index OldIndex);

    /// <seealso cref="IPartitionCommander.MoveChildInSameContainment"/>
    event EventHandler<ChildMovedInSameContainmentArgs> ChildMovedInSameContainment;

    #endregion

    #region Annotations

    /// <inheritdoc cref="IPartitionListener.AnnotationAdded"/>
    /// <param name="Parent"></param>
    /// <param name="NewAnnotation"></param>
    /// <param name="Index"></param>
    record AnnotationAddedArgs(IWritableNode Parent, IWritableNode NewAnnotation, Index Index);

    /// <seealso cref="IPartitionCommander.AddAnnotation"/>
    event EventHandler<AnnotationAddedArgs> AnnotationAdded;

    /// <inheritdoc cref="IPartitionListener.AnnotationDeleted"/>
    /// <param name="DeletedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    record AnnotationDeletedArgs(IWritableNode DeletedAnnotation, IWritableNode Parent, Index Index);

    /// <seealso cref="IPartitionCommander.DeleteAnnotation"/>
    event EventHandler<AnnotationDeletedArgs> AnnotationDeleted;

    /// <inheritdoc cref="IPartitionListener.AnnotationReplaced"/>
    /// <param name="NewAnnotation"></param>
    /// <param name="ReplacedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    record AnnotationReplacedArgs(
        IWritableNode NewAnnotation,
        IWritableNode ReplacedAnnotation,
        IWritableNode Parent,
        Index Index);

    /// <seealso cref="IPartitionCommander.ReplaceAnnotation"/>
    event EventHandler<AnnotationReplacedArgs> AnnotationReplaced;

    /// <inheritdoc cref="IPartitionListener.AnnotationMovedFromOtherParent"/>
    /// <param name="NewParent"></param>
    /// <param name="NewIndex"></param>
    /// <param name="MovedAnnotation"></param>
    /// <param name="OldParent"></param>
    /// <param name="OldIndex"></param>
    record AnnotationMovedFromOtherParentArgs(
        IWritableNode NewParent,
        Index NewIndex,
        IWritableNode MovedAnnotation,
        IWritableNode OldParent,
        Index OldIndex);

    /// <seealso cref="IPartitionCommander.MoveAnnotationFromOtherParent"/>
    event EventHandler<AnnotationMovedFromOtherParentArgs> AnnotationMovedFromOtherParent;

    /// <inheritdoc cref="IPartitionListener.remove_AnnotationMovedInSameParent"/>
    /// <param name="NewIndex"></param>
    /// <param name="MovedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="OldIndex"></param>
    record AnnotationMovedInSameParentArgs(
        Index NewIndex,
        IWritableNode MovedAnnotation,
        IWritableNode Parent,
        Index OldIndex);

    /// <seealso cref="IPartitionCommander.MoveAnnotationInSameParent"/>
    event EventHandler<AnnotationMovedInSameParentArgs> AnnotationMovedInSameParent;

    #endregion

    #region References

    /// <inheritdoc cref="IPartitionListener.ReferenceAdded"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    record ReferenceAddedArgs(IWritableNode Parent, Reference Reference, Index Index, IReferenceTarget NewTarget);

    /// <seealso cref="IPartitionCommander.AddReference"/>
    event EventHandler<ReferenceAddedArgs> ReferenceAdded;

    /// <inheritdoc cref="IPartitionListener.ReferenceDeleted"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="DeletedTarget"></param>
    record ReferenceDeletedArgs(IWritableNode Parent, Reference Reference, Index Index, IReferenceTarget DeletedTarget);

    /// <seealso cref="IPartitionCommander.DeleteReference"/>
    event EventHandler<ReferenceDeletedArgs> ReferenceDeleted;

    /// <inheritdoc cref="IPartitionListener.ReferenceChanged"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    /// <param name="ReplacedTarget"></param>
    record ReferenceChangedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        IReferenceTarget NewTarget,
        IReferenceTarget ReplacedTarget);

    /// <seealso cref="IPartitionCommander.ChangeReference"/>
    event EventHandler<ReferenceChangedArgs> ReferenceChanged;

    /// <inheritdoc cref="IPartitionListener.EntryMovedFromOtherReference"/>
    /// <param name="NewParent"></param>
    /// <param name="NewReference"></param>
    /// <param name="NewIndex"></param>
    /// <param name="OldParent"></param>
    /// <param name="OldReference"></param>
    /// <param name="OldIndex"></param>
    /// <param name="Target"></param>
    record EntryMovedFromOtherReferenceArgs(
        IWritableNode NewParent,
        Reference NewReference,
        Index NewIndex,
        IWritableNode OldParent,
        Reference OldReference,
        Index OldIndex,
        IReferenceTarget Target);

    /// <seealso cref="IPartitionCommander.MoveEntryFromOtherReference"/>
    event EventHandler<EntryMovedFromOtherReferenceArgs> EntryMovedFromOtherReference;

    /// <inheritdoc cref="IPartitionListener.EntryMovedFromOtherReferenceInSameParent"/>
    /// <param name="Parent"></param>
    /// <param name="NewReference"></param>
    /// <param name="NewIndex"></param>
    /// <param name="OldReference"></param>
    /// <param name="OldIndex"></param>
    /// <param name="Target"></param>
    record EntryMovedFromOtherReferenceInSameParentArgs(
        IWritableNode Parent,
        Reference NewReference,
        Index NewIndex,
        Reference OldReference,
        Index OldIndex,
        IReferenceTarget Target);

    /// <seealso cref="IPartitionCommander.MoveEntryFromOtherReferenceInSameParent"/>
    event EventHandler<EntryMovedFromOtherReferenceInSameParentArgs> EntryMovedFromOtherReferenceInSameParent;

    /// <inheritdoc cref="IPartitionListener.EntryMovedInSameReference"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="OldIndex"></param>
    /// <param name="NewIndex"></param>
    /// <param name="Target"></param>
    record EntryMovedInSameReferenceArgs(
        IWritableNode Parent,
        Reference Reference,
        Index OldIndex,
        Index NewIndex,
        IReferenceTarget Target);

    /// <seealso cref="IPartitionCommander.MoveEntryInSameReference"/>
    event EventHandler<EntryMovedInSameReferenceArgs> EntryMovedInSameReference;

    /// <inheritdoc cref="IPartitionListener.ReferenceResolveInfoAdded"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewResolveInfo"></param>
    /// <param name="Target"></param>
    record ReferenceResolveInfoAddedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        ResolveInfo NewResolveInfo,
        TargetNode Target);

    /// <seealso cref="IPartitionCommander.AddReferenceResolveInfo"/>
    event EventHandler<ReferenceResolveInfoAddedArgs> ReferenceResolveInfoAdded;

    /// <inheritdoc cref="IPartitionListener.ReferenceResolveInfoDeleted"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="Target"></param>
    /// <param name="DeletedResolveInfo"></param>
    record ReferenceResolveInfoDeletedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        TargetNode Target,
        ResolveInfo DeletedResolveInfo);

    /// <seealso cref="IPartitionCommander.DeleteReferenceResolveInfo"/>
    event EventHandler<ReferenceResolveInfoDeletedArgs> ReferenceResolveInfoDeleted;

    /// <inheritdoc cref="IPartitionListener.ReferenceResolveInfoChanged"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewResolveInfo"></param>
    /// <param name="Target"></param>
    /// <param name="ReplacedResolveInfo"></param>
    record ReferenceResolveInfoChangedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        ResolveInfo NewResolveInfo,
        TargetNode? Target,
        ResolveInfo ReplacedResolveInfo);

    /// <seealso cref="IPartitionCommander.ChangeReferenceResolveInfo"/>
    event EventHandler<ReferenceResolveInfoChangedArgs> ReferenceResolveInfoChanged;

    /// <inheritdoc cref="IPartitionListener.ReferenceTargetAdded"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    /// <param name="ResolveInfo"></param>
    record ReferenceTargetAddedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        TargetNode NewTarget,
        ResolveInfo ResolveInfo);

    /// <seealso cref="IPartitionCommander.AddReferenceTarget"/>
    event EventHandler<ReferenceTargetAddedArgs> ReferenceTargetAdded;

    /// <inheritdoc cref="IPartitionListener.ReferenceTargetDeleted"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="ResolveInfo"></param>
    /// <param name="DeletedTarget"></param>
    record ReferenceTargetDeletedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        ResolveInfo ResolveInfo,
        TargetNode DeletedTarget);

    /// <seealso cref="IPartitionCommander.DeleteReferenceTarget"/>
    event EventHandler<ReferenceTargetDeletedArgs> ReferenceTargetDeleted;

    /// <inheritdoc cref="IPartitionListener.ReferenceTargetChanged"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    /// <param name="ResolveInfo"></param>
    /// <param name="OldTarget"></param>
    record ReferenceTargetChangedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        TargetNode NewTarget,
        ResolveInfo? ResolveInfo,
        TargetNode OldTarget);

    /// <seealso cref="IPartitionCommander.ChangeReferenceTarget"/>
    event EventHandler<ReferenceTargetChangedArgs> ReferenceTargetChanged;

    #endregion
}

/// Raises events about <see cref="INode">nodes</see> and their <see cref="Feature">features</see>.
/// <seealso cref="IPartitionListener"/>
/// <seealso cref="PartitionEventHandler"/>
public interface IPartitionCommander
{
    #region Nodes

    /// <seealso cref="IPartitionListener.ClassifierChanged"/>
    void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier);

    /// Whether anybody would receive the <see cref="ChangeClassifier"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeClassifier"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeClassifier { get; }

    #endregion

    #region Properties

    /// <seealso cref="IPartitionListener.PropertyAdded"/>
    void AddProperty(IWritableNode node, Property property, PropertyValue newValue);

    /// Whether anybody would receive the <see cref="AddProperty"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddProperty"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddProperty { get; }

    /// <seealso cref="IPartitionListener.PropertyDeleted"/>
    void DeleteProperty(IWritableNode node, Property property, PropertyValue oldValue);

    /// Whether anybody would receive the <see cref="DeleteProperty"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteProperty"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteProperty { get; }

    /// <seealso cref="IPartitionListener.PropertyChanged"/>
    void ChangeProperty(IWritableNode node, Property property, PropertyValue newValue, PropertyValue oldValue);

    /// Whether anybody would receive the <see cref="ChangeProperty"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeProperty"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeProperty { get; }

    #endregion

    #region Children

    /// <seealso cref="IPartitionListener.ChildAdded"/>
    void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index);

    /// Whether anybody would receive the <see cref="AddChild"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddChild"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddChild { get; }

    /// <seealso cref="IPartitionListener.ChildDeleted"/>
    void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index);

    /// Whether anybody would receive the <see cref="DeleteChild"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteChild"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteChild { get; }

    /// <seealso cref="IPartitionListener.ChildReplaced"/>
    void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment, Index index);

    /// Whether anybody would receive the <see cref="ReplaceChild"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ReplaceChild"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseReplaceChild { get; }

    /// <seealso cref="IPartitionListener.ChildMovedFromOtherContainment"/>
    void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex);

    /// Whether anybody would receive the <see cref="MoveChildFromOtherContainment"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveChildFromOtherContainment"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveChildFromOtherContainment { get; }

    /// <seealso cref="IPartitionListener.ChildMovedFromOtherContainmentInSameParent"/>
    void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex, IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex);

    /// Whether anybody would receive the <see cref="MoveChildFromOtherContainmentInSameParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveChildFromOtherContainmentInSameParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveChildFromOtherContainmentInSameParent { get; }

    /// <seealso cref="IPartitionListener.ChildMovedInSameContainment"/>
    void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment, Index oldIndex);

    /// Whether anybody would receive the <see cref="MoveChildInSameContainment"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveChildInSameContainment"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveChildInSameContainment { get; }

    #endregion

    #region Annotations

    /// <seealso cref="IPartitionListener.AnnotationAdded"/>
    void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index);

    /// Whether anybody would receive the <see cref="AddAnnotation"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddAnnotation"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddAnnotation { get; }

    /// <seealso cref="IPartitionListener.AnnotationDeleted"/>
    void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index);

    /// Whether anybody would receive the <see cref="DeleteAnnotation"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteAnnotation"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteAnnotation { get; }

    /// <seealso cref="IPartitionListener.AnnotationReplaced"/>
    void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index);

    /// Whether anybody would receive the <see cref="ReplaceAnnotation"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ReplaceAnnotation"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseReplaceAnnotation { get; }

    /// <seealso cref="IPartitionListener.AnnotationMovedFromOtherParent"/>
    void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex);

    /// Whether anybody would receive the <see cref="MoveAnnotationFromOtherParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveAnnotationFromOtherParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveAnnotationFromOtherParent { get; }

    /// <seealso cref="IPartitionListener.AnnotationMovedInSameParent"/>
    void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex);

    /// Whether anybody would receive the <see cref="MoveAnnotationInSameParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveAnnotationInSameParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveAnnotationInSameParent { get; }

    #endregion

    #region References

    /// <seealso cref="IPartitionListener.ReferenceAdded"/>
    void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget);

    /// Whether anybody would receive the <see cref="AddReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddReference { get; }

    /// <seealso cref="IPartitionListener.ReferenceDeleted"/>
    void DeleteReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget);

    /// Whether anybody would receive the <see cref="DeleteReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteReference { get; }

    /// <seealso cref="IPartitionListener.ReferenceChanged"/>
    void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget);

    /// Whether anybody would receive the <see cref="ChangeReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeReference { get; }

    /// <seealso cref="IPartitionListener.EntryMovedFromOtherReference"/>
    void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target);

    /// Whether anybody would receive the <see cref="MoveEntryFromOtherReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveEntryFromOtherReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveEntryFromOtherReference { get; }

    /// <seealso cref="IPartitionListener.EntryMovedFromOtherReferenceInSameParent"/>
    void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target);

    /// Whether anybody would receive the <see cref="MoveEntryFromOtherReferenceInSameParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveEntryFromOtherReferenceInSameParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveEntryFromOtherReferenceInSameParent { get; }

    /// <seealso cref="IPartitionListener.EntryMovedInSameReference"/>
    void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target);

    /// Whether anybody would receive the <see cref="MoveEntryInSameReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveEntryInSameReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveEntryInSameReference { get; }

    /// <seealso cref="IPartitionListener.ReferenceResolveInfoAdded"/>
    void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        TargetNode target);

    /// Whether anybody would receive the <see cref="AddReferenceResolveInfo"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddReferenceResolveInfo"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddReferenceResolveInfo { get; }

    /// <seealso cref="IPartitionListener.ReferenceResolveInfoDeleted"/>
    void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, TargetNode target,
        ResolveInfo deletedResolveInfo);

    /// Whether anybody would receive the <see cref="DeleteReferenceResolveInfo"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteReferenceResolveInfo"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteReferenceResolveInfo { get; }

    /// <seealso cref="IPartitionListener.ReferenceResolveInfoChanged"/>
    void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        TargetNode? target, ResolveInfo replacedResolveInfo);

    /// Whether anybody would receive the <see cref="ChangeReferenceResolveInfo"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeReferenceResolveInfo"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeReferenceResolveInfo { get; }

    /// <seealso cref="IPartitionListener.ReferenceTargetAdded"/>
    void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo resolveInfo);

    /// Whether anybody would receive the <see cref="AddReferenceTarget"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddReferenceTarget"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddReferenceTarget { get; }

    /// <seealso cref="IPartitionListener.ReferenceTargetDeleted"/>
    void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        TargetNode deletedTarget);

    /// Whether anybody would receive the <see cref="DeleteReferenceTarget"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteReferenceTarget"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteReferenceTarget { get; }

    /// <seealso cref="IPartitionListener.ReferenceTargetChanged"/>
    void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo? resolveInfo, TargetNode oldTarget);

    /// Whether anybody would receive the <see cref="ChangeReferenceTarget"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeReferenceTarget"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeReferenceTarget { get; }

    #endregion
}

/// <summary>
/// Describes a reference target.
///
/// <para>
/// At least one of <see cref="ResolveInfo"/> and <see cref="Reference"/> MUST be non-null.
/// </para>
/// </summary>
/// <seealso cref="LionWeb.Core.Serialization.SerializedReferenceTarget"/>
public interface IReferenceTarget
{
    /// Textual hint that might be used to find the target node of this reference.
    ResolveInfo? ResolveInfo { get; }
    
    /// Target node of this reference.
    TargetNode? Reference { get; }
}

/// <inheritdoc cref="IReferenceTarget"/>
public readonly record struct ReferenceTarget(ResolveInfo? ResolveInfo, TargetNode? Reference) : IReferenceTarget;