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

/// Runs all <see cref="RegisterMigration">registered migrations</see> in their applicable order.
/// Will fail if running for more than <see cref="MaxMigrationRounds"/> rounds.
///
/// <p>
/// Each round consists of all <see cref="IMigration.IsApplicable">applicable</see> <see cref="IMigration">migrations</see>
/// ordered by their <see cref="IMigration.Priority"/>.
/// </p> 
public interface IModelMigrator
{
    /// Maximum number of rounds to try.
    int MaxMigrationRounds { get; init; }

    /// Runs <paramref name="migration"/>, if <see cref="IMigration.IsApplicable">applicable</see>.
    void RegisterMigration(IMigration migration);

    /// <summary>
    /// Execute the migration on all nodes from <paramref name="inputUtf8JsonStream"/>,
    /// and store the result to <paramref name="migratedUtf8JsonStream"/>.
    /// </summary>
    /// <returns><c>true</c> if the migration applied any changes; <c>false</c> otherwise.</returns>
    Task<bool> Migrate(Stream inputUtf8JsonStream, Stream migratedUtf8JsonStream);
}