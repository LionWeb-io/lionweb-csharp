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

namespace LionWeb.Generator.Impl;

using Core;
using Core.M3;
using Core.Notification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

public partial class FeatureGenerator
{
    private IEnumerable<MemberDeclarationSyntax> AbstractLinkMembers(Link link, bool writeable) =>
    [
        AbstractMultipleLinkProperty(link),
        AbstractLinkAdder(link, writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
        AbstractLinkInserter(link, writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
        AbstractLinkRemover(link, writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
    ];

    private IEnumerable<XmlNodeSyntax> XdocRequiredMultipleLink(Link link) =>
        XdocThrows($"If {FeatureProperty(link)} is empty", AsType(typeof(UnsetFeatureException)));

    private MethodDeclarationSyntax XdocRequiredRemover(MethodDeclarationSyntax r, Link link) =>
        r.Xdoc(XdocThrows($"If {FeatureProperty(link)} would be empty", AsType(typeof(InvalidValueException))));

    private MethodDeclarationSyntax XdocRequiredInserter(MethodDeclarationSyntax i, Link link) =>
        i.Xdoc(
            XdocThrowsFeatureNodesEmpty(link)
                .Concat(
                    XdocThrows($"If index negative or greater than {FeatureProperty(link)}.Count",
                        AsType(typeof(ArgumentOutOfRangeException))
                    )
                )
        );

    private MethodDeclarationSyntax XdocRequiredAdder(MethodDeclarationSyntax a, Link link) =>
        a.Xdoc(XdocThrowsFeatureNodesEmpty(link));

    private IEnumerable<XmlNodeSyntax> XdocThrowsFeatureNodesEmpty(Link link) =>
        XdocThrows($"If both {FeatureProperty(link)} and nodes are empty", AsType(typeof(InvalidValueException)));

    private ExpressionStatementSyntax AssureNotClearingCall(Link link) =>
        ExpressionStatement(Call("AssureNotClearing",
            IdentifierName("safeNodes"),
            FeatureField(link),
            MetaProperty(link)
        ));

    private ExpressionStatementSyntax AssureNonEmptyCall(Link link) =>
        ExpressionStatement(Call("AssureNonEmpty",
            IdentifierName("safeNodes"),
            FeatureField(link),
            MetaProperty(link)
        ));

    private ExpressionStatementSyntax AssureInRangeCall(Link link) =>
        ExpressionStatement(Call("AssureInRange",
            IdentifierName("index"),
            FeatureField(link)
        ));

    private InvocationExpressionSyntax AsReadOnlyCall(Link link) =>
        InvocationExpression(MemberAccess(FeatureField(link),
            IdentifierName("AsReadOnly")));

    private InvocationExpressionSyntax AsNonEmptyReadOnlyCall(Link link) =>
        Call("AsNonEmptyReadOnly", FeatureField(link), MetaProperty(link));

    private PropertyDeclarationSyntax AbstractMultipleLinkProperty(Link link) =>
        PropertyDeclaration(AsType(typeof(IReadOnlyList<>), AsType(link.Type)),
                Identifier(FeatureProperty(link).ToString())
            )
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ])))
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(feature),
                FeatureAttribute(),
                ObsoleteAttribute(feature)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(link));

    private IEnumerable<MethodDeclarationSyntax> LinkRemover(Link link, List<StatementSyntax> body,
        bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractLinkRemover(link, writeable: writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractLinkRemover(link, writeable: writeable),
                    Call(LinkRemove(link).ToString(), IdentifierName("nodes"))
                )
            );

        return result;
    }

    private MethodDeclarationSyntax AbstractLinkRemover(Link link, bool writeable = false) =>
        Method(LinkRemove(link).ToString(), AsType(classifier),
                [
                    Param("nodes",
                        AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable))),
                    ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(link));

    private IEnumerable<MethodDeclarationSyntax> LinkInserter(Link link, List<StatementSyntax> body,
        bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractLinkInserter(link, writeable: writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractLinkInserter(link, writeable: writeable),
                    Call(LinkInsert(link).ToString(), IdentifierName("index"),
                        IdentifierName("nodes"))
                )
            );

        return result;
    }

    private MethodDeclarationSyntax AbstractLinkInserter(Link link, bool writeable = false) =>
        Method(LinkInsert(link).ToString(), AsType(classifier), [
                Param("index", AsType(typeof(int))),
                Param("nodes", AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable))),
                ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(link));

    private IEnumerable<MethodDeclarationSyntax> LinkAdder(Link link, List<StatementSyntax> body,
        bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractLinkAdder(link, writeable: writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractLinkAdder(link, writeable: writeable),
                    Call(LinkAdd(link).ToString(), IdentifierName("nodes")))
            );

        return result;
    }

    private MethodDeclarationSyntax AbstractLinkAdder(Link link, bool writeable = false) =>
        Method(LinkAdd(link).ToString(), AsType(classifier),
                [
                    Param("nodes",
                        AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable))),
                    ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(link));

    private ExpressionSyntax LinkInsert(Link link) =>
        IdentifierName($"Insert{link.Name.ToFirstUpper()}");

    private ExpressionSyntax LinkRemove(Link link) =>
        IdentifierName($"Remove{link.Name.ToFirstUpper()}");
}