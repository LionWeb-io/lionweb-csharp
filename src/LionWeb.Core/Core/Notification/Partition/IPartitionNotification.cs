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

namespace LionWeb.Core.Notification.Partition;

using M3;
using TargetNode = IReadableNode;
using SemanticPropertyValue = object;

/// All LionWeb notifications relating to a partition.
public interface IPartitionNotification : INotification
{
    NodeId ContextNodeId { get; }
}

public abstract record APartitionNotification(INotificationId NotificationId) : IPartitionNotification
{
    /// <inheritdoc />
    public INotificationId NotificationId { get; set; } = NotificationId;

    /// <inheritdoc />
    public abstract HashSet<IReadableNode> AffectedNodes { get; }

    /// <inheritdoc />
    public abstract NodeId ContextNodeId { get; }
}

#region Nodes

/// <param name="Node"></param>
/// <param name="NewClassifier"></param>
/// <param name="OldClassifier"></param>
public record ClassifierChangedNotification(
    IWritableNode Node,
    Classifier NewClassifier,
    Classifier OldClassifier,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Node.GetId();
}

#endregion

#region Properties

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="NewValue"></param>
public record PropertyAddedNotification(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue NewValue,
    INotificationId NotificationId)
    : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Node.GetId();
}

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="OldValue"></param>
public record PropertyDeletedNotification(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue OldValue,
    INotificationId NotificationId)
    : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Node.GetId();
}

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="NewValue"></param>
/// <param name="OldValue"></param>
public record PropertyChangedNotification(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue NewValue,
    SemanticPropertyValue OldValue,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Node.GetId();
}

#endregion

#region Children

