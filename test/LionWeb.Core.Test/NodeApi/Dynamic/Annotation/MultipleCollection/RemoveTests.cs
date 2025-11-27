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

namespace LionWeb.Core.Test.NodeApi.Dynamic.Annotation.MultipleCollection;

[TestClass]
public class RemoveTests : DynamicNodeTestsBase
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new HashSet<INode>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new SingleEnumerable<INode>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Empty()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void NonContained()
    {
        var docA = newDocumentation("cA");
        var docB = newDocumentation("cB");
        var parent = newLine("cs");
        parent.AddAnnotations([docA, docB]);
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void HalfContained()
    {
        var docA = newDocumentation("cA");
        var docB = newDocumentation("cB");
        var parent = newLine("cs");
        parent.AddAnnotations([docA, docB]);
        var valueA = newBillOfMaterials("sA");
        var values = new INode[] { valueA, docA };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(docA.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Only()
    {
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var parent = newLine("g");
        parent.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void First()
    {
        var doc = newDocumentation("cId");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var parent = newLine("g");
        parent.AddAnnotations([valueA, valueB, doc]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Last()
    {
        var doc = newDocumentation("cId");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var parent = newLine("g");
        parent.AddAnnotations([doc, valueA, valueB]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Between()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var parent = newLine("g");
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
    public void Mixed()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var parent = newLine("g");
        parent.AddAnnotations([valueA, docA, valueB, docB]);
        var values = new INode[] { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }
}