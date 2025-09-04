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
using Core.Notification.Handler;
using Core.Notification.Partition;
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class NotificationApiTests: NotificationTestsBase
{
    #region get informed about changes

    [TestMethod]
    public void ReceiveNotifications_from_partition_via_subscribe()
    {
        var partition = new Geometry("geo");

        int notificationCount = 0;
        var sender = partition.GetNotificationHandler();
        sender?.Subscribe<IPartitionNotification>((_, notification) =>
        {
            notificationCount++;
            Console.WriteLine(notification);
        });

        partition.Documentation = new Documentation("added");

        Assert.AreEqual(1, notificationCount);
    }

    [TestMethod]
    public void ReceiveNotifications_from_partition_via_connected_handlers()
    {
        var partition = new Geometry("geo");

        var sender = partition.GetNotificationHandler();
        var receiver = new Observer();
        if (sender != null)
        {
            INotificationHandler.Connect(from: sender, to: receiver);
        }

        partition.Documentation = new Documentation("added");

        Assert.AreEqual(1, receiver.NotificationCount);
    }

    [TestMethod]
    public void ReceiveNotifications_from_forest()
    {
        var forest = new Forest();

        var sender = forest.GetNotificationHandler();
        var receiver = new Observer();
        INotificationHandler.Connect(from: sender, to: receiver);

        var partition = new Geometry("geo");
        forest.AddPartitions([partition]); 

        Assert.AreEqual(1, receiver.NotificationCount);
    }

    [TestMethod]
    public void ReceiveNotifications_from_forest_and_partition()
    {
        var partition = new Geometry("geo");
        var forest = new Forest();

        var forestHandler = forest.GetNotificationHandler();
        var partitionHandler = partition.GetNotificationHandler();
        
        var forestReceiver = new Observer();
        var partitionReceiver = new Observer();
        
        INotificationHandler.Connect(from: forestHandler, to: forestReceiver);
        if (partitionHandler != null)
        {
            INotificationHandler.Connect(from: partitionHandler, to: partitionReceiver);
        }

        forest.AddPartitions([partition]); 
        partition.Documentation = new Documentation("added");

        Assert.AreEqual(1, forestReceiver.NotificationCount);
        Assert.AreEqual(1, partitionReceiver.NotificationCount);
    }

    #endregion

    #region collect several changes into one change set 

    [TestMethod]
    public void ComposeNotifications_into_a_composite_notification()
    {
        var partition = new Geometry("geo");

        var sender = partition.GetNotificationHandler();
        var compositor = new NotificationCompositor("compositor");
        
        if (sender != null)
        {
            INotificationHandler.Connect(from: sender, to: compositor);    
        }
        
        var counter = new PartitionEventCounter();
        INotificationHandler.Connect(from: compositor, to: counter);
        
        var composite = compositor.Push();

        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";
        partition.AddShapes([new Circle("c")]);

        Assert.AreEqual(3, composite.Parts.Count);
        Assert.AreEqual(0, counter.Count);
        
        compositor.Pop(true);
        Assert.AreEqual(1, counter.Count);
    }

    #endregion
    
    #region replicate changes

    [TestMethod]
    public void ReplicateChanges_Partition()
    {
        var circle = new Circle("c");
        var partition = new Geometry("geo") { Shapes = [circle] };
        var clone = Clone(partition);
        
        var sender = partition.GetNotificationHandler();
        var replicator = PartitionReplicator.Create(clone, new SharedNodeMap(), sender: partition.GetId());
        if (sender != null)
        {
            INotificationHandler.Connect(from: sender, to: replicator);
        }

        circle.Name = "Hello";

        AssertEquals([partition], [clone]);
    }

    
    [TestMethod]
    public void ReplicateChanges_Forest()
    {
        var originalForest = new Forest();
        var cloneForest = new Forest();

        var sender = originalForest.GetNotificationHandler();
        var replicator = ForestReplicator.Create(cloneForest, new SharedNodeMap(), null);
        INotificationHandler.Connect(from: sender, to: replicator);
        
        var moved = new Documentation("moved");
        var originPartition = new Geometry("origin-geo") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var partition = new Geometry("geo");
        
        originalForest.AddPartitions([partition, originPartition]);
        partition.Documentation = moved;

        AssertEquals([partition, originPartition], cloneForest.Partitions.OrderBy(p => p.GetId()).ToList());
    }
    
    private void AssertEquals(IEnumerable<IReadableNode?> expected, IEnumerable<IReadableNode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
    
    #endregion

    protected override Geometry CreateReplicator(Geometry node) => throw new NotImplementedException();
}

internal class Observer : IReceivingNotificationHandler
{
    public int NotificationCount { get; private set; }
    public void Dispose() => throw new NotImplementedException();

    public bool Handles(params Type[] notificationTypes) => true;

    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        NotificationCount++;
        Console.WriteLine(notification);
    }
}