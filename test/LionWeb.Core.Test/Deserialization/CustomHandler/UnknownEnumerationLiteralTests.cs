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
using Languages.Generated.V2024_1.WithEnum.M2;
using M1;
using M3;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.UnknownEnumerationLiteral"/>
/// </summary>
[TestClass]
public class UnknownEnumerationLiteralTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<string, Enumeration, Feature, IWritableNode, Enum?> heal)
        : DeserializerExceptionHandler
    {
        public override Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration, Feature property,
            IReadableNode node) =>
            heal(key, enumeration, property, (IWritableNode)node);
    }

    [TestMethod]
    public void unknown_enumeration_literal_does_not_heal()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "WithEnum", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("WithEnum", "1", "EnumHolder"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("WithEnum", "1", "enumValue"),
                            Value = "unknown-enumeration-literal"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((s, enumeration, arg3, arg4) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.IsFalse(deserializedNodes.OfType<EnumHolder>().FirstOrDefault()?.CollectAllSetFeatures()
            .OfType<Property>().Contains(WithEnumLanguage.Instance.EnumHolder_enumValue));
    }

    [TestMethod]
    public void unknown_enumeration_literal_heals()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "WithEnum", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("WithEnum", "1", "EnumHolder"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("WithEnum", "1", "enumValue"),
                            Value = "unknown-enumeration-literal"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        MyEnum myEnumLiteral = MyEnum.literal1;
        var deserializerHealingHandler =
            new DeserializerHealingHandler((s, enumeration, arg3, arg4) => myEnumLiteral);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(deserializedNodes.OfType<EnumHolder>().FirstOrDefault()?.CollectAllSetFeatures()
            .OfType<Property>().Contains(WithEnumLanguage.Instance.EnumHolder_enumValue));
        Assert.AreEqual(myEnumLiteral, deserializedNodes.OfType<EnumHolder>().FirstOrDefault()?.EnumValue);
    }

    private enum ForTestPurpose
    {
        InvalidEnum = 1
    }

    [TestMethod]
    public void unknown_enumeration_literal_tries_to_heal_to_invalid_value()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "WithEnum", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("WithEnum", "1", "EnumHolder"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("WithEnum", "1", "enumValue"),
                            Value = "unknown-enumeration-literal"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((s, enumeration, arg3, arg4) => ForTestPurpose.InvalidEnum);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();
        Assert.ThrowsExactly<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }
}