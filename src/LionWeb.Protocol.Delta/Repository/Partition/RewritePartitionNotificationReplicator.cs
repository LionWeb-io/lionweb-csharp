// Copyright 2025 LionWeb Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
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
// SPDX-FileCopyrightText: 2025 LionWeb Project
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Protocol.Delta.Repository.Partition;

using Core;
using Core.Notification;
using Core.Notification.Handler;
using Core.Notification.Partition;

internal static class RewritePartitionNotificationReplicator
{
    public static new INotificationHandler<IPartitionNotification> Create(IPartitionInstance localPartition,
        SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localPartition.GetId();
        var filter = new IdFilteringNotificationHandler<IPartitionNotification>(internalSender);
        var replacingFilter = new IdReplacingNotificationHandler<IPartitionNotification>(internalSender);
        var remoteReplicator =
            new RewriteRemotePartitionNotificationReplicator(localPartition, filter, replacingFilter, sharedNodeMap,
                internalSender);
        var localReplicator = new LocalPartitionNotificationReplicator(sharedNodeMap, internalSender);

        var result = new CompositeNotificationHandler<IPartitionNotification>(
            [replacingFilter, remoteReplicator, filter],
            sender ?? $"Composite of {nameof(RewritePartitionNotificationReplicator)} {localPartition.GetId()}");

        var partitionHandler = localPartition.GetNotificationHandler();
        if (partitionHandler != null)
        {
            INotificationHandler.Connect(partitionHandler, localReplicator);
            INotificationHandler.Connect(localReplicator, filter);
        }

        return result;
    }
}

internal class RewriteRemotePartitionNotificationReplicator(
    IPartitionInstance localPartition,
    IdFilteringNotificationHandler<IPartitionNotification> filter,
    IdReplacingNotificationHandler<IPartitionNotification> replacingFilter,
    SharedNodeMap sharedNodeMap,
    object? sender)
    : RemotePartitionNotificationReplicator(localPartition, sharedNodeMap, filter, sender)
{
    private readonly INotificationIdProvider _notificationIdProvider = new NotificationIdProvider(null);

    protected override void SuppressNotificationForwarding(IPartitionNotification partitionNotification, Action action)
    {
        var notificationId = _notificationIdProvider.CreateNotificationId();
        var originalNotificationId = partitionNotification.NotificationId;
        replacingFilter.RegisterReplacementNotificationId(notificationId, originalNotificationId);
        Filter.RegisterNotificationId(notificationId);

        try
        {
            action();
        } finally
        {
            Filter.UnregisterNotificationId(notificationId);
            replacingFilter.UnregisterReplacementNotificationId(notificationId);
        }
    }
}