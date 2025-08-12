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

namespace LionWeb.Core.Notification;

using Forest;
using Partition;
using System.Reflection;
using Utilities;

public abstract class EventHandlerBase
{
    protected static readonly ILookup<Type, Type> AllSubtypes = InitAllSubtypes();

    private static ILookup<Type, Type> InitAllSubtypes()
    {
        Type[] allTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes();

        List<Type> baseTypes = [typeof(INotification), typeof(IForestNotification), typeof(IPartitionNotification)];

        return baseTypes
            .SelectMany(baseType => allTypes
                .Where(subType => subType.IsAssignableTo(baseType))
                .SelectMany(subType => new List<(Type, Type)> { (baseType, subType), (subType, subType) })
            )
            .ToLookup(k => k.Item1, e => e.Item2);
    }
}

/// Forwards <see cref="ICommander{TEvent}"/> commands to <see cref="IPublisher{TEvent}"/> events.
public abstract class EventHandlerBase<TEvent> : EventHandlerBase where TEvent : INotification
{
    private readonly object _sender;

    private readonly Dictionary<Type, int> _subscribedEvents = [];
    private readonly Dictionary<object, EventHandler<TEvent>> _handlers = [];

    private event EventHandler<TEvent>? Event;

    // Unique per instance
    private readonly EventId _eventIdBase;

    private int _nextId = 0;

    /// <inheritdoc cref="EventHandlerBase"/>
    /// <param name="sender">Optional sender of the events.</param>
    protected EventHandlerBase(object? sender)
    {
        _sender = sender ?? this;
        _eventIdBase = sender as string ?? IdUtils.NewId();
    }

    /// <inheritdoc />
    public virtual INotificationId CreateEventId() => new NumericNotificationId(_eventIdBase, _nextId++);

    /// <inheritdoc />
    public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : class, TEvent
    {
        RegisterSubscribedEvents<TSubscribedEvent>();

        EventHandler<TEvent> writeHandler = (sender, args) =>
        {
            if (args is TSubscribedEvent r)
                handler.Invoke(sender, r);
        };
        _handlers[handler] = writeHandler;

        Event += writeHandler;
    }

    private void RegisterSubscribedEvents<TSubscribedEvent>() where TSubscribedEvent : TEvent
    {
        var eventType = typeof(TSubscribedEvent);
        var allSubtype = AllSubtypes[eventType];
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
    public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : class, TEvent
    {
        if (!_handlers.Remove(handler, out var writeHandler))
            return;

        Event -= writeHandler;

        UnregisterSubscribedEvents<TSubscribedEvent>();
    }

    private void UnregisterSubscribedEvents<TSubscribedEvent>() where TSubscribedEvent : TEvent
    {
        var eventType = typeof(TSubscribedEvent);
        var allSubtypes = AllSubtypes[eventType];
        foreach (var subtype in allSubtypes)
        {
            _subscribedEvents[subtype]--;
        }
    }

    /// <inheritdoc />
    public void Raise(TEvent @event) =>
        Event?.Invoke(_sender, @event);

    /// <inheritdoc />
    public bool CanRaise(params Type[] eventTypes) =>
        Event != null &&
        eventTypes.Any(eventType => _subscribedEvents.TryGetValue(eventType, out var count) && count > 0);
}

/// Notification id based on a string and an increasing number.
public record NumericNotificationId(string Base, int Id) : INotificationId;