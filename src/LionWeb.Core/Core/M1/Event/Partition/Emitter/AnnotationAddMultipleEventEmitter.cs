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

#region Annotation

/// Encapsulates event-related logic and data for <i>adding</i> or <i>inserting</i> of <see cref="Annotation"/>s.
public class AnnotationAddMultipleEventEmitter : AnnotationEventEmitterBase
{
    private Index _newIndex;

    /// <param name="newParent"> Owner of the represented <see cref="Annotation"/>.</param>
    /// <param name="addedValues">Newly added values.</param>
    /// <param name="existingValues">Values already present in <see cref="IReadableNode.GetAnnotations"/>.</param>
    /// <param name="startIndex">Optional index where we add <paramref name="addedValues"/> to <see cref="Annotation"/>s.</param>
    public AnnotationAddMultipleEventEmitter(
        NodeBase newParent,
        List<INode>? addedValues,
        List<INode> existingValues,
        Index? startIndex = null
    ) : base(newParent, addedValues)
    {
        _newIndex = startIndex ?? Math.Max(existingValues.Count - 1, 0);
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        foreach ((INode? added, OldAnnotationInfo? old) in NewValues)
        {
            switch (old)
            {
                case null:
                    PartitionCommander.AddAnnotation(NewParent, added, _newIndex);
                    break;

                case not null when old.Parent != NewParent:
                    var eventId = PartitionCommander.CreateEventId();
                    RaiseOriginMoveEvent(old, eventId, added, _newIndex);
                    PartitionCommander.MoveAnnotationFromOtherParent(
                        NewParent,
                        _newIndex,
                        added,
                        old.Parent,
                        old.Index,
                        eventId
                    );
                    break;


                case not null when old.Parent == NewParent && old.Index == _newIndex:
                    // no-op
                    break;

                case not null when old.Parent == NewParent:
                    PartitionCommander.MoveAnnotationInSameParent(
                        _newIndex,
                        added,
                        NewParent,
                        old.Index
                    );
                    break;

                default:
                    throw new ArgumentException("Unknown state");
            }

            _newIndex++;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(AnnotationAddedEvent),
            typeof(AnnotationMovedFromOtherParentEvent),
            typeof(AnnotationMovedInSameParentEvent)
        );
}

#endregion