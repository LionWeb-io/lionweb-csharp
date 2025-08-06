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

namespace LionWeb.Protocol.Delta.Client;

using Core.M1.Event;

// public class LionWebClientEventProcessor<T> : IEventProcessor<IEvent>
// {
//     private readonly IClientConnector<T> _connector;
//
//     public LionWebClientEventProcessor(IClientConnector<T> connector)
//     {
//         _connector = connector;
//     }
//
//     /// <inheritdoc />
//     public void Raise(IEvent @event) =>
//         _connector.SendToRepository(@event);
//
//     /// <inheritdoc />
//     public bool CanRaise(params Type[] eventTypes) =>
//         true;
//
//     /// <inheritdoc />
//     public IEventId CreateEventId() => throw new NotImplementedException();
//
//     /// <inheritdoc />
//     public void Subscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
//         where TSubscribedEvent : class, IEvent =>
//         _connector.ReceiveFromRepository += CreateWriteHandler(handler);
//
//     protected virtual EventHandler<T> CreateWriteHandler<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
//         where TSubscribedEvent : class, IEvent
//         =>
//             (object? sender, T @event) =>
//             {
//                 if (@event is TSubscribedEvent r)
//                     handler.Invoke(sender, _connector.Convert(r));
//             };
//
//     /// <inheritdoc />
//     public void Unsubscribe<TSubscribedEvent>(EventHandler<TSubscribedEvent> handler)
//         where TSubscribedEvent : class, IEvent => throw new NotImplementedException();
// }