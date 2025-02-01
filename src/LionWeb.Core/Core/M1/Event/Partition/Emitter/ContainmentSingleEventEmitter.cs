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

/// Encapsulates event-related logic and data for changing <i>single</i> <see cref="Containment"/>s.
/// <typeparam name="T">Type of node of the represented <see cref="Containment"/>.</typeparam>
public class ContainmentSingleEventEmitter<T> : ContainmentEventEmitterBase<T> where T : INode
{
    private readonly T? _newValue;
    private readonly T? _oldValue;

    private OldContainmentInfo? _oldContainmentInfo;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="newParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="newValue">Newly set value.</param>
    /// <param name="oldValue">Previous value of <paramref name="containment"/>.</param>
    public ContainmentSingleEventEmitter(Containment containment, NodeBase newParent, T? newValue, T? oldValue)
        : base(containment, newParent)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive() || _newValue == null)
            return;

        OldContainmentInfo? oldInfo = Collect(_newValue);
        if (oldInfo == null)
            return;

        _oldContainmentInfo = oldInfo;
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        switch (_oldValue, _newValue, _oldContainmentInfo)
        {
            case (null, null, _):
            // fall-through
            case (not null, not null, _) when Equals(_oldValue, _newValue):
                // no-op
                break;

            case (not null, null, _):
                PartitionCommander.DeleteChild(
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                );
                break;

            case (null, not null, null):
                PartitionCommander.AddChild(
                    NewParent,
                    _newValue,
                    Containment,
                    0
                );
                break;

            case (not null, not null, null):
                PartitionCommander.ReplaceChild(
                    _newValue,
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                );
                break;

            case (null, not null, not null)
                when _oldContainmentInfo.Parent == NewParent && _oldContainmentInfo.Containment != Containment:
                PartitionCommander.MoveChildFromOtherContainmentInSameParent(
                    Containment,
                    0,
                    _newValue,
                    NewParent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index
                );
                break;

            case (not null, not null, not null)
                when _oldContainmentInfo.Parent == NewParent && _oldContainmentInfo.Containment != Containment:
                PartitionCommander.DeleteChild(
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                );
                PartitionCommander.MoveChildFromOtherContainmentInSameParent(
                    Containment,
                    0,
                    _newValue,
                    NewParent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index
                );
                break;

            case (not null, not null, not null)
                when _oldContainmentInfo.Parent != NewParent:
                PartitionCommander.DeleteChild(
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                );
                PartitionCommander.MoveChildFromOtherContainment(
                    NewParent,
                    Containment,
                    0,
                    _newValue,
                    _oldContainmentInfo.Parent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index
                );
                break;

            case (null, not null, not null)
                when _oldContainmentInfo.Parent != NewParent:
                var eventId = PartitionCommander.CreateEventId();
                RaiseOriginMoveEvent(_oldContainmentInfo, eventId, _newValue, 0);
                PartitionCommander.MoveChildFromOtherContainment(
                    NewParent,
                    Containment,
                    0,
                    _newValue,
                    _oldContainmentInfo.Parent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index,
                    eventId
                );
                break;

            default:
                throw new ArgumentException("Unknown state");
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    [MemberNotNullWhen(true, nameof(_oldContainmentInfo))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(ChildAddedEvent),
            typeof(ChildDeletedEvent),
            typeof(ChildReplacedEvent),
            typeof(ChildMovedFromOtherContainmentEvent),
            typeof(ChildMovedFromOtherContainmentInSameParentEvent),
            typeof(ChildMovedInSameContainmentEvent)
        );
}
