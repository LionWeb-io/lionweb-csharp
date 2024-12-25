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

namespace LionWeb.Core.Test.Deserialization;

using Core.Serialization;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class DeserializationNameTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void NonUniqueName_ResolveName()
    {
        SerializationChunk chunk = NonUniqueNameChunk();

        var deserializer = new DeserializerBuilder()
            .WithUncompressedIds(true)
            .WithLanguage(ShapesLanguage.Instance)
            .WithReferenceResolveInfoHandling(ReferenceResolveInfoHandling.Name)
            .Build();

        var actual = deserializer.Deserialize(chunk);
        var offsetDuplicate = actual.OfType<OffsetDuplicate>().First();

        Assert.AreEqual("targetName", offsetDuplicate.AltSource?.Name);
        Assert.IsInstanceOfType<Line>(offsetDuplicate.AltSource);
    }

    [TestMethod]
    public void NonUniqueName_ResolveNameUnique()
    {
        SerializationChunk chunk = NonUniqueNameChunk();

        var deserializer = new DeserializerBuilder()
            .WithUncompressedIds(true)
            .WithLanguage(ShapesLanguage.Instance)
            .WithReferenceResolveInfoHandling(ReferenceResolveInfoHandling.NameIfUnique)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(chunk));
    }

    [TestMethod]
    public void NonUniqueName_ResolveNameNone()
    {
        SerializationChunk chunk = NonUniqueNameChunk();

        var deserializer = new DeserializerBuilder()
            .WithUncompressedIds(true)
            .WithLanguage(ShapesLanguage.Instance)
            .WithReferenceResolveInfoHandling(ReferenceResolveInfoHandling.None)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(chunk));
    }

    private SerializationChunk NonUniqueNameChunk()
    {
        var chunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties = [],
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-alt-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = null, ResolveInfo = "targetName" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Line"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "targetName"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "bum",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "targetName"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };
        return chunk;
    }

    [TestMethod]
    public void UniqueName_ResolveName()
    {
        SerializationChunk chunk = UniqueNameChunk();

        var deserializer = new DeserializerBuilder()
            .WithUncompressedIds(true)
            .WithLanguage(ShapesLanguage.Instance)
            .WithReferenceResolveInfoHandling(ReferenceResolveInfoHandling.Name)
            .Build();

        var actual = deserializer.Deserialize(chunk);
        var offsetDuplicate = actual.OfType<OffsetDuplicate>().First();

        Assert.AreEqual("targetName", offsetDuplicate.AltSource?.Name);
        Assert.IsInstanceOfType<Circle>(offsetDuplicate.AltSource);
    }

    [TestMethod]
    public void UniqueName_ResolveNameUnique()
    {
        SerializationChunk chunk = UniqueNameChunk();

        var deserializer = new DeserializerBuilder()
            .WithUncompressedIds(true)
            .WithLanguage(ShapesLanguage.Instance)
            .WithReferenceResolveInfoHandling(ReferenceResolveInfoHandling.NameIfUnique)
            .Build();

        var actual = deserializer.Deserialize(chunk);
        var offsetDuplicate = actual.OfType<OffsetDuplicate>().First();

        Assert.AreEqual("targetName", offsetDuplicate.AltSource?.Name);
        Assert.IsInstanceOfType<Circle>(offsetDuplicate.AltSource);
    }

    private SerializationChunk UniqueNameChunk()
    {
        var chunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties = [],
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-alt-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = null, ResolveInfo = "targetName" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Line"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "otherName"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "bum",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "targetName"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };
        return chunk;
    }
}