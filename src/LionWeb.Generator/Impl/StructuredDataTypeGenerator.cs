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

namespace LionWeb.Generator.Impl;

using Core;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

/// <summary>
/// Generates StructuredDataType readonly record structs.
/// </summary>
public class StructuredDataTypeGenerator(StructuredDataType sdt, INames names, LionWebVersions lionWebVersion)
    : GeneratorBase(names, lionWebVersion)
{
    private string SdtName => sdt.Name.PrefixKeyword();
    private IEnumerable<Field> Fields => sdt.Fields.Ordered();

    /// <inheritdoc cref="StructuredDataTypeGenerator"/>
    public RecordDeclarationSyntax SdtType()
    {
        var members = Fields.SelectMany(Field);

        members = members.Concat([
            GenDefaultConstructor(),
            GenInternalConstructor(),
            GenGetStructuredDataType(),
            GenCollectAllSetFields(),
            GenGet(),
            GenToString()
        ]);

        return RecordDeclaration(SyntaxKind.RecordStructDeclaration,
                Token(SyntaxKind.RecordKeyword), SdtName)
            .WithAttributeLists(AsAttributes(
            [
                MetaPointerAttribute(sdt),
                ObsoleteAttribute(sdt)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))
            .WithClassOrStructKeyword(Token(SyntaxKind.StructKeyword))
            .WithBaseList(AsBase(AsType(typeof(IStructuredDataTypeInstance))))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithMembers(List(members))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));
    }

    #region DefaultConstructor

    private ConstructorDeclarationSyntax GenDefaultConstructor() =>
        Constructor(SdtName)
            .WithBody(AsStatements(Fields.Select(DefaultFieldInitializer)));

    private StatementSyntax DefaultFieldInitializer(Field field) =>
        Assignment(FieldField(field).ToString(), Null());

    #endregion

    #region InternalConstructor

    private ConstructorDeclarationSyntax GenInternalConstructor() =>
        Constructor(
                SdtName,
                Fields
                    .Select(f => Param(_names.FieldParam(f), NullableType(AsType(f.Type))))
                    .ToArray()
            )
            .WithModifiers(AsModifiers(SyntaxKind.InternalKeyword))
            .WithBody(AsStatements(Fields.Select(InternalFieldInitializer)));

    private StatementSyntax InternalFieldInitializer(Field field) =>
        Assignment(FieldField(field).ToString(), IdentifierName(_names.FieldParam(field)));

    #endregion

    private MemberDeclarationSyntax GenGetStructuredDataType()
    {
        return Method("GetStructuredDataType", AsType(typeof(StructuredDataType)), exprBody: MetaProperty())
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocInheritDoc());
    }

    private MemberAccessExpressionSyntax MetaProperty() =>
        MemberAccess(MemberAccess(LanguageType, IdentifierName("Instance")), _names.AsProperty(sdt));

    #region CollectAllSetFields

    private MemberDeclarationSyntax GenCollectAllSetFields() =>
        Method("CollectAllSetFields", AsType(typeof(IEnumerable<Field>)))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(
                new List<StatementSyntax> { ParseStatement("List<Field> result = [];") }
                    .Concat(Fields.Select(GenCollectAllSetFields))
                    .Append(ReturnStatement(IdentifierName("result")))
            ));

    private StatementSyntax GenCollectAllSetFields(Field field) =>
        IfStatement(BinaryExpression(
            SyntaxKind.NotEqualsExpression,
            FieldField(field),
            Null()
        ), ExpressionStatement(InvocationExpression(
            MemberAccess(IdentifierName("result"), IdentifierName("Add")),
            AsArguments([MetaProperty(field)])
        )));

    #endregion

    #region Get

    private MemberDeclarationSyntax GenGet() =>
        Method("Get", NullableType(AsType(typeof(object))), [
                Param("field", AsType(typeof(Field)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(
                Fields.Select(GenGetInternal)
                    .Append(ThrowStatement(NewCall([IdentifierName("field")], AsType(typeof(UnsetFieldException)))))
            ));

    private StatementSyntax GenGetInternal(Field field) =>
        IfStatement(GenEqualsIdentityField(field),
            ReturnStatement(FieldProperty(field))
        );

    private MemberDeclarationSyntax GenToString() =>
        Method(
                "ToString",
                AsType(typeof(string)),
                exprBody: InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
                    .WithContents(List(
                        Fields
                            .Select(f => new List<InterpolatedStringContentSyntax>
                            {
                                AsInterpolatedString(FieldProperty(f) + " = "), Interpolation(FieldField(f))
                            })
                            .Intersperse([AsInterpolatedString(",")])
                            .SelectMany(l => l)
                            .Prepend(AsInterpolatedString(SdtName + " " + "{{"))
                            .Append(AsInterpolatedString(" }}"))))
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword));

    private InterpolatedStringTextSyntax AsInterpolatedString(string text) =>
        InterpolatedStringText()
            .WithTextToken(Token(
                TriviaList(),
                SyntaxKind.InterpolatedStringTextToken,
                text,
                text,
                TriviaList()
            ));

    private BinaryExpressionSyntax LazyFieldNotNull(Field field, ExpressionSyntax condition) =>
        BinaryExpression(
            SyntaxKind.LogicalAndExpression,
            condition,
            BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                LazyFieldValue(field),
                Null()
            )
        );

    private InvocationExpressionSyntax GenEqualsIdentityField(Field field) =>
        InvocationExpression(MemberAccess(MetaProperty(field), IdentifierName("EqualsIdentity")),
            AsArguments([IdentifierName("field")])
        );

    #endregion

    private List<MemberDeclarationSyntax> Field(Field field)
    {
        var memberField = AstExtensions.Field(FieldField(field).ToString(), NullableType(AsType(field.Type)))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

        var property = PropertyDeclaration(AsType(field.Type), Identifier(FieldProperty(field).ToString()))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([MetaPointerAttribute(field)]))
            .WithAccessorList(AccessorList(List([
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithExpressionBody(ArrowExpressionClause(BinaryExpression(
                                    SyntaxKind.CoalesceExpression,
                                    FieldField(field),
                                    ThrowExpression(NewCall([MetaProperty(field)],
                                        AsType(typeof(UnsetFieldException))))
                                )))
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                                .WithExpressionBody(ArrowExpressionClause(AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    FieldField(field),
                                    Value())))
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        ]
                    )
                )
            );

        return [memberField, property];
    }

    private MemberAccessExpressionSyntax LazyFieldValue(Field field) =>
        MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            FieldField(field),
            IdentifierName("Value")
        );
}