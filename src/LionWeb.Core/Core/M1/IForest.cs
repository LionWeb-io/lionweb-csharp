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

    /// <inheritdoc cref="IForest"/>
    public Forest()
    {
        _partitions = new HashSet<IPartitionInstance>(new NodeIdComparer<IPartitionInstance>());
        _notificationProducer = new ForestNotificationProducer(this);
        _notificationIdProvider = new NotificationIdProvider(this);
    }

    /// <inheritdoc />
    public IReadOnlySet<IPartitionInstance> Partitions => _partitions;

    /// <inheritdoc />
    public void AddPartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null)
    {
        foreach (var partition in partitions)
        {
            if (_partitions.Add(partition))
                GetNotificationProducer()?.ProduceNotification(new PartitionAddedNotification(partition,
                    notificationId ?? _notificationIdProvider.CreateNotificationId()));
        }
    }

    /// <inheritdoc />
    public void RemovePartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null)
    {
        foreach (var partition in partitions)
        {
            if (_partitions.Remove(partition))
                GetNotificationProducer()?.ProduceNotification(new PartitionDeletedNotification(partition,
                    notificationId ?? _notificationIdProvider.CreateNotificationId()));
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
}