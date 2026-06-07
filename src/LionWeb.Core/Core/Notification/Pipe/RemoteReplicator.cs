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

    protected readonly IdFilteringNotificationFilter Filter;

    private readonly SharedNodeMap _sharedNodeMap;

    public RemoteReplicator(
        IForest? localForest,
        IdFilteringNotificationFilter filter,
        SharedNodeMap sharedNodeMap)
    {
        _localForest = localForest;
        Filter = filter;
        _sharedNodeMap = sharedNodeMap;
    }

    [Obsolete("Use RemoteReplicator(IForest?, IdFilteringNotificationFilter, SharedNodeMap) instead.")]
    public RemoteReplicator(
        IForest? localForest,
        IdFilteringNotificationFilter filter,
        SharedNodeMap sharedNodeMap,
        object? sender) : this(localForest, filter, sharedNodeMap) { }

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
            case CompositeNotification e:
                OnRemoteComposite(correspondingSender, e);
                break;
            default:
                throw new ArgumentException($"Can not process notification due to unknown {notification}!");
        }
    }
    
    #region Partitions

    private void OnRemoteNewPartition(PartitionAddedNotification n)
    {
        CheckIfNewNodeContainsExistingNodes(n, []);
        
        SuppressNotificationForwarding(n, () =>
        {
            if (_localForest is null)
                return;

            if (_localForest.AddPartitionRaw(n.NewPartition))
                _localForest.GetNotificationProducer()?.ProduceNotification(n);
        });
    }

    private void OnRemotePartitionDeleted(PartitionDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            if (_localForest is null)
                return;

            if (_localForest.RemovePartitionRaw(n.DeletedPartition))
                _localForest.GetNotificationProducer()?.ProduceNotification(n);
        });

    #endregion

    #region Properties

    private void OnRemotePropertyAdded(PropertyAddedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            SetProperty(n, n.NewValue);
        });

    private void OnRemotePropertyDeleted(PropertyDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            SetProperty(n, null);
        });

    private void OnRemotePropertyChanged(PropertyChangedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            SetProperty(n, n.NewValue);
        });

    private void SetProperty(IPropertyNotification n, object? value)
    {
        var success = n.ContextNode.SetPropertyRaw(n.Property, value);
        ProduceNotification(n, success);
    }

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedNotification n)
    {
        CheckIfNewNodeContainsExistingNodes(n, []);
        
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild(n.Parent, n.Containment, n.Index, n.NewChild);
            ProduceNotification(n, success);
        });
    }

    private void OnRemoteChildDeleted(ChildDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Deleted node", n, n.Parent, n.DeletedChild, n.Containment, n.Index);

            var localParent = n.Parent;

            bool success = n.Containment.Multiple switch
            {
                true => localParent.RemoveContainmentsRaw(n.Containment, n.DeletedChild),
                false => localParent.SetContainmentRaw(n.Containment, null)
            };

            ProduceNotification(n, success);
        });

    private void OnRemoteChildReplaced(ChildReplacedNotification n)
    {
        HashSet<NodeId> replacedNodes = CollectNodeIdsOfAllDescendantsOf(node: n.ReplacedChild);
        
        CheckIfNewNodeContainsExistingNodes(n, replacedNodes);
        
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, n.Parent, n.ReplacedChild, n.Containment, n.Index);

            var success = ReplaceChildOrAnnotation(n.ReplacedChild, n.NewChild);

            ProduceNotification(n, success);
        });
    }

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild(n.NewParent, n.NewContainment, n.NewIndex, n.MovedChild);
            ProduceMoveNotification(n, success, n.NewParent, n.OldParent);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentNotification n) => SuppressNotificationForwarding(
        n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, n.NewParent, n.ReplacedChild, n.NewContainment, n.NewIndex);
            var success = ReplaceChildOrAnnotation(n.ReplacedChild, n.MovedChild);
            ProduceMoveNotification(n, success, n.NewParent, n.OldParent);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(
        ChildMovedAndReplacedFromOtherContainmentInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, n.Parent, n.ReplacedChild, n.NewContainment, n.NewIndex);
            var success = ReplaceChildOrAnnotation(n.ReplacedChild, n.MovedChild);
            ProduceNotification(n, success);
        });

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild(n.Parent, n.NewContainment, n.NewIndex, n.MovedChild);
            ProduceNotification(n, success);
        });

    private void OnRemoteChildMovedInSameContainment(ChildMovedInSameContainmentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild(n.Parent, n.Containment, n.NewIndex, n.MovedChild);
            ProduceNotification(n, success);
        });

    private void OnRemoteChildMovedAndReplacedInSameContainment(
        ChildMovedAndReplacedInSameContainmentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, n.Parent, n.ReplacedChild, n.Containment, n.NewIndex);
            var success = ReplaceChildOrAnnotation(n.ReplacedChild, n.MovedChild);
            ProduceNotification(n, success);
        });

    private static bool MoveChild(IWritableNode localNewParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild)
    {
        bool success = newContainment.Multiple switch
        {
            true => localNewParent.InsertContainmentsRaw(newContainment, newIndex, movedChild),
            false => localNewParent.SetContainmentRaw(newContainment, movedChild)
        };
        return success;
    }

    #endregion

    private static bool ReplaceChildOrAnnotation(IWritableNode replacedChild, IWritableNode newChild)
    {
        var nodeReplacer = new NodeReplacer<INode>((INode)replacedChild, (INode)newChild);
        nodeReplacer.Replace();
        return nodeReplacer.Success;
    }

    #region Annotations

    private void OnRemoteAnnotationAdded(AnnotationAddedNotification n)
    {
        CheckIfNewNodeContainsExistingNodes(n, []);
        
        SuppressNotificationForwarding(n, () =>
        {
            var success = n.Parent.InsertAnnotationsRaw(n.Index, n.NewAnnotation);
            ProduceNotification(n, success);
        });
    }

    private void OnRemoteAnnotationDeleted(AnnotationDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Deleted annotation", n, n.DeletedAnnotation, n.Parent.GetAnnotations(), n.Index);
            
            var success = n.Parent.RemoveAnnotationsRaw(n.DeletedAnnotation);
            ProduceNotification(n, success);
        });

    private void OnRemoteAnnotationReplaced(AnnotationReplacedNotification n)
    {
        HashSet<NodeId> replacedNodes = CollectNodeIdsOfAllDescendantsOf(node: n.ReplacedAnnotation);
        
        CheckIfNewNodeContainsExistingNodes(n, replacedNodes);
        
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced annotation", n, n.ReplacedAnnotation,
                n.Parent.GetAnnotations(), n.Index);
            var success = ReplaceChildOrAnnotation(n.ReplacedAnnotation, n.NewAnnotation);
            ProduceNotification(n, success);
        });
    }

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = n.NewParent.InsertAnnotationsRaw(n.NewIndex, n.MovedAnnotation);
            ProduceMoveNotification(n, success, n.NewParent, n.OldParent);
        });

    private void OnRemoteAnnotationMovedAndReplacedFromOtherParent(
        AnnotationMovedAndReplacedFromOtherParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced annotation", n, n.ReplacedAnnotation, n.NewParent.GetAnnotations(), n.NewIndex);
            var success = ReplaceChildOrAnnotation(n.ReplacedAnnotation, n.MovedAnnotation);
            ProduceNotification(n, success);
        });

    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = n.Parent.InsertAnnotationsRaw(n.NewIndex, n.MovedAnnotation);
            ProduceNotification(n, success);
        });

    private void OnRemoteAnnotationMovedAndReplacedInSameParent(
        AnnotationMovedAndReplacedInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced annotation", n, n.ReplacedAnnotation, n.Parent.GetAnnotations(), n.NewIndex);
            var success = ReplaceChildOrAnnotation(n.ReplacedAnnotation, n.MovedAnnotation);
            ProduceNotification(n, success);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            IWritableNode localNewParent = n.Parent;
            bool success;
            const string messagePrefix = "Added reference";
            if (n.Reference.Multiple)
            {
                IReadOnlyList<IReferenceTarget> references = CheckMultipleReferencesExist(n, messagePrefix);
                if (n.Index != 0)
                    CheckMultipleReferenceBounds(n, references, messagePrefix);
                success = localNewParent.InsertReferencesRaw(n.Reference, n.Index, (ReferenceTarget)n.NewTarget);
            } else
            {
                CheckSingleReferenceBounds(n, messagePrefix);
                success = localNewParent.SetReferenceRaw(n.Reference, (ReferenceTarget?)n.NewTarget);
            }

            ProduceNotification(n, success);
        });


    private void OnRemoteReferenceDeleted(ReferenceDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var localParent = n.Parent;

            bool success;
            const string messagePrefix = "Deleted reference";
            if (n.Reference.Multiple)
            {
                CheckMultipleReference(n, messagePrefix);
                success = localParent.RemoveReferencesRaw(n.Reference, (ReferenceTarget)n.DeletedTarget);
            } else
            {
                CheckSingleReference(n, messagePrefix);
                success = localParent.SetReferenceRaw(n.Reference, null);
            }

            ProduceNotification(n, success);
        });

    private void OnRemoteReferenceChanged(ReferenceChangedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            bool success;
            const string messagePrefix = "Changed reference";
            if (n.Reference.Multiple)
            {
                CheckMultipleReference(n, messagePrefix);
                success = n.Parent.RemoveReferencesRaw(n.Reference, (ReferenceTarget)n.OldTarget) && n.Parent.InsertReferencesRaw(n.Reference, n.Index, (ReferenceTarget)n.NewTarget);
            } else
            {
                CheckSingleReference(n, messagePrefix);
                success = n.Parent.SetReferenceRaw(n.Reference, (ReferenceTarget?)n.NewTarget);
            }

            ProduceNotification(n, success);
        });

    private static readonly ReferenceTargetIdComparer _targetComparer = new ReferenceTargetIdComparer();

    private static void CheckMultipleReference(IReferenceNotification notification, string messagePrefix)
    {
        IReadOnlyList<IReferenceTarget> references = CheckMultipleReferencesExist(notification, messagePrefix);
        var index = CheckMultipleReferenceBounds(notification, references, messagePrefix);
        if (!_targetComparer.Equals(notification.Target, references[index]))
            throw new InvalidNotificationException(notification,
                $"{messagePrefix} target {notification.Target} does not match with actual target {references[index]} at index {index} in reference {notification.Reference}");
    }

    private static Index CheckMultipleReferenceBounds(IReferenceNotification notification, IReadOnlyList<IReferenceTarget> references, string messagePrefix)
    {
        var index = notification.Index;
        if (index != 0 && index == references.Count)
            index--;
        if (index < 0 || index >= references.Count)
            throw new InvalidNotificationException(notification, $"{messagePrefix} index {index} out of range (size: {references.Count}) on reference {notification.Reference}");
        return index;
    }

    private static IReadOnlyList<IReferenceTarget> CheckMultipleReferencesExist(IReferenceNotification notification, string messagePrefix)
    {
        if (!notification.Parent.TryGetReferencesRaw(notification.Reference, out var references))
            throw new InvalidNotificationException(notification, $"{messagePrefix} target {notification.Target} does not exist within reference {notification.Reference}");
        return references;
    }

    private static void CheckSingleReference(IReferenceNotification notification, string messagePrefix)
    {
        CheckSingleReferenceBounds(notification, messagePrefix);
        if (!notification.Parent.TryGetReferenceRaw(notification.Reference, out var actualTarget) || !_targetComparer.Equals(notification.Target, actualTarget))
            throw new InvalidNotificationException(notification,
                $"{messagePrefix} target {notification.Target} does not match with actual target {actualTarget} at index {notification.Index} in reference {notification.Reference}");
    }

    private static void CheckSingleReferenceBounds(IReferenceNotification notification, string messagePrefix)
    {
        if (notification.Index != 0)
            throw new InvalidNotificationException(notification, $"{messagePrefix} uses non-zero index {notification.Index} on reference {notification.Reference}");
    }

    #endregion

    private void OnRemoteComposite(INotificationSender correspondingSender, CompositeNotification e)
    {
        foreach (var item in e.Parts)
        {
            Receive(correspondingSender, item);
        }
    }

    /// Uses <see cref="IdFilteringNotificationFilter"/> to suppress forwarding notifications raised during executing <paramref name="action"/>.
    protected virtual void SuppressNotificationForwarding(INotification notification, Action action)
    {
        var notificationId = notification.NotificationId;
        Filter.RegisterNotificationId(notificationId);

        try
        {
            action();
        }
        finally
        {
            Filter.UnregisterNotificationId(notificationId);
        }
    }

    private static void ProduceNotification(IPartitionNotification notification, bool success)
    {
        if (success)
            notification.ContextNode.GetPartition()?.GetNotificationProducer()?.ProduceNotification(notification);
    }

    private static void ProduceMoveNotification(INotification notification, bool success, IWritableNode localNewParent,
        IWritableNode oldParent)
    {
        if (!success)
            return;

        var newPartition = localNewParent.GetPartition();
        if (newPartition is null)
            return;

        newPartition.GetNotificationProducer()?.ProduceNotification(notification);

        var oldPartition = oldParent.GetPartition();
        if (oldPartition is null || ReferenceEquals(newPartition, oldPartition))
            return;

        oldPartition.GetNotificationProducer()?.ProduceNotification(notification);
    }

    private static void CheckMatchingNodeId(string messagePrefix, INotification notification, IReadableNode candidate,
        IReadableNode existingChild, Index index, string? messageSuffix = null)
    {
        var candidateNodeId = candidate.GetId();

        if (index != 0)
            throw new InvalidNotificationException(notification,
                $"{messagePrefix} with id {candidateNodeId} uses non-zero index {index}{messageSuffix}");

        var actualNodeId = existingChild.GetId();

        if (actualNodeId == candidateNodeId)
            return;

        throw new InvalidNotificationException(notification,
            $"{messagePrefix} with id {candidateNodeId} does not match with actual node with id {actualNodeId}{messageSuffix}");
    }

    private static void CheckMatchingNodeId(string messagePrefix, INotification notification, IWritableNode candidate,
        IReadOnlyList<IReadableNode> list, Index index, string? messageSuffix = null)
    {
        if (index < 0 || index >= list.Count)
            throw new InvalidNotificationException(notification, $"{messagePrefix} index {index} out of range (size: {list.Count}){messageSuffix}");
        
        var candidateNodeId = candidate.GetId();
        var actualNodeId = list[index].GetId();

        if (candidateNodeId == actualNodeId)
            return;

        throw new InvalidNotificationException(notification,
            $"{messagePrefix} node with id {candidateNodeId} does not match with actual node with id {actualNodeId} at index {index}{messageSuffix}");
    }

    private static void CheckMatchingNodeId(string messagePrefix, INotification notification, IReadableNode localParent,
        IWritableNode candidate, Containment containment, Index index)
    {
        var messageSuffix = $" in containment {containment}";
        switch (containment.Multiple)
        {
            case true when localParent.TryGetContainmentsRaw(containment, out var nodes):
                CheckMatchingNodeId(messagePrefix, notification, candidate, nodes, index, messageSuffix);
                break;

            case false
                when localParent.TryGetContainmentRaw(containment, out var node) && node is not null:
                CheckMatchingNodeId(messagePrefix, notification, candidate, node, index, messageSuffix);
                break;
        }
    }
    private static HashSet<NodeId> CollectNodeIdsOfAllDescendantsOf(IReadableNode node) =>
        M1Extensions
            .Descendants(node, true, true)
            .Select(n => n.GetId())
            .ToHashSet();

    private void CheckIfNewNodeContainsExistingNodes(INewNodeNotification newNodeNotification, HashSet<NodeId> replacedNodes)
    {
        HashSet<NodeId> newNodes = CollectNodeIdsOfAllDescendantsOf(node: newNodeNotification.NewNode);

        var hasIntersection = _sharedNodeMap.NodeIds.ToHashSet().Overlaps(newNodes);

        if (!hasIntersection)
        {
            return;
        }

        var remainingNodes = _sharedNodeMap.NodeIds
            .Intersect(newNodes)
            .ExceptBy(replacedNodes, s => s)
            .ToList();

        if (remainingNodes.Count != 0)
        {
            throw new InvalidNotificationException(newNodeNotification,
                $"Trying to add existing node(s) with ids: {string.Join(",", remainingNodes)}");
        }
    }
}