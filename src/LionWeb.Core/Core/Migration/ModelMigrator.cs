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
using System.Collections;
using System.Collections.Immutable;
using Utilities;

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
    private readonly Dictionary<LanguageIdentity, DynamicLanguage> _dynamicLanguages;
    private readonly LionWebVersions _lionWebVersion;

    public ModelMigrator(LionWebVersions lionWebVersion, IEnumerable<Language> languages)
    {
        _lionWebVersion = lionWebVersion;
        _dynamicLanguages = new DynamicLanguageCloner(lionWebVersion).Clone(languages);
    }

    /// <inheritdoc />
    public int MaxMigrationRounds { get; init; } = _maxMigrationRounds;
    public CompressedIdConfig CompressedIdConfig { get; init; } = new();

    public bool SerializeEmptyFeatures { get; init; } = false;

    /// <inheritdoc />
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

            var usedLanguages = CollectUsedLanguages(rootNodes);
            var applicableMigrations = SelectApplicableMigrations(usedLanguages);

            foreach (var migration in applicableMigrations)
            {
                (var b, rootNodes) = migration.Migrate(rootNodes);
                changeInThisRound |= b;
            }
            
            anyChange |= changeInThisRound;
        } while (changeInThisRound);

        var serializer = new Serializer(_lionWebVersion)
        {
            SerializeEmptyFeatures = SerializeEmptyFeatures, Handler = new MigrationSerializerHandler()
        };

        var allNodes = rootNodes.Descendants();
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

    /// <inheritdoc />
    public void RegisterMigration(IMigration migration)
    {
        migration.Initialize(this);
        _migrations.Add(migration);
    }

    /// <inheritdoc />
    public bool TryGetLanguage(LanguageIdentity languageIdentity, out DynamicLanguage? language) =>
        _dynamicLanguages.TryGetValue(languageIdentity, out language);

    /// <inheritdoc />
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
        node.Set(containment, children.Select(ConvertSubtreeToLenient));

    public static void SetReference(this LenientNode node, Reference reference, IReadableNode target) =>
        node.Set(reference, target);
    
    public static void SetReferences(this LenientNode node, Reference reference, IEnumerable<IReadableNode> targets) =>
        node.Set(reference, targets);

    public static LenientNode ConvertSubtreeToLenient(this IReadableNode node) => node switch
    {
        LenientNode l => l,
        var n => ConvertToLenient(n)
    };

    private static LenientNode ConvertToLenient(IReadableNode node)
    {
        var result = new LenientNode(node.GetId(), node.GetClassifier());
        result.AddAnnotations(node.GetAnnotations().Select(ConvertSubtreeToLenient));
        foreach (Feature feature in node.CollectAllSetFeatures())
        {
            var value = node.Get(feature);
            switch (feature)
            {
                case Property:
                    result.Set(feature, value);
                    break;
                
                case Containment:
                    switch (value)
                    {
                        case IEnumerable enumerable:
                            result.Set(feature, enumerable.Cast<IWritableNode>().Select(ConvertSubtreeToLenient));
                            break;
                        case IWritableNode writableNode:
                            result.Set(feature, writableNode.ConvertSubtreeToLenient());
                            break;
                    }

                    break;
                
                case Reference:
                    result.Set(feature, value);
                    break;
            }
        }
        
        return result;
    }
}