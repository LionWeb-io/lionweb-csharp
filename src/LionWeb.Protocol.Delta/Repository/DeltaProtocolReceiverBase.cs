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
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Message;

public abstract class DeltaProtocolReceiverBase<TContent, TPartition, TForest> : IDisposable
    where TContent : IDeltaContent
    where TPartition : TContent, IDeltaContent
    where TForest : TContent, IDeltaContent
{
    private readonly Dictionary<NodeId, PartitionEventForwarder> _partitionEventForwarders = [];

    private readonly ForestEventForwarder _forestEventForwarder;
    private readonly PartitionSharedNodeMap _sharedNodeMap;
    private readonly ForestEventReplicator _forestEventReplicator;

    public DeltaProtocolReceiverBase(ForestEventForwarder forestEventForwarder, PartitionSharedNodeMap sharedNodeMap,
        ForestEventReplicator forestEventReplicator)
    {
        _forestEventForwarder = forestEventForwarder;
        _sharedNodeMap = sharedNodeMap;
        _forestEventReplicator = forestEventReplicator;
    }

    public void Init()
    {
        foreach (var partition in _sharedNodeMap.Values.OfType<IPartitionInstance>())
        {
            OnLocalPartitionAdded(partition);
        }

        _forestEventForwarder.Subscribe<IForestEvent>(LocalHandler);
        _forestEventReplicator.Subscribe<IForestEvent>(LocalHandler);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #region Local

    private void LocalHandler(object? _, IForestEvent forestEvent)
    {
        switch (forestEvent)
        {
            case PartitionAddedEvent e:
                OnLocalPartitionAdded(e.NewPartition);
                break;
            case PartitionDeletedEvent e:
                OnLocalPartitionDeleted(e.DeletedPartition);
                break;
        }
    }

    private void OnLocalPartitionAdded(IPartitionInstance partition)
    {
        var partitionEventForwarder = new PartitionEventForwarder(this);
        var replicator = _forestEventReplicator.LookupPartitionReplicator(partition);
        if (_partitionEventForwarders.TryAdd(partition.GetId(), partitionEventForwarder))
        {
            replicator.ReplicateFrom(partitionEventForwarder);
        }
    }

    private void OnLocalPartitionDeleted(IPartitionInstance partition) =>
        _partitionEventForwarders.Remove(partition.GetId());

    #endregion

    #region Remote

    public void Receive(TContent deltaContent)
    {
        IEvent internalEvent;
        EventForwarderBase eventForwarder;

        switch (deltaContent)
        {
            case TPartition partitionContent:
                internalEvent = MapPartition(partitionContent);
                if (_sharedNodeMap.TryGetPartition(internalEvent.ContextNodeId, out var partition))
                {
                    eventForwarder = _partitionEventForwarders[partition.GetId()];
                } else
                {
                    throw new InvalidOperationException();
                }

                break;

            case TForest forestContent:
                internalEvent = MapForest(forestContent);
                eventForwarder = _forestEventForwarder;
                break;

            default:
                throw new InvalidOperationException(deltaContent.ToString());
        }

        eventForwarder.Raise(internalEvent);
    }

    protected abstract IEvent MapPartition(TPartition partitionContent);
    protected abstract IEvent MapForest(TForest forestContent);

    #endregion
}