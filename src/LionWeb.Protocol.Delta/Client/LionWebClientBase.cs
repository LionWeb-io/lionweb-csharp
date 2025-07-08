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
using Core.M1.Event.Partition;
using Core.M3;

public abstract class LionWebClientBase<T>
{
    protected readonly LionWebVersions _lionWebVersion;
    protected readonly string _name;
    protected readonly IClientConnector<T> _connector;
    protected readonly Dictionary<string, IReadableNode> SharedNodeMap;
    protected readonly PartitionEventHandler PartitionEventHandler;

    private ParticipationId? _participationId;

    protected internal ParticipationId ParticipationId
    {
        get => _participationId ?? throw new InvalidOperationException($"{nameof(ParticipationId)} not set");
        set => _participationId = value;
    }

    public LionWebClientBase(LionWebVersions lionWebVersion, List<Language> languages, string name,
        IPartitionInstance partition, IClientConnector<T> connector)
    {
        _lionWebVersion = lionWebVersion;
        _name = name;
        _connector = connector;

        SharedNodeMap = [];
        PartitionEventHandler = new PartitionEventHandler(name);
        var replicator = new PartitionEventReplicator(partition, SharedNodeMap);
        replicator.ReplicateFrom(PartitionEventHandler);

        replicator.Subscribe<IPartitionEvent>(SendPartitionEventToRepository);

        connector.Receive += (_, content) => Receive(content);
    }

    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.SignOnRequest"/>
    public abstract Task SignOn();
    
    /// <inheritdoc cref="LionWeb.Protocol.Delta.Message.Query.SignOffRequest"/>
    public abstract Task SignOff();

    private void SendPartitionEventToRepository(object? sender, IPartitionEvent? partitionEvent)
    {
        if (partitionEvent == null)
            return;

        var converted = _connector.Convert(partitionEvent);

        Send(converted);
    }

    protected abstract Task Send(T deltaContent);

    protected abstract void Receive(T deltaContent);
}