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
public class MovedAndReplacedFromOtherContainmentTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Single()
    {
        var moved = new Documentation("moved");
        var replaced = new Documentation("replaced");
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced, Shapes = [new Line("l") { ShapeDocs = moved }]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        Assert.IsTrue(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(moved.GetId()));

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.Documentation = moved;

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);

        AssertEquals([originalPartition], [clonedPartition]);

        Assert.IsFalse(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(moved.GetId()));
    }

    [TestMethod]
    public void Single_ReplaceWith()
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
    public void Multiple()
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
    public void Multiple_ProducesNotification()
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
    public void Multiple_not_matching_node_ids()
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
    public void Single_not_matching_node_ids()
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
}