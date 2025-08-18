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
public interface IForestNotificationHandler : INotificationHandler
{
    public void InitiateNotification(INotification notification) =>
        // Receive(this, notification);
        Send(notification);

}

/// Forwards all <see cref="ModelNotificationHandlerBase{TNotification}.Receive">received</see> notifications
/// unchanged to <i>following</i> notification handler,
/// and to EventHandlers <see cref="ModelNotificationHandlerBase{TNotification}.Subscribe{TSubscribedNotification}">subscribed</see>
/// to specific notifications.
public class ForestNotificationHandler(object? sender)
    : ModelNotificationHandlerBase<IForestNotification>(sender), IForestNotificationHandler
{
    /// <inheritdoc />
    protected override void Send(INotification message)
    {
        switch (message)
        {
            case IForestNotification f:
                SendWithSender(f.Partition.GetNotificationHandler(), message);
                return;
            default:
                throw new NotImplementedException();
        }
    }
}