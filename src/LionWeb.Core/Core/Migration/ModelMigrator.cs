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

/// <inheritdoc cref="LionWeb.Core.Migration.IModelMigrator"/>
public class ModelMigrator : ILanguageRegistry, IModelMigrator
{
    private const int _maxMigrationRounds = 20;

    private readonly List<IMigration> _migrations = [];
    private readonly Dictionary<LanguageIdentity, DynamicLanguage> _dynamicInputLanguages;
    private readonly DeserializerBuilder _deserializerBuilder;
    private readonly SerializerBuilder _serializerBuilder;

    private Dictionary<LanguageIdentity, DynamicLanguage> _dynamicLanguages;

    /// <param name="lionWebVersion">Version of LionWeb standard to use for
    /// <see cref="Deserializer.LionWebVersion">deserializing</see>,
    /// <see cref="DynamicLanguageCloner">language adjustment</see>,
    /// and <see cref="Serializer.LionWebVersion">serialization</see>.</param>
    /// 
    /// <param name="languages">Languages we expect to see during the migration.
    /// We <i>can</i> handle unknown languages, classifiers, features etc.
    /// However, we <i>cannot</i> reconstruct complex inheritance hierarchies just from nodes;
    /// this might lead to serialization issues. 
    /// </param>
    public ModelMigrator(LionWebVersions lionWebVersion, IEnumerable<Language> languages)
    {
        LionWebVersion = lionWebVersion;
        _dynamicInputLanguages = new DynamicLanguageCloner(lionWebVersion).Clone(languages);
        foreach ((_, DynamicLanguage lang) in _dynamicInputLanguages)
        {
            lang.SetFactory(new MigrationFactory(lang));
        }
        _dynamicLanguages = _dynamicInputLanguages;
    }

    /// <inheritdoc />
    public int MaxMigrationRounds { get; init; } = _maxMigrationRounds;

    public DeserializerBuilder DeserializerBuilder
    {
        get => _deserializerBuilder;
        init
        {
            value.LionWebVersion.AssureCompatible(LionWebVersion);
            if (SerializerBuilder != null)
                value.LionWebVersion.AssureCompatible(SerializerBuilder.LionWebVersion);

            _deserializerBuilder = value;
        }
    }

    public SerializerBuilder SerializerBuilder
    {
        get => _serializerBuilder;
        init
        {
            value.LionWebVersion.AssureCompatible(LionWebVersion);
            if (DeserializerBuilder != null)
                value.LionWebVersion.AssureCompatible(DeserializerBuilder.LionWebVersion);

            _serializerBuilder = value;
        }
    }

    /// <inheritdoc />
    public async Task<bool> MigrateAsync(Stream inputUtf8JsonStream, Stream migratedUtf8JsonStream)
    {
        (List<IReadableNode> loaded, string? lionWebVersion) = await Deserialize(inputUtf8JsonStream);
        UpdateSerializedLionWebVersion(lionWebVersion);

        int migrationRound = 0;
        bool anyChange = false;
        bool changeInThisRound;
        var rootNodes = loaded.Cast<LenientNode>().ToList();

        do
        {
            CheckMigrationRounds(migrationRound);

            migrationRound++;
            changeInThisRound = false;

            _dynamicLanguages = CollectUsedLanguages(rootNodes);
            var usedLanguages = _dynamicLanguages.Keys.ToImmutableHashSet();
            var applicableMigrations = SelectApplicableMigrations(usedLanguages);

            foreach (var migration in applicableMigrations)
            {
                (var b, rootNodes) = migration.Migrate(rootNodes).Validate(rootNodes);
                changeInThisRound |= b;
                ValidateRootNodes(rootNodes, migration);
            }

            anyChange |= changeInThisRound;
        } while (changeInThisRound);

        await Serialize(migratedUtf8JsonStream, rootNodes);

        return anyChange;
    }

