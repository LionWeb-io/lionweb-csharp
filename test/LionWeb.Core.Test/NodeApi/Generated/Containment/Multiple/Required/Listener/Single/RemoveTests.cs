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
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var parentNode = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [parentNode] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => parentNode.RemoveContainment_1_n([child]));

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Empty_Reflective()
    {
        var parentNode = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [parentNode] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() =>
            parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { }));

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void NotContained()
    {
        var childA = new LinkTestConcept("myC");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n([child]);

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Only()
    {
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [child] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => parentNode.RemoveContainment_1_n([child]));

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void First()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [child, childA] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n([child]);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [child, childA] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childA });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Last()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, child] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n([child]);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, child] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childA });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Between()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, child, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n([child]);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var child = new LinkTestConcept("myId");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, child, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childA, childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }
}
