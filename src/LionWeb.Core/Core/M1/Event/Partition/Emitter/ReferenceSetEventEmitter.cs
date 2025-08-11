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

/// Encapsulates event-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Reference"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Reference"/>.</typeparam>
public class ReferenceSetEventEmitter<T> : ReferenceMultipleEventEmitterBase<T> where T : IReadableNode
{
    private readonly List<IListChange<T>> _changes = [];

    /// <param name="reference">Represented <see cref="Reference"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="reference"/>.</param>
    /// <param name="safeNodes">Newly added values.</param>
    /// <param name="storage">Values already present in <paramref name="reference"/>.</param>
    /// <param name="eventId">The event ID of the event emitted by this event emitter</param>
    public ReferenceSetEventEmitter(Reference reference, NodeBase destinationParent, List<T> safeNodes, List<T> storage,
        IEventId? eventId = null) :
        base(reference, destinationParent, safeNodes, eventId)
    {
        if (!IsActive())
            return;

        var listComparer = IListComparer.CreateForNodes(storage, safeNodes);
        _changes = listComparer.Compare();
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

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
                    IReferenceTarget newTarget = new ReferenceTarget(null, added.Element);
                    PartitionProcessor.Receive(new ReferenceAddedEvent(DestinationParent, Reference, added.RightIndex, newTarget,
                        GetEventId()));
                    break;
                case ListMoved<T> moved:
                    IReferenceTarget target = new ReferenceTarget(null, moved.LeftElement);
                    PartitionProcessor.Receive(new EntryMovedInSameReferenceEvent(DestinationParent, Reference, moved.RightIndex,
                        moved.LeftIndex, target,
                        GetEventId()));
                    break;
                case ListDeleted<T> deleted:
                    IReferenceTarget deletedTarget = new ReferenceTarget(null, deleted.Element);
                    PartitionProcessor.Receive(new ReferenceDeletedEvent(DestinationParent, Reference, deleted.LeftIndex,
                        deletedTarget, GetEventId()));
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionProcessor))]
    protected override bool IsActive() =>
        PartitionProcessor != null && PartitionProcessor.CanReceive(
            typeof(ReferenceAddedEvent),
            typeof(EntryMovedInSameReferenceEvent),
            typeof(ReferenceDeletedEvent)
        );
}