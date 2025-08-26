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
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class NotificationTests_Forest
{
    #region Children

    #region ChildMovedFromOtherContainment

    [TestMethod]
    [Ignore("Exceeds naive NodeCloneNotificationHandler implementation")]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_Works()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var forest = new Forest();

        var cloneForest = new Forest();

        var replicator = CreateReplicator(cloneForest, forest);

        forest.AddPartitions([node]);

        node.AddShapes([moved]);

        AssertEquals([node], cloneForest.Partitions);
    }

    [TestMethod]
    [Ignore("Exceeds naive NodeCloneNotificationHandler implementation")]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_Destination_Works()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originPartition = new Geometry("g") { Shapes = [origin] };

        var node = new Geometry("a") { Shapes = [] };

        var forest = new Forest();
        forest.AddPartitions([originPartition]);

        var cloneForest = new Forest();

        var replicator = CreateReplicator(cloneForest, forest);

        forest.AddPartitions([node]);

        node.AddShapes([moved]);

        AssertEquals([node], cloneForest.Partitions);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddBeforeSubscribe_CloneExists_NotReplicated()
    {
        var moved = new Documentation("moved");
        var node = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var forest = new Forest();
        forest.AddPartitions([node]);

        var clone = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = new Documentation("moved") }] };

        var cloneForest = new Forest();
        cloneForest.AddPartitions([clone]);

        var replicator = CreateReplicator(cloneForest, forest);

        node.Documentation = moved;

        Assert.AreEqual(node, moved.GetParent());
        Assert.IsNotNull(clone.Shapes.OfType<Shape>().First().ShapeDocs);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_DifferentPartitions_Works()
    {
        var moved = new Documentation("moved");
        var originPartition = new Geometry("g") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var node = new Geometry("a") { };

        var forest = new Forest();

        var cloneForest = new Forest();

        var replicator = CreateReplicator(cloneForest, forest);

        forest.AddPartitions([node, originPartition]);

        node.Documentation = moved;

        AssertEquals([node, originPartition], cloneForest.Partitions.OrderBy(p => p.GetId()).ToList());
    }

    #endregion

    #endregion

    private static INotificationHandler CreateReplicator(Forest cloneForest,
        Forest originalForest)
    {
        var replicator = ForestReplicator.Create(cloneForest);
        var cloneHandler = new NodeCloneNotificationHandler("forestCloner");
        originalForest.GetNotificationHandler().ConnectTo(cloneHandler);
        cloneHandler.ConnectTo(replicator);

        var receiver = new TestLocalForestChangeNotificationHandler(originalForest, cloneHandler);
        originalForest.GetNotificationHandler().ConnectTo(receiver);
        return replicator;
    }

    private void AssertEquals(IEnumerable<IReadableNode?> expected, IEnumerable<IReadableNode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}

internal class TestLocalForestChangeNotificationHandler(object? sender, NodeCloneNotificationHandler cloneHandler)
    : NotificationHandlerBase(sender), IReceivingNotificationHandler
{
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        switch (notification)
        {
            case PartitionAddedNotification partitionAddedNotification:
                OnLocalPartitionAdded(partitionAddedNotification);
                break;
            case PartitionDeletedNotification partitionDeletedNotification:
                OnLocalPartitionDeleted(partitionDeletedNotification);
                break;
        }
    }

    private void OnLocalPartitionAdded(PartitionAddedNotification partitionAddedNotification) => 
        partitionAddedNotification.NewPartition.GetNotificationHandler()?.ConnectTo(cloneHandler);

    private void OnLocalPartitionDeleted(PartitionDeletedNotification partitionDeletedNotification)
    {
        throw new NotImplementedException();
    }
}