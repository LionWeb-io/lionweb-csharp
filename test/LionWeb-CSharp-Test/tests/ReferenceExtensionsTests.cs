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

// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591

namespace LionWeb_CSharp_Test.tests;

using Examples.Shapes.Dynamic;
using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.Utilities;

[TestClass]
public class ReferenceExtensionsTests
{
    [TestMethod]
    public void can_find_a_reference_from_a_feature_of_a_concept()
    {
        var language = ShapesLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateReferenceGeometry();

        var geometry = (ExampleModels.ExampleModel(language) as Geometry)!;
        referenceGeometry.AddShapes(geometry.Shapes);

        List<INode> rootNodes = [geometry, referenceGeometry];
        var refs = ReferenceExtensions.FindIncomingReferences(geometry.Shapes[0], rootNodes).ToList();
        Assert.AreEqual(1, refs.Count());
        var ref0 = refs.First();

        var expectedReferenceValue =
            new ReferenceValue(referenceGeometry, language.ReferenceGeometry_shapes, 0, geometry.Shapes[0]);
        Assert.AreEqual(expectedReferenceValue, ref0);  // (relies on value equality of C# record types)

        var allRefs = ReferenceExtensions.ReferenceValues(rootNodes).ToList();
        Assert.AreEqual(1, allRefs.Count());
        Assert.AreEqual(expectedReferenceValue, allRefs.First());
    }

    [TestMethod]
    public void can_find_a_reference_from_an_annotation()
    {
        var language = ShapesLanguage.Instance;
        var factory = language.GetFactory();

        var circle = factory.CreateCircle();

        var line = (ExampleModels.ExampleLine(language) as Line)!;
        var bom = factory.CreateBillOfMaterials();
        bom.AddMaterials([circle]);
        line.AddAnnotations([bom]);

        var geometry = factory.CreateGeometry();
        geometry.AddShapes([circle, line]);

        var refs = ReferenceExtensions.FindIncomingReferences(circle, [bom]).ToList();
        Assert.AreEqual(1, refs.Count());
        var ref0 = refs.First();

        var expectedReferenceValue =
            new ReferenceValue(bom, language.BillOfMaterials_materials, 0, circle);
        Assert.AreEqual(expectedReferenceValue, ref0);  // (relies on value equality of C# record types)
    }
}