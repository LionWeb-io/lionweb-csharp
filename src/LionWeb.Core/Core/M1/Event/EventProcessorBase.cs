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

/// Base class to forward all <see cref="ForwardTo">raised</see> events.
public abstract class EventProcessorBase
{
    protected static readonly ILookup<Type, Type> AllSubtypes = InitAllSubtypes();

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
public abstract class EventProcessorBase<TEvent> : EventProcessorBase, IEventProcessor<TEvent> where TEvent : IEvent
{
    private readonly object _sender;
    private readonly Dictionary<Type, int> _subscribedEvents = [];
    private readonly Dictionary<IProcessor, EventHandler<TEvent>> _handlers = [];

    /// <inheritdoc cref="EventProcessorBase"/>
    /// <param name="sender">Optional sender of the events.</param>
    protected EventProcessorBase(object? sender)
    {
        _sender = sender ?? this;
    }

    /// <inheritdoc />
    public string ProcessorId =>
        _sender?.ToString() ?? GetType().Name;

    /// <inheritdoc />
    public abstract void Receive(TEvent message);

    private event EventHandler<TEvent>? InternalEvent;


    /// <inheritdoc />
    public bool CanReceive(params Type[] messageTypes) =>
        InternalEvent != null &&
        messageTypes.Any(eventType => _subscribedEvents.TryGetValue(eventType, out var count) && count > 0);


    /// <inheritdoc />
    void IProcessor<TEvent, TEvent>.Send(TEvent message) =>
        Send(message);

    /// <inheritdoc cref="IProcessor{TReceive,TSend}.Send"/>
    protected void Send(TEvent message) =>
        InternalEvent?.Invoke(_sender, message);

    /// <inheritdoc />
    void IProcessor<TEvent, TEvent>.Subscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver) =>
        Subscribe(receiver);

    /// <inheritdoc cref="IProcessor{TReceive,TSend}.Subscribe{TReceiveTo,TSendTo}"/>
    protected void Subscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver)
    {
        RegisterSubscribedEvents<TReceiveTo>();

        var handler = CreateHandler(receiver);

        _handlers[receiver] = handler;

        InternalEvent += handler;
    }

    private EventHandler<TEvent> CreateHandler<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver)
        =>
            (object? _, TEvent @event) =>
            {
                if (@event is TReceiveTo r)
                    receiver.Receive(r);
            };

    private void RegisterSubscribedEvents<TSubscribedEvent>()
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

    private void UnregisterSubscribedEvents<TSubscribedEvent>()
    {
        var eventType = typeof(TSubscribedEvent);
        var allSubtypes = AllSubtypes[eventType];
        foreach (var subtype in allSubtypes)
        {
            _subscribedEvents[subtype]--;
        }
    }

    /// <inheritdoc />
    void IProcessor<TEvent, TEvent>.Unsubscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver) =>
        Unsubscribe(receiver);

    /// <inheritdoc cref="IProcessor{TReceive,TSend}.Unsubscribe{TReceiveTo,TSendTo}"/>
    protected void Unsubscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver)
    {
        if (!_handlers.Remove(receiver, out var handler))
            return;

        InternalEvent -= handler;

        UnregisterSubscribedEvents<TReceiveTo>();
    }

    /// <inheritdoc />
    public void PrintAllReceivers(List<IProcessor> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}({_sender})");
        if (IProcessor.RecursionDetected(this, alreadyPrinted, indent))
            return;

        Console.WriteLine($"{indent}Handlers:");
        foreach (var processor in this._handlers.Keys)
        {
            processor.PrintAllReceivers(alreadyPrinted, indent + "  ");
        }
    }
}