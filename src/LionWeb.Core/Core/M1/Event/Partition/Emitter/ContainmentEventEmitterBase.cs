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

namespace LionWeb.Core.M1.Event.Partition.Emitter;

using M2;
using M3;

/// Encapsulates event-related logic and data for <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public abstract class ContainmentEventEmitterBase<T> : PartitionEventEmitterBase<T> where T : INode
{
    /// Represented <see cref="Containment"/>.
    protected readonly Containment Containment;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    protected ContainmentEventEmitterBase(Containment containment, NodeBase destinationParent) : base(destinationParent)
    {
        Containment = containment;
    }

    /// Collects <see cref="OldContainmentInfo"/> from <paramref name="value"/>, to be used in <see cref="PartitionEventEmitterBase{T}.CollectOldData"/>
    protected OldContainmentInfo? Collect(T value)
    {
        var oldParent = value.GetParent();
        if (oldParent == null)
            return null;

        var oldContainment = oldParent.GetContainmentOf(value);
        if (oldContainment == null)
            return null;

        var oldIndex = oldContainment.Multiple
            ? M2Extensions.AsNodes<INode>(oldParent.Get(oldContainment)).ToList().IndexOf(value)
            : 0;

        var oldPartition = value.GetPartition();

        return new OldContainmentInfo(oldParent, oldContainment, oldIndex, oldPartition);
    }

    /// Context of a node before it has been removed from its previous <paramref name="Parent"/>.
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    protected record OldContainmentInfo(
        INode Parent,
        Containment Containment,
        Index Index,
        IPartitionInstance? Partition);

    protected void RaiseOriginMoveEvent(OldContainmentInfo old, ChildMovedFromOtherContainmentEvent @event)
    {
        if (old.Partition != null && old.Partition != DestinationPartition)
            old.Partition.GetCommander()?.Raise(@event);
    }
}