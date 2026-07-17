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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.Listener;

[TestClass]
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.AddShapes(values);

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);

        observer.AssertEmpty();
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(0, values);

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveShapes(values);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var circle = new Circle("myId");
        parent.AddShapes([circle]);
        var values = new List<IShape>();

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);

        var notifications = observer.OfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(circle), notifications[0].DeletedTarget);
    }
}
