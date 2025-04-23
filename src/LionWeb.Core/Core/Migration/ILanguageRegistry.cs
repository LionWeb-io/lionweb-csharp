// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Migration;

using M3;
using System.Diagnostics.CodeAnalysis;

/// Provides access to all languages known during the current <see cref="IModelMigrator">migration round</see>.
public interface ILanguageRegistry
{
    /// Version of LionWeb standard used for migration.
    LionWebVersions LionWebVersion { get; }
    
    /// Enumerates all languages known in the current migration round.
    IEnumerable<DynamicLanguage> KnownLanguages { get; }
    
    /// Find language for <paramref name="languageIdentity"/>, if known.
    /// <returns><c>true</c> if a language for <paramref name="languageIdentity"/> could be found; <c>false</c> otherwise.</returns>
    bool TryGetLanguage(LanguageIdentity languageIdentity, [NotNullWhen(true)] out DynamicLanguage? language);
    
    /// Adds a new language to the current migration round.
    /// Uses <see cref="LanguageIdentity.FromLanguage"/> if <paramref name="languageIdentity"/> is <c>null</c>.
    /// <returns><c>true</c> if the language has been added to the list of languages;
    /// <c>false</c> if a language with <paramref name="languageIdentity"/> is already present.</returns>
    bool RegisterLanguage(DynamicLanguage language, LanguageIdentity? languageIdentity = null);

    /// Finds the equivalent of <paramref name="keyed"/> within the current round's languages.
    /// <exception cref="UnknownLookupException">If no equivalent can be found for <paramref name="keyed"/>.</exception>
    /// <exception cref="DuplicateLanguageKeyMapping">If more than one language could be mapped to the same key.</exception>
    T Lookup<T>(T keyed) where T : IKeyed;
}