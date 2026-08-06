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
using Pipe;
using SemanticPropertyValue = object;

/// All LionWeb notifications relating to a partition.
public interface IPartitionNotification : INotification
{
    NodeId ContextNodeId { get; }

    IWritableNode ContextNode { get; }
}

public abstract record APartitionNotification(
    INotificationId NotificationId
) : IPartitionNotification
{
    /// <inheritdoc />
    public INotificationId NotificationId { get; set; } = NotificationId;

    /// <inheritdoc />
    public abstract HashSet<IReadableNode> AffectedNodes { get; }

    /// <inheritdoc />
    public NodeId ContextNodeId => ContextNode.GetId();

    /// <inheritdoc />
    public abstract IWritableNode ContextNode { get; }

    /// <inheritdoc />
    public virtual void Freeze() { }
}

#region Nodes

/// <param name="Node"></param>
/// <param name="NewClassifier"></param>
/// <param name="OldClassifier"></param>
public record ClassifierChangedNotification(
    IWritableNode Node,
    Classifier NewClassifier,
    Classifier OldClassifier,
    INotificationId NotificationId
) : APartitionNotification(NotificationId)
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IPropertyNotification
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IPropertyNotification
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IPropertyNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Node];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Node;
}

#endregion

#region Children

public interface IChildNotification : IPartitionNotification
{
    Containment Containment { get; }
}

/// <param name="Parent"></param>
/// <param name="NewChild"></param>
/// <param name="Containment"></param>
/// <param name="Index"></param>
public record ChildAddedNotification(
    IWritableNode Parent,
    IWritableNode NewChild,
    Containment Containment,
    Index Index,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), INewNodeNotification, IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, NewChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode => NewChild;

    public IWritableNode? FrozenNewChild { get; private set; }

    /// <inheritdoc />
    public override void Freeze() =>
        FrozenNewChild ??= SameIdCloner.Clone((INode)NewChild);
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IDeletedNodeNotification, IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, DeletedChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;
    private IReadOnlyList<IReadableNode> CollectDeleted() => IDeletedNodeNotification.CollectDeleted(DeletedChild);

    /// <inheritdoc />
    public override void Freeze() => _deletedNodes ??= CollectDeleted();
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), INewNodeNotification, IDeletedNodeNotification, IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, NewChild, ReplacedChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode => NewChild;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;
    private IReadOnlyList<IReadableNode> CollectDeleted() => IDeletedNodeNotification.CollectDeleted(ReplacedChild);

    public IWritableNode? FrozenNewChild { get; private set; }

    /// <inheritdoc />
    public override void Freeze()
    {
        _deletedNodes ??= CollectDeleted();
        FrozenNewChild ??= SameIdCloner.Clone((INode)NewChild);
    }
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent, MovedChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;

    /// <inheritdoc />
    public Containment Containment => NewContainment;
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, MovedChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public Containment Containment => NewContainment;
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
    IndexOffset IndexOffset,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, MovedChild];

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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IDeletedNodeNotification, IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent, MovedChild, ReplacedChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;

    /// <inheritdoc />
    public Containment Containment => NewContainment;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;
    private IReadOnlyList<IReadableNode> CollectDeleted() => IDeletedNodeNotification.CollectDeleted(ReplacedChild);

    /// <inheritdoc />
    public override void Freeze() => _deletedNodes ??= CollectDeleted();
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
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IDeletedNodeNotification, IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, MovedChild, ReplacedChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public Containment Containment => NewContainment;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;
    private IReadOnlyList<IReadableNode> CollectDeleted() => IDeletedNodeNotification.CollectDeleted(ReplacedChild);

    /// <inheritdoc />
    public override void Freeze() => _deletedNodes ??= CollectDeleted();
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
    IndexOffset IndexOffset,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IDeletedNodeNotification, IChildNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, MovedChild, ReplacedChild];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;
    private IReadOnlyList<IReadableNode> CollectDeleted() => IDeletedNodeNotification.CollectDeleted(ReplacedChild);

    /// <inheritdoc />
    public override void Freeze() => _deletedNodes ??= CollectDeleted();
}

#endregion

#region Annotations

public interface IAnnotationNotification : IPartitionNotification;

/// <param name="Parent"></param>
/// <param name="NewAnnotation"></param>
/// <param name="Index"></param>
public record AnnotationAddedNotification(
    IWritableNode Parent,
    IWritableAnnotationInstance NewAnnotation,
    Index Index,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), INewNodeNotification, IAnnotationNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, NewAnnotation];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode => NewAnnotation;

    public IWritableAnnotationInstance? FrozenNewAnnotation { get; private set; }

    /// <inheritdoc />
    public override void Freeze() =>
        FrozenNewAnnotation ??= (IWritableAnnotationInstance)SameIdCloner.Clone((INode)NewAnnotation);
}

/// <param name="DeletedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="Index"></param>
public record AnnotationDeletedNotification(
    IWritableAnnotationInstance DeletedAnnotation,
    IWritableNode Parent,
    Index Index,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IDeletedNodeNotification, IAnnotationNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, DeletedAnnotation];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;
    private IReadOnlyList<IReadableNode> CollectDeleted() => IDeletedNodeNotification.CollectDeleted(DeletedAnnotation);

    /// <inheritdoc />
    public override void Freeze() => _deletedNodes ??= CollectDeleted();
}

