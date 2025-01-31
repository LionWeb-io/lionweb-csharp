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
using TargetNode = IReadableNode;
using SemanticPropertyValue = object;

/// Provides events about <see cref="INode">nodes</see> and their <see cref="Feature">features</see>.
/// <seealso cref="IPartitionCommander"/>
/// <seealso cref="PartitionEventHandler"/>
public interface IPartitionPublisher : IPublisher<IPartitionEvent>
{
    #region Nodes

    /// <param name="Node"></param>
    /// <param name="NewClassifier"></param>
    /// <param name="OldClassifier"></param>
    /// <seealso cref="IPartitionCommander.ChangeClassifier"/>
    record ClassifierChangedArgs(
        IWritableNode Node,
        Classifier NewClassifier,
        Classifier OldClassifier,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    #endregion

    #region Properties

    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="NewValue"></param>
    /// <seealso cref="IPartitionCommander.AddProperty"/>
    record PropertyAddedArgs(IWritableNode Node, Property Property, SemanticPropertyValue NewValue, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="OldValue"></param>
    /// <seealso cref="IPartitionCommander.DeleteProperty"/>
    record PropertyDeletedArgs(IWritableNode Node, Property Property, SemanticPropertyValue OldValue, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <inheritdoc cref="IPartitionPublisher.PropertyChanged"/>
    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="NewValue"></param>
    /// <param name="OldValue"></param>
    /// <seealso cref="IPartitionCommander.ChangeProperty"/>
    record PropertyChangedArgs(
        IWritableNode Node,
        Property Property,
        SemanticPropertyValue NewValue,
        SemanticPropertyValue OldValue,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    #endregion

    #region Children

    /// <param name="Parent"></param>
    /// <param name="NewChild"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    /// <seealso cref="IPartitionCommander.AddChild"/>
    record ChildAddedArgs(
        IWritableNode Parent,
        IWritableNode NewChild,
        Containment Containment,
        Index Index,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="DeletedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    /// <seealso cref="IPartitionCommander.DeleteChild"/>
    record ChildDeletedArgs(
        IWritableNode DeletedChild,
        IWritableNode Parent,
        Containment Containment,
        Index Index,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewChild"></param>
    /// <param name="ReplacedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    /// <seealso cref="IPartitionCommander.ReplaceChild"/>
    record ChildReplacedArgs(
        IWritableNode NewChild,
        IWritableNode ReplacedChild,
        IWritableNode Parent,
        Containment Containment,
        Index Index,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewParent"></param>
    /// <param name="NewContainment"></param>
    /// <param name="NewIndex"></param>
    /// <param name="MovedChild"></param>
    /// <param name="OldParent"></param>
    /// <param name="OldContainment"></param>
    /// <param name="OldIndex"></param>
    /// <seealso cref="IPartitionCommander.MoveChildFromOtherContainment"/>
    record ChildMovedFromOtherContainmentArgs(
        IWritableNode NewParent,
        Containment NewContainment,
        Index NewIndex,
        IWritableNode MovedChild,
        IWritableNode OldParent,
        Containment OldContainment,
        Index OldIndex,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewContainment"></param>
    /// <param name="NewIndex"></param>
    /// <param name="MovedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="OldContainment"></param>
    /// <param name="OldIndex"></param>
    /// <seealso cref="IPartitionCommander.CanRaiseMoveChildFromOtherContainmentInSameParent"/>
    record ChildMovedFromOtherContainmentInSameParentArgs(
        Containment NewContainment,
        Index NewIndex,
        IWritableNode MovedChild,
        IWritableNode Parent,
        Containment OldContainment,
        Index OldIndex,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewIndex"></param>
    /// <param name="MovedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="OldIndex"></param>
    /// <seealso cref="IPartitionCommander.MoveChildInSameContainment"/>
    record ChildMovedInSameContainmentArgs(
        Index NewIndex,
        IWritableNode MovedChild,
        IWritableNode Parent,
        Containment Containment,
        Index OldIndex,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    #endregion

    #region Annotations

    /// <param name="Parent"></param>
    /// <param name="NewAnnotation"></param>
    /// <param name="Index"></param>
    /// <seealso cref="IPartitionCommander.AddAnnotation"/>
    record AnnotationAddedArgs(IWritableNode Parent, IWritableNode NewAnnotation, Index Index, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="DeletedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    /// <seealso cref="IPartitionCommander.DeleteAnnotation"/>
    record AnnotationDeletedArgs(IWritableNode DeletedAnnotation, IWritableNode Parent, Index Index, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewAnnotation"></param>
    /// <param name="ReplacedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    /// <seealso cref="IPartitionCommander.ReplaceAnnotation"/>
    record AnnotationReplacedArgs(
        IWritableNode NewAnnotation,
        IWritableNode ReplacedAnnotation,
        IWritableNode Parent,
        Index Index, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewParent"></param>
    /// <param name="NewIndex"></param>
    /// <param name="MovedAnnotation"></param>
    /// <param name="OldParent"></param>
    /// <param name="OldIndex"></param>
    /// <seealso cref="IPartitionCommander.MoveAnnotationFromOtherParent"/>
    record AnnotationMovedFromOtherParentArgs(
        IWritableNode NewParent,
        Index NewIndex,
        IWritableNode MovedAnnotation,
        IWritableNode OldParent,
        Index OldIndex, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewIndex"></param>
    /// <param name="MovedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="OldIndex"></param>
    /// <seealso cref="IPartitionCommander.MoveAnnotationInSameParent"/>
    record AnnotationMovedInSameParentArgs(
        Index NewIndex,
        IWritableNode MovedAnnotation,
        IWritableNode Parent,
        Index OldIndex, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    #endregion

    #region References

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    /// <seealso cref="IPartitionCommander.AddReference"/>
    record ReferenceAddedArgs(IWritableNode Parent, Reference Reference, Index Index, IReferenceTarget NewTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="DeletedTarget"></param>
    /// <seealso cref="IPartitionCommander.DeleteReference"/>
    record ReferenceDeletedArgs(IWritableNode Parent, Reference Reference, Index Index, IReferenceTarget DeletedTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    /// <param name="ReplacedTarget"></param>
    /// <seealso cref="IPartitionCommander.ChangeReference"/>
    record ReferenceChangedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        IReferenceTarget NewTarget,
        IReferenceTarget ReplacedTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="NewParent"></param>
    /// <param name="NewReference"></param>
    /// <param name="NewIndex"></param>
    /// <param name="OldParent"></param>
    /// <param name="OldReference"></param>
    /// <param name="OldIndex"></param>
    /// <param name="Target"></param>
    /// <seealso cref="IPartitionCommander.MoveEntryFromOtherReference"/>
    record EntryMovedFromOtherReferenceArgs(
        IWritableNode NewParent,
        Reference NewReference,
        Index NewIndex,
        IWritableNode OldParent,
        Reference OldReference,
        Index OldIndex,
        IReferenceTarget Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="NewReference"></param>
    /// <param name="NewIndex"></param>
    /// <param name="OldReference"></param>
    /// <param name="OldIndex"></param>
    /// <param name="Target"></param>
    /// <seealso cref="IPartitionCommander.MoveEntryFromOtherReferenceInSameParent"/>
    record EntryMovedFromOtherReferenceInSameParentArgs(
        IWritableNode Parent,
        Reference NewReference,
        Index NewIndex,
        Reference OldReference,
        Index OldIndex,
        IReferenceTarget Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="OldIndex"></param>
    /// <param name="NewIndex"></param>
    /// <param name="Target"></param>
    /// <seealso cref="IPartitionCommander.MoveEntryInSameReference"/>
    record EntryMovedInSameReferenceArgs(
        IWritableNode Parent,
        Reference Reference,
        Index OldIndex,
        Index NewIndex,
        IReferenceTarget Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewResolveInfo"></param>
    /// <param name="Target"></param>
    /// <seealso cref="IPartitionCommander.AddReferenceResolveInfo"/>
    record ReferenceResolveInfoAddedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        ResolveInfo NewResolveInfo,
        TargetNode Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="Target"></param>
    /// <param name="DeletedResolveInfo"></param>
    /// <seealso cref="IPartitionCommander.DeleteReferenceResolveInfo"/>
    record ReferenceResolveInfoDeletedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        TargetNode Target,
        ResolveInfo DeletedResolveInfo, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewResolveInfo"></param>
    /// <param name="Target"></param>
    /// <param name="ReplacedResolveInfo"></param>
    /// <seealso cref="IPartitionCommander.ChangeReferenceResolveInfo"/>
    record ReferenceResolveInfoChangedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        ResolveInfo NewResolveInfo,
        TargetNode? Target,
        ResolveInfo ReplacedResolveInfo, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    /// <param name="ResolveInfo"></param>
    /// <seealso cref="IPartitionCommander.AddReferenceTarget"/>
    record ReferenceTargetAddedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        TargetNode NewTarget,
        ResolveInfo ResolveInfo, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="ResolveInfo"></param>
    /// <param name="DeletedTarget"></param>
    /// <seealso cref="IPartitionCommander.DeleteReferenceTarget"/>
    record ReferenceTargetDeletedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        ResolveInfo ResolveInfo,
        TargetNode DeletedTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    /// <param name="ResolveInfo"></param>
    /// <param name="OldTarget"></param>
    /// <seealso cref="IPartitionCommander.ChangeReferenceTarget"/>
    record ReferenceTargetChangedArgs(
        IWritableNode Parent,
        Reference Reference,
        Index Index,
        TargetNode NewTarget,
        ResolveInfo? ResolveInfo,
        TargetNode OldTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    #endregion
}

public interface IPartitionEvent : IEvent;