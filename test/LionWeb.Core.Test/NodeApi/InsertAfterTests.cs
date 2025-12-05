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

namespace LionWeb.Core.Test.NodeApi;

using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class InsertAfterTests
{
    [TestMethod]
    public void End()
    {
        var circle = new LinkTestConcept("circ0");
        var offsetDuplicate = new LinkTestConcept("off0");

        var geometry = new TestPartition("geom")
        {
            Links =
            [
                circle,
                offsetDuplicate
            ]
        };
        var line = new LinkTestConcept("line");
        offsetDuplicate.InsertAfter(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.AreEqual(geometry, offsetDuplicate.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, offsetDuplicate, line }, geometry.Links.ToList());
    }

    [TestMethod]
    public void Middle()
    {
        var circle = new LinkTestConcept("circ0");
        var offsetDuplicate = new LinkTestConcept("off0");

        var geometry = new TestPartition("geom")
        {
            Links =
            [
                circle,
                offsetDuplicate
            ]
        };
        var line = new LinkTestConcept("line");
        circle.InsertAfter(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.AreEqual(geometry, circle.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, line, offsetDuplicate }, geometry.Links.ToList());
    }

    [TestMethod]
    public void NoParent()
    {
        var circle = new LinkTestConcept("circ0");
        var line = new LinkTestConcept("line");

        Assert.ThrowsExactly<TreeShapeException>(() => circle.InsertAfter(line));
    }

    [TestMethod]
    public void SingleContainment()
    {
        var coord = new LinkTestConcept("coord0");
        var circle = new LinkTestConcept("circ0") { Containment_0_1 = coord };

        var line = new LinkTestConcept("line");

        Assert.ThrowsExactly<TreeShapeException>(() => coord.InsertAfter(line));
    }

    [TestMethod]
    public void NonFittingType()
    {
        var circle = new LinkTestConcept("circ0");

        var geometry = new TestPartition("geom")
        {
            Links =
            [
                circle
            ]
        };
        var coord = new DataTypeTestConcept("coord");
        Assert.ThrowsExactly<InvalidValueException>(() => circle.InsertAfter(coord));
    }

    [TestMethod]
    public void Null()
    {
        var circle = new LinkTestConcept("circ0");

        var geometry = new TestPartition("geom")
        {
            Links =
            [
                circle
            ]
        };
        Assert.ThrowsExactly<InvalidValueException>(() => circle.InsertAfter(null));
    }

    [TestMethod]
    public void NoOp()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n =
            [
                childA,
                childB
            ]
        };
        childA.InsertAfter(childB);

        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childA.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA, childB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SelfBeforeNewSuccessor()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");
        var childD = new LinkTestConcept("childD");
        var childE = new LinkTestConcept("childE");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n =
            [
                childA,
                childB,
                childC,
                childD,
                childE
            ]
        };
        childB.InsertAfter(childD);

        Assert.AreEqual(parent, childA.GetParent());
        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childC.GetParent());
        Assert.AreEqual(parent, childD.GetParent());
        Assert.AreEqual(parent, childE.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept>
        {
            childA,
            childB,
            childD,
            childC,
            childE
        }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SelfAfterNewSuccessor()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");
        var childD = new LinkTestConcept("childD");
        var childE = new LinkTestConcept("childE");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n =
            [
                childA,
                childB,
                childC,
                childD,
                childE
            ]
        };
        childD.InsertAfter(childB);

        Assert.AreEqual(parent, childA.GetParent());
        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childC.GetParent());
        Assert.AreEqual(parent, childD.GetParent());
        Assert.AreEqual(parent, childE.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept>
        {
            childA,
            childC,
            childD,
            childB,
            childE
        }, parent.Containment_0_n.ToList());
    }
    
    [TestMethod]
    public void SelfImmediatelyAfterNewSuccessor()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");
        var childD = new LinkTestConcept("childD");
        var childE = new LinkTestConcept("childE");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n =
            [
                childA,
                childB,
                childC,
                childD,
                childE
            ]
        };
        childD.InsertAfter(childC);

        Assert.AreEqual(parent, childA.GetParent());
        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childC.GetParent());
        Assert.AreEqual(parent, childD.GetParent());
        Assert.AreEqual(parent, childE.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept>
        {
            childA,
            childB,
            childD,
            childC,
            childE
        }, parent.Containment_0_n.ToList());
    }
}