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

namespace LionWeb.Core.M1.Event.Forest;

using Partition;

public class ForestEventApplier : EventApplierBase
{
    private readonly IForest _localForest;
    private readonly Dictionary<NodeId, PartitionEventApplier> _localPartitions = [];
    
    public ForestEventApplier(IForest localForest, Dictionary<NodeId, IReadableNode>? sharedNodeMap = null) : base(sharedNodeMap)
    {
        _localForest = localForest;
        Init();
    }

    private void Init()
    {
        foreach (var partition in _localForest.Partitions)
        {
            RegisterPartition(partition);
        }
        
        var forestListener = _localForest.Listener;
        if (forestListener == null)
            return;

        forestListener.NewPartition += OnLocalNewPartition;
        forestListener.PartitionDeleted += OnLocalPartitionDeleted;
    }

    public override void Dispose()
    {
        foreach (var partitionEventApplier in _localPartitions.Values)
        {
            partitionEventApplier.Dispose();
        }
        
        var forestListener = _localForest.Listener;
        if (forestListener == null)
            return;

        forestListener.NewPartition -= OnLocalNewPartition;
        forestListener.PartitionDeleted -= OnLocalPartitionDeleted;
    }
    
    private void RegisterPartition(IPartitionInstance partition) =>
        _localPartitions[partition.GetId()] = new PartitionEventApplier(partition, _nodeById);

    private void UnregisterPartition(IPartitionInstance partition)
    {
        var partitionEventApplier = _localPartitions[partition.GetId()];
        partitionEventApplier.Dispose();
        _localPartitions.Remove(partition.GetId());
    }

    private void OnLocalNewPartition(object? sender, IForestListener.NewPartitionArgs args) =>
        RegisterPartition(args.NewPartition);

    private void OnLocalPartitionDeleted(object? sender, IForestListener.PartitionDeletedArgs args) =>
        UnregisterPartition(args.DeletedPartition);
}