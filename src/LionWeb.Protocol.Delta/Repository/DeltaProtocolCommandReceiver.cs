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

using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Notification.Processor;
using Forest;
using Message.Command;
using Partition;

public class DeltaProtocolCommandReceiver 
    : DeltaProtocolReceiverBase<IDeltaCommand, IPartitionDeltaCommand, IForestDeltaCommand>
{
    private readonly DeltaCommandToForestEventMapper _forestMapper;
    private readonly DeltaCommandToPartitionEventMapper _partitionMapper;

    public DeltaProtocolCommandReceiver(
        PartitionSharedNodeMap sharedNodeMap,
        SharedPartitionReplicatorMap sharedPartitionReplicatorMap,
        SharedKeyedMap sharedKeyedMap,
        DeserializerBuilder deserializerBuilder,
        INotificationProcessor<IForestNotification> forestNotificationReplicator)
        : base(sharedNodeMap, sharedPartitionReplicatorMap, forestNotificationReplicator)
    {
        _forestMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
        _partitionMapper = new(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
    }

    /// <inheritdoc />
    protected override IPartitionNotification MapPartition(IPartitionDeltaCommand partitionContent) =>
        _partitionMapper.Map(partitionContent);

    /// <inheritdoc />
    protected override IForestNotification MapForest(IForestDeltaCommand forestContent) =>
        _forestMapper.Map(forestContent);
}