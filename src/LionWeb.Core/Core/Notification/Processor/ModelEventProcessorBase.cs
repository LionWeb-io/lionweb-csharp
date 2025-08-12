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

/// Forwards all <see cref="Receive">received</see> events unchanged to <i>following</i> processors,
/// and to EventHandlers <see cref="Subscribe{TSubscribedEvent}">subscribed</see> to specific events.
public abstract class ModelEventProcessorBase<TEvent>(object? sender)
    : EventProcessorBase<TEvent>(sender)
    where TEvent : IEvent
{
    /// <inheritdoc />
    public override void Receive(TEvent message) =>
        Send(message);

    /// Registers <paramref name="handler"/> to be notified of events compatible with <typeparamref name="TSubscribedEvent"/>.
    /// <typeparam name="TSubscribedEvent">
    /// Type of events <paramref name="handler"/> is interested in.
    /// Events raised by this publisher that are <i>not</i> compatible with <typeparamref name="TSubscribedEvent"/>
    /// will <i>not</i> reach <paramref name="handler"/>.
    /// </typeparam> 
    public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent =>
        IProcessor.Connect<TEvent, TSubscribedEvent, TSubscribedEvent>(this,
            new EventHandlerProcessor<TSubscribedEvent>(handler)
            {
                ProcessorId = Sender.ToString() ?? GetType().Name
            });

    /// Unregisters <paramref name="handler"/> from notification of events.
    /// Silently ignores calls for unsubscribed <paramref name="handler"/>. 
    public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
        where TSubscribedEvent : class, TEvent => throw new NotImplementedException();
}