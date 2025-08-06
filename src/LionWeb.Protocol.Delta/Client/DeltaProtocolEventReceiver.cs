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

using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Forest;
using Message.Event;
using Partition;
using Repository;

public class DeltaProtocolEventReceiver
    : DeltaProtocolReceiverBase<IDeltaEvent, IPartitionDeltaEvent, IForestDeltaEvent>
{
    private readonly DeltaEventToForestEventMapper _forestMapper;
    private readonly DeltaEventToPartitionEventMapper _partitionMapper;

    public DeltaProtocolEventReceiver(
        PartitionSharedNodeMap sharedNodeMap,
        SharedKeyedMap sharedKeyedMap,
        DeserializerBuilder deserializerBuilder,
        ForestEventReplicator forestEventReplicator)
        : base(sharedNodeMap, forestEventReplicator)
    {
        _forestMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
        _partitionMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
    }

    /// <inheritdoc />
    protected override IEvent MapPartition(IPartitionDeltaEvent partitionContent) =>
        _partitionMapper.Map(partitionContent);

    /// <inheritdoc />
    protected override IEvent MapForest(IForestDeltaEvent forestContent) =>
        _forestMapper.Map(forestContent);

    // private void OnPartitionAdded(object? _, IPartitionInstance partition)
    // {
    //     var partitionEventForwarder = new PartitionEventForwarder(this);
    //     if (!_partitionEventForwarders.TryAdd(partition.GetId(), partitionEventForwarder))
    //         return;
    //
    //     // var replicator = _forestEventReplicator.LookupPartitionReplicator(partition);
    //     // replicator.ReplicateFrom(partitionEventForwarder);
    // }
    //
    // private void OnPartitionRemoved(object? sender, IPartitionInstance partition) =>
    //     _partitionEventForwarders.Remove(partition.GetId());
    //
    // private void Receive(IDeltaEvent deltaEvent)
    // {
    //     IEvent internalEvent;
    //     EventForwarderBase eventForwarder;
    //
    //     switch (deltaEvent)
    //     {
    //         case IPartitionDeltaEvent:
    //             internalEvent = _partitionMapper.Map(deltaEvent);
    //             eventForwarder = _forestEventForwarder;
    //             if (_sharedNodeMap.TryGetPartition(internalEvent.ContextNodeId, out var partition))
    //             {
    //                 // eventForwarder = _partitionEventForwarders[partition.GetId()];
    //                 // eventForwarder = _forestEventReplicator.LookupPartitionForwarder(partition);
    //             } else
    //             {
    //                 throw new InvalidOperationException();
    //             }
    //
    //             break;
    //
    //         case IForestDeltaEvent:
    //             internalEvent = _forestMapper.Map(deltaEvent);
    //             eventForwarder = _forestEventForwarder;
    //             break;
    //
    //         default:
    //             throw new InvalidOperationException(deltaEvent.ToString());
    //     }
    //
    //     eventForwarder.Raise(internalEvent);
    // }
}