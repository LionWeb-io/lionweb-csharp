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

using Core.M2;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;
using Property = Core.M3.Property;

/// <summary>
/// Generates Language class.
/// Covers members:
/// - Instance field
/// - Entities property
/// - DependsOn property
/// - GetFactory()
/// - _key field
/// - Key property
/// - _name field
/// - Name property
/// - _version property
/// - Version field
/// - LanguageEntity fields
/// - LanguageEntity properties
/// - Feature fields
/// - Feature properties
/// - EnumerationLiteral fields
/// - EnumerationLiteral properties
/// </summary>
/// <seealso cref="LanguageConstructorGenerator"/>
public class LanguageGenerator(INames names) : LanguageGeneratorBase(names)
{
    private IdentifierNameSyntax FactoryInterfaceType => _names.FactoryInterfaceType;

    /// <inheritdoc cref="LanguageGenerator"/>
    public ClassDeclarationSyntax LanguageClass() =>
        ClassDeclaration(LanguageName)
            .WithAttributeLists(AsAttributes([
                AsAttribute(AsType(typeof(LionCoreLanguage)), [
                    ("Key", names.Language.Key.AsLiteral()),
                    ("Version", names.Language.Version.AsLiteral())
                ])
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithBaseList(AsBase(AsType(typeof(LanguageBase<>), generics: FactoryInterfaceType)))
            .WithMembers(List(new List<MemberDeclarationSyntax>
                {
                    GenLanguageInstance(),
                    new LanguageConstructorGenerator(_names).GenConstructor(),
                    GenEntities(),
                    GenDependsOn(),
                    GenGetFactory()
                }
                .Concat(LanguageKeyMembers())
                .Concat(LanguageNameMembers())
                .Concat(LanguageVersionFieldMembers())
                .Concat(Language.Entities.SelectMany(EntityLanguageMembers))));

    private FieldDeclarationSyntax GenLanguageInstance() =>
        Field("Instance", LanguageType,
                NewLazy(NewCall([Language.GetId().AsLiteral()]), LanguageType, true)
            )
            .WithModifiers(AsModifiers(
                SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword
            ));

    private PropertyDeclarationSyntax GenEntities() =>
        ReadOnlyProperty("Entities", AsType(typeof(IReadOnlyList<LanguageEntity>)),
                Collection(
                    Language.Entities.Where(e => e is not PrimitiveType)
                        .Select(AsProperty)
                )
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc());

    private PropertyDeclarationSyntax GenDependsOn() =>
        ReadOnlyProperty("DependsOn", AsType(typeof(IReadOnlyList<Language>)),
                Collection(
                    Language.DependsOn.Select(d => _names.MetaProperty(d))
                )
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc());

    private MethodDeclarationSyntax GenGetFactory() =>
        Method("GetFactory", FactoryInterfaceType, exprBody: NewCall([ThisExpression()], _names.FactoryType))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc());

    #region EntityLanguageMembers

    private IEnumerable<MemberDeclarationSyntax> EntityLanguageMembers(LanguageEntity entity)
    {
        Type type = entity switch
        {
            Annotation => typeof(Annotation),
            Concept => typeof(Concept),
            Interface => typeof(Interface),
            Enumeration => typeof(Enumeration),
            PrimitiveType => typeof(PrimitiveType),
            _ => throw new ArgumentException($"unsupported entity: {entity}", nameof(entity))
        };

        List<MemberDeclarationSyntax> result =
        [
            Field(LanguageFieldName(entity), AsType(typeof(Lazy<>), generics: AsType(type)))
                .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword)),
            ReadOnlyProperty(AsProperty(entity).ToString(), AsType(type),
                MemberAccess(IdentifierName(LanguageFieldName(entity)), IdentifierName("Value"))
            )
        ];

        switch (entity)
        {
            case Classifier classifier:
                result.AddRange(classifier.Features.SelectMany(FeatureLanguageMember));
                break;
            case Enumeration enumeration:
                result.AddRange(enumeration.Literals.SelectMany(LiteralLanguageMember));
                break;
            default:
                // fall-through
                break;
        }

        return result;
    }

    private IEnumerable<MemberDeclarationSyntax> FeatureLanguageMember(Feature feature)
    {
        Type type = feature switch
        {
            Property => typeof(Property),
            Containment => typeof(Containment),
            Reference => typeof(Reference),
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

        return
        [
            Field(LanguageFieldName(feature), AsType(typeof(Lazy<>), generics: AsType(type)))
                .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword)),
            ReadOnlyProperty(_names.AsProperty(feature).Identifier.Text, AsType(type),
                MemberAccess(IdentifierName(LanguageFieldName(feature)), IdentifierName("Value"))
            )
        ];
    }

    private IEnumerable<MemberDeclarationSyntax> LiteralLanguageMember(EnumerationLiteral literal) =>
    [
        Field(LanguageFieldName(literal), AsType(typeof(Lazy<EnumerationLiteral>)))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword)),
        ReadOnlyProperty(AsProperty(literal).Identifier.Text, AsType(typeof(EnumerationLiteral)),
            MemberAccess(IdentifierName(LanguageFieldName(literal)), IdentifierName("Value"))
        )
    ];

    #endregion

    private IEnumerable<MemberDeclarationSyntax> LanguageKeyMembers() =>
        new List<MemberDeclarationSyntax>
        {
            Field("_key", AsType(typeof(string)), Language.Key.AsLiteral())
                .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ConstKeyword)),
            ReadOnlyProperty("Key", AsType(typeof(string)), IdentifierName("_key"))
                .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
                .Xdoc(XdocInheritDoc())
        };

    private IEnumerable<MemberDeclarationSyntax> LanguageNameMembers() =>
        new List<MemberDeclarationSyntax>
        {
            Field("_name", AsType(typeof(string)), Language.Name.AsLiteral())
                .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ConstKeyword)),
            ReadOnlyProperty("Name", AsType(typeof(string)), IdentifierName("_name"))
                .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
                .Xdoc(XdocInheritDoc())
        };

    private IEnumerable<MemberDeclarationSyntax> LanguageVersionFieldMembers() =>
        new List<MemberDeclarationSyntax>
        {
            Field("_version", AsType(typeof(string)), Language.Version.AsLiteral())
                .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ConstKeyword)),
            ReadOnlyProperty("Version", AsType(typeof(string)), IdentifierName("_version"))
                .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
                .Xdoc(XdocInheritDoc())
        };
}