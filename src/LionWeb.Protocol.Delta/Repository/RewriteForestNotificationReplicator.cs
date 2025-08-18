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

namespace LionWeb.Protocol.Delta.Repository;

using Core.M1;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Handler;

internal static class RewriteForestNotificationReplicator
{
    public static IConnectingNotificationHandler Create(
        IForest localForest,
        SharedNodeMap sharedNodeMap,
        object? sender
    )
    {
        IdReplacingNotificationHandler replacingFilter = null!;
        var parts = ForestNotificationReplicator.CreateInternal(localForest,
            sharedNodeMap,
            sender,
            (filter, s) =>
            {
                replacingFilter = new IdReplacingNotificationHandler(s);
                return new RewriteRemoteNotificationReplicator(
                    localForest,
                    sharedNodeMap,
                    filter,
                    replacingFilter,
                    s
                );
            });

        var result = new CompositeNotificationHandler(
            parts.Prepend(replacingFilter).ToList(),
            sender ?? $"Composite of {nameof(RewriteForestNotificationReplicator)} {localForest}");

        return result;
    }
}

internal class RewriteRemoteNotificationReplicator(
    IForest? localForest,
    SharedNodeMap sharedNodeMap,
    IdFilteringNotificationHandler filter,
    IdReplacingNotificationHandler replacingFilter,
    object? sender
) : RemoteNotificationReplicator(localForest, sharedNodeMap, filter, sender)
{
    private readonly INotificationIdProvider _notificationIdProvider = new NotificationIdProvider(null);

    protected override void SuppressNotificationForwarding(INotification forestNotification, Action action)
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