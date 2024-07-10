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

namespace LionWeb.Core.Test;

using Examples.Shapes.Dynamic;
using Examples.Shapes.M2;
using M1;
using M2;
using M3;
using Serialization;
using System.Collections;
using Utilities;
using Comparer = Utilities.Comparer;

[TestClass]
public class SerializationTests
{
    private readonly Language _language;
    private readonly ShapesFactory _factory;

    public SerializationTests()
    {
        _language = ShapesLanguage.Instance;
        _factory = _language.GetFactory() as ShapesFactory;
    }

    [TestMethod]
    public void test_serialization_shapes_model()
    {
        INode rootNode = ExampleModels.ExampleModel(_language);

        var serializationChunk = Serializer.Serialize(new List<INode> { rootNode });
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        // Just run the deserializer for now (without really checking anything), to see whether it crashes or not:
        new Deserializer([_language]).Deserialize(serializationChunk);
    }

    // Disabled until #19 is merged
    // [TestMethod]
    // public void test_serialization_shapes_language()
    // {
    //     var serializationChunk = LanguageSerializer.Serialize(ShapesLanguage.Instance);
    //     Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));
    //
    //     var redeserialized = LanguageDeserializer.Deserialize(serializationChunk);
    //     var comparer = new Comparer([ShapesLanguage.Instance], redeserialized.Cast<IReadableNode>().ToList());
    //     Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    // }

    [TestMethod]
    public void test_serialization_shapes_language_with_external_annotations()
    {
        var serializationChunk = LanguageSerializer.Serialize(ShapesLanguage.Instance);

        var redeserialized = LanguageDeserializer.Deserialize(serializationChunk);
        Language redeserializedShapes = redeserialized.Cast<INode>().OfType<Language>().First();

        var language = new DynamicLanguage("id-anns") { Key = "key-anns", Name = "Anns", Version = "1" };

        var ann = language.Annotation("id-classifierAnn", "key-classifierAnn", "ClassifierAnn");
        Property age = ann
            .Annotating(M3Language.Instance.Classifier)
            .Implementing(BuiltInsLanguage.Instance.INamed)
            .Property("id-age", "key-age", "Age").OfType(BuiltInsLanguage.Instance.Integer);

        DynamicClassifier redeserializedCircle =
            (DynamicClassifier)redeserializedShapes.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);
        var annInst = language.GetFactory().CreateNode("ann-inst", ann);
        annInst.Set(BuiltInsLanguage.Instance.INamed_name, "Hello");
        annInst.Set(age, 23);
        redeserializedCircle.AddAnnotations([annInst]);

        var serializationChunk2 = LanguageSerializer.Serialize(redeserializedShapes);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk2));
        var redeserialized2 = LanguageDeserializer.Deserialize(serializationChunk2, language);
        Language redeserializedShapes2 = redeserialized2.Cast<INode>().OfType<Language>().First();
        DynamicClassifier redeserializedCircle2 =
            (DynamicClassifier)redeserializedShapes2.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);

        var comparer = new Comparer(redeserializedCircle.GetAnnotations().ToList(),
            redeserializedCircle2.GetAnnotations().ToList());
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void test_serialization_shapes_language_together_with_annotations()
    {
        var serializationChunk = LanguageSerializer.Serialize(ShapesLanguage.Instance);

        var redeserialized = LanguageDeserializer.Deserialize(serializationChunk);
        Language redeserializedShapes = redeserialized.Cast<INode>().OfType<Language>().First();

        var language = new DynamicLanguage("id-anns") { Key = "key-anns", Name = "Anns", Version = "1" };

        var ann = language.Annotation("id-classifierAnn", "key-classifierAnn", "ClassifierAnn");
        Property age = ann
            .Annotating(M3Language.Instance.Classifier)
            .Implementing(BuiltInsLanguage.Instance.INamed)
            .Property("id-age", "key-age", "Age").OfType(BuiltInsLanguage.Instance.Integer);

        DynamicClassifier redeserializedCircle =
            (DynamicClassifier)redeserializedShapes.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);
        var annInst = language.GetFactory().CreateNode("ann-inst", ann);
        annInst.Set(BuiltInsLanguage.Instance.INamed_name, "Hello");
        annInst.Set(age, 23);
        redeserializedCircle.AddAnnotations([annInst]);

        var serializationChunk2 = LanguageSerializer.Serialize(redeserializedShapes, language);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk2));
        var redeserialized2 = LanguageDeserializer.Deserialize(serializationChunk2);
        Language redeserializedShapes2 = redeserialized2.Cast<INode>().OfType<Language>()
            .First(l => l.Key == ShapesLanguage.Instance.Key);
        DynamicClassifier redeserializedCircle2 =
            (DynamicClassifier)redeserializedShapes2.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);

        var comparer = new Comparer(redeserializedCircle.GetAnnotations().ToList(),
            redeserializedCircle2.GetAnnotations().ToList());
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void test_serialization_lioncore()
    {
        var serializationChunk = LanguageSerializer.Serialize(M3Language.Instance);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        // Just run the deserializer for now (without really checking anything), to see whether it crashes or not:
        var deserializer = new LanguageDeserializer(serializationChunk, false);
        deserializer.DeserializeLanguages();
    }

    [TestMethod]
    public void test_no_double_serialization()
    {
        var geometry = ExampleModels.ExampleModel(_language);
        var shape0 = (geometry.Get(_language.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes")) as IEnumerable)
            .Cast<INode>().First();

        Assert.IsInstanceOfType<INode>(shape0);
        var serializationChunk = Serializer.Serialize([geometry, shape0]);
        Assert.AreEqual(4, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_optional_string_property_serialization()
    {
        var documentation = _factory.CreateDocumentation();
        var serializationChunk = Serializer.Serialize([documentation]);

        var serializedProperty = serializationChunk.Nodes[0].Properties.First(p => p.Property.Key == "key-text");
        Assert.IsNull(serializedProperty.Value);
        Assert.AreEqual("key-text", serializedProperty.Property.Key);
    }

    [TestMethod]
    public void SerializeRefToUnsetName()
    {
        var line = new Line("line") { Start = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        var refGeo = new ReferenceGeometry("ref") { Shapes = [line] };

        var serializationChunk = Serializer.Serialize([line, refGeo]);
        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);

        var comparer = new Comparer([line, refGeo], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void SerializeUnsetRequiredContainment()
    {
        var compositeShape = new CompositeShape("comp");

        var serializationChunk = Serializer.Serialize([compositeShape]);
        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);

        var comparer = new Comparer([compositeShape], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void SerializeUnsetRequiredReference()
    {
        var materialGroup = new MaterialGroup("goup");

        var serializationChunk = Serializer.Serialize([materialGroup]);
        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);

        var comparer = new Comparer([materialGroup], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }
}