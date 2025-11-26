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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        parent.AddShapes([line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, line));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Constructor()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void TryGet()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };
        Assert.IsTrue(parent.TryGetShapes(out var o));
        Assert.AreSame(line, o.FirstOrDefault());
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        parent.InsertShapes(0, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertShapes(-1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertShapes(1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");
        parent.InsertShapes(0, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");
        parent.InsertShapes(1, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circle, line }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertShapes(0, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertShapes(1, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, line, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertShapes(2, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Shapes.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        parent.RemoveShapes([line]);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new ReferenceGeometry("cs") { Shapes = [circle] };
        var line = new Line("myId");
        parent.RemoveShapes([line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };
        parent.RemoveShapes([line]);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line, circle] };
        parent.RemoveShapes([line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, line] };
        parent.RemoveShapes([line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, line, circleB] };
        parent.RemoveShapes([line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion
}