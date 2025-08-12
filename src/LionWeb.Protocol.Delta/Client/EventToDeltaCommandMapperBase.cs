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

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Partition;
using Core.M3;
using Core.Notification;
using Core.Notification.Partition;
using Core.Serialization;
using Message;

public abstract class EventToDeltaCommandMapperBase
{
    private ICommandIdProvider _commandIdProvider;
    private LionWebVersions _lionWebVersion;
    private ISerializerVersionSpecifics _propertySerializer;

    protected EventToDeltaCommandMapperBase(ICommandIdProvider commandIdProvider, LionWebVersions lionWebVersion)
    {
        _commandIdProvider = commandIdProvider;
        _lionWebVersion = lionWebVersion;
        _propertySerializer = ISerializerVersionSpecifics.Create(lionWebVersion);
    }

    protected PropertyValue? ToDelta(IReadableNode parent, Property property, Object newValue) =>
        _propertySerializer.SerializeProperty(parent, property, newValue).Value;

    private SerializedReferenceTarget ToDelta(IReferenceTarget target) =>
        new SerializedReferenceTarget { Reference = target.Reference?.GetId(), ResolveInfo = target.ResolveInfo };

    protected DeltaSerializationChunk ToDeltaChunk(IReadableNode node)
    {
        var serializer = new Serializer(_lionWebVersion);
        return new DeltaSerializationChunk(serializer.Serialize(M1Extensions.Descendants(node, true, true)).ToArray());
    }

    protected CommandId ToCommandId(INotification @event) =>
        @event.NotificationId switch
        {
            ParticipationNotificationId pei => pei.CommandId,
            _ => _commandIdProvider.Create()
        };
}

public interface ICommandIdProvider
{
    CommandId Create();
}