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

public abstract class CommanderPublisherEventProcessorBase<TEvent>(object? sender)
    : EventProcessorBase<TEvent>(sender), ICommander<TEvent>, IPublisher<TEvent>
    where TEvent : IEvent
{
    private readonly IEventIdProvider _eventIdProvider = new EventIdProvider(sender);

    /// <inheritdoc />
    public void Raise(TEvent @event)
    {
        if (@event.EventId == null)
            @event.EventId = _eventIdProvider.CreateEventId();

        Receive(@event);
    }

    /// <inheritdoc />
    public bool CanRaise(params Type[] eventTypes) =>
        CanReceive(eventTypes);

    public override void Receive(TEvent message) =>
        Send(message);

    /// <inheritdoc />
    public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent =>
        IProcessor.Forward<TEvent, TSubscribedEvent, TSubscribedEvent>(this,
            new EventHandlerProcessor<TSubscribedEvent>(handler)
            {
                ProcessorId = sender?.ToString() ?? GetType().Name
            });

    /// <inheritdoc />
    public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent => throw new NotImplementedException();
}