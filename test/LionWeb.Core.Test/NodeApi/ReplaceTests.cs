﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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
using M1;

[TestClass]
public class ReplaceTests_Containment
{
    [TestMethod]
    public void Beginning()
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
        circle.ReplaceWith(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.IsNull(circle.GetParent());

        CollectionAssert.AreEqual(new List<IShape> { line, offsetDuplicate }, geometry.Shapes.ToList());
    }

    [TestMethod]
    public void Middle()
    {
        var circle = new Circle("circ0");
        var offsetDuplicate = new OffsetDuplicate("off0");
        var composite = new CompositeShape("comp0");

        var geometry = new Geometry("geom")
        {
            Shapes =
            [
                circle,
                offsetDuplicate,
                composite
            ]
        };
        var line = new Line("line");
        offsetDuplicate.ReplaceWith(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.IsNull(offsetDuplicate.GetParent());

        CollectionAssert.AreEqual(new List<IShape> { circle, line, composite }, geometry.Shapes.ToList());
    }

    [TestMethod]
    public void NoParent()
    {
        var circle = new Circle("circ0");
        var line = new Line("line");

        Assert.ThrowsException<TreeShapeException>(() => circle.ReplaceWith(line));
    }

    [TestMethod]
    public void SingleContainment()
    {
        var coord = new Coord("coord0");
        var circle = new Circle("circ0") { Center = coord };

        var newCoord = new Coord("coord1");
        coord.ReplaceWith(newCoord);

        Assert.AreEqual(circle, newCoord.GetParent());
        Assert.IsNull(coord.GetParent());
        Assert.AreEqual(newCoord, circle.Center);
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
        Assert.ThrowsException<InvalidValueException>(() => circle.ReplaceWith(coord));
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
        Assert.ThrowsException<InvalidValueException>(() => circle.ReplaceWith((INode)null));
    }
}

[TestClass]
public class ReplaceTests_Annotation
{
    [TestMethod]
    public void Beginning()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");

        var shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        var ann = new Documentation("line");
        doc.ReplaceWith(ann);

        Assert.AreEqual(shape, ann.GetParent());
        Assert.IsNull(doc.GetParent());

        CollectionAssert.AreEqual(new List<INode> { ann, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Middle()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        var bom2 = new BillOfMaterials("comp0");

        var shape = new Circle("geom");
        shape.AddAnnotations([doc, bom, bom2]);
        var ann = new Documentation("line");
        bom.ReplaceWith(ann);

        Assert.AreEqual(shape, ann.GetParent());
        Assert.IsNull(bom.GetParent());

        CollectionAssert.AreEqual(new List<INode> { doc, ann, bom2 }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void NoParent()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("line");

        Assert.ThrowsException<TreeShapeException>(() => doc.ReplaceWith(bom));
    }

    [TestMethod]
    public void NonFittingType()
    {
        var doc = new Documentation("circ0");

        var shape = new Line("geom");
        shape.AddAnnotations([doc]);
        var coord = new Coord("coord");
        Assert.ThrowsException<InvalidValueException>(() => doc.ReplaceWith(coord));
    }

    [TestMethod]
    public void Null()
    {
        var doc = new Documentation("circ0");

        var shape = new Line("geom");
        shape.AddAnnotations([doc]);
        Assert.ThrowsException<InvalidValueException>(() => doc.ReplaceWith((INode)null));
    }
}