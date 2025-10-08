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

class TestDeltaClientConnector : IDeltaClientConnector, IDeltaRepositoryClient
{
    private readonly NotificationToDeltaCommandMapper _mapper;

    private ClientInfo _clientInfo;
    private IDeltaRepositoryConnector _deltaRepositoryConnector;

    public TestDeltaClientConnector(LionWebVersions lionWebVersion)
    {
        _mapper = new(new CommandIdProvider(), lionWebVersion);
    }

    public void Connect(ClientId clientId, IDeltaRepositoryConnector deltaRepositoryConnector)
    {
        _clientInfo = new ClientInfo { ClientId = clientId };
        deltaRepositoryConnector.AddClient(_clientInfo, this);
        _deltaRepositoryConnector = deltaRepositoryConnector;
    }

    public event EventHandler<IDeltaContent>? ReceivedFromRepository;

    public Task SendToRepository(IDeltaContent content)
    {
        _deltaRepositoryConnector.ReceiveFromClient(new DeltaMessageContext(_clientInfo, content));
        return Task.CompletedTask;
    }

    public IDeltaContent Convert(INotification notification) =>
        _mapper.Map(notification);

    public Task SendToClient(IDeltaContent content)
    {
        ReceivedFromRepository?.Invoke(this, content);
        return Task.CompletedTask;
    }
}