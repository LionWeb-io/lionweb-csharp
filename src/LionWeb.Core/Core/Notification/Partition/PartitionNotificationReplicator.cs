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

namespace LionWeb.Core.Notification.Partition;

using Handler;

/// Replicates notifications for a <i>local</i> <see cref="IPartitionInstance">partition</see>.
/// <inheritdoc cref="RemoteNotificationReplicator"/>
public static class PartitionNotificationReplicator
{
    public static IConnectingNotificationHandler Create(IPartitionInstance localPartition,
        SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localPartition.GetId();
        var filter = new IdFilteringNotificationHandler(internalSender);
        var remoteReplicator =
            new RemoteNotificationReplicator(null, sharedNodeMap, filter, internalSender);
        var localReplicator = new LocalNotificationReplicator(null, sharedNodeMap, internalSender);

        var result = new CompositeNotificationHandler([remoteReplicator, filter],
            sender ?? $"Composite of {nameof(PartitionNotificationReplicator)} {localPartition.GetId()}");

        sharedNodeMap.RegisterNode(localPartition);
        
        var partitionHandler = localPartition.GetNotificationHandler();
        if (partitionHandler == null)
            return result;

        INotificationHandler.Connect(partitionHandler, localReplicator);
        INotificationHandler.Connect(localReplicator, filter);

        return result;
    }
}
