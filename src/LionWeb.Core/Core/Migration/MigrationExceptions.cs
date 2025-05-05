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

/// Common base type for all migration exceptions.
public abstract class MigrationExceptionBase : LionWebExceptionBase
{
    /// <inheritdoc />
    protected MigrationExceptionBase(string? message) : base(message)
    {
    }

    /// <inheritdoc />
    protected MigrationExceptionBase(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <inheritdoc />
    protected MigrationExceptionBase(string? message, string? paramName) : base(message, paramName)
    {
    }

    /// <inheritdoc />
    protected MigrationExceptionBase(string? message, string? paramName, Exception? innerException) : base(message,
        paramName, innerException)
    {
    }
}

/// <see cref="IModelMigrator"/> didn't finish after <paramref name="maxMigrationRounds"/> rounds.
public class MaxMigrationRoundsExceededException(int maxMigrationRounds)
    : MigrationExceptionBase($"Exceeded {maxMigrationRounds} migration rounds.");

/// <see cref="IMigration"/> resulted in invalid root nodes.
/// <seealso cref="MigrationResult.OutputRootNodes"/>
public class InvalidRootNodesException(IMigration migration, string message)
    : MigrationExceptionBase($"migration {migration} resulted in invalid root nodes: {message}");

/// <see cref="MigrationResult"/> is inconsistent.
public class InvalidMigrationResultException(string message)
    : MigrationExceptionBase(message);

/// Not exactly one language could be mapped to the same <paramref name="key"/>.
public class AmbiguousLanguageKeyMapping(string key, IEnumerable<DynamicLanguage> languages)
    : MigrationExceptionBase($"Not exactly one mapped language for key {key}: {string.Join(", ", languages.Select(l => $"({l.Key}, {l.Version})"))}");

/// Lookup of a <see cref="IKeyed"/> language element failed. 
public class UnknownLookupException : MigrationExceptionBase
{
    /// No equivalent can be found for <paramref name="key"/>.
    public UnknownLookupException(string key, LanguageIdentity languageIdentity) : base($"No lookup found for {languageIdentity}, key {key}") { }
    
    /// No equivalent can be found for <paramref name="key"/>.
    public UnknownLookupException(string key) : base($"No lookup found for key {key}") { }

    /// No equivalent can be found for <paramref name="keyed"/>.
    public UnknownLookupException(IKeyed keyed) : base($"No lookup found for key {keyed.Key}: {keyed}") { }
}

/// Migration encountered an unexpected state.
public class IllegalMigrationStateException(string message) : MigrationExceptionBase(message);