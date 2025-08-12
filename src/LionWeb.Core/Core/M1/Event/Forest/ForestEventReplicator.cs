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
/// <inheritdoc cref="RemoteEventReplicatorBase{TEvent}"/>
public static class ForestEventReplicator
{
    public static IEventProcessor<IForestEvent> Create(IForest localForest,
        SharedPartitionReplicatorMap sharedPartitionReplicatorMap, SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localForest;
        var filter = new EventIdFilteringEventProcessor<IForestEvent>(internalSender);
        var remoteReplicator = new RemoteForestEventReplicator(localForest, sharedNodeMap, filter, internalSender);
        var localReplicator =
            new LocalForestEventReplicator(localForest, sharedPartitionReplicatorMap, sharedNodeMap, internalSender);

        var result = new CompositeEventProcessor<IForestEvent>([remoteReplicator, filter],
            sender ?? $"Composite of {nameof(ForestEventReplicator)} {localForest}");

        var forestProcessor = localForest.GetProcessor();
        if (forestProcessor != null)
        {
            IProcessor.Connect(forestProcessor, localReplicator);
            IProcessor.Connect(localReplicator, filter);
        }

        return result;
    }
}

public class RemoteForestEventReplicator : RemoteEventReplicatorBase<IForestEvent>
{
    private readonly IForest _localForest;

    protected internal RemoteForestEventReplicator(IForest localForest, SharedNodeMap sharedNodeMap,
        EventIdFilteringEventProcessor<IForestEvent> filter, object? sender) :
        base(sharedNodeMap, filter, sender)
    {
        _localForest = localForest;
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
            var newPartition = partitionAddedEvent.NewPartition;
            _localForest.AddPartitions([newPartition], partitionAddedEvent.EventId);
        });

    private void OnRemotePartitionDeleted(PartitionDeletedEvent partitionDeletedEvent) =>
        SuppressEventForwarding(partitionDeletedEvent, () =>
        {
            var localPartition = (IPartitionInstance)Lookup(partitionDeletedEvent.DeletedPartition.GetId());
            _localForest.RemovePartitions([localPartition], partitionDeletedEvent.EventId);
        });
}

public class LocalForestEventReplicator : EventProcessorBase<IForestEvent>
{
    private readonly SharedPartitionReplicatorMap _sharedPartitionReplicatorMap;
    private readonly SharedNodeMap _sharedNodeMap;

    public LocalForestEventReplicator(IForest localForest, SharedPartitionReplicatorMap sharedPartitionReplicatorMap,
        SharedNodeMap sharedNodeMap, object? sender) : base(sender)
    {
        _sharedPartitionReplicatorMap = sharedPartitionReplicatorMap;
        _sharedNodeMap = sharedNodeMap;

        foreach (var partition in localForest.Partitions)
        {
            RegisterPartition(partition);
        }
    }

    /// <inheritdoc />
    public override void Receive(IForestEvent message)
    {
        switch (message)
        {
            case PartitionAddedEvent partitionAddedEvent:
                OnLocalPartitionAdded(partitionAddedEvent);
                break;
            case PartitionDeletedEvent partitionDeletedEvent:
                OnLocalPartitionDeleted(partitionDeletedEvent);
                break;
        }

        Send(message);
    }

    protected virtual IEventProcessor<IPartitionEvent> CreatePartitionEventReplicator(IPartitionInstance partition,
        string sender) =>
        PartitionEventReplicator.Create(partition, _sharedNodeMap, sender);

    internal void RegisterPartition(IPartitionInstance partition)
    {
        var replicator = CreatePartitionEventReplicator(partition, $"{Sender}.{partition.GetId()}");
        _sharedPartitionReplicatorMap.Register(partition.GetId(), replicator);
    }

    internal void UnregisterPartition(IPartitionInstance partition) =>
        _sharedPartitionReplicatorMap.Unregister(partition.GetId());

    private void OnLocalPartitionAdded(PartitionAddedEvent partitionAddedEvent) =>
        RegisterPartition(partitionAddedEvent.NewPartition);

    private void OnLocalPartitionDeleted(PartitionDeletedEvent partitionDeletedEvent) =>
        UnregisterPartition(partitionDeletedEvent.DeletedPartition);
}