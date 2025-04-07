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
using Utilities;

public class ModelMigrator : ILanguageRegistry
{
    private const int _maxMigrationRounds = 20;
    
    private readonly List<IMigration> _migrations = [];
    private readonly Dictionary<LanguageIdentity, DynamicLanguage> _dynamicLanguages;
    private readonly LionWebVersions _lionWebVersion;

    public ModelMigrator(LionWebVersions lionWebVersion, IEnumerable<Language> languages)
    {
        _lionWebVersion = lionWebVersion;
        _dynamicLanguages = new DynamicLanguageCloner(lionWebVersion).Clone(languages);
    }

    public int MaxMigrationRounds { get; init; } = _maxMigrationRounds;
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

            var usedLanguages = CollectUsedLanguages(nodes);

            var applicableMigrations = SelectApplicableMigrations(usedLanguages);

            foreach (var migration in applicableMigrations)
            {
                anyChange = true;
                (var b, nodes) = migration.Migrate(nodes);
                changed |= b;
            }
        } while (changed);

        var serializer = new Serializer(_lionWebVersion)
        {
            SerializeEmptyFeatures = SerializeEmptyFeatures, Handler = new MigrationSerializerHandler()
        };

        var allNodes = nodes.Descendants();
        JsonUtils.WriteNodesToStream(migrated, serializer, allNodes);

        return anyChange;
    }

    private static ImmutableHashSet<LanguageIdentity> CollectUsedLanguages(List<LenientNode> nodes) =>
        nodes
            .Descendants()
            .SelectMany(n =>
                n.GetClassifier().Features.Select(f => f.GetLanguage())
                    .Prepend(n.GetClassifier().GetLanguage())
            )
            .Distinct()
            .Select(LanguageIdentity.FromLanguage)
            .ToImmutableHashSet();

    private IOrderedEnumerable<IMigration> SelectApplicableMigrations(ImmutableHashSet<LanguageIdentity> usedLanguages) =>
        _migrations
            .Where(m => m.IsApplicable(usedLanguages))
            .OrderBy(m => m.Priority);

    public void RegisterMigration(IMigration migration)
    {
        migration.Initialize(this);
        _migrations.Add(migration);
    }

    public bool TryGetLanguage(LanguageIdentity languageIdentity, out DynamicLanguage? language) =>
        _dynamicLanguages.TryGetValue(languageIdentity, out language);

    public bool RegisterLanguage(DynamicLanguage language) =>
        _dynamicLanguages.TryAdd(LanguageIdentity.FromLanguage(language),language );
}

public static class MigrationExtensions
{
    public static IEnumerable<LenientNode> Descendants(this List<LenientNode> nodes) =>
        nodes.SelectMany(Descendants);
    
    public static IEnumerable<LenientNode> Descendants(this LenientNode node) => 
        M1Extensions.Descendants<LenientNode>(node, true, true);

    public static IEnumerable<LenientNode>
        AllInstancesOf(this List<LenientNode> nodes, ClassifierIdentity classifier) =>
        nodes
            .Descendants()
            .Where(n =>
            {
                var classifierIdentity = ClassifierIdentity.FromClassifier(n.GetClassifier());
                return classifierIdentity == classifier;
            });
    
    public static IEnumerable<LenientNode>
        AllInstancesOf(this List<LenientNode> nodes, Classifier classifier) =>
        nodes
            .Descendants()
            .Where(n =>
            {
                return classifier.EqualsIdentity(n.GetClassifier());
            });
    
    public static void SetProperty(this IWritableNode node, Property property, object? value) =>
        node.Set(property, value);
    
    public static void SetChild(this LenientNode node, Containment containment, IWritableNode child) =>
        node.Set(containment, child.ConvertSubtreeToLenient());
    
    public static void SetChildren(this LenientNode node, Containment containment, IEnumerable<IWritableNode> children) =>
        node.Set(containment, children.Select(c => c.ConvertSubtreeToLenient()));

    public static void ConvertSubtreeToLenient(this IReadableNode node) => node switch
    {
        LenientNode l => l,
        var n=> new LenientNode(n.GetId(), n.GetClassifier())
    }
}