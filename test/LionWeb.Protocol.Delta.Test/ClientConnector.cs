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

namespace LionWeb.Protocol.Delta.Test;

using Client;
using Core;
using Core.Notification;
using Message;
using Repository;
using System.Text;

class ClientConnector : IDeltaClientConnector
{
    private readonly DeltaSerializer _deltaSerializer = new();
    private readonly NotificationToDeltaCommandMapper _mapper;
    private RepositoryConnector _repositoryConnector;
    private ClientInfo _clientInfo;


    public ClientConnector(LionWebVersions lionWebVersion)
    {
        _mapper = new(new CommandIdProvider(), lionWebVersion);
    }

    public void Connect(ParticipationId participationId, RepositoryConnector repositoryConnector)
    {
        _clientInfo = new ClientInfo { ParticipationId = participationId };
        repositoryConnector.AddClient(_clientInfo, this);
        _repositoryConnector = repositoryConnector;
    }

    public Task SendToRepository(IDeltaContent content)
    {
        var encoded = Encode(content);
        _repositoryConnector.MessageFromClient(_clientInfo, encoded);

        return Task.CompletedTask;
    }

    private byte[] Encode(IDeltaContent content) =>
        Encode(_deltaSerializer.Serialize(content));

    private static byte[] Encode(string msg) =>
        Encoding.UTF8.GetBytes(msg);

    public event EventHandler<IDeltaContent>? ReceiveFromRepository;

    public IDeltaContent Convert(INotification notification) =>
        _mapper.Map(notification);

    public void MessageFromRepository(byte[] encoded)
    {
        var deltaContent = _deltaSerializer.Deserialize<IDeltaContent>(Encoding.UTF8.GetString(encoded));
        ReceiveFromRepository?.Invoke(this, deltaContent);
    }
}