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
public class InsertTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };
        parent.InsertContainment_1_n(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };
        parent.InsertContainment_1_n(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Set()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new HashSet<LinkTestConcept> { valueA, valueB };
        parent.InsertContainment_1_n(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new SingleEnumerable<LinkTestConcept> { valueA, valueB };
        parent.InsertContainment_1_n(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Empty()
    {
        var parent = new LinkTestConcept("cs");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.InsertContainment_1_n(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.InsertContainment_1_n(0, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB, circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.InsertContainment_1_n(1, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, valueA, valueB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.InsertContainment_1_n(0, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { valueA, valueB, circleA, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.InsertContainment_1_n(1, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, valueA, valueB, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };
        parent.InsertContainment_1_n(2, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB, valueA, valueB }, parent.Containment_1_n.ToList());
    }
}