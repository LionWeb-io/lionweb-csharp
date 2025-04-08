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
using System.Diagnostics.CodeAnalysis;

public interface IModelMigrator
{
    int MaxMigrationRounds { get; init; }
    void RegisterMigration(IMigration migration);
    Task<bool> Migrate(Stream input, Stream migrated);
}

public class ModelMigrator : ILanguageRegistry, IModelMigrator
{
    private const int _maxMigrationRounds = 20;

    private readonly List<IMigration> _migrations = [];
    private readonly Dictionary<LanguageIdentity, DynamicLanguage> _dynamicInputLanguages;
    private Dictionary<LanguageIdentity, DynamicLanguage> _dynamicLanguages;
    private readonly LionWebVersions _lionWebVersion;

    public ModelMigrator(LionWebVersions lionWebVersion, IEnumerable<Language> languages)
    {
        _lionWebVersion = lionWebVersion;
        _dynamicInputLanguages = new DynamicLanguageCloner(lionWebVersion).Clone(languages);
        _dynamicLanguages = _dynamicInputLanguages;
    }

    /// <inheritdoc />
    public int MaxMigrationRounds { get; init; } = _maxMigrationRounds;

    public CompressedIdConfig CompressedIdConfig { get; init; } = new();

    public bool SerializeEmptyFeatures { get; init; } = false;

    /// <inheritdoc />
    public async Task<bool> Migrate(Stream input, Stream migrated)
    {
        DeserializerBuilder builder = SetupDeserializerBuilder();

        var loaded = await JsonUtils.ReadNodesFromStreamAsync(input, builder.Build());

        int migrationRound = 0;
        bool anyChange = false;
        bool changeInThisRound;
        var rootNodes = loaded.Cast<LenientNode>().ToList();

        do
        {
            if (migrationRound >= MaxMigrationRounds)
            {
                throw new ArgumentException($"Exceeded {MaxMigrationRounds} migration rounds.");
            }

            migrationRound++;
            changeInThisRound = false;

            _dynamicLanguages = CollectUsedLanguages(rootNodes);
            var usedLanguages = _dynamicLanguages.Keys.ToImmutableHashSet();
            var applicableMigrations = SelectApplicableMigrations(usedLanguages);

            foreach (var migration in applicableMigrations)
            {
                (var b, rootNodes) = migration.Migrate(rootNodes);
                changeInThisRound |= b;
                ValidateRootNodes(rootNodes, migration);
            }

            anyChange |= changeInThisRound;
        } while (changeInThisRound);

        var serializer = new Serializer(_lionWebVersion)
        {
            SerializeEmptyFeatures = SerializeEmptyFeatures, Handler = new MigrationSerializerHandler()
        };

        var allNodes = rootNodes.Descendants().ToList();
        JsonUtils.WriteNodesToStream(migrated, serializer, allNodes);

        return anyChange;
    }

    private DeserializerBuilder SetupDeserializerBuilder()
    {
        var builder = new DeserializerBuilder()
            .WithCompressedIds(CompressedIdConfig)
            .WithLionWebVersion(_lionWebVersion)
            .WithHandler(new MigrationDeserializerHandler(_lionWebVersion, _dynamicLanguages.Values))
            .WithLanguages(_dynamicLanguages.Values);
        return builder;
    }

    private Dictionary<LanguageIdentity, DynamicLanguage> CollectUsedLanguages(List<LenientNode> nodes) =>
        nodes
            .Descendants()
            .SelectMany(n =>
                n.GetClassifier().Features.Select(f => f.GetLanguage())
                    .Prepend(n.GetClassifier().GetLanguage())
            )
            .Cast<DynamicLanguage>()
            .Distinct()
            .ToDictionary(LanguageIdentity.FromLanguage, l => l);

    private IOrderedEnumerable<IMigration>
        SelectApplicableMigrations(ImmutableHashSet<LanguageIdentity> usedLanguages) =>
        _migrations
            .Where(m => m.IsApplicable(usedLanguages))
            .OrderBy(m => m.Priority);

    private void ValidateRootNodes(List<LenientNode> rootNodes, IMigration migration)
    {
        var nonRootNodes = rootNodes
            .Where(n => n.GetParent() != null)
            .ToList();

        var duplicateRootIds = rootNodes
            .GroupBy(n => n.GetId())
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .Distinct()
            .ToList();

        List<string> messages = [];
        if (nonRootNodes.Count != 0)
            messages.Add($"non-root nodes: {JoinNodeIds(nonRootNodes)}");

        if (duplicateRootIds.Count != 0)
            messages.Add($"duplicate ids: {JoinNodeIds(duplicateRootIds)}");

        if (messages.Count != 0)
            throw new ArgumentException(
                $"migration {migration} resulted in invalid root nodes: {string.Join("; ", messages)}");
        return;

        string JoinNodeIds(IEnumerable<IReadableNode> nodes) =>
            string.Join(", ", nodes.Select(n => n.GetId()));
    }

    /// <inheritdoc />
    public void RegisterMigration(IMigration migration)
    {
        migration.Initialize(this);
        _migrations.Add(migration);
    }

    /// <inheritdoc />
    public bool TryGetLanguage(LanguageIdentity languageIdentity, [NotNullWhen(true)] out DynamicLanguage? language) =>
        _dynamicLanguages.TryGetValue(languageIdentity, out language);

    /// <inheritdoc />
    public bool RegisterLanguage(DynamicLanguage language, LanguageIdentity? languageIdentity = null) =>
        _dynamicLanguages.TryAdd(languageIdentity ?? LanguageIdentity.FromLanguage(language), language);

    /// <inheritdoc />
    public T Lookup<T>(T keyed) where T : IKeyed
    {
        Language inputLanguage = keyed.GetLanguage();

        Language language;
        if (TryGetLanguage(LanguageIdentity.FromLanguage(inputLanguage), out var l))
        {
            language = l;
        } else
        {
            var sameKey = _dynamicLanguages
                .Values
                .Where(l => l.Key == inputLanguage.Key)
                .ToList();

            if (sameKey.Count == 1)
            {
                language = sameKey[0];
            } else
            {
                throw new ArgumentException($"More than one mapped language for key {inputLanguage.Key}: {sameKey}");
            }
        }

        return MigrationExtensions.Lookup(keyed, language);
    }
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

    internal static T Lookup<T>(T keyed, Language language) where T : IKeyed
    {
        var mapped = M1Extensions
            .Descendants<IKeyed>(language, true)
            .FirstOrDefault(k => k.Key == keyed.Key);

        if (mapped is T result)
            return result;

        throw new ArgumentException($"No lookup for {keyed.Key}");
    }
}