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

namespace LionWeb.Core.Notification.Forest;

using Handler;
using Partition;

/// Provides notifications for adding and deleting <see cref="IPartitionInstance">partitions</see>.
/// Raises notifications for adding and deleting <see cref="IPartitionInstance">partitions</see>.
public interface IForestNotificationHandler : INotificationHandler<IForestNotification>
{
    /// <inheritdoc cref="INotificationHandler{TNotification}.Receive"/>
    /// If used (instead of <see cref="INotificationHandler{TNotification}.Receive"/>), <paramref name="correspondingHandler"/>
    /// is the <i>preceding</i> handler for <paramref name="message"/>.<see cref="IForestNotification.Partition"/>.
    public void Receive(IPartitionNotificationHandler correspondingHandler, IForestNotification message) =>
        Receive(message);
}

/// Forwards all <see cref="ModelNotificationHandlerBase{TNotification}.Receive">received</see> notifications
/// unchanged to <i>following</i> notification handler,
/// and to EventHandlers <see cref="ModelNotificationHandlerBase{TNotification}.Subscribe{TSubscribedNotification}">subscribed</see>
/// to specific notifications.
public class ForestNotificationHandler(object? sender)
    : ModelNotificationHandlerBase<IForestNotification>(sender), IForestNotificationHandler
{
    /// <inheritdoc />
    public void Receive(IPartitionNotificationHandler correspondingHandler, IForestNotification message) => 
        Receive(message);

    /// <inheritdoc />
    protected override void Send(IForestNotification message) =>
        SendWithSender(message.Partition.GetNotificationHandler(), message);
}