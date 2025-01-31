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

public class ForestEventReplicator : EventReplicatorBase<IForestEvent, IForestPublisher>
{
    private readonly IForest _localForest;
    private readonly Dictionary<NodeId, PartitionEventReplicator> _localPartitions = [];
    private readonly List<IForestPublisher> _listeners = [];

    public ForestEventReplicator(IForest localForest, Dictionary<NodeId, IReadableNode>? sharedNodeMap = null) :
        base(sharedNodeMap)
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

        var forestListener = _localForest.Publisher;
        if (forestListener == null)
            return;

        forestListener.NewPartition += OnLocalNewPartition;
        forestListener.PartitionDeleted += OnLocalPartitionDeleted;
    }

    /// <inheritdoc />
    public override void Subscribe(IForestPublisher publisher)
    {
        SubscribeChannel(publisher);
        _listeners.Add(publisher);

        publisher.NewPartition += OnRemoteNewPartition;
        publisher.PartitionDeleted += OnRemotePartitionDeleted;
    }

    /// <inheritdoc />
    protected override void ProcessEvent(IForestEvent @event)
    {
        switch (@event)
        {
            case IForestPublisher.NewPartitionArgs a:
                OnRemoteNewPartition(null, a);
                break;
            
            case IForestPublisher.PartitionDeletedArgs a:
                OnRemotePartitionDeleted(null, a);
                break;
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();
        
        foreach (var listener in _listeners)
        {
            listener.NewPartition -= OnRemoteNewPartition;
            listener.PartitionDeleted -= OnRemotePartitionDeleted;
        }
        
        foreach (var localPartition in _localPartitions.Values)
        {
            localPartition.Dispose();
        }

        var forestListener = _localForest.Publisher;
        if (forestListener == null)
            return;

        forestListener.NewPartition -= OnLocalNewPartition;
        forestListener.PartitionDeleted -= OnLocalPartitionDeleted;
    }

    private void RegisterPartition(IPartitionInstance partition)
    {
        PartitionEventReplicator replicator = new PartitionEventReplicator(partition, _nodeById);
        if (!_localPartitions.TryAdd(partition.GetId(), replicator))
            throw new DuplicateNodeIdException(partition, Lookup(partition.GetId()));
    }

    private PartitionEventReplicator LookupPartition(IPartitionInstance partition) => _localPartitions[partition.GetId()];

    private void UnregisterPartition(IPartitionInstance partition)
    {
        var partitionEventApplier = LookupPartition(partition);
        partitionEventApplier.Dispose();
        _localPartitions.Remove(partition.GetId());
    }

    #region Local

    private void OnLocalNewPartition(object? sender, IForestPublisher.NewPartitionArgs args) =>
        RegisterPartition(args.NewPartition);

    private void OnLocalPartitionDeleted(object? sender, IForestPublisher.PartitionDeletedArgs args) =>
        UnregisterPartition(args.DeletedPartition);

    #endregion

    #region Remote

    private void OnRemoteNewPartition(object? sender, IForestPublisher.NewPartitionArgs args)
    {
        var newPartition = (INode)args.NewPartition;

        var clone = (IPartitionInstance)Clone(newPartition);

        _localForest.AddPartitions([clone]);

        var remoteListener = args.NewPartition.Publisher;
        if (remoteListener != null)
            LookupPartition(clone).Subscribe(remoteListener);
    }

    private void OnRemotePartitionDeleted(object? sender, IForestPublisher.PartitionDeletedArgs args)
    {
        var localPartition = (IPartitionInstance)Lookup(args.DeletedPartition.GetId());
        _localForest.RemovePartitions([localPartition]);
    }

    #endregion
}