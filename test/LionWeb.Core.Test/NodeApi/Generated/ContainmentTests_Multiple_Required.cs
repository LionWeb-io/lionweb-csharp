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
using System.Collections;

[TestClass]
public class ContainmentTests_Multiple_Required
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        parent.AddParts([line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, line));
        Assert.AreSame(null, line.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Single_Constructor()
    {
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [line] };
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        parent.InsertParts(0, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertParts(-1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertParts(1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var line = new Line("myId");
        parent.InsertParts(0, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var line = new Line("myId");
        parent.InsertParts(1, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circle, line }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertParts(0, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertParts(1, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, line, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertParts(2, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Parts.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts([line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var line = new Line("myId");
        parent.RemoveParts([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [line] };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts([line]));
        Assert.AreSame(parent, line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { line }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [line, circle] };
        parent.RemoveParts([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [circle, line] };
        parent.RemoveParts([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        parent.RemoveParts([line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new CompositeShape("cs");
        Assert.ThrowsException<InvalidValueException>(() => parent.AddParts(null));
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new CompositeShape("cs");
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, null));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        Assert.ThrowsException<InvalidValueException>(() => new CompositeShape("cs") { Parts = null });
    }

    [TestMethod]
    public void Null_Insert_Empty()
    {
        var parent = new CompositeShape("cs");
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertParts(0, null));
    }

    [TestMethod]
    public void Null_Insert_Empty_OutOfBounds()
    {
        var parent = new CompositeShape("cs");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertParts(1, null));
    }

    [TestMethod]
    public void Null_Remove_Empty()
    {
        var parent = new CompositeShape("cs");
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(null));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => parent.AddParts(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Constructor()
    {
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => new CompositeShape("cs") { Parts = values });
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertParts(0, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new ArrayList();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new List<IShape>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new List<Shape>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new HashSet<IShape>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Circle("myId");
        parent.AddParts([value]);
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        CollectionAssert.AreEqual(new List<IShape> { value }, parent.Parts.ToList());
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.AddParts(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void NullArray_Constructor()
    {
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => new CompositeShape("cs") { Parts = values });
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertParts(0, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new CompositeShape("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new List<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new List<Shape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new CompositeShape("cs");
        var values = new HashSet<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.AddParts(values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var circle = new Circle("cc");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.IsNull(circle.GetParent());
        Assert.AreSame(parent, value.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { value }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SingleArray_Constructor()
    {
        var value = new Line("s");
        var values = new IShape[] { value };
        var parent = new CompositeShape("cs") { Parts = values };
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        var values = new IShape[] { line };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [line] };
        var values = new IShape[] { line };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.AreSame(parent, line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { line }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [line, circle] };
        var values = new IShape[] { line };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [circle, line] };
        var values = new IShape[] { line };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var values = new IShape[] { line };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    #endregion

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new object[] { value };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new ArrayList() { value };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new List<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new List<Shape>() { value };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Line("s");
        var values = new HashSet<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Parts.Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Coord("c");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.AddParts(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleArray_Constructor()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        var parent = new CompositeShape("cs") { Parts = values };
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_ListMatchingType()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_ListSubtype()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Set()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_SingleEnumerable()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertParts(1, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle, valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertParts(0, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertParts(1, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, valueA, valueB, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertParts(2, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, valueA, valueB }, parent.Parts.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_ListSubtype()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Set()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_SingleEnumerable()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Empty()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveParts(values));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_First()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [valueA, valueB, circle] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveParts(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    #endregion

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.AddParts(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListSubtype()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.AddParts(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListSubtype_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.AddParts(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSingleEnumerable()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape>() { valueA, valueB };
        parent.AddParts(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSingleEnumerable_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count == 0);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var parent = new CompositeShape("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        var result = parent.Get(ShapesLanguage.Instance.CompositeShape_parts);
        CollectionAssert.AreEqual(new List<INode>() { valueA, valueB }, (result as IEnumerable<INode>).ToList());
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = new CompositeShape("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        var result = parent.Get(ShapesLanguage.Instance.CompositeShape_parts);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = new CompositeShape("g");
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(ShapesLanguage.Instance.CompositeShape_parts));
    }

    #endregion
}