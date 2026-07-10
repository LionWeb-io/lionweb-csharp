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
public class MovedToOutsideTests : ReplicatorTestsBase
{
    [TestMethod]
    public void InsertBeforeMultiple()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [childA, childB] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_n = [floatingChild] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        floatingChild.InsertBefore(childA);
        floatingChild.InsertBefore(childB);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void InsertAfterMultiple()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [childA, childB] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_n = [floatingChild] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        floatingChild.InsertAfter(childA);
        floatingChild.InsertAfter(childB);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void InsertMultiple()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [childA, childB] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_n = [floatingChild] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.InsertContainment_1_n(0, [childA, childB]);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void AddMultiple()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [childA, childB] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_n = [floatingChild] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.AddContainment_1_n([childA, childB]);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReplaceMultiple()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [childA, childB] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_n = [floatingChild] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        floatingChild.ReplaceWith(childA);

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

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

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_1 = floatingChild };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        floatingChild.ReplaceWith(existingChild);

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

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

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_1 = floatingChild };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.Containment_0_1 = existingChild;

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void SetSingle_new()
    {
        var existingChild = new LinkTestConcept("existingChild");
        var targetParent = new LinkTestConcept("targetParent") { Containment_0_1 = existingChild };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var freeFloating = new LinkTestConcept("freeFloating");

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.Containment_0_1 = existingChild;

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void SetMultiple_existing()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [childA, childB] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingChild = new LinkTestConcept("floatingChild");
        var freeFloating = new LinkTestConcept("freeFloating") { Containment_0_n = [floatingChild] };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<IReadableNode>{childA});
        Assert.AreEqual(1, notificationObserver.Count);

        // TODO: Should be as below, but tracking the old position of moved elements is hard
        // freeFloating.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<IReadableNode>{childA, childB});
        // Assert.AreEqual(2, notificationObserver.Count);

        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void SetMultiple_new()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var targetParent = new LinkTestConcept("targetParent") { Containment_1_n = [childA, childB] };
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var freeFloating = new LinkTestConcept("freeFloating");

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<IReadableNode>{childA});
        Assert.AreEqual(1, notificationObserver.Count);
        
        // TODO: Should be as below, but tracking the old position of moved elements is hard
        // freeFloating.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<IReadableNode>{childA, childB});
        // Assert.AreEqual(2, notificationObserver.Count);
        
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(ChildDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
}