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
using Utilities.ListComparer;

/// Encapsulates notification-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public class ContainmentSetNotificationEmitter<T> : ContainmentMultipleNotificationEmitterBase<T> where T : INode
{
    private readonly List<IListChange<T>> _changes = [];

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <paramref name="containment"/>.</param>
    /// <param name="notificationId">The notification ID of the notification emitted by this notification emitter.</param>
    public ContainmentSetNotificationEmitter(Containment containment,
        INotifiableNode destinationParent,
        List<T>? setValues,
        List<T> existingValues, INotificationId? notificationId = null) : base(containment, destinationParent, setValues, notificationId)
    {
        if (!IsActive() || setValues == null)
            return;

        var listComparer = IListComparer.CreateForNodes(existingValues, setValues);
        _changes = listComparer.Compare();
    }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        foreach (var change in _changes)
        {
            switch (change)
            {
                case ListAdded<T> added:
                    switch (NewValues[added.Element])
                    {
                        case null:
                            PartitionProcessor.Receive(new ChildAddedNotification(DestinationParent, added.Element, Containment,
                                added.RightIndex, GetNotificationId()));
                            break;

                        case { } old when old.Parent != DestinationParent:
                            var notificationId = GetNotificationId();
                            var notification = new ChildMovedFromOtherContainmentNotification(DestinationParent, Containment,
                                added.RightIndex, added.Element, old.Parent, old.Containment, old.Index, notificationId);
                            RaiseOriginMoveNotification(old, notification);
                            PartitionProcessor.Receive(notification);
                            break;


                        case { } old when old.Parent == DestinationParent && old.Containment != Containment:
                            PartitionProcessor.Receive(new ChildMovedFromOtherContainmentInSameParentNotification(Containment,
                                added.RightIndex, added.Element, DestinationParent, old.Containment, old.Index,
                                GetNotificationId()));
                            break;

                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case ListMoved<T> moved:
                    PartitionProcessor.Receive(new ChildMovedInSameContainmentNotification(moved.RightIndex, moved.LeftElement,
                        DestinationParent, Containment, moved.LeftIndex, GetNotificationId()));
                    break;
                case ListDeleted<T> deleted:
                    PartitionProcessor.Receive(new ChildDeletedNotification(deleted.Element, DestinationParent, Containment,
                        deleted.LeftIndex, GetNotificationId()));
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionProcessor))]
    protected override bool IsActive() =>
        PartitionProcessor != null && PartitionProcessor.CanReceive(
            typeof(ChildAddedNotification),
            typeof(ChildDeletedNotification),
            typeof(ChildMovedFromOtherContainmentNotification),
            typeof(ChildMovedFromOtherContainmentInSameParentNotification),
            typeof(ChildMovedInSameContainmentNotification)
        );
}