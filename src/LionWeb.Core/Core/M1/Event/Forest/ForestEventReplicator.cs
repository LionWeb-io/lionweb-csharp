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
using System.Diagnostics;

/// Replicates events for a <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>.
/// <inheritdoc cref="EventReplicatorBase{TEvent,TPublisher}"/>
public class ForestEventReplicator : EventReplicatorBase<IForestNotification, IForestPublisher>, IForestPublisher
{
    private readonly IForest _localForest;
    private readonly Dictionary<NodeId, PartitionEventReplicator> _localPartitions = [];

    public ForestEventReplicator(IForest localForest, SharedNodeMap sharedNodeMap = null) :
        base(localForest.GetPublisher(), localForest.GetCommander(), sharedNodeMap)
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

        var forestPublisher = _localForest.GetPublisher();
        if (forestPublisher == null)
            return;

        forestPublisher.Subscribe<IForestNotification>(LocalHandler);
    }

    private void LocalHandler(object? sender, IForestNotification forestNotification)
    {
        switch (forestNotification)
        {
            case PartitionAddedNotification a:
                OnLocalNewPartition(sender, a);
                break;
            case PartitionDeletedNotification a:
                OnLocalPartitionDeleted(sender, a);
                break;
        }
    }

    /// <inheritdoc />
    protected override void ProcessEvent(object? sender, IForestNotification? forestEvent)
    {
        Debug.WriteLine($"{this.GetType()}: processing event {forestEvent}");
        switch (forestEvent)
        {
            case PartitionAddedNotification a:
                OnRemoteNewPartition(null, a);
                break;

            case PartitionDeletedNotification a:
                OnRemotePartitionDeleted(null, a);
                break;
        }
    }

    /// <see cref="IPublisher{TEvent}.Unsubscribe{TSubscribedEvent}">Unsubscribes</see>
    /// from the <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>. 
    public override void Dispose()
    {
        base.Dispose();

        foreach (var localPartition in _localPartitions.Values)
        {
            localPartition.Dispose();
        }

        var forestListener = _localForest.GetPublisher();
        if (forestListener == null)
            return;

        forestListener.Unsubscribe<IForestNotification>(LocalHandler);

        GC.SuppressFinalize(this);
    }

    private void RegisterPartition(IPartitionInstance partition)
    {
        PartitionEventReplicator replicator = new PartitionEventReplicator(partition, NodeById);
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

    private void OnLocalNewPartition(object? sender, PartitionAddedNotification partitionAddedNotification) =>
        RegisterPartition(partitionAddedNotification.NewPartition);

    private void OnLocalPartitionDeleted(object? sender, PartitionDeletedNotification partitionDeletedNotification) =>
        UnregisterPartition(partitionDeletedNotification.DeletedPartition);

    #endregion

    #region Remote

    private void OnRemoteNewPartition(object? sender, PartitionAddedNotification partitionAddedNotification) =>
        SuppressEventForwarding(partitionAddedNotification, () =>
        {
            var newPartition = (INode)partitionAddedNotification.NewPartition;

            var clone = (IPartitionInstance)Clone(newPartition);

            _localForest.AddPartitions([clone]);

            var remoteListener = partitionAddedNotification.NewPartition.GetPublisher();
            if (remoteListener != null)
                LookupPartition(clone).ReplicateFrom(remoteListener);
        });

    private void OnRemotePartitionDeleted(object? sender, PartitionDeletedNotification partitionDeletedNotification) =>
        SuppressEventForwarding(partitionDeletedNotification, () =>
        {
            var localPartition = (IPartitionInstance)Lookup(partitionDeletedNotification.DeletedPartition.GetId());
            _localForest.RemovePartitions([localPartition]);
        });

    #endregion
}