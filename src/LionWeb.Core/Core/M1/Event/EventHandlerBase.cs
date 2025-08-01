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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Utilities;

public abstract class EventHandlerBase
{
    protected static readonly ILookup<Type, Type> AllSubtypes = InitAllSubtypes();

    public abstract void Raise(object @event);

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

/// Forwards <see cref="ICommander{TEvent}"/> commands to <see cref="IPublisher{TEvent}"/> events.
public abstract class EventHandlerBase<TEvent> : EventHandlerBase, ICommander<TEvent>, IPublisher<TEvent>
    where TEvent : IEvent
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
    public virtual IEventId CreateEventId() => new NumericEventId(_eventIdBase, _nextId++);

    // protected internal bool TryRegisteredEventId([NotNullWhen(true)] out IEventId? eventId) =>
    //     _eventIds.TryDequeue(out eventId);

    /// <inheritdoc />
    public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent
    {
        RegisterSubscribedEvents<TSubscribedEvent>();

        _handlers[handler] = WriteHandler;

        Event += WriteHandler;
        return;

        void WriteHandler(object? sender, TEvent args)
        {
            if (args is TSubscribedEvent r)
                handler.Invoke(sender, r);
        }
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
    public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent
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
    public override void Raise(object @event) =>
        Raise((TEvent)@event);

    /// <inheritdoc />
    public void Raise(TEvent @event) =>
        Event?.Invoke(_sender, @event);

    /// <inheritdoc />
    public bool CanRaise(params Type[] eventTypes) =>
        Event != null &&
        eventTypes.Any(eventType => _subscribedEvents.TryGetValue(eventType, out var count) && count > 0);
}

/// Internal event id based on a string and an increasing number.
public record NumericEventId(string Base, int Id) : IEventId
{
    /// <inheritdoc />
    public virtual bool Equals(NumericEventId? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Base, other.Base, StringComparison.InvariantCulture) && Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Base, StringComparer.InvariantCulture);
        hashCode.Add(Id);
        return hashCode.ToHashCode();
    }
}