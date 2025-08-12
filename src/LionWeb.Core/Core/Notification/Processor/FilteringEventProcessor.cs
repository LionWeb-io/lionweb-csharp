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

namespace LionWeb.Core.M1.Event.Processor;

using Notification;
using System.Diagnostics;

/// Forwards <see cref="Receive">received</see> events if the event passes <see cref="Filter"/>.
public abstract class FilteringEventProcessor<TEvent>(object? sender)
    : /*IDisposable,*/ EventProcessorBase<TEvent>(sender)
    where TEvent : class, INotification
{

    /// Unsubscribes all registered <see cref="Subscribe{TSubscribedEvent}">subscribers</see>.
    // public virtual void Dispose()
    // {
    //     if (_localPublisher == null)
    //         return;
    //
    //     foreach (var handler in _forwardingHandlers.Values)
    //     {
    //         _localPublisher.Unsubscribe(handler);
    //     }
    //     
    //     GC.SuppressFinalize(this);
    // }

    /// <inheritdoc />
    public override void Receive(TEvent message)
    {
        var filtered = Filter(message);
        Debug.WriteLine($"Forwarding event id {message.NotificationId}: {filtered?.NotificationId}");
        if (filtered is not null)
            Send(filtered);
    }


    /// Determines whether <paramref name="@event"/> will be <see cref="IProcessor{TReceive,TSend}.Send">sent</see> to <i>following</i> processors.
    /// <param name="event">Event to check.</param>
    /// <returns>the event to send, or <c>null</c>.</returns>
    protected abstract TEvent? Filter(TEvent @event);
}

/// Suppresses all events with <see cref="RegisterEventId">registered event ids</see>.
public class EventIdFilteringEventProcessor<TEvent>(object? sender) : FilteringEventProcessor<TEvent>(sender) where TEvent : class, INotification
{
    private readonly HashSet<INotificationId> _eventIds = [];

    /// Suppresses future events with <paramref name="eventId"/> from <see cref="IProcessor{TReceive,TSend}.Send">sending</see>.
    public void RegisterEventId(INotificationId eventId) =>
        _eventIds.Add(eventId);

    /// <see cref="IProcessor{TReceive,TSend}.Send">Sends</see> future events with <paramref name="eventId"/>.
    public void UnregisterEventId(INotificationId eventId) =>
        _eventIds.Remove(eventId);

    /// <inheritdoc />
    protected override TEvent? Filter(TEvent @event)
    {
        var result = !_eventIds.Contains(@event.NotificationId);
        return result ? @event : null;
    }
}