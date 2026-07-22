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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var parentNode = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.AddContainment_1_n(values);

        var notifications = observer.AssertOfType<ChildAddedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var parentNode = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, values);

        var notifications = observer.AssertOfType<ChildAddedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parentNode = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.InsertContainment_1_n(0, values);

        var notifications = observer.AssertOfType<ChildAddedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.InsertContainment_1_n(1, values);

        var notifications = observer.AssertOfType<ChildAddedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Insert_Two_Between_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n,
            new List<INode> { childA, valueA, valueB, childB });

        var notifications = observer.AssertOfType<ChildAddedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parentNode = new LinkTestConcept("cs");
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => parentNode.RemoveContainment_1_n(values));

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [valueA, valueB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => parentNode.RemoveContainment_1_n(values));

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var childA = new LinkTestConcept("cA");
        var childB = new LinkTestConcept("cB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n(values);

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var childA = new LinkTestConcept("cA");
        var childB = new LinkTestConcept("cB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var values = new LinkTestConcept[] { valueA, childA };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(childA, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Remove_HalfContained_Reflective()
    {
        var childA = new LinkTestConcept("cA");
        var childB = new LinkTestConcept("cB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var valueA = new LinkTestConcept("sA");
        var values = new LinkTestConcept[] { valueA, childA };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(childA, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Remove_Last()
    {
        var child = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [child, valueA, valueB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Last_Reflective()
    {
        var child = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [child, valueA, valueB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { child });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Between()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, valueA, valueB, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Between_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [childA, valueA, valueB, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childA, childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [valueA, childA, valueB, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.RemoveContainment_1_n(values);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Mixed_Reflective()
    {
        var childA = new LinkTestConcept("cIdA");
        var childB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parentNode = new LinkTestConcept("cs") { Containment_1_n = [valueA, childA, valueB, childB] };
        var partition = new TestPartition("g") { Links = [parentNode] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        parentNode.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { childA, childB });

        var notifications = observer.AssertOfType<ChildDeletedNotification>(2);
        Assert.AreSame(parentNode, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(parentNode, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    #endregion
}
