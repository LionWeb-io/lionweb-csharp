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

namespace LionWeb.Core.Test.Notification.Replicator.Containment;

using Core.Notification;
using Core.Notification.Partition;
using Languages.Generated.V2025_1.Shapes.M2;
using M1;

[TestClass]
public class ReplacedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Single()
    {
        var replaced = new Documentation("replaced") { Text = "a" };
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced
        };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        var added = new Documentation("added")
        {
            Text = "added"
        };

        Assert.IsTrue(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsFalse(sharedNodeMap.ContainsKey(added.GetId()));

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.Documentation = added;

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);

        Assert.IsFalse(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(added.GetId()));
    }

    [TestMethod]
    public void Deep()
    {
        var originalPartition = new Geometry("a");
        var bof = new BillOfMaterials("bof")
        {
            DefaultGroup = new MaterialGroup("mg") { MatterState = MatterState.liquid }
        };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        bof.DefaultGroup = new MaterialGroup("replaced") { MatterState = MatterState.gas };

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    [Ignore("Should emit ChildReplacedNotification")]
    public void Multiple_Only()
    {
        var replaced = new Circle("replaced");
        var replacement = new Line("replacement");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [replaced]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Only_ProducesNotification()
    {
        var replaced = new Circle("replaced");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [replaced]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        var replacement = new Line("replacement");

        Assert.IsTrue(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsFalse(sharedNodeMap.ContainsKey(replacement.GetId()));
        
        var childReplacedNotification = new ChildReplacedNotification(replacement, replaced, originalPartition, 
            ShapesLanguage.Instance.Geometry_shapes, 0, new NumericNotificationId("childReplacedNotification", 0));

        CreatePartitionReplicator(clonedPartition, childReplacedNotification);

        Assert.AreEqual(1, clonedPartition.Shapes.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Shapes[0].GetId());

        Assert.IsFalse(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(replacement.GetId()));
    }

    [TestMethod]
    [Ignore("Should emit ChildReplacedNotification")]
    public void Multiple_First()
    {
        var replaced = new Circle("replaced");
        var replacement = new Line("replacement");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [replaced, new Circle("child")]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        Assert.IsTrue(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsFalse(sharedNodeMap.ContainsKey(replacement.GetId()));

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);

        Assert.IsFalse(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(replacement.GetId()));
    }

    [TestMethod]
    public void Multiple_First_ProducesNotification()
    {
        var replaced = new Circle("replaced");
        var replacement = new Line("replacement");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [replaced, new Circle("child")]
        };
        var clonedPartition = ClonePartition(originalPartition);
        
        var childReplacedNotification = new ChildReplacedNotification(replacement, replaced, originalPartition, 
            ShapesLanguage.Instance.Geometry_shapes, 0, new NumericNotificationId("childReplacedNotification", 0));

        CreatePartitionReplicator(clonedPartition, childReplacedNotification);

        Assert.AreEqual(2, clonedPartition.Shapes.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Shapes[0].GetId());
    }

    [TestMethod]
    [Ignore("Should emit ChildReplacedNotification")]
    public void Multiple_Last()
    {
        var replaced = new Circle("replaced");
        var replacement = new Line("replacement");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [new Circle("child"), replaced]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last_ProducesNotification()
    {
        var replaced = new Circle("replaced");
        var replacement = new Line("replacement");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [new Circle("child"), replaced]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var childReplacedNotification = new ChildReplacedNotification(replacement, replaced, originalPartition, 
            ShapesLanguage.Instance.Geometry_shapes, 1, new NumericNotificationId("childReplacedNotification", 0));

        CreatePartitionReplicator(clonedPartition, childReplacedNotification);

        Assert.AreEqual(2, clonedPartition.Shapes.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Shapes[^1].GetId());
    }

    [TestMethod]
    public void Multiple_not_matching_node_ids()
    {
        var replaced = new Circle("replaced");
        var replacement = new Line("replacement");
        var nodeWithAnotherId = new Circle("node-with-another-id");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [new Circle("child"), replaced, nodeWithAnotherId]
        };
        
        var clonedPartition = ClonePartition(originalPartition);
        
        var notification = new ChildReplacedNotification(replacement, nodeWithAnotherId, originalPartition, 
            ShapesLanguage.Instance.Geometry_shapes, 1, new NumericNotificationId("childReplacedNotification", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }

    [TestMethod]
    public void Single_not_matching_node_ids()
    {
        var replaced = new Documentation("replaced") { Text = "replaced" };
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var added = new Documentation("added") { Text = "added" };
        var nodeWithAnotherId = new Documentation("node-with-another-id") { Text = "another node" };

        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(added);
        sharedNodeMap.RegisterNode(nodeWithAnotherId);

        var notification = new ChildReplacedNotification(added, nodeWithAnotherId, originalPartition, ShapesLanguage.Instance.Geometry_documentation, 
            0, new NumericNotificationId("childReplacedNotification", 0));

        AssertUniqueNodeIds(originalPartition, added, nodeWithAnotherId);
        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        });
    }
    
    /// <summary>
    /// A floating node is a node which is not part of any partition.
    /// In the test below, replacement node is a floating node,
    /// therefore; as a result of replacement, <see cref="ChildReplacedNotification"/> is emitted.  
    /// </summary>
    [TestMethod]
    public void Floating_node_replaces_existing_node()
    {
        var replaced = new Circle("replaced")
        {
            Center = new Coord("cc") { X = 2 }
        };

        var replacement = new Line("replacement")
        {
            Start = new Coord("sc") { X = 1 }
        };

        var originalPartition = new Geometry("a")
        {
            Shapes = [new Circle("child"), replaced]
        };

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.Center = replacement.Start;

        AssertUniqueNodeIds(originalPartition, replacement);
        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    /// <summary>
    /// Replaced and replacement nodes are part of a (same) partition.
    /// Therefore <see cref="ChildMovedAndReplacedFromOtherContainmentNotification"/> is emitted.
    /// </summary>
    [TestMethod]
    public void Nodes_are_in_same_partition()
    {
        var replaced = new Circle("replaced")
        {
            Center = new Coord("cc") { X = 2 }
        };

        var replacement = new Line("replacement")
        {
            Start = new Coord("sc") { X = 1 }
        };

        var originalPartition = new Geometry("a")
        {
            Shapes = [replacement, replaced]
        };

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.Center = replacement.Start;

        AssertUniqueNodeIds(originalPartition);
        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    /// <summary>
    /// Partitions do not know about their forests. That's why test below emits
    /// <see cref="ChildMovedAndReplacedFromOtherContainmentNotification"/>
    /// The correct notification would be <see cref="ChildReplacedNotification"/>.
    /// </summary>
    [TestMethod]
    public void Origin_partition_is_not_in_any_forest()
    {
        var replaced = new Circle("replaced")
        {
            Center = new Coord("cc") { X = 2 }
        };
        
        var destinationPartition = new Geometry("a")
        {
            Shapes = [replaced]
        };
        
        var replacement = new Line("replacement")
        {
            Start = new Coord("sc") { X = 1 }
        };
        
        var originPartition = new Geometry("b")
        {
            Shapes = [replacement]
        };
        
        var originalForest = new Forest();
        originalForest.AddPartitions([destinationPartition]);

        var notificationObserver = new NotificationObserver();
        destinationPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.Center = replacement.Start;

        AssertUniqueNodeIds(originPartition, destinationPartition);
        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);
    }

    /// <summary>
    /// Partitions do not know about their forests. That's why test below emits
    /// <see cref="ChildMovedAndReplacedFromOtherContainmentNotification"/>
    /// The correct notification would be <see cref="ChildReplacedNotification"/>.
    /// </summary>
    [TestMethod]
    public void Origin_partition_is_in_different_forest()
    {
        var replaced = new Circle("replaced")
        {
            Center = new Coord("cc") { X = 2 }
        };
        
        var destinationPartition = new Geometry("a")
        {
            Shapes = [replaced]
        };
        
        var replacement = new Line("replacement")
        {
            Start = new Coord("sc") { X = 1 }
        };
        
        var originPartition = new Geometry("b")
        {
            Shapes = [replacement]
        };

        var otherForest = new Forest();
        otherForest.AddPartitions([originPartition]);

        var originalForest = new Forest();
        originalForest.AddPartitions([destinationPartition]);

        var notificationObserver = new NotificationObserver();
        destinationPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.Center = replacement.Start;

        AssertUniqueNodeIds(originPartition, destinationPartition);
        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);
    }
}