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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;
using Notification.Partition;

[TestClass]
public class ContainmentTests_Multiple_Required_Listener
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.AddParts([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromOtherParent()
    {
        var compositeShape = new CompositeShape("cs");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.AddParts([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromOtherParent_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameParent()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.AddParts([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameParent_Reflective()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameContainment()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.AddShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameContainment_Reflective()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameContainment_NoOp()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [circle, line] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => events++);

        parent.AddShapes([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Add_FromSameContainment_NoOp_Reflective()
    {
        var line = new Line("myId");
        var circle = new Circle("circle");
        var parent = new Geometry("g") { Shapes = [circle, line] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => events++);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        Assert.AreEqual(0, events);
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line, circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After_Reflective()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { line, circleA, circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        compositeShape.InsertParts(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        compositeShape.InsertParts(2, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromOtherParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.InsertParts(2, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromOtherParent_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [compositeShape, line] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameParent()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.InsertParts(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameParent_Reflective()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_disabledParts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.NewContainment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    [Ignore("expected indices unclear")]
    public void Single_Insert_FromSameContainment()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.InsertParts(2, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameContainment_Reflective()
    {
        var line = new Line("myId");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, args) =>
        {
            events++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameContainment_NoOp()
    {
        var lineA = new Line("myA");
        var lineB = new Line("myB");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, lineA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => events++);

        compositeShape.InsertParts(1, [lineA, lineB]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameContainment_NoOp_Reflective()
    {
        var lineA = new Line("myA");
        var lineB = new Line("myB");
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, lineA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => events++);

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts,
            new List<INode> { circleA, lineA, lineB, circleB });

        Assert.AreEqual(0, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts([line]));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Empty_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() =>
            compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { }));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts([line]));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line, circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line, circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        Assert.AreEqual(1, events);
    }

    #endregion

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.AddParts(values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, _) => events++);

        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(
            () => compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.InsertParts(0, values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildReplacedNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildReplacedNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts(values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var value = new Circle("myId");
        compositeShape.AddParts([value]);

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildReplacedNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => events++);
        parent.GetPublisher().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => events++);

        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(
            () => compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values));

        Assert.AreEqual(0, events);
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        compositeShape.AddParts(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values);

        Assert.AreEqual(2, events);
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        compositeShape.InsertParts(0, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1 + events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        compositeShape.InsertParts(1, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1 + events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts,
            new List<INode> { circleA, valueA, valueB, circleB });

        Assert.AreEqual(2, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts(values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts(values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, _) => events++);

        compositeShape.RemoveParts(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            events++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained_Reflective()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last_Reflective()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        int[] indexes = { 0, 1 };
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indexes[events], args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        int[] indexes = { 0, 1 };
        parent.GetPublisher().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indexes[events], args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, events);
    }

    #endregion

    #endregion
}