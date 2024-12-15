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

/// <summary>Deserializes languages (M2).</summary>
public interface ILanguageDeserializer : IDeserializer<DynamicLanguage>
{
    /// Specialization of <see cref="IDeserializer.RegisterDependentNodes"/> that registers all of <paramref name="language"/>'s members,
    /// and enables annotations from these languages to be instantiated.
    void RegisterDependentLanguage(Language language);
}

/// Extensions for <see cref="ILanguageDeserializer"/>.
public static class ILanguageDeserializerExtensions
{
    /// <summary>
    /// Deserializes the given <paramref name="serializationChunk"/> as an iterable collection of <see cref="Language"/>s.
    /// </summary>
    /// <param name="deserializer">Language deserializer to use.</param>
    /// <param name="serializationChunk">Chunk to deserialize.</param>
    /// <param name="dependentLanguages"><see cref="ILanguageDeserializer.RegisterDependentLanguage">Dependent languages</see> to register.</param>
    /// <returns>The deserialization of the language definitions present in the given <paramref name="serializationChunk"/>.</returns>
    /// <exception cref="VersionMismatchException">
    /// If <paramref name="serializationChunk"/>'s LionWeb version is not compatible with
    /// <paramref name="deserializer"/>'s <see cref="IDeserializer.LionWebVersion"/>.
    /// </exception>
    /// <exception cref="DeserializerException"/>
    public static IEnumerable<DynamicLanguage> Deserialize(this ILanguageDeserializer deserializer,
        SerializationChunk serializationChunk,
        IEnumerable<Language>? dependentLanguages = null)
    {
        deserializer.LionWebVersion.AssureCompatible(serializationChunk.SerializationFormatVersion);

        foreach (var dependentLanguage in dependentLanguages ?? [])
        {
            deserializer.RegisterDependentLanguage(dependentLanguage);
        }

        foreach (var serializedNode in serializationChunk.Nodes)
        {
            deserializer.Process(serializedNode);
        }

        return deserializer.Finish();
    }

    /// <summary>
    /// Deserializes the given <paramref name="serializationChunk"/> as an iterable collection of <see cref="Language"/>s.
    /// </summary>
    /// <param name="serializationChunk">Chunk to deserialize.</param>
    /// <param name="dependentLanguages"><see cref="ILanguageDeserializer.RegisterDependentLanguage">Dependent languages</see> to register.</param>
    /// <returns>The deserialization of the language definitions present in the given <paramref name="serializationChunk"/>.</returns>
    /// <exception cref="VersionMismatchException">
    /// If <paramref name="serializationChunk"/>'s LionWeb version is not compatible with <see cref="LionWebVersions.Current"/>.
    /// </exception>
    /// <exception cref="DeserializerException"/>
    public static IEnumerable<DynamicLanguage> Deserialize(SerializationChunk serializationChunk,
        IEnumerable<Language>? dependentLanguages = null) =>
        Deserialize(serializationChunk, LionWebVersions.Current, dependentLanguages);

    /// <summary>
    /// Deserializes the given <paramref name="serializationChunk"/> as an iterable collection of <see cref="Language"/>s.
    /// </summary>
    /// <param name="serializationChunk">Chunk to deserialize.</param>
    /// <param name="lionWebVersion">Version of LionWeb standard to use.</param>
    /// <param name="dependentLanguages"><see cref="ILanguageDeserializer.RegisterDependentLanguage">Dependent languages</see> to register.</param>
    /// <returns>The deserialization of the language definitions present in the given <paramref name="serializationChunk"/>.</returns>
    /// <exception cref="VersionMismatchException">
    /// If <paramref name="serializationChunk"/>'s LionWeb version is not compatible <paramref name="lionWebVersion"/>.
    /// </exception>
    /// <exception cref="DeserializerException"/>
    public static IEnumerable<DynamicLanguage> Deserialize(SerializationChunk serializationChunk,
        LionWebVersions lionWebVersion, IEnumerable<Language>? dependentLanguages = null)
    {
        var versionSpecifics = IDeserializerVersionSpecifics.Create(lionWebVersion);
        return new LanguageDeserializer(versionSpecifics).Deserialize(serializationChunk, dependentLanguages);
    }
}