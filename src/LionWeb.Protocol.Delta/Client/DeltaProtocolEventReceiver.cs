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

namespace LionWeb.Protocol.Delta.Client;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Forest;
using Message.Event;
using Partition;

public class DeltaProtocolEventReceiver : IDisposable
{
    private readonly Dictionary<NodeId, PartitionEventHandler> _partitionEventHandlers = [];

    private readonly ForestEventHandler _forestEventHandler;
    private readonly PartitionSharedNodeMap _sharedNodeMap;
    private readonly SharedKeyedMap _sharedKeyedMap;
    private readonly ForestEventReplicator _forestEventReplicator;

    private readonly DeltaEventToForestEventMapper _forestMapper;
    private readonly DeltaEventToPartitionEventMapper _partitionMapper;

    public DeltaProtocolEventReceiver(ForestEventHandler forestEventHandler, PartitionSharedNodeMap sharedNodeMap,
        SharedKeyedMap sharedKeyedMap, DeserializerBuilder deserializerBuilder,
        ForestEventReplicator forestEventReplicator)
    {
        _forestEventHandler = forestEventHandler;
        _sharedNodeMap = sharedNodeMap;
        _sharedKeyedMap = sharedKeyedMap;
        _forestEventReplicator = forestEventReplicator;

        _forestMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
        _partitionMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);

        foreach (var partition in sharedNodeMap.Values.OfType<IPartitionInstance>())
        {
            OnPartitionAdded(null, partition);
        }

        sharedNodeMap.OnPartitionAdded += OnPartitionAdded;
        sharedNodeMap.OnPartitionRemoved += OnPartitionRemoved;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _sharedNodeMap.OnPartitionAdded -= OnPartitionAdded;
        _sharedNodeMap.OnPartitionRemoved -= OnPartitionRemoved;
    }

    private void OnPartitionAdded(object? _, IPartitionInstance partition)
    {
        var partitionEventHandler = new PartitionEventHandler(this);
        if (!_partitionEventHandlers.TryAdd(partition.GetId(), partitionEventHandler))
            return;

        var replicator = _forestEventReplicator.LookupPartition(partition);
        replicator.ReplicateFrom(partitionEventHandler);
    }

    private void OnPartitionRemoved(object? sender, IPartitionInstance partition) =>
        _partitionEventHandlers.Remove(partition.GetId());

    public void Receive(IDeltaEvent deltaEvent)
    {
        IEvent internalEvent;
        EventHandlerBase eventHandler;

        switch (deltaEvent)
        {
            case IPartitionDeltaEvent:
                internalEvent = _partitionMapper.Map(deltaEvent);
                if (_sharedNodeMap.TryGetPartition(internalEvent.ContextNodeId, out var partition))
                {
                    eventHandler = _partitionEventHandlers[partition.GetId()];
                } else
                {
                    throw new InvalidOperationException();
                }

                break;

            case IForestDeltaEvent:
                internalEvent = _forestMapper.Map(deltaEvent);
                eventHandler = _forestEventHandler;
                break;

            default:
                throw new InvalidOperationException(deltaEvent.ToString());
        }

        eventHandler.Raise(internalEvent);
    }
}