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

namespace LionWeb.Core.Test.Deserialization.CustomHandler;

using Core.Serialization;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M3;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.UnknownClassifier"/>
/// </summary>>
[TestClass]
public class UnknownClassifierTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<CompressedMetaPointer, CompressedId, Classifier?> heal)
        : DeserializerExceptionHandler
    {
        public override Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id) =>
            heal(classifier, id);
    }

    /// <summary>
    /// Does not heal, returns null.
    /// </summary>
    [TestMethod]
    public void unknown_classifier_does_not_heal()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-unknown-classifier"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((pointer, id) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .WithUncompressedIds()
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(0, deserializedNodes.Count);
    }

    /// <summary>
    /// Heals by replacing unknown classifier with a known classifier in <see cref="ShapesLanguage"/> language  
    ///
    /// Test scenario:
    /// Deserialize a node which is serialized with a different (possibly one of the previous)
    /// version of the language
    /// </summary>
    [TestMethod]
    public void unknown_classifier_heals_from_not_matching_language_versions()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "0", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var unknownClassifierDeserializerHealingHandler =
            new DeserializerHealingHandler((pointer, id) => ShapesLanguage.Instance.Circle);

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownClassifierDeserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreSame(ShapesLanguage.Instance.Circle, deserializedNodes[0].GetClassifier());
    }

    /// <summary>
    /// Heals by replacing unknown classifier with a classifier from another language
    /// </summary>
    [TestMethod]
    public void unknown_classifier_heals_from_unknown_classifier_key()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-unknown-classifier"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        DynamicLanguage dynamicLanguage =
            new("id-XLang", _lionWebVersion) { Key = "key-XLang", Name = "XLang", Version = "1" };
        DynamicConcept dynamicConcept =
            dynamicLanguage.Concept("id-XLang-concept", "key-unknown-classifier", "name-XLang-concept:");

        Classifier? heal = dynamicLanguage.Entities.OfType<Classifier>()
            .FirstOrDefault(classifier => classifier.Key == serializationChunk.Nodes[0].Classifier.Key);

        var deserializerHealingHandler = new DeserializerHealingHandler((pointer, id) => heal);

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .WithLanguage(dynamicLanguage)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreSame(dynamicConcept, deserializedNodes[0].GetClassifier());
    }
}