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

    /// <inheritdoc cref="IPartitionPublisher.ClassifierChanged"/>
    /// <param name="Node"></param>
    /// <param name="NewClassifier"></param>
    /// <param name="OldClassifier"></param>
    record ClassifierChangedArgs(
        IWritableNode Node,
        Classifier NewClassifier,
        Classifier OldClassifier,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.ChangeClassifier"/>
    event EventHandler<ClassifierChangedArgs> ClassifierChanged;

    #endregion

    #region Properties

    /// <inheritdoc cref="IPartitionPublisher.PropertyAdded"/>
    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="NewValue"></param>
    record PropertyAddedArgs(IWritableNode Node, Property Property, SemanticPropertyValue NewValue, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.AddProperty"/>
    event EventHandler<PropertyAddedArgs> PropertyAdded;

    /// <inheritdoc cref="IPartitionPublisher.PropertyDeleted"/>
    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="OldValue"></param>
    record PropertyDeletedArgs(IWritableNode Node, Property Property, SemanticPropertyValue OldValue, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.DeleteProperty"/>
    event EventHandler<PropertyDeletedArgs> PropertyDeleted;

    /// <inheritdoc cref="IPartitionPublisher.PropertyChanged"/>
    /// <param name="Node"></param>
    /// <param name="Property"></param>
    /// <param name="NewValue"></param>
    /// <param name="OldValue"></param>
    record PropertyChangedArgs(
        IWritableNode Node,
        Property Property,
        SemanticPropertyValue NewValue,
        SemanticPropertyValue OldValue,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.ChangeProperty"/>
    event EventHandler<PropertyChangedArgs> PropertyChanged;

    #endregion

    #region Children

    /// <inheritdoc cref="IPartitionPublisher.ChildAdded"/>
    /// <param name="Parent"></param>
    /// <param name="NewChild"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    record ChildAddedArgs(
        IWritableNode Parent,
        IWritableNode NewChild,
        Containment Containment,
        Index Index,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.AddChild"/>
    event EventHandler<ChildAddedArgs> ChildAdded;

    /// <inheritdoc cref="IPartitionPublisher.ChildDeleted"/>
    /// <param name="DeletedChild"></param>
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    record ChildDeletedArgs(
        IWritableNode DeletedChild,
        IWritableNode Parent,
        Containment Containment,
        Index Index,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.DeleteChild"/>
    event EventHandler<ChildDeletedArgs> ChildDeleted;

    /// <inheritdoc cref="IPartitionPublisher.ChildReplaced"/>
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
        Index Index,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.ReplaceChild"/>
    event EventHandler<ChildReplacedArgs> ChildReplaced;

    /// <inheritdoc cref="IPartitionPublisher.ChildMovedFromOtherContainment"/>
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
        Index OldIndex,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.MoveChildFromOtherContainment"/>
    event EventHandler<ChildMovedFromOtherContainmentArgs> ChildMovedFromOtherContainment;

    /// <inheritdoc cref="IPartitionPublisher.ChildMovedFromOtherContainmentInSameParent"/>
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
        Index OldIndex,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.CanRaiseMoveChildFromOtherContainmentInSameParent"/>
    event EventHandler<ChildMovedFromOtherContainmentInSameParentArgs> ChildMovedFromOtherContainmentInSameParent;

    /// <inheritdoc cref="IPartitionPublisher.ChildMovedInSameContainment"/>
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
        Index OldIndex,
        EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.MoveChildInSameContainment"/>
    event EventHandler<ChildMovedInSameContainmentArgs> ChildMovedInSameContainment;

    #endregion

    #region Annotations

    /// <inheritdoc cref="IPartitionPublisher.AnnotationAdded"/>
    /// <param name="Parent"></param>
    /// <param name="NewAnnotation"></param>
    /// <param name="Index"></param>
    record AnnotationAddedArgs(IWritableNode Parent, IWritableNode NewAnnotation, Index Index, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.AddAnnotation"/>
    event EventHandler<AnnotationAddedArgs> AnnotationAdded;

    /// <inheritdoc cref="IPartitionPublisher.AnnotationDeleted"/>
    /// <param name="DeletedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    record AnnotationDeletedArgs(IWritableNode DeletedAnnotation, IWritableNode Parent, Index Index, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.DeleteAnnotation"/>
    event EventHandler<AnnotationDeletedArgs> AnnotationDeleted;

    /// <inheritdoc cref="IPartitionPublisher.AnnotationReplaced"/>
    /// <param name="NewAnnotation"></param>
    /// <param name="ReplacedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    record AnnotationReplacedArgs(
        IWritableNode NewAnnotation,
        IWritableNode ReplacedAnnotation,
        IWritableNode Parent,
        Index Index, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.ReplaceAnnotation"/>
    event EventHandler<AnnotationReplacedArgs> AnnotationReplaced;

    /// <inheritdoc cref="IPartitionPublisher.AnnotationMovedFromOtherParent"/>
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
        Index OldIndex, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.MoveAnnotationFromOtherParent"/>
    event EventHandler<AnnotationMovedFromOtherParentArgs> AnnotationMovedFromOtherParent;

    /// <inheritdoc cref="IPartitionPublisher.remove_AnnotationMovedInSameParent"/>
    /// <param name="NewIndex"></param>
    /// <param name="MovedAnnotation"></param>
    /// <param name="Parent"></param>
    /// <param name="OldIndex"></param>
    record AnnotationMovedInSameParentArgs(
        Index NewIndex,
        IWritableNode MovedAnnotation,
        IWritableNode Parent,
        Index OldIndex, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.MoveAnnotationInSameParent"/>
    event EventHandler<AnnotationMovedInSameParentArgs> AnnotationMovedInSameParent;

    #endregion

    #region References

    /// <inheritdoc cref="IPartitionPublisher.ReferenceAdded"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="NewTarget"></param>
    record ReferenceAddedArgs(IWritableNode Parent, Reference Reference, Index Index, IReferenceTarget NewTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.AddReference"/>
    event EventHandler<ReferenceAddedArgs> ReferenceAdded;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceDeleted"/>
    /// <param name="Parent"></param>
    /// <param name="Reference"></param>
    /// <param name="Index"></param>
    /// <param name="DeletedTarget"></param>
    record ReferenceDeletedArgs(IWritableNode Parent, Reference Reference, Index Index, IReferenceTarget DeletedTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.DeleteReference"/>
    event EventHandler<ReferenceDeletedArgs> ReferenceDeleted;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceChanged"/>
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
        IReferenceTarget ReplacedTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.ChangeReference"/>
    event EventHandler<ReferenceChangedArgs> ReferenceChanged;

    /// <inheritdoc cref="IPartitionPublisher.EntryMovedFromOtherReference"/>
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
        IReferenceTarget Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.MoveEntryFromOtherReference"/>
    event EventHandler<EntryMovedFromOtherReferenceArgs> EntryMovedFromOtherReference;

    /// <inheritdoc cref="IPartitionPublisher.EntryMovedFromOtherReferenceInSameParent"/>
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
        IReferenceTarget Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.MoveEntryFromOtherReferenceInSameParent"/>
    event EventHandler<EntryMovedFromOtherReferenceInSameParentArgs> EntryMovedFromOtherReferenceInSameParent;

    /// <inheritdoc cref="IPartitionPublisher.EntryMovedInSameReference"/>
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
        IReferenceTarget Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.MoveEntryInSameReference"/>
    event EventHandler<EntryMovedInSameReferenceArgs> EntryMovedInSameReference;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceResolveInfoAdded"/>
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
        TargetNode Target, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.AddReferenceResolveInfo"/>
    event EventHandler<ReferenceResolveInfoAddedArgs> ReferenceResolveInfoAdded;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceResolveInfoDeleted"/>
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
        ResolveInfo DeletedResolveInfo, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.DeleteReferenceResolveInfo"/>
    event EventHandler<ReferenceResolveInfoDeletedArgs> ReferenceResolveInfoDeleted;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceResolveInfoChanged"/>
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
        ResolveInfo ReplacedResolveInfo, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.ChangeReferenceResolveInfo"/>
    event EventHandler<ReferenceResolveInfoChangedArgs> ReferenceResolveInfoChanged;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceTargetAdded"/>
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
        ResolveInfo ResolveInfo, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.AddReferenceTarget"/>
    event EventHandler<ReferenceTargetAddedArgs> ReferenceTargetAdded;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceTargetDeleted"/>
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
        TargetNode DeletedTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.DeleteReferenceTarget"/>
    event EventHandler<ReferenceTargetDeletedArgs> ReferenceTargetDeleted;

    /// <inheritdoc cref="IPartitionPublisher.ReferenceTargetChanged"/>
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
        TargetNode OldTarget, EventId EventId) : EventArgsBase(EventId), IPartitionEvent;

    /// <seealso cref="IPartitionCommander.ChangeReferenceTarget"/>
    event EventHandler<ReferenceTargetChangedArgs> ReferenceTargetChanged;

    #endregion
}

public interface IPartitionEvent : IEvent;