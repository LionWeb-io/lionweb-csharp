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

/// Forwards all <see cref="Receive">received</see> notifications unchanged to <i>following</i> notification handlers,
/// and to EventHandlers <see cref="Subscribe{TSubscribedNotification}">subscribed</see> to specific notifications.
public abstract class ModelNotificationHandlerBase<TNotification>(object? sender)
    : NotificationHandlerBase<TNotification>(sender)
    where TNotification : INotification
{
    /// <inheritdoc />
    public override void Receive(TNotification message) =>
        Send(message);

    /// Registers <paramref name="handler"/> to be notified of notifications compatible with <typeparamref name="TSubscribedNotification"/>.
    /// <typeparam name="TSubscribedNotification">
    /// Type of notifications <paramref name="handler"/> is interested in.
    /// Notifications raised by this publisher that are <i>not</i> compatible with <typeparamref name="TSubscribedNotification"/>
    /// will <i>not</i> reach <paramref name="handler"/>.
    /// </typeparam> 
    public void Subscribe<TSubscribedNotification>(EventHandler<TSubscribedNotification> handler)
        where TSubscribedNotification : class, TNotification =>
        IHandler.Connect<TNotification, TSubscribedNotification, TSubscribedNotification>(this,
            new NotificationHandlerHandler<TSubscribedNotification>(handler)
            {
                NotificationHandlerId = Sender.ToString() ?? GetType().Name
            });

    /// Unregisters <paramref name="handler"/> from notification of notifications.
    /// Silently ignores calls for unsubscribed <paramref name="handler"/>. 
    public void Unsubscribe<TSubscribedNotification>(EventHandler<TSubscribedNotification> handler)
        where TSubscribedNotification : class, TNotification => throw new NotImplementedException();
}