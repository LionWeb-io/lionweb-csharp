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

public class LionWebVersionMigration(LionWebVersions from, LionWebVersions to) : IMigrationWithLionWebVersion
{
    private bool _runMigration = true;
    private string? _serializedLionWebVersion;

    /// <inheritdoc />
    public required int Priority { get; init; }

    /// <inheritdoc />
    public string SerializedLionWebVersion
    {
        get => _serializedLionWebVersion!;
        set
        {
            _runMigration = true;
            _serializedLionWebVersion = value;
        }
    }

    /// <inheritdoc />
    public void Initialize(ILanguageRegistry languageRegistry)
    {
    }

    /// <inheritdoc />
    public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
        _runMigration && _serializedLionWebVersion != null &&
        from.IsCompatibleWith(LionWebVersions.GetPureByVersionString(SerializedLionWebVersion));

    /// <inheritdoc />
    public MigrationResult Migrate(List<LenientNode> inputRootNodes)
    {
        _runMigration = false;
        return new(true, inputRootNodes);
    }
}