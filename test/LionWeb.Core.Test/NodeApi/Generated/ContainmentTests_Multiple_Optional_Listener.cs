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
using M1.Event.Partition;

[TestClass]
public class ContainmentTests_Multiple_Optional_Listener
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.AddShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_Reflective()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromOtherParent()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        var oldParent = new CompositeShape("oldParent") { Parts = [line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.AddShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromOtherParent_Reflective()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        var oldParent = new CompositeShape("oldParent") { Parts = [line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_FromSameParent()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("oldParent") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentInSameParentEvent>((_, args) =>
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
        var compositeShape = new CompositeShape("oldParent") { DisabledParts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentInSameParentEvent>((_, args) =>
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
        parent.Publisher.Subscribe<ChildMovedInSameContainmentEvent>((_, args) =>
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
        parent.Publisher.Subscribe<ChildMovedInSameContainmentEvent>((_, args) =>
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

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line, circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { line, circleA, circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        parent.InsertShapes(2, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromOtherParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        var oldParent = new Geometry("g") { Shapes = [line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.InsertShapes(2, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromOtherParent_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        var oldParent = new Geometry("g") { Shapes = [line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(parent, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameParent()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("parent") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentInSameParentEvent>((_, args) =>
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
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("parent") { DisabledParts = [line], Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        parent.Publisher.Subscribe<ChildMovedFromOtherContainmentInSameParentEvent>((_, args) =>
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
    public void Single_Insert_FromSameContainment()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var lineA = new Line("lineA");
        var lineB = new Line("lineB");
        var compositeShape = new CompositeShape("parent") { Parts = [lineA, circleA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        List<IShape> values = [lineA, lineB];

        int events = 0;
        int[] oldIndices = [0, 2];
        int[] indices = [2, 3];
        parent.Publisher.Subscribe<ChildMovedInSameContainmentEvent>((_, args) =>
        {
            Assert.AreEqual(oldIndices[events], args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indices[events], args.NewIndex);
            Assert.AreEqual(values[events], args.MovedChild);
            events++;
        });

        compositeShape.InsertParts(2, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Single_Insert_FromSameContainment_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var lineA = new Line("lineA");
        var lineB = new Line("lineB");
        var compositeShape = new CompositeShape("parent") { Parts = [lineA, circleA, lineB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        List<IShape> values = [lineA, lineB];

        int events = 0;
        int[] oldIndices = [0, 2];
        int[] indices = [1, 3];
        parent.Publisher.Subscribe<ChildMovedInSameContainmentEvent>((_, args) =>
        {
            Assert.AreEqual(oldIndices[events], args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indices[events], args.NewIndex);
            Assert.AreEqual(values[events], args.MovedChild);
            events++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, lineA, circleB, lineB });

        Assert.AreEqual(2, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, _) => events++);

        parent.RemoveShapes([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new Geometry("cs") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, _) => events++);

        parent.RemoveShapes([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Only_Reflective()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circle, line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circle, line] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circleA, line, circleB] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circleA, line, circleB] };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.DeletedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB });

        Assert.AreEqual(1, events);
    }

    #endregion

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, _) => events++);

        parent.AddShapes(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, _) => events++);

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, _) => events++);

        parent.InsertShapes(0, values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, _) => events++);

        parent.RemoveShapes(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new Geometry("g");
        var circle = new Circle("myId");
        parent.AddShapes([circle]);
        var values = new List<Coord>();

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circle, args.DeletedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        Assert.AreEqual(1, events);
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        parent.AddShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        Assert.AreEqual(2, events);
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        parent.InsertShapes(0, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Empty_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1 + events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        parent.InsertShapes(1, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildAddedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1 + events, args.Index);
            Assert.AreEqual(values[events], args.NewChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, valueA, valueB, circleB });

        Assert.AreEqual(2, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.AddShapes(values);

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_ListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.AddShapes(values);

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, _) => events++);

        parent.RemoveShapes(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            events++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained_Reflective()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Only_Reflective()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last_Reflective()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        int[] indices = [0, 1];
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(indices[events], args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        int[] indices = [0, 1];
        parent.Publisher.Subscribe<ChildDeletedEvent>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(indices[events], args.Index);
            Assert.AreEqual(values[events], args.DeletedChild);
            events++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, events);
    }

    #endregion

    #endregion
}