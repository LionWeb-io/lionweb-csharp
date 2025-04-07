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

using M1;
using M2;
using M3;
using Serialization;
using System.Collections.Immutable;

public class ModelMigrator : ILanguageRegistry
{
    private readonly List<IMigration> _migrations = [];
    private readonly Dictionary<LanguageIdentity, DynamicLanguage> _dynamicLanguages;
    private readonly LionWebVersions _lionWebVersion;

    public ModelMigrator(LionWebVersions lionWebVersion, IEnumerable<Language> languages)
    {
        _lionWebVersion = lionWebVersion;
        _dynamicLanguages = new DynamicLanguageCloner(lionWebVersion).Clone(languages);
    }

    public int MaxMigrationRounds { get; init; } = 20;
    public CompressedIdConfig CompressedIdConfig { get; init; } = new();

    public bool SerializeEmptyFeatures { get; init; } = false;

    public async Task<bool> Migrate(Stream input, Stream migrated)
    {
        var builder = new DeserializerBuilder()
            .WithCompressedIds(CompressedIdConfig)
            .WithLionWebVersion(_lionWebVersion)
            .WithHandler(new MigrationDeserializerHandler(_lionWebVersion, _dynamicLanguages.Values))
            .WithLanguages(_dynamicLanguages.Values);

        var loaded = await JsonUtils.ReadNodesFromStreamAsync(input, builder.Build());

        int migrationRound = 0;
        bool anyChange = false;
        bool changed;
        var nodes = loaded.Cast<LenientNode>().ToList();

        do
        {
            if (migrationRound >= MaxMigrationRounds)
            {
                throw new ArgumentException($"Exceeded {MaxMigrationRounds} migration rounds.");
            }

            migrationRound++;
            changed = false;

            var usedLanguages = nodes
                .Descendants()
                .SelectMany(n =>
                    n.GetClassifier().Features.Select(f => f.GetLanguage())
                        .Prepend(n.GetClassifier().GetLanguage())
                )
                .Distinct()
                .Select(LanguageIdentity.FromLanguage)
                .ToImmutableHashSet();

            var applicableMigrations = _migrations
                .Where(m => m.IsApplicable(usedLanguages))
                .OrderBy(m => m.Priority);

            foreach (var migration in applicableMigrations)
            {
                anyChange = true;
                changed |= migration.Migrate(nodes, out nodes);
            }
        } while (changed);

        var serializer = new Serializer(_lionWebVersion)
        {
            SerializeEmptyFeatures = SerializeEmptyFeatures, Handler = new MigrationSerializerHandler()
        };

        var allNodes = nodes.Descendants().ToList();
        JsonUtils.WriteNodesToStream(migrated, serializer, allNodes);

        return anyChange;
    }

    public void RegisterMigration(IMigration migration)
    {
        migration.Initialize(this);
        _migrations.Add(migration);
    }

    public bool TryGetLanguage(LanguageIdentity languageIdentity, out DynamicLanguage? language) =>
        _dynamicLanguages.TryGetValue(languageIdentity, out language);
}

public static class MigrationExtensions
{
    public static IEnumerable<LenientNode> Descendants(this List<LenientNode> nodes) =>
        nodes.SelectMany(Descendants);
    
    public static IEnumerable<LenientNode> Descendants(this LenientNode node) => 
        M1Extensions.Descendants<LenientNode>(node, true, true);
    
}