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

namespace LionWeb.CSharp.Generator.Impl;

using Core;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

/// <summary>
/// Generates StructuredDatatype readonly record structs.
/// </summary>
public class StructuredDataTypeGenerator(StructuredDataType sdt, INames names) : GeneratorBase(names)
{
    /// <inheritdoc cref="StructuredDataTypeGenerator"/>
    public RecordDeclarationSyntax SdtType()
    {
        var containsSelf = ContainsSelf(sdt, []);
        
        return RecordDeclaration(SyntaxKind.RecordStructDeclaration,
                Token(SyntaxKind.RecordKeyword), sdt.Name)
            .WithAttributeLists(AsAttributes(
            [
                MetaPointerAttribute(sdt),
                ObsoleteAttribute(sdt)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))
            .WithClassOrStructKeyword(Token(SyntaxKind.StructKeyword))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithMembers(List(sdt.Fields.SelectMany(f => Field(f, containsSelf))))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
    }

    private bool ContainsSelf(Datatype datatype, HashSet<StructuredDataType> owners)
    {
        if (datatype is not StructuredDataType xxx)
            return false;
        
        if (!owners.Add(xxx))
            return true;

        return xxx.Fields.Any(f => ContainsSelf(f.Type, [..owners]));
    } 
    
    private List<MemberDeclarationSyntax> Field(Field field, bool containsSelf)
    {
        var propertyType = AsType(field.Type);
        var memberFieldType = NullableType(propertyType);

        bool circumventSelfContainment = containsSelf && field.Type is StructuredDataType;
        
        if (circumventSelfContainment)
        {
            propertyType = NullableType(propertyType);
            memberFieldType = NullableType(AsType(typeof(Lazy<>), propertyType));
        }

        var memberField = AstExtensions.Field(FieldField(field).ToString(), memberFieldType)
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

        var property = PropertyDeclaration(propertyType, Identifier(FieldProperty(field).ToString()))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword));

        if (circumventSelfContainment)
        {
            property = property
                .WithAccessorList(AccessorList(List([
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithExpressionBody(ArrowExpressionClause(
                                        ConditionalAccessExpression(
                                            FieldField(field),
                                            MemberBindingExpression(IdentifierName("Value"))
                                        )
                                    ))
                                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                                    .WithExpressionBody(ArrowExpressionClause(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            FieldField(field),
                                            NewCall([
                                                // ConditionalExpression(
                                                //     IsPatternExpression(
                                                //         Value(),
                                                //         DeclarationPattern(
                                                //             AsType(field.Type),
                                                //             SingleVariableDesignation(Identifier("v"))
                                                //         )
                                                //     ),
                                                //     IdentifierName("v"),
                                                //     Default()
                                                // )
                                                Value()
                                            ])
                                        )
                                    ))
                                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            ]
                        )
                    )
                );
        } else
        {
            property = property
                .WithAccessorList(AccessorList(List([
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithExpressionBody(ArrowExpressionClause(
                                        BinaryExpression(
                                            SyntaxKind.CoalesceExpression,
                                            FieldField(field),
                                            ThrowExpression(NewCall([MetaProperty(field)],
                                                AsType(typeof(UnsetFieldException))))
                                        )
                                    ))
                                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                                    .WithExpressionBody(ArrowExpressionClause(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            FieldField(field),
                                            Value())))
                                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                            ]
                        )
                    )
                );
        }

        property = property
            .WithAttributeLists(AsAttributes([MetaPointerAttribute(field)]));

        return [memberField, property];
    }
}

public readonly record struct FullyQualifiedName
{
    private readonly string _name;
    private readonly Lazy<FullyQualifiedName?> _nested;

    public string Name
    {
        get => _name ?? throw new InvalidValueException(null, null);
        init => _name = value ?? throw new InvalidValueException(null, null);
    }

    public FullyQualifiedName? Nested
    {
        get => _nested.Value;
        init => _nested = new(value);
    }
}