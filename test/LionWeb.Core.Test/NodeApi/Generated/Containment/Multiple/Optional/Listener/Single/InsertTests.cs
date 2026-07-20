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
public class InsertTests
{
    [TestMethod]
    public void Empty()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertLinks(0, [child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_Before()
    {
        var childA = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [childA] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertLinks(0, [child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var childA = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [childA] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { child, childA });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_After()
    {
        var childA = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [childA] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertLinks(1, [child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var childA = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [childA] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA, child });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Before()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertLinks(0, [child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { child, childA, childB });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Between()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertLinks(1, [child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA, child, childB });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_After()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertLinks(2, [child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA, childB, child });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");
        var oldParent = new TestPartition("g2") { Links = [child] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertLinks(2, [child]);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].NewParent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].NewContainment);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [childA, childB] };
        var child = new LinkTestConcept("myId");
        var oldParent = new TestPartition("g2") { Links = [child] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA, childB, child });

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].NewParent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].NewContainment);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("parent") { Containment_0_n = [child], Containment_1_n = [childA, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.InsertContainment_1_n(1, [child]);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("parent") { Containment_0_n = [child], Containment_1_n = [childA, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childA, child, childB });

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameContainment()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var valueA = new LinkTestConcept("lineA");
        var valueB = new LinkTestConcept("lineB");
        var parentNode = new LinkTestConcept("parent") { Containment_1_n = [valueA, childA, valueB, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        List<LinkTestConcept> values = [valueA, valueB];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.InsertContainment_1_n(2, values);

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(2);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(valueA, notifications[0].MovedChild);
        Assert.AreEqual(1, notifications[1].OldIndex);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(3, notifications[1].NewIndex);
        Assert.AreEqual(valueB, notifications[1].MovedChild);
    }

    [TestMethod]
    public void FromSameContainment_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("lineA");
        var valueB = new LinkTestConcept("lineB");
        var parentNode = new LinkTestConcept("parent") { Containment_1_n = [valueA, childA, valueB, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        List<LinkTestConcept> values = [valueA, valueB];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childA, valueA, childB, valueB });

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(2);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(valueA, notifications[0].MovedChild);
        Assert.AreEqual(2, notifications[1].OldIndex);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(3, notifications[1].NewIndex);
        Assert.AreEqual(valueB, notifications[1].MovedChild);
    }
}
