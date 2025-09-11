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
using M1;
using M3;

public class NotificationToNotificationMapper(
    SharedNodeMap sharedNodeMap,
    Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap)
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
            AnnotationAddedNotification a => OnAnnotationAdded(a),
            AnnotationDeletedNotification a => OnAnnotationDeleted(a),
            AnnotationMovedFromOtherParentNotification a => OnAnnotationMovedFromOtherParent(a),
            AnnotationMovedInSameParentNotification a => OnAnnotationMovedInSameParent(a),
            ReferenceAddedNotification a => OnReferenceAdded(a),
            ReferenceDeletedNotification a => OnReferenceDeleted(a),
            ReferenceChangedNotification a => OnReferenceChanged(a),
            _ => throw new NotImplementedException(notification.GetType().Name)
        };

    #region Partitions

    private PartitionAddedNotification OnPartitionAdded(PartitionAddedNotification notification) =>
        new(
            notification.NewPartition,
            notification.NotificationId
        );

    private PartitionDeletedNotification OnPartitionDeleted(PartitionDeletedNotification notification) =>
        new(
            notification.DeletedPartition,
            notification.NotificationId
        );

    #endregion

    #region Properties

    private PropertyAddedNotification OnPropertyAdded(PropertyAddedNotification notification) =>
        new(
            NodeCloner(notification.Node),
            notification.Property,
            notification.NewValue,
            notification.NotificationId
        );

    private PropertyDeletedNotification OnPropertyDeleted(PropertyDeletedNotification notification) =>
        new(
            NodeCloner(notification.Node),
            notification.Property,
            notification.OldValue,
            notification.NotificationId
        );

    private PropertyChangedNotification OnPropertyChanged(PropertyChangedNotification notification) =>
        new(
            NodeCloner(notification.Node),
            notification.Property,
            notification.NewValue,
            notification.OldValue,
            notification.NotificationId
        );

    #endregion


    #region Children

    private ChildAddedNotification OnChildAdded(ChildAddedNotification notification) =>
        new(
            NodeCloner(notification.Parent),
            NodeCloner(notification.NewChild),
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );

    private ChildDeletedNotification OnChildDeleted(ChildDeletedNotification notification) =>
        new(
            notification.DeletedChild,
            NodeCloner(notification.Parent),
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );

    private ChildReplacedNotification OnChildReplaced(ChildReplacedNotification notification) =>
        new(
            NodeCloner(notification.NewChild),
            NodeCloner(notification.ReplacedChild),
            notification.Parent,
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );

    private ChildMovedFromOtherContainmentNotification OnChildMovedFromOtherContainment(
        ChildMovedFromOtherContainmentNotification notification)
    {
        var movedChild = NodeCloner(notification.MovedChild);
        var oldParent = NodeCloner(notification.OldParent);
        var newParent = notification.NewParent;

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
        var movedChild = NodeCloner(notification.MovedChild);
        var newParent = notification.NewParent; //NodeCloner(notification.NewParent); //TODO: test: moved child is child of partition or subtree
        var replacedChild = NodeCloner(notification.ReplacedChild);

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
        var parent = NodeCloner(notification.Parent);
        var movedChild = NodeCloner(notification.MovedChild);
        var replacedChild = NodeCloner(notification.ReplacedChild);

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
        var parent = NodeCloner(notification.Parent);
        var movedChild = NodeCloner(notification.MovedChild);
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
        var parent = NodeCloner(notification.Parent);
        var movedChild = NodeCloner(notification.MovedChild);
        return new ChildMovedInSameContainmentNotification(
            notification.NewIndex,
            movedChild,
            parent,
            notification.Containment,
            notification.OldIndex,
            notification.NotificationId
        );
    }

    #endregion


    #region Annotations

    private AnnotationAddedNotification OnAnnotationAdded(AnnotationAddedNotification notification) =>
        new(
            NodeCloner(notification.Parent),
            NodeCloner(notification.NewAnnotation),
            notification.Index,
            notification.NotificationId
        );

    private AnnotationDeletedNotification OnAnnotationDeleted(AnnotationDeletedNotification notification) =>
        new(
            NodeCloner(notification.DeletedAnnotation),
            NodeCloner(notification.Parent),
            notification.Index,
            notification.NotificationId
        );

    private AnnotationMovedFromOtherParentNotification OnAnnotationMovedFromOtherParent(
        AnnotationMovedFromOtherParentNotification notification)
    {
        //TODO: check similar cases: moved node is not a child of partition
        var oldParent = notification.OldParent;

        var newParent = NodeCloner(notification.NewParent);
        var movedAnnotation = NodeCloner(notification.MovedAnnotation);
        return new AnnotationMovedFromOtherParentNotification(
            newParent,
            notification.NewIndex,
            movedAnnotation,
            oldParent,
            notification.OldIndex,
            notification.NotificationId
        );
    }

    private AnnotationMovedInSameParentNotification OnAnnotationMovedInSameParent(
        AnnotationMovedInSameParentNotification notification)
    {
        var parent = NodeCloner(notification.Parent);
        var movedAnnotation = NodeCloner(notification.MovedAnnotation);
        return new AnnotationMovedInSameParentNotification(
            notification.NewIndex,
            movedAnnotation,
            parent,
            notification.OldIndex,
            notification.NotificationId
        );
    }

    #endregion

    #region References

    private ReferenceAddedNotification OnReferenceAdded(ReferenceAddedNotification notification) =>
        new(
            NodeCloner(notification.Parent),
            notification.Reference,
            notification.Index,
            notification.NewTarget,
            notification.NotificationId
        );

    private ReferenceDeletedNotification OnReferenceDeleted(ReferenceDeletedNotification notification) =>
        new(
            NodeCloner(notification.Parent),
            notification.Reference,
            notification.Index,
            notification.DeletedTarget,
            notification.NotificationId
        );

    private ReferenceChangedNotification OnReferenceChanged(ReferenceChangedNotification notification) =>
        new(
            NodeCloner(notification.Parent),
            notification.Reference,
            notification.Index,
            notification.NewTarget,
            notification.OldTarget,
            notification.NotificationId
        );

    #endregion

    private T NodeCloner<T>(T node) where T : IWritableNode
    {
        var nodeId = node.GetId();

        if (sharedNodeMap.TryGetValue(nodeId, out var result) && result is T w)
            return w;

        var clone = (T)SameIdCloner.Clone((INode)node);
        sharedNodeMap.RegisterNode(clone);
        return clone;
    }
}