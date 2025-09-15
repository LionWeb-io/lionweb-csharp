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

    #endregion

    #region References

    private ReferenceAddedNotification OnReferenceAdded(ReferenceAddedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);

        return new(
            parent,
            notification.Reference,
            notification.Index,
            notification.NewTarget,
            notification.NotificationId
        );
    }

    private ReferenceDeletedNotification OnReferenceDeleted(ReferenceDeletedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);

        return new(
            parent,
            notification.Reference,
            notification.Index,
            notification.DeletedTarget,
            notification.NotificationId
        );
    }

    private ReferenceChangedNotification OnReferenceChanged(ReferenceChangedNotification notification)
    {
        var parent = LookUpNode(notification.Parent);

        return new(
            parent,
            notification.Reference,
            notification.Index,
            notification.NewTarget,
            notification.OldTarget,
            notification.NotificationId
        );
    }

    #endregion

    private T LookUpNode<T>(T node) where T : IReadableNode
    {
        var nodeId = node.GetId();
        if (sharedNodeMap.TryGetValue(nodeId, out var result) && result is T w)
            return w;

        // TODO change to correct exception 
        throw new NotImplementedException(nodeId);
    }

    private T CloneNode<T>(T node) where T : IReadableNode => (T)SameIdCloner.Clone((INode)node);
}