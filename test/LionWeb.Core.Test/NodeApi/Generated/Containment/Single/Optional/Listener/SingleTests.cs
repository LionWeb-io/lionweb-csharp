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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Optional.Listener;

[TestClass]
public class SingleTests: NotificationTestsBase
{
    [TestMethod]
    public void Single()
    {
        var parent = new TestPartition("g");
        var child = new DataTypeTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Data = child;

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_data, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new TestPartition("g");
        var child = new DataTypeTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.SetData(child);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_data, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new TestPartition("g");
        var child = new DataTypeTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, child);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_data, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var parent = new TestPartition("g");

        var child = new DataTypeTestConcept("myId");
        var oldParent = new LinkTestConcept("oldParent") { Containment_0_1 = new LinkTestConcept("dummy") };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Data = child;

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var parent = new TestPartition("g");

        var child = new DataTypeTestConcept("myId");
        var oldParent = new LinkTestConcept("oldParent") { Containment_0_1 = new LinkTestConcept("dummy") };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, child);

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromOtherParent_Replace()
    {
        var replacedChild = new DataTypeTestConcept("replacedChild");
        var parent = new TestPartition("g") { Data = replacedChild };

        var child = new DataTypeTestConcept("myId");
        var oldParent = new LinkTestConcept("oldParent") { Containment_0_1 = new LinkTestConcept("dummy") };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Data = child;

        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromOtherParent_Replace_Reflective()
    {
        var replacedChild = new DataTypeTestConcept("replacedChild");
        var parent = new TestPartition("g") { Data = replacedChild };

        var child = new DataTypeTestConcept("myId");
        var oldParent = new LinkTestConcept("oldParent") { Containment_0_1 = new LinkTestConcept("dummy") };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, child);

        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var child = new LinkTestConcept("myId");
        var node = new LinkTestConcept("g") { Containment_0_n = [child] };
        var partition = new TestPartition("g") { Links = [node] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.Containment_0_1 = child;

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var node = new LinkTestConcept("g") { Containment_0_n = [child] };
        var partition = new TestPartition("g") { Links = [node] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, child);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent_Replace()
    {
        var replacedChild = new LinkTestConcept("replacedChild");
        var child = new LinkTestConcept("myId");
        var node = new LinkTestConcept("g") { Containment_0_n = [child], Containment_0_1 = replacedChild };
        var partition = new TestPartition("g") { Links = [node] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.Containment_0_1 = child;

        var notifications = observer.AssertOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
        Assert.AreEqual(replacedChild, notifications[0].ReplacedChild);
    }

    [TestMethod]
    public void FromSameParent_Replace_Reflective()
    {
        var replacedChild = new LinkTestConcept("replacedChild");
        var child = new LinkTestConcept("myId");
        var node = new LinkTestConcept("g") { Containment_0_n = [child], Containment_0_1 = replacedChild };
        var partition = new TestPartition("g") { Links = [node] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        node.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, child);

        var notifications = observer.AssertOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(node, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(child, notifications[0].MovedChild);
        Assert.AreEqual(replacedChild, notifications[0].ReplacedChild);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldChild = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldChild };
        var child = new DataTypeTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Data = child;

        var notifications = observer.AssertOfType<ChildReplacedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_data, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
        Assert.AreEqual(oldChild, notifications[0].ReplacedChild);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldChild = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldChild };
        var child = new DataTypeTestConcept("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, child);

        var notifications = observer.AssertOfType<ChildReplacedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_data, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(child, notifications[0].NewChild);
        Assert.AreEqual(oldChild, notifications[0].ReplacedChild);
    }

    [TestMethod]
    public void Existing_Same()
    {
        var oldChild = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldChild };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Data = oldChild;

        observer.AssertEmpty();
    }

    [TestMethod]
    public void Existing_Same_Reflective()
    {
        var oldChild = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldChild };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, oldChild);

        observer.AssertEmpty();
    }

    #endregion
}
