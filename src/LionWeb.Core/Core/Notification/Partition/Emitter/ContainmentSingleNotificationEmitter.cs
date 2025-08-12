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

/// Encapsulates notification-related logic and data for changing <i>single</i> <see cref="Containment"/>s.
/// <typeparam name="T">Type of node of the represented <see cref="Containment"/>.</typeparam>
public class ContainmentSingleNotificationEmitter<T> : ContainmentNotificationEmitterBase<T> where T : INode
{
    private readonly T? _newValue;
    private readonly T? _oldValue;

    private OldContainmentInfo? _oldContainmentInfo;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="newValue">Newly set value.</param>
    /// <param name="oldValue">Previous value of <paramref name="containment"/>.</param>
    /// <param name="notificationId">The notification ID of the notification emitted by this notification emitter.</param>
    public ContainmentSingleNotificationEmitter(Containment containment, NodeBase destinationParent, T? newValue, T? oldValue,
        INotificationId? notificationId = null)
        : base(containment, destinationParent, notificationId)
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
    public override void Notify()
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
                PartitionCommander.Raise(new ChildDeletedNotification(_oldValue, DestinationParent, Containment, 0,
                    GetNotificationId()));
                break;

            case (null, not null, null):
                PartitionCommander.Raise(new ChildAddedNotification(DestinationParent, _newValue, Containment, 0,
                    GetNotificationId()));
                break;

            case (not null, not null, null):
                PartitionCommander.Raise(new ChildReplacedNotification(_newValue, _oldValue, DestinationParent, Containment, 0,
                    GetNotificationId()));
                break;

            case (null, not null, not null)
                when _oldContainmentInfo.Parent == DestinationParent && _oldContainmentInfo.Containment != Containment:
                PartitionCommander.Raise(new ChildMovedFromOtherContainmentInSameParentNotification(Containment, 0, _newValue,
                    DestinationParent, _oldContainmentInfo.Containment, _oldContainmentInfo.Index,
                    GetNotificationId()));
                break;

            case (not null, not null, not null)
                when _oldContainmentInfo.Parent == DestinationParent && _oldContainmentInfo.Containment != Containment:
                PartitionCommander.Raise(new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(Containment, 0,
                    _newValue, DestinationParent, _oldContainmentInfo.Containment, _oldContainmentInfo.Index, _oldValue,
                    GetNotificationId()));
                break;

            case (not null, not null, not null)
                when _oldContainmentInfo.Parent != DestinationParent:
                PartitionCommander.Raise(new ChildMovedAndReplacedFromOtherContainmentNotification(DestinationParent,
                    Containment, 0, _newValue, _oldContainmentInfo.Parent, _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index, _oldValue,
                    GetNotificationId()));
                break;

            case (null, not null, not null)
                when _oldContainmentInfo.Parent != DestinationParent:
                var notificationId = GetNotificationId();
                var notification = new ChildMovedFromOtherContainmentNotification(DestinationParent, Containment, 0, _newValue,
                    _oldContainmentInfo.Parent, _oldContainmentInfo.Containment, _oldContainmentInfo.Index, notificationId);
                RaiseOriginMoveEvent(_oldContainmentInfo, notification);
                PartitionCommander.Raise(notification);
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
            typeof(ChildAddedNotification),
            typeof(ChildDeletedNotification),
            typeof(ChildReplacedNotification),
            typeof(ChildMovedFromOtherContainmentNotification),
            typeof(ChildMovedFromOtherContainmentInSameParentNotification),
            typeof(ChildMovedInSameContainmentNotification),
            typeof(ChildMovedAndReplacedFromOtherContainmentNotification),
            typeof(ChildMovedAndReplacedFromOtherContainmentInSameParentNotification)
        );
}