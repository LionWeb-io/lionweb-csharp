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
using M1;

[TestClass]
public class ReferenceTests_Multiple_Optional_Listener
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
        };

        parent.AddShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Add_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { line });

        Assert.AreEqual(1, events);
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
        };

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { line, circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
        };

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circle, line });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
            events++;
        };

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
            events++;
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { line, circleA, circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
            events++;
        };

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
            events++;
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
            events++;
        };

        parent.InsertShapes(2, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.NewTarget);
            events++;
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Empty_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> {  });

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new ReferenceGeometry("cs") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Only_Reflective()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> {  });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line, circle] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line, circle] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, line] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, line] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circle });

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, line, circleB] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, line, circleB] };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, line), args.DeletedTarget);
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circleA, circleB });

        Assert.AreEqual(1, events);
    }

    #endregion

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
        };

        parent.AddShapes(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
        };
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            events++;
        };

        parent.InsertShapes(0, values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var circle = new Circle("myId");
        parent.AddShapes([circle]);
        var values = new List<IShape>();

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, circle), args.DeletedTarget);
            events++;
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);

        Assert.AreEqual(1, events);
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.NewTarget);
            events++;
        };

        parent.AddShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.NewTarget);
            events++;
        };

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);

        Assert.AreEqual(2, events);
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(events, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.NewTarget);
            events++;
        };

        parent.InsertShapes(0, values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1 + events, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.NewTarget);
            events++;
        };

        parent.InsertShapes(1, values);

        Assert.AreEqual(2, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        parent.AddShapes(values);

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.DeletedTarget);
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new ReferenceGeometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new ReferenceGeometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, circleA), args.DeletedTarget);
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.DeletedTarget);
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.DeletedTarget);
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.DeletedTarget);
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        int[] indexes = { 0, 1 };
        parent.Listener.ReferenceDeleted += (sender, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(indexes[events], args.Index);
            Assert.AreEqual(new ReferenceTarget(null, values[events]), args.DeletedTarget);
            events++;
        };

        parent.RemoveShapes(values);

        Assert.AreEqual(2, events);
    }

    #endregion

    #endregion
}