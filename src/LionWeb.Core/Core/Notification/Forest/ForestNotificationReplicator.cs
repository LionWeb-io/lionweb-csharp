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
using M1;

/// Replicates notifications for a <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>.
/// <inheritdoc cref="RemoteNotificationReplicator"/>
public static class ForestNotificationReplicator
{
    public static IConnectingNotificationHandler Create(
        IForest localForest,
        SharedNodeMap sharedNodeMap,
        object? sender
    )
    {
        var parts = CreateInternal(
            localForest,
            sharedNodeMap,
            sender,
            (filter, s) => new RemoteNotificationReplicator(localForest, sharedNodeMap, filter, s)
        );

        var result = new CompositeNotificationHandler(parts,
            sender ?? $"Composite of {nameof(ForestNotificationReplicator)} {localForest}");

        return result;
    }

    public static List<IConnectingNotificationHandler> CreateInternal(
        IForest localForest,
        SharedNodeMap sharedNodeMap,
        object? sender,
        Func<IdFilteringNotificationHandler, object, RemoteNotificationReplicator> remoteNotificationReplicator
    )
    {
        var internalSender = sender ?? localForest;
        var filter = new IdFilteringNotificationHandler(internalSender);
        var remoteReplicator = remoteNotificationReplicator(filter, internalSender);
        var localReplicator = new LocalNotificationReplicator(localForest, sharedNodeMap, internalSender);

        var result = new List<IConnectingNotificationHandler> { remoteReplicator, filter };

        var forestHandler = localForest.GetNotificationHandler();
        if (forestHandler == null)
            return result;

        INotificationHandler.Connect(forestHandler, localReplicator);
        INotificationHandler.Connect(localReplicator, filter);

        return result;
    }
}