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

using Core;
using Core.Notification;
using Message;
using Message.Event;
using Repository;
using System.Text;

class RepositoryConnector : IDeltaRepositoryConnector
{
    private readonly DeltaSerializer _deltaSerializer = new();
    private readonly NotificationToDeltaEventMapper _mapper;
    private readonly Dictionary<IClientInfo, ClientConnector> _clients = new Dictionary<IClientInfo, ClientConnector>(new ClientInfoComparer());

    private class ClientInfoComparer : IEqualityComparer<IClientInfo>
    {
        public bool Equals(IClientInfo? x, IClientInfo? y) => x.ParticipationId.Equals(y.ParticipationId);

        public int GetHashCode(IClientInfo obj) => obj.ParticipationId.GetHashCode();
    }

    public RepositoryConnector(LionWebVersions lionWebVersion)
    {
        _mapper = new(new ExceptionParticipationIdProvider(), lionWebVersion);
    }

    public void AddClient(IClientInfo clientInfo, ClientConnector clientConnector) =>
        _clients[clientInfo] = clientConnector;

    public async Task SendToClient(IClientInfo clientInfo, IDeltaContent content)
    {
        if (_clients.TryGetValue(clientInfo, out var clientConnector))
        {
            var encoded = Encode(clientInfo, content);
            clientConnector.MessageFromRepository(encoded);
        }
    }

    public async Task SendToAllClients(IDeltaContent content)
    {
        foreach ((var clientInfo, var clientConnector) in _clients)
        {
            var encoded = Encode(clientInfo, content);
            clientConnector.MessageFromRepository(encoded);
        }
    }

    private byte[] Encode(IClientInfo clientInfo, IDeltaContent content) =>
        Encode(_deltaSerializer.Serialize(UpdateSequenceNumber(content, clientInfo)));

    private static byte[] Encode(string msg) =>
        Encoding.UTF8.GetBytes(msg);

    private static IDeltaContent UpdateSequenceNumber(IDeltaContent content, IClientInfo clientInfo)
    {
        if (content is IDeltaEvent deltaEvent)
        {
            deltaEvent.SequenceNumber = clientInfo.IncrementAndGetSequenceNumber();
        }

        return content;
    }

    public event EventHandler<IMessageContext<IDeltaContent>>? ReceiveFromClient;

    public IDeltaContent Convert(INotification notification) =>
        _mapper.Map(notification);

    public void MessageFromClient(ClientInfo clientInfo, byte[] encoded)
    {
        var deltaContent = _deltaSerializer.Deserialize<IDeltaContent>(Encoding.UTF8.GetString(encoded));
        ReceiveFromClient?.Invoke(this, new DeltaMessageContext(clientInfo, deltaContent));
    }
}