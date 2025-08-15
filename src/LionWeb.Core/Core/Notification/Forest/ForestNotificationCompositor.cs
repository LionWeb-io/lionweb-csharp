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
using M1;
using Partition;

public class ForestNotificationCompositor : NotificationCompsitorBase<IForestNotification>
{
    private readonly Dictionary<NodeId, PartitionNotificationCompositor> _partitionCompositors = [];

    public ForestNotificationCompositor(IForest forest, object? handlerId) : base(handlerId)
    {
        foreach (var partition in forest.Partitions)
            RegisterPartition(partition);
    }

    /// <inheritdoc />
    public override void Receive(IForestNotification message)
    {
        switch (message)
        {
            case PartitionAddedNotification n:
                RegisterPartition(n.NewPartition);
                break;
            case PartitionDeletedNotification n:
                UnregisterPartition(n.DeletedPartition);
                break;
        }

        if (!TryAdd(message))
            Send(message);
    }
    
    protected internal bool TryAdd(INotification notification)
    {
        if (!_composites.TryPeek(out var composite))
            return false;

        composite.AddPart(notification);
        return true;
    }
    
    protected virtual PartitionNotificationCompositor CreateCompositor(IPartitionInstance partition, string sender) =>
        new(sender, this);

    private void RegisterPartition(IPartitionInstance partition)
    {
        var partitionId = partition.GetId();
        var compositor = CreateCompositor(partition, $"{Sender}.{partitionId}");
        if (!_partitionCompositors.TryAdd(partitionId, compositor))
            throw new ArgumentException(
                $"Duplicate partition compositor: {partitionId}: {compositor} {_partitionCompositors[partitionId]}");
    }

    private void UnregisterPartition(IPartitionInstance partition)
    {
        var partitionId = partition.GetId();
        if (!_partitionCompositors.Remove(partitionId))
            throw new ArgumentException($"Unknown partition compositor: {partitionId}");
    }
}