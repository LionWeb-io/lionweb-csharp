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

using Core;
using Core.M1;
using Core.M3;
using Core.Notification;
using Core.Utilities;
using Message;
using Message.Command;
using Message.Event;
using Message.Query;
using System.Collections.Concurrent;
using System.Diagnostics;

public class LionWebClient : LionWebClientBase<IDeltaContent>
{
    private readonly DeltaProtocolEventReceiver _eventReceiver;
    private readonly DeserializerBuilder _deserializerBuilder;

    private readonly ConcurrentDictionary<EventSequenceNumber, IDeltaEvent> _unprocessedEvents = [];
    private readonly ConcurrentDictionary<CommandId, bool> _ownCommands = [];
    private readonly ConcurrentDictionary<QueryId, TaskCompletionSource<IDeltaQueryResponse>> _queryResponses = [];

    #region EventSequenceNumber

    protected EventSequenceNumber _nextEventSequenceNumber = 0;
    private void IncrementEventSequenceNumber() => Interlocked.Increment(ref _nextEventSequenceNumber);
    private EventSequenceNumber EventSequenceNumber => Interlocked.Read(ref _nextEventSequenceNumber);

    #endregion

    public LionWebClient(
        LionWebVersions lionWebVersion,
        List<Language> languages,
        string name,
        IForest forest,
        IClientConnector<IDeltaContent> connector
    ) : base(lionWebVersion, languages, name, forest, connector)
    {
        _deserializerBuilder = new DeserializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithLanguages(languages)
            .WithHandler(new ReceiverDeserializerHandler());

        SharedKeyedMap sharedKeyedMap = SharedKeyedMapBuilder.BuildSharedKeyMap(languages);

        _eventReceiver = new DeltaProtocolEventReceiver(
            SharedNodeMap,
            sharedKeyedMap,
            _deserializerBuilder
        );

        _eventReceiver.ConnectTo(_replicator);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _eventReceiver.Dispose();
        base.Dispose();
    }

    #region Remote

    /// <inheritdoc />
    protected override void Receive(IDeltaContent content)
    {
        try
        {
            Log($"received {content.GetType().Name}", true);

            switch (content)
            {
                case IDeltaEvent deltaEvent:
                    ReceiveEvent(deltaEvent);
                    return;

                case IDeltaQueryResponse response:
                    Log($"received response: {response})");
                    if (_queryResponses.TryRemove(response.QueryId, out var tcs))
                    {
                        tcs.TrySetResult(response);
                        return;
                    }

                    break;
            }

            if (content is IDeltaError e)
                throw new DeltaException(e);

            Log($"ignoring received: {content.GetType()}({content.InternalParticipationId})");
        } catch (Exception e)
        {
            Log(e.ToString());
            OnCommunicationError(e);
        }
    }

    private void ReceiveEvent(IDeltaEvent deltaEvent)
    {
        Log(
            $"received event: {deltaEvent.GetType()}({string.Join(",", deltaEvent.OriginCommands.Select(oc => $"{oc.ParticipationId}:{oc.CommandId}"))},{deltaEvent.SequenceNumber})");

        CommandSource? commandSource = deltaEvent.OriginCommands.FirstOrDefault();

        deltaEvent.InternalParticipationId = commandSource?.ParticipationId;

        if (EventSequenceNumber == deltaEvent.SequenceNumber)
        {
            ProcessEvent(deltaEvent);
        } else
        {
            _unprocessedEvents[deltaEvent.SequenceNumber] = deltaEvent;
        }

        while (_unprocessedEvents.TryRemove(EventSequenceNumber, out var nextEvent))
        {
            ProcessEvent(nextEvent);
        }
    }

    private void ProcessEvent(IDeltaEvent deltaEvent)
    {
        IncrementEventSequenceNumber();

        if (deltaEvent is Error e)
            throw new DeltaException(e);

        var originCommands = deltaEvent.OriginCommands ?? [];
        var commandSource = originCommands.FirstOrDefault();
        if (originCommands.All(cmd =>
                ParticipationId == cmd.ParticipationId &&
                _ownCommands.TryRemove(cmd.CommandId, out _)))
        {
            Log(
                $"ignoring own event: {deltaEvent.GetType()}({commandSource},{deltaEvent.SequenceNumber})");
            return;
        }

        lock (_eventReceiver)
        {
            _eventReceiver.Receive(deltaEvent);
        }
    }

    #endregion

    #region Send

    #region Subscription

    /// <inheritdoc />
    public override async Task<SubscribeToChangingPartitionsResponse> SubscribeToChangingPartitions(bool creation,
        bool deletion, bool partitions) =>
        await Query<SubscribeToChangingPartitionsResponse, SubscribeToChangingPartitionsRequest>(
            new SubscribeToChangingPartitionsRequest(creation, deletion, partitions, QueryId(), null));


