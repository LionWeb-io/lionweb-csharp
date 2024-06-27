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

namespace LionWeb.Utils.Tests.Cloner;

using Core;
using Core.Utilities;
using Examples.Shapes.M2;

[TestClass]
public class ConfigClonerTests : ClonerTestsBase
{
    [TestMethod]
    public void test_noProperties()
    {
        Geometry geometry = CreateExampleGeometry(out Line line, out OffsetDuplicate duplicate, out Coord coord,
            out Documentation doc);

        var cloner = new Cloner([geometry, duplicate]);
        cloner.IncludingProperties = false;
        var actual = cloner.Clone();

        AssertExampleGeometry(geometry);

        Assert.AreEqual(5, actual.Count);

        Geometry actualGeometry = actual[geometry] as Geometry;

        Assert.IsInstanceOfType<Line>(actual[line]);
        Assert.AreNotSame(line, actual[line]);
        var actualLine = actual[line] as Line;
        Assert.AreSame(actualGeometry, actualLine.GetParent());

        Assert.IsInstanceOfType<OffsetDuplicate>(actual[duplicate]);
        Assert.AreNotSame(duplicate, actual[duplicate]);
        var actualDuplicate = actual[duplicate] as OffsetDuplicate;
        Assert.AreSame(actualGeometry, actualDuplicate.GetParent());
        Assert.AreSame(actualLine, actualDuplicate.Source);

        Assert.IsInstanceOfType<Coord>(actual[coord]);
        Assert.AreNotSame(coord, actual[coord]);
        var actualCoord = actual[coord] as Coord;
        Assert.AreSame(actualDuplicate, actualCoord.GetParent());
        Assert.AreSame(actualCoord, actualDuplicate.Offset);

        Assert.IsInstanceOfType<Documentation>(actual[doc]);
        Assert.AreNotSame(doc, actual[doc]);
        var actualDoc = actual[doc] as Documentation;
        Assert.AreSame(actualDuplicate, actualDoc.GetParent());
        Assert.IsNull(actualDoc.Text);
        Assert.AreSame(actualDoc, actualDuplicate.GetAnnotations().First());
    }

    [TestMethod]
    public void test_noChildren()
    {
        Geometry geometry = CreateExampleGeometry(out Line line, out OffsetDuplicate duplicate, out Coord _,
            out Documentation doc);

        var cloner = new Cloner([geometry, duplicate]);
        cloner.IncludingChildren = false;
        var actual = cloner.Clone();

        AssertExampleGeometry(geometry);

        Assert.AreEqual(3, actual.Count);

        Geometry actualGeometry = actual[geometry] as Geometry;

        Assert.IsInstanceOfType<OffsetDuplicate>(actual[duplicate]);
        Assert.AreNotSame(duplicate, actual[duplicate]);
        var actualDuplicate = actual[duplicate] as OffsetDuplicate;
        Assert.AreSame(actualGeometry, actualDuplicate.GetParent());
        Assert.AreEqual("Duplicate", actualDuplicate.Name);
        Assert.AreSame(line, actualDuplicate.Source);
        Assert.ThrowsException<UnsetFeatureException>(() => actualDuplicate.Offset);

        Assert.IsInstanceOfType<Documentation>(actual[doc]);
        Assert.AreNotSame(doc, actual[doc]);
        var actualDoc = actual[doc] as Documentation;
        Assert.AreSame(actualDuplicate, actualDoc.GetParent());
        Assert.AreEqual("Slightly moved around", actualDoc.Text);
        Assert.AreSame(actualDoc, actualDuplicate.GetAnnotations().First());
    }

    [TestMethod]
    public void test_noAnnotations()
    {
        Geometry geometry = CreateExampleGeometry(out Line line, out OffsetDuplicate duplicate, out Coord coord,
            out Documentation doc);

        var cloner = new Cloner([geometry, duplicate]);
        cloner.IncludingAnnotations = false;
        var actual = cloner.Clone();

        AssertExampleGeometry(geometry);

        Assert.AreEqual(4, actual.Count);

        Geometry actualGeometry = actual[geometry] as Geometry;

        Assert.IsInstanceOfType<Line>(actual[line]);
        Assert.AreNotSame(line, actual[line]);
        var actualLine = actual[line] as Line;
        Assert.AreSame(actualGeometry, actualLine.GetParent());
        Assert.AreEqual("MyLine", actualLine.Name);

        Assert.IsInstanceOfType<OffsetDuplicate>(actual[duplicate]);
        Assert.AreNotSame(duplicate, actual[duplicate]);
        var actualDuplicate = actual[duplicate] as OffsetDuplicate;
        Assert.AreSame(actualGeometry, actualDuplicate.GetParent());
        Assert.AreEqual("Duplicate", actualDuplicate.Name);
        Assert.AreSame(actualLine, actualDuplicate.Source);

        Assert.IsInstanceOfType<Coord>(actual[coord]);
        Assert.AreNotSame(coord, actual[coord]);
        var actualCoord = actual[coord] as Coord;
        Assert.AreSame(actualDuplicate, actualCoord.GetParent());
        Assert.AreEqual(-3, actualCoord.X);
        Assert.AreEqual(0, actualCoord.Y);
        Assert.AreEqual(3, actualCoord.Z);
        Assert.AreSame(actualCoord, actualDuplicate.Offset);

        Assert.AreEqual(0, actualDuplicate.GetAnnotations().Count);
    }

