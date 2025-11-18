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

public class ReferenceSingleNotificationEmitter<T> : ReferenceNotificationEmitterBase<T> where T : IReadableNode
{
    private readonly ReferenceDescriptor? _newTarget;
    private readonly ReferenceDescriptor? _oldTarget;

    /// Raises either <see cref="ReferenceAddedNotification"/>, <see cref="ReferenceDeletedNotification"/> or
    /// <see cref="ReferenceChangedNotification"/> for <paramref name="reference"/>,
    /// depending on <paramref name="oldTarget"/> and <paramref name="newTarget"/>.
    public ReferenceSingleNotificationEmitter(Reference reference, INotifiableNode destinationParent, ReferenceDescriptor? newTarget,
        ReferenceDescriptor? oldTarget, INotificationId? notificationId = null) : base(reference, destinationParent, notificationId)
    {
        _newTarget = newTarget;
        _oldTarget = oldTarget;
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        if (_oldTarget == _newTarget || ReferenceEquals(_oldTarget?.Target, _newTarget?.Target))
            return;
        
        switch (_oldTarget, _newTarget)
        {
            case (null, { } v):
                IReferenceTarget newTarget = v.ToReferenceTarget();
                ProduceNotification(new ReferenceAddedNotification(DestinationParent, Reference, 0, newTarget,
                    GetNotificationId()));
                break;
            case ({ } o, null):
                IReferenceTarget deletedTarget = o.ToReferenceTarget();
                ProduceNotification(new ReferenceDeletedNotification(DestinationParent, Reference, 0, deletedTarget,
                    GetNotificationId()));
                break;
            case ({ } o, { } n):
                IReferenceTarget replacedTarget = o.ToReferenceTarget();
                ProduceNotification(new ReferenceChangedNotification(DestinationParent, Reference, 0,
                    n.ToReferenceTarget(), replacedTarget,
                    GetNotificationId()));
                break;
        }
    }

    /// <inheritdoc />
    protected override bool IsActive() =>
        Handles(
            typeof(ReferenceAddedNotification),
            typeof(ReferenceDeletedNotification),
            typeof(ReferenceChangedNotification)
        );
}