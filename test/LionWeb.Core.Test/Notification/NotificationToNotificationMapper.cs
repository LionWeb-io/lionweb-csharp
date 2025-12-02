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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Notification.Pipe;

/// <summary>
/// This class provides a convenience notification to notification mapping layer between a local forest/partition and a forest/partition replicator.
/// It maps locally produced forest and partition notifications to new notifications by 
/// 1) Cloning newly introduced nodes, as they cannot be known in the local representation (i.e. they aren't present in the <paramref name="sharedNodeMap"/>).
/// 2) Replacing the existing nodes by their local equivalent (i.e. the value of the same node id in <paramref name="sharedNodeMap"/>).
/// </summary>
/// <param name="sharedNodeMap">Shared node map between all notification pipes </param>)
/// <seealso cref="SharedNodeMap"/>
public class NotificationToNotificationMapper(SharedNodeMap sharedNodeMap)
{
    public INotification Map(INotification notification) =>
        notification switch
        {
            PartitionAddedNotification a => OnPartitionAdded(a),
            PartitionDeletedNotification a => OnPartitionDeleted(a),
            PropertyAddedNotification a => OnPropertyAdded(a),
            PropertyDeletedNotification a => OnPropertyDeleted(a),
            PropertyChangedNotification a => OnPropertyChanged(a),
            ChildAddedNotification a => OnChildAdded(a),
            ChildDeletedNotification a => OnChildDeleted(a),
            ChildReplacedNotification a => OnChildReplaced(a),
            ChildMovedFromOtherContainmentNotification a => OnChildMovedFromOtherContainment(a),
            ChildMovedFromOtherContainmentInSameParentNotification a => OnChildMovedFromOtherContainmentInSameParent(a),
            ChildMovedInSameContainmentNotification a => OnChildMovedInSameContainment(a),
            ChildMovedAndReplacedFromOtherContainmentNotification a => OnChildMovedAndReplacedFromOtherContainment(a),
            ChildMovedAndReplacedFromOtherContainmentInSameParentNotification a => OnChildMovedAndReplacedFromOtherContainmentInSameParent(a),
            ChildMovedAndReplacedInSameContainmentNotification a => OnChildMovedAndReplacedInSameContainment(a),
            AnnotationAddedNotification a => OnAnnotationAdded(a),
            AnnotationDeletedNotification a => OnAnnotationDeleted(a),
            AnnotationReplacedNotification a => OnAnnotationReplaced(a),
            AnnotationMovedAndReplacedInSameParentNotification a => OnAnnotationMovedAndReplacedInSameParentNotification(a),
            AnnotationMovedFromOtherParentNotification a => OnAnnotationMovedFromOtherParent(a),
            AnnotationMovedAndReplacedFromOtherParentNotification a => OnAnnotationMovedAndReplacedFromOtherParent(a), 
            AnnotationMovedInSameParentNotification a => OnAnnotationMovedInSameParent(a),
            ReferenceAddedNotification a => OnReferenceAdded(a),
            ReferenceDeletedNotification a => OnReferenceDeleted(a),
            ReferenceChangedNotification a => OnReferenceChanged(a),
            EntryMovedInSameReferenceNotification a => OnEntryMovedInSameReference(a),
            _ => throw new ArgumentException($"{notification.GetType().Name} is not implemented")
        };

    #region Partitions

    private PartitionAddedNotification OnPartitionAdded(PartitionAddedNotification notification) =>
        new(
            CloneNode(notification.NewPartition),
            notification.NotificationId
        );

    private PartitionDeletedNotification OnPartitionDeleted(PartitionDeletedNotification notification) =>
        new(
            LookUpNode(notification.DeletedPartition),
            notification.NotificationId
        );

    #endregion

    #region Properties

    private PropertyAddedNotification OnPropertyAdded(PropertyAddedNotification notification)
    {
        var node = LookUpNode(notification.Node);

        return new(
            node,
            notification.Property,
            notification.NewValue,
            notification.NotificationId
        );
    }

    private PropertyDeletedNotification OnPropertyDeleted(PropertyDeletedNotification notification)
    {
        var node = LookUpNode(notification.Node);

        return new(
            node,
            notification.Property,
            notification.OldValue,
            notification.NotificationId
        );
    }

    private PropertyChangedNotification OnPropertyChanged(PropertyChangedNotification notification)
    {
        var node = LookUpNode(notification.Node);

        return new(
            node,
            notification.Property,
            notification.NewValue,
            notification.OldValue,
            notification.NotificationId
        );
    }

    #endregion

    #region Children

