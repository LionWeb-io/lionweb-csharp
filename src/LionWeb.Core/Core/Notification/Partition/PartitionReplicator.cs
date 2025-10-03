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

using Pipe;

/// Replicates notifications for a <i>local</i> <see cref="IPartitionInstance">partition</see>.
/// <inheritdoc cref="RemoteReplicator"/>
public static class PartitionReplicator
{
    public static INotificationHandler Create(IPartitionInstance localPartition, SharedNodeMap sharedNodeMap,
        object? sender)
    {
        var internalSender = sender ?? localPartition.GetId();
        var filter = new IdFilteringNotificationFilter(internalSender);
        var remoteReplicator = new RemoteReplicator(null, sharedNodeMap, filter, internalSender);
        var localReplicator = new LocalReplicator(null, sharedNodeMap, internalSender);

        var result = new MultipartNotificationHandler([remoteReplicator, filter],
            sender ?? $"Composite of {nameof(PartitionReplicator)} {localPartition.GetId()}");

        sharedNodeMap.RegisterNode(localPartition);

        var partitionSender = localPartition.GetNotificationSender();
        if (partitionSender == null)
            return result;

        partitionSender.ConnectTo(localReplicator);
        localReplicator.ConnectTo(filter);

        return result;
    }
}