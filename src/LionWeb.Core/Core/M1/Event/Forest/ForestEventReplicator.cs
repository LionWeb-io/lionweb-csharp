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
using Processor;
using System.Diagnostics;

/// Replicates events for a <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>.
/// <inheritdoc cref="EventReplicatorBase{TEvent}"/>
public class ForestEventReplicator : EventReplicatorBase<IForestEvent>
{
    public static IEventProcessor<IForestEvent> Create(IForest localForest,
        SharedPartitionReplicatorMap sharedPartitionReplicatorMap, SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localForest;
        var filter = new EventIdFilteringEventProcessor<IForestEvent>(internalSender);
        var replicator = new ForestEventReplicator(localForest, sharedPartitionReplicatorMap, sharedNodeMap, filter, internalSender);
        var result = new CompositeEventProcessor<IForestEvent>([replicator, filter],
            sender ?? $"Composite of {nameof(ForestEventReplicator)} {localForest}");
        replicator.Init();
        return result;
    }

    private readonly IForest _localForest;
    private readonly SharedPartitionReplicatorMap _sharedPartitionReplicatorMap;

    protected ForestEventReplicator(IForest localForest, SharedPartitionReplicatorMap sharedPartitionReplicatorMap,
        SharedNodeMap sharedNodeMap, EventIdFilteringEventProcessor<IForestEvent> filter, object? sender) :
        base(sharedNodeMap, filter, sender)
    {
        _localForest = localForest;
        _sharedPartitionReplicatorMap = sharedPartitionReplicatorMap;
    }

    protected void Init()
    {
        foreach (var partition in _localForest.Partitions)
        {
            RegisterPartition(partition);
        }

        var forestPublisher = _localForest.GetProcessor();
        if (forestPublisher != null)
            IProcessor.Connect(forestPublisher, new LocalReceiver(_localForest, this));
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
    
    private class LocalReceiver(object? sender, ForestEventReplicator replicator) : EventProcessorBase<IForestEvent>(sender)
    {
        public override void Receive(IForestEvent message)
        {
            switch (message)
            {
                case PartitionAddedEvent partitionAddedEvent:
                    replicator.OnLocalPartitionAdded(partitionAddedEvent);
                    break;
                case PartitionDeletedEvent partitionDeletedEvent:
                    replicator.OnLocalPartitionDeleted(partitionDeletedEvent);
                    break;
            }
            
            replicator.Send(message);
        }
    }

    protected virtual IEventProcessor<IPartitionEvent> CreatePartitionEventReplicator(IPartitionInstance partition, string sender) =>
        PartitionEventReplicator.Create(partition, SharedNodeMap, sender);

    private void OnLocalPartitionAdded(PartitionAddedEvent partitionAddedEvent) =>
        RegisterPartition(partitionAddedEvent.NewPartition);

    private void OnLocalPartitionDeleted(PartitionDeletedEvent partitionDeletedEvent) =>
        UnregisterPartition(partitionDeletedEvent.DeletedPartition);

    private void RegisterPartition(IPartitionInstance partition)
    {
        var replicator = CreatePartitionEventReplicator(partition, $"{_localForest}.{partition.GetId()}");
        _sharedPartitionReplicatorMap.Register(partition.GetId(), replicator);
    }

    private void UnregisterPartition(IPartitionInstance partition) => 
        _sharedPartitionReplicatorMap.Unregister(partition.GetId());

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
        });

    private void OnRemotePartitionDeleted(PartitionDeletedEvent partitionDeletedEvent) =>
        SuppressEventForwarding(partitionDeletedEvent, () =>
        {
            var localPartition = (IPartitionInstance)Lookup(partitionDeletedEvent.DeletedPartition.GetId());
            _localForest.RemovePartitions([localPartition], partitionDeletedEvent.EventId);
        });

    #endregion
}