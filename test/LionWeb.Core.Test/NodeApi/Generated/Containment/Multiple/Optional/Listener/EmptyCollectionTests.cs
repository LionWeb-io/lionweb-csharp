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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var parent = new Geometry("g");
        var values = Array.Empty<IShape>();

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.AddShapes(values);

        Assert.AreEqual(0, notificationObserver.Count);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = Array.Empty<IShape>();

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        Assert.AreEqual(0, notificationObserver.Count);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = Array.Empty<IShape>();

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.InsertShapes(0, values);

        Assert.AreEqual(0, notificationObserver.Count);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = Array.Empty<IShape>();

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.RemoveShapes(values);

        Assert.AreEqual(0, notificationObserver.Count);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new Geometry("g");
        var circle = new Circle("myId");
        parent.AddShapes([circle]);
        var values = new List<Coord>();

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildDeletedNotification>(notificationObserver.Notifications[0]);
    }
}