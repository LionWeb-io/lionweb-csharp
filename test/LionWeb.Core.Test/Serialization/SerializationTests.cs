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
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M2;
using M3;
using System.Collections;

[TestClass]
public class SerializationTests : SerializationTestsBase
{
    [TestMethod]
    public void Shapes_model()
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
    public void No_double_serialization()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(_language);
        var shape0 =
            (geometry.Get(_language.ClassifierByKey("TestPartition")
                .FeatureByKey("TestPartition-links")) as IEnumerable)
            .Cast<INode>().First();

        Assert.IsInstanceOfType<INode>(shape0);
        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([geometry, shape0]);
        Assert.AreEqual(4, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void Optional_string_property_serialization()
    {
        var documentation =
            ((TestLanguageFactory)TestLanguageLanguage.Instance.GetFactory()).CreateDataTypeTestConcept();
        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([documentation]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        var serializedProperty = serializationChunk.Nodes[0].Properties
            .First(p => p.Property.Key == "DataTypeTestConcept-stringValue_0_1");
        Assert.IsNull(serializedProperty.Value);
        Assert.AreEqual("DataTypeTestConcept-stringValue_0_1", serializedProperty.Property.Key);
    }

    [TestMethod]
    public void SerializeRefToUnsetName()
    {
        var line = new LinkTestConcept("line") { Containment_0_1 = new LinkTestConcept("coord") { Name = "1", } };
        var refGeo = new LinkTestConcept("ref") { Reference_0_n = [line] };

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([line, refGeo]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        AssertEquals([line, refGeo], nodes);
    }

    [TestMethod]
    public void SerializeUnsetRequiredContainment()
    {
        var compositeShape = new LinkTestConcept("comp");

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([compositeShape]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        AssertEquals([compositeShape], nodes);
    }

    [TestMethod]
    public void SerializeUnsetRequiredReference()
    {
        var materialGroup = new LinkTestConcept("goup");

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([materialGroup]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        AssertEquals([materialGroup], nodes);
    }

    [TestMethod]
    public void DuplicateId()
    {
        var materialGroup = new LinkTestConcept("duplicate") { Containment_1 = new LinkTestConcept("duplicate") };

        Assert.ThrowsExactly<SerializerException>(() =>
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().SerializeToChunk([materialGroup]));
    }

    [TestMethod]
    public void DuplicateNode()
    {
        var b = new LinkTestConcept("b");
        var a = new LinkTestConcept("a") { Containment_1 = b };
        var b2 = new LinkTestConcept("b");

        Assert.ThrowsExactly<SerializerException>(() =>
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().Serialize([a, b, b]).ToList());
        Assert.ThrowsExactly<SerializerException>(() =>
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().Serialize([a, b, b2]).ToList());
    }

    [TestMethod]
    public void DuplicateUsedLanguage()
    {
        var lang = new DynamicLanguage("abc", _lionWebVersion)
        {
            Key = TestLanguageLanguage.Instance.Key, Version = TestLanguageLanguage.Instance.Version
        };
        var materialGroup = lang.Concept("efg", TestLanguageLanguage.Instance.LinkTestConcept.Key,
            TestLanguageLanguage.Instance.LinkTestConcept.Name);
        var defaultShape = materialGroup.Containment("ijk",
            TestLanguageLanguage.Instance.LinkTestConcept_containment_1.Key,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_1.Name);

        var a = lang.GetFactory().CreateNode("a", materialGroup);
        var b = new LinkTestConcept("b");
        a.Set(defaultShape, b);

        Assert.ThrowsExactly<SerializerException>(() =>
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().SerializeToChunk([a]));
    }

    [TestMethod]
    public void DuplicateUsedLanguage_DifferentVersion()
    {
        var lang = new DynamicLanguage("abc", _lionWebVersion)
        {
            Key = TestLanguageLanguage.Instance.Key, Version = TestLanguageLanguage.Instance.Version + "hello"
        };
        var materialGroup = lang.Concept("efg", TestLanguageLanguage.Instance.LinkTestConcept.Key,
            TestLanguageLanguage.Instance.LinkTestConcept.Name);
        var defaultShape = materialGroup.Containment("ijk",
            TestLanguageLanguage.Instance.LinkTestConcept_containment_1.Key,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_1.Name);

        var a = lang.GetFactory().CreateNode("a", materialGroup);
        var b = new LinkTestConcept("b");
        a.Set(defaultShape, b);

        var serializationChunk =
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().SerializeToChunk([a]);
        Assert.AreEqual(2, serializationChunk.Languages.Length);
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var materialGroup = new LinkTestConcept("a") { Containment_1 = new LinkTestConcept("b") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize(new SingleEnumerable<INode>(materialGroup.Descendants(true)));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.AreEqual(1, serializer.UsedLanguages.Count());
    }

    [TestMethod]
    public void SingleEnumerable_fail()
    {
        var materialGroup = new LinkTestConcept("a") { Containment_1 = new LinkTestConcept("b") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var serializedNodes = serializer.Serialize(new SingleEnumerable<INode>(materialGroup.Descendants(true)));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.ThrowsExactly<AssertFailedException>(() => Assert.AreEqual(2, serializedNodes.Count()));
    }

    [TestMethod]
    public void NoUsedLanguagesBeforeSerialization()
    {
        var materialGroup = new LinkTestConcept("a") { Containment_1 = new LinkTestConcept("b") };

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
        var materialGroup = new LinkTestConcept("a") { Containment_1 = new LinkTestConcept("b") { Name = text } };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var stream = new MemoryStream();
        JsonUtils.WriteNodesToStream(stream, serializer, materialGroup.Descendants(true));

        stream.Seek(0, SeekOrigin.Begin);

        var deserializer = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build();

        var readableNodes = JsonUtils.ReadNodesFromStream(stream, deserializer);

        Assert.AreEqual(text,
            readableNodes.OfType<INode>().SelectMany(n => n.Descendants(true)).OfType<LinkTestConcept>().Last().Name);
    }

    [TestMethod]
    public async Task SerializedLionWebVersion_Async()
    {
        const string text = "\ud83d\ude0a Hällö 😊";
        var materialGroup = new LinkTestConcept("a") { Containment_1 = new LinkTestConcept("b") { Name = text } };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var stream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(stream, serializer, materialGroup.Descendants(true));

        stream.Seek(0, SeekOrigin.Begin);

        var deserializer = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build();

        string serializedLionWebVersion = null;
        var readableNodes = await JsonUtils.ReadNodesFromStreamAsync(stream, deserializer, s =>
        {
            serializedLionWebVersion = s;
        });

        Assert.AreEqual(_lionWebVersion.VersionString, serializedLionWebVersion);
    }

    [TestMethod]
    public void SerializedLionWebVersion_Sync()
    {
        const string text = "\ud83d\ude0a Hällö 😊";
        var materialGroup = new LinkTestConcept("a") { Containment_1 = new LinkTestConcept("b") { Name = text } };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build();
        var stream = new MemoryStream();
        JsonUtils.WriteNodesToStream(stream, serializer, materialGroup.Descendants(true));

        stream.Seek(0, SeekOrigin.Begin);

        var deserializer = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build();

        string serializedLionWebVersion = null;
        var readableNodes = JsonUtils.ReadNodesFromStream(stream, deserializer, s =>
        {
            serializedLionWebVersion = s;
        });

        Assert.AreEqual(_lionWebVersion.VersionString, serializedLionWebVersion);
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

        var serializer = new SerializerBuilder()
            .WithSerializedEmptyFeatures(false)
            .WithLionWebVersion(_lionWebVersion)
            .Build();
        var actual = serializer.SerializeToChunk([parent, target]);

        var nodes = new DeserializerBuilder()
            .WithLanguage(SDTLangLanguage.Instance)
            .WithLanguage(SDTLangLanguage.Instance)
            .Build()
            .Deserialize(serializedNodes);

        AssertEquals([node], nodes);
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
}