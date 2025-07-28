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

using M3;
using System.Diagnostics.CodeAnalysis;
using Utilities.ListComparer;

/// Encapsulates event-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public class ContainmentSetEventEmitter<T> : ContainmentMultipleEventEmitterBase<T> where T : INode
{
    private readonly List<IListChange<T>> _changes = [];

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <paramref name="containment"/>.</param>
    /// <param name="eventId"></param>
    public ContainmentSetEventEmitter(Containment containment,
        NodeBase destinationParent,
        List<T>? setValues,
        List<T> existingValues, IEventId? eventId = null) : base(containment, destinationParent, setValues, eventId)
    {
        if (!IsActive() || setValues == null)
            return;

        var listComparer = IListComparer.CreateForNodes(existingValues, setValues);
        _changes = listComparer.Compare();
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        foreach (var change in _changes)
        {
            switch (change)
            {
                case ListAdded<T> added:
                    switch (NewValues[added.Element])
                    {
                        case null:
                            PartitionCommander.Raise(new ChildAddedEvent(DestinationParent, added.Element, Containment,
                                added.RightIndex, GetEventId()));
                            break;

                        case { } old when old.Parent != DestinationParent:
                            var eventId = GetEventId();
                            var @event = new ChildMovedFromOtherContainmentEvent(DestinationParent, Containment,
                                added.RightIndex, added.Element, old.Parent, old.Containment, old.Index, eventId);
                            RaiseOriginMoveEvent(old, @event);
                            PartitionCommander.Raise(@event);
                            break;


                        case { } old when old.Parent == DestinationParent && old.Containment != Containment:
                            PartitionCommander.Raise(new ChildMovedFromOtherContainmentInSameParentEvent(Containment,
                                added.RightIndex, added.Element, DestinationParent, old.Containment, old.Index,
                                GetEventId()));
                            break;

                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case ListMoved<T> moved:
                    PartitionCommander.Raise(new ChildMovedInSameContainmentEvent(moved.RightIndex, moved.LeftElement,
                        DestinationParent, Containment, moved.LeftIndex, GetEventId()));
                    break;
                case ListDeleted<T> deleted:
                    PartitionCommander.Raise(new ChildDeletedEvent(deleted.Element, DestinationParent, Containment,
                        deleted.LeftIndex, GetEventId()));
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(ChildAddedEvent),
            typeof(ChildDeletedEvent),
            typeof(ChildMovedFromOtherContainmentEvent),
            typeof(ChildMovedFromOtherContainmentInSameParentEvent),
            typeof(ChildMovedInSameContainmentEvent)
        );
}