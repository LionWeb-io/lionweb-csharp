﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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

public class ForestEventReplicator : EventReplicatorBase<IForestEvent, IForestPublisher>, IForestPublisher
{
    private readonly IForest _localForest;
    private readonly Dictionary<NodeId, PartitionEventReplicator> _localPartitions = [];

    public ForestEventReplicator(IForest localForest, Dictionary<NodeId, IReadableNode>? sharedNodeMap = null) :
        base(localForest.Publisher, localForest.Commander, sharedNodeMap)
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

        var forestPublisher = _localForest.Publisher;
        if (forestPublisher == null)
            return;

        forestPublisher.Subscribe<IForestEvent>(LocalHandler);
    }

    private void LocalHandler(object? sender, IForestEvent @event)
    {
        switch (@event)
        {
            case NewPartitionEvent a:
                OnLocalNewPartition(sender, a);
                break;
            case PartitionDeletedEvent a:
                OnLocalPartitionDeleted(sender, a);
                break;
        }
    }

    /// <inheritdoc />
    protected override void ProcessEvent(object? sender, IForestEvent? @event)
    {
        switch (@event)
        {
            case NewPartitionEvent a:
                OnRemoteNewPartition(null, a);
                break;

            case PartitionDeletedEvent a:
                OnRemotePartitionDeleted(null, a);
                break;
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();

        foreach (var localPartition in _localPartitions.Values)
        {
            localPartition.Dispose();
        }

        var forestListener = _localForest.Publisher;
        if (forestListener == null)
            return;

        forestListener.Unsubscribe<IForestEvent>(LocalHandler);
    }

    private void RegisterPartition(IPartitionInstance partition)
    {
        PartitionEventReplicator replicator = new PartitionEventReplicator(partition, _nodeById);
        if (!_localPartitions.TryAdd(partition.GetId(), replicator))
            throw new DuplicateNodeIdException(partition, Lookup(partition.GetId()));
    }

    private PartitionEventReplicator LookupPartition(IPartitionInstance partition) =>
        _localPartitions[partition.GetId()];

    private void UnregisterPartition(IPartitionInstance partition)
    {
        var partitionEventApplier = LookupPartition(partition);
        partitionEventApplier.Dispose();
        _localPartitions.Remove(partition.GetId());
    }

    #region Local

    private void OnLocalNewPartition(object? sender, NewPartitionEvent @event) =>
        RegisterPartition(@event.NewPartition);

    private void OnLocalPartitionDeleted(object? sender, PartitionDeletedEvent @event) =>
        UnregisterPartition(@event.DeletedPartition);

    #endregion

    #region Remote

    private void OnRemoteNewPartition(object? sender, NewPartitionEvent @event) =>
        PauseCommands(() =>
        {
            var newPartition = (INode)@event.NewPartition;

            var clone = (IPartitionInstance)Clone(newPartition);

            _localForest.AddPartitions([clone]);

            var remoteListener = @event.NewPartition.GetPublisher();
            if (remoteListener != null)
                LookupPartition(clone).Subscribe(remoteListener);

            return null;
        });

    private void OnRemotePartitionDeleted(object? sender, PartitionDeletedEvent @event) =>
        PauseCommands(() =>
        {
            var localPartition = (IPartitionInstance)Lookup(@event.DeletedPartition.GetId());
            _localForest.RemovePartitions([localPartition]);

            return null;
        });

    #endregion
}