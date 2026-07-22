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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.Listener;

[TestClass]
public class MultipleCollectionRemoveTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.AddLinks(values);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.AddLinks(values);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void NonContained()
    {
        var childA = new LinkTestConcept("cA");
        var childB = new LinkTestConcept("cB");
        var parent = new TestPartition("cs") { Links = [childA, childB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks(values);

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void HalfContained()
    {
        var childA = new LinkTestConcept("cA");
        var childB = new LinkTestConcept("cB");
        var parent = new TestPartition("cs") { Links = [childA, childB] };
        var valueA = new LinkTestConcept("sA");
        var values = new LinkTestConcept[] { valueA, childA };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(childA, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void HalfContained_Reflective()
    {
        var childA = new LinkTestConcept("cA");
        var childB = new LinkTestConcept("cB");
        var parent = new TestPartition("cs") { Links = [childA, childB] };
        var valueA = new LinkTestConcept("sA");
        var values = new LinkTestConcept[] { valueA, childA };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(childA, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Only()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Last()
    {
        var child = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [child, valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var child = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [child, valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { child });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Between()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [childA, valueA, valueB, childB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [childA, valueA, valueB, childB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA, childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Mixed()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [valueA, childA, valueB, childB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveLinks(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Mixed_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [valueA, childA, valueB, childB] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<INode> { childA, childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }
}
