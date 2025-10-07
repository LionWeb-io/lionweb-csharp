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

namespace LionWeb.Core.Notification.Partition;

using Pipe;

/// Produces notifications about <see cref="INode">nodes</see> and their <see cref="Feature">features</see>.
public interface IPartitionNotificationProducer : INotificationProducer
{
    INotificationId CreateNotificationId();
}

/// Forwards all <see cref="INotificationProducer.ProduceNotification">initiated</see> notifications
/// unchanged to <i>following</i> notification pipes.
public class PartitionNotificationProducer(object? sender)
    : ModelNotificationProducerBase(sender), IPartitionNotificationProducer
{
    private readonly INotificationIdProvider _notificationIdProvider = new NotificationIdProvider(sender);

    /// <inheritdoc />
    public INotificationId CreateNotificationId() =>
        _notificationIdProvider.CreateNotificationId();
}