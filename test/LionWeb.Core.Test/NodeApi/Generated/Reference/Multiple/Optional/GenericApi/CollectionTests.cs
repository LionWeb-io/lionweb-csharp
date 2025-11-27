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
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];
        parent.Add(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 0, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[0];
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(ShapesLanguage.Instance.ReferenceGeometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 0, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new ReferenceGeometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new ReferenceGeometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Add(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new ReferenceGeometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Insert(ShapesLanguage.Instance.ReferenceGeometry_shapes, 0, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var line = new Line("myId");
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line] };
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [line, circle] };
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, line] };
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, line, circleB] };
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion

    #endregion
}