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

namespace LionWeb.Core.M2.Generated.Test;

using Core;
using Examples.Shapes.M2;
using System.Collections;

[TestClass]
public class ContainmentTests_Annotation
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        parent.AddAnnotations([bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(null, bom));
        Assert.AreSame(null, bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertAnnotations(-1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertAnnotations(1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var doc = new Documentation("cId");
        var parent = new Line("g");
        parent.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var doc = new Documentation("cId");
        var parent = new Line("g");
        parent.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(1, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(1, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, bom, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(2, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, docB, bom }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        parent.RemoveAnnotations([bom]);
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var doc = new Documentation("myC");
        var parent = new Line("cs");
        parent.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([bom]);
        parent.RemoveAnnotations([bom]);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([bom, doc]);
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([doc, bom]);
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([docA, bom, docB]);
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new Line("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.AddAnnotations(null));
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new Line("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(null, null));
    }

    [TestMethod]
    public void Null_Insert_Empty()
    {
        var parent = new Line("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertAnnotations(0, null));
    }

    [TestMethod]
    public void Null_Insert_Empty_OutOfBounds()
    {
        var parent = new Line("g");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertAnnotations(1, null));
    }

    [TestMethod]
    public void Null_Remove_Empty()
    {
        var parent = new Line("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveAnnotations(null));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new Line("g");
        var values = new INode[0];
        parent.AddAnnotations(values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new Line("g");
        var values = new INode[0];
        parent.Set(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new Line("g");
        var values = new INode[0];
        parent.InsertAnnotations(0, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new Line("g");
        var values = new INode[0];
        parent.RemoveAnnotations(values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new Line("g");
        var values = new ArrayList();
        parent.Set(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new Line("g");
        var values = new List<INode>();
        parent.Set(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = new Line("g");
        var values = new List<Shape>();
        parent.Set(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new Line("g");
        var values = new HashSet<INode>();
        parent.Set(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var values = new List<Coord>();
        parent.Set(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new Line("g");
        var values = new INode[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.AddAnnotations(values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new Line("g");
        var values = new INode[] { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new Line("g");
        var values = new INode[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertAnnotations(0, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new Line("g");
        var values = new INode[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveAnnotations(values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new Line("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new Line("g");
        var values = new List<INode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new Line("g");
        var values = new List<Shape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new Line("g");
        var values = new HashSet<INode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new INode[] { value };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new INode[] { value };
        parent.Set(null, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var doc = new Documentation("cc");
        var parent = new Line("g");
        parent.AddAnnotations([]);
        var value = new BillOfMaterials("s");
        var values = new INode[] { value };
        parent.Set(null, values);
        Assert.IsNull(doc.GetParent());
        Assert.AreSame(parent, value.GetParent());
        CollectionAssert.AreEqual(new List<INode> { value }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new INode[] { value };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        var values = new INode[] { bom };
        parent.RemoveAnnotations(values);
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([bom]);
        var values = new INode[] { bom };
        parent.RemoveAnnotations(values);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([bom, doc]);
        var values = new INode[] { bom };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([doc, bom]);
        var values = new INode[] { bom };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([docA, bom, docB]);
        var values = new INode[] { bom };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new object[] { value };
        parent.Set(null, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new ArrayList() { value };
        parent.Set(null, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new List<INode>() { value };
        parent.Set(null, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new List<BillOfMaterials>() { value };
        parent.Set(null, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new Line("g");
        var value = new BillOfMaterials("s");
        var values = new HashSet<INode>() { value };
        parent.Set(null, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var value = new Coord("c");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_ListMatchingType()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<INode> { valueA, valueB };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_ListSubtype()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<BillOfMaterials> { valueA, valueB };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Set()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new HashSet<INode> { valueA, valueB };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_SingleEnumerable()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new SingleEnumerable<INode> { valueA, valueB };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_Before()
    {
        var doc = new Documentation("cId");
        var parent = new Line("g");
        parent.AddAnnotations([doc]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB, doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_After()
    {
        var ann = new BillOfMaterials("cId");
        var parent = new Line("g");
        parent.AddAnnotations([ann]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.InsertAnnotations(1, values);
        Assert.AreSame(parent, ann.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { ann, valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Before()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.InsertAnnotations(0, values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB, docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.InsertAnnotations(1, values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, valueA, valueB, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_After()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.InsertAnnotations(2, values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB, valueA, valueB }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_ListSubtype()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<BillOfMaterials>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Set()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new HashSet<INode>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_SingleEnumerable()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new SingleEnumerable<INode>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Empty()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var parent = new Line("cs");
        parent.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var parent = new Line("cs");
        parent.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var values = new INode[] { valueA, docA };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(docA.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var parent = new Line("g");
        parent.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_First()
    {
        var doc = new Documentation("cId");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var parent = new Line("g");
        parent.AddAnnotations([valueA, valueB, doc]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var doc = new Documentation("cId");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var parent = new Line("g");
        parent.AddAnnotations([doc, valueA, valueB]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, valueA, valueB, docB]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var parent = new Line("g");
        parent.AddAnnotations([valueA, docA, valueB, docB]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleListSubtype()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<BillOfMaterials>() { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleListSubtype_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<BillOfMaterials>() { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new HashSet<INode>() { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new HashSet<INode>() { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleSingleEnumerable()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new SingleEnumerable<INode> { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleSingleEnumerable_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new SingleEnumerable<INode> { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void SingleList_NotAnnotating()
    {
        var parent = new Geometry("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsException<InvalidValueException>(() => parent.AddAnnotations(values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(null, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Insert()
    {
        var parent = new Geometry("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertAnnotations(0, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Remove()
    {
        var parent = new Geometry("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        parent.RemoveAnnotations(values);
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Set(null, values);
        var result = parent.Get(null);
        CollectionAssert.AreEqual(new List<INode>() {valueA, valueB}, (result as IList<INode>).ToList());
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Set(null, values);
        var result = parent.Get(null);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = new Line("g");
        var result = parent.Get(null);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    #endregion
}