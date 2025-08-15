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

public class SharedPartitionReplicatorMap
{
    private readonly Dictionary<NodeId, INotificationHandler> _localPartitionReplicators = [];

    public INotificationHandler Lookup(NodeId partitionId) =>
        _localPartitionReplicators[partitionId];

    public void Register(NodeId partitionId, INotificationHandler replicator)
    {
        if (!_localPartitionReplicators.TryAdd(partitionId, replicator))
            throw new ArgumentException(
                $"Duplicate partition replicator: {partitionId}: {replicator} {Lookup(partitionId)}");
    }

    public void Unregister(NodeId partitionId)
    {
        if (!_localPartitionReplicators.Remove(partitionId))
            throw new ArgumentException($"Unknown partition replicator: {partitionId}");
    }
}