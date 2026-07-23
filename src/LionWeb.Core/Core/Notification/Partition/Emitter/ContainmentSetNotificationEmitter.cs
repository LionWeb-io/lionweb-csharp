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
using Utilities.ListComparer;

/// Encapsulates notification-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public class ContainmentSetNotificationEmitter<T> : ContainmentMultipleNotificationEmitterBase<T> where T : IWritableNode
{
    private readonly List<T> _existingValues = [];
    private readonly List<IListChange<T>> _changes = [];

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <paramref name="containment"/>.</param>
    public ContainmentSetNotificationEmitter(Containment containment,
        INotifiableNode destinationParent,
        List<T>? setValues,
        List<T> existingValues) : base(containment, destinationParent, setValues)
    {
        if (!IsActive() || setValues == null)
            return;

        _existingValues = existingValues;
        var listComparer = IListComparer.CreateForNodes(existingValues, setValues);
        _changes = listComparer.Compare();
    }

    [Obsolete]
    public ContainmentSetNotificationEmitter(Containment containment,
        INotifiableNode destinationParent,
        List<T>? setValues,
        List<T> existingValues, INotificationId? notificationId = null) : this(containment, destinationParent, setValues, existingValues)
    {
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        base.CollectOldData();

        foreach (T existingValue in _existingValues)
        {
            OldContainmentInfos[existingValue] = Collect(existingValue);
        }
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
                    switch (OldContainmentInfos[added.Element])
                    {
                        case null:
                            ProduceNotification(new ChildAddedNotification(DestinationParent, added.Element, Containment,
                                added.RightIndex, GetNotificationId()));
                            break;

                        case { Partition: not null } old when DestinationPartition is null:
                            ProduceOriginNotification(old, new ChildDeletedNotification(added.Element, old.Parent, old.Containment, old.Index, GetNotificationId(old.Partition)));
                            break;

                        case { } old when old.Parent != DestinationParent:
                            var notification = new ChildMovedFromOtherContainmentNotification(DestinationParent, Containment,
                                added.RightIndex, added.Element, old.Parent, old.Containment, old.Index, GetNotificationId());
                            ProduceOriginNotification(old, notification);
                            ProduceNotification(notification);
                            break;


                        case { } old when old.Parent == DestinationParent && old.Containment != Containment:
                            ProduceNotification(new ChildMovedFromOtherContainmentInSameParentNotification(Containment,
                                added.RightIndex, added.Element, DestinationParent, old.Containment, old.Index,
                                GetNotificationId()));
                            break;

                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case ListMoved<T> moved:
                    ProduceNotification(new ChildMovedInSameContainmentNotification(moved.RightIndex, moved.LeftElement,
                        DestinationParent, Containment, moved.LeftIndex, moved.RightIndex - moved.LeftIndex, GetNotificationId()));
                    break;
                case ListDeleted<T> deleted:
                    var oldDeleted = OldContainmentInfos[deleted.Element];
                    if (oldDeleted is not null || DestinationPartition is not null)
                        ProduceNotification(new ChildDeletedNotification(deleted.Element, DestinationParent, Containment,
                            deleted.LeftIndex, GetNotificationId(oldDeleted?.Partition)));
                    break;
            }
        }
    }
}