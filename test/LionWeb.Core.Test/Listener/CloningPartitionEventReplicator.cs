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

namespace LionWeb.Core.Test.Listener;

using M1.Event;
using M1.Event.Partition;
using M1.Event.Processor;

internal class CloningPartitionEventReplicator(
    IPartitionInstance localPartition,
    SharedNodeMap sharedNodeMap,
    EventIdFilteringEventProcessor<IPartitionEvent> filter,
    object? sender)
    : PartitionEventReplicator(localPartition, sharedNodeMap, filter, sender)
{
    public static new IEventProcessor<IPartitionEvent> Create(IPartitionInstance localPartition,
        SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localPartition.GetId();
        var filter = new EventIdFilteringEventProcessor<IPartitionEvent>(internalSender);
        var replicator = new CloningPartitionEventReplicator(localPartition, sharedNodeMap, filter, internalSender);
        var result = new CompositeEventProcessor<IPartitionEvent>([replicator, filter],
            sender ?? $"Composite of {nameof(CloningPartitionEventReplicator)} {localPartition.GetId()}");
        replicator.Init();
        return result;
    }

    protected override INode AdjustRemoteNode(INode remoteNode) =>
        SameIdCloner.Clone(remoteNode);
}