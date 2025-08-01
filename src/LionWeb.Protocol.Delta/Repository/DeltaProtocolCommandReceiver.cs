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

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Forest;
using Message.Command;
using Partition;

public class DeltaProtocolCommandReceiver : IDisposable
{
    private readonly Dictionary<NodeId, PartitionEventForwarder> _partitionEventForwarders = [];

    private readonly ForestEventForwarder _forestEventForwarder;
    private readonly PartitionSharedNodeMap _sharedNodeMap;
    private readonly ForestEventReplicator _forestEventReplicator;
    
    private readonly DeltaCommandToForestEventMapper _forestMapper;
    private readonly DeltaCommandToPartitionEventMapper _partitionMapper;

    public DeltaProtocolCommandReceiver(ForestEventForwarder forestEventForwarder, PartitionSharedNodeMap sharedNodeMap,
        SharedKeyedMap sharedKeyedMap, DeserializerBuilder deserializerBuilder, ForestEventReplicator forestEventReplicator)
    {
        _forestEventForwarder = forestEventForwarder;
        _sharedNodeMap = sharedNodeMap;
        _forestEventReplicator = forestEventReplicator;

        _forestMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
        _partitionMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);

        foreach (var partition in sharedNodeMap.Values.OfType<IPartitionInstance>())
        {
            OnPartitionAdded(null, partition);
        }
        
        _forestEventForwarder.Subscribe<PartitionAddedEvent>(OnPartitionAdded);
        _forestEventReplicator.Subscribe<PartitionAddedEvent>(OnPartitionAdded);
        
        // sharedNodeMap.OnPartitionAdded += OnPartitionAdded;
        // sharedNodeMap.OnPartitionRemoved += OnPartitionRemoved;
    }

    private void OnPartitionAdded(object? sender, PartitionAddedEvent e)
    {
        OnPartitionAdded(sender, e.NewPartition);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        // _sharedNodeMap.OnPartitionAdded -= OnPartitionAdded;
        // _sharedNodeMap.OnPartitionRemoved -= OnPartitionRemoved;
    }

    private void OnPartitionAdded(object? _, IPartitionInstance partition)
    {
        var partitionEventForwarder = new PartitionEventForwarder(this)
        {
            ContainingForestEventForwarder = _forestEventForwarder
        };
        var replicator = _forestEventReplicator.LookupPartition(partition);
        if (_partitionEventForwarders.TryAdd(partition.GetId(), partitionEventForwarder))
        {
            replicator.ReplicateFrom(partitionEventForwarder);
        }
    }

    private void OnPartitionRemoved(object? _, IPartitionInstance partition) =>
        _partitionEventForwarders.Remove(partition.GetId());

    public void Receive(IDeltaCommand deltaCommand)
    {
        IEvent internalEvent;
        EventForwarderBase eventForwarder;

        switch (deltaCommand)
        {
            case IPartitionDeltaCommand:
                internalEvent = _partitionMapper.Map(deltaCommand);
                if (_sharedNodeMap.TryGetPartition(internalEvent.ContextNodeId, out var partition))
                {
                    eventForwarder = _partitionEventForwarders[partition.GetId()];
                } else
                {
                    throw new InvalidOperationException();
                }

                break;
            
            case IForestDeltaCommand:
                internalEvent = _forestMapper.Map(deltaCommand);
                eventForwarder = _forestEventForwarder;
                break;
            
            default:
                throw new InvalidOperationException(deltaCommand.ToString());
        }

        eventForwarder.Raise(internalEvent);
    }
}