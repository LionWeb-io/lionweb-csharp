// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Protocol.Delta.Test.Pipe.TimeDependent;

using Core;
using Core.M1;
using Core.M3;
using Core.Notification;
using Core.Notification.Pipe;
using Delta.Client;
using Delta.Repository;
using Message.Command;
using Message.Event;

public class NotificationDeltaEventMapper(SharedNodeMap sharedNodeMap, SharedKeyedMap sharedKeyedMap, LionWebVersions lionWebVersion, IEnumerable<Language> languages) : NotificationPipeBase, INotificationHandler
{
    private readonly NotificationToDeltaEventMapper _toDeltaMapper = new(new ParticipationIdProvider(), lionWebVersion);
    private readonly DeltaEventToNotificationMapper _toNotificationMapper = new(sharedNodeMap, sharedKeyedMap, new DeserializerBuilder().WithLionWebVersion(lionWebVersion).WithLanguages(languages));

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        var delta = _toDeltaMapper.Map(notification);
        var mapped = _toNotificationMapper.Map(delta);
        Send(mapped);
    }
}

public class NotificationDeltaCommandMapper(SharedNodeMap sharedNodeMap, SharedKeyedMap sharedKeyedMap, LionWebVersions lionWebVersion, IEnumerable<Language> languages) : NotificationPipeBase, INotificationHandler
{
    private readonly NotificationToDeltaCommandMapper _toDeltaMapper = new(new CommandIdProvider(), lionWebVersion);
    private readonly DeltaCommandToNotificationMapper _toNotificationMapper = new(sharedNodeMap, sharedKeyedMap, new DeserializerBuilder().WithLionWebVersion(lionWebVersion).WithLanguages(languages));

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        var delta = _toDeltaMapper.Map(notification);
        var mapped = _toNotificationMapper.Map(delta);
        Send(mapped);
    }
}

public class NotificationDeltaEventSerializationMapper(SharedNodeMap sharedNodeMap, SharedKeyedMap sharedKeyedMap, LionWebVersions lionWebVersion, IEnumerable<Language> languages) : NotificationPipeBase, INotificationHandler
{
    private readonly NotificationToDeltaEventMapper _toDeltaMapper = new(new ParticipationIdProvider(), lionWebVersion);
    private readonly DeltaEventToNotificationMapper _toNotificationMapper = new(sharedNodeMap, sharedKeyedMap, new DeserializerBuilder().WithLionWebVersion(lionWebVersion).WithLanguages(languages));
    private readonly DeltaSerializer _deltaSerializer = new ();

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        var delta = _toDeltaMapper.Map(notification);
        var json = _deltaSerializer.Serialize(delta);
        var deserialized = _deltaSerializer.Deserialize<IDeltaEvent>(json);
        var mapped = _toNotificationMapper.Map(deserialized);
        Send(mapped);
    }
}

public class NotificationDeltaCommandSerializationMapper(SharedNodeMap sharedNodeMap, SharedKeyedMap sharedKeyedMap, LionWebVersions lionWebVersion, IEnumerable<Language> languages) : NotificationPipeBase, INotificationHandler
{
    private readonly NotificationToDeltaCommandMapper _toDeltaMapper = new(new CommandIdProvider(), lionWebVersion);
    private readonly DeltaCommandToNotificationMapper _toNotificationMapper = new(sharedNodeMap, sharedKeyedMap, new DeserializerBuilder().WithLionWebVersion(lionWebVersion).WithLanguages(languages));
    private readonly DeltaSerializer _deltaSerializer = new ();

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        var delta = _toDeltaMapper.Map(notification);
        var json = _deltaSerializer.Serialize(delta);
        var deserialized = _deltaSerializer.Deserialize<IDeltaCommand>(json);
        var mapped = _toNotificationMapper.Map(deserialized);
        Send(mapped);
    }
}