/// <param name="Parent"></param>
/// <param name="NewChild"></param>
/// <param name="Containment"></param>
/// <param name="Index"></param>
public record ChildAddedNotification(
    IWritableNode Parent,
    IWritableNode NewChild,
    Containment Containment,
    Index Index,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="DeletedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="Index"></param>
public record ChildDeletedNotification(
    IWritableNode DeletedChild,
    IWritableNode Parent,
    Containment Containment,
    Index Index,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewChild"></param>
/// <param name="ReplacedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="Index"></param>
public record ChildReplacedNotification(
    IWritableNode NewChild,
    IWritableNode ReplacedChild,
    IWritableNode Parent,
    Containment Containment,
    Index Index,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewParent"></param>
/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="OldParent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedFromOtherContainmentNotification(
    IWritableNode NewParent,
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode OldParent,
    Containment OldContainment,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [OldParent, NewParent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => NewParent.GetId();
}

/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedFromOtherContainmentInSameParentNotification(
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment OldContainment,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedInSameContainmentNotification(
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment Containment,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewParent"></param>
/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="OldParent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedAndReplacedFromOtherContainmentNotification(
    IWritableNode NewParent,
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode OldParent,
    Containment OldContainment,
    Index OldIndex,
    IWritableNode ReplacedChild,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [OldParent, NewParent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => NewParent.GetId();
}

/// <param name="NewContainment"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="OldContainment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(
    Containment NewContainment,
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment OldContainment,
    Index OldIndex,
    IWritableNode ReplacedChild,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewIndex"></param>
/// <param name="MovedChild"></param>
/// <param name="Parent"></param>
/// <param name="Containment"></param>
/// <param name="OldIndex"></param>
public record ChildMovedAndReplacedInSameContainmentNotification(
    Index NewIndex,
    IWritableNode MovedChild,
    IWritableNode Parent,
    Containment Containment,
    IWritableNode ReplacedChild,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

#endregion

#region Annotations

/// <param name="Parent"></param>
/// <param name="NewAnnotation"></param>
/// <param name="Index"></param>
public record AnnotationAddedNotification(
    IWritableNode Parent,
    IWritableNode NewAnnotation,
    Index Index,
    INotificationId NotificationId)
    : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="DeletedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="Index"></param>
public record AnnotationDeletedNotification(
    IWritableNode DeletedAnnotation,
    IWritableNode Parent,
    Index Index,
    INotificationId NotificationId)
    : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewAnnotation"></param>
/// <param name="ReplacedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="Index"></param>
public record AnnotationReplacedNotification(
    IWritableNode NewAnnotation,
    IWritableNode ReplacedAnnotation,
    IWritableNode Parent,
    Index Index,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewParent"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="OldParent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedFromOtherParentNotification(
    IWritableNode NewParent,
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode OldParent,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [OldParent, NewParent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => NewParent.GetId();
}

/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedInSameParentNotification(
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode Parent,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewParent"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="OldParent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedAndReplacedFromOtherParentNotification(
    IWritableNode NewParent,
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode OldParent,
    Index OldIndex,
    IWritableNode ReplacedAnnotation,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [OldParent, NewParent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => NewParent.GetId();
}

/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedAndReplacedInSameParentNotification(
    Index NewIndex,
    IWritableNode MovedAnnotation,
    IWritableNode Parent,
    Index OldIndex,
    IWritableNode ReplacedAnnotation,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

#endregion

#region References

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
public record ReferenceAddedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    IReferenceTarget NewTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="DeletedTarget"></param>
public record ReferenceDeletedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    IReferenceTarget DeletedTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
/// <param name="OldTarget"></param>
public record ReferenceChangedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    IReferenceTarget NewTarget,
    IReferenceTarget OldTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewParent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldParent"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedFromOtherReferenceNotification(
    IWritableNode NewParent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    IWritableNode OldParent,
    Reference OldReference,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [OldParent, NewParent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => NewParent.GetId();
}

/// <param name="Parent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedFromOtherReferenceInSameParentNotification(
    IWritableNode Parent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    Reference OldReference,
    Index OldIndex,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="OldIndex"></param>
/// <param name="NewIndex"></param>
/// <param name="Target"></param>
public record EntryMovedInSameReferenceNotification(
    IWritableNode Parent,
    Reference Reference,
    Index OldIndex,
    Index NewIndex,
    IReferenceTarget Target,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="NewParent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldParent"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedAndReplacedFromOtherReferenceNotification(
    IWritableNode NewParent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    IWritableNode OldParent,
    Reference OldReference,
    Index OldIndex,
    IReferenceTarget ReplacedTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [OldParent, NewParent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => NewParent.GetId();
}

/// <param name="Parent"></param>
/// <param name="NewReference"></param>
/// <param name="NewIndex"></param>
/// <param name="OldReference"></param>
/// <param name="OldIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedAndReplacedFromOtherReferenceInSameParentNotification(
    IWritableNode Parent,
    Reference NewReference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    Reference OldReference,
    Index OldIndex,
    IReferenceTarget ReplacedTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="OldIndex"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedTarget"></param>
public record EntryMovedAndReplacedInSameReferenceNotification(
    IWritableNode Parent,
    Reference Reference,
    Index NewIndex,
    IReferenceTarget MovedTarget,
    Index OldIndex,
    IReferenceTarget ReplacedTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewResolveInfo"></param>
/// <param name="Target"></param>
public record ReferenceResolveInfoAddedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode Target,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="Target"></param>
/// <param name="DeletedResolveInfo"></param>
public record ReferenceResolveInfoDeletedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    TargetNode Target,
    ResolveInfo DeletedResolveInfo,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewResolveInfo"></param>
/// <param name="Target"></param>
/// <param name="ReplacedResolveInfo"></param>
public record ReferenceResolveInfoChangedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode? Target,
    ResolveInfo ReplacedResolveInfo,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
/// <param name="ResolveInfo"></param>
public record ReferenceTargetAddedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo ResolveInfo,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="ResolveInfo"></param>
/// <param name="DeletedTarget"></param>
public record ReferenceTargetDeletedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    ResolveInfo ResolveInfo,
    TargetNode DeletedTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
/// <param name="ResolveInfo"></param>
/// <param name="OldTarget"></param>
public record ReferenceTargetChangedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo? ResolveInfo,
    TargetNode OldTarget,
    INotificationId NotificationId) : APartitionNotification(NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override NodeId ContextNodeId => Parent.GetId();
}

#endregion