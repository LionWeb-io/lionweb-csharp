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

namespace LionWeb.Core.Notification.Pipe;

using Forest;
using M1;
using M3;
using Partition;
using System.Collections;
using System.Diagnostics;

/// Replicates <see cref="Receive">received</see> notifications on a <i>local</i> equivalent.
/// 
/// <para>
/// Example: We receive a <see cref="PropertyAddedNotification" /> for a node that we know <i>locally</i>.
/// This class adds the same property value to the <i>locally</i> known node.
/// </para>
public class RemoteReplicator : NotificationPipeBase, INotificationHandler
{
    private readonly IForest? _localForest;

    private readonly SharedNodeMap _sharedNodeMap;
    protected readonly IdFilteringNotificationFilter Filter;

    public RemoteReplicator(
        IForest? localForest,
        SharedNodeMap sharedNodeMap,
        IdFilteringNotificationFilter filter,
        object? sender) : base(sender)
    {
        _localForest = localForest;
        _sharedNodeMap = sharedNodeMap;
        Filter = filter;
    }

    /// <inheritdoc />
    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        Debug.WriteLine($"processing notification {notification.NotificationId}");
        switch (notification)
        {
            case PartitionAddedNotification a:
                OnRemoteNewPartition(a);
                break;
            case PartitionDeletedNotification a:
                OnRemotePartitionDeleted(a);
                break;
            case PropertyAddedNotification e:
                OnRemotePropertyAdded(e);
                break;
            case PropertyDeletedNotification e:
                OnRemotePropertyDeleted(e);
                break;
            case PropertyChangedNotification e:
                OnRemotePropertyChanged(e);
                break;
            case ChildAddedNotification e:
                OnRemoteChildAdded(e);
                break;
            case ChildDeletedNotification e:
                OnRemoteChildDeleted(e);
                break;
            case ChildReplacedNotification e:
                OnRemoteChildReplaced(e);
                break;
            case ChildMovedFromOtherContainmentNotification e:
                OnRemoteChildMovedFromOtherContainment(e);
                break;
            case ChildMovedFromOtherContainmentInSameParentNotification e:
                OnRemoteChildMovedFromOtherContainmentInSameParent(e);
                break;
            case ChildMovedInSameContainmentNotification e:
                OnRemoteChildMovedInSameContainment(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentNotification e:
                OnRemoteChildMovedAndReplacedFromOtherContainment(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentInSameParentNotification e:
                OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(e);
                break;
            case ChildMovedAndReplacedInSameContainmentNotification e:
                OnRemoteChildMovedAndReplacedInSameContainment(e);
                break;
            case AnnotationAddedNotification e:
                OnRemoteAnnotationAdded(e);
                break;
            case AnnotationDeletedNotification e:
                OnRemoteAnnotationDeleted(e);
                break;
            case AnnotationReplacedNotification e:
                OnRemoteAnnotationReplaced(e);
                break;
            case AnnotationMovedFromOtherParentNotification e:
                OnRemoteAnnotationMovedFromOtherParent(e);
                break;
            case AnnotationMovedInSameParentNotification e:
                OnRemoteAnnotationMovedInSameParent(e);
                break;
            case AnnotationMovedAndReplacedInSameParentNotification e:
                OnRemoteAnnotationMovedAndReplacedInSameParent(e);
                break;
            case AnnotationMovedAndReplacedFromOtherParentNotification e:
                OnRemoteAnnotationMovedAndReplacedFromOtherParent(e);
                break;
            case ReferenceAddedNotification e:
                OnRemoteReferenceAdded(e);
                break;
            case ReferenceDeletedNotification e:
                OnRemoteReferenceDeleted(e);
                break;
            case ReferenceChangedNotification e:
                OnRemoteReferenceChanged(e);
                break;
            case EntryMovedInSameReferenceNotification e:
                OnRemoteEntryMovedInSameReference(e);
                break;
            default:
                throw new ArgumentException($"Can not process notification due to unknown {notification}!");
        }
    }

    #region Partitions

    private void OnRemoteNewPartition(PartitionAddedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var newPartition = notification.NewPartition;
            _localForest?.AddPartitions([newPartition], notification.NotificationId);
        });

