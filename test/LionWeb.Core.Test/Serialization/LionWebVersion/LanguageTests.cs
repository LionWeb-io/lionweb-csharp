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
using LionWebVersions = LionWebVersions;

[TestClass]
public class LanguageTests : LionWebVersionsTestBase
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SameVersion(Type versionIface)
    {
        var lionWebVersion = LionWebVersions.GetByInterface(versionIface);

        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .WithPersistedLionCoreReferenceTargetIds(false)
            .Build();

        SerializationChunk chunk = serializer.SerializeToChunk(language.Descendants(true, true));

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

        var deserialized = new LanguageDeserializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build()
            .Deserialize(chunk).Cast<IReadableNode>()
            .ToList();

        Assert.AreEqual(1, deserialized.Count);

        AssertEquals(language, deserialized);
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SameVersion_WithIds(Type versionIface)
    {
        var lionWebVersion = LionWebVersions.GetByInterface(versionIface);

        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .WithPersistedLionCoreReferenceTargetIds(true)
            .Build();

        SerializationChunk chunk = serializer.SerializeToChunk(language.Descendants(true, true));

        foreach (var target in chunk
                     .Nodes
                     .SelectMany(n => n.References)
                     .SelectMany(r => r.Targets)
                )
        {
            Assert.IsNotNull(target.Reference);
            Assert.IsNotNull(target.ResolveInfo);
        }

        var deserialized = new LanguageDeserializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build()
            .Deserialize(chunk).Cast<IReadableNode>()
            .ToList();

        Assert.AreEqual(1, deserialized.Count);

        AssertEquals(language, deserialized);
    }

    [TestMethod]
    public void Serialize23_Deserialize24()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        SerializationChunk chunk = serializer.SerializeToChunk(language.Descendants(true, true));

        Assert.ThrowsException<VersionMismatchException>(() => new LanguageDeserializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build()
            .Deserialize(chunk)
        );
    }

    [TestMethod]
    public void Serialize23_Deserialize24_Compatible()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        SerializationChunk chunk = serializer.SerializeToChunk(language.Descendants(true, true));

        var deserialized = new LanguageDeserializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1_Compatible)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build()
            .Deserialize(chunk)
            .Cast<IReadableNode>()
            .ToList();

        Assert.AreEqual(1, deserialized.Count);

        var expected = CreateLanguage("2024_1_", LionWebVersions.v2024_1);

        AssertEquals(expected, deserialized);
    }

    [TestMethod]
    public void Serialize24_Deserialize23()
    {
        var language = CreateLanguage("2024_1_", LionWebVersions.v2024_1);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        SerializationChunk chunk = serializer.SerializeToChunk(language.Descendants(true, true));

        Assert.ThrowsException<VersionMismatchException>(() => new LanguageDeserializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build()
            .Deserialize(chunk)
        );
    }

    [TestMethod]
    public void Language23_Chunk24()
    {
        var language = CreateLanguage("2023_1_", LionWebVersions.v2023_1);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2024_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        Assert.ThrowsException<VersionMismatchException>(() =>
            serializer.SerializeToChunk(language.Descendants(true, true))
        );
    }

    [TestMethod]
    public void Language24_Chunk23()
    {
        var language = CreateLanguage("2024_1_", LionWebVersions.v2024_1);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        Assert.ThrowsException<VersionMismatchException>(() =>
            serializer.SerializeToChunk(language.Descendants(true, true))
        );
    }
}