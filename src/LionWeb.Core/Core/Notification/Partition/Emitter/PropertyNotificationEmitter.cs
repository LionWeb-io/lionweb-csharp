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

namespace LionWeb.Core.Notification.Partition.Emitter;

using M3;
using System.Diagnostics.CodeAnalysis;

/// Encapsulates notification-related logic and data for <i>adding</i> or <i>changing</i> or <i>deleting</i> of <see cref="Property"/>s.
public class PropertyNotificationEmitter : PartitionNotificationEmitterBase<INode>
{
    private readonly Property _property;
    private readonly object? _newValue;
    private readonly object? _oldValue;

    /// Raises either <see cref="PropertyAddedNotification"/>, <see cref="PropertyDeletedNotification"/> or
    /// <see cref="PropertyChangedNotification"/> for <paramref name="property"/>,
    /// depending on <paramref name="oldValue"/> and <paramref name="newValue"/>.
    public PropertyNotificationEmitter(Property property, INotifiableNode destinationParent, object? newValue, object? oldValue,
        INotificationId? notificationId = null) :
        base(destinationParent, notificationId)
    {
        _property = property;
        _newValue = newValue;
        _oldValue = oldValue;
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        switch (_oldValue, _newValue)
        {
            case (null, { } v):
                PartitionCommander.Raise(new PropertyAddedNotification(DestinationParent, _property, v,
                    GetNotificationId()));
                break;
            case ({ } o, null):
                PartitionCommander.Raise(new PropertyDeletedNotification(DestinationParent, _property, o,
                    GetNotificationId()));
                break;
            case ({ } o, { } n):
                PartitionCommander.Raise(new PropertyChangedNotification(DestinationParent, _property, n, o,
                    GetNotificationId()));
                break;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(PropertyAddedNotification),
            typeof(PropertyDeletedNotification),
            typeof(PropertyChangedNotification)
        );
}