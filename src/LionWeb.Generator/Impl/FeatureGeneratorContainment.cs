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
using Core.M2;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

public class FeatureGeneratorContainment(
    Classifier classifier,
    Containment containment,
    INames names,
    LionWebVersions lionWebVersion,
    GeneratorConfig config) : FeatureGeneratorLinkBase(classifier, containment, names, lionWebVersion, config)
{
    public IEnumerable<MemberDeclarationSyntax> RequiredSingleContainment() =>
        new List<MemberDeclarationSyntax>
        {
            SingleFeatureField(true),
            SingleRequiredFeatureProperty(AsType(containment.GetFeatureType(), writeable: true))
                .Xdoc(XdocThrowsIfSetToNull()),
            TryGet(writeable: true),
            SingleContainmentSetterRaw(writeable: true)
        }.Concat(RequiredFeatureSetter([
                ExpressionStatement(CallGeneric("SetRequiredSingleContainment", AsType(containment.Type, writeable:true), IdentifierName("value"), MetaProperty(containment), FeatureField(containment), FeatureSetRaw(containment))),
                ReturnStatement(This())
            ], true)
            .Select(s => s.Xdoc(XdocThrowsIfSetToNull()))
        );

    public IEnumerable<MemberDeclarationSyntax> OptionalSingleContainment() =>
        new List<MemberDeclarationSyntax>
            {
                SingleFeatureField(true),
                SingleOptionalFeatureProperty(writeable: true),
                TryGet(writeable: true),
                SingleContainmentSetterRaw(writeable: true)
            }
            .Concat(
                OptionalFeatureSetter([
                    ExpressionStatement(CallGeneric("SetOptionalSingleContainment", AsType(containment.Type, writeable:true), IdentifierName("value"), MetaProperty(containment), FeatureField(containment), FeatureSetRaw(containment))),
                    ReturnStatement(This())
                ], true));

    private MethodDeclarationSyntax SingleContainmentSetterRaw(bool writeable = false) =>
        Method(FeatureSetRaw(containment).ToString(), AsType(typeof(bool)), [
                Param("value", NullableType(AsType(containment.GetFeatureType(), writeable: writeable)))
            ])
            .WithBody(AsStatements([
                IfStatement(Not(Call("ExchangeChildRaw", IdentifierName("value"), FeatureField(containment))),
                    ReturnStatement(False())
                ),
                AssignFeatureField(),
                ReturnStatement(True())
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    public IEnumerable<MemberDeclarationSyntax> RequiredMultiContainment() =>
        new List<MemberDeclarationSyntax>
        {
            MultiContainmentSetterRaw(),
            ContainmentAdderRaw(),
            ContainmentInserterRaw(),
            ContainmentRemoverRaw(),
            MultipleContainmentField(),
            MultipleContainmentProperty(AsNonEmptyReadOnlyCall())
                .Xdoc(XdocRequiredMultipleLink()),
            TryGetMultiple(InvocationExpression(MemberAccess(FeatureField(containment), IdentifierName("AsReadOnly")))),
        }.Concat(
            LinkAdder([
                    ExpressionStatement(CallGeneric("AddRequiredMultipleContainment", AsType(containment.Type, writeable:true), IdentifierName("nodes"), MetaProperty(containment), FeatureField(containment), LinkAddRaw(containment))),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(XdocRequiredAdder)
        ).Concat(
            LinkInserter([
                    ExpressionStatement(CallGeneric("InsertRequiredMultipleContainment", AsType(containment.Type, writeable:true), IdentifierName("index"), IdentifierName("nodes"), MetaProperty(containment), FeatureField(containment), LinkInsertRaw(containment))),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(XdocRequiredInserter)
        ).Concat(
            LinkRemover([
                    ExpressionStatement(CallGeneric("RemoveRequiredMultipleContainment", AsType(containment.Type, writeable:true), IdentifierName("nodes"), MetaProperty(containment), FeatureField(containment), LinkRemoveRaw(containment))),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(XdocRequiredRemover)
        );

    public IEnumerable<MemberDeclarationSyntax> OptionalMultiContainment() =>
        new List<MemberDeclarationSyntax>
        {
            MultiContainmentSetterRaw(),
            ContainmentAdderRaw(),
            ContainmentInserterRaw(),
            ContainmentRemoverRaw(),
            MultipleContainmentField(),
            MultipleContainmentProperty(AsReadOnlyCall()),
            TryGetMultiple(
                InvocationExpression(MemberAccess(FeatureField(containment), IdentifierName("AsReadOnly")))),
        }.Concat(
            LinkAdder([
                ExpressionStatement(CallGeneric("AddOptionalMultipleContainment", AsType(containment.Type, writeable:true), IdentifierName("nodes"), MetaProperty(containment), FeatureField(containment), LinkAddRaw(containment))),
                ReturnStatement(This())
            ], writeable: true)
        ).Concat(
            LinkInserter([
                ExpressionStatement(CallGeneric("InsertOptionalMultipleContainment", AsType(containment.Type, writeable:true), IdentifierName("index"), IdentifierName("nodes"), MetaProperty(containment), FeatureField(containment), LinkInsertRaw(containment))),
                ReturnStatement(This())
            ], writeable: true)
        ).Concat(
            LinkRemover([
                ExpressionStatement(CallGeneric("RemoveOptionalMultipleContainment", AsType(containment.Type, writeable:true), IdentifierName("nodes"), MetaProperty(containment), FeatureField(containment), LinkRemoveRaw(containment))),
                ReturnStatement(This())
            ], writeable: true)
        );

    private MethodDeclarationSyntax MultiContainmentSetterRaw() =>
        Method(FeatureSetRaw(containment).ToString(), AsType(typeof(bool)),
                [
                    Param("nodes", AsType(typeof(List<>), AsType(containment.GetFeatureType(), writeable: true)))
                ],
                Call("ExchangeChildrenRaw", IdentifierName("nodes"), FeatureField(containment))
            )
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private MemberDeclarationSyntax ContainmentAdderRaw() =>
        Method(LinkAddRaw(containment).ToString(), AsType(typeof(bool)),
                [
                    Param("value", NullableType(AsType(containment.Type, writeable: true)))
                ],
                Call("AddChildRaw", IdentifierName("value"), FeatureField(containment))
            )
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private MemberDeclarationSyntax ContainmentInserterRaw() =>
        Method(LinkInsertRaw(containment).ToString(), AsType(typeof(bool)),
                [
                    Param("index", AsType(typeof(int))),
                    Param("value", NullableType(AsType(containment.Type, writeable: true)))
                ],
                Call("InsertChildRaw", IdentifierName("index"), IdentifierName("value"), FeatureField(containment))
            )
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private MemberDeclarationSyntax ContainmentRemoverRaw() =>
        Method(LinkRemoveRaw(containment).ToString(), AsType(typeof(bool)),
                [
                    Param("value", NullableType(AsType(containment.Type, writeable: true)))
                ],
                Call("RemoveChildRaw", IdentifierName("value"), FeatureField(containment))
            )
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private FieldDeclarationSyntax MultipleContainmentField() =>
        Field(FeatureField(containment).ToString(),
                AsType(typeof(List<>), AsType(containment.Type, writeable: true)),
                Collection([]))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

    private PropertyDeclarationSyntax MultipleContainmentProperty(InvocationExpressionSyntax getter) =>
        PropertyDeclaration(
                AsType(typeof(IReadOnlyList<>), AsType(containment.Type, writeable: true)),
                Identifier(FeatureProperty(containment).ToString()))
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(getter))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(InvocationExpression(
                        LinkAdd(containment),
                        AsArguments([IdentifierName("value")])))
                    )
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ])))
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(containment),
                FeatureAttribute(),
                ObsoleteAttribute(containment)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(containment));
}