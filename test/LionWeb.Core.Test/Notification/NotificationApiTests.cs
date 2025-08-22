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

        var sender = partition.GetNotificationHandler();
        var cloneHandler = new TestNodeCloneNotificationHandler(partition.GetId());
        if (sender != null)
        {
            INotificationHandler.Connect(from: sender, to: cloneHandler);
        }

        var clone = Clone(partition);
        var replicator = PartitionReplicator.Create(clone, new SharedNodeMap(), sender: partition.GetId());
        INotificationHandler.Connect(from: cloneHandler, to: replicator);
        
        circle.Name = "Hello";

        AssertEquals([partition], [clone]);
    }

    
    [TestMethod]
    public void ReplicateChanges_Forest()
    {
        var moved = new Documentation("moved");
        var originPartition = new Geometry("origin-geo") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var partition = new Geometry("geo");

        var originalForest = new Forest();
        var cloneForest = new Forest();

        var sender = originalForest.GetNotificationHandler();
        var cloneHandler = new TestNodeCloneNotificationHandler("forestCloner");
        INotificationHandler.Connect(from: sender, to: cloneHandler);
        
        var replicator = ForestReplicator.Create(cloneForest, new SharedNodeMap(), null);
        INotificationHandler.Connect(from: cloneHandler, to: replicator);
        
        var receiver = new TestForestChangeNotificationHandler(originalForest, cloneHandler);
        INotificationHandler.Connect(from: sender, to: receiver);
        
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


internal class TestNodeCloneNotificationHandler(object? sender) : NotificationHandlerBase(sender), 
    IConnectingNotificationHandler
{
    private readonly Dictionary<IReadableNode, IReadableNode> _memoization = [];
    private readonly Dictionary<ISendingNotificationHandler, ISendingNotificationHandler> _handlerMemoization = [];
    
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        INotification result = notification switch
        {
            PartitionAddedNotification e => e with { NewPartition = Clone(e.NewPartition) },
            PropertyAddedNotification e => e with { Node = Clone(e.Node), },
            ChildMovedFromOtherContainmentNotification e => e with
            {
                MovedChild = Clone(e.MovedChild), NewParent = Clone(e.NewParent), OldParent = Clone(e.OldParent)
            },
        };

        if (result is IForestNotification f)
        {
            _handlerMemoization[correspondingHandler] = f.Partition.GetNotificationHandler();
            SendWithSender(f.Partition.GetNotificationHandler(), f);
        } else
            SendWithSender(_handlerMemoization.GetValueOrDefault(correspondingHandler, correspondingHandler), result);
    }
    
    private T Clone<T>(T node) where T : class?, IReadableNode? =>
        (T)(_memoization.TryGetValue(node, out var result)
            ? result
            : _memoization[node] = SameIdCloner.Clone((INode)node));
} 

internal class TestForestChangeNotificationHandler(object? sender, TestNodeCloneNotificationHandler cloneHandler)
    : NotificationHandlerBase(sender), IReceivingNotificationHandler
{
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        switch (notification)
        {
            case PartitionAddedNotification partitionAddedNotification:
                OnLocalPartitionAdded(partitionAddedNotification);
                break;
        }
    }

    private void OnLocalPartitionAdded(PartitionAddedNotification partitionAddedNotification) => 
        INotificationHandler.Connect(from: partitionAddedNotification.NewPartition.GetNotificationHandler(), to: cloneHandler);
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