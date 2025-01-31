﻿// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Core.M1.Event;

using Utilities;

public abstract class EventReplicatorBase<TEvent, TPublisher> : IDisposable, IPublisher<TEvent>
    where TEvent : IEvent where TPublisher : IPublisher<TEvent>
{
    private readonly TPublisher? _localPublisher;
    private readonly ICommander<TEvent>? _localCommander;
    private readonly List<TPublisher> _publishers = [];
    private readonly HashSet<EventId> _eventIds = [];
    private readonly Dictionary<object, EventHandler<TEvent>> _forwardingHandlers = [];

    protected readonly Dictionary<NodeId, IReadableNode> _nodeById;

    protected EventReplicatorBase(TPublisher? localPublisher, ICommander<TEvent>? localCommander,
        Dictionary<NodeId, IReadableNode>? sharedNodeMap = null)
    {
        _localPublisher = localPublisher;
        _localCommander = localCommander;
        _nodeById = sharedNodeMap ?? new();
    }

    /// <inheritdoc />
    public void Subscribe<TRead>(EventHandler<TRead> handler) where TRead : TEvent
    {
        if (_localPublisher == null)
            return;
        
        EventHandler<TEvent> forwardingHandler = (sender, @event) =>
        {
            if (@event is TRead r && !_eventIds.Contains(@event.EventId))
                handler(sender, r);
        };

        _forwardingHandlers.Add(handler, forwardingHandler);

        _localPublisher.Subscribe(forwardingHandler);
    }

    /// <inheritdoc />
    public void Unsubscribe<TRead>(EventHandler<TRead> handler) where TRead : TEvent
    {
        if (_localPublisher == null)
            return;
        
        if (!_forwardingHandlers.Remove(handler, out var forwardingHandler))
            return;

        _localPublisher.Unsubscribe(forwardingHandler);
    }

    public virtual void Subscribe(TPublisher publisher)
    {
        publisher.Subscribe<TEvent>(ProcessEvent);
        _publishers.Add(publisher);
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        foreach (var publisher in _publishers)
        {
            publisher.Unsubscribe<TEvent>(ProcessEvent);
        }
    }

    protected abstract void ProcessEvent(object? sender, TEvent @event);

    protected void RegisterEventId(EventId eventId) =>
        _eventIds.Add(eventId);

    protected void UnregisterEventId(EventId eventId) =>
        _eventIds.Remove(eventId);

    protected void RegisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            if (!_nodeById.TryAdd(node.GetId(), node))
                throw new DuplicateNodeIdException(node, _nodeById[node.GetId()]);
        }
    }

    protected void UnregisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            _nodeById.Remove(node.GetId());
        }
    }

    protected virtual INode Lookup(NodeId remoteNodeId) =>
        (INode)_nodeById[remoteNodeId];

    protected virtual INode? LookupOpt(NodeId remoteNodeId) =>
        (INode?)_nodeById.GetValueOrDefault(remoteNodeId);

    protected virtual INode Clone(INode remoteNode) =>
        new SameIdCloner(remoteNode.Descendants(true, true)).Clone()[remoteNode];

    protected class SameIdCloner : Cloner
    {
        public SameIdCloner(IEnumerable<INode> inputNodes) : base(inputNodes)
        {
        }

        protected override string GetNewId(INode remoteNode) =>
            remoteNode.GetId();
    }

    protected void PauseCommands(Func<Action?> action)
    {
        EventId? eventId = null;
        if (_localCommander != null)
        {
            eventId = _localCommander.CreateEventId();
            _localCommander.RegisterEventId(eventId);
            RegisterEventId(eventId);
        }

        Action? postAction = null;
        try
        {
            postAction = action();
        } finally
        {
            if (eventId != null)
                UnregisterEventId(eventId);

            postAction?.Invoke();
        }
    }
}