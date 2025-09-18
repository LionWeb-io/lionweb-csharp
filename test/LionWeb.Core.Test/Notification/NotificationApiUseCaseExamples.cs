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
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class NotificationApiUseCaseExamples : NotificationTestsBase
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
        // tag::partition_changes[]
        var partition = new Geometry("geo");
        var receiver = new NotificationCounter();
        
        partition.GetNotificationSender()?.ConnectTo(receiver); // <1>

        partition.Documentation = new Documentation("added");   // <2>
        // end::partition_changes[]
        Assert.AreEqual(1, receiver.Count);
    }

    [TestMethod]
    public void ReceiveNotifications_from_forest()
    {
        // tag::forest_changes[]
        var forest = new Forest();
        var receiver = new NotificationCounter();

        forest.GetNotificationSender()?.ConnectTo(receiver); // <1>

        var partition = new Geometry("geo");
        forest.AddPartitions([partition]);                   // <2>
        partition.Documentation = new Documentation("doc");  // <3>
        // end::forest_changes[]
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

        Assert.AreEqual(2, forestReceiver.Count);
        Assert.AreEqual(1, partitionReceiver.Count);
    }

    #endregion

    #region collect several changes into one change set

    #region use case example

    private delegate void PartitionUpdater(Geometry partition);


    // tag::update_documentation[]
    private void UpdateDocumentation(Geometry partition)
    {
        partition.Documentation = new Documentation("documentation"); // <1>
        partition.Documentation.Text = "hello";                       // <2>
    }
    // end::update_documentation[]

    private CompositeNotification ComposeNotifications(Geometry partition, PartitionUpdater updater)
    {
        var compositor = new NotificationCompositor("compositor");

        partition.GetNotificationSender()!.ConnectTo(compositor);

        compositor.Push();
        updater.Invoke(partition);
        return compositor.Pop();
    }

    [TestMethod]
    public void CountCompositeNotificationParts()
    {
        var partition = new Geometry("geo");
        var changes = ComposeNotifications(partition, UpdateDocumentation);

        Assert.AreEqual(2, changes.Parts.Count);
    }

    [TestMethod]
    public void CountCompositeNotificationParts_simple()
    {
        // tag::composite_notification[]
        var partition = new Geometry("geo");
        var compositor = new NotificationCompositor("compositor");

        var sender = partition.GetNotificationSender();       // <1>
        sender?.ConnectTo(compositor);                        // <2>

        compositor.Push();                                    // <3>
        UpdateDocumentation(partition);                       // <4>
        var changes = compositor.Pop();                       // <5>
        
        foreach (INotification notification in changes.Parts) // <6>
        {
            Console.WriteLine(notification.ToString());
        }
        // end::composite_notification[]
        Assert.AreEqual(2, changes.Parts.Count);
    }

    #endregion


    [TestMethod]
    public void ComposeNotifications_into_a_composite_notification()
    {
        var partition = new Geometry("geo");

        var compositor = new NotificationCompositor("compositor");
        var counter = new EventCounter();

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

        public void Receive(INotificationSender _, INotification notification)
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

        public void Receive(INotificationSender _, INotification notification)
        {
            if (notification is not CompositeNotification)
            {
                Count++;
            }

            Send(notification);
        }
    }
    // tag::notification_counter[]
    private class NotificationCounter: INotificationReceiver
    {
        public int Count { get; private set; }

        public void Receive(INotificationSender _, INotification notification) =>
            Count++;
    }
    // end::notification_counter[]

    #endregion

    #region replicate changes

    #region partition replicator use case example

    //tag::creator[]
    private class Creator() : NotificationPipeBase(null), INotificationProducer
    {
        public void ProduceNotification(INotification notification) => Send(notification);
    }
    //end::creator[]
    
    //tag::replicate_changes_on[]
    private void ReplicateChangesOn(Geometry localPartition, IEnumerable<INotification> changes)
    {
        var sharedNodeMap = new SharedNodeMap();                                     // <1>
        var replicator = 
            PartitionReplicator.Create(localPartition, sharedNodeMap, "replicator"); // <2>

        var creator = new Creator();                                                 // <3>
        creator.ConnectTo(replicator);                                               // <4>

        foreach (var notification in changes)
        {
            creator.ProduceNotification(notification);                               // <5>
        }
    }
    //end::replicate_changes_on[]

    private IEnumerable<INotification> GetChangesOn(Geometry localPartition)
    {
        var documentation = new Documentation("documentation");
        
        return
        [
            new ChildAddedNotification(localPartition, documentation,
                ShapesLanguage.Instance.Geometry_documentation, 0, new NumericNotificationId("ChildAddedNotification", 0)),
            
            new PropertyAddedNotification(documentation, ShapesLanguage.Instance.Documentation_text, 
                "hello", new NumericNotificationId("PropertyAddedNotification", 0))
        ];
    }

    [TestMethod]
    public void PartitionReplicator_use_case()
    {
        //tag::partition_replicator_1[]
        var localPartition = new Geometry("geo");    // <1>
        //end::partition_replicator_1[]
        IEnumerable<INotification> changes = GetChangesOn(localPartition);

        //tag::partition_replicator_2[]
        ReplicateChangesOn(localPartition, changes); // <2>
        //end::partition_replicator_2[]

        AssertEquals([new Geometry("geo") { Documentation = new Documentation("documentation") {Text = "hello"} }], [localPartition]);
    }

    #endregion

    #region forest replicator use case example
    

    private void ReplicateChangesOn(Forest localForest, IEnumerable<INotification> changes)
    {
        var replicator = ForestReplicator.Create(localForest, new SharedNodeMap(), "forest replicator");

        var creator = new Creator();
        creator.ConnectTo(replicator);

        foreach (var notification in changes)
        {
            creator.ProduceNotification(notification);
        }
    }

    
    private IEnumerable<INotification> GetChanges()
    {
        var partition = new Geometry("geo");
        var documentation = new Documentation("documentation");
        
        return
        [
            new PartitionAddedNotification(partition, new NumericNotificationId("PartitionAddedNotification", 0)),
            
            new ChildAddedNotification(partition, documentation,
                ShapesLanguage.Instance.Geometry_documentation, 0, new NumericNotificationId("ChildAddedNotification", 0)),
            
            new PropertyAddedNotification(documentation, ShapesLanguage.Instance.Documentation_text, 
                "hello", new NumericNotificationId("PropertyAddedNotification", 0))
        ];
    }
    
    [TestMethod]
    public void ForestReplicator_use_case()
    {
        var localForest = new Forest();
        
        IEnumerable<INotification> changes = GetChanges();

        ReplicateChangesOn(localForest, changes);

        Assert.AreEqual(1, localForest.Partitions.Count);
        AssertEquals([new Geometry("geo") { Documentation = new Documentation("documentation") {Text = "hello"} }], [localForest.Partitions.First()]);
    }
    
    #endregion
    
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
}