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

/// Encapsulates notification-related logic and data for <i>adding</i> or <i>inserting</i> of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public class ContainmentAddMultipleNotificationEmitter<T> : ContainmentMultipleNotificationEmitterBase<T> where T : INode
{
    private readonly List<T> _existingValues;
    private Index _newIndex;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="addedValues">Newly added values.</param>
    /// <param name="existingValues">Values already present in <paramref name="containment"/>.</param>
    /// <param name="startIndex">Optional index where we add <paramref name="addedValues"/> to <paramref name="containment"/>.</param>
    /// <param name="notificationId">The notification ID of the notification emitted by this notification emitter.</param>
    public ContainmentAddMultipleNotificationEmitter(Containment containment,
        INotifiableNode destinationParent,
        List<T>? addedValues,
        List<T> existingValues,
        Index? startIndex = null,
        INotificationId? notificationId = null) : base(containment, destinationParent, addedValues, notificationId)
    {
        _existingValues = existingValues;
        _newIndex = startIndex ?? Math.Max(existingValues.Count, 0);
    }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        foreach ((T? added, OldContainmentInfo? old) in NewValues)
        {
            switch (old)
            {
                case null:
                    PartitionCommander.Raise(new ChildAddedNotification(DestinationParent, added, Containment, _newIndex,
                        GetNotificationId()));
                    break;

                case not null when old.Parent != DestinationParent:
                    var notificationId = GetNotificationId();
                    var notification = new ChildMovedFromOtherContainmentNotification(DestinationParent, Containment, _newIndex, added,
                        old.Parent, old.Containment, old.Index, notificationId);
                    RaiseOriginMoveNotification(old, notification);
                    PartitionCommander.Raise(notification);
                    break;

                case not null when old.Parent == DestinationParent && old.Containment == Containment && old.Index == _newIndex:
                    // no-op
                    break;

                case not null when old.Parent == DestinationParent && old.Containment == Containment:
                    if (old.Index < _newIndex)
                        _newIndex--;
                    if (old.Index != _newIndex)
                    {
                        PartitionCommander.Raise(new ChildMovedInSameContainmentNotification(_newIndex, added, DestinationParent,
                            old.Containment, old.Index, GetNotificationId()));
                    }

                    break;

                case not null when old.Parent == DestinationParent && old.Containment != Containment:
                    PartitionCommander.Raise(new ChildMovedFromOtherContainmentInSameParentNotification(Containment, _newIndex,
                        added, DestinationParent, old.Containment, old.Index, GetNotificationId()));
                    break;

                default:
                    throw new ArgumentException("Unknown state");
            }

            _newIndex++;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionProcessor))]
    protected override bool IsActive() =>
        PartitionCommander != null && PartitionCommander.CanRaise(
            typeof(ChildAddedNotification),
            typeof(ChildMovedFromOtherContainmentNotification),
            typeof(ChildMovedFromOtherContainmentInSameParentNotification),
            typeof(ChildMovedInSameContainmentNotification)
        );
}