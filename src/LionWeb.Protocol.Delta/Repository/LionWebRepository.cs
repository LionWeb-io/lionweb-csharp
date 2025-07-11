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
using Core.M1;
using Core.M3;
using Message;
using Message.Command;
using Message.Event;
using Message.Query;
using System.Diagnostics;

public class LionWebRepository : LionWebRepositoryBase<IDeltaContent>
{
    private readonly DeltaProtocolPartitionCommandReceiver _commandReceiver;

    public LionWebRepository(LionWebVersions lionWebVersion,
        List<Language> languages,
        string name,
        IPartitionInstance partition,
        IRepositoryConnector<IDeltaContent> connector) : base(lionWebVersion, languages, name, partition, connector)
    {
        DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
                .WithHandler(new ReceiverDeserializerHandler())
            ;

        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap = DeltaUtils.BuildSharedKeyMap(languages);

        _commandReceiver = new DeltaProtocolPartitionCommandReceiver(
            PartitionEventHandler,
            SharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder
        );
    }

    /// <inheritdoc />
    protected override async Task SendAll(IDeltaContent deltaContent)
    {
        switch (deltaContent)
        {
            case IDeltaEvent deltaEvent:
                var commandSource = deltaEvent is { OriginCommands: { } cmds }
                    ? cmds.First()
                    : null;
                Debug.WriteLine(
                    $"{_name}: sending event: {deltaEvent.GetType()}({commandSource},{deltaEvent.SequenceNumber})");
                break;

            default:
                Debug.WriteLine($"{_name}: sending: {deltaContent.GetType()}");
                break;
        }

        await _connector.SendAll(deltaContent);
    }

    /// <inheritdoc />
    protected override async Task Send(IClientInfo clientInfo, IDeltaContent deltaContent)
    {
        Debug.WriteLine($"{_name}: sending to {clientInfo}: {deltaContent.GetType()}");
        await _connector.Send(clientInfo, deltaContent);
    }

    /// <inheritdoc />
    protected override async Task Receive(IMessageContext<IDeltaContent> messageContext)
    {
        try
        {
            var content = messageContext.Content;
            content.InternalParticipationId = messageContext.ClientInfo.ParticipationId;
            Debug.WriteLine(
                $"{_name}: received {content.GetType().Name} for {messageContext.ClientInfo.ParticipationId}: {content})");
            Interlocked.Increment(ref _messageCount);

            switch (content)
            {
                case IDeltaCommand command:
                    Debug.WriteLine($"{_name}: received command: {command.GetType()}({command.CommandId})");
                    _commandReceiver.Receive(command);
                    break;

                case SignOnRequest signOnRequest:
                    Debug.WriteLine(
                        $"{_name}: received {nameof(SignOnRequest)} for {messageContext.ClientInfo}: {signOnRequest})");
                    await Send(messageContext.ClientInfo,
                        new SignOnResponse(messageContext.ClientInfo.ParticipationId, signOnRequest.QueryId, null));
                    break;

                default:
                    Debug.WriteLine(
                        $"{_name}: ignoring received: {content.GetType()}({content.InternalParticipationId})");
                    break;
            }
        } catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}

public class LionWebTestRepository(
    LionWebVersions lionWebVersion,
    List<Language> languages,
    string name,
    IPartitionInstance partition,
    IRepositoryConnector<IDeltaContent> connector)
    : LionWebRepository(lionWebVersion, languages, name, partition, connector)
{
    public int WaitCount { get; private set; }

    private const int SleepInterval = 100;

    private void WaitForCount(int count)
    {
        while (MessageCount < count)
        {
            Debug.WriteLine($"{_name}: {nameof(MessageCount)}: {MessageCount} vs. {nameof(count)}: {count}");
            Thread.Sleep(SleepInterval);
        }
    }

    public void WaitForReceived(int delta) =>
        WaitForCount(WaitCount += delta);
}

public class ExceptionParticipationIdProvider : IParticipationIdProvider
{
    /// <inheritdoc />
    public string ParticipationId => throw new NotImplementedException();
}