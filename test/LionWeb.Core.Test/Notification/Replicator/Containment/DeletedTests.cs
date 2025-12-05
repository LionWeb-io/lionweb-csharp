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

[TestClass]
public class DeletedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var deleted = new LinkTestConcept("deleted");
        var originalPartition = new TestPartition("a") { Links = [deleted] };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        Assert.IsTrue(sharedNodeMap.ContainsKey(deleted.GetId()));

        originalPartition.RemoveLinks([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);

        Assert.IsFalse(sharedNodeMap.ContainsKey(deleted.GetId()));
    }

    [TestMethod]
    public void Multiple_First()
    {
        var deleted = new LinkTestConcept("deleted");
        var originalPartition = new TestPartition("a") { Links = [deleted, new LinkTestConcept("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveLinks([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var deleted = new LinkTestConcept("deleted");
        var originalPartition = new TestPartition("a") { Links = [new LinkTestConcept("l"), deleted] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveLinks([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single()
    {
        var deleted = new LinkTestConcept("deleted");
        var parent = new LinkTestConcept("parent") { Containment_0_1 = deleted };
        var originalPartition = new TestPartition("a") { Links = [parent]};
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        parent.Containment_0_1 = null;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_not_matching_node_id()
    {
        var deleted = new LinkTestConcept("deleted");
        var nodeWithAnotherId = new LinkTestConcept("node-with-another-id");
        var originalPartition = new TestPartition("a") { Links = [deleted, nodeWithAnotherId, new LinkTestConcept("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildDeletedNotification(nodeWithAnotherId, originalPartition,
            TestLanguageLanguage.Instance.TestPartition_contents, 0, new NumericNotificationId("childDeletedNotificationMultiple", 0));

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
        var deleted = new LinkTestConcept("deleted");
        var origin = new LinkTestConcept("origin") { Containment_1_n = [deleted] };
        var originalPartition = new TestPartition("a") { Links = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildDeletedNotification(deleted, origin, TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0,
            new NumericNotificationId("childDeletedNotification", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.ThrowsExactly<UnsetFeatureException>(() => clonedPartition.Links[0].Containment_1_n);
    }

    /// <summary>
    /// This test confirms that remote replicator is able to detach child from its parent
    /// which is a required single containment.
    /// </summary>
    [TestMethod]
    public void Single_required_containment()
    {
        var deleted = new LinkTestConcept("deleted");
        var origin = new LinkTestConcept("c") { Containment_1 = deleted };

        var originalPartition = new TestPartition("a") { Links = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildDeletedNotification(deleted, origin, TestLanguageLanguage.Instance.LinkTestConcept_containment_1, 0,
            new NumericNotificationId("childDeletedNotification", 0));

        CreatePartitionReplicator(clonedPartition, notification);

        Assert.ThrowsExactly<UnsetFeatureException>(() => clonedPartition.Links[0].Containment_1);
    }

    /// <summary>
    /// This test confirms that no notification is generated from DetachFromParent method
    /// TODO: This is a known bug, we want to have a notification emitted.
    /// </summary>
    [TestMethod]
    public void Single_uses_detach_from_parent()
    {
        var deleted = new LinkTestConcept("deleted");
        var origin = new LinkTestConcept("c") { Containment_1 = deleted };

        var originalPartition = new TestPartition("a") { Links = [origin] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        deleted.DetachFromParent();

        Assert.AreEqual(0, notificationObserver.Notifications.Count);
    }

    [TestMethod]
    public void Single_not_matching_node_id()
    {
        var deleted = new LinkTestConcept("deleted");
        var nodeWithAnotherId = new LinkTestConcept("node-with-another-id");
        var originalParent = new LinkTestConcept("parent")
        {
            Containment_0_1 = deleted
        };
        var originalPartition = new TestPartition("a") { Links = [originalParent] };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(nodeWithAnotherId);

        var notification = new ChildDeletedNotification(nodeWithAnotherId, originalParent,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, 0, new NumericNotificationId("childDeletedNotificationSingle", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        });
    }
}