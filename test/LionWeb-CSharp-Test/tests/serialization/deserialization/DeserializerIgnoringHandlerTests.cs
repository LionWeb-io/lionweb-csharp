// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb_CSharp_Test.tests.serialization.deserialization;

using Examples.Shapes.M2;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

[TestClass]
public class DeserializerIgnoringHandlerTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void unknown_classifier()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnknownClassifier(
            CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
            CompressedId.Create("a", true)));
    }


    [TestMethod]
    public void duplicate_node_id()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().DuplicateNodeId(
            CompressedId.Create("a", true),
            new Line("line"),
            new Line("line")));
    }

    [TestMethod]
    public void unknown_feature()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnknownFeature<Containment>(
            CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
            new DynamicConcept("concept", new DynamicLanguage("lang", _lionWebVersion)) { Name = "concept-name" },
            new Line("line")));
    }

    [TestMethod]
    public void invalid_feature()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().InvalidFeature<Containment>(
            CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
            new DynamicConcept("concept", new DynamicLanguage("lang", _lionWebVersion)) { Name = "concept-name" },
            new Line("line")));
    }

    [TestMethod]
    public void invalid_link_value()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().InvalidLinkValue<Containment>(
            [],
            new DynamicReference("dyn-reference",
                new DynamicConcept("dyn-concept-1", new DynamicLanguage("dyn-lang-1", _lionWebVersion) { Name = "lang-name" })
                {
                    Name = "concept-name"
                })
            {
                Name = "dynamic-reference",
                Type = new DynamicConcept("dyn-concept-2", new DynamicLanguage("dyn-lang-2", _lionWebVersion) { Name = "lang-name" })
                {
                    Name = "concept-name"
                }
            },
            new Line("line")));
    }

    [TestMethod]
    public void unresolvable_child()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnresolvableChild(
            CompressedId.Create("a", true),
            new DynamicContainment("dyn-containment",
                new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion))),
            new Line("line")));
    }

    [TestMethod]
    public void unresolvable_reference_target()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnresolvableReferenceTarget(
            CompressedId.Create("a", true),
            "resolve-info",
            new DynamicReference("dyn-reference",
                new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion))),
            new Line("line")));
    }

    [TestMethod]
    public void unresolvable_annotation()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnresolvableAnnotation(
            CompressedId.Create("a", true),
            new Line("line")));
    }

    [TestMethod]
    public void invalid_annotation()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().InvalidAnnotation(new Documentation("doc"), new Line("line")));
    }

    [TestMethod]
    public void circular_containment()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().CircularContainment(
            new Line("line"),
            new Line("line")));
    }

    [TestMethod]
    public void duplicate_containment()
    {
        Assert.IsFalse(new DeserializerIgnoringHandler().DuplicateContainment(
            new Line("line"),
            new Line("line"),
            new Line("line")));
    }


    [TestMethod]
    public void unknown_enumeration_literal()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnknownEnumerationLiteral(
            "a",
            new DynamicEnumeration("dyn-enum", new DynamicLanguage("dyn-lang-1", _lionWebVersion)),
            new DynamicProperty("dyn-property",
                new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang-2", _lionWebVersion))),
            new Line("line")
        ));
    }

    [TestMethod]
    public void unknown_datatype()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnknownDatatype(
            new DynamicProperty("dyn-property", new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion))),
            "a",
            new Line("line")));
    }

    [TestMethod]
    public void invalid_property_value()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().InvalidPropertyValue<int>(
            "a",
            new DynamicProperty("dyn-property", new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion))),
            CompressedId.Create("b", true)));
    }

    [TestMethod]
    public void skip_deserializing_dependent_node()
    {
        Assert.IsTrue(new DeserializerIgnoringHandler().SkipDeserializingDependentNode(CompressedId.Create("a", true)));
    }
}