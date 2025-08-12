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
using Core.M1.Event;
using Core.M1.Event.Forest;
using Core.M1.Event.Partition;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Notification.Processor;
using Partition;

internal static class RewriteForestEventReplicator
{
    public static new INotificationProcessor<IForestNotification> Create(IForest localForest,
        SharedPartitionReplicatorMap sharedPartitionReplicatorMap, SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localForest;
        var filter = new NotificationIdFilteringNotificationProcessor<IForestNotification>(internalSender);
        var replacingFilter = new NotificationIdReplacingNotificationProcessor<IForestNotification>(internalSender);
        var remoteReplicator = new RewriteRemoteForestNotificationReplicator(localForest, sharedNodeMap, filter, replacingFilter, internalSender);
        var localReplicator = new RewriteLocalForestNotificationReplicator(localForest, sharedPartitionReplicatorMap, sharedNodeMap, internalSender);
        
        var result = new CompositeNotificationProcessor<IForestNotification>([replacingFilter, remoteReplicator, filter],
            sender ?? $"Composite of {nameof(RewriteForestEventReplicator)} {localForest}");

        var forestProcessor = localForest.GetProcessor();
        if (forestProcessor != null)
        {
            IProcessor.Connect(forestProcessor, localReplicator);
            IProcessor.Connect(localReplicator, filter);
        }

        return result;
    }
}

internal class RewriteRemoteForestNotificationReplicator(
    IForest localForest,
    SharedNodeMap sharedNodeMap,
    NotificationIdFilteringNotificationProcessor<IForestNotification> filter,
    NotificationIdReplacingNotificationProcessor<IForestNotification> replacingFilter,
    object? sender
) : RemoteForestNotificationReplicator(localForest, sharedNodeMap, filter, sender)
{
    private readonly INotificationIdProvider _eventIdProvider = new NotificationIdProvider(null);

    protected override void SuppressNotificationForwarding(IForestNotification forestEvent, Action action)
    {
        var eventId = _eventIdProvider.CreateNotificationId();
        var originalEventId = forestEvent.NotificationId;
        replacingFilter.RegisterReplacementNotificationId(eventId, originalEventId);
        Filter.RegisterNotificationId(eventId);

        try
        {
            action();
        } finally
        {
            Filter.UnregisterNotificationId(eventId);
            replacingFilter.UnregisterReplacementNotificationId(eventId);
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
    protected override INotificationProcessor<IPartitionNotification> CreatePartitionEventReplicator(IPartitionInstance partition,
        string sender) =>
        RewritePartitionEventReplicator.Create(partition, sharedNodeMap, sender);
}