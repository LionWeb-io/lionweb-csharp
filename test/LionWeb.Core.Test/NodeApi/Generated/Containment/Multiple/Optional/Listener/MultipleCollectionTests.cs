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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.Listener;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.AddShapes(values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(0, values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Insert_Empty_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(1, values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[1].Containment);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, valueA, valueB, circleB });

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(parent, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[1].Containment);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    #endregion
}
