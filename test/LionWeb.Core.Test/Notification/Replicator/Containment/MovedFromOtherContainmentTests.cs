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
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class MovedFromOtherContainmentTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple()
    {
        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n =  [moved] };
        var originalPartition = new TestPartition("a") { Contents =  [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.AddContents([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single()
    {
        var moved = new LinkTestConcept("moved");
        var parent = new LinkTestConcept("parent");
        var originalPartition = new TestPartition("a")
        {
            Contents =  [new LinkTestConcept("l") { Containment_1 = moved }, parent]
        };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        parent.Containment_1 = moved;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Partitions_are_in_same_forest()
    {
        var replaced = new LinkTestConcept("replaced")
        {
            Containment_0_1 = new LinkTestConcept("cc") { Name = "2" }
        };
        
        var destinationPartition = new TestPartition("a")
        {
            Contents =  [replaced]
        };
        
        var replacement = new LinkTestConcept("replacement")
        {
            Containment_1 = new LinkTestConcept("sc") { Name = "1" }
        };
        
        var originPartition = new TestPartition("b")
        {
            Contents =  [replacement]
        };
        
        var originalForest = new Forest();
        var clonedForest = new Forest();

        CreateForestReplicator(clonedForest, originalForest);

        originalForest.AddPartitions([originPartition, destinationPartition]);

        var notificationObserver = new NotificationObserver();
        destinationPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        replaced.Containment_0_1 = replacement.Containment_1;

        AssertUniqueNodeIds(originPartition, destinationPartition);
        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(notificationObserver.Notifications[0]);
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }
}