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

using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;
using LionWeb.Core.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

[TestClass]
public class LionWebVersionsTests
{
    int id = 0;

    #region Instance

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

        Assert.ThrowsException<VersionMismatchException>(() => Deserialize(language, LionWebVersions.v2024_1, chunk));
    }

    [TestMethod]
    public void Serialize24_Deserialize23()
    {
        DynamicLanguage language = Serialize(LionWebVersions.v2024_1, out INode conceptNodeA, out INode conceptNodeB,
            out SerializationChunk chunk);

        Assert.ThrowsException<VersionMismatchException>(() => Deserialize(language, LionWebVersions.v2023_1, chunk));
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

        Assert.ThrowsException<VersionMismatchException>(() =>
            new Serializer(LionWebVersions.v2024_1) { StoreUncompressedIds = true }.SerializeToChunk(input)
        );
    }

    [TestMethod]
    public void InstanceLanguage23_Chunk24_Compatible()
    {
        CreateInstances(LionWebVersions.v2023_1, out _, out _, out var input);

        var chunk =
            new Serializer(LionWebVersions.v2024_1_Compatible) { StoreUncompressedIds = true }.SerializeToChunk(input);
        Assert.AreEqual("2024.1", chunk.SerializationFormatVersion);
    }

    [TestMethod]
    public void InstanceLanguage24_Chunk23()
    {
        CreateInstances(LionWebVersions.v2024_1, out _, out _, out var input);

        Assert.ThrowsException<VersionMismatchException>(() =>
            new Serializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.SerializeToChunk(input)
        );
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

        Assert.ThrowsException<VersionMismatchException>(() =>
            new Serializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.SerializeToChunk([node23, node24])
        );
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

        var chunk = new Serializer(LionWebVersions.v2024_1_Compatible) { StoreUncompressedIds = true }
            .SerializeToChunk([node23, node24]);

        Assert.AreEqual("2024.1", chunk.SerializationFormatVersion);
    }

    [TestMethod]
    public void DeserializeInstancesFromDifferentVersions()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var lang24 = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        lang24.Version = "xx2024.1";

        Assert.ThrowsException<VersionMismatchException>(() =>
            new DeserializerBuilder()
                .WithLanguage(lang23)
                .WithLanguage(lang24)
                .WithLionWebVersion(LionWebVersions.v2023_1)
                .WithUncompressedIds(true)
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
            .WithUncompressedIds(true)
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
            .WithUncompressedIds(true)
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
            .WithUncompressedIds(true)
            .Build();

        var stream = new MemoryStream();

        var chunk = new SerializationChunk { SerializationFormatVersion = "2024.1", Languages = [], Nodes = [] };
        JsonSerializer.Serialize(stream, chunk,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

        stream.Seek(0, SeekOrigin.Begin);

        try
        {
            var x = JsonUtils.ReadNodesFromStreamAsync(stream, deserializer).Result;
        } catch (AggregateException e)
        {
            Assert.IsInstanceOfType<VersionMismatchException>(e.InnerExceptions.First());
        }
    }

    [TestMethod]
    public void DeserializeStream_FormatEnd()
    {
        var lang23 = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        lang23.Version = "xx2023.1";

        var deserializer = new DeserializerBuilder()
            .WithLanguage(lang23)
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithUncompressedIds(true)
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

        try
        {
            var x = JsonUtils.ReadNodesFromStreamAsync(stream, deserializer).Result;
        } catch (AggregateException e)
        {
            Assert.IsInstanceOfType<VersionMismatchException>(e.InnerExceptions.First());
        }
    }

    #endregion

    #region Language

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SameVersion_Language(Type versionIface)
    {
        var lionWebVersion = LionWebVersions.GetByInterface(versionIface);

        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);
        SerializationChunk chunk = new Serializer(lionWebVersion)
        {
            StoreUncompressedIds = true, PersistLionCoreReferenceTargetIds = false
        }.SerializeToChunk(language.Descendants(true, true));

        foreach (var target in chunk
                     .Nodes
                     .SelectMany(n => n.References)
                     .SelectMany(r => r.Targets)
                     .Where(t => chunk.Nodes.All(n => n.Id != t.Reference))
                )
        {
            Assert.IsNull(target.Reference);
            Assert.IsNotNull(target.ResolveInfo);
        }

        var deserialized = new LanguageDeserializer(lionWebVersion) { StoreUncompressedIds = true }
            .Deserialize(chunk).Cast<IReadableNode>()
            .ToList();

        Assert.AreEqual(1, deserialized.Count);

        AssertEquals(language, deserialized);
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SameVersion_Language_WithIds(Type versionIface)
    {
        var lionWebVersion = LionWebVersions.GetByInterface(versionIface);

        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);
        SerializationChunk chunk = new Serializer(lionWebVersion)
        {
            StoreUncompressedIds = true, PersistLionCoreReferenceTargetIds = true
        }.SerializeToChunk(language.Descendants(true, true));

        foreach (var target in chunk
                     .Nodes
                     .SelectMany(n => n.References)
                     .SelectMany(r => r.Targets)
                )
        {
            Assert.IsNotNull(target.Reference);
            Assert.IsNotNull(target.ResolveInfo);
        }

        var deserialized = new LanguageDeserializer(lionWebVersion) { StoreUncompressedIds = true }
            .Deserialize(chunk).Cast<IReadableNode>()
            .ToList();

        Assert.AreEqual(1, deserialized.Count);

        AssertEquals(language, deserialized);
    }

    [TestMethod]
    public void Serialize23_Deserialize24_Language()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        SerializationChunk chunk =
            new Serializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        Assert.ThrowsException<VersionMismatchException>(() =>
            new LanguageDeserializer(LionWebVersions.v2024_1) { StoreUncompressedIds = true }.Deserialize(chunk)
        );
    }

    [TestMethod]
    public void Serialize23_Deserialize24_Compatible_Language()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        SerializationChunk chunk =
            new Serializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        var deserialized = new LanguageDeserializer(LionWebVersions.v2024_1_Compatible) { StoreUncompressedIds = true }
            .Deserialize(chunk)
            .Cast<IReadableNode>()
            .ToList();

        Assert.AreEqual(1, deserialized.Count);

        var expected = CreateLanguage("2024_1_", LionWebVersions.v2024_1);

        AssertEquals(expected, deserialized);
    }

    [TestMethod]
    public void Serialize24_Deserialize23_Language()
    {
        var language = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        SerializationChunk chunk =
            new Serializer(LionWebVersions.v2024_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        Assert.ThrowsException<VersionMismatchException>(() =>
            new LanguageDeserializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.Deserialize(chunk)
        );
    }

    [TestMethod]
    public void Language23_Chunk24()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);

        Assert.ThrowsException<VersionMismatchException>(() =>
            new Serializer(LionWebVersions.v2024_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true))
        );
    }

    [TestMethod]
    public void Language24_Chunk23()
    {
        var language = CreateLanguage("2024_1_", LionWebVersions.v2024_1);

        Assert.ThrowsException<VersionMismatchException>(() =>
            new Serializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true))
        );
    }

    #endregion

    private List<IReadableNode> Deserialize(DynamicLanguage language, LionWebVersions lionWebVersion,
        SerializationChunk chunk)
    {
        var deserializer = new DeserializerBuilder()
            .WithLanguage(language)
            .WithLionWebVersion(lionWebVersion)
            .WithUncompressedIds(true)
            .Build();
        List<IReadableNode> nodes = deserializer.Deserialize(chunk);
        return nodes;
    }

    private DynamicLanguage Serialize(LionWebVersions lionWebVersion, out INode conceptNodeA, out INode conceptNodeB,
        out SerializationChunk chunk)
    {
        DynamicLanguage language = CreateInstances(lionWebVersion, out conceptNodeA, out conceptNodeB,
            out List<IReadableNode> input);
        chunk = new Serializer(lionWebVersion) { StoreUncompressedIds = true }.SerializeToChunk(input);
        return language;
    }

    private DynamicLanguage CreateInstances(LionWebVersions lionWebVersion, out INode conceptNodeA,
        out INode conceptNodeB,
        out List<IReadableNode> input)
    {
        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);
        var myConcept = language.ClassifierByKey("key-myConcept") as Concept;
        var myAnnotation = language.ClassifierByKey("key-myAnnotation") as Annotation;

        var factory = language.GetFactory();

        conceptNodeA = factory.CreateNode(NextId("n"), myConcept);
        conceptNodeA.Set(lionWebVersion.BuiltIns.INamed_name, "A");

        conceptNodeB = factory.CreateNode(NextId("n"), myConcept);
        conceptNodeB.Set(lionWebVersion.BuiltIns.INamed_name, "B");
        conceptNodeB.Set(myConcept.FeatureByKey("key-myRef"), conceptNodeA);

        var annotation = factory.CreateNode(NextId("n"), myAnnotation);
        conceptNodeA.AddAnnotations([annotation]);

        input = [conceptNodeA, conceptNodeB, annotation];
        return language;
    }

    private DynamicLanguage CreateLanguage(string idBase, LionWebVersions lionWebVersion)
    {
        var language =
            new DynamicLanguage(NextId(idBase), lionWebVersion)
            {
                Key = "key-myLanguage", Version = "1", Name = "myLanguage"
            };
        var myConcept = language.Concept(NextId(idBase), "key-myConcept", "myConcept")
            .Implementing(lionWebVersion.BuiltIns.INamed);
        var myAnnotation = language.Annotation(NextId(idBase), "key-myAnnotation", "myAnnotation")
            .Annotating(myConcept);
        myConcept.Reference(NextId(idBase), "key-myRef", "myRef")
            .OfType(lionWebVersion.BuiltIns.Node)
            .IsOptional(true)
            .IsMultiple(false);

        return language;
    }

    private string NextId(string idBase) => idBase + id++;

    private static void AssertEquals(List<IReadableNode?> expected, List<IReadableNode> nodes)
    {
        var differences = new Comparer(expected, nodes!).Compare().ToList();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    private static void AssertEquals(DynamicLanguage language, List<IReadableNode> deserialized)
    {
        var differences = new Comparer([language], deserialized!).Compare().ToList();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext { get; set; }
}

internal class LazySerializationChunk
{
    /// <inheritdoc cref="SerializationChunk.SerializationFormatVersion"/>
    [JsonPropertyOrder(0)]
    public string SerializationFormatVersion { get; init; }

    /// <inheritdoc cref="SerializationChunk.Nodes"/>
    [JsonPropertyOrder(1)]
    public IEnumerable<SerializedNode> Nodes { get; init; }

    /// <inheritdoc cref="SerializationChunk.Languages"/>
    [JsonPropertyOrder(2)]
    public IEnumerable<SerializedLanguageReference> Languages { get; init; }
}