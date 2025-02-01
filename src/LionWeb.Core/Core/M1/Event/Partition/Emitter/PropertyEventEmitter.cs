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

public class PropertyEventEmitter : PartitionEventEmitterBase<INode>
{
    private readonly Property _property;
    private readonly object? _newValue;
    private readonly object? _oldValue;

    /// Raises either <see cref="IPartitionCommander.AddProperty"/>, <see cref="IPartitionCommander.DeleteProperty"/> or
    /// <see cref="IPartitionCommander.ChangeProperty"/> for <paramref name="property"/>,
    /// depending on <paramref name="oldValue"/> and <paramref name="newValue"/>.
    public PropertyEventEmitter(Property property, NodeBase newParent, object? newValue, object? oldValue) :
        base(newParent)
    {
        _property = property;
        _newValue = newValue;
        _oldValue = oldValue;
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        switch (_oldValue, _newValue)
        {
            case (null, { } v):
                PartitionCommander.Raise(new PropertyAddedEvent(NewParent, _property, v,
                    PartitionCommander.CreateEventId()));
                break;
            case ({ } o, null):
                PartitionCommander.Raise(new PropertyDeletedEvent(NewParent, _property, o,
                    PartitionCommander.CreateEventId()));
                break;
            case ({ } o, { } n):
                PartitionCommander.Raise(new PropertyChangedEvent(NewParent, _property, n, o,
                    PartitionCommander.CreateEventId()));
                break;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(PropertyAddedEvent),
            typeof(PropertyDeletedEvent),
            typeof(PropertyChangedEvent)
        );
}