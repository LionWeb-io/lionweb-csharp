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

namespace LionWeb.Core.M3;

using M2;
using Serialization;

/// <summary>
/// Utility methods for working with languages.
/// </summary>
public static class LanguagesUtils
{
    /// <inheritdoc cref="LoadLanguages(string,string, LionWebVersions)"/>
    public static IEnumerable<DynamicLanguage> LoadLanguages(string assemblyName, string resourceName) =>
        LoadLanguages(assemblyName, resourceName, LionWebVersions.Current);

    /// <summary>
    /// Loads the languages defined in the resource with the given <paramref name="resourceName" /> inside the assembly with the given <paramref name="assemblyName" />,
    /// which must be a JSON file in the LionWeb serialization (chunk) format that contains LionCore languages (M2).
    /// </summary>
    public static IEnumerable<DynamicLanguage> LoadLanguages<TVersion>(string assemblyName, string resourceName,
        TVersion lionWebVersion) where TVersion : LionWebVersions
    {
        Stream stream = ResourcesUtils.GetAssemblyByName(assemblyName).GetManifestResourceStream(resourceName) ??
                        throw new ArgumentException($"Cannot read resource: {resourceName}", nameof(resourceName));
        return JsonUtils.ReadNodesFromStream(stream, new LanguageDeserializer<TVersion>(lionWebVersion)).GetAwaiter().GetResult()
            .Cast<DynamicLanguage>();
    }
}