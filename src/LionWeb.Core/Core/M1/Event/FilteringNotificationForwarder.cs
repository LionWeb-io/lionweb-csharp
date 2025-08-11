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

using System.Diagnostics;

/// Forwards notifications raised by <paramref name="localPublisher"/> to <see cref="Subscribe{TSubscribedEvent}">subscribers</see>
/// if the event passes <see cref="Filter{TSubscribedEvent}"/>.
public abstract class FilteringNotificationForwarder<TEvent, TPublisher>(TPublisher? localPublisher)
    : IDisposable, IPublisher<TEvent>
    where TEvent : class, INotification
    where TPublisher : IPublisher<TEvent>
{
    private readonly TPublisher? _localPublisher = localPublisher;
    private readonly Dictionary<object, EventHandler<TEvent>> _forwardingHandlers = [];

    /// <inheritdoc />
    public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : class, TEvent
    {
        if (_localPublisher == null)
            return;

        EventHandler<TEvent> forwardingHandler = (sender, @event) =>
        {
            if (@event is not TSubscribedEvent s)
                return;

            var r = Filter<TSubscribedEvent>(s);
            Debug.WriteLine($"Forwarding event id {@event.NotificationId}: {r?.NotificationId}");
            if (r is not null)
                handler(sender, s);
        };

        _forwardingHandlers.Add(handler, forwardingHandler);

        _localPublisher.Subscribe(forwardingHandler);
    }

    /// <inheritdoc />
    public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : class, TEvent
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
    protected abstract TSubscribedEvent? Filter<TSubscribedEvent>(TEvent @event) where TSubscribedEvent : class, TEvent;
}

/// Suppresses all notifications with <see cref="RegisterNotificationId">registered notification ids</see>.
public abstract class NotificationIdFilteringNotificationForwarder<TEvent, TPublisher>(TPublisher? localPublisher)
    : FilteringNotificationForwarder<TEvent, TPublisher>(localPublisher)
    where TEvent : class, INotification where TPublisher : IPublisher<TEvent>
{
    private readonly HashSet<INotificationId> _notificationIds = [];

    /// Suppresses future notifications with <paramref name="notificationId"/> from <see cref="FilteringNotificationForwarder{TEvent,TPublisher}.Subscribe{TSubscribedEvent}">subscribers</see>.
    protected void RegisterNotificationId(INotificationId notificationId) =>
        _notificationIds.Add(notificationId);

    /// Forwards future notifications with <paramref name="notificationId"/> to <see cref="FilteringNotificationForwarder{TEvent,TPublisher}.Subscribe{TSubscribedEvent}">subscribers</see>.
    protected void UnregisterNotificationId(INotificationId notificationId) =>
        _notificationIds.Remove(notificationId);

    /// <inheritdoc />
    protected override TSubscribedEvent? Filter<TSubscribedEvent>(TEvent @event)  where TSubscribedEvent : class
    {
        var result = !_notificationIds.Contains(@event.NotificationId);
        return result ? @event as TSubscribedEvent : null;
    }
}