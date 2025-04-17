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

/// Migrates a list of nodes from one state to the next.
/// <seealso cref="MigrationBase{T}"/>
public interface IMigration
{
    /// Default value for <see cref="Priority"/>.
    public const int DefaultPriority = 10_000;

    /// Priority to sort several <see cref="IsApplicable">applicable</see> migrations deterministically.
    /// Defaults to <see cref="DefaultPriority"/>.
    int Priority { get; }

    /// Provides a migration access to the <paramref name="languageRegistry"/> upon <see cref="IModelMigrator.RegisterMigration">registration</see>.
    void Initialize(ILanguageRegistry languageRegistry);

    /// Whether this migration can work on nodes with any of <paramref name="languageIdentities"/>.
    bool IsApplicable(ISet<LanguageIdentity> languageIdentities);

    /// <summary>
    /// Runs the actual migration.
    /// </summary>
    /// <param name="inputRootNodes">List of <i>root nodes</i> to migrate.
    /// Root nodes are all nodes without a <see cref="IReadableNode.GetParent">parent</see>.</param>
    // TODO: Maybe rename to Execute / Run / Apply?
    // What happens if return=false and outputRootNodes != inputRootNodes?
    MigrationResult Migrate(List<LenientNode> inputRootNodes);
}

/// A <see cref="IMigration">migration</see> that needs to know the <see cref="LionWebVersions">version of the LionWeb standard</see>
/// the input nodes had been serialized with.
public interface IMigrationWithLionWebVersion : IMigration
{
    /// The <see cref="LionWebVersions">version of the LionWeb standard</see> the input nodes had been serialized with.
    string SerializedLionWebVersion { get; set; }
}

/// <summary>
/// Reports the result of a <see cref="IMigration">migration</see>.
/// </summary>
/// <param name="Changed">Whether the migration applied any changes.</param>
/// <param name="OutputRootNodes">All <i>root nodes</i> after migration.
/// Might be the same, more, or less than <see cref="IMigration.Migrate">inputRootNodes</see>.</param>
public record MigrationResult(bool Changed, List<LenientNode> OutputRootNodes);