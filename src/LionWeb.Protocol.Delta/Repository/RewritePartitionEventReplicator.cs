﻿// Copyright 2025 LionWeb Project
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

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1.Event;
using Core.M1.Event.Partition;
using System.Diagnostics;

internal class RewritePartitionEventReplicator(
    IPartitionInstance localPartition,
    Dictionary<String, IReadableNode>? sharedNodeMap = null)
    : PartitionEventReplicator(localPartition, sharedNodeMap)
{
    private readonly Dictionary<IEventId, IEventId> _originalEventIds = [];
        
    protected override void SuppressEventForwarding(IPartitionEvent partitionEvent, Action action)
    {
        IEventId? eventId = null;
        if (_localCommander != null)
        {
            eventId = _localCommander.CreateEventId();
            var originalEventId = partitionEvent.EventId;
            _originalEventIds[eventId] = originalEventId;
            _localCommander.RegisterEventId(eventId);
            RegisterEventId(eventId);
        }

        try
        {
            action();
        } finally
        {
            if (eventId != null)
            {
                UnregisterEventId(eventId);
                _originalEventIds.Remove(eventId);
            }
        }
    }

    protected override TSubscribedEvent? Filter<TSubscribedEvent>(IPartitionEvent partitionEvent) where TSubscribedEvent : class
    {
        IPartitionEvent? result = base.Filter<TSubscribedEvent>(partitionEvent);
        Debug.WriteLine($"result: {result}");
        if (_originalEventIds.TryGetValue(partitionEvent.EventId, out var originalId))
        {
            Debug.WriteLine($"originalId: {originalId}");
            result = partitionEvent;
            result.EventId = originalId;
        }

        return (TSubscribedEvent?)result;
    }
}