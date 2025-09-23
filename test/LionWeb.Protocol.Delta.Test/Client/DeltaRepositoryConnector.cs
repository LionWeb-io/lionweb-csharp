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

namespace LionWeb.Protocol.Delta.Test.Client;

using Core;
using Core.Notification;
using Delta.Client;
using Delta.Repository;
using Message;

internal class DeltaRepositoryConnector : IDeltaRepositoryConnector
{
    private readonly NotificationToDeltaCommandMapper _mapper;

    public DeltaRepositoryConnector(LionWebVersions lionWebVersion)
    {
        _mapper = new NotificationToDeltaCommandMapper(new CommandIdProvider(), lionWebVersion);
    }

    public Action<IDeltaContent> Sender { get; set; }

    public Task SendToClient(IClientInfo clientInfo, IDeltaContent content)
    {
        Sender?.Invoke(content);
        return Task.CompletedTask;
    }

    public Task SendToAllClients(IDeltaContent content, HashSet<NodeId> affectedPartitions)
    {
        Sender?.Invoke(content);
        return Task.CompletedTask;
    }

    public event EventHandler<IMessageContext<IDeltaContent>>? ReceiveFromClient;
    public void ReceiveMessageFromClient(IDeltaMessageContext context) => ReceiveFromClient?.Invoke(null, context);
    public IDeltaContent Convert(INotification notification) => _mapper.Map(notification);
}