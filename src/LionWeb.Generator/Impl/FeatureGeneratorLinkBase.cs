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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace LionWeb.Generator.Impl;

using Core;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

public abstract class FeatureGeneratorLinkBase(Classifier classifier, Link link, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) : FeatureGeneratorBase(classifier, link, names, lionWebVersion, config)
{
    public IEnumerable<MemberDeclarationSyntax> AbstractLinkMembers(bool writeable) =>
    [
        AbstractMultipleLinkProperty(),
        AbstractLinkAdder(writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
        AbstractLinkInserter(writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
        AbstractLinkRemover(writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
    ];

    protected IEnumerable<XmlNodeSyntax> XdocRequiredMultipleLink() =>
        XdocThrows($"If {FeatureProperty(link)} is empty", AsType(typeof(UnsetFeatureException)));

    protected MethodDeclarationSyntax XdocRequiredRemover(MethodDeclarationSyntax r) =>
        r.Xdoc(XdocThrows($"If {FeatureProperty(link)} would be empty", AsType(typeof(InvalidValueException))));

    protected MethodDeclarationSyntax XdocRequiredInserter(MethodDeclarationSyntax i) =>
        i.Xdoc(
            XdocThrowsFeatureNodesEmpty()
                .Concat(
                    XdocThrows($"If index negative or greater than {FeatureProperty(link)}.Count",
                        AsType(typeof(ArgumentOutOfRangeException))
                    )
                )
        );

    protected MethodDeclarationSyntax XdocRequiredAdder(MethodDeclarationSyntax a) =>
        a.Xdoc(XdocThrowsFeatureNodesEmpty());

    private IEnumerable<XmlNodeSyntax> XdocThrowsFeatureNodesEmpty() =>
        XdocThrows($"If both {FeatureProperty(link)} and nodes are empty", AsType(typeof(InvalidValueException)));

    protected InvocationExpressionSyntax AsReadOnlyCall() =>
        InvocationExpression(MemberAccess(FeatureField(link),
            IdentifierName("AsReadOnly")));

    protected InvocationExpressionSyntax AsNonEmptyReadOnlyCall() =>
        Call("AsNonEmptyReadOnly", FeatureField(link), MetaProperty(link));

    private PropertyDeclarationSyntax AbstractMultipleLinkProperty() =>
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
                MetaPointerAttribute(link),
                FeatureAttribute(),
                ObsoleteAttribute(link)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(link));

    protected IEnumerable<MethodDeclarationSyntax> LinkRemover(List<StatementSyntax> body, bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractLinkRemover(writeable: writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractLinkRemover(writeable: writeable),
                    Call(LinkRemove().ToString(), IdentifierName("nodes"))
                )
            );

        return result;
    }

    private MethodDeclarationSyntax AbstractLinkRemover(bool writeable = false) =>
        Method(LinkRemove().ToString(), AsType(classifier),
                [
                    Param("nodes",
                        AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(link)]))
            .Xdoc(XdocDefault(link));

    protected IEnumerable<MethodDeclarationSyntax> LinkInserter(List<StatementSyntax> body, bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractLinkInserter(writeable: writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractLinkInserter(writeable: writeable),
                    Call(LinkInsert().ToString(), IdentifierName("index"),
                        IdentifierName("nodes"))
                )
            );

        return result;
    }

    private MethodDeclarationSyntax AbstractLinkInserter(bool writeable = false) =>
        Method(LinkInsert().ToString(), AsType(classifier), [
                Param("index", AsType(typeof(int))),
                Param("nodes", AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(link)]))
            .Xdoc(XdocDefault(link));

    protected IEnumerable<MethodDeclarationSyntax> LinkAdder(List<StatementSyntax> body, bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractLinkAdder(writeable: writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractLinkAdder(writeable: writeable),
                    Call(LinkAdd(link).ToString(), IdentifierName("nodes")))
            );

        return result;
    }

    private MethodDeclarationSyntax AbstractLinkAdder(bool writeable = false) =>
        Method(LinkAdd(link).ToString(), AsType(classifier),
                [
                    Param("nodes",
                        AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(link)]))
            .Xdoc(XdocDefault(link));

    private ExpressionSyntax LinkInsert() =>
        LinkInsert(link);

    private ExpressionSyntax LinkRemove() =>
        LinkRemove(link);
}