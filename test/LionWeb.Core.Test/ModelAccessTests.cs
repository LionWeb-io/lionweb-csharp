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

namespace LionWeb.Core.Test;

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class ModelAccessTests
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    
    [TestMethod]
    public void test_multi_valued_containment_access()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(ShapesLanguage.Instance) as Geometry;
        Assert.AreEqual(1, geometry.Shapes.Count);
        var shape = geometry.Shapes[0];
        Assert.IsInstanceOfType<Line>(shape);
        var line = shape as Line;
        Assert.AreEqual("line1", line.Name);
        Assert.AreEqual(geometry, line.GetParent());
        var start = line.Start;
        Assert.AreEqual(line, start.GetParent());
    }

    [TestMethod]
    public void test_factory_method_with_out_var()
    {
        var factory = ShapesLanguage.Instance.GetFactory();
        var line = new Line(IdUtils.NewId())
        {
            Name = "line1",
            Start = new Coord(IdUtils.NewId()) { X = -1, Y = -2, Z = -3 },
            End = new Coord(IdUtils.NewId()) { X = 1, Y = 2, Z = 3 }
        };
        var start = line.Start;
        var end = line.End;
        Assert.AreEqual("line1", line.Name);
        Assert.AreEqual(start, line.Start);
        Assert.IsInstanceOfType<Coord>(start);
        Assert.AreEqual(end, line.End);
        Assert.IsInstanceOfType<Coord>(end);
    }

    [TestMethod]
    public void test_nullable_features_can_be_set_to_null()
    {
        // no compiler warnings should be given on any of these lines:
        var documentation = ShapesLanguage.Instance.GetFactory().CreateDocumentation();
        documentation.SetText(null);
        Assert.IsNull(documentation.Text);
        var geometry = ShapesLanguage.Instance.GetFactory().CreateGeometry();
        geometry.SetDocumentation(null);
        Assert.IsNull(geometry.Documentation);
        geometry.Documentation = documentation;
        geometry.SetDocumentation(geometry.Documentation);
        Assert.IsNotNull(geometry.Documentation);
    }

    [TestMethod]
    public void test_AllChildren()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(ShapesLanguage.Instance) as Geometry;
        var allChildren = geometry.Children(false, true);
        Assert.AreEqual(1, allChildren.Count());
        Assert.AreEqual(geometry.Shapes[0], allChildren.ElementAt(0));
    }

    [TestMethod]
    public void test_GetThisAndAllChildren()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(ShapesLanguage.Instance) as Geometry;
        var thisAndAllChildren = geometry.Children(true, true);
        Assert.AreEqual(2, thisAndAllChildren.Count());
        Assert.AreEqual(geometry, thisAndAllChildren.ElementAt(0));
        Assert.AreEqual(geometry.Shapes[0], thisAndAllChildren.ElementAt(1));
    }

    [TestMethod]
    public void test_AllDescendants()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(ShapesLanguage.Instance) as Geometry;
        var allDescendants = geometry.Descendants(false, true);
        Assert.AreEqual(3, allDescendants.Count());
        Assert.IsFalse(allDescendants.Contains(geometry));
        var line = geometry.Shapes[0] as Line;
        Assert.AreEqual(line, allDescendants.ElementAt(0));
        Assert.AreEqual(line.End, allDescendants.ElementAt(1));
        Assert.AreEqual(line.Start, allDescendants.ElementAt(2));
    }

    [TestMethod]
    public void test_AllNodes()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(ShapesLanguage.Instance) as Geometry;
        var allNodes = geometry.Descendants(true, true);
        Assert.AreEqual(4, allNodes.Count());
        Assert.AreEqual(geometry, allNodes.ElementAt(0));
        var line = geometry.Shapes[0] as Line;
        Assert.AreEqual(line, allNodes.ElementAt(1));
        Assert.AreEqual(line.End, allNodes.ElementAt(2));
        Assert.AreEqual(line.Start, allNodes.ElementAt(3));
    }
}