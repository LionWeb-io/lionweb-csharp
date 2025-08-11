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

namespace LionWeb.Protocol.Delta.Client;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Core.M1.Event.Processor;
using Core.M3;

public abstract class LionWebClientBase<T> : ILionWebClient, IDisposable
{
    private readonly string _name;
    protected readonly LionWebVersions _lionWebVersion;
    protected readonly IClientConnector<T> _connector;
    protected readonly PartitionSharedNodeMap SharedNodeMap;
    protected readonly IEventProcessor<IForestEvent> _replicator;

    private ParticipationId? _participationId;
    private readonly ClientId? _clientId;
    protected readonly SharedPartitionReplicatorMap SharedPartitionReplicatorMap;

    protected internal ParticipationId ParticipationId
    {
        get => _participationId ?? throw new InvalidOperationException($"{nameof(ParticipationId)} not set");
        set => _participationId = value;
    }

    public ClientId ClientId
    {
        get => _clientId ?? _name;
        init => _clientId = value;
    }

    public LionWebClientBase(LionWebVersions lionWebVersion, List<Language> languages, string name,
        IForest forest, IClientConnector<T> connector)
    {
        _lionWebVersion = lionWebVersion;
        _name = name;
        _connector = connector;

        SharedNodeMap = new();
        SharedPartitionReplicatorMap = new SharedPartitionReplicatorMap();
        _replicator = ForestEventReplicator.Create(forest, SharedPartitionReplicatorMap, SharedNodeMap, _name);
        
        if(forest.GetProcessor() is { } proc)
            IProcessor.Connect(proc, new LocalForestChangeReceiver(name, this));
        IProcessor.Connect(_replicator, new LocalForestReceiver(name, this));

        _connector.ReceiveFromRepository += OnReceiveFromRepository;
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _connector.ReceiveFromRepository -= OnReceiveFromRepository;
        // _replicator.Dispose();
        SharedNodeMap.Dispose();
    }

    #region Local
    
    private class LocalForestChangeReceiver(object? sender, LionWebClientBase<T> client) : EventProcessorBase<IForestEvent>(sender)
    {
        public override void Receive(IForestEvent message)
        {
            switch (message)
            {
                case PartitionAddedEvent partitionAddedEvent:
                    client.OnLocalPartitionAdded(partitionAddedEvent);
                    break;
                case PartitionDeletedEvent partitionDeletedEvent:
                    client.OnLocalPartitionDeleted(partitionDeletedEvent);
                    break;
            }
        }
    }
    
    private class LocalForestReceiver(object? sender, LionWebClientBase<T> client) : EventProcessorBase<IForestEvent>(sender)
    {
        public override void Receive(IForestEvent message) => 
            client.SendEventToRepository(sender, message);
    }
    
    private class LocalPartitionReceiver(object? sender, LionWebClientBase<T> client) : EventProcessorBase<IPartitionEvent>(sender)
    {
        public override void Receive(IPartitionEvent message) =>
            client.SendEventToRepository(sender, message);
    }

    private void OnLocalPartitionAdded(PartitionAddedEvent partitionAddedEvent)
    {
        var partitionReplicator = SharedPartitionReplicatorMap.Lookup(partitionAddedEvent.NewPartition.GetId());
        IProcessor.Connect(partitionReplicator, new LocalPartitionReceiver(_name, this));
    }

    private void OnLocalPartitionDeleted(PartitionDeletedEvent partitionDeletedEvent)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Remote

    protected abstract void Receive(T deltaContent);

    private void OnReceiveFromRepository(object? _, T content) =>
        Receive(content);

    #endregion

    #region Send

    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.SignOnRequest"/>
    /// <returns><see cref="LionWeb.Protocol.Delta.Message.Query.SignOnResponse"/></returns>
    public abstract Task SignOn();

    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.SignOffRequest"/>
    /// <returns><see cref="LionWeb.Protocol.Delta.Message.Query.SignOffResponse"/></returns>
    public abstract Task SignOff();

    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.GetAvailableIdsRequest"/>
    /// <returns><see cref="LionWeb.Protocol.Delta.Message.Query.GetAvailableIdsResponse"/></returns>
    public abstract Task GetAvailableIds(int count);

    protected abstract Task Send(T deltaContent);

    private void SendEventToRepository(object? sender, IEvent? internalEvent)
    {
        if (internalEvent == null)
            return;

        var converted = _connector.Convert(internalEvent);

        Send(converted);
        Log("Sent to repository");
    }

    #endregion

    protected virtual void Log(string message, bool header = false)
    {
        var prependedMessage = $"{_name}: {message}";
        Console.WriteLine(header
            ? $"{ILionWebClient.HeaderColor_Start}{prependedMessage}{ILionWebClient.HeaderColor_End}"
            : prependedMessage);
    }
}