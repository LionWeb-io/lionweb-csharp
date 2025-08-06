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

public interface
    IEventProcessor<in TEvent> : IProcessor<TEvent, TEvent> //, ICommander<TEvent>, IPublisher<TEvent> where TEvent : IEvent
{
}

/// Sends event notifications of events compatible with <typeparamref name="TEvent"/>
/// via a corresponding <see cref="IPublisher{TEvent}"/>.
public interface ICommander<in TEvent> where TEvent : IEvent
{
    /// Raises <paramref name="@event"/>, i.e. sends it to all <see cref="IPublisher{TEvent}.Subscribe{TSubscribedEvent}">subscribers</see>.
    public void Raise(TEvent @event);

    /// Whether anybody would receive any of the <paramref name="eventTypes"/> events.
    /// Useful for returning eagerly from complex logic to calculate the event contents.
    /// <value>
    ///     <c>true</c> if someone would receive any of the <paramref name="eventTypes"/> events; <c>false</c> otherwise.
    /// </value>
    public bool CanRaise(params Type[] eventTypes);

    /// Creates a globally unique event id.
    // public IEventId CreateEventId();
}

/// Provides event notifications (aka callbacks) of events compatible with <typeparamref name="TEvent"/>
/// to <see cref="Subscribe{TSubscribedEvent}">subscribers</see>. 
public interface IPublisher<in TEvent> where TEvent : IEvent
{
    /// Registers <paramref name="handler"/> to be notified of events compatible with <typeparamref name="TSubscribedEvent"/>.
    /// <typeparam name="TSubscribedEvent">
    /// Type of events <paramref name="handler"/> is interested in.
    /// Events raised by this publisher that are <i>not</i> compatible with <typeparamref name="TSubscribedEvent"/>
    /// will <i>not</i> reach <paramref name="handler"/>.
    /// </typeparam> 
    void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : class, TEvent;

    /// Unregisters <paramref name="handler"/> from notification of events.
    /// Silently ignores calls for unsubscribed <paramref name="handler"/>. 
    void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler) where TSubscribedEvent : class, TEvent;
}