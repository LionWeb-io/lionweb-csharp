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
using M1;

[TestClass]
public class MovedFromOtherContainmentTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple()
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
    public void Single()
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

    [TestMethod]
    public void Partitions_are_in_same_forest()
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
}