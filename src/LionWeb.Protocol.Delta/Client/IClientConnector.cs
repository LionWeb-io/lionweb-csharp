// Copyright 2024 TRUMPF Laser GmbH
//
// Licensed under the Apache License, Version 2.0 (the "License");
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
// SPDX-FileCopyrightText: 2024 TRUMPF Laser GmbH
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Protocol.Delta.Client;

using Core.M1.Event;
using Message;

public interface IClientConnector<T>
{
    Task SendToRepository(T content);
    event EventHandler<T> ReceiveFromRepository;

    T Convert(IEvent internalEvent);
}

public interface IDeltaClientConnector : IClientConnector<IDeltaContent>;

public interface IEventClientConnector : IClientConnector<IEvent>
{
    IEvent IClientConnector<IEvent>.Convert(IEvent internalEvent) => internalEvent;
}