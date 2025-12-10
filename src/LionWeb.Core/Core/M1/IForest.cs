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

namespace LionWeb.Core.M1;

using Notification;
using Notification.Forest;
using Notification.Pipe;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// A collection of model trees, represented by each trees' <see cref="IPartitionInstance">partition</see> (aka root node).
/// <seealso cref="LionWeb.Core.M1.Raw.IForestRawExtensions"/>
public interface IForest
{
    /// Contains all known partitions.
    /// <seealso cref="AddPartitions"/>
    /// <seealso cref="RemovePartitions"/>
    IReadOnlySet<IPartitionInstance> Partitions { get; }

    /// Tries to find partition with <see cref="IReadableNode.GetId">node id</see> <paramref name="nodeId"/>.
    bool TryGetPartition(NodeId nodeId, [NotNullWhen(true)] out IPartitionInstance? partition);

    /// Adds <paramref name="partitions"/> to <c>this</c> forest.
    void AddPartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null);

    /// Removes <paramref name="partitions"/> from <c>this</c> forest.
    void RemovePartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null);

    /// <c>this</c> forest's notification sender, if any.
    INotificationSender? GetNotificationSender();

    /// <c>this</c> forest's notification producer, if any.
    protected internal IForestNotificationProducer? GetNotificationProducer();

    #region raw api

    /// <summary>
    /// Tries to get the partition with <paramref name="nodeId"/> from <c>this</c> forest.
    /// </summary>
    /// <returns>
    /// <c>true</c> if partition <paramref name="nodeId"/> is known to <c>this</c> forest;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="IForest.TryGetPartition"/>
    protected internal bool TryGetPartitionRaw(NodeId nodeId, [NotNullWhen(true)] out IPartitionInstance? partition);

    /// <summary>
    /// Adds <paramref name="partition"/> to <c>this</c> forest.
    /// </summary>
    /// <param name="partition">Partition to add to <c>this</c> forest.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="partition"/> has been added and that changed the forest
    /// (i.e. <paramref name="partition"/> is a valid partition for <c>this</c> and not yet the last partition in <c>this</c>);
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// Does <i>not</i> trigger a notification, but subscribes <c>this</c> forest to <paramref name="partition"/>
    /// (i.e. future events on <paramref name="partition"/> will be forwarded to <c>this</c>).
    /// </remarks>
    /// <seealso cref="IForest.AddPartitions"/>
    protected internal bool AddPartitionRaw(IPartitionInstance partition);

    /// <summary>
    /// Removes <paramref name="partition"/> from <c>this</c> forest.
    /// </summary>
    /// <param name="partition">Partition to remove from <c>this</c> forest.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="partition"/> has been removed and that changed the forest
    /// (i.e. <paramref name="partition"/> was a partition in <c>this</c>);
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// Does <i>not</i> trigger a notification, but unsubscribes <c>this</c> forest from <paramref name="partition"/>
    /// (i.e. future events on <paramref name="partition"/> will not be forwarded to <c>this</c>).
    /// </remarks>
    /// <seealso cref="IForest.RemovePartitions"/>
    protected internal bool RemovePartitionRaw(IPartitionInstance partition);

    #endregion
}

/// <inheritdoc />
public class Forest : IForest
{
    private readonly HashSet<IPartitionInstance> _partitions;
    private readonly IForestNotificationProducer _notificationProducer;
    private readonly INotificationIdProvider _notificationIdProvider;
    private readonly DeduplicatingNotificationForwarder _notificationForwarder;

    /// <inheritdoc cref="IForest"/>
    public Forest()
    {
        _partitions = new HashSet<IPartitionInstance>(new NodeIdComparer<IPartitionInstance>());
        _notificationProducer = new ForestNotificationProducer(this);
        _notificationIdProvider = new NotificationIdProvider(this);
        _notificationForwarder = new DeduplicatingNotificationForwarder(_notificationProducer, this);
    }

    /// <inheritdoc />
    public IReadOnlySet<IPartitionInstance> Partitions => _partitions;

    /// <inheritdoc />
    public bool TryGetPartition(NodeId nodeId, [NotNullWhen(true)] out IPartitionInstance? partition)
    {
        partition = _partitions.FirstOrDefault(p => p.GetId() == nodeId);
        return partition is not null;
    }

    bool IForest.TryGetPartitionRaw(NodeId nodeId, [NotNullWhen(true)] out IPartitionInstance? partition) =>
        TryGetPartition(nodeId, out partition);

