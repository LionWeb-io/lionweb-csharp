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

#pragma warning disable 1591

namespace LionWeb.CSharp.Generator.Impl;

using Core.M2;
using Core.M3;
using Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

// use https://roslynquoter.azurewebsites.net/ to learn how to build C# ASTs

public class DefinitionGenerator(INames names) : GeneratorBase(names)
{
    private IEnumerable<Enumeration> Enumerations => Language.Entities.OfType<Enumeration>();

    private IEnumerable<Classifier> Classifiers => Language.Entities.OfType<Classifier>();

    public CompilationUnitSyntax DefinitionFile() =>
        CompilationUnit()
            .WithMembers(List<MemberDeclarationSyntax>([
                    FileScopedNamespaceDeclaration(ParseName(_names.NamespaceName))
                        .WithNamespaceKeyword(Prelude())
                        .WithMembers(List(
                            new List<MemberDeclarationSyntax> { new LanguageGenerator(_names).LanguageClass() }
                                .Concat(new FactoryGenerator(_names).FactoryTypes())
                                .Concat(Classifiers.Select(
                                    c => new ClassifierGenerator(c, _names).ClassifierType()))
                                .Concat(Enumerations.Select(e => new EnumGenerator(e, _names).EnumType()))
                        ))
                        .WithUsings(List(CollectUsings()))
                ])
            )
            .NormalizeWhitespace();

    private SyntaxToken Prelude() =>
        Token(
            ParseLeadingTrivia(
                """
                    // Generated by the C# M2TypesGenerator: modify at your own risk!
                
                    // ReSharper disable InconsistentNaming
                    // ReSharper disable SuggestVarOrType_SimpleTypes
                    // ReSharper disable SuggestVarOrType_Elsewhere
                    
                    #pragma warning disable 1591
                    #nullable enable
                """),
            SyntaxKind.NamespaceKeyword,
            TriviaList()
        );

    private UsingDirectiveSyntax[] CollectUsings() =>
        _names.UsedTypes
            .Select(t => t.Namespace)
            .Where(n => n != null)
            .Prepend(typeof(EqualityExtensions).Namespace)
            .Prepend(typeof(BuiltInsLanguage).Namespace)
            .Distinct()
            .Order()
            .Select(n => UsingDirective(ParseName(n)))
            .Concat(PrimitiveTypesAsUsings())
            .ToArray();

    private IEnumerable<UsingDirectiveSyntax> PrimitiveTypesAsUsings() =>
        Language.Entities.OfType<PrimitiveType>().Select(p =>
            UsingDirective(
                NameEquals(IdentifierName(p.Name)),
                AsType(typeof(string))
            )
        );
}