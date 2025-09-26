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

public class ReferenceSingleNotificationEmitter : ReferenceNotificationEmitterBase<INode>
{
    private readonly IReadableNode? _newTarget;
    private readonly IReadableNode? _oldTarget;

    /// Raises either <see cref="ReferenceAddedNotification"/>, <see cref="ReferenceDeletedNotification"/> or
    /// <see cref="ReferenceChangedNotification"/> for <paramref name="reference"/>,
    /// depending on <paramref name="oldTarget"/> and <paramref name="newTarget"/>.
    public ReferenceSingleNotificationEmitter(Reference reference, INotifiableNode destinationParent, IReadableNode? newTarget,
        IReadableNode? oldTarget, INotificationId? notificationId = null) : base(reference, destinationParent, notificationId)
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

        if (ReferenceEquals(_oldTarget, _newTarget))
            return;
        
        switch (_oldTarget, _newTarget)
        {
            case (null, { } v):
                IReferenceTarget newTarget = new ReferenceTarget(null, v);
                ProduceNotification(new ReferenceAddedNotification(DestinationParent, Reference, 0, newTarget,
                    GetNotificationId()));
                break;
            case ({ } o, null):
                IReferenceTarget deletedTarget = new ReferenceTarget(null, o);
                ProduceNotification(new ReferenceDeletedNotification(DestinationParent, Reference, 0, deletedTarget,
                    GetNotificationId()));
                break;
            case ({ } o, { } n):
                IReferenceTarget replacedTarget = new ReferenceTarget(null, o);
                ProduceNotification(new ReferenceChangedNotification(DestinationParent, Reference, 0,
                    new ReferenceTarget(null, n), replacedTarget,
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