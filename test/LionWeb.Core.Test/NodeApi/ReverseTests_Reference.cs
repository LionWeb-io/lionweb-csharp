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

namespace LionWeb.Core.Test.NodeApi;

using Core.Notification;
using Core.Notification.Partition;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;
using Notification;

[TestClass]
public class ReverseTests_Reference
{
    private Reference _reference_0_1 = TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1;
    private Reference _reference_0_n = TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n;

    [TestMethod]
    public void ReverseEmpty()
    {
        var parent = new LinkTestConcept("parent");
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);
        
        parent.ReverseInPlace(_reference_0_n);
        
        Assert.IsEmpty(parent.Reference_0_n);
        Assert.IsEmpty(observer.Notifications);
    }
    
    [TestMethod]
    public void Reverse1()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") {Reference_0_n = [target]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_reference_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{target}, parent.Reference_0_n.ToList());
        Assert.IsEmpty(observer.Notifications);
    }
    
    [TestMethod]
    public void Reverse2()
    {
        var targetA = new LinkTestConcept("targetA");
        var targetB = new LinkTestConcept("targetB");
        var parent = new LinkTestConcept("parent") {Reference_0_n = [targetA, targetB]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_reference_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{targetB, targetA}, parent.Reference_0_n.ToList());
        Assert.HasCount(2, observer.Notifications);
        
        int notificationIndex = 0;
        AssertReferenceDeleted(parent, targetB, 1, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetB, 0, observer.Notifications[notificationIndex++]);
    }
    
    [TestMethod]
    public void Reverse3()
    {
        var targetA = new LinkTestConcept("targetA");
        var targetB = new LinkTestConcept("targetB");
        var targetC = new LinkTestConcept("targetC");
        var parent = new LinkTestConcept("parent") {Reference_0_n = [targetA, targetB, targetC]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_reference_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{targetC, targetB, targetA}, parent.Reference_0_n.ToList());
        Assert.HasCount(4, observer.Notifications);
        
        int notificationIndex = 0;
        AssertReferenceDeleted(parent, targetC, 2, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetC, 0, observer.Notifications[notificationIndex++]);
        
        AssertReferenceDeleted(parent, targetA, 1, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetA, 2, observer.Notifications[notificationIndex++]);
    }
    
    [TestMethod]
    public void Reverse4()
    {
        var targetA = new LinkTestConcept("targetA");
        var targetB = new LinkTestConcept("targetB");
        var targetC = new LinkTestConcept("targetC");
        var targetD = new LinkTestConcept("targetD");
        var parent = new LinkTestConcept("parent") {Reference_0_n = [targetA, targetB, targetC, targetD]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_reference_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{targetD, targetC, targetB, targetA}, parent.Reference_0_n.ToList());
        Assert.HasCount(6, observer.Notifications);
        
        int notificationIndex = 0;
        AssertReferenceDeleted(parent, targetD, 3, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetD, 0, observer.Notifications[notificationIndex++]);

        AssertReferenceDeleted(parent, targetA, 1, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetA, 3, observer.Notifications[notificationIndex++]);

        AssertReferenceDeleted(parent, targetC, 2, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetC, 1, observer.Notifications[notificationIndex++]);
    }
    
    [TestMethod]
    public void Reverse5()
    {
        var targetA = new LinkTestConcept("targetA");
        var targetB = new LinkTestConcept("targetB");
        var targetC = new LinkTestConcept("targetC");
        var targetD = new LinkTestConcept("targetD");
        var targetE = new LinkTestConcept("targetE");
        var parent = new LinkTestConcept("parent") {Reference_0_n = [targetA, targetB, targetC, targetD, targetE]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_reference_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{targetE, targetD, targetC, targetB, targetA}, parent.Reference_0_n.ToList());
        Assert.HasCount(8, observer.Notifications);

        int notificationIndex = 0;
        AssertReferenceDeleted(parent, targetE, 4, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetE, 0, observer.Notifications[notificationIndex++]);

        AssertReferenceDeleted(parent, targetA, 1, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetA, 4, observer.Notifications[notificationIndex++]);

        AssertReferenceDeleted(parent, targetD, 3, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetD, 1, observer.Notifications[notificationIndex++]);

        AssertReferenceDeleted(parent, targetB, 2, observer.Notifications[notificationIndex++]);
        AssertReferenceAdded(parent, targetB, 3, observer.Notifications[notificationIndex++]);
    }

    [TestMethod]
    public void ReverseSingle_Empty()
    {
        var parent = new LinkTestConcept("parent");
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidFeatureException>(() => parent.ReverseInPlace(_reference_0_1));
        
        Assert.IsNull(parent.Reference_0_1);
        Assert.IsEmpty(observer.Notifications);
    }

    [TestMethod]
    public void ReverseSingle_Set()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") {Reference_0_1 = target};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidFeatureException>(() => parent.ReverseInPlace(_reference_0_1));
        
        Assert.AreSame(target, parent.Reference_0_1);
        Assert.IsEmpty(observer.Notifications);
    }

    private void AssertReferenceDeleted(LinkTestConcept parent, LinkTestConcept target, int index, INotification notification)
    {
        Assert.IsInstanceOfType<ReferenceDeletedNotification>(notification);
        var notificationEDeleted = (ReferenceDeletedNotification)notification;
        Assert.AreSame(parent, notificationEDeleted.Parent);
        Assert.AreSame(target, notificationEDeleted.DeletedTarget.Target);
        Assert.AreSame(_reference_0_n, notificationEDeleted.Reference);
        Assert.AreEqual(index, notificationEDeleted.Index);
    }

    private void AssertReferenceAdded(LinkTestConcept parent, LinkTestConcept target, int index, INotification notification)
    {
        Assert.IsInstanceOfType<ReferenceAddedNotification>(notification);
        var notificationEAdded = (ReferenceAddedNotification)notification;
        Assert.AreSame(parent, notificationEAdded.Parent);
        Assert.AreSame(target, notificationEAdded.NewTarget.Target);
        Assert.AreSame(_reference_0_n, notificationEAdded.Reference);
        Assert.AreEqual(index, notificationEAdded.Index);
    }
}