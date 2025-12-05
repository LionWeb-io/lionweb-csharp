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

namespace LionWeb.Core.Test.Notification.Replicator;

using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class ForestTests : ReplicatorTestsBase
{
    #region Partition

    [TestMethod]
    public void PartitionAdded()
    {
        var node = new TestPartition("a");
        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([node]);

        AssertEquals([node], clonedForest.Partitions);
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var node = new TestPartition("a");
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
    public void PartitionAddedAndDeleted_AfterSubscribe()
    {
        var node = new TestPartition("a");
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
        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n =  [moved] };
        var node = new TestPartition("a") { Links =  [origin] };

        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([node]);
        node.AddLinks([moved]);

        AssertEquals([node], clonedForest.Partitions);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_Destination_Fails()
    {
        // Original and cloned forests are out of sync: their initial states differ.
        // One approach to tackle this: cloned forest can check available partitions in original forest, and can clone them.
        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n =  [moved] };
        var originPartition = new TestPartition("g") { Links =  [origin] };

        var destinationPartition = new TestPartition("a") { Links =  [] };

        var originalForest = new Forest();
        originalForest.AddPartitions([originPartition]);

        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([destinationPartition]);
        // Node "moved" is unknown in the cloned forest
        Assert.ThrowsExactly<NotImplementedException>(() => destinationPartition.AddLinks([moved]));
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddBeforeSubscribe_CloneExists_Replicated()
    {
        var moved = new LinkTestConcept("moved");
        var node = new TestPartition("a") { Links =  [new LinkTestConcept("l") { Containment_0_1 = moved }] };

        var originalForest = new Forest();
        originalForest.AddPartitions([node]);

        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("l") { Containment_0_1 = new LinkTestConcept("moved") }] };

        var clonedForest = new Forest();
        clonedForest.AddPartitions([clone]);

        CreateForestReplicator(clonedForest, originalForest);

        node.Links[0].Containment_1 = moved;

        Assert.AreSame(node.Links[0], moved.GetParent());
        AssertEquals([node], [clone]);
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_DifferentPartitions_Works()
    {
        var moved = new LinkTestConcept("moved");
        var originPartition = new TestPartition("g") { Links =  [new LinkTestConcept("l") { Containment_0_1 = moved }] };

        var node = new TestPartition("a") { };

        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        var notificationObserver = new NotificationObserver();

        originalForest.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalForest.AddPartitions([node, originPartition]);

        node.AddLinks([moved]);

        Assert.AreEqual(
            notificationObserver.Notifications.DistinctBy(n => n.NotificationId).Count(),
            notificationObserver.Count,
            string.Join("\n", notificationObserver.Notifications)
        );

        AssertEquals([node, originPartition], clonedForest.Partitions.OrderBy(p => p.GetId()).ToList());
    }

    #endregion

    #endregion
}