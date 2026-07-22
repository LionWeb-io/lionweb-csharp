// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

using LionWeb.Core.Notification.Partition;
using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.Listener.Single;

[TestClass]
public class SingleTests: NotificationTestsBase
{
    [TestMethod]
    public void Add()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.AddLinks([child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Add_Reflective()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { child });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Add_FromOtherParent()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("myId");
        var oldParent = new LinkTestConcept("oldParent") { Containment_1_n = [child] };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.AddLinks([child]);

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void Add_FromOtherParent_Reflective()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("myId");
        var oldParent = new LinkTestConcept("oldParent") { Containment_1_n = [child] };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { child });

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void Add_FromSameParent()
    {
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("oldParent") { Containment_0_n = [child] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.AddContainment_1_n([child]);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameParent_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("oldParent") { Containment_0_n = [child] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { child });

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment()
    {
        var child = new LinkTestConcept("myId");
        var childB = new LinkTestConcept("circle");
        var parent = new TestPartition("g") { Links = [child, childB] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.AddLinks([child]);

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var childB = new LinkTestConcept("circle");
        var parent = new TestPartition("g") { Links = [child, childB] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childB, child });

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }
}
