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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.GenericApi;

using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class GenericApiTests
{
    #region Single

    [TestMethod]
    public void Reference_Single_Add()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        parent.Add(ShapesLanguage.Instance.ReferenceGeometry_shapes, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 0, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, -1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 0, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 1, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circle, line }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 0, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 1, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, line, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 2, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Shapes.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new ReferenceGeometry("cs") { Shapes = [circle] };
        var line = new Line("myId");
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, [line]);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line, circle] };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, line] };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, line, circleB] };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new ReferenceGeometry("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(ShapesLanguage.Instance.ReferenceGeometry_shapes, [null]));
    }

    [TestMethod]
    public void Null_Insert_Empty()
    {
        var parent = new ReferenceGeometry("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 0, null));
    }

    [TestMethod]
    public void Null_Insert_Empty_OutOfBounds()
    {
        var parent = new ReferenceGeometry("g");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 1, [null]));
    }

    [TestMethod]
    public void Null_Remove_Empty()
    {
        var parent = new ReferenceGeometry("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, null));
    }

    #endregion
}