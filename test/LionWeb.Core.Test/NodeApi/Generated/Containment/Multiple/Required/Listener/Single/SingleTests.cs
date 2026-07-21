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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener.Single;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var node = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [node] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.AddContainment_1_n([child]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Add_Reflective()
    {
        var node = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [node] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { child });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Add_FromOtherParent()
    {
        var node = new LinkTestConcept("cs");
        var child = new LinkTestConcept("myId");
        var partition = new TestPartition("g") { Links = [node, child] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.AddContainment_1_n([child]);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(partition, notifications[0].OldParent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].OldContainment);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].NewParent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromOtherParent_Reflective()
    {
        var node = new LinkTestConcept("cs");
        var child = new LinkTestConcept("myId");
        var partition = new TestPartition("g") { Links = [node, child] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { child });

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(partition, notifications[0].OldParent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].OldContainment);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].NewParent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameParent()
    {
        var child = new LinkTestConcept("myId");
        var node = new LinkTestConcept("cs") { Containment_0_n = [child] };
        var partition = new TestPartition("g") { Links = [node] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.AddContainment_1_n([child]);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameParent_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var node = new LinkTestConcept("cs") { Containment_0_n = [child] };
        var partition = new TestPartition("g") { Links = [node] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { child });

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment()
    {
        var child = new LinkTestConcept("myId");
        var otherChild = new LinkTestConcept("otherChild");
        var partition = new TestPartition("g") { Links = [child, otherChild] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        partition.AddLinks([child]);

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(partition, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var otherChild = new LinkTestConcept("otherChild");
        var partition = new TestPartition("g") { Links = [child, otherChild] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        partition.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { otherChild, child });

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(partition, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment_NoOp()
    {
        var child = new LinkTestConcept("myId");
        var otherChild = new LinkTestConcept("otherChild");
        var partition = new TestPartition("g") { Links = [otherChild, child] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        partition.AddLinks([child]);

        observer.AssertNone<ChildMovedInSameContainmentNotification>();
    }

    [TestMethod]
    public void Add_FromSameContainment_NoOp_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var otherChild = new LinkTestConcept("otherChild");
        var partition = new TestPartition("g") { Links = [otherChild, child] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        partition.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { otherChild, child });

        observer.AssertNone<ChildMovedInSameContainmentNotification>();
    }
}
