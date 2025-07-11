﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M1.Event.Partition.Emitter;

using M3;
using System.Diagnostics.CodeAnalysis;

/// Encapsulates event-related logic and data to execute
/// <see cref="CollectOldData">before</see> and <see cref="RaiseEvent">after</see>
/// the actual manipulation of the underlying nodes for one specific <see cref="Feature"/>.
/// <typeparam name="T">Type of nodes of the represented <see cref="Feature"/>.</typeparam>
public abstract class PartitionEventEmitterBase<T> where T : IReadableNode
{
    protected readonly IPartitionInstance? DestinationPartition;

    /// <see cref="IPartitionCommander"/> to use for our events, if any.
    protected readonly IPartitionCommander? PartitionCommander;

    /// Owner of the represented <see cref="Feature"/>.
    protected readonly NodeBase DestinationParent;

    /// <param name="destinationParent"> Owner of the represented <see cref="Feature"/>.</param>
    protected PartitionEventEmitterBase(NodeBase destinationParent)
    {
        DestinationParent = destinationParent;
        DestinationPartition = destinationParent.GetPartition();
        PartitionCommander = DestinationPartition?.GetCommander();
    }

    /// Logic to execute <i>before</i> any changes to the underlying nodes.
    public abstract void CollectOldData();

    /// Logic to execute <i>after</i> any changes to the underlying nodes.
    public abstract void RaiseEvent();

    /// <summary>
    /// Whether this event should execute at all.
    /// </summary>
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected abstract bool IsActive();
}