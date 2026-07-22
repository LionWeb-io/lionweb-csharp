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
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks([child]);

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void NotContained()
    {
        var childA = new LinkTestConcept("myC");
        var parent = new TestPartition("cs") { Links = [childA] };
        var child = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks([child]);

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Only()
    {
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [child] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks([child]);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [child] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void First()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [child, childA] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks([child]);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [child, childA] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Last()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [childA, child] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks([child]);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var childA = new LinkTestConcept("cId");
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [childA, child] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Between()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [childA, child, childB] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks([child]);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var child = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [childA, child, childB] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA, childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].DeletedChild);
    }
}
