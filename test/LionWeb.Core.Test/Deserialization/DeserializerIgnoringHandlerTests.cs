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

namespace LionWeb.Core.Test.Deserialization;

using Core.Serialization;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M3;

[TestClass]
public class DeserializerIgnoringHandlerTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void unknown_classifier()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnknownClassifier(
            CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), new CompressedIdConfig(KeepOriginal:true)),
            ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true))));
    }


    [TestMethod]
    public void duplicate_node_id()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().DuplicateNodeId(
            ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true)),
            new Line("line"),
            new Line("line")));
    }

    [TestMethod]
    public void unknown_feature()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnknownFeature<Containment>(
            CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), new CompressedIdConfig(KeepOriginal:true)),
            new DynamicConcept("concept", new DynamicLanguage("lang", _lionWebVersion)) { Name = "concept-name" },
            new Line("line")));
    }

    [TestMethod]
    public void invalid_feature()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().InvalidFeature<Containment>(
            CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), new CompressedIdConfig(KeepOriginal:true)),
            new DynamicConcept("concept", new DynamicLanguage("lang", _lionWebVersion)) { Name = "concept-name" },
            new Line("line")));
    }

    [TestMethod]
    public void invalid_link_value()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().InvalidLinkValue<Containment>(
            [],
            new DynamicReference("dyn-reference",
                new DynamicConcept("dyn-concept-1",
                    new DynamicLanguage("dyn-lang-1", _lionWebVersion) { Name = "lang-name" })
                {
                    Name = "concept-name"
                })
            {
                Name = "dynamic-reference",
                Type = new DynamicConcept("dyn-concept-2",
                    new DynamicLanguage("dyn-lang-2", _lionWebVersion) { Name = "lang-name" }) { Name = "concept-name" }
            },
            new Line("line")));
    }

    [TestMethod]
    public void unresolvable_child()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnresolvableChild(
            ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true)),
            new DynamicContainment("dyn-containment",
                new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion))),
            new Line("line")));
    }

    [TestMethod]
    public void unresolvable_reference_target()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnresolvableReferenceTarget(
            ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true)),
            "resolve-info",
            new DynamicReference("dyn-reference",
                new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion))),
            new Line("line")));
    }

    [TestMethod]
    public void unresolvable_annotation()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().UnresolvableAnnotation(
            ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true)),
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
        var concept = new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion));
        Assert.IsNull(new DeserializerIgnoringHandler().UnknownDatatype("a", concept,
            new DynamicProperty("dyn-property", concept), new Line("line")));
    }

    [TestMethod]
    public void invalid_property_value()
    {
        Assert.IsNull(new DeserializerIgnoringHandler().InvalidPropertyValue<int>(
            "a",
            new DynamicProperty("dyn-property",
                new DynamicConcept("dyn-concept", new DynamicLanguage("dyn-lang", _lionWebVersion))),
            ICompressedId.Create("b", new CompressedIdConfig(KeepOriginal:true))));
    }

    [TestMethod]
    public void skip_deserializing_dependent_node()
    {
        Assert.IsTrue(new DeserializerIgnoringHandler().SkipDeserializingDependentNode(ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true))));
    }
}