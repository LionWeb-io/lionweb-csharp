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
using System.Diagnostics.CodeAnalysis;

/// Replaces all language element keys as mapped in <paramref name="keyMapping"/>.
/// <paramref name="keyMapping"/>'s <i>keys</i> MUST include the keys of all processed languages.
/// <param name="keyMapping">All keys to map.</param>
/// <param name="languageVersionMapping">Optional mapping of <i>old</i> language keys to <i>new</i> language version.</param>
public class KeysMigration(Dictionary<string, string> keyMapping, Dictionary<string, string>? languageVersionMapping = null) : IMigration
{
    /// <inheritdoc />
    public required int Priority { get; init; }

    /// <inheritdoc />
    public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
        languageIdentities.Any(l => keyMapping.ContainsKey(l.Key));

    /// <inheritdoc />
    public void Initialize(ILanguageRegistry languageRegistry) { }

    /// <inheritdoc />
    public MigrationResult Migrate(List<LenientNode> inputRootNodes)
    {
        var languages = MigrationExtensions.CollectUsedLanguages(inputRootNodes).ToList();

        var result = false;

        foreach (var language in languages)
        {
            var migrationFactory = (MigrationFactory)language.GetFactory();

            result |= ReplaceVersion(language);
            result |= ReplaceKey(language);

            foreach (var entity in language.Entities.Cast<DynamicLanguageEntity>())
            {
                result |= ReplaceKey(entity);

                switch (entity)
                {
                    case Classifier classifier:
                        result = classifier.Features.Cast<DynamicFeature>()
                            .Aggregate(result, (current, feature) => current | ReplaceKey(feature));

                        break;

                    case Enumeration enumeration:
                        result = enumeration.Literals.Cast<DynamicEnumerationLiteral>()
                            .Aggregate(result, (current, literal) => current | ReplaceKey(literal));
                        migrationFactory.UpdateEnumeration(enumeration);

                        break;

                    case StructuredDataType structuredDataType:
                        result = structuredDataType.Fields.Cast<DynamicField>()
                            .Aggregate(result, (current, field) => current | ReplaceKey(field));

                        break;
                }
            }
        }

        if (result)
            MigrateValueLiterals(inputRootNodes);


        return new MigrationResult(result, inputRootNodes);
    }

    private bool ReplaceVersion(DynamicLanguage language)
    {
        if(languageVersionMapping == null)
            return false;

        if (languageVersionMapping.TryGetValue(language.Key, out var version) && version != language.Version)
        {
            language.Version = version;
            return true;
        }
        
        return false;
    }

    private bool ReplaceKey(DynamicIKeyed keyed)
    {
        if (TryGet(keyed.Key, out var replacement))
        {
            keyed.Key = replacement;
            return true;
        }

        return false;
    }

    private void MigrateValueLiterals(List<LenientNode> inputRootNodes)
    {
        foreach (var node in inputRootNodes.Descendants())
        {
            foreach (var property in node.CollectAllSetFeatures().OfType<Property>().ToList())
            {
                switch (property.Type)
                {
                    case Enumeration e:
                        if (node.TryGet(property, out var lit) && MigrateEnum(e, lit, out var enm))
                            node.Set(property, enm);

                        break;

                    case StructuredDataType sdt:
                        if (node.TryGet(property, out var sdtVal) && MigrateSdt(sdt, sdtVal, out var inst))
                            node.Set(property, inst);

                        break;
                }
            }
        }
    }

    private bool MigrateEnum(Enumeration e, [NotNullWhen(true)] object? lit, out Enum? result)
    {
        result = null;
        if (lit is not Enum enm)
            return false;

        var key = enm.LionCoreKey();
        if (key == null || !TryGet(key, out var mappedKey))
            return false;

        result = e.GetLanguage().GetFactory().GetEnumerationLiteral(e.Literals.First(l => l.Key == mappedKey));
        return true;
    }

    private bool MigrateSdt(StructuredDataType sdt, [NotNullWhen(true)] object? value,
        out IStructuredDataTypeInstance? result)
    {
        result = null;
        if (value is not IStructuredDataTypeInstance sdtInstance)
            return false;

        var enumerations = CollectEnumValues(sdtInstance).ToList();
        if (!enumerations.Any(e =>
            {
                var lionCoreKey = e.LionCoreKey();
                return lionCoreKey != null && TryGet(lionCoreKey, out var mappedKey) && lionCoreKey != mappedKey;
            })
           )
            return false;

        result = sdt.GetLanguage().GetFactory().CreateStructuredDataTypeInstance(sdt,
            new FieldValues(sdtInstance
                .CollectAllSetFields().Select(f =>
                {
                    var fieldValue = sdtInstance.Get(f);

                    return (f, newValue: f.Type switch
                    {
                        Enumeration e => MigrateEnum(e, fieldValue, out var lit) ? lit : fieldValue,
                        StructuredDataType s => MigrateSdt(s, fieldValue, out var inst) ? inst : fieldValue,
                        _ => fieldValue
                    });
                })));
        return true;
    }

    private IEnumerable<Enum> CollectEnumValues(IStructuredDataTypeInstance sdt) => sdt.CollectAllSetFields()
        .SelectMany(f =>
            f.Type switch
            {
                Enumeration => [(Enum)sdt.Get(f)!],
                StructuredDataType when sdt.Get(f) is IStructuredDataTypeInstance inst => CollectEnumValues(inst),
                _ => []
            });

    private bool TryGet(string key, [NotNullWhen(true)] out string? value) =>
        keyMapping.TryGetValue(key, out value) && value != key;
}