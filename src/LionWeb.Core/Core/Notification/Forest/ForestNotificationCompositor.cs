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

namespace LionWeb.Core.Notification.Forest;

using Handler;
using Partition;

/// <inheritdoc cref="NotificationCompositorBase{TNotification}"/>
///
/// Composes notifications across several partitions, and changes to partitions within a forest.
///
/// Automatically handles partitions added <i>after</i> this compositor has been connected;
/// cannot handle partitions already existing in the connected forest beforehand. 
public class ForestNotificationCompositor(object? handlerId)
    : NotificationCompositorBase<IForestNotification>(handlerId), IForestNotificationHandler
{
    private readonly Dictionary<NodeId, PartitionNotificationCompositor> _partitionCompositors = [];

    /// <inheritdoc />
    public void Receive(IPartitionNotificationHandler correspondingHandler, IForestNotification message)
    {
        switch (message)
        {
            case PartitionAddedNotification n:
                RegisterPartition(correspondingHandler, n.NewPartition);
                break;
            case PartitionDeletedNotification n:
                UnregisterPartition(correspondingHandler, n.DeletedPartition);
                break;
        }

        if (!TryAdd(message))
            Send(message);
    }

    /// <inheritdoc />
    public override void Receive(IForestNotification message) => throw new NotImplementedException();

    protected internal bool TryAdd(INotification notification)
    {
        if (!_composites.TryPeek(out var composite))
            return false;

        composite.AddPart(notification);
        return true;
    }

    protected virtual PartitionNotificationCompositor CreateCompositor(IPartitionInstance partition, string sender) =>
        new(sender, this);

    private void RegisterPartition(IPartitionNotificationHandler correspondingHandler, IPartitionInstance partition)
    {
        var partitionId = partition.GetId();
        var compositor = CreateCompositor(partition, $"{Sender}.{partitionId}");
        if (!_partitionCompositors.TryAdd(partitionId, compositor))
            throw new ArgumentException(
                $"Duplicate partition compositor: {partitionId}: {compositor} {_partitionCompositors[partitionId]}");

        INotificationHandler.Connect(correspondingHandler, compositor);
    }

    private void UnregisterPartition(IPartitionNotificationHandler correspondingHandler, IPartitionInstance partition)
    {
        var partitionId = partition.GetId();
        if (!_partitionCompositors.Remove(partitionId, out var partitionCompositor))
            throw new ArgumentException($"Unknown partition compositor: {partitionId}");

        correspondingHandler.Unsubscribe<IPartitionNotificationHandler>(partitionCompositor);
        partitionCompositor.Dispose();
    }
}