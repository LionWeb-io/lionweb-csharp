﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.NodeApi.Lenient;

using Languages;
using Languages.Generated.V2024_1.Shapes.M2;
using M2;
using M3;
using System.Collections;

[TestClass]
public class ContainmentTests_Multiple_Optional : LenientNodeTestsBase
{
    #region Single

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = newGeometry("g");
        var line = newLine("myId");
        parent.Set(Geometry_shapes, line);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(line));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newGeometry("g");
        parent.Set(Geometry_shapes, null);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = newGeometry("g");
        var values = new LenientNode[0];
        parent.Set(Geometry_shapes, values);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var values = new ArrayList();
        parent.Set(Geometry_shapes, values);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<LenientNode>();
        parent.Set(Geometry_shapes, values);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<INode>();
        parent.Set(Geometry_shapes, values);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = newGeometry("g");
        var values = new HashSet<LenientNode>();
        parent.Set(Geometry_shapes, values);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<string>();
        parent.Set(Geometry_shapes, values);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = newGeometry("g");
        parent.Set(Geometry_shapes, new List<LenientNode> { newCircle("myId") });
        var values = new List<LenientNode>();
        parent.Set(Geometry_shapes, values);
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = newGeometry("g");
        var values = new LenientNode[] { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_shapes, values));
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_shapes, values));
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_shapes, values));
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<INode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_shapes, values));
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<string>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_shapes, values));
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = newGeometry("g");
        var values = new HashSet<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_shapes, values));
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Count() == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = newGeometry("g");
        var value = newLine("s");
        var values = new LenientNode[] { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var circle = newCircle("cc");
        var parent = newGeometry("g");
        parent.Set(Geometry_shapes, new LenientNode[] { circle });
        var value = newLine("s");
        var values = new LenientNode[] { value };
        parent.Set(Geometry_shapes, values);
        Assert.IsNull(circle.GetParent());
        Assert.AreSame(parent, value.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { value },
            (parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).ToList());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = newGeometry("g");
        var value = newLine("s");
        var values = new object[] { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var value = newLine("s");
        var values = new ArrayList() { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newLine("s");
        var values = new List<LenientNode>() { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = newGeometry("g");
        var value = newLine("s");
        var values = new List<INode>() { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = newGeometry("g");
        var value = newLine("s");
        var values = new HashSet<LenientNode>() { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCoord("c");
        var values = new List<LenientNode>() { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCoord("c");
        var values = new ArrayList() { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCoord("c");
        var values = new object[] { value };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(value));
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new LenientNode[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleListSubtype_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new HashSet<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleSingleEnumerable_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new SingleEnumerable<LenientNode> { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new object[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new LenientNode[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        var result = parent.Get(Geometry_shapes);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = newGeometry("g");
        var result = parent.Get(Geometry_shapes);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    #endregion

    #region NodeVariants

    [TestMethod]
    public void INode_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<INode> { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void DynamicNode_Reflective()
    {
        var parent = newGeometry("g");
        Classifier line = ShapesDynamic.Language.ClassifierByKey("key-Line");
        var valueA = new DynamicNode("sA", line);
        var valueB = new DynamicNode("sA", line);
        var values = new List<INode> { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void LenientNode_Reflective()
    {
        var parent = newGeometry("g");
        Classifier line = ShapesDynamic.Language.ClassifierByKey("key-Line");
        var valueA = new LenientNode("sA", line);
        var valueB = new LenientNode("sA", line);
        var values = new List<INode> { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void IReadableNode_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = new ReadOnlyLine("sA", null) {Name = "nameA", Uuid = "uuidA", Start = new Coord("startA"), End = new Coord("endA")};
        var valueB = new ReadOnlyLine("sB", null) {Name = "nameB", Uuid = "uuidB", Start = new Coord("startB"), End = new Coord("endB")};
        var values = new ArrayList { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Geometry_shapes, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Any());
    }

    #endregion

    #region MetamodelViolation

    [TestMethod]
    public void String_Reflective()
    {
        var parent = newGeometry("od");
        var value = "a";
        parent.Set(Geometry_shapes, value);
        Assert.AreEqual("a", parent.Get(Geometry_shapes));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = newGeometry("od");
        var value = -10;
        parent.Set(Geometry_shapes, value);
        Assert.AreEqual(-10, parent.Get(Geometry_shapes));
    }

    #endregion
}