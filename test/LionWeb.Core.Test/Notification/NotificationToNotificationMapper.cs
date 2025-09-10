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
using Core.Serialization;
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
            Clone(notification.Node),
            notification.Property,
            notification.NewValue,
            notification.NotificationId
        );

    private PropertyDeletedNotification OnPropertyDeleted(PropertyDeletedNotification notification) =>
        new(
            Clone(notification.Node),
            notification.Property,
            notification.OldValue,
            notification.NotificationId
        );

    private PropertyChangedNotification OnPropertyChanged(PropertyChangedNotification notification) =>
        new(
            Clone(notification.Node),
            notification.Property,
            notification.NewValue,
            notification.OldValue,
            notification.NotificationId
        );

    #endregion


    #region Children

    private ChildAddedNotification OnChildAdded(ChildAddedNotification notification) =>
        new(
            Clone(notification.Parent),
            Clone(notification.NewChild),
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );

    private ChildDeletedNotification OnChildDeleted(ChildDeletedNotification notification) =>
        new(
            notification.DeletedChild,
            Clone(notification.Parent),
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );

    private ChildReplacedNotification OnChildReplaced(ChildReplacedNotification notification) =>
        new(
            Clone(notification.NewChild),
            Clone(notification.ReplacedChild),
            notification.Parent,
            notification.Containment,
            notification.Index,
            notification.NotificationId
        );

    private ChildMovedFromOtherContainmentNotification OnChildMovedFromOtherContainment(
        ChildMovedFromOtherContainmentNotification notification)
    {
        var movedChild = Clone(notification.MovedChild);
        var oldParent = Clone(notification.OldParent);
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
        var movedChild = Clone(notification.MovedChild);
        var newParent = notification.NewParent; //Clone(notification.NewParent); //TODO: test: moved child is child of partition or subtree
        var replacedChild = Clone(notification.ReplacedChild);

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
        var parent = Clone(notification.Parent);
        var movedChild = Clone(notification.MovedChild);
        var replacedChild = Clone(notification.ReplacedChild);

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
        var parent = Clone(notification.Parent);
        var movedChild = Clone(notification.MovedChild);
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
        var parent = Clone(notification.Parent);
        var movedChild = Clone(notification.MovedChild);
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
            Clone(notification.Parent),
            Clone(notification.NewAnnotation),
            notification.Index,
            notification.NotificationId
        );

    private AnnotationDeletedNotification OnAnnotationDeleted(AnnotationDeletedNotification notification) =>
        new(
            Clone(notification.DeletedAnnotation),
            Clone(notification.Parent),
            notification.Index,
            notification.NotificationId
        );

    private AnnotationMovedFromOtherParentNotification OnAnnotationMovedFromOtherParent(
        AnnotationMovedFromOtherParentNotification notification)
    {
        //TODO: check similar cases: moved node is not a child of partition
        var oldParent = notification.OldParent; 
        
        var newParent = Clone(notification.NewParent);
        var movedAnnotation = Clone(notification.MovedAnnotation);
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
        var parent = Clone(notification.Parent);
        var movedAnnotation = Clone(notification.MovedAnnotation);
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
            Clone(notification.Parent),
            notification.Reference,
            notification.Index,
            notification.NewTarget,
            notification.NotificationId
        );

    private ReferenceDeletedNotification OnReferenceDeleted(ReferenceDeletedNotification notification) =>
        new(
            Clone(notification.Parent),
            notification.Reference,
            notification.Index,
            notification.DeletedTarget,
            notification.NotificationId
        );

    private ReferenceChangedNotification OnReferenceChanged(ReferenceChangedNotification notification) =>
        new(
            Clone(notification.Parent),
            notification.Reference,
            notification.Index,
            notification.NewTarget,
            notification.OldTarget,
            notification.NotificationId
        );
    
    #endregion

    private IWritableNode Clone(IWritableNode node)
    {
        var nodeId = node.GetId();

        if (sharedNodeMap.TryGetValue(nodeId, out var result) && result is IWritableNode w)
            return w;

        var clone = SameIdCloner.Clone((INode)node);
        sharedNodeMap.RegisterNode(clone);
        return clone;
    }

    private T Clone<T>(T node) where T : class?, IReadableNode?
    {
        var nodeId = node.GetId();

        if (sharedNodeMap.TryGetValue(nodeId, out var result) && result is T w)
            return w;
        
        var clone = (T)SameIdCloner.Clone((INode)node);
        sharedNodeMap.RegisterNode(clone);
        return clone;
    }

    private T ToFeature<T>(MetaPointer deltaReference, IReadableNode node) where T : Feature
    {
        if (sharedKeyedMap.TryGetValue(Compress(deltaReference), out var e) && e is T c)
            return c;

        throw new UnknownFeatureException(node.GetClassifier(), deltaReference);
    }

    private CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, true);
}