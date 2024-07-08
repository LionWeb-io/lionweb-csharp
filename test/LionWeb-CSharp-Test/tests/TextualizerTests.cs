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

namespace LionWeb.Utilities.Test;

using Core.Utilities;
using Examples.Shapes.Dynamic;
using Examples.Shapes.M2;

[TestClass]
public class TextualizerTests
{
    [TestMethod]
    public void smoke_test_textualizer()
    {
        var model = ExampleModels.ExampleModel(ShapesDynamic.Language);
        Console.WriteLine(model.AsString());
    }

    [TestMethod]
    public void does_not_crash_on_reference_to_a_node_with_an_unset_name()
    {
        var factory = ShapesLanguage.Instance.GetFactory();
        var compositeShape = factory.NewCompositeShape("foo");
        var referenceGeometry = factory.NewReferenceGeometry("bar").AddShapes([compositeShape]);

        Assert.AreEqual("""
                        CompositeShape (id: foo) {
                        }
                        """, compositeShape.AsString());
        Assert.AreEqual("""
                        ReferenceGeometry (id: bar) {
                            shapes -> foo <no name set!>
                        }
                        """, referenceGeometry.AsString());

        compositeShape.Name = "MyCompositeShape";
        Assert.AreEqual("""
                        CompositeShape (id: foo) {
                            name = "MyCompositeShape"
                        }
                        """, compositeShape.AsString());
        Assert.AreEqual("""
                        ReferenceGeometry (id: bar) {
                            shapes -> foo (MyCompositeShape)
                        }
                        """, referenceGeometry.AsString());
    }
}