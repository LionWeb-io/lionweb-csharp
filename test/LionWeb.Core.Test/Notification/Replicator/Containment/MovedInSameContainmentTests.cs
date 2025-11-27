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

using Core.Notification.Partition;
using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class MovedInSameContainmentTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Forward()
    {
        var moved = new Circle("moved");
        var originalPartition = new Geometry("a") { Shapes = [moved, new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.AddShapes([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Backward()
    {
        var moved = new Circle("moved");
        var originalPartition = new Geometry("a") { Shapes = [new Line("l"), moved] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.InsertShapes(0, [moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void Adds_first_of_the_existing_children()
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
    public void Adds_two_of_the_existing_children()
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
    public void Sets_two_of_the_existing_children_using_set()
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

        var values = new IShape[] { a, b };
        originalPartition.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        Assert.AreEqual(2, notificationObserver.Count);

        foreach (var notification in notificationObserver.Notifications)
        {
            Assert.IsInstanceOfType<ChildDeletedNotification>(notification);
            var childDeletedNotification = notification as ChildDeletedNotification;
            Assert.AreEqual(2, childDeletedNotification?.Index);
        }

        AssertEquals(originalPartition.Shapes, clonedPartition.Shapes);
    }

    [TestMethod]
    public void Adds_three_of_the_existing_children()
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
    public void Adds_three_of_the_existing_children_in_a_mixed_order()
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
    public void Adds_new_node_to_the_existing_children()
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