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

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<TestAnnotation>() { valueA, valueB };
        parent.RemoveAnnotations(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var docA = new TestAnnotation("cA");
        var docB = new TestAnnotation("cB");
        var parent = new LinkTestConcept("cs");
        parent.AddAnnotations([docA, docB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var docA = new TestAnnotation("cA");
        var docB = new TestAnnotation("cB");
        var parent = new LinkTestConcept("cs");
        parent.AddAnnotations([docA, docB]);
        var valueA = new TestAnnotation("sA");
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
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
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
        var doc = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
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
        var doc = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
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
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
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
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
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
