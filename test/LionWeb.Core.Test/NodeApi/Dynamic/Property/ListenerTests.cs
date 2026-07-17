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
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Dynamic.Property;

[TestClass]
public class ListenerTests : DynamicNodeTestsBase
{
    [TestMethod]
    public void PropertyAdded()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("d");
        parent.Set(Geometry_documentation, doc);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        doc.Set(Documentation_text, "hello");

        var notifications = observer.OfType<PropertyAddedNotification>(1);
        Assert.AreSame(doc, notifications[0].Node);
        Assert.AreSame(Documentation_text, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].NewValue);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("d");
        parent.Set(Geometry_documentation, doc);
        doc.Set(Documentation_text, "hello");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        doc.Set(Documentation_text, null);

        var notifications = observer.OfType<PropertyDeletedNotification>(1);
        Assert.AreSame(doc, notifications[0].Node);
        Assert.AreSame(Documentation_text, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("d");
        parent.Set(Geometry_documentation, doc);
        doc.Set(Documentation_text, "hello");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        doc.Set(Documentation_text, "bye");

        var notifications = observer.OfType<PropertyChangedNotification>(1);
        Assert.AreSame(doc, notifications[0].Node);
        Assert.AreSame(Documentation_text, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
        Assert.AreEqual("bye", notifications[0].NewValue);
        observer.AssertNone<PropertyAddedNotification>();
        observer.AssertNone<PropertyDeletedNotification>();
    }
}
