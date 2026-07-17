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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener.Single;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.AddParts([line]);

        var notifications = observer.OfType<ChildAddedNotification>(1);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Add_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line });

        var notifications = observer.OfType<ChildAddedNotification>(1);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(line, notifications[0].NewChild);
    }

    [TestMethod]
    public void Add_FromOtherParent()
    {
        var compositeShape = new CompositeShape("cs");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.AddParts([line]);

        var notifications = observer.OfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(parent, notifications[0].OldParent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].OldContainment);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].NewParent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromOtherParent_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line });

        var notifications = observer.OfType<ChildMovedFromOtherContainmentNotification>(1);
        Assert.AreSame(parent, notifications[0].OldParent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].OldContainment);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].NewParent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameParent()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.AddParts([line]);

        var notifications = observer.OfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameParent_Reflective()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line });

        var notifications = observer.OfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(compositeShape, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.AddShapes([line]);

        var notifications = observer.OfType<ChildMovedInSameContainmentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment_Reflective()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        var notifications = observer.OfType<ChildMovedInSameContainmentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, notifications[0].Containment);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(line, notifications[0].MovedChild);
    }

    [TestMethod]
    public void Add_FromSameContainment_NoOp()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [circle, line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.AddShapes([line]);

        observer.AssertNone<ChildMovedInSameContainmentNotification>();
    }

    [TestMethod]
    public void Add_FromSameContainment_NoOp_Reflective()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [circle, line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        observer.AssertNone<ChildMovedInSameContainmentNotification>();
    }
}
