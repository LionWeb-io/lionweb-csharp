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

namespace LionWeb.Core.M2;

using M1;
using M3;
using Serialization;

/// <inheritdoc />
public interface ILanguageDeserializer : IDeserializer<DynamicLanguage>
{
    void RegisterDependentLanguage(Language language);
}

public static class ILanguageDeserializerExtensions
{
    /// <summary>
    /// Deserializes the given <paramref name="serializationChunk">serialization chunk</paramref> as an iterable collection of <see cref="Language"/>s.
    /// The <paramref name="dependentLanguages">dependent languages</paramref> should contain all languages that are referenced by the top-level
    /// <c>languages</c> property of the serialization chunk.
    /// </summary>
    /// 
    /// <returns>The deserialization of the language definitions present in the given <paramref name="serializationChunk"/>.</returns>
    public static IEnumerable<DynamicLanguage> Deserialize(this ILanguageDeserializer deserializer,
        SerializationChunk serializationChunk,
        params Language[] dependentLanguages)
    {
        foreach (var dependentLanguage in dependentLanguages)
        {
            deserializer.RegisterDependentLanguage(dependentLanguage);
        }

        foreach (var serializedNode in serializationChunk.Nodes)
        {
            deserializer.Process(serializedNode);
        }

        return deserializer.Finish();
    }
    
    /// <inheritdoc cref="ILanguageDeserializerExtensions.Deserialize"/>
    public static IEnumerable<DynamicLanguage> Deserialize(SerializationChunk serializationChunk, params Language[] dependentLanguages) =>
        Deserialize(serializationChunk, LionWebVersions.Current, dependentLanguages);

    /// <inheritdoc cref="ILanguageDeserializerExtensions.Deserialize"/>
    public static IEnumerable<DynamicLanguage> Deserialize(SerializationChunk serializationChunk,
        LionWebVersions lionWebVersion, params Language[] dependentLanguages)
    {
        var versionSpecifics = IDeserializerVersionSpecifics.Create(lionWebVersion);
        return new LanguageDeserializer(versionSpecifics).Deserialize(serializationChunk, dependentLanguages);
    }
}