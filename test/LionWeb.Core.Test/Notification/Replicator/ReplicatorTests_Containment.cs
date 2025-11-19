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
using Core.Notification.Partition;
using Languages.Generated.V2025_1.Shapes.M2;
using Languages.Generated.V2025_1.TestLanguage;
using M1;

[TestClass]
public class ReplicatorTests_Containment : ReplicatorTestsBase
{
    #region Children

    #region ChildAdded

    [TestMethod]
    public void ChildAdded_Multiple_Only()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        originalPartition.AddShapes([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_First()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        originalPartition.InsertShapes(0, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_Last()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        originalPartition.InsertShapes(1, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[1]);
    }

    [TestMethod]
    public void ChildAdded_Single()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Documentation("added");
        originalPartition.Documentation = added;

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Documentation);
    }

    [TestMethod]
    public void ChildAdded_Deep()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added") { Center = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        originalPartition.AddShapes([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }

    #endregion

    #region ChildDeleted

    [TestMethod]
    public void ChildDeleted_Multiple_Only()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [deleted] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_First()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [deleted, new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_Last()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [new Line("l"), deleted] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildDeleted_Single()
    {
        var deleted = new Documentation("deleted");
        var originalPartition = new Geometry("a") { Documentation = deleted };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.Documentation = null;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_not_matching_node_id()
    {
        var deleted = new Circle("deleted");
        var nodeWithAnotherId = new Circle("node-with-another-id");
        var originalPartition = new Geometry("a") { Shapes = [deleted, nodeWithAnotherId, new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildDeletedNotification(nodeWithAnotherId, originalPartition,
            ShapesLanguage.Instance.Geometry_shapes, 0, new NumericNotificationId("childDeletedNotificationMultiple", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }


    /// <summary>
    /// This test confirms that remote replicator is able to detach child from its parent
    /// which is a required multiple containment.
    /// </summary>
    [TestMethod]
    public void ChildDeleted_Multiple_required_containment()
    {
        var deleted = new Circle("deleted");
        var origin = new CompositeShape("origin") { Parts = [deleted] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildDeletedNotification(deleted, origin, ShapesLanguage.Instance.CompositeShape_parts, 0,
            new NumericNotificationId("childDeletedNotification", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.ThrowsExactly<UnsetFeatureException>(() => ((CompositeShape)clonedPartition.Shapes[0]).Parts);
    }

    /// <summary>
    /// This test confirms that remote replicator is able to detach child from its parent
    /// which is a required single containment.
    /// </summary>
    [TestMethod]
    public void ChildDeleted_Single_required_containment()
    {
        var deleted = new Coord("deleted");
        var origin = new Circle("c") { Center = deleted };

        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildDeletedNotification(deleted, origin, ShapesLanguage.Instance.Circle_center, 0,
            new NumericNotificationId("childDeletedNotification", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.ThrowsExactly<UnsetFeatureException>(() => ((Circle)clonedPartition.Shapes[0]).Center);
    }

    /// <summary>
    /// This test confirms that no notification is generated from DetachFromParent method
    /// TODO: This is a known bug, we want to have a notification emitted.
    /// </summary>
    [TestMethod]
    public void ChildDeleted_Single_uses_detach_from_parent()
    {
        var deleted = new Coord("deleted");
        var origin = new Circle("c") { Center = deleted };

        var originalPartition = new Geometry("a") { Shapes = [origin] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        deleted.DetachFromParent();

        Assert.AreEqual(0, notificationObserver.Notifications.Count);
    }

    [TestMethod]
    public void ChildDeleted_Single_not_matching_node_id()
    {
        var deleted = new Documentation("deleted");
        var nodeWithAnotherId = new Documentation("node-with-another-id");
        var originalPartition = new Geometry("a")
        {
            Documentation = deleted
        };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(nodeWithAnotherId);

        var notification = new ChildDeletedNotification(nodeWithAnotherId, originalPartition,
            ShapesLanguage.Instance.Geometry_documentation, 0, new NumericNotificationId("childDeletedNotificationSingle", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        });
    }

    #endregion

    #region ChildReplaced

    [TestMethod]
    public void ChildReplaced_Single()
    {
        var originalPartition = new Geometry("a")
        {
            Documentation = new Documentation("replaced") { Text = "a" }
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Documentation("added")
        {
            Text = "added"
        };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.Documentation = added;

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildReplaced_Deep()
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
    public void ChildReplaced_Multiple_Only()
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
    public void ChildReplaced_Multiple_Only_ProducesNotification()
    {
        var replaced = new Circle("replaced");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [replaced]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var replacement = new Line("replacement");
        var childReplacedNotification = new ChildReplacedNotification(replacement, replaced, originalPartition, 
            ShapesLanguage.Instance.Geometry_shapes, 0, new NumericNotificationId("childReplacedNotification", 0));

        CreatePartitionReplicator(clonedPartition, childReplacedNotification);

        Assert.AreEqual(1, clonedPartition.Shapes.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Shapes[0].GetId());
    }

    [TestMethod]
    [Ignore("Should emit ChildReplacedNotification")]
    public void ChildReplaced_Multiple_First()
    {
        var replaced = new Circle("replaced");
        var replacement = new Line("replacement");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [replaced, new Circle("child")]
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
    public void ChildReplaced_Multiple_First_ProducesNotification()
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
    public void ChildReplaced_Multiple_Last()
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
    public void ChildReplaced_Multiple_Last_ProducesNotification()
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
    public void ChildReplaced_Multiple_not_matching_node_ids()
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
    public void ChildReplaced_Single_not_matching_node_ids()
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

    #endregion

    #region ChildMovedAndReplacedInSameContainment

    [TestMethod]
    [Ignore("Should emit ChildMovedAndReplacedInSameContainmentNotification")]
    public void ChildMovedAndReplacedInSameContainment_Backward()
    {
        var replacement = new Line("replacement");
        var replaced = new Circle("replaced");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [new Circle("child"), replaced, replacement]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedInSameContainmentNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment_Forward_ProducesNotification()
    {
        var moved = new Circle("moved");
        var replaced = new Line("replaced");
        var originalPartition = new Geometry("a") { Shapes = [moved, replaced] };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, moved, originalPartition, ShapesLanguage.Instance.Geometry_shapes, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.AreEqual(1, clonedPartition.Shapes.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Shapes[0].GetId());
    }

    [TestMethod]
    [Ignore("Should emit ChildMovedAndReplacedInSameContainmentNotification")]
    public void ChildMovedAndReplacedInSameContainment_Forward()
    {
        var replacement = new Line("replacement");
        var replaced = new Circle("replaced");
        
        var originalPartition = new Geometry("a")
        {
            Shapes = [new Circle("child"), replacement, replaced]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedInSameContainmentNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment_BackwardP_ProducesNotification()
    {
        var moved = new Circle("moved");
        var replaced = new Line("replaced");
        var originalPartition = new Geometry("a") { Shapes = [replaced, moved] };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 0;
        var oldIndex = 1;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, moved, originalPartition, ShapesLanguage.Instance.Geometry_shapes, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.AreEqual(1, clonedPartition.Shapes.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Shapes[0].GetId());
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment_Backward_MoreThanThreeChildren_ProducesNotification()
    {
        var replacement = new Line("E");
        var replaced = new Circle("B");

        var originalPartition = new Geometry("container")
        {
            Shapes = [new Circle("A"), replaced, new Circle("C"), new Circle("D"), replacement, new Circle("F")]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 4;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, replacement, originalPartition, ShapesLanguage.Instance.Geometry_shapes, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(5, clonedPartition.Shapes.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Shapes[1].GetId());
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment_Forward_MoreThanThreeChildren_ProducesNotification()
    {
        var replacement = new Line("E");
        var replaced = new Circle("B");

        var originalPartition = new Geometry("container")
        {
            Shapes = [new Circle("A"), replacement, new Circle("C"), new Circle("D"), replaced, new Circle("F")]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 4;
        var oldIndex = 1;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, replacement, originalPartition, ShapesLanguage.Instance.Geometry_shapes, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(5, clonedPartition.Shapes.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Shapes[^2].GetId());
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment_not_matching_node_ids()
    {
        var moved = new Circle("moved");
        var replaced = new Line("replaced");
        var nodeWithAnotherId = new Line("node-with-another-id");
        var originalPartition = new Geometry("a") { Shapes = [moved, replaced, nodeWithAnotherId] };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, moved, originalPartition, ShapesLanguage.Instance.Geometry_shapes, 
            nodeWithAnotherId, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }

    #endregion

    #region ChildMovedFromOtherContainment

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.AddShapes([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single()
    {
        var moved = new Documentation("moved");
        var originalPartition = new Geometry("a")
        {
            Shapes = [new Line("l") { ShapeDocs = moved }]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.Documentation = moved;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion

    #region ChildMovedAndReplacedFromOtherContainment

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Single()
    {
        var moved = new Documentation("moved");
        var replaced = new Documentation("replaced");
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced, Shapes = [new Line("l") { ShapeDocs = moved }]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.Documentation = moved;

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Single_ReplaceWith()
    {
        var moved = new Documentation("moved");
        var replaced = new Documentation("replaced");
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced, Shapes = [new Line("l") { ShapeDocs = moved }]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.ReplaceWith(moved);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    [Ignore("Should emit ChildMovedAndReplacedFromOtherContainmentNotification")]
    public void ChildMovedAndReplacedFromOtherContainment_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var replaced = new Circle("replaced");
        var originalPartition = new Geometry("a") { Shapes = [origin, replaced] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.ReplaceWith(moved);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Multiple_ProducesNotification()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var replaced = new Circle("replaced");
        var originalPartition = new Geometry("a") { Shapes = [origin, replaced] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var notification = new ChildMovedAndReplacedFromOtherContainmentNotification(originalPartition, ShapesLanguage.Instance.Geometry_shapes, 
            newIndex, moved, origin, ShapesLanguage.Instance.CompositeShape_parts, oldIndex, replaced, new NumericNotificationId("childMovedAndReplacedFromOtherContainment", 0));
        
        CreatePartitionReplicator(clonedPartition, notification);

        Assert.AreEqual(2, clonedPartition.Shapes.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Shapes[^1].GetId());
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Multiple_not_matching_node_ids()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var replaced = new Circle("replaced");
        var nodeWithAnotherId = new Circle("node-with-another-id");
        var originalPartition = new Geometry("a") { Shapes = [origin, replaced, nodeWithAnotherId] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var notification = new ChildMovedAndReplacedFromOtherContainmentNotification(originalPartition, ShapesLanguage.Instance.Geometry_shapes,
            newIndex, moved, origin, ShapesLanguage.Instance.CompositeShape_parts, oldIndex, nodeWithAnotherId,
            new NumericNotificationId("childMovedAndReplacedFromOtherContainment", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Single_not_matching_node_ids()
    {
        var moved = new Documentation("moved");
        var replaced = new Documentation("replaced");
        var line = new Line("l")
        {
            ShapeDocs = moved
        };
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced, 
            Shapes = [line]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var nodeWithAnotherId = new Circle("node-with-another-id");
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(nodeWithAnotherId);

        var newIndex = 0;
        var oldIndex = 0;
        var notification = new ChildMovedAndReplacedFromOtherContainmentNotification(originalPartition, ShapesLanguage.Instance.Geometry_documentation,
            newIndex, moved, line, ShapesLanguage.Instance.Geometry_shapes, oldIndex, nodeWithAnotherId,
            new NumericNotificationId("childMovedAndReplacedFromOtherContainment", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        });
    }

    #endregion

    #region ChildMovedAndReplacedFromOtherContainmentInSameParent

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent_Single()
    {
        var line = new Line("l")
        {
            Start = new Coord("moved"), End = new Coord("replaced")
        };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonedPartition = ClonePartition(originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        line.End = line.Start;

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(notificationObserver.Notifications[0]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent_Single_ReplaceWith()
    {
        var line = new Line("l")
        {
            Start = new Coord("moved"), End = new Coord("replaced")
        };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        line.End.ReplaceWith(line.Start);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(notificationObserver.Notifications[0]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent_Single_not_matching_node_ids()
    {
        var moved = new Coord("moved");
        var replaced = new Coord("replaced");
        var line = new Line("l")
        {
            Start = moved, 
            End = replaced
        };
        
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonedPartition = ClonePartition(originalPartition);

        var nodeWithAnotherId = new Coord("node-with-another-id");
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(nodeWithAnotherId);

        var notification = new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(ShapesLanguage.Instance.Geometry_shapes, 0, 
            moved, originalPartition, ShapesLanguage.Instance.Geometry_shapes, 0, nodeWithAnotherId, 
            new NumericNotificationId("childMovedAndReplacedFromOtherContainmentInSameParentNotification", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        });
    }


    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParentNotification_not_matching_node_ids()
    {
        var moved = new LinkTestConcept("moved");
        var replaced = new LinkTestConcept("replaced");
        var nodeWithAnotherId = new LinkTestConcept("node-with-another-id");
        var originalPartition = new LinkTestConcept("a")
        {
            Containment_0_n = [moved],
            Containment_1_n = [replaced, nodeWithAnotherId]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 
            0, moved, originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, nodeWithAnotherId, 
            new NumericNotificationId("childMovedAndReplacedFromOtherContainmentInSameParentNotification", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }

    #endregion

    #region ChildMovedFromOtherContainmentInSameParent

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        origin.AddDisabledParts([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Single()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        origin.EvilPart = moved;

        AssertEquals([originalPartition], [clonedPartition]);
    }


    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Reverse_first_moves_required_containment()
    {
        var moved1 = new LinkTestConcept("moved1");
        var moved2 = new LinkTestConcept("moved2");
        var originalPartition = new LinkTestConcept("a")
        {
            Containment_0_1 = moved1,
            Containment_1 = moved2,
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.Containment_0_1 = moved2; // emits ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        originalPartition.Containment_1 = moved1; // emits ChildAddedNotification

        AssertEquals([originalPartition], [clonedPartition]);
    }


    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Reverse_first_moves_optional_containment()
    {
        var moved1 = new LinkTestConcept("moved1");
        var moved2 = new LinkTestConcept("moved2");
        var originalPartition = new LinkTestConcept("a")
        {
            Containment_0_1 = moved1,
            Containment_1 = moved2,
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.Containment_1 = moved1; // emits ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        originalPartition.Containment_0_1 = moved2; // emits ChildAddedNotification

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    #endregion

    #region ChildMovedInSameContainment

    [TestMethod]
    public void ChildMovedInSameContainment_Forward()
    {
        var moved = new Circle("moved");
        var originalPartition = new Geometry("a") { Shapes = [moved, new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.AddShapes([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_Backward()
    {
        var moved = new Circle("moved");
        var originalPartition = new Geometry("a") { Shapes = [new Line("l"), moved] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.InsertShapes(0, [moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion

    #endregion

    /// <summary>
    /// A floating node is a node which is not part of any partition.
    /// In the test below, replacement node is a floating node,
    /// therefore; as a result of replacement, <see cref="ChildReplacedNotification"/> is emitted.  
    /// </summary>
    [TestMethod]
    public void ChildReplaced_floating_node_replaces_existing_node()
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
    public void ChildReplaced_nodes_are_in_same_partition()
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

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_partitions_are_in_same_forest()
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
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([originPartition, destinationPartition]);

        var notificationObserver = new NotificationObserver();
        destinationPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.Center = replacement.Start;

        AssertUniqueNodeIds(originPartition, destinationPartition);
        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }

    /// <summary>
    /// Partitions do not know about their forests. That's why test below emits
    /// <see cref="ChildMovedAndReplacedFromOtherContainmentNotification"/>
    /// The correct notification would be <see cref="ChildReplacedNotification"/>.
    /// </summary>
    [TestMethod]
    public void ChildReplaced_origin_partition_is_not_in_any_forest()
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
    public void ChildReplaced_origin_partition_is_in_different_forest()
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

    [TestMethod]
    public void ChildMovedInSameContainment_adds_first_of_the_existing_children()
    {
        var a = new Circle("a");
        var b = new Circle("b");
        var c = new Circle("c");
        var d = new Circle("d");
        var originalPartition = new Geometry("geo") { Shapes = [a, b, c, d] };

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.AddShapes([a]);

        Assert.AreEqual(1, notificationObserver.Count);
        
        var notification = notificationObserver.Notifications[0];
        Assert.IsInstanceOfType<ChildMovedInSameContainmentNotification>(notification);
        
        var childMovedInSameContainmentNotification = notification as ChildMovedInSameContainmentNotification;
        Assert.AreEqual(0, childMovedInSameContainmentNotification!.OldIndex);
        Assert.AreEqual(3, childMovedInSameContainmentNotification.NewIndex);
        
        AssertEquals(originalPartition.Shapes, clonedPartition.Shapes);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_adds_two_of_the_existing_children()
    {
        var a = new Circle("a");
        var b = new Circle("b");
        var c = new Circle("c");
        var d = new Circle("d");
        var originalPartition = new Geometry("geo") { Shapes = [a, b, c, d] };

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.AddShapes([a, b]);

        Assert.AreEqual(2, notificationObserver.Count);

        foreach (var notification in notificationObserver.Notifications)
        {
            Assert.IsInstanceOfType<ChildMovedInSameContainmentNotification>(notification);
            var childMovedInSameContainmentNotification = notification as ChildMovedInSameContainmentNotification;
            Assert.AreEqual(0, childMovedInSameContainmentNotification?.OldIndex);
            Assert.AreEqual(3, childMovedInSameContainmentNotification?.NewIndex);
        }
        
        AssertEquals(originalPartition.Shapes, clonedPartition.Shapes);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_adds_three_of_the_existing_children()
    {
        var a = new Circle("a");
        var b = new Circle("b");
        var c = new Circle("c");
        var d = new Circle("d");
        var originalPartition = new Geometry("geo") { Shapes = [a, b, c, d] };

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.AddShapes([a, b, c]);

        Assert.AreEqual(3, notificationObserver.Count);

        foreach (var notification in notificationObserver.Notifications)
        {
            Assert.IsInstanceOfType<ChildMovedInSameContainmentNotification>(notification);
            var childMovedInSameContainmentNotification = notification as ChildMovedInSameContainmentNotification;
            Assert.AreEqual(0, childMovedInSameContainmentNotification?.OldIndex);
            Assert.AreEqual(3, childMovedInSameContainmentNotification?.NewIndex);
        }
        
        AssertEquals(originalPartition.Shapes, clonedPartition.Shapes);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_adds_three_of_the_existing_children_in_a_mixed_order()
    {
        var a = new Circle("a");
        var b = new Circle("b");
        var c = new Circle("c");
        var d = new Circle("d");
        var originalPartition = new Geometry("geo") { Shapes = [a, b, c, d] };

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.AddShapes([c, a, b]);

        Assert.AreEqual(3, notificationObserver.Count);

        foreach (var notification in notificationObserver.Notifications)
        {
            Assert.IsInstanceOfType<ChildMovedInSameContainmentNotification>(notification);
        }
        
        var firstNotification = notificationObserver.Notifications[0] as ChildMovedInSameContainmentNotification;
        Assert.AreEqual(2, firstNotification?.OldIndex);
        Assert.AreEqual(3, firstNotification?.NewIndex);
        
        var secondNotification = notificationObserver.Notifications[1] as ChildMovedInSameContainmentNotification;
        Assert.AreEqual(0, secondNotification?.OldIndex);
        Assert.AreEqual(3, secondNotification?.NewIndex);
        
        var lastNotification = notificationObserver.Notifications[^1] as ChildMovedInSameContainmentNotification;
        Assert.AreEqual(0, lastNotification?.OldIndex);
        Assert.AreEqual(3, lastNotification?.NewIndex);
        
        AssertEquals(originalPartition.Shapes, clonedPartition.Shapes);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_adds_new_node_to_the_existing_children()
    {
        var a = new Circle("a");
        var b = new Circle("b");
        var c = new Circle("c");
        var d = new Circle("d");
        var originalPartition = new Geometry("geo") { Shapes = [a, b, c, d] };

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        var e = new Circle("e");
        originalPartition.AddShapes([a, b, c, d, e]);

        Assert.AreEqual(5, notificationObserver.Count);

        foreach (var notification in notificationObserver.Notifications[..4])
        {
            Assert.IsInstanceOfType<ChildMovedInSameContainmentNotification>(notification);
            var childMovedInSameContainmentNotification = notification as ChildMovedInSameContainmentNotification;
            Assert.AreEqual(0, childMovedInSameContainmentNotification?.OldIndex);
            Assert.AreEqual(3, childMovedInSameContainmentNotification?.NewIndex);
        }

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[^1]);

        AssertEquals(originalPartition.Shapes, clonedPartition.Shapes);
    }
}