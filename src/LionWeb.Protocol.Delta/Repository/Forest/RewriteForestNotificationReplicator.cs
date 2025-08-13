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

namespace LionWeb.Protocol.Delta.Repository.Forest;

using Core;
using Core.M1;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Handler;
using Core.Notification.Partition;
using Partition;

internal static class RewriteForestNotificationReplicator
{
    public static new INotificationHandler<IForestNotification> Create(IForest localForest,
        SharedPartitionReplicatorMap sharedPartitionReplicatorMap, SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localForest;
        var filter = new IdFilteringNotificationHandler<IForestNotification>(internalSender);
        var replacingFilter = new IdReplacingNotificationHandler<IForestNotification>(internalSender);
        var remoteReplicator =
            new RewriteRemoteForestNotificationReplicator(localForest, sharedNodeMap, filter, replacingFilter,
                internalSender);
        var localReplicator = new RewriteLocalForestNotificationReplicator(localForest, sharedPartitionReplicatorMap,
            sharedNodeMap, internalSender);

        var result = new CompositeNotificationHandler<IForestNotification>(
            [replacingFilter, remoteReplicator, filter],
            sender ?? $"Composite of {nameof(RewriteForestNotificationReplicator)} {localForest}");

        var forestHandler = localForest.GetNotificationHandler();
        if (forestHandler != null)
        {
            INotificationHandler.Connect(forestHandler, localReplicator);
            INotificationHandler.Connect(localReplicator, filter);
        }

        return result;
    }
}

internal class RewriteRemoteForestNotificationReplicator(
    IForest localForest,
    SharedNodeMap sharedNodeMap,
    IdFilteringNotificationHandler<IForestNotification> filter,
    IdReplacingNotificationHandler<IForestNotification> replacingFilter,
    object? sender
) : RemoteForestNotificationReplicator(localForest, sharedNodeMap, filter, sender)
{
    private readonly INotificationIdProvider _notificationIdProvider = new NotificationIdProvider(null);

    protected override void SuppressNotificationForwarding(IForestNotification forestNotification, Action action)
    {
        var notificationId = _notificationIdProvider.CreateNotificationId();
        var originalNotificationId = forestNotification.NotificationId;
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

internal class RewriteLocalForestNotificationReplicator(
    IForest localForest,
    SharedPartitionReplicatorMap sharedPartitionReplicatorMap,
    SharedNodeMap sharedNodeMap,
    object? sender)
    : LocalForestNotificationReplicator(localForest, sharedPartitionReplicatorMap, sharedNodeMap, sender)
{
    protected override INotificationHandler<IPartitionNotification> CreatePartitionNotificationReplicator(
        IPartitionInstance partition, string sender) =>
        RewritePartitionNotificationReplicator.Create(partition, sharedNodeMap, sender);
}