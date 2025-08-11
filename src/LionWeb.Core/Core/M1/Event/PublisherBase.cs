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

[Obsolete]
public abstract class PublisherBase<TEvent> : EventProcessorBase, IPublisher<TEvent> where TEvent : IEvent
{
    protected readonly Dictionary<Type, int> _subscribedEvents = [];
    private readonly Dictionary<object, EventHandler<TEvent>> _handlers = [];

    protected event EventHandler<TEvent>? InternalEvent;

    /// <inheritdoc />
    public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent
    {
        RegisterSubscribedEvents<TSubscribedEvent>();

        var writeHandler = CreateHandler(handler);

        _handlers[handler] = writeHandler;

        InternalEvent += writeHandler;
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

    protected virtual EventHandler<TEvent> CreateHandler<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent
        =>
            (object? sender, TEvent @event) =>
            {
                if (@event is TSubscribedEvent r)
                    handler.Invoke(sender, r);
            };

    protected void RaiseInternal(object? sender, TEvent? @event) => InternalEvent?.Invoke(sender, @event);

    protected bool CanRaiseInternal(Type[] eventTypes) =>
        InternalEvent != null &&
        eventTypes.Any(eventType => _subscribedEvents.TryGetValue(eventType, out var count) && count > 0);

    /// <inheritdoc />
    public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent
    {
        if (!_handlers.Remove(handler, out var writeHandler))
            return;

        InternalEvent -= writeHandler;

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
}