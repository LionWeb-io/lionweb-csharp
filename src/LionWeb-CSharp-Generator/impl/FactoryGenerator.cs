﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.CSharp.Generator.Impl;

using Core;
using Core.M2;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

/// <summary>
/// Generates Factory class.
/// </summary>
public class FactoryGenerator(INames names) : GeneratorBase(names)
{
    private string FactoryName => _names.FactoryName;

    private IEnumerable<Classifier> ConcreteClassifiers =>
        Language
            .Entities
            .OfType<Classifier>()
            .Where(c => c is Concept { Abstract: false } or Annotation);

    private IEnumerable<Enumeration> Enumerations => Language.Entities.OfType<Enumeration>();

    /// <inheritdoc cref="FactoryGenerator"/>
    public ClassDeclarationSyntax FactoryClass() =>
        ClassDeclaration(FactoryName)
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithBaseList(AsBase(AsType(typeof(AbstractBaseNodeFactory))))
            .WithMembers(List(new List<MemberDeclarationSyntax>
            {
                GenLanguageField(), GenConstructor(), GenCreateNode(), GenGetEnumerationLiteral()
            }.Concat(ConcreteClassifiers.SelectMany(GenNewMethods))));

    private FieldDeclarationSyntax GenLanguageField() =>
        Field("_language", LanguageType)
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

    private ConstructorDeclarationSyntax GenConstructor() =>
        Constructor(FactoryName, Param("language", LanguageType))
            .WithInitializer(Initializer("language"))
            .WithBody(AsStatements([Assignment("_language", IdentifierName("language"))]));


    #region CreateNode

    private MethodDeclarationSyntax GenCreateNode() =>
        Method("CreateNode", AsType(typeof(INode)), [
                Param("id", AsType(typeof(string))),
                Param("classifier", AsType(typeof(Classifier)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(
                ConcreteClassifiers
                    .Select(GenCreateNode)
                    .Append(ThrowStatement(NewCall([IdentifierName("classifier")],
                        AsType(typeof(UnsupportedClassifierException))))
                    )
            ));

    private StatementSyntax GenCreateNode(Classifier cl) =>
        IfStatement(
            InvocationExpression(
                MemberAccess(MemberAccess(IdentifierName("_language"), _names.AsProperty(cl)),
                    IdentifierName("EqualsIdentity")
                ),
                AsArguments([IdentifierName("classifier")])
            ),
            ReturnStatement(Call(AsNewMethod(cl), IdentifierName("id")))
        );

    #endregion

    #region GetEnumerationLiteral

    private MethodDeclarationSyntax GenGetEnumerationLiteral() =>
        Method("GetEnumerationLiteral", AsType(typeof(Enum)), [Param("literal", AsType(typeof(EnumerationLiteral)))])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(
                Enumerations
                    .Select(GenGetEnumerationLiteral)
                    .Append(
                        ThrowStatement(NewCall([IdentifierName("literal")],
                            AsType(typeof(UnsupportedEnumerationLiteralException))
                        ))
                    )
            ));

    private StatementSyntax GenGetEnumerationLiteral(Enumeration enumeration) =>
        IfStatement(
            InvocationExpression(
                MemberAccess(MemberAccess(IdentifierName("_language"), _names.AsProperty(enumeration)),
                    IdentifierName("EqualsIdentity")
                ),
                AsArguments([ParseExpression("literal.GetEnumeration()")])
            ),
            ReturnStatement(InvocationExpression(Generic("EnumValueFor", _names.AsType(enumeration, false)),
                AsArguments([IdentifierName("literal")]))
            )
        );

    #endregion

    #region New / Create Methods

    private IEnumerable<MethodDeclarationSyntax> GenNewMethods(Classifier classifier) =>
        new List<MethodDeclarationSyntax> { GenNewMethod(classifier), GenCreateMethod(classifier) };

    private MethodDeclarationSyntax GenNewMethod(Classifier classifier) =>
        Method(AsNewMethod(classifier), AsType(classifier), [Param("id", AsType(typeof(string)))],
                NewCall([IdentifierName("id")])
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword));

    private MethodDeclarationSyntax GenCreateMethod(Classifier classifier) =>
        Method(AsCreateMethod(classifier), AsType(classifier),
                exprBody: Call(AsNewMethod(classifier), ParseExpression("GetNewId()"))
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword));

    #endregion

    private string AsNewMethod(Classifier classifier) =>
        $"New{classifier.Name}";

    private string AsCreateMethod(Classifier classifier) =>
        $"Create{classifier.Name}";
}