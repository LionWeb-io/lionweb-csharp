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

/// Encapsulates notification-related logic and data for <i>multiple</i> <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public abstract class ContainmentMultipleNotificationEmitterBase<T> : ContainmentNotificationEmitterBase<T> where T : INode
{
    /// Newly set values and their previous context.
    protected readonly Dictionary<T, OldContainmentInfo?> NewValues;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="newValues">Newly set values.</param>
    /// <param name="notificationId">The notification ID of the notification emitted by this notification emitter.</param>
    protected ContainmentMultipleNotificationEmitterBase(Containment containment, INotifiableNode destinationParent,
        List<T>? newValues, INotificationId? notificationId = null) :
        base(containment, destinationParent, notificationId)
    {
        try
        {
            NewValues = newValues?.ToDictionary<T, T, OldContainmentInfo?>(k => k, _ => null) ?? [];
        } catch (ArgumentException e)
        {
            throw new ArgumentException(string.Join(",", newValues?.Select(n => n.GetId()) ?? []), e);
        }
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive())
            return;

        foreach (T newValue in NewValues.Keys.ToList())
        {
            NewValues[newValue] = Collect(newValue);
        }
    }
}