    private void OnRemotePartitionDeleted(PartitionDeletedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localPartition = (IPartitionInstance?)LookupOpt(notification.DeletedPartition.GetId());
            if (localPartition != null)
                _localForest?.RemovePartitions([localPartition], notification.NotificationId);
        });

    #endregion

    #region Properties

    private void OnRemotePropertyAdded(PropertyAddedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            Debug.WriteLine(
                $"Node {notification.Node.PrintIdentity()}: Setting {notification.Property} to {notification.NewValue}");
            var node = Lookup(notification.Node.GetId());
            node.Set(notification.Property, notification.NewValue, notification.NotificationId);
        });

    private void OnRemotePropertyDeleted(PropertyDeletedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var node = Lookup(notification.Node.GetId());
            node.Set(notification.Property, null, notification.NotificationId);
        });

    private void OnRemotePropertyChanged(PropertyChangedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var node = Lookup(notification.Node.GetId());
            node.Set(notification.Property, notification.NewValue, notification.NotificationId);
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            var newChildNode = (INode)notification.NewChild;

            Debug.WriteLine(
                $"Parent {localParent.PrintIdentity()}: Adding {newChildNode.PrintIdentity()} to {notification.Containment} at {notification.Index}");
            var newValue = InsertContainment(localParent, notification.Containment, notification.Index, newChildNode);

            localParent.Set(notification.Containment, newValue, notification.NotificationId);
        });

    private void OnRemoteChildDeleted(ChildDeletedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            
            object? newValue = null;
            if (notification.Containment.Multiple)
            {
                var existingChildren = localParent.Get(notification.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.RemoveAt(notification.Index);
                    newValue = children;
                }
            }

            localParent.Set(notification.Containment, newValue, notification.NotificationId);
        });

    private void OnRemoteChildReplaced(ChildReplacedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var newChild = LookupOpt(notification.NewChild.GetId()) ?? (INode)notification.NewChild;
            var replacedChild = Lookup(notification.ReplacedChild.GetId());
            replacedChild.ReplaceWith(newChild);
        });

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localNewParent = Lookup(notification.NewParent.GetId());
            var movedChild = LookupOpt(notification.MovedChild.GetId()) ?? (INode)notification.MovedChild;
            var newValue = InsertContainment(localNewParent, notification.NewContainment, notification.NewIndex,
                movedChild);

            localNewParent.Set(notification.NewContainment, newValue, notification.NotificationId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentNotification notification) => SuppressNotificationForwarding(
        notification, () =>
        {
            var movedChild = Lookup(notification.MovedChild.GetId());
            var replacedChild = Lookup(notification.ReplacedChild.GetId());
            replacedChild.ReplaceWith(movedChild);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(
        ChildMovedAndReplacedFromOtherContainmentInSameParentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var movedChild = Lookup(notification.MovedChild.GetId());
            var replacedChild = Lookup(notification.ReplacedChild.GetId());
            replacedChild.ReplaceWith(movedChild);
        });

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            var newValue = InsertContainment(localParent, notification.NewContainment, notification.NewIndex,
                Lookup(notification.MovedChild.GetId()));

            localParent.Set(notification.NewContainment, newValue, notification.NotificationId);
        });

    private void OnRemoteChildMovedInSameContainment(ChildMovedInSameContainmentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            INode nodeToInsert = Lookup(notification.MovedChild.GetId());
            object newValue = nodeToInsert;
            var existingChildren = localParent.Get(notification.Containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.RemoveAt(notification.OldIndex);
                children.Insert(notification.NewIndex, nodeToInsert);
                newValue = children;
            }

            localParent.Set(notification.Containment, newValue, notification.NotificationId);
        });

    private void OnRemoteChildMovedAndReplacedInSameContainment(ChildMovedAndReplacedInSameContainmentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var movedChild = Lookup(notification.MovedChild.GetId());
            var replacedChild = Lookup(notification.ReplacedChild.GetId());
            replacedChild.ReplaceWith(movedChild);
        });

    private static object InsertContainment(INode localParent, Containment containment, Index index, INode nodeToInsert)
    {
        if (localParent.CollectAllSetFeatures().Contains(containment))
        {
            var existingChildren = localParent.Get(containment);
            switch (existingChildren)
            {
                case IList l:
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.Insert(index, nodeToInsert);
                    return children;
                case IWritableNode _:
                    return nodeToInsert;
                default:
                    // when containment data is corrupted or assigned to an invalid value after its creation 
                    throw new InvalidValueException(containment, existingChildren);
            }
        }

        if (containment.Multiple)
        {
            return new List<IWritableNode> { nodeToInsert };
        }

        return nodeToInsert;
    }

    #endregion

    #region Annotations

    private void OnRemoteAnnotationAdded(AnnotationAddedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            var newAnnotation = (INode)notification.NewAnnotation;
            localParent.InsertAnnotations(notification.Index, [newAnnotation], notification.NotificationId);
        });

    private void OnRemoteAnnotationDeleted(AnnotationDeletedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            var localDeleted = Lookup(notification.DeletedAnnotation.GetId());
            localParent.RemoveAnnotations([localDeleted], notification.NotificationId);
        });

    private void OnRemoteAnnotationReplaced(AnnotationReplacedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var newAnnotation = LookupOpt(notification.NewAnnotation.GetId()) ?? (INode)notification.NewAnnotation;
            var replacedAnnotation = Lookup(notification.ReplacedAnnotation.GetId());
            replacedAnnotation.ReplaceWith(newAnnotation);
        });

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localNewParent = Lookup(notification.NewParent.GetId());
            var movedAnnotation = LookupOpt(notification.MovedAnnotation.GetId()) ?? (INode)notification.MovedAnnotation;
            localNewParent.InsertAnnotations(notification.NewIndex, [movedAnnotation], notification.NotificationId);
        });

    private void OnRemoteAnnotationMovedAndReplacedFromOtherParent(AnnotationMovedAndReplacedFromOtherParentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var movedAnnotation = LookupOpt(notification.MovedAnnotation.GetId()) ?? (INode)notification.MovedAnnotation;
            var replacedAnnotation = Lookup(notification.ReplacedAnnotation.GetId());
            replacedAnnotation.ReplaceWith(movedAnnotation);
        });
    
    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            var movedAnnotation =  LookupOpt(notification.MovedAnnotation.GetId()) ?? Lookup(notification.MovedAnnotation.GetId());
            localParent.InsertAnnotations(notification.NewIndex, [movedAnnotation], notification.NotificationId);
        });

    private void OnRemoteAnnotationMovedAndReplacedInSameParent(AnnotationMovedAndReplacedInSameParentNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var movedAnnotation =  LookupOpt(notification.MovedAnnotation.GetId()) ?? Lookup(notification.MovedAnnotation.GetId());
            var replacedAnnotation = Lookup(notification.ReplacedAnnotation.GetId());
            replacedAnnotation.ReplaceWith(movedAnnotation);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());
            INode target = Lookup(notification.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, notification.Reference, notification.Index,
                target);

            localParent.Set(notification.Reference, newValue, notification.NotificationId);
        });

    private void OnRemoteReferenceDeleted(ReferenceDeletedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());

            object? newValue = null;
            if (notification.Reference.Multiple)
            {
                var existingTargets = localParent.Get(notification.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.RemoveAt(notification.Index);
                    newValue = targets;
                }
            }

            localParent.Set(notification.Reference, newValue, notification.NotificationId);
        });

    private void OnRemoteReferenceChanged(ReferenceChangedNotification notification) =>
        SuppressNotificationForwarding(notification, () =>
        {
            var localParent = Lookup(notification.Parent.GetId());

            object newValue = Lookup(notification.NewTarget.Reference.GetId());
            if (notification.Reference.Multiple)
            {
                var existingTargets = localParent.Get(notification.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(notification.Index, (IReadableNode)newValue);
                    targets.RemoveAt(notification.Index + 1);
                    newValue = targets;
                }
            }

            localParent.Set(notification.Reference, newValue, notification.NotificationId);
        });

    private void OnRemoteEntryMovedInSameReference(EntryMovedInSameReferenceNotification notification)
    {
        var localParent = Lookup(notification.Parent.GetId());
        var target = Lookup(notification.Target.Reference.GetId());

        object newValue = target;
        var reference = notification.Reference;
        if (localParent.TryGet(reference, out object? existingTargets))
        {
            if (existingTargets is IList l)
            {
                var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                targets.RemoveAt(notification.OldIndex);
                targets.Insert(notification.NewIndex, target);
                newValue = targets;
            }
        } else
        {
            newValue = new List<IReadableNode>() { target };
        }

        localParent.Set(reference, newValue);
    }

    private static object InsertReference(INode localParent, Reference reference, Index index, IReadableNode target)
    {
        object newValue = target;
        if (reference.Multiple)
        {
            if (localParent.CollectAllSetFeatures().Contains(reference))
            {
                var existingTargets = localParent.Get(reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(index, target);
                    newValue = targets;
                }
            } else
            {
                newValue = new List<IReadableNode>() { target };
            }
        }

        return newValue;
    }

    #endregion

    private INode Lookup(NodeId nodeId) =>
        (INode)_sharedNodeMap[nodeId];

    private INode? LookupOpt(NodeId nodeId) =>
        _sharedNodeMap.TryGetValue(nodeId, out var result) ? (INode?)result : null;

    /// Uses <see cref="IdFilteringNotificationFilter"/> to suppress forwarding notifications raised during executing <paramref name="action"/>. 
    protected virtual void SuppressNotificationForwarding(INotification notification, Action action)
    {
        var notificationId = notification.NotificationId;
        Filter.RegisterNotificationId(notificationId);

        try
        {
            action();
        } finally
        {
            Filter.UnregisterNotificationId(notificationId);
        }
    }
}