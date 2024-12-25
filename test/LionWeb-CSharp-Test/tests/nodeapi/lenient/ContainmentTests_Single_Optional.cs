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

namespace LionWeb.Core.M2.Lenient.Test;

using Examples.Shapes.Dynamic;
using Examples.V2024_1.Shapes.M2;
using M3;
using System.Collections;
using Utils.Tests;

[TestClass]
public class ContainmentTests_Single_Optional : LenientNodeTestsBase
{
    #region Single

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("myId");
        parent.Set(Geometry_documentation, doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Get(Geometry_documentation));
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldDoc = newDocumentation("old");
        var parent = newGeometry("g");
        parent.Set(Geometry_documentation, oldDoc);
        var doc = newDocumentation("myId");
        parent.Set(Geometry_documentation, doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Get(Geometry_documentation));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newGeometry("g");
        parent.Set(Geometry_documentation, null);
        Assert.IsNull(parent.Get(Geometry_documentation));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = newGeometry("g");
        var values = new LenientNode[0];
        parent.Set(Geometry_documentation, values);
        Assert.IsNull(parent.Get(Geometry_documentation));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var values = new ArrayList();
        parent.Set(Geometry_documentation, values);
        Assert.IsNull(parent.Get(Geometry_documentation));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<LenientNode>();
        parent.Set(Geometry_documentation, values);
        Assert.IsNull(parent.Get(Geometry_documentation));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = newGeometry("g");
        var values = new HashSet<LenientNode>();
        parent.Set(Geometry_documentation, values);
        Assert.IsNull(parent.Get(Geometry_documentation));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<string>();
        parent.Set(Geometry_documentation, values);
        Assert.IsNull(parent.Get(Geometry_documentation));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = newGeometry("g");
        var values = new LenientNode[] { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(Geometry_documentation, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_documentation, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_documentation, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var values = new List<string>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Geometry_documentation, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = newGeometry("g");
        var values = new HashSet<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(Geometry_documentation, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("s");
        var values = new LenientNode[] { value };

        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("s");
        var values = new object[] { value };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("s");
        var values = new ArrayList() { value };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("s");
        var values = new List<LenientNode>() { value };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("s");
        var values = new HashSet<LenientNode>() { value };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCoord("c");
        var values = new List<LenientNode>() { value };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCoord("c");
        var values = new ArrayList() { value };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCoord("c");
        var values = new object[] { value };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newDocumentation("sA");
        var valueB = newDocumentation("sB");
        var values = new LenientNode[] { valueA, valueB };

        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newDocumentation("sA");
        var valueB = newDocumentation("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newDocumentation("sA");
        var valueB = newDocumentation("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newDocumentation("sA");
        var valueB = newDocumentation("sB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newDocumentation("sA");
        var valueB = newDocumentation("sB");
        var values = new HashSet<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new object[] { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(Geometry_documentation) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
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
        parent.Set(Geometry_documentation, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_documentation) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_documentation) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void DynamicNode_Reflective()
    {
        var parent = newGeometry("g");
        Classifier line = ShapesDynamic.Language.ClassifierByKey("key-Line");
        var valueA = new DynamicNode("sA", line);
        var valueB = new DynamicNode("sA", line);
        var values = new List<INode> { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_documentation) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_documentation) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void LenientNode_Reflective()
    {
        var parent = newGeometry("g");
        Classifier line = ShapesDynamic.Language.ClassifierByKey("key-Line");
        var valueA = new LenientNode("sA", line);
        var valueB = new LenientNode("sA", line);
        var values = new List<INode> { valueA, valueB };
        parent.Set(Geometry_documentation, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_documentation) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_documentation) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void IReadableNode_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = new ReadOnlyLine("sA", null) {Name = "nameA", Uuid = "uuidA", Start = new Coord("startA"), End = new Coord("endA")};
        var valueB = new ReadOnlyLine("sB", null) {Name = "nameB", Uuid = "uuidB", Start = new Coord("startB"), End = new Coord("endB")};
        var values = new ArrayList { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Geometry_documentation, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsNull(parent.Get(Geometry_documentation));
    }

    #endregion

    #region MetamodelViolation

    [TestMethod]
    public void String_Reflective()
    {
        var parent = newGeometry("od");
        var value = "a";
        parent.Set(Geometry_documentation, value);
        Assert.AreEqual("a", parent.Get(Geometry_documentation));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = newGeometry("od");
        var value = -10;
        parent.Set(Geometry_documentation, value);
        Assert.AreEqual(-10, parent.Get(Geometry_documentation));
    }

    #endregion
}