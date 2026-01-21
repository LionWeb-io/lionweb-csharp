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
using Core.M1;
using Core.M3;
using Core.Test.Languages.Generated.V2024_1.MultiInheritLang;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using GeneratorExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Annotation = Core.M3.Annotation;

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
            LionWebVersion = language.LionWebVersion
        };

        var compilationUnit = generator.Generate();

        compilationUnit = compilationUnit.SealClassifiers(generator.Correlator, [language]);

        var workspace = new AdhocWorkspace();
        var generatedContent = Formatter.Format(compilationUnit, workspace).GetText().ToString();
        Assert.Contains("public sealed partial class CombinedConcept", generatedContent);
    }

    [TestMethod]
    public void SealedNoClassifiers()
    {
        var lionWebVersion = LionWebVersions.Current;
        var language = new DynamicLanguage("l", lionWebVersion) { Key = "lang", Name = "lang", Version = "lang" };
        language.Enumeration("enm", "enm", "enm");
        language.StructuredDataType("sdt", "sdt", "sdt");

        var generator = new GeneratorFacade
        {
            Names = new Names(language, "TestLanguage"),
            LionWebVersion = language.LionWebVersion
        };

        var compilationUnit = generator.Generate();

        compilationUnit = compilationUnit.SealClassifiers(generator.Correlator, [language]);

        var generatedContent = compilationUnit.GetText().ToString();
        Assert.DoesNotContain("sealed", generatedContent);
    }

    [TestMethod]
    public void SealedNoMatchingClassifiers()
    {
        var lionWebVersion = LionWebVersions.Current;
        var lang0 = new DynamicLanguage("lang0", lionWebVersion) { Key = "lang0", Name = "lang0", Version = "lang0" };
        lang0.Interface("iface", "iface", "iface");
        lang0.Concept("conc", "conc", "conc").IsAbstract(true);

        var lang1 = new DynamicLanguage("lang1", lionWebVersion) { Key = "lang1", Name = "lang1", Version = "lang1" };
        lang1.Concept("conc", "conc", "conc");

        var generator = new GeneratorFacade
        {
            Names = new Names(lang0, "TestLanguage"),
            LionWebVersion = lang0.LionWebVersion
        };

        var compilationUnit = generator.Generate();

        compilationUnit = compilationUnit.SealClassifiers(generator.Correlator, [lang0, lang1]);

        var generatedContent = compilationUnit.GetText().ToString();
        Assert.DoesNotContain("sealed", generatedContent);
    }

    [TestMethod]
    public void AddAnnotation()
    {
        var language = TestLanguageLanguage.Instance;

        var generator = new GeneratorFacade
        {
            Names = new Names(language, "TestLanguage"), LionWebVersion = language.LionWebVersion
        };

        var compilationUnit = generator.Generate();
        var correlator = generator.Correlator;

        int i = 0;
        var annotations = M1Extensions.Descendants<IKeyed>(language)
            .OfType<Annotation>()
            .ToDictionary(IKeyed (k) => k,
                v => (CollectionExpressionSyntax)ParseExpression($"""[new TestAnnotation("testAnnotation{i++}")]""")
                );

        compilationUnit = compilationUnit.AddM2Annotations(correlator, annotations);

        var generatedContent = compilationUnit.GetText().ToString();
        Assert.Contains("""[new TestAnnotation("testAnnotation0")]""", generatedContent);
    }
}