    [TestMethod]
    public void test_withReferences()
    {
        Geometry geometry = CreateExampleGeometry(out Line line, out OffsetDuplicate duplicate, out Coord coord,
            out Documentation doc);

        var cloner = new Cloner([duplicate]);
        cloner.IncludingReferences = true;
        var actual = cloner.Clone();

        AssertExampleGeometry(geometry);

        Assert.AreEqual(4, actual.Count);

        Assert.IsInstanceOfType<Line>(actual[line]);
        Assert.AreNotSame(line, actual[line]);
        var actualLine = actual[line] as Line;
        Assert.IsNull(actualLine.GetParent());
        Assert.AreEqual("MyLine", actualLine.Name);

        Assert.IsInstanceOfType<OffsetDuplicate>(actual[duplicate]);
        Assert.AreNotSame(duplicate, actual[duplicate]);
        var actualDuplicate = actual[duplicate] as OffsetDuplicate;
        Assert.IsNull(actualDuplicate.GetParent());
        Assert.AreEqual("Duplicate", actualDuplicate.Name);
        Assert.AreSame(actualLine, actualDuplicate.Source);

        Assert.IsInstanceOfType<Coord>(actual[coord]);
        Assert.AreNotSame(coord, actual[coord]);
        var actualCoord = actual[coord] as Coord;
        Assert.AreSame(actualDuplicate, actualCoord.GetParent());
        Assert.AreEqual(-3, actualCoord.X);
        Assert.AreEqual(0, actualCoord.Y);
        Assert.AreEqual(3, actualCoord.Z);
        Assert.AreSame(actualCoord, actualDuplicate.Offset);

        Assert.IsInstanceOfType<Documentation>(actual[doc]);
        Assert.AreNotSame(doc, actual[doc]);
        var actualDoc = actual[doc] as Documentation;
        Assert.AreSame(actualDuplicate, actualDoc.GetParent());
        Assert.AreEqual("Slightly moved around", actualDoc.Text);
        Assert.AreSame(actualDoc, actualDuplicate.GetAnnotations().First());
    }

    [TestMethod]
    public void test_withParents()
    {
        Geometry geometry = CreateExampleGeometry(out Line line, out OffsetDuplicate duplicate, out Coord coord,
            out Documentation doc);

        var cloner = new Cloner([coord]);
        cloner.IncludingParents = true;
        var actual = cloner.Clone();

        AssertExampleGeometry(geometry);

        Assert.AreEqual(5, actual.Count);

        Geometry actualGeometry = actual[geometry] as Geometry;

        Assert.IsInstanceOfType<Line>(actual[line]);
        Assert.AreNotSame(line, actual[line]);
        var actualLine = actual[line] as Line;
        Assert.AreSame(actualGeometry, actualLine.GetParent());
        Assert.AreEqual("MyLine", actualLine.Name);

        Assert.IsInstanceOfType<OffsetDuplicate>(actual[duplicate]);
        Assert.AreNotSame(duplicate, actual[duplicate]);
        var actualDuplicate = actual[duplicate] as OffsetDuplicate;
        Assert.AreSame(actualGeometry, actualDuplicate.GetParent());
        Assert.AreEqual("Duplicate", actualDuplicate.Name);
        Assert.AreSame(actualLine, actualDuplicate.Source);

        Assert.IsInstanceOfType<Coord>(actual[coord]);
        Assert.AreNotSame(coord, actual[coord]);
        var actualCoord = actual[coord] as Coord;
        Assert.AreSame(actualDuplicate, actualCoord.GetParent());
        Assert.AreEqual(-3, actualCoord.X);
        Assert.AreEqual(0, actualCoord.Y);
        Assert.AreEqual(3, actualCoord.Z);
        Assert.AreSame(actualCoord, actualDuplicate.Offset);

        Assert.IsInstanceOfType<Documentation>(actual[doc]);
        Assert.AreNotSame(doc, actual[doc]);
        var actualDoc = actual[doc] as Documentation;
        Assert.AreSame(actualDuplicate, actualDoc.GetParent());
        Assert.AreEqual("Slightly moved around", actualDoc.Text);
        Assert.AreSame(actualDoc, actualDuplicate.GetAnnotations().First());
    }
}