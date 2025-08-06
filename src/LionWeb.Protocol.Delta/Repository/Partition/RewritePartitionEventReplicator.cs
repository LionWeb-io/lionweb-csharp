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

internal class RewritePartitionEventReplicator(
    IPartitionInstance localPartition,
    EventIdFilteringEventProcessor<IPartitionEvent> filter,
    EventIdReplacingEventProcessor<IPartitionEvent> replacingFilter,
    SharedNodeMap sharedNodeMap)
    : PartitionEventReplicator(localPartition, sharedNodeMap, filter)
{
    public static IEventProcessor<IPartitionEvent> Create(IPartitionInstance localPartition, SharedNodeMap sharedNodeMap)
    {
        var filter = new EventIdFilteringEventProcessor<IPartitionEvent>(null);
        var replacingFilter = new EventIdReplacingEventProcessor<IPartitionEvent>(null);
        var replicator = new RewritePartitionEventReplicator(localPartition, filter, replacingFilter, sharedNodeMap);
        var result = new CompositeEventProcessor<IPartitionEvent>([filter, replacingFilter, replicator], localPartition);
        replicator.Init();
        return result;
    }
    
    private readonly IEventIdProvider _eventIdProvider = new EventIdProvider(null);

    protected override void SuppressEventForwarding(IPartitionEvent partitionEvent, Action action)
    {
        IEventId? eventId = _eventIdProvider.CreateEventId();
        var originalEventId = partitionEvent.EventId;
        replacingFilter.RegisterReplacementEventId(eventId, originalEventId);
        filter.RegisterEventId(eventId);

        try
        {
            action();
        } finally
        {
            filter.UnregisterEventId(eventId);
            replacingFilter.UnregisterReplacementEventId(eventId);
        }
    }
}

internal class EventIdReplacingEventProcessor<TEvent>(object? sender)
    : FilteringEventProcessor<TEvent>(sender) where TEvent : class, IEvent
{
    private readonly Dictionary<IEventId, IEventId> _originalEventIds = [];

    public void RegisterReplacementEventId(IEventId eventId, IEventId original) =>
        _originalEventIds[eventId] = original;

    public void UnregisterReplacementEventId(IEventId eventId) =>
        _originalEventIds.Remove(eventId);

    protected override TEvent? Filter(TEvent @event)
    {
        TEvent result = @event;
        if (_originalEventIds.TryGetValue(@event.EventId, out var originalId))
        {
            result.EventId = originalId;
        }

        return result;
    }
}