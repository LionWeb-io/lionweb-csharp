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

[TestClass]
public class DeletedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [deleted] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_First()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [deleted, new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [new Line("l"), deleted] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single()
    {
        var deleted = new Documentation("deleted");
        var originalPartition = new Geometry("a") { Documentation = deleted };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.Documentation = null;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_not_matching_node_id()
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
    public void Multiple_required_containment()
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
    public void Single_required_containment()
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
    public void Single_uses_detach_from_parent()
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
    public void Single_not_matching_node_id()
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
}