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

// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
namespace LionWeb_CSharp_Test.tests.serialization;

using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;
using LionWeb.Core.Utilities;
using Comparer = LionWeb.Core.Utilities.Comparer;

[TestClass]
public class LanguageSerializationTests
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersionsExtensions.GetCurrent();
    private readonly ILionCoreLanguage _m3=_lionWebVersion.GetLionCore();
    private readonly IBuiltInsLanguage _builtIns=_lionWebVersion.GetBuiltIns();
    
    [TestMethod]
    public void test_serialization_shapes_language_with_external_annotations()
    {
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([ShapesLanguage.Instance]);

        var redeserialized = LanguageDeserializer.Deserialize(serializationChunk);
        Language redeserializedShapes = redeserialized.Cast<INode>().OfType<Language>().First();

        var language = new DynamicLanguage("id-anns", _lionWebVersion) { Key = "key-anns", Name = "Anns", Version = "1" };

        var ann = language.Annotation("id-classifierAnn", "key-classifierAnn", "ClassifierAnn");
        Property age = ann
            .Annotating(_m3.Classifier)
            .Implementing(_builtIns.INamed)
            .Property("id-age", "key-age", "Age").OfType(_builtIns.Integer);

        DynamicClassifier redeserializedCircle =
            (DynamicClassifier)redeserializedShapes.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);
        var annInst = language.GetFactory().CreateNode("ann-inst", ann);
        annInst.Set(_builtIns.INamed_name, "Hello");
        annInst.Set(age, 23);
        redeserializedCircle.AddAnnotations([annInst]);

        var serializationChunk2 = new Serializer(_lionWebVersion).SerializeToChunk([redeserializedShapes]);
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
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([ShapesLanguage.Instance]);

        var redeserialized = LanguageDeserializer.Deserialize(serializationChunk);
        Language redeserializedShapes = redeserialized.Cast<INode>().OfType<Language>().First();

        var language = new DynamicLanguage("id-anns", _lionWebVersion) { Key = "key-anns", Name = "Anns", Version = "1" };

        var ann = language.Annotation("id-classifierAnn", "key-classifierAnn", "ClassifierAnn");
        Property age = ann
            .Annotating(_m3.Classifier)
            .Implementing(_builtIns.INamed)
            .Property("id-age", "key-age", "Age").OfType(_builtIns.Integer);

        DynamicClassifier redeserializedCircle =
            (DynamicClassifier)redeserializedShapes.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);
        var annInst = language.GetFactory().CreateNode("ann-inst", ann);
        annInst.Set(_builtIns.INamed_name, "Hello");
        annInst.Set(age, 23);
        redeserializedCircle.AddAnnotations([annInst]);

        var serializationChunk2 = new Serializer(_lionWebVersion).SerializeToChunk([redeserializedShapes, language]);
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
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([_m3]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        // Just run the deserializer for now (without really checking anything), to see whether it crashes or not:
        var deserializer = new LanguageDeserializer(_lionWebVersion, false);
        foreach (var serializedNode in serializationChunk.Nodes)
        {
            deserializer.Process(serializedNode);
        }
        deserializer.Finish();
    }

    [TestMethod]
    public void test_serialization_shapes_language()
    {
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([ShapesLanguage.Instance]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        var redeserialized = LanguageDeserializer.Deserialize(serializationChunk);
        var comparer = new Comparer([ShapesLanguage.Instance], redeserialized.Cast<IReadableNode>().ToList());
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
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