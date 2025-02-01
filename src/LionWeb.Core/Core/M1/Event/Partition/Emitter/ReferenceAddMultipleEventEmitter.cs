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

public class ReferenceAddMultipleEventEmitter<T> : ReferenceMultipleEventEmitterBase<T> where T : IReadableNode
{
    private readonly Index _startIndex;

    /// Raises <see cref="ReferenceAddedEvent"/> for <paramref name="reference"/> for each entry in <paramref name="safeNodes"/>.
    /// <param name="reference">Reference to raise events for.</param>
    /// <param name="safeNodes">Targets to raise events for.</param>
    /// <param name="startIndex">Index where we add <paramref name="safeNodes"/> to <paramref name="reference"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="reference"/>.</typeparam>
    public ReferenceAddMultipleEventEmitter(Reference reference, NodeBase newParent, List<T> safeNodes,
        Index startIndex) : base(reference, newParent, safeNodes)
    {
        _startIndex = startIndex;
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        Index index = _startIndex;
        foreach (var node in _safeNodes)
        {
            IReferenceTarget newTarget = new ReferenceTarget(null, node);
            PartitionCommander.Raise(new ReferenceAddedEvent(NewParent, _reference, index++, newTarget,
                PartitionCommander.CreateEventId()));
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(ReferenceAddedEvent)
        );
}