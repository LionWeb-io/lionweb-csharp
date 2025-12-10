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

using M1;
using Pipe;

/// Replicates notifications for a <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>.
/// <inheritdoc cref="RemoteReplicator"/>
public static class ForestReplicator
{
    /// Builds up the ForestReplicator MultipartNotificationHandler:
    /// <code>
    ///
    /// +-------------------------------------------------+
    /// | Remote receiver, e.g. DeltaProtocolReceiverBase |
    /// +--╫----------------------------------------------+
    ///    ║ 7,8
    ///    ║ +--------------------------------------------------------------------------+
    ///    ║ | ForestReplicator MultipartNotificationHandler                            |
    ///    ║ +--------------------+------------------+----------------------------------+
    ///    ╚═╪═» <see cref="RemoteReplicator"/> | <see cref="LocalReplicator"/> ═╪═» <see cref="IdFilteringNotificationFilter"/> |
    ///      |                    |          ^       |                               ║  |
    ///      +-------------╫------+----------╫-------+-------------------------------╫--+
    ///        9           ║             3,4 ║ 11      5   12                        ║
    /// +---------------+  ║  +-----------+--╫-------------------------+   +---------╫-------------------+
    /// | Local changes ╪══╩══╪═» <see cref="IForest"/> | <see cref="IForestNotificationProducer"/> |   |         v                   |
    /// +---------------+     +-----------+----------------------------+   | Remote sender, e.g.         |
    ///   1                                 2   10                         | LionWebClientBase.          |
    ///                                                                    |   LocalNotificationReceiver |
    ///                                                                    +-----------------------------+
    ///                                                                      6
    /// </code>
    /// <list type="number">
    /// <item>On side A, we change a <see cref="IForest"/> locally.</item>
    /// <item>On side A, the <see cref="IForestNotificationProducer"/> picks up the change.</item>
    /// <item>On side A, the <see cref="LocalReplicator"/> receives the notification.</item>
    /// <item>On side A, the <see cref="LocalReplicator"/> updates <see cref="SharedNodeMap">internal look-up structures</see>, and forwards the notification.</item>
    /// <item>On side A, the <see cref="IdFilteringNotificationFilter"/> doesn't suppress the notification, and forwards it.</item>
    /// <item>On side A, the remote sender transmits the notification.</item>
    /// <item>On side B, the remote receiver receives the notification.</item>
    /// <item>On side B, the remote receiver maps the incoming notification to a local one, with locally known nodes.</item>
    /// <item>
    ///   On side B, the <see cref="RemoteReplicator"/> interprets the notification,
    ///   instructs the <see cref="IdFilteringNotificationFilter"/> to ignore the upcoming notification,
    ///   and changes the <see cref="IForest"/> locally.
    /// </item>
    /// <item>On side B, the <see cref="IForestNotificationProducer"/> picks up the change.</item>
    /// <item>On side B, the <see cref="LocalReplicator"/> updates <see cref="SharedNodeMap">internal look-up structures</see>, and forwards the notification.</item>
    /// <item>On side B, the <see cref="IdFilteringNotificationFilter"/> suppresses the notification.</item>
    /// </list>
    public static INotificationHandler Create(
        IForest localForest,
        SharedNodeMap sharedNodeMap,
        object? sender
    )
    {
        var parts = CreateInternal(
            localForest,
            sharedNodeMap,
            sender,
            (filter, s) => new RemoteReplicator(localForest, filter, sharedNodeMap, s)
        );

        var result = new MultipartNotificationHandler(parts,
            sender ?? $"Multipart of {nameof(ForestReplicator)} {localForest}");

        return result;
    }

    public static List<INotificationHandler> CreateInternal(
        IForest localForest,
        SharedNodeMap sharedNodeMap,
        object? sender,
        Func<IdFilteringNotificationFilter, object, RemoteReplicator> remoteNotificationReplicator
    )
    {
        var internalSender = sender ?? localForest;
        var filter = new IdFilteringNotificationFilter(internalSender);
        var remoteReplicator = remoteNotificationReplicator(filter, internalSender);
        var localReplicator = new LocalReplicator(localForest, sharedNodeMap, internalSender);

        var result = new List<INotificationHandler> { remoteReplicator, filter };

        var forestSender = localForest.GetNotificationSender();
        if (forestSender == null)
            return result;

        forestSender.ConnectTo(localReplicator);
        localReplicator.ConnectTo(filter);

        return result;
    }
}