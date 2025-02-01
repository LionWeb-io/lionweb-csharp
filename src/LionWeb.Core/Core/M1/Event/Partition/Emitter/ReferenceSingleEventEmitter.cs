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

public class ReferenceSingleEventEmitter : ReferenceEventEmitterBase<INode>
{
    private readonly IReadableNode? _newTarget;
    private readonly IReadableNode? _oldTarget;

    /// Raises either <see cref="ReferenceAddedEvent"/>, <see cref="ReferenceDeletedEvent"/> or
    /// <see cref="ReferenceChangedEvent"/> for <paramref name="reference"/>,
    /// depending on <paramref name="oldTarget"/> and <paramref name="newTarget"/>.
    public ReferenceSingleEventEmitter(Reference reference, NodeBase newParent, IReadableNode? newTarget,
        IReadableNode? oldTarget) : base(reference, newParent)
    {
        _newTarget = newTarget;
        _oldTarget = oldTarget;
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        switch (_oldTarget, _newTarget)
        {
            case (null, { } v):
                IReferenceTarget newTarget = new ReferenceTarget(null, v);
                PartitionCommander.Raise(new ReferenceAddedEvent(NewParent, _reference, 0, newTarget,
                    PartitionCommander.CreateEventId()));
                break;
            case ({ } o, null):
                IReferenceTarget deletedTarget = new ReferenceTarget(null, o);
                PartitionCommander.Raise(new ReferenceDeletedEvent(NewParent, _reference, 0, deletedTarget,
                    PartitionCommander.CreateEventId()));
                break;
            case ({ } o, { } n):
                IReferenceTarget replacedTarget = new ReferenceTarget(null, o);
                PartitionCommander.Raise(new ReferenceChangedEvent(NewParent, _reference, 0,
                    new ReferenceTarget(null, n), replacedTarget,
                    PartitionCommander.CreateEventId()));
                break;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(ReferenceAddedEvent),
            typeof(ReferenceDeletedEvent),
            typeof(ReferenceChangedEvent)
        );
}