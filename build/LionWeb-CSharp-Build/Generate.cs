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
    var specificLanguage = ISpecificLanguage.Get(lionWebVersion);
    var deprecatedLang = DeserializeExternalLanguage(lionWebVersion, "deprecated", specificLanguage).First();
    deprecatedLang.Name = "Deprecated";
    var shapesLanguage = ShapesDefinition.Language;
    shapesLanguage.Name = "Shapes";
    var aLang = testLanguagesDefinitions.ALang;
    var bLang = testLanguagesDefinitions.BLang;
    var tinyRefLang = testLanguagesDefinitions.TinyRefLang;
    var keywordLang = testLanguagesDefinitions.KeywordLang;

    string prefix = $"LionWeb.Core.Test.Languages.Generated.V{lionWebVersion.VersionString.Replace('.', '_')}";
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
    ];

    if (sdtLang != null)
        names.Add(new(sdtLang, $"{prefix}.SDTLang"));

    if (keywordLang != null)
        names.Add(new(keywordLang, $"@namespace.@int.@public"));

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
        var generator = new GeneratorFacade { Names = name, LionWebVersion = lionWebVersion };
        generator.Generate();
        Console.WriteLine($"generated code for: {name.Language.Name}");

        var path = @$"{generationPath}/V{lionWebVersion.VersionString.Replace('.', '_')}/{name.Language.Name}.g.cs";
        generator.Persist(path);
        Console.WriteLine($"persisted to: {path}");
    }
}

return;

void SerializeLanguagesLocally(LionWebVersions lionWebVersion, string name, params Language[] languages)
{
    JsonUtils.WriteNodesToStream(
        new FileStream($"chunks/localDefs/{lionWebVersion.VersionString}/{name}.json", FileMode.Create),
        new Serializer(lionWebVersion),
        languages.SelectMany(l => M1Extensions.Descendants<IReadableNode>(l, true, true)));
}

DynamicLanguage[] DeserializeExternalLanguage(LionWebVersions lionWebVersion, string name,
    params Language[] dependentLanguages)
{
    SerializationChunk serializationChunk =
        JsonUtils.ReadJsonFromString<SerializationChunk>(
            File.ReadAllText($"chunks/externalDefs/{lionWebVersion.VersionString}/{name}.json"));
    return new LanguageDeserializer(lionWebVersion, compressedIdConfig: new(KeepOriginal: true))
        .Deserialize(serializationChunk, dependentLanguages).ToArray();
}