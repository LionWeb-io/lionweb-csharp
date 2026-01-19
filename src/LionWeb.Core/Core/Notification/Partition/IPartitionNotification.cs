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

using M1;
using M3;
using SemanticPropertyValue = object;

/// All LionWeb notifications relating to a partition.
public interface IPartitionNotification : INotification
{
    NodeId ContextNodeId { get; }
    
    IWritableNode ContextNode { get; }
}

public abstract record APartitionNotification(INotificationId NotificationId) : IPartitionNotification
{
    /// <inheritdoc />
    public INotificationId NotificationId { get; set; } = NotificationId;

    /// <inheritdoc />
    public abstract HashSet<IReadableNode> AffectedNodes { get; }

    /// <inheritdoc />
    public NodeId ContextNodeId => ContextNode.GetId();

    /// <inheritdoc />
    public abstract IWritableNode ContextNode { get; }
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
    public override IWritableNode ContextNode => Node;
}

#endregion

#region Properties

public interface IPropertyNotification : IPartitionNotification
{
    Property Property { get; }
}

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="NewValue"></param>
public record PropertyAddedNotification(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue NewValue,
    INotificationId NotificationId)
    : APartitionNotification(NotificationId), IPropertyNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Node;
}

/// <param name="Node"></param>
/// <param name="Property"></param>
/// <param name="OldValue"></param>
public record PropertyDeletedNotification(
    IWritableNode Node,
    Property Property,
    SemanticPropertyValue OldValue,
    INotificationId NotificationId)
    : APartitionNotification(NotificationId), IPropertyNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Node;
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
    INotificationId NotificationId)
    : APartitionNotification(NotificationId),IPropertyNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Node;
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), INewNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode => NewChild;
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(DeletedChild, true, true);
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), INewNodeNotification, IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode =>  NewChild;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(ReplacedChild, true, true);
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
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;
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
    public override IWritableNode ContextNode => Parent;
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
    public override IWritableNode ContextNode => Parent;
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(ReplacedChild, true, true);
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(ReplacedChild, true, true);
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(ReplacedChild, true, true);
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
    : APartitionNotification(NotificationId), INewNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode => NewAnnotation;
}

/// <param name="DeletedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="Index"></param>
public record AnnotationDeletedNotification(
    IWritableNode DeletedAnnotation,
    IWritableNode Parent,
    Index Index,
    INotificationId NotificationId)
    : APartitionNotification(NotificationId), IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(DeletedAnnotation, true, true);
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), INewNodeNotification, IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode => NewAnnotation;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(ReplacedAnnotation, true, true);
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
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;
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
    public override IWritableNode ContextNode => Parent;
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(ReplacedAnnotation, true, true);
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
    INotificationId NotificationId) : APartitionNotification(NotificationId), IDeletedNodeNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IEnumerable<IReadableNode> DeletedNodes =>
        M1Extensions.Descendants<IReadableNode>(ReplacedAnnotation, true, true);
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
    public override IWritableNode ContextNode => Parent;
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
    public override IWritableNode ContextNode => Parent;
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
    public override IWritableNode ContextNode => Parent;
}

#endregion