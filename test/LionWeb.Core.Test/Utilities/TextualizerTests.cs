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

namespace LionWeb.Core.Test.Utilities;

using Core.Utilities;
using Languages;
using Languages.Generated.V2024_1.Shapes.M2;
using M2;
using M3;

[TestClass]
public class TextualizerTests
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private static readonly IBuiltInsLanguage _builtIns = _lionWebVersion.BuiltIns;

    [TestMethod]
    public void smoke_test_textualizer()
    {
        var model = new ExampleModels(_lionWebVersion).ExampleModel(ShapesDynamic.Language);
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

    [TestMethod]
    public void does_not_crash_on_features_with_unset_names()
    {
        var language = new DynamicLanguage("lang", _lionWebVersion) { Key = "key-lang", Version = "0" };

        var concept = new DynamicConcept("concept", language) { Key = "key-concept" };
        language.AddEntities([concept]);

        // shows that INamed_name is declared as a set feature, but its getter still throws:
        Assert.IsTrue(concept.CollectAllSetFeatures().Contains(_builtIns.INamed_name));
        Assert.ThrowsException<UnsetFeatureException>(() => concept.Name);

        var property = new DynamicProperty("prop", concept) { Key = "key-prop" };
        property.Type = _builtIns.String;
        var containment = new DynamicContainment("cont", concept) { Key = "key-cont" };
        containment.Type = concept;
        var reference = new DynamicReference("ref", concept) { Key = "key-ref" };
        reference.Type = concept;
        concept.AddFeatures([property, containment, reference]);

        var instance1 = new DynamicNode("instance1", concept);
        var instance2 = new DynamicNode("instance2", concept);

        instance1.Set(property, "val-prop");
        instance1.Set(containment, instance2);
        instance1.Set(reference, instance1); // (self-ref.)

        Assert.AreEqual("""
                        <no name set!> (id: instance1) {
                            <no name set!> = "val-prop"
                            <no name set!> -> instance1
                            <no name set!>:
                                <no name set!> (id: instance2) {
                                }
                        }
                        """, instance1.AsString());
    }
}