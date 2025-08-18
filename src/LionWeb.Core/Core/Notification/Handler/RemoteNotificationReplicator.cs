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

namespace LionWeb.Core.Notification.Handler;

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
public class RemoteNotificationReplicator : NotificationHandlerBase, IConnectingNotificationHandler
{
    private readonly IForest? _localForest;

    private readonly SharedNodeMap _sharedNodeMap;
    protected readonly IdFilteringNotificationHandler Filter;

    public RemoteNotificationReplicator(
        IForest? localForest,
        SharedNodeMap sharedNodeMap,
        IdFilteringNotificationHandler filter,
        object? sender) : base(sender)
    {
        _localForest = localForest;
        _sharedNodeMap = sharedNodeMap;
        Filter = filter;
    }

    /// <inheritdoc />
    public void Receive(ISendingNotificationHandler notificationHandler, INotification notification)
    {
        Debug.WriteLine($"processing notification {notification.NotificationId}");
        switch (notification)
        {
            case PartitionAddedNotification a:
                OnRemoteNewPartition(notificationHandler, a);
                break;
            case PartitionDeletedNotification a:
                OnRemotePartitionDeleted(notificationHandler, a);
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
            case AnnotationAddedNotification e:
                OnRemoteAnnotationAdded(e);
                break;
            case AnnotationDeletedNotification e:
                OnRemoteAnnotationDeleted(e);
                break;
            case AnnotationMovedFromOtherParentNotification e:
                OnRemoteAnnotationMovedFromOtherParent(e);
                break;
            case AnnotationMovedInSameParentNotification e:
                OnRemoteAnnotationMovedInSameParent(e);
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
            default:
                throw new ArgumentException($"Can not process notification due to unknown {notification}!");
        }
    }

    #region Partitions

    private void OnRemoteNewPartition(ISendingNotificationHandler notificationHandler,
        PartitionAddedNotification partitionAdded) =>
        SuppressNotificationForwarding(partitionAdded, () =>
        {
            var newPartition = partitionAdded.NewPartition;
            _localForest?.AddPartitions([newPartition], partitionAdded.NotificationId);
            INotificationHandler.Connect(notificationHandler, this);
        });

    private void OnRemotePartitionDeleted(ISendingNotificationHandler notificationHandler,
        PartitionDeletedNotification partitionDeleted) =>
        SuppressNotificationForwarding(partitionDeleted, () =>
        {
            notificationHandler.Unsubscribe(this);
            var localPartition = (IPartitionInstance?)LookupOpt(partitionDeleted.DeletedPartition.GetId());
            if (localPartition != null)
                _localForest?.RemovePartitions([localPartition], partitionDeleted.NotificationId);
        });

    #endregion

    #region Properties

    private void OnRemotePropertyAdded(PropertyAddedNotification propertyAdded) =>
        SuppressNotificationForwarding(propertyAdded, () =>
        {
            Debug.WriteLine(
                $"Node {propertyAdded.Node.PrintIdentity()}: Setting {propertyAdded.Property} to {propertyAdded.NewValue}");
            Lookup(propertyAdded.Node.GetId()).Set(propertyAdded.Property, propertyAdded.NewValue,
                propertyAdded.NotificationId);
        });

    private void OnRemotePropertyDeleted(PropertyDeletedNotification propertyDeleted) =>
        SuppressNotificationForwarding(propertyDeleted, () =>
        {
            Lookup(propertyDeleted.Node.GetId())
                .Set(propertyDeleted.Property, null, propertyDeleted.NotificationId);
        });

