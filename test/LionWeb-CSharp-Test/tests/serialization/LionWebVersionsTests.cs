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

[TestClass]
public class LionWebVersionsTests
{
    int id = 0;

    [TestMethod]
    [DataRow("2023.1")]
    [DataRow("2024.1")]
    public void SameVersion(string versionString)
    {
        var lionWebVersion = LionWebVersions.GetByVersionString(versionString);

        DynamicLanguage language = Serialize(lionWebVersion, out INode conceptNodeA, out INode conceptNodeB,
            out SerializationChunk chunk);

        List<IReadableNode> nodes = Deserialize(language, lionWebVersion, chunk);

        var differences = new Comparer([conceptNodeA, conceptNodeB], nodes).Compare();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
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
    [DataRow("2023.1")]
    [DataRow("2024.1")]
    public void SameVersion_Language(string versionString)
    {
        var lionWebVersion = LionWebVersions.GetByVersionString(versionString);

        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);
        SerializationChunk chunk =
            new Serializer(lionWebVersion) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        var versionSpecifics = IDeserializerVersionSpecifics.Create(lionWebVersion);
        var deserialized = new LanguageDeserializer(versionSpecifics) { StoreUncompressedIds = true }
            .Deserialize(chunk, new Language[0]).Cast<IReadableNode>()
            .ToList();

        Assert.AreEqual(1, deserialized.Count);

        var differences = new Comparer([language], deserialized).Compare();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    [TestMethod]
    public void Serialize23_Deserialize24_Language()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        SerializationChunk chunk =
            new Serializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        var versionSpecifics = IDeserializerVersionSpecifics.Create(LionWebVersions.v2024_1);
        var deserialized = new LanguageDeserializer(versionSpecifics) { StoreUncompressedIds = true }
            .Deserialize(chunk, new Language[0])
            .Cast<IReadableNode>().ToList();

        Assert.AreEqual(1, deserialized.Count);

        var differences = new Comparer([language], deserialized).Compare();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    [TestMethod]
    public void Serialize24_Deserialize23_Language()
    {
        var language = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        SerializationChunk chunk =
            new Serializer(LionWebVersions.v2024_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        var versionSpecifics = IDeserializerVersionSpecifics.Create(LionWebVersions.v2023_1);
        var deserialized = new LanguageDeserializer(versionSpecifics) { StoreUncompressedIds = true }
            .Deserialize(chunk, new Language[0])
            .Cast<IReadableNode>().ToList();

        Assert.AreEqual(1, deserialized.Count);

        var differences = new Comparer([language], deserialized).Compare();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    [TestMethod]
    public void Language23_Chunk24()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);
        SerializationChunk chunk =
            new Serializer(LionWebVersions.v2024_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        var versionSpecifics = IDeserializerVersionSpecifics.Create(LionWebVersions.v2024_1);
        var deserialized = new LanguageDeserializer(versionSpecifics) { StoreUncompressedIds = true }
            .Deserialize(chunk, new Language[0])
            .Cast<IReadableNode>().ToList();

        Assert.AreEqual(1, deserialized.Count);

        var differences = new Comparer([language], deserialized).Compare();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    [TestMethod]
    public void Language24_Chunk23()
    {
        var language = CreateLanguage("2024_1_", LionWebVersions.v2024_1);
        SerializationChunk chunk =
            new Serializer(LionWebVersions.v2023_1) { StoreUncompressedIds = true }.SerializeToChunk(
                language.Descendants(true, true));

        var versionSpecifics = IDeserializerVersionSpecifics.Create(LionWebVersions.v2023_1);
        var deserialized = new LanguageDeserializer(versionSpecifics) { StoreUncompressedIds = true }
            .Deserialize(chunk, new Language[0])
            .Cast<IReadableNode>().ToList();

        Assert.AreEqual(1, deserialized.Count);

        var differences = new Comparer([language], deserialized).Compare();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

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
        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);
        var myConcept = language.ClassifierByKey("key-myConcept") as Concept;
        var myAnnotation = language.ClassifierByKey("key-myAnnotation") as Annotation;

        var factory = language.GetFactory();

        conceptNodeA = factory.CreateNode(NextId("n"), myConcept);
        // conceptNodeA.Set(lionWebVersion.BuiltIns.INamed_name, "A");

        conceptNodeB = factory.CreateNode(NextId("n"), myConcept);
        // conceptNodeB.Set(lionWebVersion.BuiltIns.INamed_name, "B");
        conceptNodeB.Set(myConcept.FeatureByKey("key-myRef"), conceptNodeA);

        var annotation = factory.CreateNode(NextId("n"), myAnnotation);
        conceptNodeA.AddAnnotations([annotation]);

        List<IReadableNode> input = [conceptNodeA, conceptNodeB, annotation];
        chunk = new Serializer(lionWebVersion) { StoreUncompressedIds = true }.SerializeToChunk(input);
        return language;
    }

    private DynamicLanguage CreateLanguage(string idBase, LionWebVersions lionWebVersion)
    {
        var language =
            new DynamicLanguage(NextId(idBase), lionWebVersion)
            {
                Key = "key-myLanguage", Version = "1", Name = "myLanguage"
            };
        var myConcept = language.Concept(NextId(idBase), "key-myConcept", "myConcept");
        // .Implementing(lionWebVersion.BuiltIns.INamed);
        var myAnnotation = language.Annotation(NextId(idBase), "key-myAnnotation", "myAnnotation")
            .Annotating(myConcept);
        myConcept.Reference(NextId(idBase), "key-myRef", "myRef")
            .OfType(lionWebVersion.BuiltIns.Node)
            .IsOptional(true)
            .IsMultiple(false);

        return language;
    }

    private string NextId(string idBase) => idBase + id++;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext { get; set; }
}