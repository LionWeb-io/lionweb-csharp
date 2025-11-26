// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.GenericApi;

using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class SingleTests
{
    #region add

    [TestMethod]
    public void Add()
    {
        var parent = new Geometry("g");
        var line = new Line("line");
        parent.Add(ShapesLanguage.Instance.Geometry_shapes, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Multiple_Add()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");
        parent.Add(ShapesLanguage.Instance.Geometry_shapes, [line1, line2]);
        Assert.AreEqual(2, parent.Shapes.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.AreSame(parent, line2.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line1));
        Assert.IsTrue(parent.Shapes.Contains(line2));
    }

    #endregion

    #region insert

    [TestMethod]
    public void Insert()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line1]);
        Assert.AreEqual(1, parent.Shapes.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line1));
    }


    [TestMethod]
    public void Multiple_Insert()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line1, line2]);
        Assert.AreEqual(2, parent.Shapes.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.AreSame(parent, line2.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line1));
        Assert.IsTrue(parent.Shapes.Contains(line2));
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.Geometry_shapes, -1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line]);
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
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 1, [line]);
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
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line]);
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
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 1, [line]);
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
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 2, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Shapes.ToList());
    }

    #endregion

    #region remove

    [TestMethod]
    public void Remove()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line]);
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Multiple_Remove()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line1]);
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line2]);
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line1, line2]);
        Assert.IsEmpty(parent.Shapes);
        Assert.IsNull(line1.GetParent());
        Assert.IsNull(line2.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line1));
        Assert.IsFalse(parent.Shapes.Contains(line2));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new Geometry("cs") { Shapes = [circle] };
        var line = new Line("myId");
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line, circle] };
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
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
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
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
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion
}