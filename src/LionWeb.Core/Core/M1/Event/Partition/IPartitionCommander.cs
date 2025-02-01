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
/// <seealso cref="IPartitionPublisher"/>
/// <seealso cref="PartitionEventHandler"/>
public interface IPartitionCommander : ICommander<IPartitionEvent>
{
    #region Children

    /// <seealso cref="ChildAddedEvent"/>
    void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index,
        EventId? eventId = null);

    /// <seealso cref="ChildDeletedEvent"/>
    void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index,
        EventId? eventId = null);

    /// <seealso cref="ChildReplacedEvent"/>
    void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment, Index index, EventId? eventId = null);

    /// <seealso cref="ChildMovedFromOtherContainmentEvent"/>
    void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex,
        EventId? eventId = null);

    /// <seealso cref="ChildMovedFromOtherContainmentInSameParentEvent"/>
    void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex, IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex, EventId? eventId = null);

    /// <seealso cref="ChildMovedInSameContainmentEvent"/>
    void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment, Index oldIndex, EventId? eventId = null);

    #endregion

    #region References

    /// <seealso cref="ReferenceAddedEvent"/>
    void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        EventId? eventId = null);

    /// <seealso cref="ReferenceDeletedEvent"/>
    void DeleteReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget,
        EventId? eventId = null);

    /// <seealso cref="ReferenceChangedEvent"/>
    void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget, EventId? eventId = null);

    /// <seealso cref="EntryMovedFromOtherReferenceEvent"/>
    void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target,
        EventId? eventId = null);

    /// <seealso cref="EntryMovedFromOtherReferenceInSameParentEvent"/>
    void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target, EventId? eventId = null);

    /// <seealso cref="EntryMovedInSameReferenceEvent"/>
    void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target, EventId? eventId = null);

    /// <seealso cref="ReferenceResolveInfoAddedEvent"/>
    void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        IReadableNode target, EventId? eventId = null);

    /// <seealso cref="ReferenceResolveInfoDeletedEvent"/>
    void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, IReadableNode target,
        ResolveInfo deletedResolveInfo, EventId? eventId = null);

    /// <seealso cref="ReferenceResolveInfoChangedEvent"/>
    void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, ResolveInfo newResolveInfo,
        IReadableNode? target, ResolveInfo replacedResolveInfo, EventId? eventId = null);

    /// <seealso cref="ReferenceTargetAddedEvent"/>
    void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo resolveInfo, EventId? eventId = null);

    /// <seealso cref="ReferenceTargetDeletedEvent"/>
    void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        IReadableNode deletedTarget, EventId? eventId = null);

    /// <seealso cref="ReferenceTargetChangedEvent"/>
    void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, IReadableNode newTarget,
        ResolveInfo? resolveInfo, IReadableNode oldTarget, EventId? eventId = null);

    #endregion
}