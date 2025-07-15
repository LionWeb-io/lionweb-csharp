// Copyright 2024 TRUMPF Laser GmbH
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
// SPDX-FileCopyrightText: 2024 TRUMPF Laser GmbH
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M3;
using Forest;

public abstract class LionWebRepositoryBase<T> : IDisposable
{
    protected readonly string _name;
    protected readonly IRepositoryConnector<T> _connector;
    protected readonly PartitionSharedNodeMap SharedNodeMap;
    protected readonly ForestEventHandler ForestEventHandler;

    private long nextFreeNodeId = 0;

    protected long _messageCount;
    protected readonly RewriteForestEventReplicator _replicator;
    public long MessageCount => Interlocked.Read(ref _messageCount);

    public LionWebRepositoryBase(LionWebVersions lionWebVersion, List<Language> languages, string name,
        IForest forest, IRepositoryConnector<T> connector)
    {
        _name = name;
        _connector = connector;

        SharedNodeMap = new();
        ForestEventHandler = new ForestEventHandler(name);
        _replicator = new RewriteForestEventReplicator(forest, SharedNodeMap);
        _replicator.ReplicateFrom(ForestEventHandler);

        _replicator.Subscribe<IForestEvent>(SendEventToAllClients);

        _connector.Receive += OnReceive;
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _connector.Receive -= OnReceive;
        _replicator.Dispose();
        SharedNodeMap.Dispose();
    }

    private void OnReceive(object? _, IMessageContext<T> content) =>
        Receive(content);

    private void SendEventToAllClients(object? sender, IEvent? internalEvent)
    {
        if (internalEvent == null)
            return;

        var converted = _connector.Convert(internalEvent);
        SendAll(converted);
    }

    protected IEnumerable<FreeId> GetFreeNodeIds(int count)
    {
        int returnedCount = 0;
        while (returnedCount < count)
        {
            NodeId nextId = "repoProvidedId-" + ++nextFreeNodeId;
            if (!SharedNodeMap.ContainsKey(nextId))
            {
                returnedCount++;
                yield return nextId;
            }
        }
    }

    protected abstract Task Send(IClientInfo clientInfo, T deltaContent);
    protected abstract Task SendAll(T deltaContent);

    protected abstract Task Receive(IMessageContext<T> content);
}