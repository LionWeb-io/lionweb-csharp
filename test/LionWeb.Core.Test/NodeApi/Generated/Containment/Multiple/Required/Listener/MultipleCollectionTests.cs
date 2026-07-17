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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.AddParts(values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.InsertParts(0, values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.InsertParts(1, values);

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    [TestMethod]
    public void Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts,
            new List<INode> { circleA, valueA, valueB, circleB });

        var notifications = observer.OfType<ChildAddedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewChild);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => compositeShape.RemoveParts(values));

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => compositeShape.RemoveParts(values));

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.RemoveParts(values);

        observer.AssertNone<ChildDeletedNotification>();
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.RemoveParts(values);

        var notifications = observer.OfType<ChildDeletedNotification>(1);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(circleA, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Remove_HalfContained_Reflective()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleB });

        var notifications = observer.OfType<ChildDeletedNotification>(1);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(circleA, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.RemoveParts(values);

        var notifications = observer.OfType<ChildDeletedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Last_Reflective()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle });

        var notifications = observer.OfType<ChildDeletedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.RemoveParts(values);

        var notifications = observer.OfType<ChildDeletedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        var notifications = observer.OfType<ChildDeletedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.RemoveParts(values);

        var notifications = observer.OfType<ChildDeletedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    [TestMethod]
    public void Remove_Mixed_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        var notifications = observer.OfType<ChildDeletedNotification>(2);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedChild);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedChild);
    }

    #endregion
}
