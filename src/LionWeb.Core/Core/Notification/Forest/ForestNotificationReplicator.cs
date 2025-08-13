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
using Partition;
using System.Diagnostics;

/// Replicates notifications for a <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>.
/// <inheritdoc cref="RemoteNotificationReplicatorBase{TNotification}"/>
public static class ForestNotificationReplicator
{
    public static INotificationHandler<IForestNotification> Create(IForest localForest,
        SharedPartitionReplicatorMap sharedPartitionReplicatorMap, SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localForest;
        var filter = new IdFilteringNotificationHandler<IForestNotification>(internalSender);
        var remoteReplicator = new RemoteForestNotificationReplicator(localForest, sharedNodeMap, filter, internalSender);
        var localReplicator =
            new LocalForestNotificationReplicator(localForest, sharedPartitionReplicatorMap, sharedNodeMap, internalSender);

        var result = new CompositeNotificationHandler<IForestNotification>([remoteReplicator, filter],
            sender ?? $"Composite of {nameof(ForestNotificationReplicator)} {localForest}");

        var forestHandler = localForest.GetNotificationHandler();
        if (forestHandler != null)
        {
            IHandler.Connect(forestHandler, localReplicator);
            IHandler.Connect(localReplicator, filter);
        }

        return result;
    }
}

public class RemoteForestNotificationReplicator : RemoteNotificationReplicatorBase<IForestNotification>
{
    private readonly IForest _localForest;

    protected internal RemoteForestNotificationReplicator(IForest localForest, SharedNodeMap sharedNodeMap,
        IdFilteringNotificationHandler<IForestNotification> filter, object? sender) :
        base(sharedNodeMap, filter, sender)
    {
        _localForest = localForest;
    }

    /// <see cref="IPublisher{TEvent}.Unsubscribe{TSubscribedEvent}">Unsubscribes</see>
    /// from the <i>local</i> <see cref="IForest"/> and all its <see cref="IPartitionInstance">partitions</see>. 
    // public override void Dispose()
    // {
    //     base.Dispose();
    //
    //     foreach (var localPartition in _localPartitionReplicators.Values)
    //     {
    //         localPartition.Dispose();
    //     }
    //
    //     var forestListener = _localForest.GetPublisher();
    //     if (forestListener == null)
    //         return;
    //
    //     forestListener.Unsubscribe<IForestEvent>(LocalHandler);
    //
    //     GC.SuppressFinalize(this);
    // }

    /// <inheritdoc />
    protected override void ProcessNotification(IForestNotification? forestNotification)
    {
        Debug.WriteLine($"{this.GetType()}: processing notification {forestNotification}");
        switch (forestNotification)
        {
            case PartitionAddedNotification a:
                OnRemoteNewPartition(a);
                break;

            case PartitionDeletedNotification a:
                OnRemotePartitionDeleted(a);
                break;
        }
    }


    private void OnRemoteNewPartition(PartitionAddedNotification partitionAdded) =>
        SuppressNotificationForwarding(partitionAdded, () =>
        {
            var newPartition = partitionAdded.NewPartition;
            _localForest.AddPartitions([newPartition], partitionAdded.NotificationId);
        });

    private void OnRemotePartitionDeleted(PartitionDeletedNotification partitionDeleted) =>
        SuppressNotificationForwarding(partitionDeleted, () =>
        {
            var localPartition = (IPartitionInstance)Lookup(partitionDeleted.DeletedPartition.GetId());
            _localForest.RemovePartitions([localPartition], partitionDeleted.NotificationId);
        });
}

public class LocalForestNotificationReplicator : NotificationHandlerBase<IForestNotification>
{
    private readonly SharedPartitionReplicatorMap _sharedPartitionReplicatorMap;
    private readonly SharedNodeMap _sharedNodeMap;

    public LocalForestNotificationReplicator(IForest localForest, SharedPartitionReplicatorMap sharedPartitionReplicatorMap,
        SharedNodeMap sharedNodeMap, object? sender) : base(sender)
    {
        _sharedPartitionReplicatorMap = sharedPartitionReplicatorMap;
        _sharedNodeMap = sharedNodeMap;

        foreach (var partition in localForest.Partitions)
        {
            RegisterPartition(partition);
        }
    }

    /// <inheritdoc />
    public override void Receive(IForestNotification message)
    {
        switch (message)
        {
            case PartitionAddedNotification partitionAdded:
                OnLocalPartitionAdded(partitionAdded);
                break;
            case PartitionDeletedNotification partitionDeleted:
                OnLocalPartitionDeleted(partitionDeleted);
                break;
        }

        Send(message);
    }

    protected virtual INotificationHandler<IPartitionNotification> CreatePartitionNotificationReplicator(IPartitionInstance partition,
        string sender) =>
        PartitionNotificationReplicator.Create(partition, _sharedNodeMap, sender);

    internal void RegisterPartition(IPartitionInstance partition)
    {
        var replicator = CreatePartitionNotificationReplicator(partition, $"{Sender}.{partition.GetId()}");
        _sharedPartitionReplicatorMap.Register(partition.GetId(), replicator);
    }

    internal void UnregisterPartition(IPartitionInstance partition) =>
        _sharedPartitionReplicatorMap.Unregister(partition.GetId());

    private void OnLocalPartitionAdded(PartitionAddedNotification partitionAdded) =>
        RegisterPartition(partitionAdded.NewPartition);

    private void OnLocalPartitionDeleted(PartitionDeletedNotification partitionDeleted) =>
        UnregisterPartition(partitionDeleted.DeletedPartition);
}