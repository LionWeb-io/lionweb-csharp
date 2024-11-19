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

namespace LionWeb_CSharp_Test.tests.serialization;

using Examples.Shapes.Dynamic;
using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.M2.Generated.Test;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;
using LionWeb.Core.Utilities;
using System.Collections;
using Comparer = LionWeb.Core.Utilities.Comparer;

[TestClass]
public class SerializationTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersionsExtensions.GetCurrent();
    private readonly Language _language;

    public SerializationTests()
    {
        _language = ShapesLanguage.Instance;
    }

    [TestMethod]
    public void test_serialization_shapes_model()
    {
        INode rootNode = ExampleModels.ExampleModel(_language);

        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([rootNode]);
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
        var geometry = ExampleModels.ExampleModel(_language);
        var shape0 = (geometry.Get(_language.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes")) as IEnumerable)
            .Cast<INode>().First();

        Assert.IsInstanceOfType<INode>(shape0);
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([geometry, shape0]);
        Assert.AreEqual(4, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_optional_string_property_serialization()
    {
        var documentation = ((ShapesFactory)_language.GetFactory()).CreateDocumentation();
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([documentation]);
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

        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([line, refGeo]);
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

        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([compositeShape]);
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

        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([materialGroup]);
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

        Assert.ThrowsException<ArgumentException>(() => new Serializer(_lionWebVersion).SerializeToChunk([materialGroup]));
    }

    [TestMethod]
    [Ignore]
    public void DuplicateNode()
    {
        var b = new Circle("b");
        var a = new MaterialGroup("a") { DefaultShape = b };
        var b2 = new Circle("b");

        var x = new Serializer(_lionWebVersion).Serialize([a, b, b]).ToList();
        var serializedNodes = new Serializer(_lionWebVersion).Serialize([a, b, b2]).ToList();
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

        Assert.ThrowsException<ArgumentException>(() => new Serializer(_lionWebVersion).SerializeToChunk([a]));
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

        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([a]);
        Assert.AreEqual(2, serializationChunk.Languages.Length);
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var materialGroup = new MaterialGroup("a") { DefaultShape = new Circle("b") };

        var serializer = new Serializer(_lionWebVersion);
        var serializedNodes = serializer.Serialize(new SingleEnumerable<INode>(materialGroup.Descendants(true)));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.AreEqual(1, serializer.UsedLanguages.Count());
    }

    [TestMethod]
    public void SingleEnumerable_fail()
    {
        var materialGroup = new MaterialGroup("a") { DefaultShape = new Circle("b") };

        var serializer = new Serializer(_lionWebVersion);
        var serializedNodes = serializer.Serialize(new SingleEnumerable<INode>(materialGroup.Descendants(true)));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.ThrowsException<AssertFailedException>(() => Assert.AreEqual(2, serializedNodes.Count()));
    }

    [TestMethod]
    public void NoUsedLanguagesBeforeSerialization()
    {
        var materialGroup = new MaterialGroup("a") { DefaultShape = new Circle("b") };

        var serializer = new Serializer(_lionWebVersion);
        Assert.AreEqual(0, serializer.UsedLanguages.Count());
        var serializedNodes = serializer.Serialize(materialGroup.Descendants(true));
        Assert.AreEqual(2, serializedNodes.Count());
        Assert.AreEqual(1, serializer.UsedLanguages.Count());
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