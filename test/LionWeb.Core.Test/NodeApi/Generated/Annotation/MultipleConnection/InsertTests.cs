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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.MultipleConnection;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class InsertTests
{
    [TestMethod]
    public void ListMatchingType()
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
    public void ListSubtype()
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
    public void Set()
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
    public void SingleEnumerable()
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
    public void Empty()
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
    public void One_Before()
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
    public void One_After()
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
    public void Two_Before()
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
    public void Two_Between()
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
    public void Two_After()
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
}