    /// <inheritdoc />
    public override async Task<IPartitionInstance> SubscribeToPartitionContents(TargetNode partition)
    {
        var response = await Query<SubscribeToPartitionContentsResponse, SubscribeToPartitionContentsRequest>(
            new SubscribeToPartitionContentsRequest(partition, QueryId(), null));

        var deserializer = _deserializerBuilder.Build();
        deserializer.RegisterDependentNodes(_forest.Descendants(true));
        var partitionInstances = deserializer
            .Deserialize(response.Contents.Nodes)
            .Cast<IPartitionInstance>()
            .ToList();

        Debug.Assert(partitionInstances.Count == 1);
        var result = partitionInstances.First();

        var notificationId = new NotificationIdProvider(this).Create();
        _ownNotifications[notificationId] = true;

        _forest.AddPartitions([result], notificationId);
        return result;
    }

    /// <inheritdoc />
    public override async Task<UnsubscribeFromPartitionContentsResponse> UnsubscribeFromPartitionContents(
        TargetNode partition) =>
        await Query<UnsubscribeFromPartitionContentsResponse, UnsubscribeFromPartitionContentsRequest>(
            new UnsubscribeFromPartitionContentsRequest(partition, QueryId(), null));

    #endregion

    #region Participation

    /// <inheritdoc />
    public override async Task<SignOnResponse> SignOn(RepositoryId repositoryId)
    {
        var signOnResponse =
            await Query<SignOnResponse, SignOnRequest>(new SignOnRequest(_lionWebVersion.VersionString, ClientId,
                IdUtils.NewId(), repositoryId, null));
        ParticipationId = signOnResponse.ParticipationId;
        return signOnResponse;
    }

    /// <inheritdoc />
    public override async Task<SignOffResponse> SignOff()
    {
        var response = await Query<SignOffResponse, SignOffRequest>(new SignOffRequest(QueryId(), null));
        _participationId = null;
        return response;
    }

    /// <inheritdoc />
    public override async Task<ReconnectResponse> Reconnect(ParticipationId participationId)
    {
        if(SignedIn)
            throw new DeltaException(DeltaErrorCode.AlreadySignedOn.AsErrorResponse(null, null));

        var response = await Query<ReconnectResponse, ReconnectRequest>(new ReconnectRequest(participationId, EventSequenceNumber, QueryId(), null));
        ParticipationId = participationId;
        return response;
    }

    #endregion

    #region Miscellaneous

    /// <inheritdoc />
    public override async Task<GetAvailableIdsResponse> GetAvailableIds(int count) =>
        await Query<GetAvailableIdsResponse, GetAvailableIdsRequest>(
            new GetAvailableIdsRequest(count, QueryId(), null));

    /// <inheritdoc />
    public override async Task<List<IPartitionInstance>> ListPartitions()
    {
        var response =
            await Query<ListPartitionsResponse, ListPartitionsRequest>(new ListPartitionsRequest(QueryId(), null));
        Log($"ListPartitions response: {response}");
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithLanguages(_languages)
            .WithHandler(new NoFeaturesDeserializationHandler())
            .Build();
        return deserializer.Deserialize(response.Partitions.Nodes).Cast<IPartitionInstance>().ToList();
    }

    #endregion

    private async Task<TResponse> Query<TResponse, TRequest>(TRequest request)
        where TResponse : class, IDeltaQueryResponse where TRequest : IDeltaQueryRequest
    {
        if (request.RequiresParticipationId && !SignedIn)
            throw new DeltaException(DeltaErrorCode.NotSignedOn.AsErrorResponse(request.QueryId, null));
        
        var tcs = new TaskCompletionSource<IDeltaQueryResponse>();
        _queryResponses[request.QueryId] = tcs;
        await Send(request);
        var response = await tcs.Task;
        if (response is IDeltaError err)
            throw new DeltaException(err);

        return response as TResponse ?? throw new ArgumentException(response.GetType().Name);
    }

    /// <inheritdoc />
    protected override async Task Send(IDeltaContent deltaContent)
    {
        try
        {
            if (deltaContent.RequiresParticipationId)
                deltaContent.InternalParticipationId = ParticipationId;

            if (deltaContent is IDeltaCommand { CommandId: { } commandId })
                _ownCommands.TryAdd(commandId, true);

            Log($"sending: {deltaContent.GetType().Name}", true);
            await _connector.SendToRepository(deltaContent);
        } catch (Exception e)
        {
            Log(e.ToString());
            OnCommunicationError(e);
        }
    }

    #endregion

    private NodeId QueryId() => IdUtils.NewId();
}