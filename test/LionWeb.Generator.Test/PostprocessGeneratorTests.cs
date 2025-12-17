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

using Core.M2;
using Core.M3;
using Core.Test.Languages.Generated.V2024_1.MultiInheritLang;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using GeneratorExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
}