    private ChildAddedNotification OnChildAdded(ChildAddedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var newChild = CloneNode(notification.NewChild);

        return new(
            parent,
            newChild,
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );
    }

    private ChildDeletedNotification OnChildDeleted(ChildDeletedNotification notification)
    {
        var deletedChild = LookUpNode(notification.DeletedChild);
        var parent = LookUpNode(notification.Parent);

        return new(
            deletedChild,
            parent,
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );
    }

    private ChildReplacedNotification OnChildReplaced(ChildReplacedNotification notification)
    {
        var newChild = CloneNode(notification.NewChild);
        var replacedChild = LookUpNode(notification.ReplacedChild);
        var parent = LookUpNode(notification.Parent);

        return new(
            newChild,
            replacedChild,
            parent,
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );
    }

    private ChildMovedFromOtherContainmentNotification OnChildMovedFromOtherContainment(
        ChildMovedFromOtherContainmentNotification notification)
    {
        var movedChild = LookUpNode(notification.MovedChild);
        var newParent = LookUpNode(notification.NewParent);
        var oldParent = LookUpNode(notification.OldParent);

        return new ChildMovedFromOtherContainmentNotification(
            newParent,
            notification.NewContainment,
            notification.NewIndex,
            movedChild,
            oldParent,
            notification.OldContainment,
            notification.OldIndex,
            notification.NotificationId
        );
    }

