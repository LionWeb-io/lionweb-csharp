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

[TestClass]
public class NotificationApiTests : NotificationTestsBase
{
    #region get informed about changes

    [TestMethod]
    public void ReceiveNotifications_from_partition_via_subscribe()
    {
        var partition = new Geometry("geo");

        int notificationCount = 0;
        var sender = partition.GetNotificationSender();
        sender!.Subscribe<IPartitionNotification>((_, notification) =>
        {
            notificationCount++;
            Console.WriteLine(notification);
        });

        partition.Documentation = new Documentation("added");

        Assert.AreEqual(1, notificationCount);
    }


    [TestMethod]
    public void ReceiveNotifications_from_partition_via_connected_pipes()
    {
        var partition = new Geometry("geo");
        var receiver = new NotificationCounter();

        partition.GetNotificationSender()!.ConnectTo(receiver);

        partition.Documentation = new Documentation("added");

        Assert.AreEqual(1, receiver.Count);
    }

    [TestMethod]
    public void ReceiveNotifications_from_forest()
    {
        var forest = new Forest();
        var receiver = new NotificationCounter();

        forest.GetNotificationSender()!.ConnectTo(receiver);

        var partition = new Geometry("geo");
        forest.AddPartitions([partition]);

        Assert.AreEqual(1, receiver.Count);
    }

    [TestMethod]
    public void ReceiveNotifications_from_forest_and_partition()
    {
        var partition = new Geometry("geo");
        var forest = new Forest();

        var forestReceiver = new NotificationCounter();
        var partitionReceiver = new NotificationCounter();

        forest.GetNotificationSender()!.ConnectTo(forestReceiver);
        partition.GetNotificationSender()!.ConnectTo(partitionReceiver);

        forest.AddPartitions([partition]);
        partition.Documentation = new Documentation("added");

        Assert.AreEqual(1, forestReceiver.Count);
        Assert.AreEqual(1, partitionReceiver.Count);
    }
    
    
    #endregion

    #region collect several changes into one change set

    [TestMethod]
    public void ComposeNotifications_into_a_composite_notification()
    {
        var partition = new Geometry("geo");

        var compositor = new NotificationCompositor("compositor");
        var counter = new PartitionEventCounter();

        partition.GetNotificationSender()!.ConnectTo(compositor);
        compositor.ConnectTo(counter);

        var composite = compositor.Push();

        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";
        partition.AddShapes([new Circle("c")]);

        Assert.AreEqual(3, composite.Parts.Count);
        Assert.AreEqual(0, counter.Count);

        compositor.Pop(true);
        Assert.AreEqual(1, counter.Count);
    }

    [TestMethod]
    public void CountNotificationTypes_pipeline_pipes()
    {
        var partition = new Geometry("geo");

        var compositor = new NotificationCompositor("compositor");
        var compositeNotificationCounter = new CompositeNotificationCounter();
        var rawNotificationCounter = new RawNotificationCounter();
        var allNotificationCounter = new NotificationCounter();

        // partition -> compositor -> raw -> composite -> all
        partition.GetNotificationSender()!.ConnectTo(compositor);
        compositor.ConnectTo(rawNotificationCounter);
        rawNotificationCounter.ConnectTo(compositeNotificationCounter);
        compositeNotificationCounter.ConnectTo(allNotificationCounter);

        partition.AddShapes([new Line("l")]);

        _ = compositor.Push();

        partition.Documentation = new Documentation("documentation") { Text = "hello" };
        partition.AddShapes([new Circle("c1")]);

        compositor.Pop(true);

        partition.AddShapes([new Circle("c2")]);

        Assert.AreEqual(allNotificationCounter.Count,
            rawNotificationCounter.Count + compositeNotificationCounter.Count);
        Assert.AreEqual(3, allNotificationCounter.Count);
        Assert.AreEqual(2, rawNotificationCounter.Count);
        Assert.AreEqual(1, compositeNotificationCounter.Count);
    }

