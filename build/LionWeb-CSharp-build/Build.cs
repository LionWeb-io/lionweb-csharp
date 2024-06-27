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

var chunksPath = "chunks";

void SaveChunk(string name, SerializationChunk chunk)
{
    JsonUtils.WriteJsonToFile($"{chunksPath}/{name}.json", chunk);
}


void SerializeLanguageLocally(Language language, string name)
{
    SaveChunk($"localDefs/{name}", LanguageSerializer.Serialize(language));
}

// -> Serialize the LionCore language (as defined in itself, i.e. using the C# LionCore M3 implementation) as JSON:
SerializeLanguageLocally(M3Language.Instance, "lioncore");
// This lioncore.json should be the same as externalDefs/lioncore.json modulo ordering.

// -> Serialize the Shapes language (as defined using the C# LionCore M3 implementation) as JSON:
SerializeLanguageLocally(ShapesDefinition.Language, "shapesLanguage");
// This localDefs/shapesLanguage-serialized.json should be the same as externalDefs/shapesLanguage.json module ordering.


Language[] DeserializeExternalLanguage(string name, params Language[] dependentLanguages)
{
    var serializationChunk = JsonUtils.ReadJsonFromFile<SerializationChunk>($"{chunksPath}/externalDefs/{name}.json");
    var languages = LanguageDeserializer.Deserialize(serializationChunk, dependentLanguages).ToArray();
    // Just run the serializer for now (without really checking anything), to see whether it crashes or not:
    LanguageSerializer.Serialize(languages);
    return languages;
}


// (adjust some languages' names for factory names:)

var lionCoreLanguage = M3Language.Instance;
// lionCoreLanguage.Name = "LionCore"; TODO
var libraryLanguage = DeserializeExternalLanguage("library").First();
var multiLanguage = DeserializeExternalLanguage("multi", libraryLanguage).First();
var withEnumLanguage = DeserializeExternalLanguage("with-enum").First();
(withEnumLanguage as DynamicLanguage).Name = "WithEnum";
var shapesLanguage = ShapesDefinition.Language;
shapesLanguage.Name = "Shapes";

var names = new Dictionary<string, INames>()
{
    // TODO: Does not work yet
    // ["LionCoreTypes.g.cs"] = new Names(lionCoreLanguage, "LionWeb.Duplicate.M3"),
    ["LibraryTypes.g.cs"] = new Names(libraryLanguage, "Examples.Library.M2"),
    ["MultiTypes.g.cs"] =
        new Names(multiLanguage, "Examples.Multi.M2")
        {
            NamespaceMappings = { [libraryLanguage] = "Examples.Library.M2" }
        },
    ["WithEnumTypes.g.cs"] = new Names(withEnumLanguage, "Examples.WithEnum.M2"),
    ["ShapesTypes.g.cs"] = new Names(shapesLanguage, "Examples.Shapes.M2")
};

var generationPath = "../../test/LionWeb-CSharp-Test/languages/structure";

void GenerateAndSaveAll()
{
    foreach (var name in names)
    {
        new GeneratorFacade() { Names = name.Value }.Persist($"{generationPath}/{name.Key}");
    }
}

// -> Generate C# types from the LionCore M3 and Shapes languages (as defined using the C# LionCore M3 implementation):
GenerateAndSaveAll();