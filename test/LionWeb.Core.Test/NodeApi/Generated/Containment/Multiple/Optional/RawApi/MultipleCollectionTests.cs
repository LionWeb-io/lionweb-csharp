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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class MultipleCollectionTests
{
    #region Insert

    [TestMethod]
    public void Insert_ListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, values));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, values));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, values));
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB, circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 1, values));
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle, valueA, valueB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, values));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB, circleA, circleB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 1, values));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleA, valueA, valueB, circleB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 2, values));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB, valueA, valueB }, parent.Containment_0_n.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode>() { valueA, valueB };
        Assert.IsFalse(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(valueA));
        Assert.IsFalse(parent.Containment_0_n.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsFalse(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(valueA));
        Assert.IsFalse(parent.Containment_0_n.Contains(valueB));
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode> { valueA, valueB };
        Assert.IsFalse(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var values = new List<INode> { valueA, circleA };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [valueA, valueB] };
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [valueA, valueB, circle] };
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circle, valueA, valueB] };
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, valueA, valueB, circleB] };
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [valueA, circleA, valueB, circleB] };
        var values = new List<INode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB }, parent.Containment_0_n.ToList());
    }

    #endregion


    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<INode>() { valueA, valueB };
        Assert.IsTrue(parent.AddRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(valueB));
    }
}