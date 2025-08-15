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

using System.Diagnostics;

/// Forwards <see cref="Receive">received</see> notifications if the notification passes <see cref="Filter"/>.
public abstract class FilteringNotificationHandler(object? sender) : NotificationHandlerBase(sender)
{

    /// Unsubscribes all registered <see cref="Subscribe{TSubscribedEvent}">subscribers</see>.
    // public virtual void Dispose()
    // {
    //     if (_localPublisher == null)
    //         return;
    //
    //     foreach (var handler in _forwardingHandlers.Values)
    //     {
    //         _localPublisher.Unsubscribe(handler);
    //     }
    //     
    //     GC.SuppressFinalize(this);
    // }

    /// <inheritdoc />
    public override void Receive(INotification message)
    {
        var filtered = Filter(message);
        Debug.WriteLine($"Forwarding notification id {message.NotificationId}: {filtered?.NotificationId}");
        if (filtered is not null)
            Send(filtered);
    }


    /// Determines whether <paramref name="notification"/> will be <see cref="IHandler{TReceive,TSend}.Send">sent</see> to <i>following</i> notification handlers.
    /// <param name="notification">Notification to check.</param>
    /// <returns>the notification to send, or <c>null</c>.</returns>
    protected abstract INotification? Filter(INotification notification);
}

/// Suppresses all notifications with <see cref="RegisterNotificationId">registered notification ids</see>.
public class IdFilteringNotificationHandler(object? sender) : FilteringNotificationHandler(sender)
{
    private readonly HashSet<INotificationId> _notificationIds = [];

    /// Suppresses future notifications with <paramref name="notificationId"/> from <see cref="IHandler{TReceive,TSend}.Send">sending</see>.
    public void RegisterNotificationId(INotificationId notificationId) =>
        _notificationIds.Add(notificationId);

    /// <see cref="IHandler{TReceive,TSend}.Send">Sends</see> future notifications with <paramref name="notificationId"/>.
    public void UnregisterNotificationId(INotificationId notificationId) =>
        _notificationIds.Remove(notificationId);

    /// <inheritdoc />
    protected override INotification? Filter(INotification notification)
    {
        var result = !_notificationIds.Contains(notification.NotificationId);
        return result ? notification : null;
    }
}