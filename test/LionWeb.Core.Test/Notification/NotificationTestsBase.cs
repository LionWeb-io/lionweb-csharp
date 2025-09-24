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
using Core.Notification.Pipe;
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using Comparer = Core.Utilities.Comparer;

public abstract class NotificationTestsBase
{
    protected T ClonePartition<T>(T node) where T : IPartitionInstance, INode =>
        (T)new SameIdCloner([node]) { IncludingReferences = true }.Clone()[node];

    protected static void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }

    protected static void AssertEquals(IEnumerable<IReadableNode?> expected, IEnumerable<IReadableNode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
    
    protected static void CreateForestReplicator(IForest clonedForest, IForest originalForest)
    {
        var sharedNodeMap = new SharedNodeMap();
        var notificationMapper = new NotificationMapper(sharedNodeMap);
        originalForest.GetNotificationSender()!.ConnectTo(notificationMapper);
        
        var replicator = ForestReplicator.Create(clonedForest, sharedNodeMap, null);
        notificationMapper.ConnectTo(replicator);
    }
    
    protected static void CreatePartitionReplicator(IPartitionInstance clonedPartition, IPartitionInstance originalPartition)
    {
        var sharedNodeMap = new SharedNodeMap();
        var notificationMapper = new NotificationMapper(sharedNodeMap);
        originalPartition.GetNotificationSender()!.ConnectTo(notificationMapper);

        var replicator = PartitionReplicator.Create(clonedPartition, sharedNodeMap, null);
        notificationMapper.ConnectTo(replicator);
    }
    
    protected static void CreatePartitionReplicator(Geometry clonedPartition, INotification notification)
    {
        var sharedNodeMap = new SharedNodeMap();
        var notificationMapper = new NotificationMapper(sharedNodeMap);
        var replicator = PartitionReplicator.Create(clonedPartition, sharedNodeMap, null);
        var notificationForwarder = new NotificationForwarder();
        
        notificationForwarder.ConnectTo(notificationMapper);
        notificationMapper.ConnectTo(replicator);
        notificationForwarder.ProduceNotification(notification);
    }
}

internal class NotificationForwarder() : NotificationPipeBase(null), INotificationProducer
{
    public void ProduceNotification(INotification notification) => Send(notification);
}

public interface IReplicatorCreator
{
    Geometry CreateReplicator(Geometry node);
}

internal class NotificationMapper(SharedNodeMap sharedNodeMap) : NotificationPipeBase(null), INotificationHandler
{
    private readonly NotificationToNotificationMapper _notificationMapper = new(sharedNodeMap);

    private void ProduceNotification(INotification notification) => Send(notification);

    public void Receive(INotificationSender correspondingSender, INotification notification) =>
        ProduceNotification(_notificationMapper.Map(notification));
}

[Obsolete("Will be removed after tests are refactored with proper ConnectTo() calls")]
public static class NotificationExtensions
{
    // TODO: Replace tests by proper ConnectTo() calls
    public static void Subscribe<T>(this INotificationSender sender, Action<object?, T> handler) =>
        sender.ConnectTo(new FilteredReceiver<T>(handler));
}

[Obsolete("Will be removed after tests are refactored with proper ConnectTo() calls")]
internal class FilteredReceiver<T>(Action<object?, T> handler) : INotificationReceiver
{
    public bool Handles(params Type[] notificationTypes) =>
        notificationTypes.Any(t => typeof(T).IsAssignableFrom(t));

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        if (notification is T t)
            handler.Invoke(correspondingSender, t);
    }
}