    private void OnRemotePropertyChanged(PropertyChangedNotification propertyChanged) =>
        SuppressNotificationForwarding(propertyChanged, () =>
        {
            Lookup(propertyChanged.Node.GetId()).Set(propertyChanged.Property, propertyChanged.NewValue,
                propertyChanged.NotificationId);
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedNotification childAdded) =>
        SuppressNotificationForwarding(childAdded, () =>
        {
            var localParent = Lookup(childAdded.Parent.GetId());
            var newChildNode = (INode)childAdded.NewChild;

            Debug.WriteLine(
                $"Parent {localParent.PrintIdentity()}: Adding {newChildNode.PrintIdentity()} to {childAdded.Containment} at {childAdded.Index}");
            var newValue = InsertContainment(localParent, childAdded.Containment, childAdded.Index,
                newChildNode);

            localParent.Set(childAdded.Containment, newValue, childAdded.NotificationId);
        });

    private void OnRemoteChildDeleted(ChildDeletedNotification childDeleted) =>
        SuppressNotificationForwarding(childDeleted, () =>
        {
            var localParent = Lookup(childDeleted.Parent.GetId());

            object? newValue = null;
            if (childDeleted.Containment.Multiple)
            {
                var existingChildren = localParent.Get(childDeleted.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.RemoveAt(childDeleted.Index);
                    newValue = children;
                }
            }

            localParent.Set(childDeleted.Containment, newValue, childDeleted.NotificationId);
        });

    private void OnRemoteChildReplaced(ChildReplacedNotification childReplaced) =>
        SuppressNotificationForwarding(childReplaced, () =>
        {
            var localParent = Lookup(childReplaced.Parent.GetId());
            var substituteNode = (INode)childReplaced.NewChild;
            var newValue = ReplaceContainment(localParent, childReplaced.Containment, childReplaced.Index,
                substituteNode);

            localParent.Set(childReplaced.Containment, newValue, childReplaced.NotificationId);
        });

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification childMoved) =>
        SuppressNotificationForwarding(childMoved, () =>
        {
            var localNewParent = Lookup(childMoved.NewParent.GetId());
            var nodeToInsert = LookupOpt(childMoved.MovedChild.GetId()) ?? (INode)childMoved.MovedChild;
            var newValue = InsertContainment(localNewParent, childMoved.NewContainment, childMoved.NewIndex,
                nodeToInsert);

            localNewParent.Set(childMoved.NewContainment, newValue, childMoved.NotificationId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentNotification childMovedAndReplaced) => SuppressNotificationForwarding(
        childMovedAndReplaced, () =>
        {
            var localNewParent = Lookup(childMovedAndReplaced.NewParent.GetId());
            var substituteNode = Lookup(childMovedAndReplaced.MovedChild.GetId());
            var newValue = ReplaceContainment(localNewParent, childMovedAndReplaced.NewContainment,
                childMovedAndReplaced.NewIndex,
                substituteNode);

            localNewParent.Set(childMovedAndReplaced.NewContainment, newValue, childMovedAndReplaced.NotificationId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(
        ChildMovedAndReplacedFromOtherContainmentInSameParentNotification childMovedAndReplaced)
    {
        var parent = Lookup(childMovedAndReplaced.Parent.GetId());
        var substituteNode = Lookup(childMovedAndReplaced.MovedChild.GetId());
        var newValue = ReplaceContainment(parent, childMovedAndReplaced.NewContainment,
            childMovedAndReplaced.NewIndex, substituteNode);

        parent.Set(childMovedAndReplaced.NewContainment, newValue);
    }

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification childMoved) =>
        SuppressNotificationForwarding(childMoved, () =>
        {
            var localParent = Lookup(childMoved.Parent.GetId());
            var newValue = InsertContainment(localParent, childMoved.NewContainment, childMoved.NewIndex,
                Lookup(childMoved.MovedChild.GetId()));

            localParent.Set(childMoved.NewContainment, newValue, childMoved.NotificationId);
        });

    private void OnRemoteChildMovedInSameContainment(ChildMovedInSameContainmentNotification childMoved) =>
        SuppressNotificationForwarding(childMoved, () =>
        {
            var localParent = Lookup(childMoved.Parent.GetId());
            INode nodeToInsert = Lookup(childMoved.MovedChild.GetId());
            object newValue = nodeToInsert;
            var existingChildren = localParent.Get(childMoved.Containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.RemoveAt(childMoved.OldIndex);
                children.Insert(childMoved.NewIndex, nodeToInsert);
                newValue = children;
            }

            localParent.Set(childMoved.Containment, newValue, childMoved.NotificationId);
        });

    private static object ReplaceContainment(INode localParent, Containment containment, Index index,
        INode substituteNode)
    {
        if (localParent.TryGet(containment, out var existingChildren))
        {
            switch (existingChildren)
            {
                case IList l:
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.Insert(index, substituteNode);
                    children.RemoveAt(index + 1);
                    return children;
                case IWritableNode _ when index == 0:
                    return substituteNode;
                default:
                    // when containment data is corrupted or assigned to an invalid value after its creation
                    throw new InvalidValueException(containment, existingChildren);
            }
        }

        if (containment.Multiple)
        {
            return new List<IWritableNode> { substituteNode };
        }

        return substituteNode;
    }

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

    private void OnRemoteAnnotationAdded(AnnotationAddedNotification annotationAdded) =>
        SuppressNotificationForwarding(annotationAdded, () =>
        {
            var localParent = Lookup(annotationAdded.Parent.GetId());
            var annotation = (INode)annotationAdded.NewAnnotation;
            localParent.InsertAnnotations(annotationAdded.Index, [annotation], annotationAdded.NotificationId);
        });

    private void OnRemoteAnnotationDeleted(AnnotationDeletedNotification annotationDeleted) =>
        SuppressNotificationForwarding(annotationDeleted, () =>
        {
            var localParent = Lookup(annotationDeleted.Parent.GetId());
            var localDeleted = Lookup(annotationDeleted.DeletedAnnotation.GetId());
            localParent.RemoveAnnotations([localDeleted], annotationDeleted.NotificationId);
        });

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification annotationMoved) =>
        SuppressNotificationForwarding(annotationMoved, () =>
        {
            var localNewParent = Lookup(annotationMoved.NewParent.GetId());
            var moved = LookupOpt(annotationMoved.MovedAnnotation.GetId()) ??
                        (INode)annotationMoved.MovedAnnotation;
            localNewParent.InsertAnnotations(annotationMoved.NewIndex, [moved], annotationMoved.NotificationId);
        });

    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentNotification annotationMoved) =>
        SuppressNotificationForwarding(annotationMoved, () =>
        {
            var localParent = Lookup(annotationMoved.Parent.GetId());
            INode nodeToInsert = Lookup(annotationMoved.MovedAnnotation.GetId());
            localParent.InsertAnnotations(annotationMoved.NewIndex, [nodeToInsert], annotationMoved.NotificationId);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedNotification referenceAdded) =>
        SuppressNotificationForwarding(referenceAdded, () =>
        {
            var localParent = Lookup(referenceAdded.Parent.GetId());
            INode target = Lookup(referenceAdded.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, referenceAdded.Reference, referenceAdded.Index,
                target);

            localParent.Set(referenceAdded.Reference, newValue, referenceAdded.NotificationId);
        });

    private void OnRemoteReferenceDeleted(ReferenceDeletedNotification referenceDeleted) =>
        SuppressNotificationForwarding(referenceDeleted, () =>
        {
            var localParent = Lookup(referenceDeleted.Parent.GetId());

            object? newValue = null;
            if (referenceDeleted.Reference.Multiple)
            {
                var existingTargets = localParent.Get(referenceDeleted.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.RemoveAt(referenceDeleted.Index);
                    newValue = targets;
                }
            }

            localParent.Set(referenceDeleted.Reference, newValue, referenceDeleted.NotificationId);
        });

    private void OnRemoteReferenceChanged(ReferenceChangedNotification referenceChanged) =>
        SuppressNotificationForwarding(referenceChanged, () =>
        {
            var localParent = Lookup(referenceChanged.Parent.GetId());

            object newValue = Lookup(referenceChanged.NewTarget.Reference.GetId());
            if (referenceChanged.Reference.Multiple)
            {
                var existingTargets = localParent.Get(referenceChanged.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(referenceChanged.Index, (IReadableNode)newValue);
                    targets.RemoveAt(referenceChanged.Index + 1);
                    newValue = targets;
                }
            }

            localParent.Set(referenceChanged.Reference, newValue, referenceChanged.NotificationId);
        });

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

    /// Uses <see cref="IdFilteringNotificationHandler"/> to suppress forwarding notifications raised during executing <paramref name="action"/>. 
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