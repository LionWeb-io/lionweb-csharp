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
public class InsertTests
{
    [TestMethod]
    public void Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line, circle });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line, circleA, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        compositeShape.InsertParts(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        compositeShape.InsertParts(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.InsertParts(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.InsertParts(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    [Ignore("expected indices unclear")]
    public void FromSameContainment()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.InsertParts(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameContainment_Reflective()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameContainment_NoOp()
    {
        var lineA = new Line("myA");
        var lineB = new Line("myB");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, lineA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);

        compositeShape.InsertParts(1, [lineA, lineB]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void FromSameContainment_NoOp_Reflective()
    {
        var lineA = new Line("myA");
        var lineB = new Line("myB");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, lineA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts,
            new List<INode> { circleA, lineA, lineB, circleB });

        Assert.AreEqual(0, notifications);
    }
}