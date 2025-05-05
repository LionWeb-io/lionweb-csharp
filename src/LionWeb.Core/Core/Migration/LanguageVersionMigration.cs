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

/// Bumps all languages mentioned in <paramref name="versionMappings"/>
/// from <see cref="VersionMapping.FromVersion"/> to <see cref="VersionMapping.ToVersion"/>.
public class LanguageVersionMigration(ISet<LanguageVersionMigration.VersionMapping> versionMappings) : IMigration
{
    /// Describes a language version mapping.
    public record VersionMapping
    {
        /// Describes a language version mapping.
        /// <param name="Key"><see cref="IKeyed.Key"/> of the language to migrate.</param>
        /// <param name="FromVersion"><see cref="Language.Version"/> of the language to migrate <i>from</i>.</param>
        /// <param name="ToVersion"><see cref="Language.Version"/> of the language to migrate <i>to</i>.</param>
        /// <exception cref="IllegalMigrationStateException">If <paramref name="FromVersion"/> == <paramref name="ToVersion"/>.</exception>
        public VersionMapping(string Key, string FromVersion, string ToVersion)
        {
            if (FromVersion == ToVersion)
                throw new IllegalMigrationStateException(
                    $"For Key = '{Key}': FromVersion is same as ToVersion: '{FromVersion}'");

            this.Key = Key;
            this.FromVersion = FromVersion;
            this.ToVersion = ToVersion;
        }

        /// <param name="toLanguage">Language to migrate <i>to</i>.</param>
        /// <param name="fromVersion"><see cref="Language.Version"/> of the language to migrate <i>from</i>.</param>
        public VersionMapping(Language toLanguage, string fromVersion)
            : this(toLanguage.Key, fromVersion, toLanguage.Version)
        { }

        /// <param name="toLanguage">Language to migrate <i>to</i>.</param>
        /// <param name="fromVersion"><see cref="Language.Version"/> of the language to migrate <i>from</i>.</param>
        public VersionMapping(LanguageIdentity toLanguage, string fromVersion)
            : this(toLanguage.Key, fromVersion, toLanguage.Version)
        { }

        /// <see cref="IKeyed.Key"/> of the language to migrate.
        public string Key { get; init; }

        /// <see cref="Language.Version"/> of the language to migrate <i>from</i>.
        public string FromVersion { get; init; }

        /// <see cref="Language.Version"/> of the language to migrate <i>to</i>.
        public string ToVersion { get; init; }
    };

    /// <inheritdoc />
    public required int Priority { get; init; }

    /// <inheritdoc />
    public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
        languageIdentities.Any(l => TryGetMappedLanguage(l, out _));

    /// <inheritdoc />
    public void Initialize(ILanguageRegistry languageRegistry) { }

    /// <inheritdoc />
    public MigrationResult Migrate(List<LenientNode> inputRootNodes)
    {
        var result = false;

        var languages = MigrationExtensions.CollectUsedLanguages(inputRootNodes);

        foreach (var language in languages)
        {
            result |= ReplaceVersion(language);
        }

        return new MigrationResult(result, inputRootNodes);
    }

    private bool ReplaceVersion(DynamicLanguage language)
    {
        if (TryGetMappedLanguage(language, out var mapping))
        {
            language.Version = mapping.ToVersion;
            return true;
        }

        return false;
    }

    private bool TryGetMappedLanguage(Language language, [NotNullWhen(true)] out VersionMapping? versionMapping) =>
        TryGetMappedLanguage(LanguageIdentity.FromLanguage(language), out versionMapping);

    private bool TryGetMappedLanguage(LanguageIdentity languageIdentity,
        [NotNullWhen(true)] out VersionMapping? versionMapping)
    {
        versionMapping = versionMappings.FirstOrDefault(m =>
            languageIdentity.Key == m.Key && languageIdentity.Version == m.FromVersion);

        return versionMapping != null;
    }
}