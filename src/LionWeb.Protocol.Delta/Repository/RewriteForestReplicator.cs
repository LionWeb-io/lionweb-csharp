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
using Core.Notification.Pipe;

// TODO: Add docs once functionality (and usage) is clear.
internal static class RewriteForestReplicator
{
    public static INotificationHandler Create(
        IForest localForest,
        SharedNodeMap sharedNodeMap,
        object? sender
    )
    {
        IdReplacingNotificationFilter replacingFilter = null!;
        var parts = ForestReplicator.CreateInternal(localForest,
            sharedNodeMap,
            sender,
            (filter, s) =>
            {
                replacingFilter = new IdReplacingNotificationFilter(s);
                return new RewriteRemoteReplicator(
                    localForest,
                    sharedNodeMap,
                    filter,
                    replacingFilter,
                    s
                );
            });

        var result = new MultipartNotificationHandler(
            parts.Prepend(replacingFilter).ToList(),
            sender ?? $"Composite of {nameof(RewriteForestReplicator)} {localForest}");

        return result;
    }
}

internal class RewriteRemoteReplicator(
    IForest? localForest,
    SharedNodeMap sharedNodeMap,
    IdFilteringNotificationFilter filter,
    IdReplacingNotificationFilter replacingFilter,
    object? sender
) : RemoteReplicator(localForest, sharedNodeMap, filter, sender)
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