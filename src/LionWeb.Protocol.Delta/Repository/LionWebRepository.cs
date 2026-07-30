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
using Core.Notification.Forest;
using Message;
using Message.Command;
using Message.Event;
using Message.Query;

public class LionWebRepository : LionWebRepositoryBase<IDeltaContent>
{
    private readonly DeltaProtocolCommandReceiver _commandReceiver;
    private readonly SerializerBuilder _serializerBuilder;
    private readonly IParticipationIdProvider _participationIdProvider;

    public LionWebRepository(
        LionWebVersions lionWebVersion,
        List<Language> languages,
        string name,
        IForest forest,
        IRepositoryConnector<IDeltaContent> connector
    ) : base(lionWebVersion, languages, name, forest, connector)
    {
        var unresolvedReferencesManager = new LoggingUnresolvedReferencesManager(s => Log(s));

        DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
                .WithLanguageReferences()
                .WithHandler(new DeltaDeserializerHandler(unresolvedReferencesManager))
            ;

        forest.GetNotificationSender()?.ConnectTo(unresolvedReferencesManager);

        SharedKeyedMap sharedKeyedMap = SharedKeyedMapBuilder.BuildSharedKeyMap(languages);

        _commandReceiver = new DeltaProtocolCommandReceiver(
            SharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder);

        _commandReceiver.ConnectTo(_replicator);

        _serializerBuilder = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion);

        _participationIdProvider = new ParticipationIdProvider();
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

                    if (!messageContext.ClientInfo.SignedOn)
                    {
                        await Send(messageContext.ClientInfo,
                            DeltaErrorCode.NotSignedOn.AsErrorEvent(
                                [new CommandSource(messageContext.ClientInfo.ParticipationId, command.CommandId)],
                                null));
                        return;
                    }

                    var notification = _commandReceiver.Map(command);
                    if (notification is PartitionAddedNotification partitionAdded)
                    {
                        messageContext.ClientInfo.SubscribedPartitions.Add(partitionAdded.NewPartition.GetId());
                    }

                    _commandReceiver.ProduceNotification(notification);

                    if (notification is PartitionDeletedNotification partitionDeleted)
                    {
                        messageContext.ClientInfo.SubscribedPartitions.Remove(partitionDeleted.DeletedPartition
                            .GetId());
                    }

                    break;

                case IDeltaQueryRequest request:
                    Log($"received query: {request.GetType()}({request.QueryId}) for for {messageContext.ClientInfo}");
                    var response = HandleQueryRequest(request, messageContext.ClientInfo);
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

