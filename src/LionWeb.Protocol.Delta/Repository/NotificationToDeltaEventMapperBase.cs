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
using Core.M1;
using Core.M3;
using Core.Notification;
using Message;
using Message.Event;

public abstract class NotificationToDeltaEventMapperBase
{
    private IParticipationIdProvider _participationIdProvider;
    private LionWebVersions _lionWebVersion;
    private ISerializerVersionSpecifics _propertySerializer;

    protected NotificationToDeltaEventMapperBase(IParticipationIdProvider participationIdProvider,
        LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        _participationIdProvider = participationIdProvider;
        _propertySerializer = ISerializerVersionSpecifics.Create(lionWebVersion);
    }

    protected PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    protected DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }

    protected TargetNode[] ToDescendants(IReadableNode node) =>
        M1Extensions.Descendants(node, false, true).Select(n => n.GetId()).ToArray();

    protected CommandSource[] ToCommandSources(INotification notification)
    {
        ParticipationId participationId;
        EventId commandId;
        if (notification.NotificationId is ParticipationNotificationId pei)
        {
            participationId = pei.ParticipationId;
            commandId = pei.CommandId;
        } else
        {
            participationId = _participationIdProvider.ParticipationId;
            commandId = notification.NotificationId.ToString();
        }

        return [new CommandSource(participationId, commandId)];
    }
}

public interface IParticipationIdProvider
{
    ParticipationId ParticipationId { get; }
}