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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional;

using Languages.Generated.V2024_1.Shapes.M2;
using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        parent.AddShapes([line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.Geometry_shapes, line));
        Assert.AreSame(null, line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Constructor()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void TryGet()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };
        Assert.IsTrue(parent.TryGetShapes(out var o));
        Assert.AreSame(line, o.FirstOrDefault());
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        parent.InsertShapes(0, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertShapes(-1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertShapes(1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");
        parent.InsertShapes(0, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");
        parent.InsertShapes(1, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circle, line }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertShapes(0, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertShapes(1, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, line, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertShapes(2, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_MoveForward()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n = 
            [
                childA,
                childB,
                childC
            ]
        };
        parent.InsertContainment_0_n(2, [childA]);

        Assert.AreEqual(parent, childA.GetParent());
        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childC.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB, childC, childA }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_MoveBackward()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n = 
            [
                childA,
                childB,
                childC
            ]
        };
        parent.InsertContainment_0_n(1, [childC]);

        Assert.AreEqual(parent, childA.GetParent());
        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childC.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA, childC, childB }, parent.Containment_0_n.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        parent.RemoveShapes([line]);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new Geometry("cs") { Shapes = [circle] };
        var line = new Line("myId");
        parent.RemoveShapes([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };
        parent.RemoveShapes([line]);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line, circle] };
        parent.RemoveShapes([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circle, line] };
        parent.RemoveShapes([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circleA, line, circleB] };
        parent.RemoveShapes([line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion
}