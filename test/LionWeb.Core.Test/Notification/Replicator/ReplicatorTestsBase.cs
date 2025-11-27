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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using M1;
using Replicator;

public abstract class ReplicatorTestsBase: NotificationTestsBase
{
    /// <summary>
    /// This replicator exercises the following path:
    /// change on <paramref name="originalForest"/> -> notification -> notificationToNotificationMapper -> notification ->
    /// forestReplicator -> change applied on <paramref name="clonedForest"/>
    /// </summary>
    /// <seealso cref="NotificationToNotificationMapper"/>
    protected static void CreateForestReplicator(IForest clonedForest, IForest originalForest)
    {
        var sharedNodeMap = new SharedNodeMap();
        var notificationMapper = new NotificationMapper(sharedNodeMap);
        originalForest.GetNotificationSender()!.ConnectTo(notificationMapper);
        
        var replicator = ForestReplicator.Create(clonedForest, sharedNodeMap, null);
        notificationMapper.ConnectTo(replicator);
    }
    
    /// <summary>
    /// This replicator exercises the following path:
    /// change on <paramref name="originalPartition"/> -> notification -> notificationToNotificationMapper -> notification ->
    /// partitionReplicator -> change applied on <paramref name="clonedPartition"/>
    /// </summary>
    /// <seealso cref="NotificationToNotificationMapper"/>
    protected static void CreatePartitionReplicator(IPartitionInstance clonedPartition, IPartitionInstance originalPartition, SharedNodeMap? sharedNodeMap = null)
    {
        sharedNodeMap ??= new SharedNodeMap();
        
        var notificationMapper = new NotificationMapper(sharedNodeMap);
        originalPartition.GetNotificationSender()!.ConnectTo(notificationMapper);

        var replicator = PartitionReplicator.Create(clonedPartition, sharedNodeMap, null);
        notificationMapper.ConnectTo(replicator);
    }
    
    /// <summary>
    /// This replicator exercises the following path:
    /// <paramref name="notification"/> -> notificationForwarder -> notificationToNotificationMapper -> notification ->
    /// partitionReplicator -> change applied on <paramref name="clonedPartition"/>
    /// Different from the <see cref="CreatePartitionReplicator(LionWeb.Core.IPartitionInstance,LionWeb.Core.IPartitionInstance)"/>, this method
    /// does not receive notifications triggered by a change on a partition, but it directly accepts (e.g. a manually created notification) notifications.
    /// If there are nodes which are not part of the <paramref name="clonedPartition"/>, <paramref name="sharedNodeMap"/> can be used to add those nodes. 
    /// </summary>
    /// <seealso cref="NotificationToNotificationMapper"/>
    /// <seealso cref="NotificationForwarder"/>
    protected static void CreatePartitionReplicator(IPartitionInstance clonedPartition, INotification notification, SharedNodeMap? sharedNodeMap = null)
    {
        sharedNodeMap ??= new SharedNodeMap();
        var notificationMapper = new NotificationMapper(sharedNodeMap);
        var replicator = PartitionReplicator.Create(clonedPartition, sharedNodeMap, null);
        var notificationForwarder = new NotificationForwarder();
        
        notificationForwarder.ConnectTo(notificationMapper);
        notificationMapper.ConnectTo(replicator);
        
        notificationForwarder.ProduceNotification(notification);
    }
}