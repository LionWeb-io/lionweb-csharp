// Copyright 2025 LionWeb Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
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
// SPDX-FileCopyrightText: 2025 LionWeb Project
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Protocol.Delta.Repository.Partition;

using Core;
using Core.M1.Event;
using Core.M1.Event.Partition;
using Core.M1.Event.Processor;

internal static class RewritePartitionEventReplicator
{
    public static new IEventProcessor<IPartitionEvent> Create(IPartitionInstance localPartition,
        SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localPartition.GetId();
        var filter = new EventIdFilteringEventProcessor<IPartitionEvent>(internalSender);
        var replacingFilter = new EventIdReplacingEventProcessor<IPartitionEvent>(internalSender);
        var remoteReplicator =
            new RewriteRemotePartitionEventReplicator(localPartition, filter, replacingFilter, sharedNodeMap,
                internalSender);
        var localReplicator = new LocalPartitionEventReplicator(sharedNodeMap, internalSender);

        var result = new CompositeEventProcessor<IPartitionEvent>([replacingFilter, remoteReplicator, filter],
            sender ?? $"Composite of {nameof(RewritePartitionEventReplicator)} {localPartition.GetId()}");

        var partitionProcessor = localPartition.GetProcessor();
        if (partitionProcessor != null)
        {
            IProcessor.Connect(partitionProcessor, localReplicator);
            IProcessor.Connect(localReplicator, filter);
        }

        return result;
    }
}

internal class RewriteRemotePartitionEventReplicator(
    IPartitionInstance localPartition,
    EventIdFilteringEventProcessor<IPartitionEvent> filter,
    EventIdReplacingEventProcessor<IPartitionEvent> replacingFilter,
    SharedNodeMap sharedNodeMap,
    object? sender)
    : RemotePartitionEventReplicator(localPartition, sharedNodeMap, filter, sender)
{
    private readonly IEventIdProvider _eventIdProvider = new EventIdProvider(null);

    protected override void SuppressEventForwarding(IPartitionEvent partitionEvent, Action action)
    {
        IEventId eventId = _eventIdProvider.CreateEventId();
        var originalEventId = partitionEvent.EventId;
        replacingFilter.RegisterReplacementEventId(eventId, originalEventId);
        Filter.RegisterEventId(eventId);

        try
        {
            action();
        } finally
        {
            Filter.UnregisterEventId(eventId);
            replacingFilter.UnregisterReplacementEventId(eventId);
        }
    }
}