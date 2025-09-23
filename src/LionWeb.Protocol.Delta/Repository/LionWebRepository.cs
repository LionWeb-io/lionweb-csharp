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

    public LionWebRepository(
        LionWebVersions lionWebVersion,
        List<Language> languages,
        string name,
        IForest forest,
        IRepositoryConnector<IDeltaContent> connector
    ) : base(lionWebVersion, languages, name, forest, connector)
    {
        DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
                .WithHandler(new ReceiverDeserializerHandler())
            ;

        SharedKeyedMap sharedKeyedMap = SharedKeyedMapBuilder.BuildSharedKeyMap(languages);

        _commandReceiver = new DeltaProtocolCommandReceiver(
            SharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder
        );

        _commandReceiver.ConnectTo(_replicator);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _commandReceiver.Dispose();
        base.Dispose();
    }

    #region Remote

    /// <inheritdoc />
    protected override async Task Receive(IMessageContext<IDeltaContent> messageContext)
    {
        try
        {
            var content = messageContext.Content;
            content.InternalParticipationId = messageContext.ClientInfo.ParticipationId;
            Log(
                $"received {content.GetType().Name} for {messageContext.ClientInfo.ParticipationId}", true);

            switch (content)
            {
                case IDeltaCommand command:
                    Log($"received command: {command.GetType()}({command.CommandId})");
                    _commandReceiver.Receive(command);
                    break;

                case IDeltaQueryRequest request:
                    Log($"received query: {request.GetType()}({request.QueryId}) for for {messageContext.ClientInfo}");
                    var response = HandleRequest(request, messageContext.ClientInfo);
                    await Send(messageContext.ClientInfo, response);
                    break;

                default:
                    Log(
                        $"ignoring received: {content.GetType()}({content.InternalParticipationId})");
                    break;
            }
        } catch (Exception e)
        {
            Log(e.ToString());
            OnCommunicationError(e);
        }
    }

    private IDeltaQueryResponse HandleRequest(IDeltaQueryRequest request, IClientInfo clientInfo)
    {
        switch (request)
        {
            case GetAvailableIdsRequest idsRequest:
                return GetAvailableIds(idsRequest);

            case ListPartitionsRequest listPartitionsRequest:
                return ListPartitions(listPartitionsRequest);

            case SignOnRequest signOnRequest:
                return SignOn(clientInfo, signOnRequest);

            case SignOffRequest signOffRequest:
                return SignOff(signOffRequest);

            case ReconnectRequest reconnectRequest:
                return Reconnect(reconnectRequest);

            case SubscribeToChangingPartitionsRequest subscribeToChangingPartitionsRequest:
                return SubscribeToChangingPartitions(subscribeToChangingPartitionsRequest);

            case SubscribeToPartitionContentsRequest subscribeToPartitionContentsRequest:
                return SubscribeToPartitionContents(subscribeToPartitionContentsRequest);

            case UnsubscribeFromPartitionContentsRequest unsubscribeFromPartitionContentsRequest:
                return UnsubscribeFromPartitionContents(unsubscribeFromPartitionContentsRequest);

            default:
                Log(
                    $"ignoring received query: {request.GetType()}({request.InternalParticipationId})");
                return null;
        }
    }

    private IDeltaQueryResponse GetAvailableIds(GetAvailableIdsRequest idsRequest) =>
        new GetAvailableIdsResponse(GetFreeNodeIds(idsRequest.count).ToArray(), idsRequest.QueryId, null);

    private IDeltaQueryResponse ListPartitions(ListPartitionsRequest listPartitionsRequest)
    {
        var serializer = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build();

        var chunk = new DeltaSerializationChunk(serializer.Serialize(_forest.Partitions).ToArray());
        return new ListPartitionsResponse(chunk, listPartitionsRequest.QueryId, null);
    }

    private IDeltaQueryResponse SignOn(IClientInfo clientInfo, SignOnRequest signOnRequest) =>
        new SignOnResponse(clientInfo.ParticipationId, signOnRequest.QueryId, null);

    private IDeltaQueryResponse SignOff(SignOffRequest signOffRequest) =>
        new SignOffResponse(signOffRequest.QueryId, null);

    private IDeltaQueryResponse Reconnect(ReconnectRequest reconnectRequest) =>
        new ReconnectResponse(-1, reconnectRequest.QueryId, null);

    private IDeltaQueryResponse SubscribeToChangingPartitions(
        SubscribeToChangingPartitionsRequest subscribeToChangingPartitionsRequest) =>
        new SubscribeToChangingPartitionsResponse(subscribeToChangingPartitionsRequest.QueryId, null);

    private IDeltaQueryResponse SubscribeToPartitionContents(
        SubscribeToPartitionContentsRequest subscribeToPartitionContentsRequest) =>
        new SubscribeToPartitionContentsResponse(new DeltaSerializationChunk([]),
            subscribeToPartitionContentsRequest.QueryId, null);

    private IDeltaQueryResponse UnsubscribeFromPartitionContents(
        UnsubscribeFromPartitionContentsRequest unsubscribeFromPartitionContentsRequest) =>
        new UnsubscribeFromPartitionContentsResponse(unsubscribeFromPartitionContentsRequest.QueryId,
            null);

    #endregion

    #region Send

    /// <inheritdoc />
    protected override async Task Send(IClientInfo clientInfo, IDeltaContent deltaContent)
    {
        try
        {
            Log($"sending to {clientInfo}: {deltaContent.GetType().Name}", true);
            await _connector.SendToClient(clientInfo, deltaContent);
        } catch (Exception e)
        {
            OnCommunicationError(e);
        }
    }

    /// <inheritdoc />
    protected override async Task SendAll(IDeltaContent deltaContent)
    {
        try
        {
            switch (deltaContent)
            {
                case IDeltaEvent deltaEvent:
                    var commandSource = deltaEvent is { OriginCommands: { } cmds }
                        ? cmds.First()
                        : null;
                    Log(
                        $"sending event: {deltaEvent.GetType().Name}({commandSource},{deltaEvent.SequenceNumber})",
                        true);
                    break;

                default:
                    Log($"sending: {deltaContent.GetType().Name}", true);
                    break;
            }

            await _connector.SendToAllClients(deltaContent);
        } catch (Exception e)
        {
            Log(e.ToString());
            OnCommunicationError(e);
        }
    }

    #endregion
}

public class ExceptionParticipationIdProvider : IParticipationIdProvider
{
    private int _nextParticipationId = 1000;

    /// <inheritdoc />
    public string ParticipationId =>
        // $"participationId{++_nextParticipationId}";
        throw new NotImplementedException();
}