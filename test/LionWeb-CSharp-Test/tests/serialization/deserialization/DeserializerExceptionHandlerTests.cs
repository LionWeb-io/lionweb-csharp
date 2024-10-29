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
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

[TestClass]
public class DeserializerExceptionHandlerTests
{
    [TestMethod]
    public void unknown_classifier()
    {
        Assert.ThrowsException<UnsupportedClassifierException>(() =>
            new DeserializerExceptionHandler().UnknownClassifier(
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
                CompressedId.Create("a", true)));
    }

    [TestMethod]
    public void unknown_feature()
    {
        Assert.ThrowsException<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().UnknownFeature<Containment>(
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
                new DynamicConcept("concept", new DynamicLanguage("lang")) { Name = "concept-name" },
                new Line("line")));
    }

    [TestMethod]
    public void invalid_feature()
    {
        Assert.ThrowsException<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().InvalidFeature<Containment>(
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
                new DynamicConcept("concept", new DynamicLanguage("lang")) { Name = "concept-name" },
                new Line("line")));
    }

    [TestMethod]
    public void invalid_containment()
    {
        Assert.ThrowsException<UnsupportedClassifierException>(() =>
            new DeserializerExceptionHandler().InvalidContainment(new Line("a")));
    }

    [TestMethod]
    public void invalid_reference()
    {
        Assert.ThrowsException<UnsupportedClassifierException>(() =>
            new DeserializerExceptionHandler().InvalidReference(new Line("a")));
    }

    [TestMethod]
    public void unresolvable_parent()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnresolvableParent(
                CompressedId.Create("a", true),
                new Line("b")));
    }

    [TestMethod]
    public void unresolvable_child()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnresolvableChild(
                CompressedId.Create("a", true),
                new DynamicContainment("b", new DynamicConcept("c", new DynamicLanguage("lang"))),
                new Line("d")));
    }

    [TestMethod]
    public void unresolvable_reference_target()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnresolvableReferenceTarget(
                CompressedId.Create("a", true),
                "resolve-info",
                new DynamicReference("b", new DynamicConcept("c", new DynamicLanguage("lang"))),
                new Line("d")));
    }

    [TestMethod]
    public void unresolvable_annotation()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnresolvableAnnotation(
                CompressedId.Create("a", true),
                new Line("b")));
    }

    [TestMethod]
    public void invalid_annotation()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().InvalidAnnotation(new Documentation("a"), new Line("b")));
    }

    [TestMethod]
    public void invalid_annotation_parent()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().InvalidAnnotationParent(new Documentation("a"), new Line("b")));
    }

    [TestMethod]
    public void unknown_enumeration_literal()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownEnumerationLiteral(
                "a",
                new DynamicEnumeration("b", new DynamicLanguage("c")),
                new DynamicProperty("d", new DynamicConcept("e", new DynamicLanguage("lang"))),
                new Line("f")
            ));
    }

    [TestMethod]
    public void unknown_datatype()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownDatatype(
                new DynamicProperty("a", new DynamicConcept("b", new DynamicLanguage("lang"))),
                "c",
                new Line("d")));
    }

    [TestMethod]
    public void invalid_property_value()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().InvalidPropertyValue<int>(
                "a",
                new DynamicProperty("b", new DynamicConcept("c", new DynamicLanguage("lang"))),
                CompressedId.Create("d", true)));
    }

    [TestMethod]
    public void skip_deserializing_dependent_node()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().SkipDeserializingDependentNode(CompressedId.Create("a", true)));
    }
}