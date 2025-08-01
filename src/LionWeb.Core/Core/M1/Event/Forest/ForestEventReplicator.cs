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
public class ForestEventReplicator : EventReplicatorBase<IForestEvent, IForestPublisher>, IForestPublisher
{
    private readonly IForest _localForest;
    private readonly Dictionary<NodeId, PartitionEventReplicator> _localPartitionReplicators = [];
    private readonly Dictionary<NodeId, PartitionEventForwarder> _localPartitionForwarders = [];

    public ForestEventReplicator(IForest localForest, SharedNodeMap sharedNodeMap = null) :
        base(localForest.GetPublisher(), localForest.GetCommander(), sharedNodeMap)
    {
        _localForest = localForest;
    }

    public void Init()
    {
        foreach (var partition in _localForest.Partitions)
        {
            RegisterPartition(partition);
        }

        var forestPublisher = _localForest.GetPublisher();
        if (forestPublisher == null)
            return;

        forestPublisher.Subscribe<IForestEvent>(LocalHandler);
    }

    private void LocalHandler(object? sender, IForestEvent forestEvent)
    {
        switch (forestEvent)
        {
            case PartitionAddedEvent a:
                OnLocalNewPartition(sender, a);
                break;
            case PartitionDeletedEvent a:
                OnLocalPartitionDeleted(sender, a);
                break;
        }
    }

    /// <inheritdoc />
    protected override void ProcessEvent(object? sender, IForestEvent? forestEvent)
    {
        Debug.WriteLine($"{this.GetType()}: processing event {forestEvent}");
        switch (forestEvent)
        {
            case PartitionAddedEvent a:
                OnRemoteNewPartition(null, a);
                break;

            case PartitionDeletedEvent a:
                OnRemotePartitionDeleted(null, a);
                break;
        }
    }

    /// <see cref="IPublisher{TEvent}.Unsubscribe{TSubscribedEvent}">Unsubscribes</see>
    /// from the <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>. 
    public override void Dispose()
    {
        base.Dispose();

        foreach (var localPartition in _localPartitionReplicators.Values)
        {
            localPartition.Dispose();
        }

        var forestListener = _localForest.GetPublisher();
        if (forestListener == null)
            return;

        forestListener.Unsubscribe<IForestEvent>(LocalHandler);

        GC.SuppressFinalize(this);
    }

    private void RegisterPartition(IPartitionInstance partition)
    {
        var forwarder = new PartitionEventForwarder(this);
        PartitionEventReplicator replicator = CreatePartitionEventReplicator(partition);
        if (!_localPartitionForwarders.TryAdd(partition.GetId(), forwarder) || !_localPartitionReplicators.TryAdd(partition.GetId(), replicator))
            throw new DuplicateNodeIdException(partition, Lookup(partition.GetId()));
        replicator.Init();
        replicator.ReplicateFrom(forwarder);
    }

    protected virtual PartitionEventReplicator CreatePartitionEventReplicator(IPartitionInstance partition) =>
        new(partition, SharedNodeMap);

    // public void RegisterPartition(IPartitionInstance partition, PartitionEventReplicator replicator)
    // {
    //     if (!_localPartitions.TryAdd(partition.GetId(), replicator))
    //         throw new DuplicateNodeIdException(partition, Lookup(partition.GetId()));
    // }
    
    public PartitionEventReplicator LookupPartitionReplicator(IPartitionInstance partition) =>
        _localPartitionReplicators[partition.GetId()];
    
    public PartitionEventForwarder LookupPartitionForwarder(IPartitionInstance partition) =>
        _localPartitionForwarders[partition.GetId()];

    private void UnregisterPartition(IPartitionInstance partition)
    {
        var partitionEventApplier = LookupPartitionReplicator(partition);
        partitionEventApplier.Dispose();
        _localPartitionReplicators.Remove(partition.GetId());
        _localPartitionForwarders.Remove(partition.GetId());
    }

    #region Local

    private void OnLocalNewPartition(object? sender, PartitionAddedEvent partitionAddedEvent)
    {
        RegisterPartition(partitionAddedEvent.NewPartition);
    }

    private void OnLocalPartitionDeleted(object? sender, PartitionDeletedEvent partitionDeletedEvent) =>
        UnregisterPartition(partitionDeletedEvent.DeletedPartition);

    #endregion

    #region Remote

    private void OnRemoteNewPartition(object? sender, PartitionAddedEvent partitionAddedEvent) =>
        SuppressEventForwarding(partitionAddedEvent, () =>
        {
            var newPartition = (INode)partitionAddedEvent.NewPartition;

            var clone = (IPartitionInstance)AdjustRemoteNode(newPartition);

            _localForest.AddPartitions([clone], partitionAddedEvent.EventId);

            var remoteListener = partitionAddedEvent.NewPartition.GetPublisher();
            if (remoteListener != null)
                LookupPartitionReplicator(clone).ReplicateFrom(remoteListener);
        });

    private void OnRemotePartitionDeleted(object? sender, PartitionDeletedEvent partitionDeletedEvent) =>
        SuppressEventForwarding(partitionDeletedEvent, () =>
        {
            var localPartition = (IPartitionInstance)Lookup(partitionDeletedEvent.DeletedPartition.GetId());
            _localForest.RemovePartitions([localPartition],  partitionDeletedEvent.EventId);
        });

    #endregion
}