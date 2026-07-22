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

namespace LionWeb.Core.Test.NodeApi.Generated.Property;

[TestClass]
public class ListenerTests
{
    private static readonly M3.Property _iNamedName = LionWebVersions.v2024_1.BuiltIns.INamed_name;
    [TestMethod]
    public void PropertyAdded_Optional()
    {
        var parent = new TestPartition("g");
        var node = new DataTypeTestConcept("d");
        parent.Data = node;

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        node.StringValue_0_1 = "hello";

        var notifications = observer.AssertOfType<PropertyAddedNotification>(1);
        Assert.AreSame(node, notifications[0].Node);
        Assert.AreSame(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].NewValue);
    }

    [TestMethod]
    public void PropertyDeleted_Optional()
    {
        var parent = new TestPartition("g");
        var node = new DataTypeTestConcept("d");
        parent.Data = node;
        node.StringValue_0_1 = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        node.StringValue_0_1 = null;

        var notifications = observer.AssertOfType<PropertyDeletedNotification>(1);
        Assert.AreSame(node, notifications[0].Node);
        Assert.AreSame(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
    }

    [TestMethod]
    public void PropertyChanged_Optional()
    {
        var parent = new TestPartition("g");
        var node = new DataTypeTestConcept("d");
        parent.Data = node;
        node.StringValue_0_1 = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        node.StringValue_0_1 = "bye";

        var notifications = observer.AssertOfType<PropertyChangedNotification>(1);
        Assert.AreSame(node, notifications[0].Node);
        Assert.AreSame(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
        Assert.AreEqual("bye", notifications[0].NewValue);
        observer.AssertNone<PropertyAddedNotification>();
        observer.AssertNone<PropertyDeletedNotification>();
    }

    [TestMethod]
    public void PropertyAdded_Required()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("d");
        parent.AddLinks([child]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Name = "hello";

        var notifications = observer.AssertOfType<PropertyAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Node);
        Assert.AreSame(_iNamedName, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].NewValue);
    }

    [TestMethod]
    public void PropertyDeleted_Required()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("d");
        parent.AddLinks([child]);
        child.Name = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => child.Name = null);

        observer.AssertEmpty();
    }

    [TestMethod]
    public void PropertyChanged_Required()
    {
        var parent = new TestPartition("g");
        var child = new LinkTestConcept("d");
        parent.AddLinks([child]);
        child.Name = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Name = "bye";

        var notifications = observer.AssertOfType<PropertyChangedNotification>(1);
        Assert.AreSame(child, notifications[0].Node);
        Assert.AreSame(_iNamedName, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
        Assert.AreEqual("bye", notifications[0].NewValue);
        observer.AssertNone<PropertyAddedNotification>();
        observer.AssertNone<PropertyDeletedNotification>();
    }
}
