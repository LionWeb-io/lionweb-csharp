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
using Utilities;

/// A collection of model trees, represented by each trees' <see cref="IPartitionInstance">partition</see> (aka root node).
public interface IForest
{
    /// Contains all known partitions.
    /// <seealso cref="AddPartitions"/>
    /// <seealso cref="RemovePartitions"/>
    IReadOnlySet<IPartitionInstance> Partitions { get; }

    /// Adds <paramref name="partitions"/> to <c>this</c> forest.
    void AddPartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null);

    /// Removes <paramref name="partitions"/> from <c>this</c> forest.
    void RemovePartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null);

    /// <c>this</c> forest's notification sender, if any.
    INotificationSender? GetNotificationSender();

    /// <c>this</c> forest's notification producer, if any.
    protected IForestNotificationProducer? GetNotificationProducer();
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
    public void AddPartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null)
    {
        foreach (var partition in partitions)
        {
            if (!_partitions.Add(partition))
                continue;

            ProducePartitionAdded(notificationId, partition);
            ConnectToPartition(partition);
        }
    }

    private void ProducePartitionAdded(INotificationId? notificationId, IPartitionInstance partition) =>
        GetNotificationProducer()?.ProduceNotification(new PartitionAddedNotification(partition,
            notificationId ?? _notificationIdProvider.CreateNotificationId()));

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
            if (!_partitions.Remove(partition))
                continue;

            ProducePartitionRemoved(notificationId, partition);
            UnsubscribeFromPartition(partition);
        }
    }

    private void ProducePartitionRemoved(INotificationId? notificationId, IPartitionInstance partition) =>
        GetNotificationProducer()?.ProduceNotification(new PartitionDeletedNotification(partition,
            notificationId ?? _notificationIdProvider.CreateNotificationId()));

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