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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener.Single;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts([line]));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Empty_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsException<InvalidValueException>(() =>
            compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { }));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void NotContained()
    {
        var circle = new Circle("myC");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Only()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts([line]));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line, circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line, circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        Assert.AreEqual(1, notifications);
    }
}