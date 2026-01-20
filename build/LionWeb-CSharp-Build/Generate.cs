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
using Io.Lionweb.Mps.Specific.V2024_1;
using LionWeb.Build.Languages;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Migration;
using LionWeb.Core.Serialization;
using LionWeb.Generator;
using LionWeb.Generator.GeneratorExtensions;
using LionWeb.Generator.Impl;
using LionWeb.Generator.Names;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
    // If we update MPS-Specific in MPS or source generators, we want to re-generate it. Otherwise, we use the shipped version.
    // Follow the 3 steps below in order to generate MPS-specific languages.
    // After the generation, content of the generated files should be manually copied to where it is needed:
    // To the relevant sub folders in LionWeb.Generator.MpsSpecific/VersionSpecific
    // Make sure the generated language class extends/implements the followings: LanguageBase<ISpecificFactory>, ISpecificLanguage
    // 1) Comment out the line below  
    var specificLanguage = ISpecificLanguage.Get(lionWebVersion);
    // 2) Uncomment the line below
    // var specificLanguage = DeserializeExternalLanguage(lionWebVersion, "MpsSpecific-metamodel-annotated").First();
    var deprecatedLang = DeserializeExternalLanguage(lionWebVersion, "deprecated", specificLanguage).First();
    deprecatedLang.Name = "Deprecated";
    var testLanguage = DeserializeExternalLanguage(lionWebVersion, "testLanguage", specificLanguage).First();
    var nullableReferencesTestLanguage = new DynamicLanguageCloner(lionWebVersion).Clone(testLanguage);
    nullableReferencesTestLanguage.Name = "NullableReferencesTestLanguage";
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
    var generalNodeLang = testLanguagesDefinitions.GeneralNodeLang;
    var lowerCaseLang = testLanguagesDefinitions.LowerCaseLang;
    var upperCaseLang = testLanguagesDefinitions.UpperCaseLang;
    var namespaceStartsWithLionWebLang = testLanguagesDefinitions.NamespaceStartsWithLionWebLang;
    var namespaceContainsLionWebLang = testLanguagesDefinitions.NamespaceContainsLionWebLang;
    var languageWithLionWebNamedConcepts = testLanguagesDefinitions.LanguageWithLionWebNamedConcepts;
    var featureSameNameAsContainingConcept = testLanguagesDefinitions.FeatureSameNameAsContainingConcept;
    var customPrimitiveTypeLang = testLanguagesDefinitions.CustomPrimitiveTypeLang;

    var lionWebVersionNamespace = "V" + lionWebVersion.VersionString.Replace('.', '_');
    string prefix = $"LionWeb.Core.Test.Languages.Generated.{lionWebVersionNamespace}";

    IDictionary<PrimitiveType, Type> aLangBLangPrimitiveTypes = new Dictionary<PrimitiveType, Type>
    {
        { aLang.FindByKey<PrimitiveType>("key-AType"), typeof(CustomType)},
        { bLang.FindByKey<PrimitiveType>("key-BType"), typeof(LionWeb.Build.Languages.SubNamespace.CustomType)},
    };
    List<Names> names =
    [
        new(libraryLanguage, $"{prefix}.Library.M2"),
        new(multiLanguage, $"{prefix}.Multi.M2") { NamespaceMappings = { [libraryLanguage] = $"{prefix}.Library.M2" } },
        new(withEnumLanguage, $"{prefix}.WithEnum.M2"),
        new(shapesLanguage, $"{prefix}.Shapes.M2")
        {
            PrimitiveTypeMappings = { { shapesLanguage.FindByKey<PrimitiveType>("key-Time"), typeof(TimeOnly) } }
        },
        new(aLang, $"{prefix}.Circular.A") { NamespaceMappings = { [bLang] = $"{prefix}.Circular.B" }, PrimitiveTypeMappings = aLangBLangPrimitiveTypes },
        new(bLang, $"{prefix}.Circular.B") { NamespaceMappings = { [aLang] = $"{prefix}.Circular.A" }, PrimitiveTypeMappings = aLangBLangPrimitiveTypes },
        new(tinyRefLang, $"{prefix}.TinyRefLang"),
        new(deprecatedLang, $"{prefix}.DeprecatedLang")
        {
            PrimitiveTypeMappings =
            {
                {
                    deprecatedLang.FindByKey<PrimitiveType>(
                        "MDkzNjAxODQtODU5OC00NGU3LTliZjUtZmIxY2U0NWE0ODBhLzc4MTUyNDM0Nzk0ODc5OTM0ODQ"),
                    typeof(decimal)
                }
            }
        },
        new(multiInheritLang, $"{prefix}.MultiInheritLang"),
        new(namedLang, $"{prefix}.NamedLang"),
        new(namedLangReadInterfaces, $"{prefix}.NamedLangReadInterfaces"),
        new(generalNodeLang, $"{prefix}.GeneralNodeLang"),
        new(testLanguage, $"{prefix}.TestLanguage"),
        new(nullableReferencesTestLanguage, $"{prefix}.NullableReferencesTestLang"),
        new(namespaceStartsWithLionWebLang, $"LionWeb.Generated.{lionWebVersionNamespace}.namespaceStartsWithLionWebLang"),
        new(namespaceContainsLionWebLang, $"io.LionWeb.Generated.{lionWebVersionNamespace}.namespaceStartsWithLionWebLang"),
        new(languageWithLionWebNamedConcepts, $"{prefix}.LanguageWithLionWebNamedConcepts"),
        new(featureSameNameAsContainingConcept, $"{prefix}.FeatureSameNameAsContainingConcept"),
        // We don't really want these file in tests project, but update the version in Generator.
        // However, it's not worth writing a separate code path for this one language (as we want to externalize it anyways).
        // Step 3: Uncomment the line below 
        // new(specificLanguage, $"Io.Lionweb.Mps.Specific.{lionWebVersionNamespace}")
    ];

    Dictionary<Language, GeneratorConfig> configs = new()
    {
        { namedLangReadInterfaces, new() { WritableInterfaces = false } },
        { nullableReferencesTestLanguage, new() { UnresolvedReferenceHandling = UnresolvedReferenceHandling.ReturnAsNull } }
    };

    if (sdtLang != null)
        names.Add(new(sdtLang, $"{prefix}.SDTLang"));

    if (keywordLang != null)
        names.Add(new(keywordLang, $"@namespace.@int.@public.{lionWebVersionNamespace}")
        {
            PrimitiveTypeMappings = { { keywordLang.FindByKey<PrimitiveType>("key-keyword-prim"), typeof(CustomEnumType) } }
        });

    if (lowerCaseLang != null)
        names.Add(new(lowerCaseLang, $"{prefix}.myLowerCaseLang"));
    
    if (upperCaseLang != null)
        names.Add(new(upperCaseLang, $"{prefix}.MYUpperCaseLang"));

    if (customPrimitiveTypeLang != null)
        names.Add(new(customPrimitiveTypeLang, $"{prefix}.CustomPrimitiveTypeLang")
        {
            PrimitiveTypeMappings = testLanguagesDefinitions.CustomPrimitiveTypeMapping
        });
    
    names.AddRange(testLanguagesDefinitions
        .MixedLangs
        .Select(l =>
            new Names(l, $"{prefix}.Mixed.{l.Name}")
            {
                NamespaceMappings = testLanguagesDefinitions
                    .MixedLangs
                    .Select(n => (n, $"{prefix}.Mixed.{n.Name}"))
                    .ToDictionary()
            }
        )
    );

    var generationPath = "../../test/LionWeb.Core.Test.Languages/Generated";
    Directory.CreateDirectory(generationPath);

    foreach (var name in names)
    {
        var generator = new GeneratorFacade
        {
            Names = name,
            LionWebVersion = lionWebVersion,
            Config = configs.GetValueOrDefault(name.Language) ?? new()
        };
        var compilationUnit = generator.Generate();
        if (name.Language == deprecatedLang)
        {
            compilationUnit = AddM2DeprecatedAnnotations(compilationUnit, deprecatedLang, specificLanguage, generator);
        }
        Console.WriteLine($"generated code for: {name.Language.Name}");

        var path = $"{generationPath}/{lionWebVersionNamespace}/{name.Language.Name}.g.cs";
        generator.Persist(path, compilationUnit);
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

CompilationUnitSyntax AddM2DeprecatedAnnotations(CompilationUnitSyntax compilationUnitSyntax, DynamicLanguage deprecatedLang, ISpecificLanguage specificLanguage, GeneratorFacade generatorFacade)
{
    int i = 0;
    var annotations = M1Extensions.Descendants<IKeyed>(deprecatedLang)
        .Select(keyed => (keyed,
            ann: keyed.GetAnnotations().FirstOrDefault(a => a.GetClassifier() == specificLanguage.Deprecated)))
        .Where(pair => pair is { ann: not null })
        .ToDictionary(
            pair => pair.keyed,
            pair =>
            {
                var creation = ObjectCreationExpression(IdentifierName(nameof(Deprecated)))
                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                        Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                            Literal($"deprecatedAnnotation{i}")))
                    )));

                var setProperties = pair.ann!
                    .CollectAllSetFeatures()
                    .OfType<Property>()
                    .ToList();

                if (setProperties.Count > 0)
                {
                    creation = creation.WithInitializer(InitializerExpression(
                        SyntaxKind.ObjectInitializerExpression,
                        SeparatedList<ExpressionSyntax>(
                            setProperties
                                .Select(p => AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName(p.Name.ToFirstUpper()),
                                    LiteralExpression(SyntaxKind.StringLiteralExpression,
                                        Literal((string)pair.ann.Get(p)))
                                )),
                            [Token(SyntaxKind.CommaToken)]
                        )
                    ));
                }

                return CollectionExpression(
                    SingletonSeparatedList<CollectionElementSyntax>(ExpressionElement(creation)));
            });

    return compilationUnitSyntax
        .AddUsing(specificLanguage.GetType())
        .AddM2Annotations(generatorFacade.Correlator, annotations);
}