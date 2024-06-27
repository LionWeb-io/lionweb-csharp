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
using LionWeb.CSharp.Generator.Impl;
using LionWeb.CSharp.Generator.Names;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;
using LionWeb.CSharp.Generator;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Frozen;
using System.Reflection;

var aLang = new DynamicLanguage("id-ALang") { Key = "key-ALang", Name = "ALang", Version = "1" };
var aConcept = aLang.Concept("id-AConcept", "key-AConcept", "AConcept");
var aEnum = aLang.Enumeration("id-aEnum", "key-AEnum", "AEnum");

var bLang = new DynamicLanguage("id-BLang") { Key = "key-BLang", Name = "BLang", Version = "2" };
var bConcept = bLang.Concept("id-BConcept", "key-BConcept", "BConcept");

aLang.AddDependsOn([bLang]);
bLang.AddDependsOn([aLang]);

aEnum.EnumerationLiteral("id-left", "key-left", "left");
aEnum.EnumerationLiteral("id-right", "key-right", "right");

aConcept.Reference("id-AConcept-BRef", "key-BRef", "BRef").IsOptional().OfType(bConcept);
bConcept.Reference("id-BConcept-ARef", "key-ARef", "ARef").IsOptional().OfType(aConcept);
bConcept.Property("id-BConcept-AEnumProp", "key-AEnumProp", "AEnumProp").OfType(aEnum);

List<LanguageNamespace> languages =
[
    new(aLang, "Examples.Cirular.A"),
    new(bLang, "Examples.Cirular.B"),
    new(ShapesDefinition.Language, "Examples.Shapes.Trial.M2")
];

string importFilePath = @"language-definitions.json";
try
{
    var memberInfos = typeof(SyntaxKind).GetMembers(BindingFlags.Public | BindingFlags.Static);
    var keywords = memberInfos
        .Where(i => i.Name.EndsWith("Keyword"))
        .Select(i => i.Name.Replace("Keyword", "").ToLower())
        .ToFrozenSet();

    var serializationChunk = JsonUtils.ReadJsonFromFile<SerializationChunk>(importFilePath);
    var loadedLanguages = LanguageDeserializer.Deserialize(serializationChunk).ToList();
    // foreach (var l in loadedLanguages)
    // {
    //     l.Name = l.Name.Replace(".", "_");
    //     if (keywords.Contains(l.Name.ToLower()))
    //         l.Name += "_";
    // }

    languages.AddRange(loadedLanguages.Select(l =>
        new LanguageNamespace(l,
            string.Join(".", l.Name.Split('.').Select(s => s.ToFirstUpper()))
        )));
} catch (FileNotFoundException)
{
    Console.WriteLine($"Cannot find input file {importFilePath}, skipping import.");
}

foreach (var pair in languages)
{
    var language = pair.Language;
    var names = new Names(language, pair.NamespaceName);
    foreach ((Language? lang, var ns) in languages.Except([pair]))
    {
        names.AddNamespaceMapping(lang, ns);
    }

    var generator = new GeneratorFacade { Names = names };

    generator.Generate();
    Console.WriteLine($"generated code for: {language.Name}");

    Directory.CreateDirectory(@"generated");
    var path = @$"generated/{language.Name}.g.cs";
    generator.Persist(path);
    Console.WriteLine($"persisted to: {path}");

    // foreach (var diagnostic in generator.Compile().Where(d => d.Severity == DiagnosticSeverity.Error))
    // {
    //     Console.WriteLine(diagnostic);
    // }
}

internal record LanguageNamespace(Language Language, string NamespaceName);