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
public class InsertTests
{
    [TestMethod]
    public void Empty()
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

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
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

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
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

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line, circle });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
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

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
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

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line, circleA, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        parent.InsertShapes(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        var oldParent = new Geometry("g") { Shapes = [line] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.InsertShapes(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        var oldParent = new Geometry("g") { Shapes = [line] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("parent") { DisabledParts = [line], Parts = [circleA, circleB] };
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
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("parent") { DisabledParts = [line], Parts = [circleA, circleB] };
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
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var lineA = new Line("lineA");
        var lineB = new Line("lineB");
        var compositeShape = new CompositeShape("parent") { Parts = [lineA, circleA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        List<IShape> values = [lineA, lineB];

        int notifications = 0;
        int[] oldIndices = [0, 2];
        int[] indices = [2, 3];
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            Assert.AreEqual(oldIndices[notifications], args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indices[notifications], args.NewIndex);
            Assert.AreEqual(values[notifications], args.MovedChild);
            notifications++;
        });

        compositeShape.InsertParts(2, values);

        Assert.AreEqual(2, notifications);
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

        int notifications = 0;
        int[] oldIndices = [0, 2];
        int[] indices = [1, 3];
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            Assert.AreEqual(oldIndices[notifications], args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indices[notifications], args.NewIndex);
            Assert.AreEqual(values[notifications], args.MovedChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, lineA, circleB, lineB });

        Assert.AreEqual(2, notifications);
    }
}