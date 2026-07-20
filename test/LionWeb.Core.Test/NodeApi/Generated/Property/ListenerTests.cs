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
using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Generated.Property;

[TestClass]
public class ListenerTests
{
    [TestMethod]
    public void PropertyAdded_Optional()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("d");
        parent.Documentation = doc;

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        doc.Text = "hello";

        var notifications = observer.AssertOfType<PropertyAddedNotification>(1);
        Assert.AreSame(doc, notifications[0].Node);
        Assert.AreSame(ShapesLanguage.Instance.Documentation_text, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].NewValue);
    }

    [TestMethod]
    public void PropertyDeleted_Optional()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("d");
        parent.Documentation = doc;
        doc.Text = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        doc.Text = null;

        var notifications = observer.AssertOfType<PropertyDeletedNotification>(1);
        Assert.AreSame(doc, notifications[0].Node);
        Assert.AreSame(ShapesLanguage.Instance.Documentation_text, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
    }

    [TestMethod]
    public void PropertyChanged_Optional()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("d");
        parent.Documentation = doc;
        doc.Text = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        doc.Text = "bye";

        var notifications = observer.AssertOfType<PropertyChangedNotification>(1);
        Assert.AreSame(doc, notifications[0].Node);
        Assert.AreSame(ShapesLanguage.Instance.Documentation_text, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
        Assert.AreEqual("bye", notifications[0].NewValue);
        observer.AssertNone<PropertyAddedNotification>();
        observer.AssertNone<PropertyDeletedNotification>();
    }

    [TestMethod]
    public void PropertyAdded_Required()
    {
        var parent = new Geometry("g");
        var circle = new Circle("d");
        parent.AddShapes([circle]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        circle.Uuid = "hello";

        var notifications = observer.AssertOfType<PropertyAddedNotification>(1);
        Assert.AreSame(circle, notifications[0].Node);
        Assert.AreSame(ShapesLanguage.Instance.IShape_uuid, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].NewValue);
    }

    [TestMethod]
    public void PropertyDeleted_Required()
    {
        var parent = new Geometry("g");
        var circle = new Circle("d");
        parent.AddShapes([circle]);
        circle.Uuid = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => circle.Uuid = null);

        observer.AssertEmpty();
    }

    [TestMethod]
    public void PropertyChanged_Required()
    {
        var parent = new Geometry("g");
        var circle = new Circle("d");
        parent.AddShapes([circle]);
        circle.Uuid = "hello";

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        circle.Uuid = "bye";

        var notifications = observer.AssertOfType<PropertyChangedNotification>(1);
        Assert.AreSame(circle, notifications[0].Node);
        Assert.AreSame(ShapesLanguage.Instance.IShape_uuid, notifications[0].Property);
        Assert.AreEqual("hello", notifications[0].OldValue);
        Assert.AreEqual("bye", notifications[0].NewValue);
        observer.AssertNone<PropertyAddedNotification>();
        observer.AssertNone<PropertyDeletedNotification>();
    }
}