    private async Task<(List<IReadableNode> loaded, string? lionWebVersion)> Deserialize(Stream inputUtf8JsonStream)
    {
        string? lionWebVersion = null;
        var builder = DeserializerBuilder ?? new DeserializerBuilder().WithLionWebVersion(LionWebVersion);
        var deserializer = builder
            .WithHandler(new MigrationDeserializerHandler(LionWebVersion, _dynamicLanguages.Values,
                builder.Handler ?? new DeserializerExceptionHandler()))
            .WithLanguages(_dynamicLanguages.Values).Build();

        var loaded = await JsonUtils.ReadNodesFromStreamAsync(inputUtf8JsonStream, deserializer, serializedVersion =>
        {
            lionWebVersion = serializedVersion;
            deserializer.LionWebVersion.AssureCompatible(serializedVersion);
        });

        return (loaded, lionWebVersion);
    }

    private void UpdateSerializedLionWebVersion(string? lionWebVersion)
    {
        if (lionWebVersion == null)
            return;

        foreach (var migration in _migrations.OfType<IMigrationWithLionWebVersion>())
            migration.SerializedLionWebVersion = lionWebVersion;
    }

    private void CheckMigrationRounds(int migrationRound)
    {
        if (migrationRound >= MaxMigrationRounds)
            throw new MaxMigrationRoundsExceededException(MaxMigrationRounds);
    }

    private Dictionary<LanguageIdentity, DynamicLanguage> CollectUsedLanguages(List<LenientNode> nodes) =>
        MigrationExtensions.CollectUsedLanguages(nodes)
            .ToDictionary(LanguageIdentity.FromLanguage, l => l);

    private IOrderedEnumerable<IMigration> SelectApplicableMigrations(ImmutableHashSet<LanguageIdentity> usedLanguages)
        => _migrations
            .Where(m => m.IsApplicable(usedLanguages))
            .OrderBy(m => m.Priority);

    private void ValidateRootNodes(List<LenientNode> rootNodes, IMigration migration)
    {
        var nullRootNodes = rootNodes
            .Where(n => n == null)
            .ToList();

        if (nullRootNodes.Count > 0)
            throw new InvalidRootNodesException(migration, "null root nodes");
        
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
            throw new InvalidRootNodesException(migration, string.Join("; ", messages));
        return;

        string JoinNodeIds(IEnumerable<IReadableNode> nodes) =>
            string.Join(", ", nodes.Select(n => n.GetId()));
    }

    private async Task Serialize(Stream migratedUtf8JsonStream, List<LenientNode> rootNodes)
    {
        var builder = SerializerBuilder ?? new SerializerBuilder().WithLionWebVersion(LionWebVersion);

        var serializer = builder
            .WithHandler(new MigrationSerializerHandler(builder.Handler ?? new SerializerExceptionHandler()))
            .Build();

        var allNodes = rootNodes.Descendants().ToList();
        await JsonUtils.WriteNodesToStreamAsync(migratedUtf8JsonStream, serializer, allNodes);
    }

    /// <inheritdoc />
    public void RegisterMigration(IMigration migration)
    {
        migration.Initialize(this);
        _migrations.Add(migration);
    }

    #region ILanguageRegistry

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; }

    /// <inheritdoc />
    public IEnumerable<DynamicLanguage> KnownLanguages { get => _dynamicLanguages.Values; }

    /// <inheritdoc />
    public bool TryGetLanguage(LanguageIdentity languageIdentity, [NotNullWhen(true)] out DynamicLanguage? language) =>
        _dynamicLanguages.TryGetValue(languageIdentity, out language) ||
        _dynamicInputLanguages.TryGetValue(languageIdentity, out language);

    /// <inheritdoc />
    public bool RegisterLanguage(DynamicLanguage language, LanguageIdentity? languageIdentity = null)
    {
        language.SetFactory(new MigrationFactory(language));
        return _dynamicLanguages.TryAdd(languageIdentity ?? LanguageIdentity.FromLanguage(language), language);
    }

    /// <inheritdoc />
    public T Lookup<T>(T keyed) where T : IKeyed
    {
        Language inputLanguage = keyed.GetLanguage();

        Language language;
        if (TryGetLanguage(LanguageIdentity.FromLanguage(inputLanguage), out var inputLanguageEquivalent))
        {
            language = inputLanguageEquivalent;
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
                throw new AmbiguousLanguageKeyMapping(inputLanguage.Key, sameKey);
            }
        }

        return MigrationExtensions.Lookup(keyed, language);
    }

    #endregion
}