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

namespace LionWeb.Protocol.Delta.Test.Pipe.TimeDependent;

using Core;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Test.Notification.Replicator;

[TestClass]
public class TimeDependentNotificationToNotificationTests : TimeDependentReplicatorTestsBase
{
    protected override void Connect()
    {
        var sharedNodeMap = new SharedNodeMap();

        var notificationMapper = new NotificationMapper(sharedNodeMap);
        _originalForest.GetNotificationSender()!.ConnectTo(_compositor);

        var replicator = ForestReplicator.Create(_clonedForest, sharedNodeMap);
        notificationMapper.ConnectTo(replicator);

        _compositor.ConnectTo(notificationMapper);
    }
}

[TestClass]
public class TimeDependentNotificationToEventTests : TimeDependentReplicatorTestsBase
{
    protected override void Connect()
    {
        var sharedNodeMap = new SharedNodeMap();

        var notificationMapper = new NotificationDeltaEventMapper(sharedNodeMap, SharedKeyedMapBuilder.BuildSharedKeyMap(_languages), _lionWebVersions, _languages);
        _originalForest.GetNotificationSender()!.ConnectTo(_compositor);

        var replicator = ForestReplicator.Create(_clonedForest, sharedNodeMap);
        notificationMapper.ConnectTo(replicator);

        _compositor.ConnectTo(notificationMapper);
    }
}

[TestClass]
public class TimeDependentNotificationToEventSerializationTests : TimeDependentReplicatorTestsBase
{
    protected override void Connect()
    {
        var sharedNodeMap = new SharedNodeMap();

        var notificationMapper = new NotificationDeltaEventSerializationMapper(sharedNodeMap, SharedKeyedMapBuilder.BuildSharedKeyMap(_languages), _lionWebVersions, _languages);
        _originalForest.GetNotificationSender()!.ConnectTo(_compositor);

        var replicator = ForestReplicator.Create(_clonedForest, sharedNodeMap);
        notificationMapper.ConnectTo(replicator);

        _compositor.ConnectTo(notificationMapper);
    }
}

[TestClass]
public class TimeDependentNotificationToCommandTests : TimeDependentReplicatorTestsBase
{
    protected override void Connect()
    {
        var sharedNodeMap = new SharedNodeMap();

        var notificationMapper = new NotificationDeltaCommandMapper(sharedNodeMap, SharedKeyedMapBuilder.BuildSharedKeyMap(_languages), _lionWebVersions, _languages);
        _originalForest.GetNotificationSender()!.ConnectTo(_compositor);

        var replicator = ForestReplicator.Create(_clonedForest, sharedNodeMap);
        notificationMapper.ConnectTo(replicator);

        _compositor.ConnectTo(notificationMapper);
    }
}

[TestClass]
public class TimeDependentNotificationToCommandSerializationTests : TimeDependentReplicatorTestsBase
{
    protected override void Connect()
    {
        var sharedNodeMap = new SharedNodeMap();

        var notificationMapper = new NotificationDeltaCommandSerializationMapper(sharedNodeMap, SharedKeyedMapBuilder.BuildSharedKeyMap(_languages), _lionWebVersions, _languages);
        _originalForest.GetNotificationSender()!.ConnectTo(_compositor);

        var replicator = ForestReplicator.Create(_clonedForest, sharedNodeMap);
        notificationMapper.ConnectTo(replicator);

        _compositor.ConnectTo(notificationMapper);
    }
}