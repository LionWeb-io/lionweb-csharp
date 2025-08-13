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

namespace LionWeb.Protocol.Delta.Repository.Forest;

using Core;
using Core.Notification.Forest;
using Message.Event;

public class ForestNotificationToDeltaEventMapper(
    IParticipationIdProvider participationIdProvider,
    LionWebVersions lionWebVersion)
    : NotificationToDeltaEventMapperBase(participationIdProvider, lionWebVersion)
{
    public IDeltaEvent Map(IForestNotification forestNotification) =>
        forestNotification switch
        {
            PartitionAddedNotification a => OnPartitionAdded(a),
            PartitionDeletedNotification a => OnPartitionDeleted(a),
            _ => throw new NotImplementedException(forestNotification.GetType().Name)
        };

    private PartitionAdded OnPartitionAdded(PartitionAddedNotification partitionAddedNotification) =>
        new(
            ToDeltaChunk(partitionAddedNotification.NewPartition),
            ToCommandSources(partitionAddedNotification),
            []
        );

    private PartitionDeleted OnPartitionDeleted(PartitionDeletedNotification partitionDeletedNotification) =>
        new(
            partitionDeletedNotification.DeletedPartition.GetId(),
            ToDescendants(partitionDeletedNotification.DeletedPartition),
            ToCommandSources(partitionDeletedNotification),
            []
        );
}