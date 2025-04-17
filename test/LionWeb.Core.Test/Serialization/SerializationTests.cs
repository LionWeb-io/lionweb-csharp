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

namespace LionWeb.Core.Test.Serialization;

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.Mixed.MixedConceptLang;
using Languages.Generated.V2024_1.Mixed.MixedDirectEnumLang;
using Languages.Generated.V2024_1.Mixed.MixedDirectSdtLang;
using Languages.Generated.V2024_1.Mixed.MixedNestedEnumLang;
using Languages.Generated.V2024_1.Mixed.MixedNestedSdtLang;
using Languages.Generated.V2024_1.SDTLang;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M2;
using M3;
using System.Collections;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class SerializationTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private readonly Language _language;

    public SerializationTests()
    {
        _language = ShapesLanguage.Instance;
    }

    [TestMethod]
    public void test_serialization_shapes_model()
    {
        INode rootNode = new ExampleModels(_lionWebVersion).ExampleModel(_language);

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([rootNode]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        // Just run the deserializer for now (without really checking anything), to see whether it crashes or not:
        new DeserializerBuilder()
            .WithLanguage(_language)
            .Build()
            .Deserialize(serializationChunk);
    }

    [TestMethod]
    public void test_no_double_serialization()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(_language);
        var shape0 = (geometry.Get(_language.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes")) as IEnumerable)
            .Cast<INode>().First();

        Assert.IsInstanceOfType<INode>(shape0);
        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([geometry, shape0]);
        Assert.AreEqual(4, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_optional_string_property_serialization()
    {
        var documentation = ((ShapesFactory)_language.GetFactory()).CreateDocumentation();
        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([documentation]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        var serializedProperty = serializationChunk.Nodes[0].Properties.First(p => p.Property.Key == "key-text");
        Assert.IsNull(serializedProperty.Value);
        Assert.AreEqual("key-text", serializedProperty.Property.Key);
    }

    [TestMethod]
    public void SerializeRefToUnsetName()
    {
        var line = new Line("line") { Start = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        var refGeo = new ReferenceGeometry("ref") { Shapes = [line] };

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([line, refGeo]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        var comparer = new Comparer([line, refGeo], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void SerializeUnsetRequiredContainment()
    {
        var compositeShape = new CompositeShape("comp");

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([compositeShape]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        var comparer = new Comparer([compositeShape], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void SerializeUnsetRequiredReference()
    {
        var materialGroup = new MaterialGroup("goup");

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([materialGroup]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        var comparer = new Comparer([materialGroup], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void DuplicateId()
    {
        var materialGroup = new MaterialGroup("duplicate") { DefaultShape = new Circle("duplicate") };

        Assert.ThrowsException<SerializerException>(() =>
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().SerializeToChunk([materialGroup]));
    }

    [TestMethod]
    [Ignore]
    public void DuplicateNode()
    {
        var b = new Circle("b");
        var a = new MaterialGroup("a") { DefaultShape = b };
        var b2 = new Circle("b");

        var x = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().Serialize([a, b, b]).ToList();
        var serializedNodes = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().Serialize([a, b, b2])
            .ToList();
        Assert.AreEqual(2, serializedNodes.Count);
    }

    [TestMethod]
    public void DuplicateUsedLanguage()
    {
        var lang = new DynamicLanguage("abc", _lionWebVersion)
        {
            Key = ShapesLanguage.Instance.Key, Version = ShapesLanguage.Instance.Version
        };
        var materialGroup = lang.Concept("efg", ShapesLanguage.Instance.MaterialGroup.Key,
            ShapesLanguage.Instance.MaterialGroup.Name);
        var defaultShape = materialGroup.Containment("ijk", ShapesLanguage.Instance.MaterialGroup_defaultShape.Key,
            ShapesLanguage.Instance.MaterialGroup_defaultShape.Name);

        var a = lang.GetFactory().CreateNode("a", materialGroup);
        var b = new Circle("b");
        a.Set(defaultShape, b);

        Assert.ThrowsException<SerializerException>(() =>
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().SerializeToChunk([a]));
    }

    [TestMethod]
    public void DuplicateUsedLanguage_DifferentVersion()
    {
        var lang = new DynamicLanguage("abc", _lionWebVersion)
        {
            Key = ShapesLanguage.Instance.Key, Version = ShapesLanguage.Instance.Version + "hello"
        };
        var materialGroup = lang.Concept("efg", ShapesLanguage.Instance.MaterialGroup.Key,
            ShapesLanguage.Instance.MaterialGroup.Name);
        var defaultShape = materialGroup.Containment("ijk", ShapesLanguage.Instance.MaterialGroup_defaultShape.Key,
            ShapesLanguage.Instance.MaterialGroup_defaultShape.Name);

        var a = lang.GetFactory().CreateNode("a", materialGroup);
        var b = new Circle("b");
        a.Set(defaultShape, b);

        var serializationChunk =
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().SerializeToChunk([a]);
        Assert.AreEqual(2, serializationChunk.Languages.Length);
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var materialGroup = new MaterialGroup("a") { DefaultShape = new Circle("b") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize(new SingleEnumerable<INode>(materialGroup.Descendants(true)));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.AreEqual(1, serializer.UsedLanguages.Count());
    }

    [TestMethod]
    public void SingleEnumerable_fail()
    {
        var materialGroup = new MaterialGroup("a") { DefaultShape = new Circle("b") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize(new SingleEnumerable<INode>(materialGroup.Descendants(true)));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.ThrowsException<AssertFailedException>(() => Assert.AreEqual(2, serializedNodes.Count()));
    }

    [TestMethod]
    public void NoUsedLanguagesBeforeSerialization()
    {
        var materialGroup = new MaterialGroup("a") { DefaultShape = new Circle("b") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        Assert.AreEqual(0, serializer.UsedLanguages.Count());
        var serializedNodes = serializer.Serialize(materialGroup.Descendants(true));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.AreEqual(1, serializer.UsedLanguages.Count());
    }

    [TestMethod]
    public void Utf8()
    {
        const string text = "\ud83d\ude0a Hällö 😊";
        var materialGroup = new MaterialGroup("a") { DefaultShape = new Circle("b") { Name = text } };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var stream = new MemoryStream();
        JsonUtils.WriteNodesToStream(stream, serializer, materialGroup.Descendants(true));

        stream.Seek(0, SeekOrigin.Begin);

        var deserializer = new DeserializerBuilder()
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        var readableNodes = JsonUtils.ReadNodesFromStreamAsync(stream, deserializer).Result;

        Assert.AreEqual(text,
            readableNodes.OfType<INode>().SelectMany(n => n.Descendants(true)).OfType<Circle>().First().Name);
    }

    [TestMethod]
    public void SerializeStructuredDataType()
    {
        var node = new SDTConcept("nodeId")
        {
            Amount =
                new Amount { Value = new Decimal { Int = 23, Frac = 42 }, Currency = Currency.EUR, Digital = true },
            Decimal = new Decimal { Int = 19 },
            Complex = new ComplexNumber { Real = new Decimal { Int = 1, Frac = 0 }, Imaginary = new Decimal() }
        };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize([node]).ToList();
        Assert.AreEqual(1, serializedNodes.Count);
        var serializedNode = serializedNodes.First();

        Assert.AreEqual(
            """{"key-SDTCurr":"key-SDTEur","key-SDTDigital":"true","key-SDTValue":{"key-SDTFrac":"42","key-SDTInt":"23"}}""",
            serializedNode.Properties.First(p => p.Property.Key == "key-SDTamountField").Value);

        Assert.AreEqual(
            """{"key-SDTInt":"19"}""",
            serializedNode.Properties.First(p => p.Property.Key == "key-SDTDecimalField").Value);

        Assert.AreEqual(
            """{"key-SDTImaginary":{},"key-SDTReal":{"key-SDTFrac":"0","key-SDTInt":"1"}}""",
            serializedNode.Properties.First(p => p.Property.Key == "key-SDTComplexField").Value);


        var nodes = new DeserializerBuilder()
            .WithLanguage(SDTLangLanguage.Instance)
            .WithLanguage(SDTLangLanguage.Instance)
            .Build()
            .Deserialize(serializedNodes);

        var comparer = new Comparer([node], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void SerializePropertyUsedLanguages()
    {
        var node = new MixedConcept("mixedId")
        {
            EnumProp = DirectEnum.directEnumA,
            SdtProp = new DirectSdt
            {
                DirectSdtEnum = NestedEnum.nestedLiteralA,
                DirectSdtSdt = new NestedSdt { NestedSdtField = "hello" }
            }
        };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize([node]).ToList();
        Assert.AreEqual(1, serializedNodes.Count);
        CollectionAssert.AreEquivalent(new List<SerializedLanguageReference>
        {
            new() { Key = "key-mixedBasePropertyLang", Version = "1" },
            new() { Key = "key-mixedBaseContainmentLang", Version = "1" },
            new() { Key = "key-mixedBaseReferenceLang", Version = "1" },
            new() { Key = "key-mixedBaseConceptLang", Version = "1" },
            new() { Key = "key-mixedConceptLang", Version = "1" },
            new() { Key = "key-mixedDirectEnumLang", Version = "1" },
            new() { Key = "key-mixedNestedEnumLang", Version = "1" },
            new() { Key = "key-mixedDirectSdtLang", Version = "1" },
            new() { Key = "key-mixedNestedSdtLang", Version = "1" },
        }, serializer.UsedLanguages.ToList());
    }

    [TestMethod]
    public void EmptyFeatures_Serialize()
    {
        var circle = new Circle("circle");
        var node = new OffsetDuplicate("od") { Uuid = "abc", Source = circle, Docs = new Documentation("docs") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).WithSerializedEmptyFeatures(true)
            .Build();

        var chunk = serializer.SerializeToChunk([node, circle]);

        var lang = ShapesLanguage.Instance;
        var langKey = lang.Key;
        var langVersion = lang.Version;
        var expected = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = langKey, Version = langVersion }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "od",
                    Classifier = lang.OffsetDuplicate.ToMetaPointer(),
                    Properties =
                    [
                        new SerializedProperty { Property = lang.IShape_uuid.ToMetaPointer(), Value = "abc" },
                        new SerializedProperty
                        {
                            Property = _lionWebVersion.BuiltIns.INamed_name.ToMetaPointer(), Value = null
                        },
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = lang.OffsetDuplicate_docs.ToMetaPointer(), Children = ["docs"]
                        },
                        new SerializedContainment
                        {
                            Containment = lang.IShape_fixpoints.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment
                        {
                            Containment = lang.OffsetDuplicate_secretDocs.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment
                        {
                            Containment = lang.OffsetDuplicate_offset.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment { Containment = lang.Shape_shapeDocs.ToMetaPointer(), Children = [] },
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = lang.OffsetDuplicate_source.ToMetaPointer(),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "circle", ResolveInfo = null }
                            ]
                        },
                        new SerializedReference
                        {
                            Reference = lang.OffsetDuplicate_altSource.ToMetaPointer(), Targets = []
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "docs",
                    Classifier = lang.Documentation.ToMetaPointer(),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = lang.Documentation_technical.ToMetaPointer(), Value = null
                        },
                        new SerializedProperty { Property = lang.Documentation_text.ToMetaPointer(), Value = null },
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "od"
                },
                new SerializedNode
                {
                    Id = "circle",
                    Classifier = lang.Circle.ToMetaPointer(),
                    Properties =
                    [
                        new SerializedProperty { Property = lang.IShape_uuid.ToMetaPointer(), Value = null },
                        new SerializedProperty { Property = lang.Circle_r.ToMetaPointer(), Value = null },
                        new SerializedProperty
                        {
                            Property = _lionWebVersion.BuiltIns.INamed_name.ToMetaPointer(), Value = null
                        },
                    ],
                    Containments =
                    [
                        new SerializedContainment { Containment = lang.Circle_center.ToMetaPointer(), Children = [] },
                        new SerializedContainment
                        {
                            Containment = lang.IShape_fixpoints.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment { Containment = lang.Shape_shapeDocs.ToMetaPointer(), Children = [] },
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };

        AssertEquals(expected, chunk);
    }

    [TestMethod]
    public void EmptyFeatures_Skip()
    {
        var circle = new Circle("circle");
        var node = new OffsetDuplicate("od") { Uuid = "abc", Source = circle, Docs = new Documentation("docs") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).WithSerializedEmptyFeatures(false)
            .Build();

        var chunk = serializer.SerializeToChunk([node, circle]);

        var langKey = ShapesLanguage.Instance.Key;
        var langVersion = ShapesLanguage.Instance.Version;
        var expected = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = langKey, Version = langVersion }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "od",
                    Classifier = new MetaPointer(langKey, langVersion, ShapesLanguage.Instance.OffsetDuplicate.Key),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer(langKey, langVersion,
                                ShapesLanguage.Instance.IShape_uuid.Key),
                            Value = "abc"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer(langKey, langVersion,
                                ShapesLanguage.Instance.OffsetDuplicate_docs.Key),
                            Children = ["docs"]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer(langKey, langVersion,
                                ShapesLanguage.Instance.OffsetDuplicate_source.Key),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "circle", ResolveInfo = null }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "docs",
                    Classifier = new MetaPointer(langKey, langVersion, ShapesLanguage.Instance.Documentation.Key),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "od"
                },
                new SerializedNode
                {
                    Id = "circle",
                    Classifier = new MetaPointer(langKey, langVersion, ShapesLanguage.Instance.Circle.Key),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };

        AssertEquals(expected, chunk);
    }

    private void AssertEquals(SerializationChunk expected, SerializationChunk actual)
    {
        Assert.AreEqual(expected.SerializationFormatVersion, actual.SerializationFormatVersion);
        CollectionAssert.AreEqual(expected.Languages, actual.Languages);
        Assert.AreEqual(expected.Nodes.Length, actual.Nodes.Length);
        var expectedNodes = expected.Nodes.GetEnumerator();
        var actualNodes = actual.Nodes.GetEnumerator();
        while (expectedNodes.MoveNext() && actualNodes.MoveNext())
        {
            var expectedNode = (SerializedNode)expectedNodes.Current!;
            var actualNode = (SerializedNode)actualNodes.Current!;
            Assert.AreEqual(expectedNode.Id, actualNode.Id);
            Assert.AreEqual(expectedNode.Classifier, actualNode.Classifier);
            CollectionAssert.AreEqual(
                expectedNode.Properties.OrderBy(p => p.Property.Key).ToList(),
                actualNode.Properties.OrderBy(p => p.Property.Key).ToList()
            );

            Assert.AreEqual(expectedNode.Containments.Length, actualNode.Containments.Length);
            using var expectedContainments = expectedNode.Containments.OrderBy(c => c.Containment.Key).GetEnumerator();
            using var actualContainments = actualNode.Containments.OrderBy(c => c.Containment.Key).GetEnumerator();
            while (expectedContainments.MoveNext() && actualContainments.MoveNext())
            {
                var expectedContainment = expectedContainments.Current;
                var actualContainment = actualContainments.Current;
                Assert.AreEqual(expectedContainment.Containment, actualContainment.Containment);
                CollectionAssert.AreEqual(expectedContainment.Children, actualContainment.Children);
            }

            Assert.AreEqual(expectedNode.References.Length, actualNode.References.Length);
            using var expectedReferences = expectedNode.References.OrderBy(r => r.Reference.Key).GetEnumerator();
            using var actualReferences = actualNode.References.OrderBy(r => r.Reference.Key).GetEnumerator();
            while (expectedReferences.MoveNext() && actualReferences.MoveNext())
            {
                var expectedReference = expectedReferences.Current;
                var actualReference = actualReferences.Current;
                Assert.AreEqual(expectedReference.Reference, actualReference.Reference);
                CollectionAssert.AreEqual(expectedReference.Targets, actualReference.Targets);
            }

            CollectionAssert.AreEqual(expectedNode.Annotations, actualNode.Annotations);

            Assert.AreEqual(expectedNode.Parent, actualNode.Parent);
        }
    }

    private TestContext testContextInstance;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}