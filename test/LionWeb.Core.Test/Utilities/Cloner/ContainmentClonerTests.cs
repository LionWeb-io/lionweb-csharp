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

namespace LionWeb.Core.Test.Utilities.Cloner;

using Core.Utilities;
using Languages;
using Languages.Generated.V2024_1.Shapes.M2;
using M2;

[TestClass]
public class ContainmentClonerTests : ClonerTestsBase
{
    [TestMethod]
    public void test_deep()
    {
        CompositeShape topShape = ShapesLanguage.Instance.GetFactory().CreateCompositeShape();
        topShape.Name = "Top";

        Line middleLine = ShapesLanguage.Instance.GetFactory().CreateLine();
        middleLine.Name = "MiddleLine";
        CompositeShape middleShape = ShapesLanguage.Instance.GetFactory().CreateCompositeShape();
        middleShape.Name = "Middle";
        topShape.AddParts([middleLine, middleShape]);

        CompositeShape bottomShape = ShapesLanguage.Instance.GetFactory().CreateCompositeShape();
        bottomShape.Name = "Bottom";
        Line bottomLine = ShapesLanguage.Instance.GetFactory().CreateLine();
        bottomLine.Name = "BottomLine";
        middleShape.AddParts([bottomShape, bottomLine]);

        OffsetDuplicate duplicate = ShapesLanguage.Instance.GetFactory().CreateOffsetDuplicate();
        duplicate.Name = "Duplicate";
        duplicate.Source = middleLine;
        bottomShape.AddParts([duplicate]);


        var actual = Cloner.Clone([topShape]).ToList();
        Assert.AreEqual(1, actual.Count);

        Assert.IsInstanceOfType<CompositeShape>(actual.First());
        var actualTopShape = actual.First() as CompositeShape;
        Assert.AreNotSame(topShape, actualTopShape);
        Assert.AreEqual("Top", actualTopShape.Name);

        Assert.AreEqual(2, actualTopShape.Parts.Count);

        Assert.IsInstanceOfType<Line>(actualTopShape.Parts.First());
        var actualMiddleLine = actualTopShape.Parts.First() as Line;
        Assert.AreNotSame(middleLine, actualMiddleLine);
        Assert.AreSame(actualTopShape, actualMiddleLine.GetParent());
        Assert.AreEqual("MiddleLine", actualMiddleLine.Name);

        Assert.IsInstanceOfType<CompositeShape>(actualTopShape.Parts.Last());
        var actualMiddleShape = actualTopShape.Parts.Last() as CompositeShape;
        Assert.AreNotSame(middleShape, actualMiddleShape);
        Assert.AreSame(actualTopShape, actualMiddleShape.GetParent());
        Assert.AreEqual("Middle", actualMiddleShape.Name);

        Assert.AreEqual(2, actualMiddleShape.Parts.Count);

        Assert.IsInstanceOfType<CompositeShape>(actualMiddleShape.Parts.First());
        var actualBottomShape = actualMiddleShape.Parts.First() as CompositeShape;
        Assert.AreNotSame(bottomShape, actualBottomShape);
        Assert.AreSame(actualMiddleShape, actualBottomShape.GetParent());
        Assert.AreEqual("Bottom", actualBottomShape.Name);

        Assert.IsInstanceOfType<Line>(actualMiddleShape.Parts.Last());
        var actualBottomLine = actualMiddleShape.Parts.Last() as Line;
        Assert.AreNotSame(bottomLine, actualBottomLine);
        Assert.AreSame(actualMiddleShape, actualBottomLine.GetParent());
        Assert.AreEqual("BottomLine", actualBottomLine.Name);

        Assert.AreEqual(1, actualBottomShape.Parts.Count);

        Assert.IsInstanceOfType<OffsetDuplicate>(actualBottomShape.Parts.First());
        var actualDuplicate = actualBottomShape.Parts.First() as OffsetDuplicate;
        Assert.AreNotSame(duplicate, actualDuplicate);
        Assert.AreSame(actualBottomShape, actualDuplicate.GetParent());
        Assert.AreEqual("Duplicate", actualDuplicate.Name);
        Assert.AreSame(actualMiddleLine, actualDuplicate.Source);
    }

    [TestMethod]
    public void test_SingleChild()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";
        Coord start = ShapesLanguage.Instance.GetFactory().CreateCoord();
        start.X = 1;
        start.Y = 0;
        start.Z = -3;
        line.Start = start;

        Line actual = Cloner.Clone(line);

        Assert.IsInstanceOfType<Line>(actual);
        Assert.AreNotSame(line, actual);
        Assert.AreNotEqual(line.GetId(), actual.GetId());
        var actualLine = actual as Line;
        Assert.AreEqual("MyLine", actualLine.Name);

        Assert.IsInstanceOfType<Coord>(actualLine.Start);
        var actualStart = actualLine.Start;
        Assert.AreNotSame(start, actualStart);
        Assert.AreNotEqual(start.GetId(), actualStart.GetId());
        Assert.AreEqual(1, actualStart.X);
        Assert.AreEqual(0, actualStart.Y);
        Assert.AreEqual(-3, actualStart.Z);

        Assert.ThrowsExactly<UnsetFeatureException>(() => actualLine.End);
    }

    [TestMethod]
    public void test_MultiChild()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().CreateCircle();
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineB.Name = "LineB";

        Geometry geometry = ShapesLanguage.Instance.GetFactory().CreateGeometry();
        geometry.AddShapes([lineA, circle, lineB]);

        Geometry actual = Cloner.Clone(geometry);

        Assert.IsInstanceOfType<Geometry>(actual);
        Assert.AreNotSame(geometry, actual);
        Assert.AreNotEqual(geometry.GetId(), actual.GetId());
        var actualGeometry = actual as Geometry;
        Assert.AreEqual(3, actualGeometry.Shapes.Count);

        Assert.IsInstanceOfType<Line>(actualGeometry.Shapes[0]);
        var actualLineA = actualGeometry.Shapes[0] as Line;
        Assert.AreNotSame(lineA, actualLineA);
        Assert.AreNotEqual(lineA.GetId(), actualLineA.GetId());
        Assert.AreEqual("LineA", actualLineA.Name);

        Assert.IsInstanceOfType<Circle>(actualGeometry.Shapes[1]);
        var actualCircle = actualGeometry.Shapes[1] as Circle;
        Assert.AreNotSame(circle, actualCircle);
        Assert.AreNotEqual(circle.GetId(), actualCircle.GetId());
        Assert.AreEqual("Circle", actualCircle.Name);

        Assert.IsInstanceOfType<Line>(actualGeometry.Shapes[2]);
        var actualLineB = actualGeometry.Shapes[2] as Line;
        Assert.AreNotSame(lineB, actualLineB);
        Assert.AreNotEqual(lineB.GetId(), actualLineB.GetId());
        Assert.AreEqual("LineB", actualLineB.Name);
    }
   
    
    [TestMethod]
    public void test_nullMultiContainment()
    {
        var geometry = new BadGeometry(IdUtils.NewId());
        geometry.AddShapes([]);

        Assert.IsFalse(geometry.CollectAllSetFeatures()
            .Contains(ShapesDynamic.Language.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes")));

        var result = new Cloner([geometry]).Clone().Values.First();

        Assert.IsInstanceOfType<Geometry>(result);
    }
}