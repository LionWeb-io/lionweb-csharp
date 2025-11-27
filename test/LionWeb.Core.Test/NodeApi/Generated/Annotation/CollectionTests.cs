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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation;

using Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class CollectionTests
{
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
        Assert.ThrowsExactly<InvalidValueException>(() => parent.AddAnnotations(values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new Line("g");
        var values = new INode[] { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new Line("g");
        var values = new INode[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.InsertAnnotations(0, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new Line("g");
        var values = new INode[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveAnnotations(values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new Line("g");
        var values = new ArrayList() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new Line("g");
        var values = new List<INode>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new Line("g");
        var values = new List<Shape>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var values = new List<Coord>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new Line("g");
        var values = new HashSet<INode>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
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
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Line("g");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    #endregion
}