// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Serialization;

using Core.Migration;
using Core.Serialization;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;
using System.Text.Json;

[TestClass]
public class SerializerLanguageReferencesTests : SerializationTestsBase
{
    private static readonly DynamicLanguage _language2023;
    private static readonly IVersion2024_1_Compatible _lwVersion = LionWebVersions.v2024_1_Compatible;

    static SerializerLanguageReferencesTests()
    {
        _language2023 = new DynamicLanguageCloner(LionWebVersions.v2023_1).Clone(Languages.Generated.V2023_1.TestLanguage.TestLanguageLanguage.Instance);
        _language2023.Version = "other";
    }

    private static IEnumerable<object[]> Variants() =>
        [
            [CreateLanguagesAtStart()],
            [CreateLanguagesAtEnd()],
        ];
    
    [DynamicData(nameof(Variants), DynamicDataSourceType.Method)]
    [TestMethod]
    public async Task Async_Continue(object chunk)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, chunk, chunk.GetType(), LionWebJsonSerializerContext.Default);
        stream.Seek(0, SeekOrigin.Begin);

        List<SerializedLanguageReference> languageReferences = [];
        var deserializedNodes = await JsonUtils.ReadNodesFromStreamAsync(stream, CreateDeserializer(), languageReferencesChecker: langRefs =>
        {
            languageReferences.AddRange(langRefs);
            return true;
        });
        
        Assert.HasCount(2, deserializedNodes);
        
        CollectionAssert.AreEqual(CreateLanguages(), languageReferences);
    }

    [DynamicData(nameof(Variants), DynamicDataSourceType.Method)]
    [TestMethod]
    public async Task Async_Stop(object chunk)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, chunk, chunk.GetType(), LionWebJsonSerializerContext.Default);
        stream.Seek(0, SeekOrigin.Begin);

        List<SerializedLanguageReference> languageReferences = [];
        var deserializedNodes = await JsonUtils.ReadNodesFromStreamAsync(stream, CreateDeserializer(), languageReferencesChecker: langRefs =>
        {
            languageReferences.AddRange(langRefs);
            return false;
        });
        
        Assert.IsEmpty(deserializedNodes);
        
        CollectionAssert.AreEqual(CreateLanguages(), languageReferences);
    }

    private static IDeserializer CreateDeserializer() =>
        new DeserializerBuilder()
            .WithLionWebVersion(_lwVersion)
            .WithLanguage(TestLanguageLanguage.Instance)
            .WithLanguage(_language2023)
            .Build();

    private static SerializationChunk CreateLanguagesAtStart()
    {
        var chunk = new SerializationChunk
        {
            SerializationFormatVersion = "2024.1",
            Nodes = CreateNodes(),
            Languages = CreateLanguages()
        };
        return chunk;
    }

    [DynamicData(nameof(Variants), DynamicDataSourceType.Method)]
    [TestMethod]
    public void Sync_Continue(object chunk)
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, chunk, chunk.GetType(), LionWebJsonSerializerContext.Default);
        stream.Seek(0, SeekOrigin.Begin);

        List<SerializedLanguageReference> languageReferences = [];
        var deserializedNodes = JsonUtils.ReadNodesFromStream(stream, CreateDeserializer(), languageReferencesChecker: langRefs =>
        {
            languageReferences.AddRange(langRefs);
            return true;
        });
        
        Assert.HasCount(2, deserializedNodes);
        
        CollectionAssert.AreEqual(CreateLanguages(), languageReferences);
    }

    [DynamicData(nameof(Variants), DynamicDataSourceType.Method)]
    [TestMethod]
    public void Sync_Stop(object chunk)
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, chunk, chunk.GetType(), LionWebJsonSerializerContext.Default);
        stream.Seek(0, SeekOrigin.Begin);

        List<SerializedLanguageReference> languageReferences = [];
        var deserializedNodes = JsonUtils.ReadNodesFromStream(stream, CreateDeserializer(), languageReferencesChecker: langRefs =>
        {
            languageReferences.AddRange(langRefs);
            return false;
        });
        
        Assert.IsEmpty(deserializedNodes);
        
        CollectionAssert.AreEqual(CreateLanguages(), languageReferences);
    }

    private static LazySerializationChunk CreateLanguagesAtEnd()
    {
        var chunk = new LazySerializationChunk
        {
            SerializationFormatVersion = "2024.1",
            Nodes = CreateNodes(),
            Languages = CreateLanguages()
        };
        return chunk;
    }

    private static SerializedNode[] CreateNodes() =>
    [   
        new()
        {
            Id = "root2024",
            Classifier = new MetaPointer("TestLanguage", "0", "LinkTestConcept"),
            Properties = [
                new SerializedProperty
                {
                    Property = new MetaPointer("LionCore-builtins", "2024.1", "LionCore-builtins-INamed-name"),
                    Value = "root2024"
                }
            ]
        },
        new()
        {
            Id = "root2023",
            Classifier = new MetaPointer("TestLanguage", "other", "LinkTestConcept"),
            Properties = [
                new SerializedProperty
                {
                    Property = new MetaPointer("LionCore-builtins", "2023.1", "LionCore-builtins-INamed-name"),
                    Value = "root2023"
                }
            ]
        }
    ];

    private static SerializedLanguageReference[] CreateLanguages() =>
    [
        new() {Key = "TestLanguage", Version = "0"},
        new() {Key = "LionCore-builtins", Version = "2024.1"},
        new() {Key = "TestLanguage", Version = "other"},
        new() {Key = "LionCore-builtins", Version = "2023.1"},
    ];
}