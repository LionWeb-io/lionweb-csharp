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

namespace LionWeb.Core.Notification.Handler;

using Forest;
using M1;
using Partition;

public class LocalNotificationReplicator : NotificationHandlerBase, IConnectingNotificationHandler
{
    private readonly SharedNodeMap _sharedNodeMap;

    public LocalNotificationReplicator(
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
            RegisterPartition(partition.GetNotificationHandler(), partition);
        }
    }

    /// <inheritdoc />
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        switch (notification)
        {
            case PartitionAddedNotification partitionAdded:
                OnLocalPartitionAdded(correspondingHandler, partitionAdded);
                break;
            case PartitionDeletedNotification partitionDeleted:
                OnLocalPartitionDeleted(correspondingHandler, partitionDeleted);
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

        SendWithSender(correspondingHandler, notification);
    }

    #region Forest

    private void RegisterPartition(ISendingNotificationHandler correspondingHandler,
        IPartitionInstance partition)
    {
        _sharedNodeMap.RegisterNode(partition);
        INotificationHandler.Connect(correspondingHandler, this);
    }

    private void UnregisterPartition(ISendingNotificationHandler correspondingHandler,
        IPartitionInstance partition)
    {
        correspondingHandler.Unsubscribe(this);
        _sharedNodeMap.UnregisterNode(partition);
    }

    private void OnLocalPartitionAdded(ISendingNotificationHandler correspondingHandler,
        PartitionAddedNotification partitionAdded)
    {
        RegisterPartition(correspondingHandler, partitionAdded.NewPartition);
    }

    private void OnLocalPartitionDeleted(ISendingNotificationHandler correspondingHandler,
        PartitionDeletedNotification partitionDeleted)
    {
        UnregisterPartition(correspondingHandler, partitionDeleted.DeletedPartition);
    }

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