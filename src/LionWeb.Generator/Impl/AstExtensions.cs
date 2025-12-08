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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// Convenience functions for Roslyn AST
public static partial class AstExtensions
{
    /// <paramref name="value"/> as Roslyn literal.
    public static LiteralExpressionSyntax AsLiteral(this string value) =>
        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value));

    /// <paramref name="value"/> as Roslyn literal.
    public static LiteralExpressionSyntax AsLiteral(this int value) =>
        LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

    /// <paramref name="value"/> as Roslyn literal.
    public static LiteralExpressionSyntax AsLiteral(this bool value) =>
        LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);

    /// <paramref name="values"/> ready to be fed to <see cref="SyntaxFactory.InvocationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax,Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax)"/>.
    public static ArgumentListSyntax AsArguments(IEnumerable<ExpressionSyntax> values) =>
        ArgumentList(SeparatedList(values.Select(Argument)));

    /// <paramref name="modifiers"/> ready to be fed to <see cref="BaseTypeDeclarationSyntax.WithModifiers"/>.
    public static SyntaxTokenList AsModifiers(params SyntaxKind[] modifiers) =>
        TokenList(modifiers.Select(Token));

    /// <paramref name="bases"/> ready to be fed to <see cref="TypeDeclarationSyntax.WithBaseList"/>.
    public static BaseListSyntax AsBase(params TypeSyntax?[] bases) =>
        BaseList(SeparatedList<BaseTypeSyntax>(bases.Where(b => b != null).Select(SimpleBaseType!)));

    /// <paramref name="attributes"/> ready to be fed to <see cref="BaseTypeDeclarationSyntax.WithAttributeLists"/>.
    public static SyntaxList<AttributeListSyntax> AsAttributes(IEnumerable<AttributeSyntax?> attributes) =>
        List(attributes.OfType<AttributeSyntax>().Select(a => AttributeList(SingletonSeparatedList(a))));

    /// <returns><c>[attributeName(parameters.string = parameters.expression, ...)</c></returns>
    public static AttributeSyntax AsAttribute(TypeSyntax attributeName,
        IEnumerable<(string, ExpressionSyntax)> parameters) =>
        Attribute(IdentifierName(attributeName.ToString()))
            .WithArgumentList(AttributeArgumentList(SeparatedList(
                parameters.Select(p =>
                    AttributeArgument(p.Item2)
                        .WithNameEquals(NameEquals(IdentifierName(p.Item1)))
                )
            )));

    /// Wraps <paramref name="expressions"/> in a CollectionExpression.
    public static CollectionExpressionSyntax Collection(IEnumerable<ExpressionSyntax> expressions) =>
        CollectionExpression(SeparatedList<CollectionElementSyntax>(expressions.Select(ExpressionElement)));

    /// Wraps <paramref name="statements"/> in a Block.
    public static BlockSyntax AsStatements(IEnumerable<StatementSyntax> statements) =>
        Block(List(statements));

    /// <returns><c>public type name => expression;</c></returns>
    public static PropertyDeclarationSyntax ReadOnlyProperty(string name, TypeSyntax type, ExpressionSyntax expression)
        => PropertyDeclaration(type, Identifier(name))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithExpressionBody(ArrowExpressionClause(expression))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    /// <returns><c>type name { get => getter; set => setter; }</c></returns>
    public static PropertyDeclarationSyntax Property(string name, TypeSyntax type, ExpressionSyntax getter,
        ExpressionSyntax setter) =>
        PropertyDeclaration(type, Identifier(name))
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(getter))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(setter))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ])));

    /// <returns><c>type name = init;</c></returns>
    public static FieldDeclarationSyntax Field(string name, TypeSyntax type, ExpressionSyntax? init = null)
    {
        var declarator = VariableDeclarator(Identifier(name));
        if (init != null)
        {
            declarator = declarator
                .WithInitializer(EqualsValueClause(init));
        }

        return FieldDeclaration(VariableDeclaration(type)
            .WithVariables(SingletonSeparatedList(declarator))
        );
    }

    /// <returns><c>type name = init;</c></returns>
    public static LocalDeclarationStatementSyntax Variable(string name, TypeSyntax type, ExpressionSyntax? init = null)
    {
        var declarator = VariableDeclarator(Identifier(name));

        if (init != null)
        {
            declarator = declarator
                .WithInitializer(EqualsValueClause(init));
        }

        return LocalDeclarationStatement(
            VariableDeclaration(type)
                .WithVariables(SingletonSeparatedList(declarator))
        );
    }

    /// <returns><c>public name(parameters)</c></returns>
    public static ConstructorDeclarationSyntax Constructor(string name, params ParameterSyntax[] parameters) =>
        ConstructorDeclaration(Identifier(name))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithParameterList(ParameterList(SeparatedList(parameters)));

    /// <returns><c>type name</c></returns>
    public static ParameterSyntax Param(string name, TypeSyntax type) =>
        Parameter(Identifier(name))
            .WithType(type);

    /// <returns><c> : base(paramNames)</c></returns>
    public static ConstructorInitializerSyntax Initializer(params string[] paramNames) =>
        ConstructorInitializer(
            SyntaxKind.BaseConstructorInitializer,
            AsArguments(paramNames.Select(IdentifierName))
        );

    /// <returns><c>name = expression;</c></returns>
    public static ExpressionStatementSyntax Assignment(string name, ExpressionSyntax expression) =>
        ExpressionStatement(AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(name),
            expression)
        );

    /// <returns><c>new Lazy&lt;type&gt;(() => expression).Value</c></returns>
    public static ExpressionSyntax NewLazy(ExpressionSyntax expression, TypeSyntax? type = null, bool value = false)
    {
        BaseObjectCreationExpressionSyntax creator;
        if (type != null)
        {
            creator = ObjectCreationExpression(Generic("Lazy", type));
        } else
        {
            creator = ImplicitObjectCreationExpression();
        }

        ExpressionSyntax result = creator
            .WithArgumentList(AsArguments([ParenthesizedLambdaExpression().WithExpressionBody(expression)]));

        if (value)
        {
            result = MemberAccess(
                result,
                IdentifierName("Value")
            );
        }

        return result;
    }

    /// <returns><c>name&lt;types&gt;</c></returns>
    public static GenericNameSyntax Generic(string name, params TypeSyntax[] types) =>
        GenericName(Identifier(name))
            .WithTypeArgumentList(TypeArgumentList(SeparatedList(types)));

    /// <returns><c>new type(arguments) { propertiesString = propertiesExpression }</c></returns>
    public static BaseObjectCreationExpressionSyntax NewCall(List<ExpressionSyntax> arguments, TypeSyntax? type = null,
        params (string, ExpressionSyntax)[] properties)
    {
        BaseObjectCreationExpressionSyntax creator;
        if (type != null)
        {
            creator = ObjectCreationExpression(type);
        } else
        {
            creator = ImplicitObjectCreationExpression();
        }

        var result = creator;

        if (arguments.Count != 0)
        {
            result = result
                .WithArgumentList(AsArguments(arguments));
        }

        if (properties != null && properties.Length != 0)
        {
            result = result
                .WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                    SeparatedList<ExpressionSyntax>(
                        properties.Select(p =>
                            AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(p.Item1),
                                p.Item2)
                        )
                    )
                ));
        }

        return result;
    }

    /// <returns><c>name(parameters)</c></returns>
    public static InvocationExpressionSyntax Call(string name, params ExpressionSyntax[] arguments)
    {
        var result = InvocationExpression(IdentifierName(name));

        if (arguments.Length != 0)
        {
            result = result
                .WithArgumentList(AsArguments(arguments));
        }

        return result;
    }

    /// <returns><c>name&lt;type&gt;(parameters)</c></returns>
    public static InvocationExpressionSyntax CallGeneric(string name, TypeSyntax type,
        params ExpressionSyntax[] arguments)
    {
        var result = InvocationExpression(
            GenericName(Identifier(name))
                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(type)))
        );

        if (arguments.Length != 0)
        {
            result = result
                .WithArgumentList(AsArguments(arguments));
        }

        return result;
    }

    /// <returns><c>returnType name(parameters) => exprBody;</c></returns>
    public static MethodDeclarationSyntax Method(string name, TypeSyntax? returnType = null,
        IEnumerable<ParameterSyntax>? parameters = null, ExpressionSyntax? exprBody = null)
    {
        var result = MethodDeclaration(returnType ?? Void(), Identifier(name));

        if (parameters != null)
        {
            var parameterSyntaxes = parameters.ToList();
            if (parameterSyntaxes.Count != 0)
            {
                result = result
                    .WithParameterList(ParameterList(SeparatedList(parameterSyntaxes)));
            }
        }

        if (exprBody != null)
        {
            result = result
                .WithExpressionBody(ArrowExpressionClause(exprBody))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        return result;
    }

    /// <returns><c>name.expression</c></returns>
    public static MemberAccessExpressionSyntax MemberAccess(ExpressionSyntax expression, SimpleNameSyntax name) =>
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);


    /// <returns><c>!=</c></returns>
    public static BinaryExpressionSyntax NotEquals(ExpressionSyntax left, ExpressionSyntax right) =>
        BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);

    /// <returns><c>==</c></returns>
    public static BinaryExpressionSyntax EqualsSign(ExpressionSyntax left, ExpressionSyntax right) =>
        BinaryExpression(SyntaxKind.EqualsExpression, left, right);

    /// <returns><c>||</c></returns>
    public static BinaryExpressionSyntax Or(ExpressionSyntax left, ExpressionSyntax right) =>
        BinaryExpression(SyntaxKind.LogicalOrExpression, left, right);

    /// <returns><c>&&</c></returns>
    public static BinaryExpressionSyntax And(ExpressionSyntax left, ExpressionSyntax right) =>
        BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);

    /// <returns><c>!</c></returns>
    public static PrefixUnaryExpressionSyntax Not(ExpressionSyntax inner) =>
        PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, inner);

    public static IsPatternExpressionSyntax IsNull(string name) =>
        IsPatternExpression(IdentifierName(name), ConstantPattern(Null()));

    /// <returns><paramref name="value"/> <c>?? throw</c> <paramref name="exception"/></returns>
    public static ExpressionSyntax NotNullOrThrow(ExpressionSyntax value, ExpressionSyntax exception) =>
        BinaryExpression(
            SyntaxKind.CoalesceExpression,
            value,
            ThrowExpression(exception)
        );

    /// <returns><c>void</c></returns>
    public static PredefinedTypeSyntax Void() =>
        PredefinedType(Token(SyntaxKind.VoidKeyword));

    /// <returns><c>var</c></returns>
    public static IdentifierNameSyntax Var() =>
        IdentifierName(Identifier(
            TriviaList(),
            SyntaxKind.VarKeyword,
            "var",
            "var",
            TriviaList()
        ));

    /// <returns><c>true</c></returns>
    public static LiteralExpressionSyntax True() =>
        LiteralExpression(SyntaxKind.TrueLiteralExpression);

    /// <returns><c>false</c></returns>
    public static LiteralExpressionSyntax False() =>
        LiteralExpression(SyntaxKind.FalseLiteralExpression);

    /// <returns><c>null</c></returns>
    public static LiteralExpressionSyntax Null() =>
        LiteralExpression(SyntaxKind.NullLiteralExpression);

    /// <returns><c>_</c></returns>
    public static IdentifierNameSyntax Underscore() =>
        IdentifierName
        (
            Identifier
            (
                TriviaList(),
                SyntaxKind.UnderscoreToken,
                "_",
                "_",
                TriviaList()
            )
        );

    /// <returns><c>default</c></returns>
    public static LiteralExpressionSyntax Default() =>
        LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword));

    /// <returns><c>value</c></returns>
    public static IdentifierNameSyntax Value() =>
        IdentifierName("value");

    /// <returns><c>this</c></returns>
    public static ThisExpressionSyntax This() =>
        ThisExpression();

    /// Adds <paramref name="element"/> between any two elements of <paramref name="source"/>.
    public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, T element)
    {
        bool first = true;
        foreach (T value in source)
        {
            if (!first)
                yield return element;
            yield return value;
            first = false;
        }
    }
}