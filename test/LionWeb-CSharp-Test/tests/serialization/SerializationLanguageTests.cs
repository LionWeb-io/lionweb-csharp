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
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private readonly ILionCoreLanguage _m3 = _lionWebVersion.LionCore;
    private readonly IBuiltInsLanguage _builtIns = _lionWebVersion.BuiltIns;

    [TestMethod]
    public void shapes_language_with_external_annotations()
    {
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([ShapesLanguage.Instance]);

        var redeserialized = ILanguageDeserializerExtensions.Deserialize(serializationChunk);
        Language redeserializedShapes = redeserialized.Cast<INode>().OfType<Language>().First();

        var language =
            new DynamicLanguage("id-anns", _lionWebVersion) { Key = "key-anns", Name = "Anns", Version = "1" };

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
        var redeserialized2 = ILanguageDeserializerExtensions.Deserialize(serializationChunk2, [language]);
        Language redeserializedShapes2 = redeserialized2.Cast<INode>().OfType<Language>().First();
        DynamicClassifier redeserializedCircle2 =
            (DynamicClassifier)redeserializedShapes2.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);

        var comparer = new Comparer(redeserializedCircle.GetAnnotations().ToList(),
            redeserializedCircle2.GetAnnotations().ToList());
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void shapes_language_together_with_annotations()
    {
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([ShapesLanguage.Instance]);

        var redeserialized =
            new LanguageDeserializer(IDeserializerVersionSpecifics.Create(_lionWebVersion))
            {
                StoreUncompressedIds = true
            }.Deserialize(serializationChunk);
        Language redeserializedShapes = redeserialized.Cast<INode>().OfType<Language>().First();

        var language =
            new DynamicLanguage("id-anns", _lionWebVersion) { Key = "key-anns", Name = "Anns", Version = "1" };

        var ann = language.Annotation("id-classifierAnn", "key-classifierAnn", "ClassifierAnn")
            .Annotating(_m3.Classifier)
            .Implementing(_builtIns.INamed);
        Property age = ann.Property("id-age", "key-age", "Age")
            .OfType(_builtIns.Integer);

        var otherClassifier = ann.Reference("id-otherClassifier", "key-otherClassifier", "OtherClassifier")
            .OfType(_m3.Classifier);

        DynamicClassifier redeserializedCircle =
            (DynamicClassifier)redeserializedShapes.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);
        var annInst = language.GetFactory().CreateNode("ann-inst", ann);
        annInst.Set(_builtIns.INamed_name, "Hello");
        annInst.Set(age, 23);

        DynamicClassifier redeserializedIShape =
            (DynamicClassifier)redeserializedShapes.ClassifierByKey(ShapesLanguage.Instance.IShape.Key);
        annInst.Set(otherClassifier, redeserializedIShape);

        redeserializedCircle.AddAnnotations([annInst]);

        var serializationChunk2 = new Serializer(_lionWebVersion).SerializeToChunk([redeserializedShapes, language]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk2));
        var redeserialized2 =
            new LanguageDeserializer(IDeserializerVersionSpecifics.Create(_lionWebVersion))
            {
                StoreUncompressedIds = true
            }.Deserialize(serializationChunk2);
        Language redeserializedShapes2 = redeserialized2.Cast<INode>().OfType<Language>()
            .First(l => l.Key == ShapesLanguage.Instance.Key);
        DynamicClassifier redeserializedCircle2 =
            (DynamicClassifier)redeserializedShapes2.ClassifierByKey(ShapesLanguage.Instance.Circle.Key);

        var comparer = new Comparer(redeserializedCircle.GetAnnotations().ToList(),
            redeserializedCircle2.GetAnnotations().ToList());
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void LionCore()
    {
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([_m3]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        // Just run the deserializer for now (without really checking anything), to see whether it crashes or not:
        var deserializer = new LanguageDeserializer(IDeserializerVersionSpecifics.Create(_lionWebVersion))
        {
            Handler = new SkipDeserializationHandler()
        };
        foreach (var serializedNode in serializationChunk.Nodes)
        {
            deserializer.Process(serializedNode);
        }

        deserializer.Finish();
    }

    private class SkipDeserializationHandler : DeserializerExceptionHandler
    {
        public override bool SkipDeserializingDependentNode(CompressedId id) => false;
    }

    [TestMethod]
    public void LionCore_Builtins_24_Compatible()
    {
        List<Language> input =
        [
            LionWebVersions.v2023_1.LionCore,
            LionWebVersions.v2023_1.BuiltIns,
            LionWebVersions.v2024_1.LionCore,
            LionWebVersions.v2024_1.BuiltIns
        ];
        var chunk =
            new Serializer(LionWebVersions.v2024_1_Compatible) { StoreUncompressedIds = true }.SerializeToChunk(input);

        var deserializer =
            new LanguageDeserializer(IDeserializerVersionSpecifics.Create(LionWebVersions.v2024_1_Compatible))
            {
                StoreUncompressedIds = true, Handler = new SkipDeserializationHandler()
            };

        var actual = deserializer.Deserialize(chunk).Cast<Language>().ToList();

        // We can't just compare the language nodes, because M3 concepts are self-defined (e.g. LionCore.Classifier = Concept, not Language)
        
        using var expectedEnumerator = input.SelectMany(i => M1Extensions.Descendants<IKeyed>(i, true, true)).GetEnumerator();
        using var actualEnumerator = actual.SelectMany(i => M1Extensions.Descendants<IKeyed>(i, true, true)).GetEnumerator();

        while (expectedEnumerator.MoveNext() && actualEnumerator.MoveNext())
        {
            var ex = expectedEnumerator.Current;
            var act = actualEnumerator.Current;
            Assert.AreEqual(ex.Key, act.Key);
            Assert.AreEqual(ex.GetId(), act.GetId());
            Assert.AreEqual(ex.Name, act.Name);
        }
        
        Assert.IsFalse(expectedEnumerator.MoveNext());
        Assert.IsFalse(actualEnumerator.MoveNext());
    }

    [TestMethod]
    public void shapes_language()
    {
        var serializationChunk = new Serializer(_lionWebVersion).SerializeToChunk([ShapesLanguage.Instance]);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        var redeserialized = ILanguageDeserializerExtensions.Deserialize(serializationChunk);
        var comparer = new Comparer([ShapesLanguage.Instance], redeserialized.Cast<IReadableNode>().ToList());
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    [TestMethod]
    public void LanguageWithDuplicateNodeId_Language()
    {
        var language = new DynamicLanguage("testDuplicateNodeId", _lionWebVersion)
        {
            Key = "key-myLanguage", Version = "1", Name = "myLanguage"
        };
        var myConcept = language.Concept("testDuplicateNodeId", "key-myConcept", "myConcept");
        var myAnnotation = language.Annotation("otherNodeId", "key-myAnnotation", "myAnnotation")
            .Annotating(myConcept);

        var chunk = new SerializationChunk()
        {
            SerializationFormatVersion = "2023.1",
            Languages = [new SerializedLanguageReference { Key = "key-myLanguage", Version = "1" }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "testDuplicateNodeId",
                    Classifier = new MetaPointer("LionCore-M3", "2023.1", "Language"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2023.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "myLanguage"
                        },
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-M3", "2023.1", "IKeyed-key"),
                            Value = "key-myLanguage"
                        },
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-M3", "2023.1", "Language-version"), Value = "1"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("LionCore-M3", "2023.1", "Language-entities"),
                            Children = ["testDuplicateNodeId", "otherNodeId"]
                        }
                    ],
                    Annotations = [],
                    References = []
                },
                new SerializedNode
                {
                    Id = "otherNodeId",
                    Classifier = new MetaPointer("LionCore-M3", "2023.1", "Concept"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2023.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "myConcept"
                        },
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-M3", "2023.1", "IKeyed-key"),
                            Value = "key-myConcept"
                        },
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-M3", "2023.1", "Concept-abstract"), Value = "false"
                        },
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-M3", "2023.1", "Concept-partition"),
                            Value = "false"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "testDuplicateNodeId"
                },
                new SerializedNode
                {
                    Id = "testDuplicateNodeId",
                    Classifier = new MetaPointer("LionCore-M3", "2023.1", "Annotation"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2023.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "myAnnotation"
                        },
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-M3", "2023.1", "IKeyed-key"),
                            Value = "key-myAnnotation"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "testDuplicateNodeId"
                }
            ]
        };

        Assert.ThrowsException<DeserializerException>(() => ILanguageDeserializerExtensions.Deserialize(chunk));
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