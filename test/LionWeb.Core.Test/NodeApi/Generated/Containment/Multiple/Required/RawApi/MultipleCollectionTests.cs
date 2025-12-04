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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void List()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(valueB));
    }

    #region Insert

    [TestMethod]
    public void Insert_ListMatchingType()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0, valueA));
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, valueB));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0, valueA));
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, valueB));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0, valueA));
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, valueB));
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { valueA, valueB, circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, valueA));
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 2, valueB));
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circle, valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0, valueA));
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, valueB));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { valueA, valueB, circleA, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, valueA));
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 2, valueB));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circleA, valueA, valueB, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 2, valueA));
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 3, valueB));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circleA, circleB, valueA, valueB }, parent.Containment_1_n.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [valueA, valueB] };
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circleA, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var values = new List<IWritableNode> { valueA, circleA };
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, circleA));
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [valueA, valueB, circle] };
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle, valueA, valueB] };
        var values = new List<IWritableNode> { circle, valueA, valueB };
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, circle));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, valueA, valueB, circleB] };
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circleA, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [valueA, circleA, valueB, circleB] };
        var values = new List<IWritableNode> { valueA, valueB };
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IWritableNode> { circleA, circleB }, parent.Containment_1_n.ToList());
    }

    #endregion

    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<IWritableNode>() { valueA, valueB };
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueA));
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, valueB));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(valueB));
    }
}