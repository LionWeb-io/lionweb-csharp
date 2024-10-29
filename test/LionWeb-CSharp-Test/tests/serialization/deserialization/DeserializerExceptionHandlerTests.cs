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
            new DeserializerExceptionHandler().UnknownClassifier("a",
                new MetaPointer("key-Shapes", "1", "key-Geometry")));
    }

    [TestMethod]
    public void unknown_feature()
    {
        Assert.ThrowsException<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().UnknownFeature<Containment>(
                new DynamicConcept("concept", new DynamicLanguage("lang")) { Name = "concept-name" },
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
                new Line("line")));
    }

    [TestMethod]
    public void unknown_feature_keepOriginal_is_false()
    {
        Assert.ThrowsException<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().UnknownFeature<Containment>(
                new DynamicConcept("concept", new DynamicLanguage("lang")) { Name = "concept-name" },
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), false),
                new Line("line")));
    }

    [TestMethod]
    public void invalid_feature()
    {
        Assert.ThrowsException<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().InvalidFeature<Containment>(
                new DynamicConcept("concept", new DynamicLanguage("lang")) { Name = "concept-name" },
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), true),
                new Line("line")));
    }

    [TestMethod]
    public void invalid_feature_keepOriginal_is_false()
    {
        Assert.ThrowsException<UnknownFeatureException>(() =>
            new DeserializerExceptionHandler().InvalidFeature<Containment>(
                new DynamicConcept("concept", new DynamicLanguage("lang")) { Name = "concept-name" },
                CompressedMetaPointer.Create(new MetaPointer("key-Shapes", "1", "key-Geometry"), false),
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
    public void unknown_parent()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownParent(CompressedId.Create("a", true), new Line("b")));
    }

    [TestMethod]
    public void unknown_child()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownChild(CompressedId.Create("a", true), new Line("b")));
    }

    [TestMethod]
    public void unknown_reference_target()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownReferenceTarget(
                CompressedId.Create("a", true), "resolve-info", new Line("b")));
    }

    [TestMethod]
    public void unknown_annotation()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownAnnotation(CompressedId.Create("a", true), new Line("b")));
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
            new DeserializerExceptionHandler().InvalidAnnotationParent(new Documentation("a"), "b"));
    }

    [TestMethod]
    public void unknown_enumeration_literal()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownEnumerationLiteral(
                "a", new DynamicEnumeration("b", new DynamicLanguage("c")), "key"));
    }

    [TestMethod]
    public void unknown_datatype()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownDatatype(
                "a",
                new DynamicContainment("b", new DynamicConcept("c", new DynamicLanguage("lang"))),
                "value"));
    }

    [TestMethod]
    public void skip_deserializing_dependent_node()
    {
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().SkipDeserializingDependentNode("a"));
    }
}