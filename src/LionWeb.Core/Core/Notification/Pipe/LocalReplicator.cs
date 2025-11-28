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
            RegisterPartition(partition);
        }
    }

    /// <inheritdoc />
    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        switch (notification)
        {
            case PartitionAddedNotification partitionAdded:
                OnLocalPartitionAdded(partitionAdded);
                break;
            case PartitionDeletedNotification partitionDeleted:
                OnLocalPartitionDeleted(partitionDeleted);
                break;

            case ChildAddedNotification e:
                OnLocalChildAdded(e);
                break;
            case ChildDeletedNotification e:
                OnLocalChildDeleted(e);
                break;
            case ChildReplacedNotification e:
                OnLocalChildReplaced(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentNotification e:
                OnLocalChildMovedAndReplacedFromOtherContainment(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentInSameParentNotification e:
                OnLocalChildMovedAndReplacedFromOtherContainmentInSameParent(e);
                break;
            case ChildMovedAndReplacedInSameContainmentNotification e:
                OnLocalChildMovedAndReplacedInSameContainment(e);
                break;

            case AnnotationAddedNotification e:
                OnLocalAnnotationAdded(e);
                break;
            case AnnotationDeletedNotification e:
                OnLocalAnnotationDeleted(e);
                break;
            case AnnotationReplacedNotification e:
                OnLocalAnnotationReplaced(e);
                break;
            case AnnotationMovedAndReplacedFromOtherParentNotification e:
                OnLocalAnnotationMovedAndReplacedFromOtherParent(e);
                break;
            case AnnotationMovedAndReplacedInSameParentNotification e:
                OnLocalAnnotationMovedAndReplacedInSameParent(e);
                break;
        }

        SendWithSender(correspondingSender, notification);
    }

    #region Forest

    private void RegisterPartition(IPartitionInstance partition) =>
        _sharedNodeMap.RegisterNode(partition);

    private void UnregisterPartition(IPartitionInstance partition) =>
        _sharedNodeMap.UnregisterNode(partition);

    private void OnLocalPartitionAdded(PartitionAddedNotification partitionAdded) =>
        RegisterPartition(partitionAdded.NewPartition);

    private void OnLocalPartitionDeleted(PartitionDeletedNotification partitionDeleted) =>
        UnregisterPartition(partitionDeleted.DeletedPartition);

    #endregion

    #region Partition

    #region Containment

    private void OnLocalChildAdded(ChildAddedNotification childAdded) =>
        _sharedNodeMap.RegisterNode(childAdded.NewChild);

    private void OnLocalChildDeleted(ChildDeletedNotification childDeleted) =>
        _sharedNodeMap.UnregisterNode(childDeleted.DeletedChild);

    private void OnLocalChildReplaced(ChildReplacedNotification childReplaced)
    {
        _sharedNodeMap.UnregisterNode(childReplaced.ReplacedChild);
        _sharedNodeMap.RegisterNode(childReplaced.NewChild);
    }

    private void OnLocalChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentNotification childMovedAndReplaced) =>
        _sharedNodeMap.UnregisterNode(childMovedAndReplaced.ReplacedChild);

    private void OnLocalChildMovedAndReplacedFromOtherContainmentInSameParent(
        ChildMovedAndReplacedFromOtherContainmentInSameParentNotification notification) =>
        _sharedNodeMap.UnregisterNode(notification.ReplacedChild);

    private void OnLocalChildMovedAndReplacedInSameContainment(
        ChildMovedAndReplacedInSameContainmentNotification childMovedAndReplaced) =>
        _sharedNodeMap.UnregisterNode(childMovedAndReplaced.ReplacedChild);

    #endregion

    #region Annotation

    private void OnLocalAnnotationAdded(AnnotationAddedNotification annotationAdded) =>
        _sharedNodeMap.RegisterNode(annotationAdded.NewAnnotation);

    private void OnLocalAnnotationDeleted(AnnotationDeletedNotification annotationDeleted) =>
        _sharedNodeMap.UnregisterNode(annotationDeleted.DeletedAnnotation);

    private void OnLocalAnnotationReplaced(AnnotationReplacedNotification annotationReplaced)
    {
        _sharedNodeMap.UnregisterNode(annotationReplaced.ReplacedAnnotation);
        _sharedNodeMap.RegisterNode(annotationReplaced.NewAnnotation);
    }

    private void OnLocalAnnotationMovedAndReplacedFromOtherParent(
        AnnotationMovedAndReplacedFromOtherParentNotification annotationMovedAndReplaced) =>
        _sharedNodeMap.UnregisterNode(annotationMovedAndReplaced.ReplacedAnnotation);

    private void OnLocalAnnotationMovedAndReplacedInSameParent(
        AnnotationMovedAndReplacedInSameParentNotification annotationMovedAndReplaced) =>
        _sharedNodeMap.UnregisterNode(annotationMovedAndReplaced.ReplacedAnnotation);

    #endregion

    #endregion
}