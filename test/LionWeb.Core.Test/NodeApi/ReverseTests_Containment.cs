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
public class ReverseTests_Containment
{
    private Containment _containment_0_1 = TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1;
    private Containment _containment_0_n = TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n;

    [TestMethod]
    public void ReverseEmpty()
    {
        var parent = new LinkTestConcept("parent");
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);
        
        parent.ReverseInPlace(_containment_0_n);
        
        Assert.IsEmpty(parent.Containment_0_n);
        Assert.IsEmpty(observer.Notifications);
    }
    
    [TestMethod]
    public void Reverse1()
    {
        var child = new LinkTestConcept("child");
        var parent = new LinkTestConcept("parent") {Containment_0_n = [child]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_containment_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{child}, parent.Containment_0_n.ToList());
        Assert.IsEmpty(observer.Notifications);
    }
    
    [TestMethod]
    public void Reverse2()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var parent = new LinkTestConcept("parent") {Containment_0_n = [childA, childB]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_containment_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{childB, childA}, parent.Containment_0_n.ToList());
        Assert.HasCount(1, observer.Notifications);

        int notificationIndex = 0;
        AssertChildMoved(parent, childB, 1, 0, observer.Notifications[notificationIndex++]);
    }
    
    [TestMethod]
    public void Reverse3()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");
        var parent = new LinkTestConcept("parent") {Containment_0_n = [childA, childB, childC]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_containment_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{childC, childB, childA}, parent.Containment_0_n.ToList());
        Assert.HasCount(2, observer.Notifications);
        
        int notificationIndex = 0;
        AssertChildMoved(parent, childC, 2, 0, observer.Notifications[notificationIndex++]);
        AssertChildMoved(parent, childA, 1, 2, observer.Notifications[notificationIndex++]);
    }
    
    [TestMethod]
    public void Reverse4()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");
        var childD = new LinkTestConcept("childD");
        var parent = new LinkTestConcept("parent") {Containment_0_n = [childA, childB, childC, childD]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_containment_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{childD, childC, childB, childA}, parent.Containment_0_n.ToList());
        Assert.HasCount(3, observer.Notifications);
        
        int notificationIndex = 0;
        AssertChildMoved(parent, childD, 3, 0, observer.Notifications[notificationIndex++]);
        AssertChildMoved(parent, childA, 1, 3, observer.Notifications[notificationIndex++]);
        AssertChildMoved(parent, childC, 2, 1, observer.Notifications[notificationIndex++]);
    }
    
    [TestMethod]
    public void Reverse5()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");
        var childD = new LinkTestConcept("childD");
        var childE = new LinkTestConcept("childE");
        var parent = new LinkTestConcept("parent") {Containment_0_n = [childA, childB, childC, childD, childE]};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parent.ReverseInPlace(_containment_0_n);
        
        CollectionAssert.AreEqual(new List<LinkTestConcept>{childE, childD, childC, childB, childA}, parent.Containment_0_n.ToList());
        Assert.HasCount(4, observer.Notifications);

        int notificationIndex = 0;
        AssertChildMoved(parent, childE, 4, 0, observer.Notifications[notificationIndex++]);
        AssertChildMoved(parent, childA, 1, 4, observer.Notifications[notificationIndex++]);
        AssertChildMoved(parent, childD, 3, 1, observer.Notifications[notificationIndex++]);
        AssertChildMoved(parent, childB, 2, 3, observer.Notifications[notificationIndex++]);
    }

    [TestMethod]
    public void ReverseSingle_Empty()
    {
        var parent = new LinkTestConcept("parent");
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidFeatureException>(() => parent.ReverseInPlace(_containment_0_1));
        
        Assert.IsNull(parent.Containment_0_1);
        Assert.IsEmpty(observer.Notifications);
    }

    [TestMethod]
    public void ReverseSingle_Set()
    {
        var child = new LinkTestConcept("child");
        var parent = new LinkTestConcept("parent") {Containment_0_1 = child};
        var partition = new TestPartition("partition") {Links = [parent]};
        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidFeatureException>(() => parent.ReverseInPlace(_containment_0_1));
        
        Assert.AreSame(child, parent.Containment_0_1);
        Assert.IsEmpty(observer.Notifications);
    }

    private void AssertChildMoved(LinkTestConcept parent, LinkTestConcept child, int oldIndex, int newIndex, INotification notification)
    {
        Assert.IsInstanceOfType<ChildMovedInSameContainmentNotification>(notification);
        var notificationE = (ChildMovedInSameContainmentNotification)notification;
        Assert.AreSame(parent, notificationE.Parent);
        Assert.AreSame(child, notificationE.MovedChild);
        Assert.AreSame(_containment_0_n, notificationE.Containment);
        Assert.AreEqual(oldIndex, notificationE.OldIndex);
        Assert.AreEqual(newIndex, notificationE.NewIndex);
    }
}