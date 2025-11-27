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

namespace LionWeb.Core.Test.Serialization.LionWebVersion;

using Core.Serialization;
using M1;
using M2;
using M3;
using System.Text.Json;
using LionWebVersions = LionWebVersions;

[TestClass]
public class InstanceTests : LionWebVersionsTestBase
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SameVersion(Type versionIface)
    {
        var lionWebVersion = LionWebVersions.GetByInterface(versionIface);

        DynamicLanguage language = Serialize(lionWebVersion, out INode conceptNodeA, out INode conceptNodeB,
            out SerializationChunk chunk);

        List<IReadableNode> nodes = Deserialize(language, lionWebVersion, chunk);

        AssertEquals([conceptNodeA, conceptNodeB], nodes);
    }

    [TestMethod]
    public void Serialize23_Deserialize24()
    {
        DynamicLanguage language = Serialize(LionWebVersions.v2023_1, out INode conceptNodeA, out INode conceptNodeB,
            out SerializationChunk chunk);

        Assert.ThrowsExactly<VersionMismatchException>(() => Deserialize(language, LionWebVersions.v2024_1, chunk));
    }

    [TestMethod]
    public void Serialize24_Deserialize23()
    {
        DynamicLanguage language = Serialize(LionWebVersions.v2024_1, out INode conceptNodeA, out INode conceptNodeB,
            out SerializationChunk chunk);

        Assert.ThrowsExactly<VersionMismatchException>(() => Deserialize(language, LionWebVersions.v2023_1, chunk));
    }

    [TestMethod]
    public void Serialize23_Deserialize24_Compatible()
    {
        DynamicLanguage language = Serialize(LionWebVersions.v2023_1, out INode conceptNodeA, out INode conceptNodeB,
            out SerializationChunk chunk);

        List<IReadableNode> nodes = Deserialize(language, LionWebVersions.v2024_1_Compatible, chunk);

        AssertEquals([conceptNodeA, conceptNodeB], nodes);
    }

    [TestMethod]
    public void Serialize24_Deserialize24_Compatible()
    {
        DynamicLanguage language = Serialize(LionWebVersions.v2024_1, out INode conceptNodeA, out INode conceptNodeB,
            out SerializationChunk chunk);

        List<IReadableNode> nodes = Deserialize(language, LionWebVersions.v2024_1_Compatible, chunk);

        AssertEquals([conceptNodeA, conceptNodeB], nodes);
    }

    [TestMethod]
    public void InstanceLanguage23_Chunk24()
    {
        CreateInstances(LionWebVersions.v2023_1, out _, out _, out var input);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        Assert.ThrowsExactly<VersionMismatchException>(() => serializer.SerializeToChunk(input));
    }

    [TestMethod]
    public void InstanceLanguage23_Chunk24_Compatible()
    {
        CreateInstances(LionWebVersions.v2023_1, out _, out _, out var input);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1_Compatible)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        var chunk = serializer.SerializeToChunk(input);
        Assert.AreEqual("2024.1", chunk.SerializationFormatVersion);
    }

    [TestMethod]
    public void InstanceLanguage24_Chunk23()
    {
        CreateInstances(LionWebVersions.v2024_1, out _, out _, out var input);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        Assert.ThrowsExactly<VersionMismatchException>(() => serializer.SerializeToChunk(input));
    }

    [TestMethod]
    public void SerializeInstancesFromDifferentVersions()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var lang24 = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        lang24.Version = "xx2024.1";

        var concept23 = lang23.ClassifierByKey("key-myConcept");
        var concept24 = lang24.ClassifierByKey("key-myConcept");

        var node23 = lang23.GetFactory().CreateNode(NextId("2023_1_inst_"), concept23);
        var node24 = lang24.GetFactory().CreateNode(NextId("2024_1_inst_"), concept24);

        node23.Set(concept23.FeatureByKey("key-myRef"), node24);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        Assert.ThrowsExactly<VersionMismatchException>(() => serializer.SerializeToChunk([node23, node24]));
    }

    [TestMethod]
    public void SerializeInstancesFromDifferentVersions_Compatible()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var lang24 = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        lang24.Version = "xx2024.1";

        var concept23 = lang23.ClassifierByKey("key-myConcept");
        var concept24 = lang24.ClassifierByKey("key-myConcept");

        var node23 = lang23.GetFactory().CreateNode(NextId("2023_1_inst_"), concept23);
        var node24 = lang24.GetFactory().CreateNode(NextId("2024_1_inst_"), concept24);

        node23.Set(concept23.FeatureByKey("key-myRef"), node24);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1_Compatible)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        var chunk = serializer.SerializeToChunk([node23, node24]);

        Assert.AreEqual("2024.1", chunk.SerializationFormatVersion);
    }

    [TestMethod]
    public void DeserializeInstancesFromDifferentVersions()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var lang24 = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        lang24.Version = "xx2024.1";

        Assert.ThrowsExactly<VersionMismatchException>(() =>
            new DeserializerBuilder()
                .WithLanguage(lang23)
                .WithLanguage(lang24)
                .WithLionWebVersion(LionWebVersions.v2023_1)
                .WithCompressedIds(new(KeepOriginal: true))
                .Build()
        );
    }

    [TestMethod]
    public void DeserializeInstancesFromDifferentVersions_24_Compatible()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var lang24 = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        lang24.Version = "xx2024.1";

        var deserializer = new DeserializerBuilder()
            .WithLanguage(lang23)
            .WithLanguage(lang24)
            .WithLionWebVersion(LionWebVersions.v2024_1_Compatible)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        var chunk = new SerializationChunk
        {
            SerializationFormatVersion = "2024.1",
            Languages =
            [
                new SerializedLanguageReference { Key = lang23.Key, Version = lang23.Version },
                new SerializedLanguageReference { Key = lang24.Key, Version = lang24.Version }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "a",
                    Classifier = new MetaPointer(lang23.Key, lang23.Version, "key-myConcept"),
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer(lang23.Key, lang23.Version, "key-myRef"),
                            Targets = [new SerializedReferenceTarget { Reference = "b" }]
                        }
                    ],
                    Properties = [],
                    Containments = [],
                    Annotations = []
                },
                new SerializedNode
                {
                    Id = "b",
                    Classifier = new MetaPointer(lang24.Key, lang24.Version, "key-myConcept"),
                    References = [],
                    Properties = [],
                    Containments = [],
                    Annotations = []
                }
            ]
        };

        var nodes = deserializer.Deserialize(chunk);
        Assert.AreEqual(2, nodes.Count);
    }

    [TestMethod]
    public void DeserializeInstancesFromDifferentVersions_23_Compatible()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var lang24 = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        lang24.Version = "xx2024.1";

        var deserializer = new DeserializerBuilder()
            .WithLanguage(lang23)
            .WithLanguage(lang24)
            .WithLionWebVersion(LionWebVersions.v2024_1_Compatible)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        var chunk = new SerializationChunk
        {
            SerializationFormatVersion = "2023.1",
            Languages =
            [
                new SerializedLanguageReference { Key = lang23.Key, Version = lang23.Version },
                new SerializedLanguageReference { Key = lang24.Key, Version = lang24.Version }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "a",
                    Classifier = new MetaPointer(lang23.Key, lang23.Version, "key-myConcept"),
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer(lang23.Key, lang23.Version, "key-myRef"),
                            Targets = [new SerializedReferenceTarget { Reference = "b" }]
                        }
                    ],
                    Properties = [],
                    Containments = [],
                    Annotations = []
                },
                new SerializedNode
                {
                    Id = "b",
                    Classifier = new MetaPointer(lang24.Key, lang24.Version, "key-myConcept"),
                    References = [],
                    Properties = [],
                    Containments = [],
                    Annotations = []
                }
            ]
        };

        var nodes = deserializer.Deserialize(chunk);
        Assert.AreEqual(2, nodes.Count);
    }

    [TestMethod]
    public void DeserializeStream_FormatStart()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var deserializer = new DeserializerBuilder()
            .WithLanguage(lang23)
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        var stream = new MemoryStream();

        var chunk = new SerializationChunk { SerializationFormatVersion = "2024.1", Languages = [], Nodes = [] };
        JsonSerializer.Serialize(stream, chunk,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

        stream.Seek(0, SeekOrigin.Begin);

        Assert.ThrowsExactly<VersionMismatchException>(() => JsonUtils.ReadNodesFromStream(stream, deserializer));
    }

    [TestMethod]
    public void DeserializeStream_FormatEnd()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var deserializer = new DeserializerBuilder()
            .WithLanguage(lang23)
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        var stream = new MemoryStream();

        var chunk = new LazySerializationChunk
        {
            SerializationFormatVersion = "2024.1",
            Languages =
            [
                new SerializedLanguageReference { Key = lang23.Key, Version = lang23.Version },
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "a",
                    Classifier = new MetaPointer(lang23.Key, lang23.Version, "key-myConcept"),
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer(lang23.Key, lang23.Version, "key-myRef"),
                            Targets = [new SerializedReferenceTarget { Reference = "b" }]
                        }
                    ],
                    Properties = [],
                    Containments = [],
                    Annotations = []
                },
                new SerializedNode
                {
                    Id = "b",
                    Classifier = new MetaPointer(lang23.Key, lang23.Version, "key-myConcept"),
                    References = [],
                    Properties = [],
                    Containments = [],
                    Annotations = []
                }
            ]
        };
        JsonSerializer.Serialize(stream, chunk,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

        stream.Seek(0, SeekOrigin.Begin);

        Assert.ThrowsExactly<VersionMismatchException>(() => JsonUtils.ReadNodesFromStream(stream, deserializer));
    }
}