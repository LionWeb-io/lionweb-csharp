// Copyright 2025 LionWeb Project
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

namespace LionWeb.Protocol.Delta.Client;

using Command;
using Core;
using Core.M1;
using Core.M1.Event;
using Core.M3;
using Event;
using Query;
using System.Diagnostics;

public interface IDeltaClientConnector : IClientConnector<IDeltaContent>;

public interface IEventClientConnector : IClientConnector<IEvent> 
{
    IEvent IClientConnector<IEvent>.Convert(IEvent internalEvent) => internalEvent;
}

public class LionWebClient : LionWebClientBase<IDeltaContent>
{
    protected DeltaProtocolPartitionEventReceiver _eventReceiver;

    public LionWebClient(LionWebVersions lionWebVersion,
        List<Language> languages,
        string name,
        IPartitionInstance partition,
        IClientConnector<IDeltaContent> connector) : base(lionWebVersion, languages, name, partition, connector)
    {
        DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
                .WithHandler(new ReceiverDeserializerHandler())
            ;
        Dictionary<CompressedMetaPointer, IKeyed>
            sharedKeyedMap = DeltaCommandToDeltaEventMapper.BuildSharedKeyMap(languages);
        _eventReceiver = new DeltaProtocolPartitionEventReceiver(
            PartitionEventHandler,
            SharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder
        );
    }

    public override async Task Send(IDeltaContent deltaContent)
    {
        if (deltaContent.RequiresParticipationId)
            deltaContent.InternalParticipationId = _participationId;

        if (deltaContent is IDeltaCommand { CommandId: { } commandId })
            _ownCommands.TryAdd(commandId, 1);

        Debug.WriteLine($"{_name}: sending: {deltaContent.GetType()}");
        await _connector.Send(deltaContent);
    }

    public override void Receive(IDeltaContent content)
    {
        try
        {
            Interlocked.Increment(ref _messageCount);
            switch (content)
            {
                case IDeltaEvent deltaEvent:
                    CommandSource? commandSource = null;
                    if (deltaEvent is IDeltaEvent singleDeltaEvent)
                    {
                        commandSource = singleDeltaEvent.OriginCommands.FirstOrDefault();
                        if (singleDeltaEvent.OriginCommands.All(cmd =>
                                _participationId == cmd.ParticipationId &&
                                _ownCommands.TryRemove(cmd.CommandId, out _)))
                        {
                            Debug.WriteLine(
                                $"{_name}: ignoring own event: {deltaEvent.GetType()}({commandSource},{deltaEvent.SequenceNumber})");
                            return;
                        }
                    }

                    Debug.WriteLine(
                        $"{_name}: received event: {deltaEvent.GetType()}({commandSource},{deltaEvent.SequenceNumber})");
                    deltaEvent.InternalParticipationId = commandSource?.ParticipationId;
                    _eventReceiver.Receive(deltaEvent);
                    break;

                case SignOnResponse signOnResponse:
                    Debug.WriteLine($"{_name}: received {nameof(SignOnResponse)}: {signOnResponse})");
                    _participationId = signOnResponse.ParticipationId;
                    break;

                default:
                    Debug.WriteLine(
                        $"{_name}: ignoring received: {content.GetType()}({content.InternalParticipationId})");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}

public class LionWebTestClient(
    LionWebVersions lionWebVersion,
    List<Language> languages,
    string name,
    IPartitionInstance partition,
    IDeltaClientConnector connector)
    : LionWebClient(lionWebVersion, languages, name, partition, connector)
{
    private const int SleepInterval = 100;

    private void WaitForCount(long count)
    {
        while (MessageCount < count)
        {
            Thread.Sleep(SleepInterval);
        }
    }

    public void WaitForReplies(int delta) =>
        WaitForCount(MessageCount + delta);
}

public class CommandIdProvider : ICommandIdProvider
{
    private int nextId = 0;
    public string Create() => (++nextId).ToString();
}