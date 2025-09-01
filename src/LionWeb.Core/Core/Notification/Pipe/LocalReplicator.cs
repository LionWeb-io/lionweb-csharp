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

namespace LionWeb.Core.Notification.Pipe;

using Forest;
using M1;
using Partition;

/// Replicates all notifications from a <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>.
/// Keeps track of <i>locally</i> <see cref="OnLocalPartitionAdded">added</see> and <see cref="OnLocalPartitionDeleted">deleted</see>
/// partitions, and updates the <see cref="SharedNodeMap"/> with <i>locally</i> <see cref="SharedNodeMap.RegisterNode">added</see>
/// and <see cref="SharedNodeMap.UnregisterNode">removed</see> nodes. 
public class LocalReplicator : NotificationPipeBase, INotificationHandler
{
    private readonly SharedNodeMap _sharedNodeMap;

    public LocalReplicator(
        IForest? localForest,
        SharedNodeMap sharedNodeMap,
        object? sender
    ) : base(sender)
    {
        _sharedNodeMap = sharedNodeMap;

        if (localForest == null)
            return;

        foreach (var partition in localForest.Partitions)
        {
            var notificationSender = partition.GetNotificationSender();
            if (notificationSender != null)
                RegisterPartition(notificationSender, partition);
        }
    }

    /// <inheritdoc />
    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        switch (notification)
        {
            case PartitionAddedNotification partitionAdded:
                OnLocalPartitionAdded(correspondingSender, partitionAdded);
                break;
            case PartitionDeletedNotification partitionDeleted:
                OnLocalPartitionDeleted(correspondingSender, partitionDeleted);
                break;
            case ChildAddedNotification e:
                OnLocalChildAdded(e);
                break;
            case ChildDeletedNotification e:
                OnLocalChildDeleted(e);
                break;
            case AnnotationAddedNotification e:
                OnLocalAnnotationAdded(e);
                break;
            case AnnotationDeletedNotification e:
                OnLocalAnnotationDeleted(e);
                break;
        }

        SendWithSender(correspondingSender, notification);
    }

    #region Forest

    private void RegisterPartition(INotificationSender correspondingSender, IPartitionInstance partition)
    {
        _sharedNodeMap.RegisterNode(partition);
        correspondingSender.ConnectTo(this);
    }

    private void UnregisterPartition(INotificationSender correspondingSender, IPartitionInstance partition)
    {
        correspondingSender.Unsubscribe(this);
        _sharedNodeMap.UnregisterNode(partition);
    }

    private void OnLocalPartitionAdded(INotificationSender correspondingSender, PartitionAddedNotification partitionAdded) =>
        RegisterPartition(correspondingSender, partitionAdded.NewPartition);

    private void OnLocalPartitionDeleted(INotificationSender correspondingSender, PartitionDeletedNotification partitionDeleted) =>
        UnregisterPartition(correspondingSender, partitionDeleted.DeletedPartition);

    #endregion

    #region Partition

    private void OnLocalChildAdded(ChildAddedNotification childAdded) =>
        _sharedNodeMap.RegisterNode(childAdded.NewChild);

    private void OnLocalChildDeleted(ChildDeletedNotification childDeleted) =>
        _sharedNodeMap.UnregisterNode(childDeleted.DeletedChild);

    private void OnLocalAnnotationAdded(AnnotationAddedNotification annotationAdded) =>
        _sharedNodeMap.RegisterNode(annotationAdded.NewAnnotation);

    private void OnLocalAnnotationDeleted(AnnotationDeletedNotification annotationDeleted) =>
        _sharedNodeMap.UnregisterNode(annotationDeleted.DeletedAnnotation);

    #endregion
}