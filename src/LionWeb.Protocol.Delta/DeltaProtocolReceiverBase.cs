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

namespace LionWeb.Protocol.Delta;

using Core;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Core.M1.Event.Processor;
using Message;

public abstract class DeltaProtocolReceiverBase<TContent, TPartition, TForest> : IDisposable
    where TContent : IDeltaContent
    where TPartition : TContent, IDeltaContent
    where TForest : TContent, IDeltaContent
{
    private readonly PartitionSharedNodeMap _sharedNodeMap;
    private readonly SharedPartitionReplicatorMap _sharedPartitionReplicatorMap;
    private readonly IEventProcessor<IForestEvent> _forestEventReplicator;
    private readonly LocalForestReceiver _localForestReceiver;

    public DeltaProtocolReceiverBase(PartitionSharedNodeMap sharedNodeMap, SharedPartitionReplicatorMap sharedPartitionReplicatorMap, IEventProcessor<IForestEvent> forestEventReplicator)
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedPartitionReplicatorMap = sharedPartitionReplicatorMap;
        _forestEventReplicator = forestEventReplicator;
        _localForestReceiver = new LocalForestReceiver(this, this);
    }

    public void Init()
    {
        foreach (var partition in _sharedNodeMap.Values.OfType<IPartitionInstance>())
        {
            OnLocalPartitionAdded(partition);
        }

        IProcessor.Connect(_forestEventReplicator,  _localForestReceiver);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #region Local

    private class LocalForestReceiver(object? sender, DeltaProtocolReceiverBase<TContent, TPartition, TForest> receiver) : EventProcessorBase<IForestEvent>(sender)
    {
        public override void Receive(IForestEvent forestEvent)
        {
            switch (forestEvent)
            {
                case PartitionAddedEvent e:
                    receiver.OnLocalPartitionAdded(e.NewPartition);
                    break;
                case PartitionDeletedEvent e:
                    receiver.OnLocalPartitionDeleted(e.DeletedPartition);
                    break;
            }
        }
    }

    internal void OnLocalPartitionAdded(IPartitionInstance partition)
    {
    }

    private void OnLocalPartitionDeleted(IPartitionInstance partition)
    {
    }

    #endregion

    #region Remote

    public void Receive(TContent deltaContent)
    {
        switch (deltaContent)
        {
            case TPartition partitionContent:
                var partitionEvent = MapPartition(partitionContent);
                if (_sharedNodeMap.TryGetPartition(partitionEvent.ContextNodeId, out var partition))
                {
                    var eventForwarder = _sharedPartitionReplicatorMap.Lookup(partition.GetId());
                    eventForwarder.Receive(partitionEvent);
                } else
                {
                    throw new InvalidOperationException();
                }

                break;

            case TForest forestContent:
                var forestEvent = MapForest(forestContent);
                _forestEventReplicator.Receive(forestEvent);
                break;

            default:
                throw new InvalidOperationException(deltaContent.ToString());
        }
    }

    protected abstract IPartitionEvent MapPartition(TPartition partitionContent);
    protected abstract IForestEvent MapForest(TForest forestContent);

    #endregion
}