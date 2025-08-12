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
using Utilities;

/// A collection of model trees, represented by each trees' <see cref="IPartitionInstance">partition</see> (aka root node).
public interface IForest
{
    /// Contains all known partitions.
    /// <seealso cref="AddPartitions"/>
    /// <seealso cref="RemovePartitions"/>
    public IReadOnlySet<IPartitionInstance> Partitions { get; }

    /// Adds <paramref name="partitions"/> to <c>this</c> forest.
    public void AddPartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null);

    /// Removes <paramref name="partitions"/> from <c>this</c> forest.
    public void RemovePartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null);

    /// <c>this</c> forest's processor, if any.
    IForestProcessor? GetProcessor();
}

/// <inheritdoc />
public class Forest : IForest
{
    private readonly HashSet<IPartitionInstance> _partitions;
    private readonly ForestNotificationProcessor _notificationProcessor;
    private readonly INotificationIdProvider _notificationIdProvider;

    /// <inheritdoc cref="IForest"/>
    public Forest()
    {
        _partitions = new HashSet<IPartitionInstance>(new NodeIdComparer<IPartitionInstance>());
        _notificationProcessor = new ForestNotificationProcessor(this);
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
                _notificationProcessor.Receive(new PartitionAddedNotification(partition,
                    notificationId ?? _notificationIdProvider.CreateNotificationId()));
        }
    }

    /// <inheritdoc />
    public void RemovePartitions(IEnumerable<IPartitionInstance> partitions, INotificationId? notificationId = null)
    {
        foreach (var partition in partitions)
        {
            if (_partitions.Remove(partition))
                _notificationProcessor.Receive(new PartitionDeletedNotification(partition,
                    notificationId ?? _notificationIdProvider.CreateNotificationId()));
        }
    }

    /// <inheritdoc />
    public IForestProcessor GetProcessor() => _notificationProcessor;

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Forest)}@{GetHashCode()}";
}