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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.MultipleCollection;

using Languages.Generated.V2024_1.TestLanguage;
using System.Collections;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.AddLinks(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Array_Constructor()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };
        var parent = new TestPartition("g") { Links = values };
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void UntypedArray_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void UntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.AddLinks(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.AddLinks(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new HashSet<LinkTestConcept>() { valueA, valueB };
        parent.AddLinks(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new HashSet<LinkTestConcept>() { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new SingleEnumerable<LinkTestConcept> { valueA, valueB };
        parent.AddLinks(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new SingleEnumerable<LinkTestConcept> { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void ListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("cA");
        var valueB = new DataTypeTestConcept("cB");
        var values = new List<DataTypeTestConcept>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void UntypedListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("cA");
        var valueB = new DataTypeTestConcept("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void UntypedArrayNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("cA");
        var valueB = new DataTypeTestConcept("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        var result = parent.Get(TestLanguageLanguage.Instance.TestPartition_links);
        CollectionAssert.AreEqual(new List<INode>() { valueA, valueB }, (result as IEnumerable<INode>).ToList());
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        var result = parent.Get(TestLanguageLanguage.Instance.TestPartition_links);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = new TestPartition("g");
        var result = parent.Get(TestLanguageLanguage.Instance.TestPartition_links);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }
}