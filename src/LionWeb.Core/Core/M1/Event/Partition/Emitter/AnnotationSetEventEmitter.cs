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

/// Encapsulates event-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Annotation"/>s.
public class AnnotationSetEventEmitter : AnnotationEventEmitterBase
{
    private readonly List<IListChange<INode>> _changes = [];

    /// <param name="destinationParent"> Owner of the represented <see cref="Annotation"/>s.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <see cref="IReadableNode.GetAnnotations"/>.</param>
    /// <param name="eventId">The event ID of the event emitted by this event emitter</param>
    public AnnotationSetEventEmitter(NodeBase destinationParent,
        List<INode>? setValues,
        List<INode> existingValues,
        IEventId? eventId = null) : base(destinationParent, setValues, eventId)
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
                case ListAdded<INode> added:
                    switch (NewValues[added.Element])
                    {
                        case null:
                            PartitionProcessor.Receive(new AnnotationAddedEvent(DestinationParent, added.Element,
                                added.RightIndex, GetEventId()));
                            break;

                        case { } old when old.Parent != DestinationParent:
                            var eventId = GetEventId();
                            var @event = new AnnotationMovedFromOtherParentEvent(DestinationParent, added.RightIndex,
                                added.Element, old.Parent, old.Index, eventId);
                            RaiseOriginMoveEvent(old, @event);
                            PartitionProcessor.Receive(@event);
                            break;


                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case ListMoved<INode> moved:
                    PartitionProcessor.Receive(new AnnotationMovedInSameParentEvent(moved.RightIndex, moved.LeftElement,
                        DestinationParent, moved.LeftIndex, GetEventId()));
                    break;

                case ListDeleted<INode> deleted:
                    PartitionProcessor.Receive(new AnnotationDeletedEvent(deleted.Element, DestinationParent, deleted.LeftIndex,
                        GetEventId()));
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionProcessor))]
    protected override bool IsActive() =>
        PartitionProcessor != null && PartitionProcessor.CanReceive(
            typeof(AnnotationAddedEvent),
            typeof(AnnotationDeletedEvent),
            typeof(AnnotationMovedFromOtherParentEvent),
            typeof(AnnotationMovedInSameParentEvent)
        );
}