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
using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class ClonerTests : ClonerTestsBase
{
    [TestMethod]
    public void Empty()
    {
        var actual = Cloner.Clone(Enumerable.Empty<INode>());
        Assert.IsTrue(!actual.Any());
    }

    [TestMethod]
    public void Single()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        Line actual = Cloner.Clone(line);

        Assert.IsInstanceOfType<Line>(actual);
        Assert.AreNotSame(line, actual);
        Assert.AreNotEqual(line.GetId(), actual.GetId());
        Assert.AreEqual("MyLine", (actual as Line).Name);
    }

    [TestMethod]
    public void Single_SeveralMentions()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        var actual = new Cloner([line, line, line]).Clone();

        Assert.AreEqual(1, actual.Count);

        Assert.IsInstanceOfType<Line>(actual.Values.First());
        var actualLine = actual.Values.First() as Line;
        Assert.AreNotSame(line, actualLine);
        Assert.AreNotEqual(line.GetId(), actualLine.GetId());
        Assert.AreEqual("MyLine", actualLine.Name);
    }

    [TestMethod]
    public void SingleAnnotation()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().CreateCircle();
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineB.Name = "LineB";

        var bom = ShapesLanguage.Instance.GetFactory().CreateBillOfMaterials();
        bom.AddMaterials([lineB, circle]);
        MaterialGroup materialGroup = ShapesLanguage.Instance.GetFactory().CreateMaterialGroup();
        materialGroup.AddMaterials([circle, lineB]);
        bom.AddGroups([materialGroup]);

        lineA.AddAnnotations([bom]);

        var actual = Cloner.Clone([lineA, lineB]).ToList();

        Assert.AreEqual(2, actual.Count);

        Assert.IsInstanceOfType<Line>(actual.First());
        var actualLineA = actual.First() as Line;

        Assert.AreEqual(1, actualLineA.GetAnnotations().Count);
        Assert.IsInstanceOfType<BillOfMaterials>(actualLineA.GetAnnotations().First());
        BillOfMaterials actualBom = actualLineA.GetAnnotations().First() as BillOfMaterials;
        Assert.AreNotSame(bom, actualBom);
        Assert.AreNotEqual(bom.GetId(), actualBom.GetId());

        Assert.AreEqual(2, actualBom.Materials.Count);

        Assert.IsInstanceOfType<Line>(actualBom.Materials.First());
        Line actualLineB = actualBom.Materials.First() as Line;
        Assert.AreNotSame(lineB, actualLineB);
        Assert.AreNotEqual(lineB.GetId(), actualLineB.GetId());

        Assert.AreSame(circle, bom.Materials.Last());

        Assert.AreEqual(1, actualBom.Groups.Count);
        Assert.IsInstanceOfType<MaterialGroup>(actualBom.Groups.First());
        MaterialGroup actualMaterialGroup = actualBom.Groups.First() as MaterialGroup;
        Assert.AreNotSame(materialGroup, actualMaterialGroup);
        Assert.AreNotEqual(materialGroup.GetId(), actualMaterialGroup.GetId());

        Assert.AreEqual(2, actualMaterialGroup.Materials.Count);
        Assert.AreSame(circle, actualMaterialGroup.Materials.First());
        Assert.AreSame(actualLineB, actualMaterialGroup.Materials.Last());
    }

    [TestMethod]
    public void MultiAnnotation()
    {
        Line line = ShapesLanguage.Instance.GetFactory().CreateLine();
        line.Name = "MyLine";

        Documentation docA = ShapesLanguage.Instance.GetFactory().CreateDocumentation();
        docA.Text = "This is very interesting";

        BillOfMaterials bom = ShapesLanguage.Instance.GetFactory().CreateBillOfMaterials();

        Documentation docB = ShapesLanguage.Instance.GetFactory().CreateDocumentation();
        docB.Text = "Boring!";

        line.AddAnnotations([docA, bom, docB]);

        Line actual = Cloner.Clone(line);

        Assert.AreEqual(3, actual.GetAnnotations().Count);

        Assert.IsInstanceOfType<Documentation>(actual.GetAnnotations()[0]);
        Documentation actualDocA = actual.GetAnnotations()[0] as Documentation;
        Assert.AreNotSame(docA, actualDocA);
        Assert.AreNotEqual(docA.GetId(), actualDocA.GetId());
        Assert.AreEqual("This is very interesting", actualDocA.Text);

        Assert.IsInstanceOfType<BillOfMaterials>(actual.GetAnnotations()[1]);
        BillOfMaterials actualBom = actual.GetAnnotations()[1] as BillOfMaterials;
        Assert.AreNotSame(bom, actualBom);
        Assert.AreNotEqual(bom.GetId(), actualBom.GetId());

        Assert.IsInstanceOfType<Documentation>(actual.GetAnnotations()[2]);
        Documentation actualDocB = actual.GetAnnotations()[2] as Documentation;
        Assert.AreNotSame(docB, actualDocB);
        Assert.AreNotEqual(docB.GetId(), actualDocB.GetId());
        Assert.AreEqual("Boring!", actualDocB.Text);
    }
}