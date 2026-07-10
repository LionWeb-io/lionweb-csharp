// Copyright 2026 TRUMPF Laser SE and other contributors
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
public class MovedFromOutsideTests : ReplicatorTestsBase
{
    [TestMethod]
    public void InsertBeforeMultiple()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [existingChild] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        targetParent.AddContainment_0_n([childA, childB]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA, childB]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        existingChild.InsertBefore(childA);
        existingChild.InsertBefore(childB);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void InsertAfterMultiple()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [existingChild] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        targetParent.AddContainment_0_n([childA, childB]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA, childB]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        existingChild.InsertAfter(childA);
        existingChild.InsertAfter(childB);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void InsertMultiple()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [existingChild] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        targetParent.AddContainment_0_n([childA, childB]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA, childB]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.InsertContainment_1_n(0, [childA, childB]);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void AddMultiple()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [existingChild] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        targetParent.AddContainment_0_n([childA, childB]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA, childB]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.AddContainment_1_n([childA, childB]);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReplaceMultiple()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [existingChild] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        targetParent.AddContainment_0_n([childA, childB]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA, childB]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        existingChild.ReplaceWith(childA);

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildReplacedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReplaceSingle()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_0_1 = existingChild };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        targetParent.AddContainment_0_n([childA]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        existingChild.ReplaceWith(childA);

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildReplacedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void SetSingle_existing()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_0_1 = existingChild };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        targetParent.AddContainment_0_n([childA]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.Containment_0_1 = childA;

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildReplacedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void SetSingle_new()
    {
        var targetParent = new LinkTestConcept("targetParent");
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        targetParent.AddContainment_0_n([childA]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.Containment_0_1 = childA;

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void SetMultiple_existing()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [existingChild] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        targetParent.AddContainment_0_n([childA, childB]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA, childB]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<IReadableNode> { childA, childB });

        Assert.AreEqual(3, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildDeletedNotification>(notificationObserver.Notifications[0]);
        Assert.HasCount(2, notificationObserver.Notifications.OfType<ChildAddedNotification>());

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void SetMultiple_new()
    {
        var targetParent = new LinkTestConcept("targetParent");
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        targetParent.AddContainment_0_n([childA, childB]);
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddContainment_0_n([childA, childB]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<IReadableNode> { childA, childB });

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
}