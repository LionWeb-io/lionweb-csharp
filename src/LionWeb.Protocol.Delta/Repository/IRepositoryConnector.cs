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

namespace LionWeb.Protocol.Delta.Repository;

using Core.Notification;

public interface IRepositoryConnector<T>
{
    Task SendToClient(IClientInfo clientInfo, T content);
    Task SendToAllClients(T content);
    event EventHandler<IMessageContext<T>> ReceiveFromClient;
    T Convert(INotification notification);
}

public interface IMessageContext<T>
{
    IClientInfo ClientInfo { get; }
    T Content { get; }
}

public interface IClientInfo
{
    ParticipationId ParticipationId { get; }
    
    EventSequenceNumber GetAndIncrementSequenceNumber();
}
