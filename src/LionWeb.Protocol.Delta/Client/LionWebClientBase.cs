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
using Core.M3;

public interface ILionWebClient
{
    private const string _magenta = "\x1b[95m";
    private const string _bold = "\x1b[1m";
    private const string _unbold = "\x1b[22m";
    private const string _defaultColor = "\x1b[39m";
    
    public const string HeaderColor_Start = _magenta + _bold;
    public const string HeaderColor_End =  _unbold + _defaultColor;
}

public abstract class LionWebClientBase<T> : ILionWebClient
{
    protected readonly LionWebVersions _lionWebVersion;
    protected readonly string _name;
    protected readonly IClientConnector<T> _connector;
    protected readonly SharedNodeMap SharedNodeMap;
    protected readonly PartitionEventHandler PartitionEventHandler;

    private ParticipationId? _participationId;
    private readonly ClientId? _clientId;

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
        IPartitionInstance partition, IClientConnector<T> connector)
    {
        _lionWebVersion = lionWebVersion;
        _name = name;
        _connector = connector;

        SharedNodeMap = new();
        PartitionEventHandler = new PartitionEventHandler(name);
        var replicator = new PartitionEventReplicator(partition, SharedNodeMap);
        replicator.ReplicateFrom(PartitionEventHandler);

        replicator.Subscribe<IPartitionNotification>(SendPartitionEventToRepository);

        connector.ReceiveFromRepository += (_, content) => Receive(content);
    }

    private void OnReceive(object? _, T content) =>
        Receive(content);

    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.SignOnRequest"/>
    /// <returns><see cref="LionWeb.Protocol.Delta.Message.Query.SignOnResponse"/></returns>
    public abstract Task SignOn();

    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.SignOffRequest"/>
    /// <returns><see cref="LionWeb.Protocol.Delta.Message.Query.SignOffResponse"/></returns>
    public abstract Task SignOff();

    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.GetAvailableIdsRequest"/>
    /// <returns><see cref="LionWeb.Protocol.Delta.Message.Query.GetAvailableIdsResponse"/></returns>
    public abstract Task GetAvailableIds(int count);
    
    private void SendPartitionEventToRepository(object? sender, IPartitionNotification? partitionEvent)
    {
        if (partitionEvent == null)
            return;

        var converted = _connector.Convert(partitionEvent);

        Send(converted);
    }

    protected virtual void Log(string message, bool header = false)
    {
        var prependedMessage = $"{_name}: {message}";
        Console.WriteLine(header
            ? $"{ILionWebClient.HeaderColor_Start}{prependedMessage}{ILionWebClient.HeaderColor_End}"
            : prependedMessage);
    }

    protected abstract Task Send(T deltaContent);

    protected abstract void Receive(T deltaContent);
}