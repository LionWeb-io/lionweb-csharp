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

using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void MultipleArray()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<INode> { valueA, valueB };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_ListSubtype()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<TestAnnotation> { valueA, valueB };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Set()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new HashSet<INode> { valueA, valueB };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_SingleEnumerable()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new SingleEnumerable<INode> { valueA, valueB };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_Before()
    {
        var existing = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [existing]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, existing.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB, existing }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_After()
    {
        var existing = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [existing]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 1, values);
        Assert.AreSame(parent, existing.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existing, valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Before()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB, existingA, existingB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 1, values);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existingA, valueA, valueB, existingB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_After()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };
        parent.Insert(null, 2, values);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existingA, existingB, valueA, valueB }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<TestAnnotation>() { valueA, valueB };
        parent.Remove(null, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Set()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var existingA = new TestAnnotation("cA");
        var existingB = new TestAnnotation("cB");
        var parent = new LinkTestConcept("cs");
        parent.Add(null, [existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existingA, existingB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var existingA = new TestAnnotation("cA");
        var existingB = new TestAnnotation("cB");
        var parent = new LinkTestConcept("cs");
        parent.Add(null, [existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var values = new INode[] { valueA, existingA };
        parent.Remove(null, values);
        Assert.AreSame(parent, existingB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(existingA.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existingB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
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
        var existing = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [valueA, valueB, existing]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
        Assert.AreSame(parent, existing.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existing }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var existing = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [existing, valueA, valueB]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
        Assert.AreSame(parent, existing.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existing }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [existingA, valueA, valueB, existingB]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existingA, existingB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [valueA, existingA, valueB, existingB]);
        var values = new INode[] { valueA, valueB };
        parent.Remove(null, values);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existingA, existingB }, parent.GetAnnotations().ToList());
    }

    #endregion

    [TestMethod]
    public void MultipleListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<TestAnnotation>() { valueA, valueB };
        parent.Add(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }


    [TestMethod]
    public void MultipleSet()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
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
        var parent = new TestPartition("g");
        var value = new RestrictedTestAnnotation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(null, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Insert()
    {
        var parent = new TestPartition("g");
        var value = new RestrictedTestAnnotation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(null, 0, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Remove()
    {
        var parent = new TestPartition("g");
        var value = new RestrictedTestAnnotation("sA");
        var values = new List<INode>() { value };
        parent.Remove(null, values);
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }
}