    [TestMethod]
    public void CountNotificationTypes_fanout_pipes()
    {
        var partition = new Geometry("geo");

        var compositor = new NotificationCompositor("compositor");
        var compositeNotificationCounter = new CompositeNotificationCounter();
        var rawNotificationCounter = new RawNotificationCounter();
        var allNotificationCounter = new NotificationCounter();

        // partition -> compositor -> raw | composite | all
        partition.GetNotificationSender()!.ConnectTo(compositor);
        compositor.ConnectTo(rawNotificationCounter);
        compositor.ConnectTo(compositeNotificationCounter);
        compositor.ConnectTo(allNotificationCounter);

        partition.AddShapes([new Line("l")]);

        _ = compositor.Push();

        partition.Documentation = new Documentation("documentation") { Text = "hello" };
        partition.AddShapes([new Circle("c1")]);

        compositor.Pop(true);

        partition.AddShapes([new Circle("c2")]);

        Assert.AreEqual(allNotificationCounter.Count,
            rawNotificationCounter.Count + compositeNotificationCounter.Count);
        Assert.AreEqual(3, allNotificationCounter.Count);
        Assert.AreEqual(2, rawNotificationCounter.Count);
        Assert.AreEqual(1, compositeNotificationCounter.Count);
    }

    [TestMethod]
    [Ignore("introduces cycle")]
    public void CountNotificationTypes_cycle_pipes()
    {
        var partition = new Geometry("geo");

        var compositor = new NotificationCompositor("compositor");
        var compositeNotificationCounter = new CompositeNotificationCounter();

        // partition -> composite -> compositor -> composite 
        partition.GetNotificationSender()!.ConnectTo(compositeNotificationCounter);
        compositeNotificationCounter.ConnectTo(compositor);
        compositor.ConnectTo(compositeNotificationCounter);

        partition.AddShapes([new Line("l")]);

        _ = compositor.Push();

        partition.Documentation = new Documentation("documentation") { Text = "hello" };
        partition.AddShapes([new Circle("c1")]);

        compositor.Pop(true);

        partition.AddShapes([new Circle("c2")]);

        Assert.AreEqual(1, compositeNotificationCounter.Count);
    }

    private class CompositeNotificationCounter() : NotificationPipeBase(null), INotificationHandler
    {
        public int Count { get; private set; }

        public void Receive(INotificationSender correspondingSender, INotification notification)
        {
            if (notification is CompositeNotification)
            {
                Count++;
            }

            Send(notification);
        }
    }

    private class RawNotificationCounter() : NotificationPipeBase(null), INotificationHandler
    {
        public int Count { get; private set; }

        public void Receive(INotificationSender correspondingSender, INotification notification)
        {
            if (notification is not CompositeNotification)
            {
                Count++;
            }

            Send(notification);
        }
    }

    private class NotificationCounter() : NotificationPipeBase(null), INotificationHandler
    {
        public int Count { get; private set; }

        public void Receive(INotificationSender correspondingSender, INotification notification)
        {
            Count++;
            Send(notification);
        }
    }

    #endregion

    #region replicate changes

    [TestMethod]
    public void ReplicateChanges_Partition()
    {
        var circle = new Circle("c");
        var partition = new Geometry("geo") { Shapes = [circle] };
        var clone = Clone(partition);

        var replicator = PartitionReplicator.Create(clone, new(), partition.GetId());
        partition.GetNotificationSender()!.ConnectTo(replicator);

        circle.Name = "Hello";

        AssertEquals([partition], [clone]);
    }


    [TestMethod]
    public void ReplicateChanges_Forest()
    {
        var originalForest = new Forest();
        var cloneForest = new Forest();

        var replicator = ForestReplicator.Create(cloneForest, new(), null);
        originalForest.GetNotificationSender()!.ConnectTo(replicator);

        var moved = new Documentation("moved");
        var originPartition = new Geometry("origin-geo") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var partition = new Geometry("geo");

        originalForest.AddPartitions([partition, originPartition]);
        partition.Documentation = moved;

        AssertEquals([partition, originPartition], cloneForest.Partitions.OrderBy(p => p.GetId()).ToList());
    }

    #endregion

    private void AssertEquals(IEnumerable<IReadableNode?> expected, IEnumerable<IReadableNode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }

    protected override Geometry CreateReplicator(Geometry node) => throw new NotImplementedException();
}

