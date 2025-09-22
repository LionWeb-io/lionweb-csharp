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

using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class NotificationTests_Forest : NotificationTestsBase
{
    #region Partition

    [TestMethod]
    public void PartitionAdded()
    {
        var node = new Geometry("a");
        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([node]);

        AssertEquals([node], clonedForest.Partitions);
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var node = new Geometry("a");
        var originalForest = new Forest();
        originalForest.AddPartitions([node]);

        var clonedForest = new Forest();
        clonedForest.AddPartitions([node]);

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.RemovePartitions([node]);

        Assert.IsEmpty(clonedForest.Partitions);
        Assert.IsEmpty(originalForest.Partitions);
    }
    
    [TestMethod]
    public void PartitionAddedDeleted_AfterSubscribe()
    {
        var node = new Geometry("a");
        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([node]);
        originalForest.RemovePartitions([node]);

        Assert.IsEmpty(clonedForest.Partitions);
        Assert.IsEmpty(originalForest.Partitions);
    }

    #endregion

    #region Children

    #region ChildMovedFromOtherContainment

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_Works()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([node]);
        node.AddShapes([moved]);

        AssertEquals([node], clonedForest.Partitions);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_Destination_Works()
    {
        // Original and cloned forests are out of sync: their initial state differ.
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originPartition = new Geometry("g") { Shapes = [origin] };

        var node = new Geometry("a") { Shapes = [] };

        var originalForest = new Forest();
        originalForest.AddPartitions([originPartition]);

        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([node]);
        Assert.ThrowsExactly<NotImplementedException>(() => node.AddShapes([moved]));
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddBeforeSubscribe_CloneExists_Replicated()
    {
        var moved = new Documentation("moved");
        var node = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var originalForest = new Forest();
        originalForest.AddPartitions([node]);

        var clone = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = new Documentation("moved") }] };

        var clonedForest = new Forest();
        clonedForest.AddPartitions([clone]);

        CreateForestReplicator(clonedForest, originalForest);

        node.Documentation = moved;

        Assert.AreSame(node, moved.GetParent());
        AssertEquals([node], [clone]);
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_DifferentPartitions_Works()
    {
        var moved = new Documentation("moved");
        var originPartition = new Geometry("g") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var node = new Geometry("a") { };

        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        var notificationCounter = new NotificationCounter();

        originalForest.GetNotificationSender()!.ConnectTo(notificationCounter);

        originalForest.AddPartitions([node, originPartition]);

        node.Documentation = moved;

        Assert.AreEqual(
            notificationCounter.Notifications.DistinctBy(n => n.NotificationId).Count(),
            notificationCounter.Count,
            string.Join("\n", notificationCounter.Notifications)
        );

        AssertEquals([node, originPartition], clonedForest.Partitions.OrderBy(p => p.GetId()).ToList());
    }

    #endregion

    #endregion
}