    private IDeltaQueryResponse HandleQueryRequest(IDeltaQueryRequest request, IClientInfo clientInfo)
    {
        switch (request)
        {
            case GetAvailableIdsRequest idsRequest:
                return GetAvailableIds(idsRequest);

            case ListPartitionsRequest listPartitionsRequest:
                return ListPartitions(listPartitionsRequest);

            case ListAndSubscribePartitionsRequest listAndSubscribePartitionsRequest:
                return ListAndSubscribePartitions(listAndSubscribePartitionsRequest, clientInfo);

            case SignOnRequest signOnRequest:
                return SignOn(signOnRequest, clientInfo);

            case SignOffRequest signOffRequest:
                return SignOff(signOffRequest, clientInfo);

            case ReconnectRequest reconnectRequest:
                return Reconnect(reconnectRequest, clientInfo);

            case SubscribeToChangingPartitionsRequest subscribeToChangingPartitionsRequest:
                return SubscribeToChangingPartitions(subscribeToChangingPartitionsRequest, clientInfo);

            case InformAboutChangingPartitionsRequest informAboutChangingPartitionsRequest:
                return InformAboutChangingPartitions(informAboutChangingPartitionsRequest, clientInfo);

            case SubscribeToPartitionContentsRequest subscribeToPartitionContentsRequest:
                return SubscribeToPartitionContents(subscribeToPartitionContentsRequest, clientInfo);

            case UnsubscribeFromPartitionContentsRequest unsubscribeFromPartitionContentsRequest:
                return UnsubscribeFromPartitionContents(unsubscribeFromPartitionContentsRequest, clientInfo);

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
        DeltaSerializationChunk chunk = Serialize(_forest
            .Partitions
            .SelectMany(p =>
                p.Descendants(n => M1Extensions.Ancestors(n).Count() <= listPartitionsRequest.DepthLimit, true, true)
            )
        );
        return new ListPartitionsResponse(chunk, false, listPartitionsRequest.QueryId, null);
    }

    private IDeltaQueryResponse ListAndSubscribePartitions(ListAndSubscribePartitionsRequest listAndSubscribePartitionsRequest, IClientInfo clientInfo)
    {
        DeltaSerializationChunk chunk = Serialize(_forest.Partitions);
        clientInfo.SubscribedPartitions.UnionWith(_forest.Partitions.Select(p => p.GetId()));
        return new ListAndSubscribePartitionsResponse(chunk, false, listAndSubscribePartitionsRequest.QueryId, null);
    }

    private IDeltaQueryResponse SignOn(SignOnRequest signOnRequest, IClientInfo clientInfo)
    {
        if (clientInfo.SignedOn)
            return DeltaErrorCode.AlreadySignedOn.AsErrorResponse(signOnRequest.QueryId, null);

        clientInfo.SignedOn = true;
        clientInfo.ClientId = signOnRequest.ClientId;
        clientInfo.ParticipationId = _participationIdProvider.Create();
        return new SignOnResponse(clientInfo.ParticipationId, signOnRequest.QueryId, null);
    }

    private IDeltaQueryResponse SignOff(SignOffRequest signOffRequest, IClientInfo clientInfo)
    {
        if (!clientInfo.SignedOn)
            return DeltaErrorCode.NotSignedOn.AsErrorResponse(signOffRequest.QueryId, null);

        clientInfo.SignedOn = false;
        return new SignOffResponse(signOffRequest.QueryId, null);
    }

    private IDeltaQueryResponse Reconnect(ReconnectRequest reconnectRequest, IClientInfo clientInfo)
    {
        if (clientInfo.SignedOn)
            return DeltaErrorCode.AlreadySignedOn.AsErrorResponse(reconnectRequest.QueryId, null);

        if (clientInfo.SequenceNumber != reconnectRequest.LastReceivedSequenceNumber)
            return DeltaErrorCode.NotCurrentSequenceNumber.AsErrorResponse(reconnectRequest.QueryId, null,
                clientInfo.SequenceNumber, reconnectRequest.LastReceivedSequenceNumber);

        var response = new ReconnectResponse(clientInfo.SequenceNumber, reconnectRequest.QueryId, null);
        clientInfo.SignedOn = true;
        return response;
    }

    private IDeltaQueryResponse SubscribeToChangingPartitions(
        SubscribeToChangingPartitionsRequest subscribeToChangingPartitionsRequest, IClientInfo clientInfo)
    {
        if (clientInfo.PartitionChanges == PartitionChanges.InformAboutChangingPartitions)
            return DeltaErrorCode.MixedSubscribeInformPartitionChanges.AsErrorResponse(subscribeToChangingPartitionsRequest.QueryId, null, clientInfo.PartitionChanges,
                PartitionChanges.SubscribeToChangingPartitions); 
        
        var creation = subscribeToChangingPartitionsRequest.Creation;
        var deletion = subscribeToChangingPartitionsRequest.Deletion;
        
        clientInfo.PartitionChanges = creation || deletion ? PartitionChanges.SubscribeToChangingPartitions : PartitionChanges.None;

        clientInfo.NotifyAboutPartitionCreation = creation;
        clientInfo.NotifyAboutPartitionDeletion = deletion;

        return new SubscribeToChangingPartitionsResponse(subscribeToChangingPartitionsRequest.QueryId, null);
    }

    private IDeltaQueryResponse InformAboutChangingPartitions(
        InformAboutChangingPartitionsRequest informAboutChangingPartitionsRequest, IClientInfo clientInfo)
    {
        if (clientInfo.PartitionChanges == PartitionChanges.SubscribeToChangingPartitions)
            return DeltaErrorCode.MixedSubscribeInformPartitionChanges.AsErrorResponse(informAboutChangingPartitionsRequest.QueryId, null, clientInfo.PartitionChanges,
                PartitionChanges.InformAboutChangingPartitions); 
        
        var creation = informAboutChangingPartitionsRequest.Creation;
        var deletion = informAboutChangingPartitionsRequest.Deletion;
        
        clientInfo.PartitionChanges = creation || deletion ? PartitionChanges.InformAboutChangingPartitions : PartitionChanges.None;

        clientInfo.NotifyAboutPartitionCreation = creation;
        clientInfo.NotifyAboutPartitionDeletion = deletion;

        return new InformAboutChangingPartitionsResponse(informAboutChangingPartitionsRequest.QueryId, null);
    }

    private IDeltaQueryResponse SubscribeToPartitionContents(
        SubscribeToPartitionContentsRequest subscribeToPartitionContentsRequest, IClientInfo clientInfo)
    {
        if (clientInfo.SubscribedPartitions.Add(subscribeToPartitionContentsRequest.Partition) &&
            SharedNodeMap.TryGetPartition(subscribeToPartitionContentsRequest.Partition, out var partition))
        {
            return new SubscribeToPartitionContentsResponse(
                Serialize(M1Extensions.Descendants<IReadableNode>(partition, true, true)),
                subscribeToPartitionContentsRequest.QueryId, null);
        }

        return DeltaErrorCode.UnknownPartition.AsErrorResponse(subscribeToPartitionContentsRequest.QueryId, null,
            subscribeToPartitionContentsRequest.Partition);
    }


    private IDeltaQueryResponse UnsubscribeFromPartitionContents(
        UnsubscribeFromPartitionContentsRequest unsubscribeFromPartitionContentsRequest, IClientInfo clientInfo)
    {
        if (clientInfo.SubscribedPartitions.Remove(unsubscribeFromPartitionContentsRequest.Partition))
            return new UnsubscribeFromPartitionContentsResponse(unsubscribeFromPartitionContentsRequest.QueryId,
                null);

        return DeltaErrorCode.NotSubscribed.AsErrorResponse(unsubscribeFromPartitionContentsRequest.QueryId, null, unsubscribeFromPartitionContentsRequest.Partition);
    }

    #endregion

    #region Send

    /// <inheritdoc />
    protected override async Task Send(IClientInfo clientInfo, IDeltaContent deltaContent)
    {
        try
        {
            Log($"sending to {clientInfo}: {deltaContent.GetType().Name}", true);
            await _connector.SendToClient(deltaContent, clientInfo);
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
            HashSet<NodeId> affectedPartitions = [];
            Dictionary<NodeId, DepthLimit>? depths = deltaContent is PartitionAdded ? [] : null;

            switch (deltaContent)
            {
                case IDeltaEvent deltaEvent:
                    var commandSource = deltaEvent is { OriginCommands: { } cmds }
                        ? cmds.First()
                        : null;

                    foreach (var affectedNode in deltaEvent.AffectedNodes)
                    {
                        if (SharedNodeMap.TryGetPartition(affectedNode, out var partition))
                        {
                            affectedPartitions.Add(partition.GetId());
                            AddDepths(partition, depths);
                        }
                    }

                    if (deltaEvent is PartitionDeleted d)
                        affectedPartitions.Add(d.DeletedPartition);

                    Log(
                        $"sending event: {deltaEvent.GetType().Name}({commandSource},{deltaEvent.SequenceNumber})",
                        true);
                    break;

                default:
                    Log($"sending: {deltaContent.GetType().Name}", true);
                    break;
            }

            await _connector.SendToAllClients(deltaContent, affectedPartitions);
        } catch (Exception e)
        {
            Log(e.ToString());
            OnCommunicationError(e);
        }
    }

    private void AddDepths(IReadableNode node, DepthLimit currentDepth, Dictionary<NodeId, DepthLimit>? depths)
    {
        
        throw new NotImplementedException();
    }

    #endregion

    private DeltaSerializationChunk Serialize(IEnumerable<IReadableNode> nodes)
    {
        var serializer = _serializerBuilder.Build();
        var chunk = new DeltaSerializationChunk(serializer.Serialize(nodes).ToArray());
        return chunk;
    }
}