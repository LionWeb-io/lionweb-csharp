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
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class MovedAndReplacedInSameContainmentTests : ReplicatorTestsBase
{
    [TestMethod]
    [Ignore("Should emit ChildMovedAndReplacedInSameContainmentNotification")]
    public void Backward()
    {
        var replacement = new LinkTestConcept("replacement");
        var replaced = new LinkTestConcept("replaced");
        
        var originalPartition = new TestPartition("a")
        {
            Links =  [new LinkTestConcept("child"), replaced, replacement]
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
    public void Forward_ProducesNotification()
    {
        var moved = new LinkTestConcept("moved");
        var replaced = new LinkTestConcept("replaced");
        var originalPartition = new TestPartition("a") { Links =  [moved, replaced] };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, moved, originalPartition, TestLanguageLanguage.Instance.TestPartition_links, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.AreEqual(1, clonedPartition.Links.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Links[0].GetId());
    }

    [TestMethod]
    [Ignore("Should emit ChildMovedAndReplacedInSameContainmentNotification")]
    public void Forward()
    {
        var replacement = new LinkTestConcept("replacement");
        var replaced = new LinkTestConcept("replaced");
        
        var originalPartition = new TestPartition("a")
        {
            Links =  [new LinkTestConcept("child"), replacement, replaced]
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
    public void BackwardP_ProducesNotification()
    {
        var moved = new LinkTestConcept("moved");
        var replaced = new LinkTestConcept("replaced");
        var originalPartition = new TestPartition("a") { Links =  [replaced, moved] };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 0;
        var oldIndex = 1;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, moved, originalPartition, TestLanguageLanguage.Instance.TestPartition_links, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.AreEqual(1, clonedPartition.Links.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Links[0].GetId());
    }

    [TestMethod]
    public void Backward_MoreThanThreeChildren_ProducesNotification()
    {
        var replacement = new LinkTestConcept("E");
        var replaced = new LinkTestConcept("B");

        var originalPartition = new TestPartition("container")
        {
            Links =  [new LinkTestConcept("A"), replaced, new LinkTestConcept("C"), new LinkTestConcept("D"), replacement, new LinkTestConcept("F")]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 4;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, replacement, originalPartition, TestLanguageLanguage.Instance.TestPartition_links, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(5, clonedPartition.Links.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Links[1].GetId());

        Assert.IsFalse(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(replacement.GetId()));
    }

    [TestMethod]
    public void Forward_MoreThanThreeChildren_ProducesNotification()
    {
        var replacement = new LinkTestConcept("E");
        var replaced = new LinkTestConcept("B");

        var originalPartition = new TestPartition("container")
        {
            Links =  [new LinkTestConcept("A"), replacement, new LinkTestConcept("C"), new LinkTestConcept("D"), replaced, new LinkTestConcept("F")]
        };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 4;
        var oldIndex = 1;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, replacement, originalPartition, TestLanguageLanguage.Instance.TestPartition_links, 
            replaced, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        replaced.ReplaceWith(replacement);

        Assert.AreEqual(5, clonedPartition.Links.Count);
        Assert.AreEqual(replacement.GetId(), clonedPartition.Links[^2].GetId());
    }

    [TestMethod]
    public void Not_matching_node_ids()
    {
        var moved = new LinkTestConcept("moved");
        var replaced = new LinkTestConcept("replaced");
        var nodeWithAnotherId = new LinkTestConcept("node-with-another-id");
        var originalPartition = new TestPartition("a") { Links =  [moved, replaced, nodeWithAnotherId] };
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(newIndex, moved, originalPartition, TestLanguageLanguage.Instance.TestPartition_links, 
            nodeWithAnotherId, oldIndex, new NumericNotificationId("childMovedAndReplacedInSameContainment", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }
}