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

namespace LionWeb.Protocol.Delta.Repository.Forest;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Partition;

internal class RewriteForestEventReplicator(
    IForest localForest,
    SharedPartitionReplicatorMap sharedPartitionReplicatorMap,
    SharedNodeMap sharedNodeMap,
    EventIdFilteringEventProcessor<IForestEvent> filter,
    EventIdReplacingEventProcessor<IForestEvent> replacingFilter,
    object? sender
    ) : ForestEventReplicator(localForest, sharedPartitionReplicatorMap, sharedNodeMap, filter, sender)
{
    public static IEventProcessor<IForestEvent> Create(IForest localForest, SharedPartitionReplicatorMap sharedPartitionReplicatorMap, SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localForest;
        var filter = new EventIdFilteringEventProcessor<IForestEvent>(internalSender);
        var replacingFilter = new EventIdReplacingEventProcessor<IForestEvent>(internalSender);
        var replicator = new RewriteForestEventReplicator(localForest, sharedPartitionReplicatorMap, sharedNodeMap, filter, replacingFilter, internalSender);
        var result = new CompositeEventProcessor<IForestEvent>([replacingFilter, replicator, filter],
            sender ?? $"Composite of {nameof(RewriteForestEventReplicator)} {localForest}");
        replicator.Init();
        return result;
    }

    protected override IEventProcessor<IPartitionEvent> CreatePartitionEventReplicator(IPartitionInstance partition) =>
        RewritePartitionEventReplicator.Create(partition, SharedNodeMap, $"{localForest}.{partition.GetId()}");

    private readonly IEventIdProvider _eventIdProvider = new EventIdProvider(null);

    protected override void SuppressEventForwarding(IForestEvent forestEvent, Action action)
    {
        IEventId? eventId = _eventIdProvider.CreateEventId();
        var originalEventId = forestEvent.EventId;
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