    private ChildMovedAndReplacedFromOtherContainmentNotification OnChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentNotification notification)
    {
        var newParent = LookUpNode(notification.NewParent);
        var movedChild = LookUpNode(notification.MovedChild);
        var replacedChild = LookUpNode(notification.ReplacedChild);

        return new ChildMovedAndReplacedFromOtherContainmentNotification(
            newParent,
            notification.NewContainment,
            notification.NewIndex,
            movedChild,
            notification.OldParent,
            notification.OldContainment,
            notification.OldIndex,
            replacedChild,
            notification.NotificationId
        );
    }

    private ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        OnChildMovedAndReplacedFromOtherContainmentInSameParent(
            ChildMovedAndReplacedFromOtherContainmentInSameParentNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var movedChild = LookUpNode(notification.MovedChild);
        var replacedChild = LookUpNode(notification.ReplacedChild);

        return new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(
            notification.NewContainment,
            notification.NewIndex,
            movedChild,
            parent,
            notification.OldContainment,
            notification.OldIndex,
            replacedChild,
            notification.NotificationId
        );
    }

    private ChildMovedFromOtherContainmentInSameParentNotification OnChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var movedChild = LookUpNode(notification.MovedChild);

        return new ChildMovedFromOtherContainmentInSameParentNotification(
            notification.NewContainment,
            notification.NewIndex,
            movedChild,
            parent,
            notification.OldContainment,
            notification.OldIndex,
            notification.NotificationId
        );
    }

    private ChildMovedInSameContainmentNotification OnChildMovedInSameContainment(
        ChildMovedInSameContainmentNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var movedChild = LookUpNode(notification.MovedChild);

        return new ChildMovedInSameContainmentNotification(
            notification.NewIndex,
            movedChild,
            parent,
            notification.Containment,
            notification.OldIndex,
            notification.NotificationId
        );
    }

    private ChildMovedAndReplacedInSameContainmentNotification OnChildMovedAndReplacedInSameContainment(ChildMovedAndReplacedInSameContainmentNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var movedChild = LookUpNode(notification.MovedChild);
        var replacedChild = LookUpNode(notification.ReplacedChild);
        
        return new ChildMovedAndReplacedInSameContainmentNotification(
            notification.NewIndex,
            movedChild,
            parent,
            notification.Containment,
            replacedChild,
            notification.OldIndex,
            notification.NotificationId
        );
    }

    #endregion

    #region Annotations

    private AnnotationAddedNotification OnAnnotationAdded(AnnotationAddedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var newAnnotation = CloneNode(notification.NewAnnotation);

        return new(
            parent,
            newAnnotation,
            notification.Index,
            notification.NotificationId
        );
    }

    private AnnotationDeletedNotification OnAnnotationDeleted(AnnotationDeletedNotification notification)
    {
        var deletedAnnotation = LookUpNode(notification.DeletedAnnotation);
        var parent = LookUpNode(notification.Parent);

        return new(
            deletedAnnotation,
            parent,
            notification.Index,
            notification.NotificationId
        );
    }

    private AnnotationReplacedNotification OnAnnotationReplaced(AnnotationReplacedNotification notification)
    {
        var newAnnotation = CloneNode(notification.NewAnnotation);
        var replacedAnnotation = LookUpNode(notification.ReplacedAnnotation);
        var parent = LookUpNode(notification.Parent);

        return new AnnotationReplacedNotification(
            newAnnotation,
            replacedAnnotation,
            parent,
            notification.Index,
            notification.NotificationId);
    }

    private AnnotationMovedFromOtherParentNotification OnAnnotationMovedFromOtherParent(
        AnnotationMovedFromOtherParentNotification notification)
    {
        var newParent = LookUpNode(notification.NewParent);
        var movedAnnotation = LookUpNode(notification.MovedAnnotation);

        return new AnnotationMovedFromOtherParentNotification(
            newParent,
            notification.NewIndex,
            movedAnnotation,
            notification.OldParent,
            notification.OldIndex,
            notification.NotificationId
        );
    }
    
    private AnnotationMovedAndReplacedFromOtherParentNotification OnAnnotationMovedAndReplacedFromOtherParent(AnnotationMovedAndReplacedFromOtherParentNotification notification)
    {
        var newParent = LookUpNode(notification.NewParent);
        var oldParent = LookUpNode(notification.OldParent);
        var movedAnnotation = LookUpNode(notification.MovedAnnotation);
        var replacedAnnotation = LookUpNode(notification.ReplacedAnnotation);
        
        return new AnnotationMovedAndReplacedFromOtherParentNotification(
            newParent,
            notification.NewIndex,
            movedAnnotation,
            oldParent,
            notification.OldIndex,
            replacedAnnotation,
            notification.NotificationId);
    }

    private AnnotationMovedInSameParentNotification OnAnnotationMovedInSameParent(
        AnnotationMovedInSameParentNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var movedAnnotation = LookUpNode(notification.MovedAnnotation);

        return new AnnotationMovedInSameParentNotification(
            notification.NewIndex,
            movedAnnotation,
            parent,
            notification.OldIndex,
            notification.NotificationId
        );
    }
    
    private INotification OnAnnotationMovedAndReplacedInSameParentNotification(AnnotationMovedAndReplacedInSameParentNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var movedAnnotation = LookUpNode(notification.MovedAnnotation);
        var replacedAnnotation = LookUpNode(notification.ReplacedAnnotation);
        
        return new AnnotationMovedAndReplacedInSameParentNotification(
            notification.NewIndex,
            movedAnnotation,
            parent,
            notification.OldIndex,
            replacedAnnotation,
            notification.NotificationId
        );
    }

    #endregion

    #region References

    private ReferenceAddedNotification OnReferenceAdded(ReferenceAddedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var newTarget = LookUpNode(notification.NewTarget);

        return new(
            parent,
            notification.Reference,
            notification.Index,
            newTarget,
            notification.NotificationId
        );
    }

    private ReferenceDeletedNotification OnReferenceDeleted(ReferenceDeletedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var deletedTarget = LookUpNode(notification.DeletedTarget);

        return new(
            parent,
            notification.Reference,
            notification.Index,
            deletedTarget,
            notification.NotificationId
        );
    }

    private ReferenceChangedNotification OnReferenceChanged(ReferenceChangedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var newTarget = LookUpNode(notification.NewTarget);
        var oldTarget = LookUpNode(notification.OldTarget);

        return new(
            parent,
            notification.Reference,
            notification.Index,
            newTarget,
            oldTarget,
            notification.NotificationId
        );
    }
    
    private EntryMovedInSameReferenceNotification OnEntryMovedInSameReference(EntryMovedInSameReferenceNotification notification)
    {
        var parent = LookUpNode(notification.Parent);
        var target = LookUpNode(notification.Target);
        
        return new EntryMovedInSameReferenceNotification(
            parent, 
            notification.Reference,
            notification.OldIndex,
            notification.NewIndex,
            target,
            notification.NotificationId);
    }

    #endregion

    private T LookUpNode<T>(T node) where T : IReadableNode
    {
        var nodeId = node.GetId();
        if (sharedNodeMap.TryGetValue(nodeId, out var result))
        {
            if (result is T w)
                return w;

            throw new InvalidCastException($"Can not cast {result.GetType().Name} to {typeof(T).Name}");
        }

        throw new InvalidOperationException($"Unknown node with id: {nodeId}");
    }

    private ReferenceTarget LookUpNode(IReferenceTarget target)
    {
        var nodeId = target.TargetId;
        if (sharedNodeMap.TryGetValue(nodeId, out var result))
            return new ReferenceTarget(target.ResolveInfo, target.TargetId, result);

        throw new InvalidOperationException($"Unknown target node with id: {nodeId}");
    }

    private T CloneNode<T>(T node) where T : IReadableNode => (T)SameIdCloner.Clone((INode)node);
}