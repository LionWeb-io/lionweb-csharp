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
public class RemoteReplicatorRaw : RemoteReplicator
{
    private readonly IForestRaw? _localForest;

    protected readonly IdFilteringNotificationFilter Filter;

    public RemoteReplicatorRaw(
        IForestRaw? localForest,
        IdFilteringNotificationFilter filter,
        object? sender) : base(localForest, filter, sender)
    {
        _localForest = localForest;
        Filter = filter;
    }

    /// <inheritdoc />
    public override void Receive(INotificationSender correspondingSender, INotification notification)
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

    private void OnRemoteNewPartition(PartitionAddedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            if (_localForest is null)
                return;

            if (_localForest.AddPartitionRaw(n.NewPartition))
                _localForest.GetNotificationProducer()?.ProduceNotification(n);
        });

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
        var success = ((IWritableNodeRaw)n.ContextNode).SetPropertyRaw(n.Property, value);
        ProduceNotification(n, success);
    }

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild((IWritableNodeRaw)n.Parent, n.Containment, n.Index, n.NewChild);
            ProduceNotification(n, success);
        });

    private void OnRemoteChildDeleted(ChildDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Deleted node", n, (IReadableNodeRaw)n.Parent, n.DeletedChild, n.Containment, n.Index);

            var localParent = (IWritableNodeRaw)n.Parent;

            bool success = n.Containment.Multiple switch
            {
                true => localParent.RemoveContainmentsRaw(n.Containment, [n.DeletedChild]),
                false => localParent.SetContainmentRaw(n.Containment, null)
            };

            ProduceNotification(n, success);
        });

    private void OnRemoteChildReplaced(ChildReplacedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, (IReadableNodeRaw)n.Parent, n.ReplacedChild, n.Containment, n.Index);

            var success = ReplaceChild((IWritableNodeRaw)n.Parent, n.ReplacedChild, n.Containment, n.Index, n.NewChild);

            ProduceNotification(n, success);
        });

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild((IWritableNodeRaw)n.NewParent, n.NewContainment, n.NewIndex, n.MovedChild);
            ProduceMoveNotification(n, success, n.NewParent, n.OldParent);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentNotification n) => SuppressNotificationForwarding(
        n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, (IReadableNodeRaw)n.NewParent, n.ReplacedChild, n.NewContainment, n.NewIndex);
            var success = ReplaceChild((IWritableNodeRaw)n.NewParent, n.ReplacedChild, n.NewContainment, n.NewIndex, n.MovedChild);
            ProduceMoveNotification(n, success, n.NewParent, n.OldParent);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(
        ChildMovedAndReplacedFromOtherContainmentInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, (IReadableNodeRaw)n.Parent, n.ReplacedChild, n.NewContainment, n.NewIndex);
            var success = ReplaceChild((IWritableNodeRaw)n.Parent, n.ReplacedChild, n.NewContainment, n.NewIndex, n.MovedChild);
            ProduceNotification(n, success);
        });

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild((IWritableNodeRaw)n.Parent, n.NewContainment, n.NewIndex, n.MovedChild);
            ProduceNotification(n, success);
        });

    private void OnRemoteChildMovedInSameContainment(ChildMovedInSameContainmentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = MoveChild((IWritableNodeRaw)n.Parent, n.Containment, n.NewIndex, n.MovedChild);
            ProduceNotification(n, success);
        });

    private void OnRemoteChildMovedAndReplacedInSameContainment(
        ChildMovedAndReplacedInSameContainmentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced node", n, (IReadableNodeRaw)n.Parent, n.ReplacedChild, n.Containment, n.NewIndex);
            var success = ReplaceChild((IWritableNodeRaw)n.Parent, n.ReplacedChild, n.Containment, n.NewIndex, n.MovedChild);
            ProduceNotification(n, success);
        });

    private static bool MoveChild(IWritableNodeRaw localNewParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild)
    {
        bool success = newContainment.Multiple switch
        {
            true => localNewParent.InsertContainmentsRaw(newContainment, newIndex, [movedChild]),
            false => localNewParent.SetContainmentRaw(newContainment, movedChild)
        };
        return success;
    }

    private static bool ReplaceChild(IWritableNodeRaw localParent, IWritableNode replacedChild, Containment containment,
        Index index, IWritableNode newChild)
    {
        bool success = containment.Multiple switch
        {
            true => localParent.RemoveContainmentsRaw(containment, [replacedChild])
                    && localParent.InsertContainmentsRaw(containment, index, [newChild]),

            false => localParent.SetContainmentRaw(containment, newChild)
        };
        return success;
    }

    #endregion

    #region Annotations

    private void OnRemoteAnnotationAdded(AnnotationAddedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = ((IWritableNodeRaw)n.Parent).InsertAnnotationsRaw(n.Index, [(IAnnotationInstance)n.NewAnnotation]);
            ProduceNotification(n, success);
        });

    private void OnRemoteAnnotationDeleted(AnnotationDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Deleted annotation", n, n.DeletedAnnotation, n.Parent.GetAnnotations(), n.Index);
            
            var success = ((IWritableNodeRaw)n.Parent).RemoveAnnotationsRaw([(IAnnotationInstance)n.DeletedAnnotation]);
            ProduceNotification(n, success);
        });

    private void OnRemoteAnnotationReplaced(AnnotationReplacedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced annotation", n, n.ReplacedAnnotation,
                n.Parent.GetAnnotations(), n.Index);

            var localParent = (IWritableNodeRaw)n.Parent;
            var success = localParent.RemoveAnnotationsRaw([(IAnnotationInstance)n.ReplacedAnnotation])
                          && localParent.InsertAnnotationsRaw(n.Index, [(IAnnotationInstance)n.NewAnnotation]);

            ProduceNotification(n, success);
        });

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = ((IWritableNodeRaw)n.NewParent).InsertAnnotationsRaw(n.NewIndex, [(IAnnotationInstance)n.MovedAnnotation]);
            ProduceMoveNotification(n, success, n.NewParent, n.OldParent);
        });

    private void OnRemoteAnnotationMovedAndReplacedFromOtherParent(
        AnnotationMovedAndReplacedFromOtherParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced annotation", n, n.ReplacedAnnotation, n.NewParent.GetAnnotations(), n.NewIndex);

            var localNewParent = (IWritableNodeRaw)n.NewParent;
            var success = localNewParent.RemoveAnnotationsRaw([(IAnnotationInstance)n.ReplacedAnnotation])
                          && localNewParent.InsertAnnotationsRaw(n.NewIndex, [(IAnnotationInstance)n.MovedAnnotation]);

            ProduceMoveNotification(n, success, n.NewParent, n.OldParent);
        });

    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = ((IWritableNodeRaw)n.Parent).InsertAnnotationsRaw(n.NewIndex, [(IAnnotationInstance)n.MovedAnnotation]);
            ProduceNotification(n, success);
        });

    private void OnRemoteAnnotationMovedAndReplacedInSameParent(
        AnnotationMovedAndReplacedInSameParentNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            CheckMatchingNodeId("Replaced annotation", n, n.ReplacedAnnotation, n.Parent.GetAnnotations(), n.NewIndex);

            var localParent = (IWritableNodeRaw)n.Parent;
            var success = localParent.RemoveAnnotationsRaw([(IAnnotationInstance)n.ReplacedAnnotation])
                          && localParent.InsertAnnotationsRaw(n.NewIndex, [(IAnnotationInstance)n.MovedAnnotation]);

            ProduceNotification(n, success);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            IWritableNodeRaw localNewParent = (IWritableNodeRaw)n.Parent;
            var success = n.Reference.Multiple switch
            {
                true => localNewParent.InsertReferencesRaw(n.Reference, n.Index, [(ReferenceTarget)n.NewTarget]),
                false => localNewParent.SetReferenceRaw(n.Reference, (ReferenceTarget?)n.NewTarget)
            };
            ProduceNotification(n, success);
        });


    private void OnRemoteReferenceDeleted(ReferenceDeletedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var localParent = (IWritableNodeRaw)n.Parent;

            bool success = n.Reference.Multiple switch
            {
                true => localParent.RemoveReferencesRaw(n.Reference, [(ReferenceTarget)n.DeletedTarget]),
                false => localParent.SetReferenceRaw(n.Reference, null)
            };

            ProduceNotification(n, success);
        });

    private void OnRemoteReferenceChanged(ReferenceChangedNotification n) =>
        SuppressNotificationForwarding(n, () =>
        {
            var success = ReplaceReference((IWritableNodeRaw)n.Parent, n.OldTarget, n.Reference, n.Index, n.NewTarget);
            ProduceNotification(n, success);
        });

    private void OnRemoteEntryMovedInSameReference(EntryMovedInSameReferenceNotification n)
    {
        IWritableNodeRaw localNewParent = (IWritableNodeRaw)n.Parent;
        var success = n.Reference.Multiple switch
        {
            true => localNewParent.RemoveReferencesRaw(n.Reference, [(ReferenceTarget)n.Target])
                && localNewParent.InsertReferencesRaw(n.Reference, n.NewIndex, [(ReferenceTarget)n.Target]),
            false => localNewParent.SetReferenceRaw(n.Reference, (ReferenceTarget?)n.Target)
        };
        ProduceNotification(n, success);
    }

    private static bool ReplaceReference(IWritableNodeRaw localParent, IReferenceTarget replacedTarget,
        Reference reference,
        Index index, IReferenceTarget newTarget)
    {
        bool success = reference.Multiple switch
        {
            true => localParent.RemoveReferencesRaw(reference, [(ReferenceTarget)replacedTarget])
                    && localParent.InsertReferencesRaw(reference, index, [(ReferenceTarget)newTarget]),

            false => localParent.SetReferenceRaw(reference, (ReferenceTarget?)newTarget)
        };
        return success;
    }

    #endregion

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
                $"{messagePrefix} with id {candidateNodeId} uses non-zero index {index}{messagePrefix}");

        var actualNodeId = existingChild.GetId();

        if (actualNodeId == candidateNodeId)
            return;

        throw new InvalidNotificationException(notification,
            $"{messagePrefix} with id {candidateNodeId} does not match with actual node with id {actualNodeId}{messageSuffix}");
    }

    private static void CheckMatchingNodeId(string messagePrefix, INotification notification, IWritableNode candidate,
        IReadOnlyList<IReadableNode> list, Index index, string? messageSuffix = null)
    {
        var candidateNodeId = candidate.GetId();
        var actualNodeId = list[index].GetId();

        if (candidateNodeId == actualNodeId)
            return;

        throw new InvalidNotificationException(notification,
            $"{messagePrefix} node with id {candidateNodeId} does not match with actual node with id {actualNodeId} at index {index}{messageSuffix}");
    }

    private static void CheckMatchingNodeId(string messagePrefix, INotification notification, IReadableNodeRaw localParent,
        IWritableNode candidate, Containment containment, Index index)
    {
        var messageSuffix = $"in containment {containment}";
        switch (containment.Multiple)
        {
            case true when localParent.TryGetContainmentsRaw(containment, out var nodes):
                CheckMatchingNodeId(messagePrefix, notification, candidate, nodes.ToList(), index, messageSuffix);
                break;

            case false
                when localParent.TryGetContainmentRaw(containment, out var node) && node is not null:
                CheckMatchingNodeId(messagePrefix, notification, candidate, node, index, messageSuffix);
                break;
        }
    }
}