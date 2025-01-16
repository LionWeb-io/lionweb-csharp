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
/// Tests for <see cref="IDeserializerHandler.InvalidPropertyValue{TValue}"/>
/// </summary>
[TestClass]
public class InvalidPropertyValueTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<string?, Feature, ICompressedId, object?> heal)
        : DeserializerExceptionHandler
    {
        public override object? InvalidPropertyValue<TValue>(string? value, Feature property, ICompressedId nodeId) =>
            heal(value, property, nodeId);
    }

    [TestMethod]
    public void invalid_property_value_does_not_heal()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("key-Shapes", "1", "key-x"), Value = "expects an integer"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((s, feature, arg3) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .WithCompressedIds(new(KeepOriginal:true))
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.IsFalse(deserializedNodes.OfType<Coord>().FirstOrDefault()?.CollectAllSetFeatures().OfType<Property>()
            .Contains(ShapesLanguage.Instance.Coord_x));
    }

    [TestMethod]
    public void invalid_property_value_heals()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("key-Shapes", "1", "key-x"), Value = "expects an integer"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };


        var deserializerHealingHandler = new DeserializerHealingHandler((s, feature, arg3) => 42);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.IsTrue(deserializedNodes.OfType<Coord>().FirstOrDefault()?.CollectAllSetFeatures().OfType<Property>()
            .Contains(ShapesLanguage.Instance.Coord_x));
    }
    
    [TestMethod]
    public void invalid_property_value_tries_to_heal_to_invalid_value()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("key-Shapes", "1", "key-x"), Value = "expects an integer"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };


        var deserializerHealingHandler = new DeserializerHealingHandler((s, feature, arg3) => 42.5);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }
}