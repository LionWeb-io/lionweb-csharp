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
public class ForestEventReplicator : EventReplicatorBase<IForestEvent>
{
    public static IEventProcessor<IForestEvent> Create(IForest localForest, SharedNodeMap sharedNodeMap)
    {
        var filter = new EventIdFilteringEventProcessor<IForestEvent>(null);
        var replicator = new ForestEventReplicator(localForest, sharedNodeMap, filter);
        var result = new CompositeEventProcessor<IForestEvent>([filter, replicator], localForest);
        replicator.Init();
        return result;
    }

    private readonly IForest _localForest;

    private readonly Dictionary<NodeId, IEventProcessor<IPartitionEvent>> _localPartitionReplicators = [];

    protected ForestEventReplicator(IForest localForest, SharedNodeMap sharedNodeMap, EventIdFilteringEventProcessor<IForestEvent> filter) :
        base(sharedNodeMap, filter)
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
        if (forestPublisher is IForestProcessor processor)
            IProcessor.Forward(processor, this);
    }

    /// <see cref="IPublisher{TEvent}.Unsubscribe{TSubscribedEvent}">Unsubscribes</see>
    /// from the <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>. 
    // public override void Dispose()
    // {
    //     base.Dispose();
    //
    //     foreach (var localPartition in _localPartitionReplicators.Values)
    //     {
    //         localPartition.Dispose();
    //     }
    //
    //     var forestListener = _localForest.GetPublisher();
    //     if (forestListener == null)
    //         return;
    //
    //     forestListener.Unsubscribe<IForestEvent>(LocalHandler);
    //
    //     GC.SuppressFinalize(this);
    // }

    #region Local

    public IEventProcessor<IPartitionEvent> LookupPartitionReplicator(IPartitionInstance partition) =>
        _localPartitionReplicators[partition.GetId()];

    protected virtual IEventProcessor<IPartitionEvent> CreatePartitionEventReplicator(IPartitionInstance partition) =>
        PartitionEventReplicator.Create(partition, SharedNodeMap);

    private void LocalHandler(object? _, IForestEvent forestEvent)
    {
        switch (forestEvent)
        {
            case PartitionAddedEvent e:
                OnLocalNewPartition(e);
                break;
            case PartitionDeletedEvent e:
                OnLocalPartitionDeleted(e);
                break;
        }
    }

    private void OnLocalNewPartition(PartitionAddedEvent partitionAddedEvent) =>
        RegisterPartition(partitionAddedEvent.NewPartition);

    private void OnLocalPartitionDeleted(PartitionDeletedEvent partitionDeletedEvent) =>
        UnregisterPartition(partitionDeletedEvent.DeletedPartition);

    private void RegisterPartition(IPartitionInstance partition)
    {
        // var forwarder = new PartitionEventProcessor(this);
        var replicator = CreatePartitionEventReplicator(partition);
        if (!_localPartitionReplicators.TryAdd(partition.GetId(), replicator))
            throw new DuplicateNodeIdException(partition, Lookup(partition.GetId()));
        // replicator.Init();
        // replicator.ReplicateFrom(forwarder);
    }

    private void UnregisterPartition(IPartitionInstance partition)
    {
        var partitionEventApplier = LookupPartitionReplicator(partition);
        // partitionEventApplier.Dispose();
        _localPartitionReplicators.Remove(partition.GetId());
    }

    #endregion

    #region Remote

    /// <inheritdoc />
    protected override void ProcessEvent(IForestEvent? forestEvent)
    {
        Debug.WriteLine($"{this.GetType()}: processing event {forestEvent}");
        switch (forestEvent)
        {
            case PartitionAddedEvent e:
                OnRemoteNewPartition(e);
                break;

            case PartitionDeletedEvent e:
                OnRemotePartitionDeleted(e);
                break;
        }
    }

    private void OnRemoteNewPartition(PartitionAddedEvent partitionAddedEvent) =>
        SuppressEventForwarding(partitionAddedEvent, () =>
        {
            var newPartition = (INode)partitionAddedEvent.NewPartition;

            var clone = (IPartitionInstance)AdjustRemoteNode(newPartition);

            _localForest.AddPartitions([clone], partitionAddedEvent.EventId);

            var remoteListener = partitionAddedEvent.NewPartition.GetPublisher();
            if (remoteListener is IPartitionProcessor processor)
                IProcessor.Forward(processor, LookupPartitionReplicator(clone));
                // new EventForwarder<IPartitionEvent>(remoteListener, LookupPartitionReplicator(clone));
                // LookupPartitionReplicator(clone).ReplicateFrom(remoteListener);
        });

    private void OnRemotePartitionDeleted(PartitionDeletedEvent partitionDeletedEvent) =>
        SuppressEventForwarding(partitionDeletedEvent, () =>
        {
            var localPartition = (IPartitionInstance)Lookup(partitionDeletedEvent.DeletedPartition.GetId());
            _localForest.RemovePartitions([localPartition],  partitionDeletedEvent.EventId);
        });

    #endregion
}