/// <param name="NewAnnotation"></param>
/// <param name="ReplacedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="Index"></param>
public record AnnotationReplacedNotification(
    IWritableAnnotationInstance NewAnnotation,
    IWritableAnnotationInstance ReplacedAnnotation,
    IWritableNode Parent,
    Index Index,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), INewNodeNotification, IDeletedNodeNotification, IAnnotationNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, NewAnnotation, ReplacedAnnotation];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadableNode NewNode => NewAnnotation;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;

    private IReadOnlyList<IReadableNode> CollectDeleted() =>
        IDeletedNodeNotification.CollectDeleted(ReplacedAnnotation);

    public IWritableAnnotationInstance? FrozenNewAnnotation { get; private set; }

    /// <inheritdoc />
    public override void Freeze()
    {
        _deletedNodes ??= CollectDeleted();
        FrozenNewAnnotation ??= (IWritableAnnotationInstance)SameIdCloner.Clone((INode)NewAnnotation);
    }
}

/// <param name="NewParent"></param>
/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="OldParent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedFromOtherParentNotification(
    IWritableNode NewParent,
    Index NewIndex,
    IWritableAnnotationInstance MovedAnnotation,
    IWritableNode OldParent,
    Index OldIndex,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IAnnotationNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent, MovedAnnotation];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;
}

/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedInSameParentNotification(
    Index NewIndex,
    IWritableAnnotationInstance MovedAnnotation,
    IWritableNode Parent,
    Index OldIndex,
    IndexOffset IndexOffset,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IAnnotationNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, MovedAnnotation];

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
    IWritableAnnotationInstance MovedAnnotation,
    IWritableNode OldParent,
    Index OldIndex,
    IWritableAnnotationInstance ReplacedAnnotation,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IDeletedNodeNotification, IAnnotationNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [NewParent, OldParent, MovedAnnotation, ReplacedAnnotation];

    /// <inheritdoc />
    public override IWritableNode ContextNode => NewParent;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;

    private IReadOnlyList<IReadableNode> CollectDeleted() =>
        IDeletedNodeNotification.CollectDeleted(ReplacedAnnotation);

    /// <inheritdoc />
    public override void Freeze() => _deletedNodes ??= CollectDeleted();
}

/// <param name="NewIndex"></param>
/// <param name="MovedAnnotation"></param>
/// <param name="Parent"></param>
/// <param name="OldIndex"></param>
public record AnnotationMovedAndReplacedInSameParentNotification(
    Index NewIndex,
    IWritableAnnotationInstance MovedAnnotation,
    IWritableNode Parent,
    Index OldIndex,
    IndexOffset IndexOffset,
    IWritableAnnotationInstance ReplacedAnnotation,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IDeletedNodeNotification, IAnnotationNotification
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes => [Parent, MovedAnnotation, ReplacedAnnotation];

    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    /// <inheritdoc />
    public IReadOnlyList<IReadableNode> DeletedNodes => _deletedNodes ?? CollectDeleted();

    private IReadOnlyList<IReadableNode>? _deletedNodes;

    private IReadOnlyList<IReadableNode> CollectDeleted() =>
        IDeletedNodeNotification.CollectDeleted(ReplacedAnnotation);

    /// <inheritdoc />
    public override void Freeze() => _deletedNodes ??= CollectDeleted();
}

#endregion

#region References

public interface IReferenceNotification : IPartitionNotification
{
    IWritableNode Parent { get; }
    Reference Reference { get; }
    Index Index { get; }
    IReferenceTarget Target { get; }
}

/// <inheritdoc cref="IReferenceNotification" />
public abstract record AReferenceNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    INotificationId NotificationId
) : APartitionNotification(NotificationId), IReferenceNotification
{
    /// <inheritdoc />
    public override IWritableNode ContextNode => Parent;

    protected static HashSet<IReadableNode> ConcatTarget(HashSet<IReadableNode> nodes, IReferenceTarget target)
    {
        if (target.Target is { } t)
            nodes.Add(t);

        return nodes;
    }

    /// <inheritdoc />
    public abstract IReferenceTarget Target { get; }
}

/// <param name="Parent"></param>
/// <param name="Reference"></param>
/// <param name="Index"></param>
/// <param name="NewTarget"></param>
public record ReferenceAddedNotification(
    IWritableNode Parent,
    Reference Reference,
    Index Index,
    IReferenceTarget NewTarget,
    INotificationId NotificationId
) : AReferenceNotification(Parent, Reference, Index, NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes =>
        ConcatTarget([Parent], NewTarget);

    /// <inheritdoc />
    public override IReferenceTarget Target => NewTarget;
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
    INotificationId NotificationId
) : AReferenceNotification(Parent, Reference, Index, NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes =>
        ConcatTarget([Parent], DeletedTarget);

    /// <inheritdoc />
    public override IReferenceTarget Target => DeletedTarget;
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
    INotificationId NotificationId
) : AReferenceNotification(Parent, Reference, Index, NotificationId)
{
    /// <inheritdoc />
    public override HashSet<IReadableNode> AffectedNodes =>
        ConcatTarget(ConcatTarget([Parent], OldTarget), NewTarget);

    /// <inheritdoc />
    public override IReferenceTarget Target => OldTarget;
}

#endregion