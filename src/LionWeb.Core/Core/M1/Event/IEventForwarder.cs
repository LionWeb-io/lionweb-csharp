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

public interface IEventForwarder;

public class EventForwarder<TEvent> : IEventForwarder where TEvent : class, IEvent
{
    private readonly IPublisher<TEvent> _from;
    private readonly ICommander<TEvent> _to;

    public EventForwarder(IPublisher<TEvent> from, ICommander<TEvent> to)
    {
        _from = from;
        _to = to;
        from.Subscribe<TEvent>(Handler);
    }
    
    private void Handler(object? sender, TEvent @event)
    {
        if (_to.CanRaise(typeof(TEvent)))
            _to.Raise(@event);
    }
}

