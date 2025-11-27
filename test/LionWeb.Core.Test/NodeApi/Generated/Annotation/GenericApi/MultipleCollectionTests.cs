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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.GenericApi;

using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void MultipleArray()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Add(null, values);
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
        parent.Insert(null, 0, values);
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
        parent.Insert(null, 0, values);
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
        parent.Insert(null, 0, values);
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
        parent.Insert(null, 0, values);
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
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_Before()
    {
        var doc = new Documentation("cId");
        var parent = new Line("g");
        parent.Add(null, [doc]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 0, values);
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
        parent.Add(null, [ann]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 1, values);
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
        parent.Add(null, [docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 0, values);
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
        parent.Add(null, [docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 1, values);
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
        parent.Add(null, [docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 2, values);
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
        parent.Remove(null, values);
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
        parent.Remove(null, values);
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
        parent.Remove(null, values);
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
        parent.Remove(null, values);
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
        parent.Remove(null, values);
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
        parent.Add(null, [docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
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
        parent.Add(null, [docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var values = new INode[] { valueA, docA };
        parent.Remove(null, values);
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
        parent.Add(null, [valueA, valueB]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
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
        parent.Add(null, [valueA, valueB, doc]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
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
        parent.Add(null, [doc, valueA, valueB]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
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
        parent.Add(null, [docA, valueA, valueB, docB]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
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
        parent.Add(null, [valueA, docA, valueB, docB]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    [TestMethod]
    public void MultipleListMatchingType()
    {
        var parent = new Line("g");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.Add(null, values);
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
        parent.Add(null, values);
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
        parent.Add(null, values);
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
        parent.Add(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void SingleList_NotAnnotating()
    {
        var parent = new Geometry("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(null, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Insert()
    {
        var parent = new Geometry("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(null, 0, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Remove()
    {
        var parent = new Geometry("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        parent.Remove(null, values);
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }
}