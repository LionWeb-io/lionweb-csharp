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

using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Forest;
using Message.Event;
using Partition;

public class NotificationToDeltaEventMapper
{
    private readonly PartitionNotificationToDeltaEventMapper _partitionNotificationToDeltaMapper;
    private readonly ForestNotificationToDeltaEventMapper _forestNotificationToDeltaMapper;

    public NotificationToDeltaEventMapper(PartitionNotificationToDeltaEventMapper partitionNotificationToDeltaMapper,
        ForestNotificationToDeltaEventMapper forestNotificationToDeltaMapper)
    {
        _partitionNotificationToDeltaMapper = partitionNotificationToDeltaMapper;
        _forestNotificationToDeltaMapper = forestNotificationToDeltaMapper;
    }

    public IDeltaEvent Map(INotification notification) =>
        notification switch
        {
            IPartitionNotification e => _partitionNotificationToDeltaMapper.Map(e),
            IForestNotification e => _forestNotificationToDeltaMapper.Map(e),
            _ => throw new NotImplementedException(notification.GetType().Name)
        };
}