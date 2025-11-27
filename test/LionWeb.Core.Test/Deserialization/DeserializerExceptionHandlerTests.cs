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
using M2;
using M3;

[TestClass]
public class DeserializerExceptionHandlerTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void unknown_classifier()
    {
        Assert.ThrowsExactly<UnsupportedClassifierException>(() =>
            new DeserializerExceptionHandler().UnknownClassifier(
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), new CompressedIdConfig(true, false)),
                ICompressedId.Create("a", new CompressedIdConfig(true, true))));
    }

    [TestMethod]
    public void duplicate_node_id()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().DuplicateNodeId(
                ICompressedId.Create("a", new CompressedIdConfig(true, true)),
                new Line("line"),
                new Line("line")));
    }

    [TestMethod]
    public void unknown_feature()
    {
        Assert.ThrowsExactly<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().UnknownFeature<Containment>(
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), new CompressedIdConfig(KeepOriginal:true)),
                new DynamicConcept("concept", _lionWebVersion, new DynamicLanguage("lang", _lionWebVersion)) { Name = "concept-name" },
                new Line("line")));
    }

    [TestMethod]
    public void invalid_feature()
    {
        Assert.ThrowsExactly<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().InvalidFeature<Containment>(
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), new CompressedIdConfig(KeepOriginal:true)),
                new DynamicConcept("concept", _lionWebVersion, new DynamicLanguage("lang", _lionWebVersion)) { Name = "concept-name" },
                new Line("line")));
    }

    [TestMethod]
    public void invalid_link_value()
    {
        Assert.ThrowsExactly<InvalidValueException>(() =>
            new DeserializerExceptionHandler().InvalidLinkValue<Containment>(
                [],
                new DynamicReference("dyn-reference", _lionWebVersion, 
                    new DynamicConcept("dyn-concept-1", _lionWebVersion,
                        new DynamicLanguage("dyn-lang-1", _lionWebVersion) { Name = "lang-name" })
                    {
                        Name = "concept-name"
                    })
                {
                    Name = "dynamic-reference",
                    Type = new DynamicConcept("dyn-concept-2", _lionWebVersion,
                        new DynamicLanguage("dyn-lang-2", _lionWebVersion) { Name = "lang-name" })
                    {
                        Name = "concept-name"
                    }
                },
                new Line("line")));
    }

    [TestMethod]
    public void invalid_containment()
    {
        Assert.ThrowsExactly<UnsupportedClassifierException>(() =>
            new LanguageDeserializerExceptionHandler().InvalidContainment(new Line("line")));
    }

    [TestMethod]
    public void invalid_reference()
    {
        Assert.ThrowsExactly<UnsupportedClassifierException>(() =>
            new DeserializerExceptionHandler().InvalidReference(new Line("line")));
    }

    [TestMethod]
    public void unresolvable_child()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnresolvableChild(
                ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true)),
                new DynamicContainment("dyn-containment", _lionWebVersion,
                    new DynamicConcept("dyn-concept", _lionWebVersion, new DynamicLanguage("dyn-lang", _lionWebVersion))),
                new Line("line")));
    }

    [TestMethod]
    public void unresolvable_reference_target()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnresolvableReferenceTarget(
                new ReferenceTarget(null, "a", null),
                new DynamicReference("dyn-reference", _lionWebVersion,
                    new DynamicConcept("dyn-concept", _lionWebVersion, new DynamicLanguage("dyn-lang", _lionWebVersion))),
                new Line("line")));
    }

    [TestMethod]
    public void unresolvable_annotation()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnresolvableAnnotation(
                ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true)),
                new Line("line")));
    }

    [TestMethod]
    public void invalid_annotation()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().InvalidAnnotation(new Documentation("doc"), new Line("line")));
    }

    [TestMethod]
    public void circular_containment()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().CircularContainment(
                new Line("line"),
                new Line("line")));
    }

    [TestMethod]
    public void duplicate_containment()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().DuplicateContainment(
                new Line("line"),
                new Line("line"),
                new Line("line")));
    }

    [TestMethod]
    public void invalid_annotation_parent()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new LanguageDeserializerExceptionHandler().InvalidAnnotationParent(new Documentation("doc"), new Line("line")));
    }

    [TestMethod]
    public void unknown_enumeration_literal()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownEnumerationLiteral(
                "a",
                new DynamicEnumeration("dyn-enum", _lionWebVersion, new DynamicLanguage("dyn-lang-1", _lionWebVersion)),
                new DynamicProperty("dyn-property", _lionWebVersion,
                    new DynamicConcept("dyn-concept", _lionWebVersion, new DynamicLanguage("dyn-lang-2", _lionWebVersion))),
                new Line("line")
            ));
    }

    [TestMethod]
    public void unknown_datatype()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
        {
            var concept = new DynamicConcept("dyn-concept", _lionWebVersion, new DynamicLanguage("dyn-lang", _lionWebVersion));
            return new DeserializerExceptionHandler().UnknownDatatype("a", concept,
                new DynamicProperty("dyn-property", _lionWebVersion, concept),
                new Line("line"));
        });
    }

    [TestMethod]
    public void invalid_property_value()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().InvalidPropertyValue<int>(
                "a",
                new DynamicProperty("dyn-property", _lionWebVersion,
                    new DynamicConcept("dyn-concept", _lionWebVersion, new DynamicLanguage("dyn-lang", _lionWebVersion))),
                ICompressedId.Create("b", new CompressedIdConfig(KeepOriginal:true))));
    }

    [TestMethod]
    public void skip_deserializing_dependent_node()
    {
        Assert.ThrowsExactly<DeserializerException>(() =>
            new DeserializerExceptionHandler().SkipDeserializingDependentNode(ICompressedId.Create("a", new CompressedIdConfig(KeepOriginal:true))));
    }
}