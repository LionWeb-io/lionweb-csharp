// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

using Io.Lionweb.Mps.Specific;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Migration;
using LionWeb.Core.Serialization;
using LionWeb.Generator;
using LionWeb.Generator.Names;

foreach (LionWebVersions lionWebVersion in LionWebVersions.AllPureVersions)
{
    ShapesDefinition._lionWebVersion = lionWebVersion;

    Console.WriteLine($"\n### LionWeb specification version: {lionWebVersion}\n");

    SerializeLanguagesLocally(lionWebVersion, "lioncore", lionWebVersion.LionCore);
    SerializeLanguagesLocally(lionWebVersion, "shapes", ShapesDefinition.Language);

    var testLanguagesDefinitions = new TestLanguagesDefinitions(lionWebVersion);
    var sdtLang = testLanguagesDefinitions.SdtLang;
    if (sdtLang != null)
        SerializeLanguagesLocally(lionWebVersion, "sdtLang", sdtLang);

    var libraryLanguage = DeserializeExternalLanguage(lionWebVersion, "library").First();
    var multiLanguage = DeserializeExternalLanguage(lionWebVersion, "multi", libraryLanguage).First();
    var withEnumLanguage = DeserializeExternalLanguage(lionWebVersion, "with-enum").First();
    withEnumLanguage.Name = "WithEnum";
    // If we update MPS-Specific in MPS, we want to re-generate it. Otherwise, we use the shipped version.
    // Follow the 3 steps below in order to generate MPS-specific languages.
    // After the generation, content of the generated files should be manually copied to where it is needed.
    // Make sure the generated language class extends/implements the followings: LanguageBase<ISpecificFactory>, ISpecificLanguage
    // 1) Comment out the line below  
    var specificLanguage = ISpecificLanguage.Get(lionWebVersion);
    // 2) Uncomment the line below
    // var specificLanguage = DeserializeExternalLanguage(lionWebVersion, "MpsSpecific-metamodel-annotated").First();
    var deprecatedLang = DeserializeExternalLanguage(lionWebVersion, "deprecated", specificLanguage).First();
    deprecatedLang.Name = "Deprecated";
    var testLanguage = DeserializeExternalLanguage(lionWebVersion, "testLanguage", specificLanguage).First();
    var shapesLanguage = ShapesDefinition.Language;
    shapesLanguage.Name = "Shapes";
    var aLang = testLanguagesDefinitions.ALang;
    var bLang = testLanguagesDefinitions.BLang;
    var tinyRefLang = testLanguagesDefinitions.TinyRefLang;
    var keywordLang = testLanguagesDefinitions.KeywordLang;
    var multiInheritLang = testLanguagesDefinitions.MultiInheritLang;
    var namedLang = testLanguagesDefinitions.NamedLang;
    var namedLangReadInterfaces = new DynamicLanguageCloner(lionWebVersion).Clone(namedLang);
    namedLangReadInterfaces.Name = "NamedReadInterfaces";

    var lionWebVersionNamespace = "V" + lionWebVersion.VersionString.Replace('.', '_');
    string prefix = $"LionWeb.Core.Test.Languages.Generated.{lionWebVersionNamespace}";
    
    List<Names> names =
    [
        new(libraryLanguage, $"{prefix}.Library.M2"),
        new(multiLanguage, $"{prefix}.Multi.M2") { NamespaceMappings = { [libraryLanguage] = $"{prefix}.Library.M2" } },
        new(withEnumLanguage, $"{prefix}.WithEnum.M2"),
        new(shapesLanguage, $"{prefix}.Shapes.M2"),
        new(aLang, $"{prefix}.Circular.A") { NamespaceMappings = { [bLang] = $"{prefix}.Circular.B" } },
        new(bLang, $"{prefix}.Circular.B") { NamespaceMappings = { [aLang] = $"{prefix}.Circular.A" } },
        new(tinyRefLang, $"{prefix}.TinyRefLang"),
        new(deprecatedLang, $"{prefix}.DeprecatedLang"),
        new(multiInheritLang, $"{prefix}.MultiInheritLang"),
        new(namedLang, $"{prefix}.NamedLang"),
        new(namedLangReadInterfaces, $"{prefix}.NamedLangReadInterfaces"),
        new(testLanguage, $"{prefix}.TestLanguage"),
        // We don't really want these file in tests project, but update the version in Generator.
        // However, it's not worth writing a separate code path for this one language (as we want to externalize it anyways).
        // Step 3: Uncomment the line below 
        // new(specificLanguage, $"Io.Lionweb.Mps.Specific.{lionWebVersionNamespace}")
    ];
    
    Dictionary<Language, GeneratorConfig> configs = new() { { namedLangReadInterfaces, new() {WritableInterfaces = false} } };

    if (sdtLang != null)
        names.Add(new(sdtLang, $"{prefix}.SDTLang"));

    if (keywordLang != null)
        names.Add(new(keywordLang, $"@namespace.@int.@public.{lionWebVersionNamespace}"));

    names.AddRange(testLanguagesDefinitions
        .MixedLangs
        .Select(l =>
            new Names(l, $"{prefix}.Mixed.{l.Name}")
            {
                NamespaceMappings = testLanguagesDefinitions
                    .MixedLangs
                    .Except([l])
                    .Select(n => (n, $"{prefix}.Mixed.{n.Name}"))
                    .ToDictionary()
            }
        )
    );

    var generationPath = "../../test/LionWeb.Core.Test/Languages/Generated";
    Directory.CreateDirectory(generationPath);

    foreach (var name in names)
    {
        var generator = new GeneratorFacade { Names = name, LionWebVersion = lionWebVersion, Config = configs.GetValueOrDefault(name.Language) ?? new() };
        generator.Generate();
        Console.WriteLine($"generated code for: {name.Language.Name}");

        var path = @$"{generationPath}/{lionWebVersionNamespace}/{name.Language.Name}.g.cs";
        generator.Persist(path);
        Console.WriteLine($"persisted to: {path}");
    }
}

return;

void SerializeLanguagesLocally(LionWebVersions lionWebVersion, string name, params Language[] languages)
{
    JsonUtils.WriteNodesToStream(
        new FileStream($"chunks/localDefs/{lionWebVersion.VersionString}/{name}.json", FileMode.Create),
        new SerializerBuilder().WithLionWebVersion(lionWebVersion).Build(),
        languages.SelectMany(l => M1Extensions.Descendants<IReadableNode>(l, true, true)));
}

DynamicLanguage[] DeserializeExternalLanguage(LionWebVersions lionWebVersion, string name,
    params Language[] dependentLanguages)
{
    SerializationChunk serializationChunk =
        JsonUtils.ReadJsonFromString<SerializationChunk>(
            File.ReadAllText($"chunks/externalDefs/{lionWebVersion.VersionString}/{name}.json"));
    return new LanguageDeserializerBuilder()
        .WithLionWebVersion(lionWebVersion)
        .WithCompressedIds(new(KeepOriginal: true))
        .Build()
        .Deserialize(serializationChunk, dependentLanguages).ToArray();
}