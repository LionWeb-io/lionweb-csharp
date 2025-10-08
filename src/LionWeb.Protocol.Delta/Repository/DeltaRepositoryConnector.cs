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

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.Notification;
using Message;
using Message.Event;

// TODO: Move more logic out of connector
public class DeltaRepositoryConnector : IDeltaRepositoryConnector
{
    private readonly DeltaSerializer _deltaSerializer = new();
    private readonly NotificationToDeltaEventMapper _mapper;

    private readonly Dictionary<IClientInfo, IDeltaRepositoryClient> _clients = new(IClientInfo.IdentityComparer);

    public DeltaRepositoryConnector(LionWebVersions lionWebVersion)
    {
        _mapper = new(new ExceptionParticipationIdProvider(), lionWebVersion);
    }

    /// <inheritdoc />
    public event EventHandler<IMessageContext<IDeltaContent>>? ReceivedFromClient;

    /// <inheritdoc />
    public void AddClient(IClientInfo clientInfo, IDeltaRepositoryClient clientConnector) =>
        _clients[clientInfo] = clientConnector;

    /// <inheritdoc />
    public void RemoveClient(ClientInfo clientInfo) =>
        _clients.Remove(clientInfo);

    /// <inheritdoc />
    public async Task SendToClient(IClientInfo clientInfo, IDeltaContent content)
    {
        if ((clientInfo.SignedOn || content is IDeltaError || !content.RequiresParticipationId) &&
            _clients.TryGetValue(clientInfo, out var clientConnector))
        {
            await clientConnector.SendToClient(UpdateSequenceNumber(content, clientInfo));
        }
    }

    /// <inheritdoc />
    public async Task SendToAllClients(IDeltaContent content, HashSet<NodeId> affectedPartitions)
    {
        foreach (var (clientInfo, clientConnector) in _clients)
        {
            if (!clientInfo.SignedOn)
                continue;

            var shouldSend = false;

            if ((clientInfo.NotifyAboutParitionDeletion ||
                 content.InternalParticipationId == clientInfo.ParticipationId) && content is PartitionDeleted)
                shouldSend = true;
            else if (clientInfo.NotifyAboutParitionCreation && content is PartitionAdded)
                shouldSend = true;

            if (clientInfo.SubscribedPartitions.Overlaps(affectedPartitions))
                shouldSend = true;

            if (clientInfo.SubscribeCreatedParitions && content is PartitionAdded a)
                clientInfo.SubscribedPartitions.Add(a.AffectedNode);

            if (content is PartitionDeleted d)
                clientInfo.SubscribedPartitions.Remove(d.DeletedPartition);

            if (shouldSend)
            {
                await clientConnector.SendToClient(UpdateSequenceNumber(content, clientInfo));
            }
        }
    }

    private static IDeltaContent UpdateSequenceNumber(IDeltaContent content, IClientInfo clientInfo)
    {
        if (content is IDeltaEvent deltaEvent)
        {
            deltaEvent.SequenceNumber = clientInfo.IncrementAndGetSequenceNumber();
        }

        return content;
    }

    /// <inheritdoc />
    public void ReceiveFromClient(IMessageContext<IDeltaContent> message) =>
        ReceivedFromClient?.Invoke(this, message);

    /// <inheritdoc />
    public IDeltaContent Convert(INotification notification)
    {
        var result = _mapper.Map(notification);
        if (notification.NotificationId is ParticipationNotificationId p && result.RequiresParticipationId)
            result.InternalParticipationId = p.ParticipationId;
        return result;
    }
}