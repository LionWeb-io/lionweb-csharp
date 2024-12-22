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

using Core.M2;
using Core.Utilities;
using Examples.Shapes.Dynamic;
using Examples.V2024_1.Shapes.M2;

[TestClass]
public class MultiRefClonerTests : ClonerTestsBase
{
    [TestMethod]
    public void IncludedBefore()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().CreateCircle();
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineB.Name = "LineB";

        ReferenceGeometry geometry = ShapesLanguage.Instance.GetFactory().CreateReferenceGeometry();
        geometry.AddShapes([lineA, circle, lineB]);

        var actual = Cloner.Clone([lineA, circle, lineB, geometry]).ToList();

        Assert.IsInstanceOfType<ReferenceGeometry>(actual.Last());
        var actualGeometry = actual.Last() as ReferenceGeometry;
        Assert.AreNotSame(geometry, actualGeometry);
        Assert.AreNotEqual(geometry.GetId(), actualGeometry.GetId());
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
    public void IncludedMixed()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().CreateCircle();
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineB.Name = "LineB";

        ReferenceGeometry geometry = ShapesLanguage.Instance.GetFactory().CreateReferenceGeometry();
        geometry.AddShapes([lineA, circle, lineB]);

        var actual = Cloner.Clone([lineA, geometry, circle, lineB]).ToList();

        Assert.IsInstanceOfType<ReferenceGeometry>(actual[1]);
        var actualGeometry = actual[1] as ReferenceGeometry;
        Assert.AreNotSame(geometry, actualGeometry);
        Assert.AreNotEqual(geometry.GetId(), actualGeometry.GetId());
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
    public void IncludedMixedExternal()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().CreateCircle();
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineB.Name = "LineB";

        ReferenceGeometry geometry = ShapesLanguage.Instance.GetFactory().CreateReferenceGeometry();
        geometry.AddShapes([lineA, circle, lineB]);

        var cloner = new Cloner([lineA, geometry, lineB]);
        cloner.KeepExternalReferences = true;
        var actual = cloner.Clone().Values.ToList();
        Assert.AreEqual(3, actual.Count);

        Assert.IsInstanceOfType<ReferenceGeometry>(actual[1]);
        var actualGeometry = actual[1] as ReferenceGeometry;
        Assert.AreNotSame(geometry, actualGeometry);
        Assert.AreNotEqual(geometry.GetId(), actualGeometry.GetId());
        Assert.AreEqual(3, actualGeometry.Shapes.Count);

        Assert.IsInstanceOfType<Line>(actualGeometry.Shapes[0]);
        var actualLineA = actualGeometry.Shapes[0] as Line;
        Assert.AreNotSame(lineA, actualLineA);
        Assert.AreNotEqual(lineA.GetId(), actualLineA.GetId());
        Assert.AreEqual("LineA", actualLineA.Name);

        Assert.IsInstanceOfType<Circle>(actualGeometry.Shapes[1]);
        var actualCircle = actualGeometry.Shapes[1] as Circle;
        Assert.AreSame(circle, actualCircle);
        Assert.AreEqual(circle.GetId(), actualCircle.GetId());

        Assert.IsInstanceOfType<Line>(actualGeometry.Shapes[2]);
        var actualLineB = actualGeometry.Shapes[2] as Line;
        Assert.AreNotSame(lineB, actualLineB);
        Assert.AreNotEqual(lineB.GetId(), actualLineB.GetId());
        Assert.AreEqual("LineB", actualLineB.Name);
    }

    [TestMethod]
    public void IncludedMixedNoExternal()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().CreateCircle();
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineB.Name = "LineB";

        ReferenceGeometry geometry = ShapesLanguage.Instance.GetFactory().CreateReferenceGeometry();
        geometry.AddShapes([lineA, circle, lineB]);

        var cloner = new Cloner([lineA, geometry, lineB]);
        cloner.KeepExternalReferences = false;
        var actual = cloner.Clone().Values.ToList();
        Assert.AreEqual(3, actual.Count);

        Assert.IsInstanceOfType<ReferenceGeometry>(actual[1]);
        var actualGeometry = actual[1] as ReferenceGeometry;
        Assert.AreNotSame(geometry, actualGeometry);
        Assert.AreNotEqual(geometry.GetId(), actualGeometry.GetId());
        Assert.AreEqual(2, actualGeometry.Shapes.Count);

        Assert.IsInstanceOfType<Line>(actualGeometry.Shapes[0]);
        var actualLineA = actualGeometry.Shapes[0] as Line;
        Assert.AreNotSame(lineA, actualLineA);
        Assert.AreNotEqual(lineA.GetId(), actualLineA.GetId());
        Assert.AreEqual("LineA", actualLineA.Name);

        Assert.IsInstanceOfType<Line>(actualGeometry.Shapes[1]);
        var actualLineB = actualGeometry.Shapes[1] as Line;
        Assert.AreNotSame(lineB, actualLineB);
        Assert.AreNotEqual(lineB.GetId(), actualLineB.GetId());
        Assert.AreEqual("LineB", actualLineB.Name);
    }


    [TestMethod]
    public void NullMultiReference()
    {
        var referenceGeometry = new BadReferenceGeometry(IdUtils.NewId());
        referenceGeometry.AddShapes([]);

        Assert.IsFalse(referenceGeometry.CollectAllSetFeatures().Contains(ShapesDynamic.Language
            .ClassifierByKey("key-ReferenceGeometry").FeatureByKey("key-shapes-references")));

        var result = new Cloner([referenceGeometry]) { IncludingReferences = true }.Clone().Values.First();

        Assert.IsInstanceOfType<ReferenceGeometry>(result);
    }
}