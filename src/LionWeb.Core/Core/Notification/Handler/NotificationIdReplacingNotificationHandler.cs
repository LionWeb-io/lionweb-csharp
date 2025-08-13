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

namespace LionWeb.Core.Notification.Handler;

/// TODO: Do we really need this class?
/// Replaces all <see cref="RegisterReplacementNotificationId">registered notification ids</see>.
public class NotificationIdReplacingNotificationHandler<TNotification>(object? sender)
    : FilteringNotificationHandler<TNotification>(sender) where TNotification : class, INotification
{
    private readonly Dictionary<INotificationId, INotificationId> _originalNotificationIds = [];

    /// Replaces id <paramref name="notificationId"/> of future notifications with <paramref name="replacement"/>.
    public void RegisterReplacementNotificationId(INotificationId notificationId, INotificationId replacement) =>
        _originalNotificationIds[notificationId] = replacement;

    /// Stops replacing id <paramref name="notificationId"/> of future notifications.
    public void UnregisterReplacementNotificationId(INotificationId notificationId) =>
        _originalNotificationIds.Remove(notificationId);

    /// <inheritdoc />
    protected override TNotification Filter(TNotification notification)
    {
        TNotification result = notification;
        if (_originalNotificationIds.TryGetValue(notification.NotificationId, out var replacement))
        {
            result.NotificationId = replacement;
        }

        return result;
    }
}