    /// <inheritdoc />
    public void AddPartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null)
    {
        foreach (var partition in partitions)
        {
            if (!AddPartitionRaw(partition))
                continue;

            ProducePartitionAddedNotification(notificationId, partition);
        }
    }

    bool IForest.AddPartitionRaw(IPartitionInstance partition) =>
        AddPartitionRaw(partition);

    /// <inheritdoc cref="IForest.AddPartitionRaw" />
    protected internal bool AddPartitionRaw(IPartitionInstance partition)
    {
        if (!_partitions.Add(partition))
            return false;

        ConnectToPartition(partition);
        return true;
    }

    private void ProducePartitionAddedNotification(INotificationId? notificationId, IPartitionInstance partition) =>
        GetNotificationProducer()?.ProduceNotification(new PartitionAddedNotification(partition,
            notificationId ?? _notificationIdProvider.Create()));

    private void ConnectToPartition(IPartitionInstance partition)
    {
        if (partition.GetNotificationProducer() is { } partitionProducer)
        {
            partitionProducer.ConnectTo(_notificationForwarder);
        }
    }

    /// <inheritdoc />
    public void RemovePartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null)
    {
        foreach (var partition in partitions)
        {
            if (!RemovePartitionRaw(partition))
                continue;

            ProducePartitionDeletedNotification(notificationId, partition);
        }
    }

    bool IForest.RemovePartitionRaw(IPartitionInstance partition) =>
        RemovePartitionRaw(partition);

    /// <inheritdoc cref="IForest.RemovePartitionRaw" />
    protected internal bool RemovePartitionRaw(IPartitionInstance partition)
    {
        if (!_partitions.Remove(partition))
            return false;

        UnsubscribeFromPartition(partition);
        return true;
    }


    private void ProducePartitionDeletedNotification(INotificationId? notificationId, IPartitionInstance partition) =>
        GetNotificationProducer()?.ProduceNotification(new PartitionDeletedNotification(partition,
            notificationId ?? _notificationIdProvider.Create()));

    private void UnsubscribeFromPartition(IPartitionInstance partition)
    {
        if (partition.GetNotificationProducer() is { } partitionProducer)
        {
            partitionProducer.Unsubscribe(_notificationForwarder);
        }
    }

    /// <inheritdoc />
    public INotificationSender? GetNotificationSender() =>
        GetNotificationProducer();

    /// <inheritdoc />
    public IForestNotificationProducer? GetNotificationProducer() =>
        _notificationProducer;

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(Forest)}@{GetHashCode()}";

    /// Forwards notifications from all partitions contained in this forest
    /// to this forest's _following_ pipe members.
    ///
    /// <para>
    /// If we moved a node between two of this forest's partitions,
    /// we'd get _two_ times the same notification (by its id).
    /// That's by design, because a receiver connected to only one of the partitions
    /// needs to know something changed.
    /// However, receivers connected to this forest have no use of two times the same notification.
    /// Thus, we filter out notifications with the same id if we receive them within a short window
    /// (window size <paramref name="lookbackSize"/> notifications).
    /// </para> 
    private sealed class DeduplicatingNotificationForwarder(
        IForestNotificationProducer target,
        object? sender,
        int lookbackSize = 4)
        : NotificationPipeBase(sender), INotificationReceiver
    {
        private readonly Queue<INotificationId> _recentlySeenNotificationIds = new(lookbackSize);

        /// <inheritdoc />
        public void Receive(INotificationSender correspondingSender, INotification notification)
        {
            lock (_recentlySeenNotificationIds)
            {
                if (_recentlySeenNotificationIds.Contains(notification.NotificationId))
                    return;

                if (_recentlySeenNotificationIds.Count == lookbackSize)
                    _recentlySeenNotificationIds.Dequeue();

                _recentlySeenNotificationIds.Enqueue(notification.NotificationId);
            }

            target.ProduceNotification(correspondingSender, notification);
        }
    }
}

public static class ForestExtensions
{
    /// <summary>
    /// Enumerates all partitions of <paramref name="forest"/>, and all their direct and indirect children.
    /// Optionally includes directly and indirectly contained annotations.
    /// </summary>
    /// <param name="forest">Forest to find descendants of.</param>
    /// <param name="includeAnnotations">If true, the result includes directly and indirectly contained annotations.</param>
    /// <returns>All directly and indirectly contained nodes of <paramref name="forest"/>.</returns>
    /// <exception cref="TreeShapeException">If containment hierarchy contains cycles.</exception>
    public static IEnumerable<IReadableNode> Descendants(this IForest forest, bool includeAnnotations = true) =>
        forest
            .Partitions
            .SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, includeAnnotations));
}