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

using M2;
using M3;

/// Migrates to a newer <see cref="LionWebVersions">Version of LionWeb standard</see>.
/// Only compatible with an <see cref="IModelMigrator"/> that uses a LionWebVersion compatible with
/// <i>both</i> of <paramref name="from"/> and <param name="to"/>;
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
        from.AssureCompatible(languageRegistry.LionWebVersion);
        to.AssureCompatible(languageRegistry.LionWebVersion);
    }

    /// <inheritdoc />
    public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
        _runMigration && _serializedLionWebVersion != null && !from.Equals(to) &&
        from.IsCompatibleWith(LionWebVersions.GetPureByVersionString(SerializedLionWebVersion));

    /// <inheritdoc />
    public MigrationResult Migrate(List<LenientNode> inputRootNodes)
    {
        _runMigration = false;

        foreach (var node in inputRootNodes.Descendants().ToList())
        {
            foreach (var property in node.CollectAllSetFeatures().OfType<Property>()
                         .Where(f => f.GetLanguage() is IBuiltInsLanguage).ToList())
            {
                if (node.TryGet(property, out var value))
                {
                    node.Set(property, null);
                    node.Set(to.BuiltIns.FindByKey<Property>(property.Key), value);
                }
            }
        }

        return new(true, inputRootNodes);
    }
}