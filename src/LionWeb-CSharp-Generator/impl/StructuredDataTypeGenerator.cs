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
/// Generates StructuredDataType readonly record structs.
/// </summary>
public class StructuredDataTypeGenerator(StructuredDataType sdt, INames names) : GeneratorBase(names)
{
    /// <inheritdoc cref="StructuredDataTypeGenerator"/>
    public RecordDeclarationSyntax SdtType()
    {
        var members = sdt.Fields.SelectMany(Field);

        members = members.Concat([
            GenDefaultConstructor(),
            GenInternalConstructor(),
            GenGetStructuredDataType(),
            GenCollectAllSetFields(),
            GenGet()
        ]);

        return RecordDeclaration(SyntaxKind.RecordStructDeclaration,
                Token(SyntaxKind.RecordKeyword), sdt.Name)
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
        Constructor(sdt.Name)
            .WithBody(AsStatements(sdt.Fields.Select(DefaultFieldInitializer)));

    private StatementSyntax DefaultFieldInitializer(Field field)
    {
        ExpressionSyntax initializer = IsLazyField(field)
            ? NewCall([Null()])
            : Null();

        return Assignment(FieldField(field).ToString(), initializer);
    }

    #endregion
    
    #region InternalConstructor
    
    private ConstructorDeclarationSyntax GenInternalConstructor() =>
        Constructor(
            sdt.Name, 
            sdt.Fields.Select(f => Param(ParamField(f), NullableType(AsType(f.Type)))).ToArray()
        )
            .WithModifiers(AsModifiers(SyntaxKind.InternalKeyword))
            .WithBody(AsStatements(sdt.Fields.Select(InternalFieldInitializer)));

    private StatementSyntax InternalFieldInitializer(Field field)
    {
        var paramName = IdentifierName(ParamField(field));
        ExpressionSyntax initializer = IsLazyField(field)
            ? NewCall([paramName])
            : paramName;

        return Assignment(FieldField(field).ToString(), initializer);
    }

    private string ParamField(Field field) => FieldProperty(field).ToString().ToFirstLower() + "_";

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
                    .Concat(sdt.Fields.Select(GenCollectAllSetFields))
                    .Append(ReturnStatement(IdentifierName("result")))
            ));

    private StatementSyntax GenCollectAllSetFields(Field field)
    {
        ExpressionSyntax condition = BinaryExpression(
            SyntaxKind.NotEqualsExpression,
            FieldField(field),
            Null()
        );

        if (IsLazyField(field))
        {
            condition = LazyFieldNotNull(field, condition);
        }

        return IfStatement(condition, ExpressionStatement(InvocationExpression(
            MemberAccess(IdentifierName("result"), IdentifierName("Add")),
            AsArguments([MetaProperty(field)])
        )));
    }

    #endregion

    #region Get

    private MemberDeclarationSyntax GenGet() =>
        Method("Get", NullableType(AsType(typeof(object))), [
                Param("field", AsType(typeof(Field)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(
                sdt.Fields.Select(GenGetInternal)
                    .Append(ThrowStatement(NewCall([IdentifierName("field")], AsType(typeof(UnsetFieldException)))))
            ));

    private StatementSyntax GenGetInternal(Field field)
    {
        ExpressionSyntax condition = GenEqualsIdentityField(field);

        if (IsLazyField(field))
        {
            condition = LazyFieldNotNull(field, condition);
        }

        return IfStatement(condition,
            ReturnStatement(FieldProperty(field))
        );
    }

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
        var propertyType = AsType(field.Type);
        TypeSyntax memberFieldType = NullableType(propertyType);

        bool circumventSelfContainment = IsLazyField(field);

        if (circumventSelfContainment)
        {
            memberFieldType = AsType(typeof(NullableStructMember<>), propertyType);
            propertyType = NullableType(propertyType);
        }

        var memberField = AstExtensions.Field(FieldField(field).ToString(), memberFieldType)
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

        ExpressionSyntax getter;
        ExpressionSyntax setter;

        if (circumventSelfContainment)
        {
            getter = LazyFieldValue(field);
            setter = AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                FieldField(field),
                NewCall([Value()])
            );
        } else
        {
            getter = BinaryExpression(
                SyntaxKind.CoalesceExpression,
                FieldField(field),
                ThrowExpression(NewCall([MetaProperty(field)],
                    AsType(typeof(UnsetFieldException))))
            );
            setter = AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    FieldField(field),
                    Value())
                ;
        }

        var property = PropertyDeclaration(propertyType, Identifier(FieldProperty(field).ToString()))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([MetaPointerAttribute(field)]))
            .WithAccessorList(AccessorList(List([
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithExpressionBody(ArrowExpressionClause(getter))
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                                .WithExpressionBody(ArrowExpressionClause(setter))
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

    private bool IsLazyField(Field field) =>
        field.Type is StructuredDataType s && ContainsSelf(s, []);

    private bool ContainsSelf(Datatype datatype, HashSet<StructuredDataType> owners)
    {
        if (datatype is not StructuredDataType structuredDataType)
            return false;

        if (!owners.Add(structuredDataType))
            return true;

        return structuredDataType.Fields.Any(f => ContainsSelf(f.Type, [..owners]));
    }
}