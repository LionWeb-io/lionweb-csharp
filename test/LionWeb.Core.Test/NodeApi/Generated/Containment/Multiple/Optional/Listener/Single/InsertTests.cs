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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.Listener.Single;

[TestClass]
public class InsertTests
{
    [TestMethod]
    public void Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(0, [line]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(0, [line]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line, circle });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(1, [line]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(0, [line]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line, circleA, circleB });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(1, [line]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, line, circleB });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(2, [line]);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB, line });

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        var oldParent = new Geometry("g") { Shapes = [line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.InsertShapes(2, [line]);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].NewParent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].NewContainment);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        var oldParent = new Geometry("g") { Shapes = [line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB, line });

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].NewParent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].NewContainment);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("parent") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.InsertParts(1, [line]);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].NewContainment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("parent") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, line, circleB });

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].NewContainment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameContainment()
    {
        var circleA = new Circle("circleA");
        var circleB = new Circle("circleB");
        var lineA = new Line("lineA");
        var lineB = new Line("lineB");
        var compositeShape = new CompositeShape("parent") { Parts = [lineA, circleA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        List<IShape> values = [lineA, lineB];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.InsertParts(2, values);

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(2);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(lineA, notifications[0].MovedChild);
        Assert.AreEqual(1, notifications[1].OldIndex);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(3, notifications[1].NewIndex);
        Assert.AreEqual(lineB, notifications[1].MovedChild);
    }

    [TestMethod]
    public void FromSameContainment_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var lineA = new Line("lineA");
        var lineB = new Line("lineB");
        var compositeShape = new CompositeShape("parent") { Parts = [lineA, circleA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        List<IShape> values = [lineA, lineB];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, lineA, circleB, lineB });

        var notifications = observer.AssertOfType<ChildMovedInSameContainmentNotification>(2);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(lineA, notifications[0].MovedChild);
        Assert.AreEqual(2, notifications[1].OldIndex);
        Assert.AreSame(compositeShape, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[1].Containment);
        Assert.AreEqual(3, notifications[1].NewIndex);
        Assert.AreEqual(lineB, notifications[1].MovedChild);
    }
}
