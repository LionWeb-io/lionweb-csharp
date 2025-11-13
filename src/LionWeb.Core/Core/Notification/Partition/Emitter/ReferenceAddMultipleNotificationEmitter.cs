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

public class ReferenceAddMultipleNotificationEmitter<T> : ReferenceMultipleNotificationEmitterBase<T> where T : IReadableNode
{
    private readonly Index _startIndex;

    /// Raises <see cref="ReferenceAddedNotification"/> for <paramref name="reference"/> for each entry in <paramref name="safeNodes"/>.
    /// <param name="reference">Reference to raise notifications for.</param>
    /// <param name="destinationParent">Owner of the represented <paramref name="reference"/> </param>
    /// <param name="safeNodes">Targets to raise notifications for.</param>
    /// <param name="startIndex">Index where we add <paramref name="safeNodes"/> to <paramref name="reference"/>.</param>
    /// <param name="notificationId">The notification ID of the notification emitted by this notification emitter.</param>
    /// <typeparam name="T">Type of members of <paramref name="reference"/>.</typeparam>
    public ReferenceAddMultipleNotificationEmitter(Reference reference, INotifiableNode destinationParent, List<IReferenceDescriptor> safeNodes,
        Index startIndex, INotificationId? notificationId = null) : base(reference, destinationParent, safeNodes, notificationId)
    {
        _startIndex = startIndex;
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        Index index = _startIndex;
        foreach (var referenceDescriptor in SafeNodes)
        {
            IReferenceTarget newTarget = referenceDescriptor.ToReferenceTarget();
            ProduceNotification(new ReferenceAddedNotification(DestinationParent, Reference, index++, newTarget,
                GetNotificationId()));
        }
    }

    /// <inheritdoc />
    protected override bool IsActive() =>
        Handles(
            typeof(ReferenceAddedNotification)
        );
}