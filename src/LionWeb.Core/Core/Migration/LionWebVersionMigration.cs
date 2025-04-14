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

public class LionWebVersionMigration(LionWebVersions from, LionWebVersions to) : IMigrationWithLionWebVersion
{
    private ILanguageRegistry _languageRegistry;
    private readonly DynamicLanguageCloner _languageCloner = new NonRepeatingDynamicLanguageCloner(to);

    private bool _runMigration = true;
    private string _serializedLionWebVersion;

    /// <inheritdoc />
    public required int Priority { get; init; }

    /// <inheritdoc />
    public string SerializedLionWebVersion
    {
        get => _serializedLionWebVersion;
        set
        {
            _runMigration = true;
            _serializedLionWebVersion = value;
        }
    }

    /// <inheritdoc />
    public void Initialize(ILanguageRegistry languageRegistry)
    {
        _languageRegistry = languageRegistry;
        _languageCloner.Clone([from.BuiltIns, from.LionCore]);
    }

    /// <inheritdoc />
    public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
        _runMigration && from.IsCompatibleWith(LionWebVersions.GetPureByVersionString(SerializedLionWebVersion));
    // {
    //     var isApplicable = _languageCloner.Clone(languageIdentities
    //         .Select(li => _languageRegistry.TryGetLanguage(li, out var language) ? language : null)
    //         .OfType<DynamicLanguage>()).Count != 0;
    //     return isApplicable;
    // }

    public MigrationResult Migrate(List<LenientNode> inputRootNodes)
    {
        // foreach (var node in inputRootNodes.Descendants().ToList())
        // {
        //     node.SetClassifier(Map(node.GetClassifier()));
        //     foreach (var feature in node.CollectAllSetFeatures().ToList())
        //     {
        //         var value = node.Get(feature);
        //         var mapped = Map(feature);
        //
        //         if (mapped is Property prop)
        //         {
        //             value = (prop.Type, value) switch
        //             {
        //                 (Enumeration enm, Enum lit) => prop.Type.GetLanguage()
        //                     .GetFactory()
        //                     .GetEnumerationLiteral(enm.Literals.First(l => l.Key == lit.LionCoreKey())),
        //
        //                 (StructuredDataType, IStructuredDataTypeInstance inst) => From(inst),
        //                 _ => value
        //             };
        //         }
        //
        //         node.Set(feature, null);
        //         node.Set(mapped, value);
        //     }
        // }

        _runMigration = false;
        return new(true, inputRootNodes);
    }

    private T Map<T>(T input) where T : IKeyed
    {
        return _languageRegistry.Lookup(input);
        // if (_languageCloner.DynamicMap.TryGetValue(input, out var result) && result is T r)
        // {
        //     return r;
        // }
        //
        // throw new KeyNotFoundException();
    }

    public IStructuredDataTypeInstance From(IStructuredDataTypeInstance instance)
    {
        var sdt = Map(instance.GetStructuredDataType());

        return sdt.GetLanguage().GetFactory()
            .CreateStructuredDataTypeInstance(sdt, new FieldValues(instance.CollectAllSetFields().Select(MapSdt)));

        (Field f, object?) MapSdt(Field f) =>
            (f, instance.Get(f) switch
            {
                IStructuredDataTypeInstance i => From(i),
                var v => v
            });
    }

    private class NonRepeatingDynamicLanguageCloner(LionWebVersions lionWebVersion)
        : DynamicLanguageCloner(lionWebVersion)
    {
        public override Dictionary<LanguageIdentity, DynamicLanguage> Clone(IEnumerable<Language> languages) =>
            base.Clone(languages.Where(l => !_dynamicMap.ContainsKey(l)));
    };
}