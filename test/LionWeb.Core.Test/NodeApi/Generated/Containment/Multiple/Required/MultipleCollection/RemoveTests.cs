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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.MultipleCollection;

using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveContainment_1_n(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveContainment_1_n(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new HashSet<LinkTestConcept> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveContainment_1_n(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new SingleEnumerable<LinkTestConcept> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveContainment_1_n(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueB));
    }

    [TestMethod]
    public void Empty()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveContainment_1_n(values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(valueB));
    }

    [TestMethod]
    public void Only()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveContainment_1_n(values));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void NonContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.RemoveContainment_1_n(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void HalfContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var values = new LinkTestConcept[] { valueA, circleA };
        parent.RemoveContainment_1_n(values);
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void First()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [valueA, valueB, circle] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.RemoveContainment_1_n(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Last()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle, valueA, valueB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.RemoveContainment_1_n(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, valueA, valueB, circleB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.RemoveContainment_1_n(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Mixed()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [valueA, circleA, valueB, circleB] };
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.RemoveContainment_1_n(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Containment_1_n.ToList());
    }
}