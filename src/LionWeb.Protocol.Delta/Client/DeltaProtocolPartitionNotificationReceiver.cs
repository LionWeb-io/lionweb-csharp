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

using Core.M1;
using Core.M3;
using Core.Notification;
using Core.Notification.Partition;
using Message.Event;
using Partition;

public class DeltaProtocolPartitionNotificationReceiver
{
    private readonly PartitionNotificationHandler _notificationHandler;
    private readonly DeltaEventToPartitionNotificationMapper _mapper;

    public DeltaProtocolPartitionNotificationReceiver(
        PartitionNotificationHandler notificationHandler,
        SharedNodeMap sharedNodeMap,
        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _notificationHandler = notificationHandler;
        _mapper = new DeltaEventToPartitionNotificationMapper(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
    }


    public void Receive(IDeltaEvent deltaEvent)
    {
        IPartitionNotification partitionNotification = _mapper.Map(deltaEvent);

        _notificationHandler.Receive(partitionNotification);
    }
}