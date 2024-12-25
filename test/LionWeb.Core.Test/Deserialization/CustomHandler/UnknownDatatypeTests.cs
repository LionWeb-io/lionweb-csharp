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
using M1;
using M3;
using Serialization;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.UnknownDatatype"/>
/// </summary>
[TestClass]
public class UnknownDatatypeTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<Feature, string?, IWritableNode, object?> heal)
        : DeserializerExceptionHandler
    {
        public override object? UnknownDatatype(string? value, LanguageEntity datatype, Feature property,
            IReadableNode node) =>
            heal(property, value, (IWritableNode)node);
    }


    [TestMethod]
    public void unknown_datatype_does_not_heal()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-myLang", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-myLang", "1", "key-Clock"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("key-myLang", "1", "key-wallClockTime"), Value = "17:43"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var myLang =
            new DynamicLanguage("id-myLang", _lionWebVersion) { Key = "key-myLang", Name = "myLang", Version = "1" };
        DynamicConcept clock = myLang.Concept("id-Clock", "key-Clock", "Clock");
        DynamicPrimitiveType time = myLang.PrimitiveType("id-Time", "key-Time", "Time");
        DynamicProperty wallClockTime =
            clock.Property("id-wallClockTime", "key-wallClockTime", "wallClockTime").OfType(time);

        myLang.NodeFactory = new SerializationLenientTests.LenientFactory(myLang);

        var deserializerHealingHandler =
            new DeserializerHealingHandler((feature, s, arg3) => null);

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(myLang)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreEqual(0, deserializedNodes[0].CollectAllSetFeatures().OfType<Property>().Count());
    }

    [TestMethod]
    public void unknown_datatype_heals()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-myLang", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-myLang", "1", "key-Clock"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("key-myLang", "1", "key-wallClockTime"), Value = "17:43"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var myLang =
            new DynamicLanguage("id-myLang", _lionWebVersion) { Key = "key-myLang", Name = "myLang", Version = "1" };
        DynamicConcept clock = myLang.Concept("id-Clock", "key-Clock", "Clock");
        DynamicPrimitiveType time = myLang.PrimitiveType("id-Time", "key-Time", "Time");
        DynamicProperty wallClockTime =
            clock.Property("id-wallClockTime", "key-wallClockTime", "wallClockTime").OfType(time);

        myLang.NodeFactory = new SerializationLenientTests.LenientFactory(myLang);

        var deserializerHealingHandler =
            new DeserializerHealingHandler((feature, s, arg3) => DateTime.Parse(s));

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(myLang)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreEqual(1, deserializedNodes[0].CollectAllSetFeatures().OfType<Property>().Count());
        Assert.IsInstanceOfType<DateTime>(deserializedNodes[0].Get(wallClockTime));
    }
}