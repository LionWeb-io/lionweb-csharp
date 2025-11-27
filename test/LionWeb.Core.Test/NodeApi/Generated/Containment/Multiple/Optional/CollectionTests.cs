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
using System.Collections;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.AddShapes(values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Constructor()
    {
        var values = new IShape[0];
        var parent = new Geometry("g") { Shapes = values };
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.InsertShapes(0, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.RemoveShapes(values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var values = new ArrayList();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<IShape>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Shape>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new Geometry("g");
        var values = new HashSet<IShape>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Coord>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new Geometry("g");
        parent.AddShapes([new Circle("myId")]);
        var values = new List<Coord>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.AddShapes(values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullArray_Constructor()
    {
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => new Geometry("g") { Shapes = values });
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertShapes(0, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveShapes(values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Shape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new Geometry("g");
        var values = new HashSet<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.AddShapes(values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var circle = new Circle("cc");
        var parent = new Geometry("g") { Shapes = [circle] };
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsNull(circle.GetParent());
        Assert.AreSame(parent, value.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { value }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Constructor()
    {
        var value = new Line("s");
        var values = new IShape[] { value };
        var parent = new Geometry("g") { Shapes = values };
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line, circle] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circle, line] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circleA, line, circleB] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new object[] { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new ArrayList() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new List<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new List<Shape>() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new HashSet<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion
}