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

/// Encapsulates notification-related logic and data for <i>adding</i> or <i>inserting</i> of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
[Obsolete("Use ContainmentAddSingleNotificationEmitter instead.")]
public class ContainmentAddMultipleNotificationEmitter<T> : ContainmentAddSingleNotificationEmitter<T> where T : IWritableNode
{
    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="addedValues">Newly added values.</param>
    /// <param name="existingValues">Values already present in <paramref name="containment"/>.</param>
    /// <param name="startIndex">Optional index where we add <paramref name="addedValues"/> to <paramref name="containment"/>.</param>
    public ContainmentAddMultipleNotificationEmitter(Containment containment,
        INotifiableNode destinationParent,
        List<T>? addedValues,
        List<T> existingValues,
        Index? startIndex = null,
        INotificationId? notificationId = null) : base(containment, destinationParent, addedValues.First(), existingValues, startIndex) { }
}