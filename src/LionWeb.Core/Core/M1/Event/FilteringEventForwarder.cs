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

/// Forwards events raised by <paramref name="localPublisher"/> to <see cref="Subscribe{TSubscribedEvent}">subscribers</see>
/// if the event passes <see cref="Filter{TSubscribedEvent}"/>.
public abstract class FilteringEventForwarder<TEvent, TPublisher>(TPublisher? localPublisher)
    : IDisposable, IPublisher<TEvent>
    where TEvent : IEvent
    where TPublisher : IPublisher<TEvent>
{
    private readonly TPublisher? _localPublisher = localPublisher;
    private readonly Dictionary<object, EventHandler<TEvent>> _forwardingHandlers = [];

    /// <inheritdoc />
    public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : TEvent
    {
        if (_localPublisher == null)
            return;

        EventHandler<TEvent> forwardingHandler = (sender, @event) =>
        {
            if (@event is TSubscribedEvent r && Filter<TSubscribedEvent>(@event))
                handler(sender, r);
        };

        _forwardingHandlers.Add(handler, forwardingHandler);

        _localPublisher.Subscribe(forwardingHandler);
    }

    /// <inheritdoc />
    public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : TEvent
    {
        if (_localPublisher == null)
            return;

        if (!_forwardingHandlers.Remove(handler, out var forwardingHandler))
            return;

        _localPublisher.Unsubscribe(forwardingHandler);
    }

    /// Unsubscribes all registered <see cref="Subscribe{TSubscribedEvent}">subscribers</see>.
    public virtual void Dispose()
    {
        if (_localPublisher == null)
            return;

        foreach (var handler in _forwardingHandlers.Values)
        {
            _localPublisher.Unsubscribe(handler);
        }
        
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Determines whether <paramref name="@event"/> will be forwarded to <see cref="Subscribe{TSubscribedEvent}">subscribers</see>.
    /// </summary>
    /// <param name="event">Event to check.</param>
    /// <typeparam name="TSubscribedEvent">Requested type of event.</typeparam>
    /// <returns><c>true</c> if <paramref name="@event"/> should be forwarded to <see cref="Subscribe{TSubscribedEvent}">subscribers</see>; <c>false</c> otherwise.</returns>
    protected abstract bool Filter<TSubscribedEvent>(TEvent @event) where TSubscribedEvent : TEvent;
}

/// Suppresses all events with <see cref="RegisterEventId">registered event ids</see>.
public abstract class EventIdFilteringEventForwarder<TEvent, TPublisher>(TPublisher? localPublisher)
    : FilteringEventForwarder<TEvent, TPublisher>(localPublisher)
    where TEvent : IEvent where TPublisher : IPublisher<TEvent>
{
    private readonly HashSet<EventId> _eventIds = [];

    /// Suppresses future events with <paramref name="eventId"/> from <see cref="FilteringEventForwarder{TEvent,TPublisher}.Subscribe{TSubscribedEvent}">subscribers</see>.
    protected void RegisterEventId(EventId eventId) =>
        _eventIds.Add(eventId);

    /// Forwards future events with <paramref name="eventId"/> to <see cref="FilteringEventForwarder{TEvent,TPublisher}.Subscribe{TSubscribedEvent}">subscribers</see>.
    protected void UnregisterEventId(EventId eventId) =>
        _eventIds.Remove(eventId);

    /// <inheritdoc />
    protected override bool Filter<TSubscribedEvent>(TEvent @event) =>
        !_eventIds.Contains(@event.EventId);
}