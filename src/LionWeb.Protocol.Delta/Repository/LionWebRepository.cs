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

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1;
using Core.M3;
using Message;
using Message.Command;
using Message.Event;
using Message.Query;

public class LionWebRepository : LionWebRepositoryBase<IDeltaContent>
{
    private readonly DeltaProtocolCommandReceiver _commandReceiver;

    public LionWebRepository(LionWebVersions lionWebVersion,
        List<Language> languages,
        string name,
        IForest forest,
        IRepositoryConnector<IDeltaContent> connector) : base(lionWebVersion, languages, name, forest, connector)
    {
        DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
                .WithHandler(new ReceiverDeserializerHandler())
            ;

        SharedKeyedMap sharedKeyedMap = DeltaUtils.BuildSharedKeyMap(languages);

        _commandReceiver = new DeltaProtocolCommandReceiver(
            ForestEventHandler,
            SharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder,
            _replicator
        );
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _commandReceiver.Dispose();
        base.Dispose();
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
                Log(
                    $"sending event: {deltaEvent.GetType().Name}({commandSource},{deltaEvent.SequenceNumber})", true);
                break;

            default:
                Log($"sending: {deltaContent.GetType().Name}", true);
                break;
        }

        await _connector.SendToAllClients(deltaContent);
    }

    /// <inheritdoc />
    protected override async Task Send(IClientInfo clientInfo, IDeltaContent deltaContent)
    {
        Log($"sending to {clientInfo}: {deltaContent.GetType().Name}", true);
        await _connector.SendToClient(clientInfo, deltaContent);
    }

    /// <inheritdoc />
    protected override async Task Receive(IMessageContext<IDeltaContent> messageContext)
    {
        try
        {
            var content = messageContext.Content;
            content.InternalParticipationId = messageContext.ClientInfo.ParticipationId;
            Log(
                $"received {content.GetType().Name} for {messageContext.ClientInfo.ParticipationId}", true);
            Log(content.ToString());
            Interlocked.Increment(ref _messageCount);

            switch (content)
            {
                case IDeltaCommand command:
                    Log($"received command: {command.GetType()}({command.CommandId})");
                    _commandReceiver.Receive(command);
                    break;

                case GetAvailableIdsRequest idsRequest:
                    Log(
                        $"received {nameof(GetAvailableIdsRequest)} for {messageContext.ClientInfo}: {idsRequest})");
                    await Send(messageContext.ClientInfo,
                        new GetAvailableIdsResponse(GetFreeNodeIds(idsRequest.count).ToArray(), idsRequest.QueryId,
                            null));
                    break;

                case SignOnRequest signOnRequest:
                    Log(
                        $"received {nameof(SignOnRequest)} for {messageContext.ClientInfo}: {signOnRequest})");
                    await Send(messageContext.ClientInfo,
                        new SignOnResponse(messageContext.ClientInfo.ParticipationId, signOnRequest.QueryId, null));
                    break;

                default:
                    Log(
                        $"ignoring received: {content.GetType()}({content.InternalParticipationId})");
                    break;
            }
        } catch (Exception e)
        {
            Log(e.ToString());
        }
    }
}

public class LionWebTestRepository(
    LionWebVersions lionWebVersion,
    List<Language> languages,
    string name,
    IForest forest,
    IRepositoryConnector<IDeltaContent> connector)
    : LionWebRepository(lionWebVersion, languages, name, forest, connector)
{
    public int WaitCount { get; private set; }

    private const int SleepInterval = 100;

    private void WaitForCount(int count)
    {
        while (MessageCount < count)
        {
            Log($"{nameof(MessageCount)}: {MessageCount} vs. {nameof(count)}: {count}");
            Thread.Sleep(SleepInterval);
        }
    }

    public void WaitForReceived(int delta) =>
        WaitForCount(WaitCount += delta);
}

public class ExceptionParticipationIdProvider : IParticipationIdProvider
{
    private int _nextParticipationId = 1000;
    
    /// <inheritdoc />
    public string ParticipationId =>
        $"participationId{++_nextParticipationId}";
        // throw new NotImplementedException();
}