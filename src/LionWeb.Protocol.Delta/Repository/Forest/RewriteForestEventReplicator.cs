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
    SharedNodeMap sharedNodeMap,
    EventIdFilteringEventProcessor<IForestEvent> filter,
    EventIdReplacingEventProcessor<IForestEvent> replacingFilter
)
    : ForestEventReplicator(localForest, sharedNodeMap, filter)
{
    public static IEventProcessor<IForestEvent> Create(IForest localForest, SharedNodeMap sharedNodeMap)
    {
        var filter = new EventIdFilteringEventProcessor<IForestEvent>(null);
        var replacingFilter = new EventIdReplacingEventProcessor<IForestEvent>(null);
        var replicator = new RewriteForestEventReplicator(localForest, sharedNodeMap, filter, replacingFilter);
        var result = new CompositeEventProcessor<IForestEvent>([filter, replacingFilter, replicator], localForest);
        replicator.Init();
        return result;
    }

    protected override IEventProcessor<IPartitionEvent> CreatePartitionEventReplicator(IPartitionInstance partition) =>
        RewritePartitionEventReplicator.Create(partition, SharedNodeMap);

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