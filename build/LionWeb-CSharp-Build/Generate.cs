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

using Examples.Shapes.Dynamic;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;
using LionWeb.CSharp.Generator;
using LionWeb.CSharp.Generator.Names;

void SerializeLanguagesLocally(string name, params Language[] languages)
{
    JsonUtils.WriteJsonToFile($"chunks/localDefs/{name}.json", LanguageSerializer.Serialize(languages));
}

SerializeLanguagesLocally("lioncore", M3Language.Instance);
SerializeLanguagesLocally("shapes", ShapesDefinition.Language);


DynamicLanguage[] DeserializeExternalLanguage(string name, params Language[] dependentLanguages) =>
    LanguageDeserializer.Deserialize(
        JsonUtils.ReadJsonFromFile<SerializationChunk>($"chunks/externalDefs/{name}.json"),
        dependentLanguages
    ).ToArray();


var libraryLanguage = DeserializeExternalLanguage("library").First();
var multiLanguage = DeserializeExternalLanguage("multi", libraryLanguage).First();
var withEnumLanguage = DeserializeExternalLanguage("with-enum").First();
withEnumLanguage.Name = "WithEnum";
var shapesLanguage = ShapesDefinition.Language;
shapesLanguage.Name = "Shapes";
var testLanguagesDefinitions = new TestLanguagesDefinitions();
var aLang = testLanguagesDefinitions.ALang;
var bLang = testLanguagesDefinitions.BLang;

List<Names> names =
[
    // TODO  get this working:
    // new Names(M3Language.Instance, "LionWeb.Duplicate.M3"),
    new (libraryLanguage, "Examples.Library.M2"),
    new (multiLanguage, "Examples.Multi.M2")
    {
        NamespaceMappings = { [libraryLanguage] = "Examples.Library.M2" }
    },
    new (withEnumLanguage, "Examples.WithEnum.M2"),
    new (shapesLanguage, "Examples.Shapes.M2"),
    new (testLanguagesDefinitions.ALang, "Examples.Circular.A")
    {
        NamespaceMappings = { [bLang] = "Examples.Circular.B" }
    },
    new (testLanguagesDefinitions.BLang, "Examples.Circular.B")
    {
        NamespaceMappings = { [aLang] = "Examples.Circular.A" }
    },
];


var generationPath = "../../test/LionWeb-CSharp-Test/languages/generated";
Directory.CreateDirectory(generationPath);

foreach (var name in names)
{
    var generator = new GeneratorFacade { Names = name };
    generator.Generate();
    Console.WriteLine($"generated code for: {name.Language.Name}");

    var path = @$"{generationPath}/{name.Language.Name}.g.cs";
    generator.Persist(path);
    Console.WriteLine($"persisted to: {path}");
}