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
/// Generates concept/annotation members relevant for containment handling.
/// Covers members:
/// - DetachChild()
/// - GetContainmentOf()
/// </summary>
public class ContainmentMethodsGenerator(Classifier classifier, INames names, LionWebVersions lionWebVersion)
    : ClassifierGeneratorBase(names, lionWebVersion)
{
    /// <inheritdoc cref="ContainmentMethodsGenerator"/>
    public IEnumerable<MemberDeclarationSyntax> ContainmentMethods()
    {
        if (!ContainmentsToImplement.Any())
            return Enumerable.Empty<MemberDeclarationSyntax>();

        return
        [
            GenDetachChild(),
            GenGetContainmentOf()
        ];
    }

    private IEnumerable<Containment> ContainmentsToImplement =>
        FeaturesToImplement(classifier).OfType<Containment>();

    #region DetachChild

    private MemberDeclarationSyntax GenDetachChild() =>
        Method("DetachChild", AsType(typeof(bool)), [Param("child", AsType(typeof(INode)))])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.DetachChild(child)"),
                        ReturnTrue()),
                    ParseStatement("Containment? c = GetContainmentOf(child);")
                }.Concat(ContainmentsToImplement.Select(GenDetachChild))
                .Append(ReturnStatement(false.AsLiteral()))
            ));

    private StatementSyntax GenDetachChild(Containment containment)
    {
        StatementSyntax statement = containment.Multiple switch
        {
            true => RemoveSelfParentCall(containment),
            false => Assignment(FeatureField(containment).ToString(), Null())
        };

        return IfStatement(InvocationExpression(
                MemberAccess(MetaProperty(containment), IdentifierName("EqualsIdentity")),
                AsArguments([IdentifierName("c")])),
            AsStatements([
                statement,
                ReturnTrue()
            ])
        );
    }

    private ExpressionStatementSyntax RemoveSelfParentCall(Containment containment) =>
        ExpressionStatement(Call("RemoveSelfParent",
            IdentifierName("child"),
            FeatureField(containment),
            MetaProperty(containment)
        ));

    private ReturnStatementSyntax ReturnTrue() =>
        ReturnStatement(true.AsLiteral());

    #endregion

    #region GetContainmentOf

    private MemberDeclarationSyntax GenGetContainmentOf() =>
        Method("GetContainmentOf", NullableType(AsType(typeof(Containment))), [Param("child", AsType(typeof(INode)))])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    ParseStatement("Containment? result = base.GetContainmentOf(child);"),
                    IfStatement(NotEquals(IdentifierName("result"), Null()),
                        ReturnStatement(IdentifierName("result")))
                }.Concat(ContainmentsToImplement.Select(GenGetContainmentOf))
                .Append(ReturnStatement(Null()))
            ));

    private StatementSyntax GenGetContainmentOf(Containment containment, int index)
    {
        string varName = $"child{index}";

        ExpressionSyntax condition = containment.Multiple switch
        {
            true => BinaryExpression(SyntaxKind.LogicalAndExpression,
                IsPatternExpression(
                    IdentifierName("child"),
                    DeclarationPattern(AsType(containment.Type), SingleVariableDesignation(Identifier(varName)))
                ),
                InvocationExpression(MemberAccess(FeatureField(containment), IdentifierName("Contains")),
                    AsArguments([IdentifierName(varName)]))
            ),
            false => Call("ReferenceEquals", FeatureField(containment), IdentifierName("child"))
        };

        return IfStatement(condition, ReturnStatement(MetaProperty(containment)));
    }

    #endregion
}