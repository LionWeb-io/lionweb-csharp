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

/// Raises events about <see cref="INode">nodes</see> and their <see cref="Feature">features</see>.
/// <seealso cref="IPartitionListener"/>
/// <seealso cref="PartitionEventHandler"/>
public interface IPartitionCommander
{
    #region Nodes

    /// <seealso cref="IPartitionListener.ClassifierChanged"/>
    void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="ChangeClassifier"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeClassifier"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeClassifier { get; }

    #endregion

    #region Properties

    /// <seealso cref="IPartitionListener.PropertyAdded"/>
    void AddProperty(IWritableNode node, Property property, Object newValue, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="AddProperty"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddProperty"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddProperty { get; }

    /// <seealso cref="IPartitionListener.PropertyDeleted"/>
    void DeleteProperty(IWritableNode node, Property property, Object oldValue, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="DeleteProperty"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteProperty"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteProperty { get; }

    /// <seealso cref="IPartitionListener.PropertyChanged"/>
    void ChangeProperty(IWritableNode node, Property property, Object newValue, Object oldValue, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="ChangeProperty"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeProperty"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeProperty { get; }

    #endregion

    #region Children

    /// <seealso cref="IPartitionListener.ChildAdded"/>
    void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="AddChild"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddChild"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddChild { get; }

    /// <seealso cref="IPartitionListener.ChildDeleted"/>
    void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="DeleteChild"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteChild"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteChild { get; }

    /// <seealso cref="IPartitionListener.ChildReplaced"/>
    void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment, Index index, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="ReplaceChild"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ReplaceChild"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseReplaceChild { get; }

    /// <seealso cref="IPartitionListener.ChildMovedFromOtherContainment"/>
    void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveChildFromOtherContainment"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveChildFromOtherContainment"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveChildFromOtherContainment { get; }

    /// <seealso cref="IPartitionListener.ChildMovedFromOtherContainmentInSameParent"/>
    void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex, IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveChildFromOtherContainmentInSameParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveChildFromOtherContainmentInSameParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveChildFromOtherContainmentInSameParent { get; }

    /// <seealso cref="IPartitionListener.ChildMovedInSameContainment"/>
    void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment, Index oldIndex, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveChildInSameContainment"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveChildInSameContainment"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveChildInSameContainment { get; }

    #endregion

    #region Annotations

    /// <seealso cref="IPartitionListener.AnnotationAdded"/>
    void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="AddAnnotation"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddAnnotation"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddAnnotation { get; }

    /// <seealso cref="IPartitionListener.AnnotationDeleted"/>
    void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="DeleteAnnotation"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteAnnotation"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteAnnotation { get; }

    /// <seealso cref="IPartitionListener.AnnotationReplaced"/>
    void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="ReplaceAnnotation"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ReplaceAnnotation"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseReplaceAnnotation { get; }

    /// <seealso cref="IPartitionListener.AnnotationMovedFromOtherParent"/>
    void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveAnnotationFromOtherParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveAnnotationFromOtherParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveAnnotationFromOtherParent { get; }

    /// <seealso cref="IPartitionListener.AnnotationMovedInSameParent"/>
    void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveAnnotationInSameParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveAnnotationInSameParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveAnnotationInSameParent { get; }

    #endregion

    #region References

    /// <seealso cref="IPartitionListener.ReferenceAdded"/>
    void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="AddReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddReference { get; }

    /// <seealso cref="IPartitionListener.ReferenceDeleted"/>
    void DeleteReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="DeleteReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteReference { get; }

    /// <seealso cref="IPartitionListener.ReferenceChanged"/>
    void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="ChangeReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeReference { get; }

    /// <seealso cref="IPartitionListener.EntryMovedFromOtherReference"/>
    void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveEntryFromOtherReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveEntryFromOtherReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveEntryFromOtherReference { get; }

    /// <seealso cref="IPartitionListener.EntryMovedFromOtherReferenceInSameParent"/>
    void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveEntryFromOtherReferenceInSameParent"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveEntryFromOtherReferenceInSameParent"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveEntryFromOtherReferenceInSameParent { get; }

    /// <seealso cref="IPartitionListener.EntryMovedInSameReference"/>
    void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="MoveEntryInSameReference"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="MoveEntryInSameReference"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseMoveEntryInSameReference { get; }

    /// <seealso cref="IPartitionListener.ReferenceResolveInfoAdded"/>
    void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        IReadableNode target, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="AddReferenceResolveInfo"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddReferenceResolveInfo"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddReferenceResolveInfo { get; }

    /// <seealso cref="IPartitionListener.ReferenceResolveInfoDeleted"/>
    void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, IReadableNode target,
        ResolveInfo deletedResolveInfo, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="DeleteReferenceResolveInfo"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteReferenceResolveInfo"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteReferenceResolveInfo { get; }

    /// <seealso cref="IPartitionListener.ReferenceResolveInfoChanged"/>
    void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        IReadableNode? target, ResolveInfo replacedResolveInfo, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="ChangeReferenceResolveInfo"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeReferenceResolveInfo"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeReferenceResolveInfo { get; }

    /// <seealso cref="IPartitionListener.ReferenceTargetAdded"/>
    void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo resolveInfo, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="AddReferenceTarget"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddReferenceTarget"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddReferenceTarget { get; }

    /// <seealso cref="IPartitionListener.ReferenceTargetDeleted"/>
    void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        IReadableNode deletedTarget, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="DeleteReferenceTarget"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeleteReferenceTarget"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeleteReferenceTarget { get; }

    /// <seealso cref="IPartitionListener.ReferenceTargetChanged"/>
    void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo? resolveInfo, IReadableNode oldTarget, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="ChangeReferenceTarget"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="ChangeReferenceTarget"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseChangeReferenceTarget { get; }

    #endregion
}