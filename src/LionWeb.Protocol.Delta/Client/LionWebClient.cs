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
using Core.Notification.Handler;
using Core.Utilities;
using Message;
using Message.Command;
using Message.Event;
using Message.Query;
using System.Collections.Concurrent;

public class LionWebClient : LionWebClientBase<IDeltaContent>
{
    private readonly DeltaProtocolEventReceiver _eventReceiver;

    private readonly ConcurrentDictionary<EventSequenceNumber, IDeltaEvent> _unprocessedEvents = [];
    private readonly ConcurrentDictionary<CommandId, bool> _ownCommands = [];
    private readonly ConcurrentDictionary<QueryId, TaskCompletionSource<IDeltaQueryResponse>> _queryResponses = [];

    #region EventSequenceNumber

    private EventSequenceNumber _nextEventSequenceNumber = 0;
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
        DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
                .WithHandler(new ReceiverDeserializerHandler())
            ;

        SharedKeyedMap sharedKeyedMap = SharedKeyedMapBuilder.BuildSharedKeyMap(languages);

        _eventReceiver = new DeltaProtocolEventReceiver(
            SharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder
        );
        
        INotificationHandler.Connect(_eventReceiver, _replicator);
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

                    break;

                case IDeltaQueryResponse response:
                    Log($"received response: {response})");
                    if (_queryResponses.TryRemove(response.QueryId, out var tcs))
                    {
                        var x = tcs.TrySetResult(response);
                    }

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

    /// <inheritdoc />
    public override async Task<SignOnResponse> SignOn()
    {
        var signOnResponse =
            await Query<SignOnResponse, SignOnRequest>(new SignOnRequest(_lionWebVersion.VersionString, ClientId,
                IdUtils.NewId(), null));
        ParticipationId = signOnResponse.ParticipationId;
        return signOnResponse;
    }

    /// <inheritdoc />
    public override async Task<SignOffResponse> SignOff() =>
        await Query<SignOffResponse, SignOffRequest>(new SignOffRequest(IdUtils.NewId(), null));

    /// <inheritdoc />
    public override async Task<GetAvailableIdsResponse> GetAvailableIds(int count) =>
        await Query<GetAvailableIdsResponse, GetAvailableIdsRequest>(
            new GetAvailableIdsRequest(count, IdUtils.NewId(), null));

    private async Task<TResponse> Query<TResponse, TRequest>(TRequest request)
        where TResponse : class, IDeltaQueryResponse where TRequest : IDeltaQueryRequest
    {
        var tcs = new TaskCompletionSource<IDeltaQueryResponse>();
        _queryResponses[request.QueryId] = tcs;
        await Send(request);
        var response = await tcs.Task;
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
}

public class CommandIdProvider : ICommandIdProvider
{
    private int nextId = 0;

    /// <inheritdoc />
    public string Create() => (++nextId).ToString();
}