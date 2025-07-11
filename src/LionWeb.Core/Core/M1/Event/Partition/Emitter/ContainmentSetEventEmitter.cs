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
using Utilities;

/// Encapsulates event-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public class ContainmentSetEventEmitter<T> : ContainmentMultipleEventEmitterBase<T> where T : INode
{
    private readonly List<IListComparer<T>.IChange> _changes = [];

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <paramref name="containment"/>.</param>
    public ContainmentSetEventEmitter(
        Containment containment,
        NodeBase destinationParent,
        List<T>? setValues,
        List<T> existingValues
    ) : base(containment, destinationParent, setValues)
    {
        if (!IsActive() || setValues == null)
            return;

        var listComparer = new MoveDetector<T>(new RelativeChangesListComparer<T>(existingValues, setValues).Compare());
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
                case IListComparer<T>.Added added:
                    switch (NewValues[added.Element])
                    {
                        case null:
                            PartitionCommander.Raise(new ChildAddedEvent(DestinationParent, added.Element, Containment,
                                added.RightIndex, PartitionCommander.CreateEventId()));
                            break;

                        case { } old when old.Parent != DestinationParent:
                            var eventId = PartitionCommander.CreateEventId();
                            var @event = new ChildMovedFromOtherContainmentEvent(DestinationParent, Containment,
                                added.RightIndex, added.Element, old.Parent, old.Containment, old.Index, eventId);
                            RaiseOriginMoveEvent(old, @event);
                            PartitionCommander.Raise(@event);
                            break;


                        case { } old when old.Parent == DestinationParent && old.Containment != Containment:
                            PartitionCommander.Raise(new ChildMovedFromOtherContainmentInSameParentEvent(Containment,
                                added.RightIndex, added.Element, DestinationParent, old.Containment, old.Index,
                                PartitionCommander.CreateEventId()));
                            break;

                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case IListComparer<T>.Moved moved:
                    PartitionCommander.Raise(new ChildMovedInSameContainmentEvent(moved.RightIndex, moved.LeftElement,
                        DestinationParent, Containment, moved.LeftIndex, PartitionCommander.CreateEventId()));
                    break;
                case IListComparer<T>.Deleted deleted:
                    PartitionCommander.Raise(new ChildDeletedEvent(deleted.Element, DestinationParent, Containment,
                        deleted.LeftIndex, PartitionCommander.CreateEventId()));
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