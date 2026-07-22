// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.GenericApi;

using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;

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
        parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }

    #region Insert

    [TestMethod]
    public void Insert_ListMatchingType()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_ListSubtype()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Set()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new HashSet<LinkTestConcept> { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_SingleEnumerable()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new SingleEnumerable<LinkTestConcept> { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB, circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 1, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, valueA, valueB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB, circleA, circleB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 1, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, valueA, valueB, circleB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 2, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB, valueA, valueB }, parent.Links.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Links.Contains(valueA));
        Assert.IsFalse(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Remove_ListSubtype()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Links.Contains(valueA));
        Assert.IsFalse(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Set()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new HashSet<LinkTestConcept>() { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Links.Contains(valueA));
        Assert.IsFalse(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Remove_SingleEnumerable()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new SingleEnumerable<LinkTestConcept>() { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Links.Contains(valueA));
        Assert.IsFalse(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Links.Contains(valueA));
        Assert.IsFalse(parent.Links.Contains(valueB));
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new TestPartition("cs") { Links = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new TestPartition("cs") { Links = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var values = new LinkTestConcept[] { valueA, circleA };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [valueA, valueB, circle] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [circle, valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [circleA, valueA, valueB, circleB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new TestPartition("g") { Links = [valueA, circleA, valueB, circleB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Links.ToList());
    }

    #endregion


    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values);
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
        parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values);
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
        parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values);
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
        parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Links.Contains(valueB));
    }
}