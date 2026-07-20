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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.Listener.Single;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveShapes([line]);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Empty_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> {  });

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void NotContained()
    {
        var circle = new Circle("myC");
        var parent = new ReferenceGeometry("cs") { Shapes = [circle] };
        var line = new Line("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveShapes([line]);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Only()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveShapes([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> {  });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line, circle] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveShapes([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line, circle] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circle });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveShapes([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, line] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circle });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, line, circleB] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.RemoveShapes([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, line, circleB] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circleA, circleB });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }
}
