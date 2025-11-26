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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.Listener.Single;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.AddShapes([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_Reflective()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromOtherParent()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        var oldParent = new CompositeShape("oldParent") { Parts = [line] };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.AddShapes([line]);

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void Add_FromOtherParent_Reflective()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        var oldParent = new CompositeShape("oldParent") { Parts = [line] };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line });

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void Add_FromSameParent()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("oldParent") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.AddParts([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromSameParent_Reflective()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("oldParent") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromSameContainment()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.AddShapes([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromSameContainment_Reflective()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        Assert.AreEqual(1, notifications);
    }
}