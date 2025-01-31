// Copyright 2025 TRUMPF Laser SE and other contributors
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

using Forest;
using Partition;
using System.Reflection;

public abstract class EventHandlerBase
{
    protected static readonly ILookup<Type, Type> _allSubtypes = InitAllSubtypes();

    private static ILookup<Type, Type> InitAllSubtypes()
    {
        Type[] allTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes();

        List<Type> baseTypes = [typeof(IEvent), typeof(IForestEvent), typeof(IPartitionEvent)];

        return baseTypes
            .SelectMany(baseType => allTypes
                .Where(subType => subType.IsAssignableTo(baseType))
                .SelectMany(subType => new List<(Type, Type)> { (baseType, subType), (subType, subType) })
            )
            .ToLookup(k => k.Item1, e => e.Item2);
    }
}

public abstract class EventHandlerBase<TWrite> : EventHandlerBase, ICommander<TWrite>, IPublisher<TWrite>
    where TWrite : IEvent
{
    private readonly object _sender;

    private readonly Dictionary<Type, int> _subscribedEvents = [];
    private readonly Dictionary<object, EventHandler<TWrite>> _handlers = [];

    private readonly AsyncLocal<Queue<string>> _eventIds = new() { Value = new() };

    private event EventHandler<TWrite>? Event;

    private int _nextId = 0;


    /// <inheritdoc cref="EventHandlerBase"/>
    /// <param name="sender">Optional sender of the events.</param>
    protected EventHandlerBase(object? sender)
    {
        _sender = sender ?? this;
    }

    /// <inheritdoc />
    public void RegisterEventId(string eventId)
        => _eventIds.Value!.Enqueue(eventId);

    /// <inheritdoc />
    public virtual EventId CreateEventId() =>
        _eventIds.Value!.TryDequeue(out var r) ? r : _nextId++.ToString();

    /// <inheritdoc />
    public void Subscribe<TRead>(EventHandler<TRead> handler) where TRead : TWrite
    {
        RegisterSubscribedEvents<TRead>();

        EventHandler<TWrite> writeHandler = (sender, args) =>
        {
            if (args is TRead r)
                handler.Invoke(sender, r);
        };
        _handlers[handler] = writeHandler;

        Event += writeHandler;
    }

    private void RegisterSubscribedEvents<TRead>() where TRead : TWrite
    {
        var eventType = typeof(TRead);
        var allSubtype = _allSubtypes[eventType];
        foreach (var subtype in allSubtype)
        {
            if (_subscribedEvents.TryGetValue(subtype, out var count))
            {
                _subscribedEvents[subtype] = count + 1;
            } else
            {
                _subscribedEvents[subtype] = 1;
            }
        }
    }


    /// <inheritdoc />
    public void Unsubscribe<TRead>(EventHandler<TRead> handler) where TRead : TWrite
    {
        if (!_handlers.Remove(handler, out var writeHandler))
            return;

        Event -= writeHandler;

        UnregisterSubscribedEvents<TRead>();
    }

    private void UnregisterSubscribedEvents<TRead>() where TRead : TWrite
    {
        var eventType = typeof(TRead);
        var allSubtypes = _allSubtypes[eventType];
        foreach (var subtype in allSubtypes)
        {
            _subscribedEvents[subtype]--;
        }
    }

    /// <inheritdoc />
    public void Raise(TWrite @event) =>
        Event?.Invoke(_sender, @event);

    /// <inheritdoc />
    public bool CanRaise(Type eventType) =>
        Event != null && _subscribedEvents.TryGetValue(eventType, out var count) && count > 0;
}