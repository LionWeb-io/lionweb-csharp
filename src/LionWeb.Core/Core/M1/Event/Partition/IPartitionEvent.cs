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

/// All LionWeb events relating to a partition.
public interface IPartitionEvent : IEvent;

#region Nodes

/// <param name="Node"></param>
/// <param name="NewClassifier"></param>
/// <param name="OldClassifier"></param>
public record ClassifierChangedEvent(
    IWritableNode Node,
    Classifier NewClassifier,
    Classifier OldClassifier,
    IEventId EventId) : IPartitionEvent;

#endregion

#region Properties

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="NewValue"></param>
public record PropertyAddedEvent(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue NewValue,
    IEventId EventId)
    : IPartitionEvent;

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="OldValue"></param>
public record PropertyDeletedEvent(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue OldValue,
    IEventId EventId)
    : IPartitionEvent;

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="NewValue"></param>
/// <param name="OldValue"></param>
public record PropertyChangedEvent(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue NewValue,
    SemanticPropertyValue OldValue,
    IEventId EventId) : IPartitionEvent;

#endregion

#region Children

/// <param name="Parent"></param>
/// <param name="NewChild"></param>
/// <param name="Containment"></param>
/// <param name="Index"></param>
public record ChildAddedEvent(
    IWritableNode Parent,
    IWritableNode NewChild,
    Containment Containment,
    Index Index,
    IEventId EventId) : IPartitionEvent;

/// <param name="DeletedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="Index"></param>
public record ChildDeletedEvent(
    IWritableNode DeletedChild,
    IWritableNode Parent,
    Containment Containment,
    Index Index,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewChild"></param>
/// <param name="ReplacedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="Index"></param>
public record ChildReplacedEvent(
    IWritableNode NewChild,
    IWritableNode ReplacedChild,
    IWritableNode Parent,
    Containment Containment,
    Index Index,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewParent"></param>
/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="OldParent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedFromOtherContainmentEvent(
    IWritableNode NewParent,
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode OldParent,
    Containment OldContainment,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedFromOtherContainmentInSameParentEvent(
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment OldContainment,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedInSameContainmentEvent(
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment Containment,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewParent"></param>
/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="OldParent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedAndReplacedFromOtherContainmentEvent(
    IWritableNode NewParent,
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode OldParent,
    Containment OldContainment,
    Index OldIndex,
    IWritableNode ReplacedChild,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedAndReplacedFromOtherContainmentInSameParentEvent(
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment OldContainment,
    Index OldIndex,
    IWritableNode ReplacedChild,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedAndReplacedInSameContainmentEvent(
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment Containment,
    IWritableNode ReplacedChild,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

#endregion

#region Annotations

/// <param name="Parent"></param>
/// <param name="NewAnnotation"></param>
/// <param name="Index"></param>
public record AnnotationAddedEvent(
    IWritableNode Parent,
    IWritableNode NewAnnotation,
    Index Index,
    IEventId EventId)
    : IPartitionEvent;

/// <param name="DeletedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="Index"></param>
public record AnnotationDeletedEvent(
    IWritableNode DeletedAnnotation,
    IWritableNode Parent,
    Index Index,
    IEventId EventId)
    : IPartitionEvent;

/// <param name="NewAnnotation"></param>
/// <param name="ReplacedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="Index"></param>
public record AnnotationReplacedEvent(
    IWritableNode NewAnnotation,
    IWritableNode ReplacedAnnotation,
    IWritableNode Parent,
    Index Index,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewParent"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="OldParent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedFromOtherParentEvent(
    IWritableNode NewParent,
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode OldParent,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedInSameParentEvent(
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode Parent,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewParent"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="OldParent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedAndReplacedFromOtherParentEvent(
    IWritableNode NewParent,
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode OldParent,
    Index OldIndex,
    IWritableNode ReplacedAnnotation,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedAndReplacedInSameParentEvent(
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode Parent,
    Index OldIndex,
    IWritableNode ReplacedAnnotation,
    IEventId EventId) : IPartitionEvent;

#endregion

#region References

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
public record ReferenceAddedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    IReferenceTarget NewTarget,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="DeletedTarget"></param>
public record ReferenceDeletedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    IReferenceTarget DeletedTarget,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
/// <param name="ReplacedTarget"></param>
public record ReferenceChangedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    IReferenceTarget NewTarget,
    IReferenceTarget ReplacedTarget,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewParent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldParent"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedFromOtherReferenceEvent(
    IWritableNode NewParent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    IWritableNode OldParent,
    Reference OldReference,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedFromOtherReferenceInSameParentEvent(
    IWritableNode Parent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    Reference OldReference,
    Index OldIndex,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="OldIndex"></param>
/// <param name="NewIndex"></param>
/// <param name="Target"></param>
public record EntryMovedInSameReferenceEvent(
    IWritableNode Parent,
    Reference Reference,
    Index OldIndex,
    Index NewIndex,
    IReferenceTarget Target,
    IEventId EventId) : IPartitionEvent;

/// <param name="NewParent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldParent"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedAndReplacedFromOtherReferenceEvent(
    IWritableNode NewParent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    IWritableNode OldParent,
    Reference OldReference,
    Index OldIndex,
    IReferenceTarget ReplacedTarget,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedAndReplacedFromOtherReferenceInSameParentEvent(
    IWritableNode Parent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    Reference OldReference,
    Index OldIndex,
    IReferenceTarget ReplacedTarget,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="OldIndex"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedAndReplacedInSameReferenceEvent(
    IWritableNode Parent,
    Reference Reference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    Index OldIndex,
    IReferenceTarget ReplacedTarget,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewResolveInfo"></param>
/// <param name="Target"></param>
public record ReferenceResolveInfoAddedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode Target,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="Target"></param>
/// <param name="DeletedResolveInfo"></param>
public record ReferenceResolveInfoDeletedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    TargetNode Target,
    ResolveInfo DeletedResolveInfo,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewResolveInfo"></param>
/// <param name="Target"></param>
/// <param name="ReplacedResolveInfo"></param>
public record ReferenceResolveInfoChangedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode? Target,
    ResolveInfo ReplacedResolveInfo,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
/// <param name="ResolveInfo"></param>
public record ReferenceTargetAddedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo ResolveInfo,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="ResolveInfo"></param>
/// <param name="DeletedTarget"></param>
public record ReferenceTargetDeletedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    ResolveInfo ResolveInfo,
    TargetNode DeletedTarget,
    IEventId EventId) : IPartitionEvent;

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
/// <param name="ResolveInfo"></param>
/// <param name="OldTarget"></param>
public record ReferenceTargetChangedEvent(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo? ResolveInfo,
    TargetNode OldTarget,
    IEventId EventId) : IPartitionEvent;

#endregion