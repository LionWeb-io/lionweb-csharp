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

/// Composes two or more <see cref="IEventProcessor{TEvent}"/>s.
///
/// Every message this processor <see cref="Receive">receives</see>
/// is forwarded to the first <see cref="_eventProcessors">component</see>.
/// Each component is connected to the next component.
/// The last component <see cref="IProcessor{TReceive,TSend}.Send">sends</see> to
/// this processor's <i>following</i> processors.
public class CompositeEventProcessor<TEvent> : IEventProcessor<TEvent>
    where TEvent : class, IEvent
{
    private readonly IEventProcessor<TEvent> _firstProcessor;
    private readonly IEventProcessor<TEvent> _lastProcessor;
    private readonly IEnumerable<IEventProcessor<TEvent>> _eventProcessors;
    private readonly object? _sender;

    public CompositeEventProcessor(List<IEventProcessor<TEvent>> eventProcessors, object? sender)
    {
        _eventProcessors = eventProcessors;
        _sender = sender;
        if(eventProcessors.Count < 2)
            throw new ArgumentException($"{nameof(CompositeEventProcessor<TEvent>)} must get at least 2 processors");
        
        _firstProcessor = eventProcessors[0];
        _lastProcessor = eventProcessors[^1];

        var enumerator = eventProcessors.GetEnumerator();
        enumerator.MoveNext();
        var previous = enumerator.Current;
        while(enumerator.MoveNext())
        {
            var current = enumerator.Current;
            IProcessor.Connect(previous, current);
            
            previous = current;
        }
    }

    /// <inheritdoc />
    public string ProcessorId =>
        _sender?.ToString() ?? GetType().Name;

    /// <inheritdoc />
    public void Receive(TEvent message) =>
        _firstProcessor.Receive(message);

    /// <inheritdoc />
    public bool CanReceive(params Type[] messageTypes) => 
        _firstProcessor.CanReceive(messageTypes);

    void IProcessor<TEvent, TEvent>.Send(TEvent message) =>
        throw new ArgumentException("Should never be called");

    void IProcessor<TEvent, TEvent>.Subscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver) => 
        _lastProcessor.Subscribe(receiver);

    void IProcessor<TEvent, TEvent>.Unsubscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver) =>
        _lastProcessor.Unsubscribe(receiver);

    /// <inheritdoc />
    public void PrintAllReceivers(List<IProcessor> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}({_sender})");
        if (IProcessor.RecursionDetected(this, alreadyPrinted, indent))
            return;

        Console.WriteLine($"{indent}Members:");
        foreach (var processor in this._eventProcessors)
        {
            processor.PrintAllReceivers(alreadyPrinted, indent + "  ");
        }
    }
}