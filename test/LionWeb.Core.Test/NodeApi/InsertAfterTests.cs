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

using Languages.Generated.V2024_1.Shapes.M2;
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class InsertAfterTests
{
    [TestMethod]
    public void End()
    {
        var circle = new Circle("circ0");
        var offsetDuplicate = new OffsetDuplicate("off0");

        var geometry = new Geometry("geom")
        {
            Shapes =
            [
                circle,
                offsetDuplicate
            ]
        };
        var line = new Line("line");
        offsetDuplicate.InsertAfter(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.AreEqual(geometry, offsetDuplicate.GetParent());

        CollectionAssert.AreEqual(new List<IShape> { circle, offsetDuplicate, line }, geometry.Shapes.ToList());
    }

    [TestMethod]
    public void Middle()
    {
        var circle = new Circle("circ0");
        var offsetDuplicate = new OffsetDuplicate("off0");

        var geometry = new Geometry("geom")
        {
            Shapes =
            [
                circle,
                offsetDuplicate
            ]
        };
        var line = new Line("line");
        circle.InsertAfter(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.AreEqual(geometry, circle.GetParent());

        CollectionAssert.AreEqual(new List<IShape> { circle, line, offsetDuplicate }, geometry.Shapes.ToList());
    }

    [TestMethod]
    public void NoParent()
    {
        var circle = new Circle("circ0");
        var line = new Line("line");

        Assert.ThrowsExactly<TreeShapeException>(() => circle.InsertAfter(line));
    }

    [TestMethod]
    public void SingleContainment()
    {
        var coord = new Coord("coord0");
        var circle = new Circle("circ0") { Center = coord };

        var line = new Line("line");

        Assert.ThrowsExactly<TreeShapeException>(() => coord.InsertAfter(line));
    }

    [TestMethod]
    public void NonFittingType()
    {
        var circle = new Circle("circ0");

        var geometry = new Geometry("geom")
        {
            Shapes =
            [
                circle
            ]
        };
        var coord = new Coord("coord");
        Assert.ThrowsExactly<InvalidValueException>(() => circle.InsertAfter(coord));
    }

    [TestMethod]
    public void Null()
    {
        var circle = new Circle("circ0");

        var geometry = new Geometry("geom")
        {
            Shapes =
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
}