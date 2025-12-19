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

namespace LionWeb.Generator.Test;

using Core;
using Core.M2;
using Core.M3;
using Core.Serialization;
using Core.Test.Languages.Generated.V2023_1.MultiInheritLang;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using GeneratorExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

[TestClass]
public class PostprocessGeneratorTests
{
    [TestMethod]
    public void AddSealed()
    {
        var language = MultiInheritLangLanguage.Instance;

        var generator = new GeneratorFacade
        {
            Names = new Names(language, "TestLanguage"),
            LionWebVersion = TestLanguageLanguage.Instance.LionWebVersion
        };

        var compilationUnit = generator.Generate();
        var correlator = generator.Correlator;

        var classifiersWithoutSpecializations = language.Entities
            .OfType<Classifier>()
            .Where(c => !c.AllSpecializations([language]).Any())
            .Select(c => correlator.FindAll<IClassifierToMainCorrelation>(c).Single());

        foreach (var correlation in classifiersWithoutSpecializations)
        {
            var typeDeclaration = correlation.LookupIn(compilationUnit);
            compilationUnit = compilationUnit.ReplaceNode(
                typeDeclaration,
                typeDeclaration
                    .AddModifiers(Token(SyntaxKind.SealedKeyword))
            );
        }

        var path = Path.GetTempFileName();
        try
        {
            generator.Persist(path, compilationUnit);
            Console.WriteLine($"Wrote output to {path}");

            var generatedContent = File.ReadAllText(path);
            Assert.Contains("public partial sealed class CombinedConcept", generatedContent);
        } finally
        {
            if (Path.Exists(path))
                File.Delete(path);
        }
    }

    [TestMethod]
    public void SplitClassesInFiles()
    {
        // Supposed to check out https://github.com/LionWeb-io/sysmlv2-demo/tree/main next to lionweb-csharp
        var sysml2Dir = "./../../../../../../sysmlv2-demo/models";

        SerializationChunk typesChunk =
            JsonUtils.ReadJsonFromString<SerializationChunk>(File.ReadAllText($"{sysml2Dir}/types_lionweb.json"));
        SerializationChunk kermlChunk =
            JsonUtils.ReadJsonFromString<SerializationChunk>(
                File.ReadAllText($"{sysml2Dir}/kerml_lionweb_lionweb.json"));
        SerializationChunk sysmlChunk =
            JsonUtils.ReadJsonFromString<SerializationChunk>(
                File.ReadAllText($"{sysml2Dir}/SysML_lionweb_lionweb.json"));

        var lionWebVersion = LionWebVersions.v2023_1;

        List<Language> dependentLanguages = [];

        var typesLanguage = Deserialize(lionWebVersion, typesChunk, dependentLanguages);
        dependentLanguages.Add(typesLanguage);
        var kermlLanguage = Deserialize(lionWebVersion, kermlChunk, dependentLanguages);
        dependentLanguages.Add(kermlLanguage);
        var sysmlLanguage = Deserialize(lionWebVersion, sysmlChunk, dependentLanguages);
        dependentLanguages.Add(sysmlLanguage);

        var namespaceMappings = new Dictionary<Language, string>
        {
            { typesLanguage, "io.LionWeb.SysMl2.Types" },
            { kermlLanguage, "io.LionWeb.SysMl2.KerMl" },
            { sysmlLanguage, "io.LionWeb.SysMl2.SysMl2" },
        };

        var primitiveTypeMappings = new Dictionary<PrimitiveType, Type>
        {
            { typesLanguage.FindByKey<PrimitiveType>("types-Boolean"), typeof(bool) },
            { typesLanguage.FindByKey<PrimitiveType>("types-Integer"), typeof(int) },
            { typesLanguage.FindByKey<PrimitiveType>("types-Real"), typeof(decimal) },
            { typesLanguage.FindByKey<PrimitiveType>("types-String"), typeof(string) },
            { typesLanguage.FindByKey<PrimitiveType>("types-UnlimitedNatural"), typeof(decimal) }
        };

        var basePath = Directory.CreateTempSubdirectory();
        Console.WriteLine($"BasePath: {basePath}");

        foreach (var language in dependentLanguages)
        {
            var generator = new GeneratorFacade
            {
                Names = new Names(language, namespaceMappings[language])
                {
                    NamespaceMappings = namespaceMappings, PrimitiveTypeMappings = primitiveTypeMappings
                },
                LionWebVersion = lionWebVersion
            };

            var compilationUnit = generator.Generate();

            List<CompilationUnitSyntax> compilationUnits = [];

            foreach (var ns in compilationUnit.Members.OfType<FileScopedNamespaceDeclarationSyntax>())
            {
                foreach (var type in ns.Members.OfType<BaseTypeDeclarationSyntax>())
                {
                    var copy = compilationUnit.ReplaceNode(ns,
                        ns.WithMembers(SingletonList<MemberDeclarationSyntax>(type)));
                    var namespaces = namespaceMappings[language].Split('.');
                    var directory = Path.Combine([basePath.FullName, ..namespaces]);
                    Directory.CreateDirectory(directory);
                    var file = Path.Combine(directory, $"{type.Identifier.Text}.g.cs");
                    generator.Persist(file, copy, false);
                    Console.WriteLine($"Wrote output to {file}");
                }
            }
        }
    }

    private static DynamicLanguage Deserialize(IVersion2023_1 lionWebVersion, SerializationChunk typesChunk,
        List<Language> dependentLanguages) =>
        new LanguageDeserializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build()
            .Deserialize(typesChunk, dependentLanguages).First();
}