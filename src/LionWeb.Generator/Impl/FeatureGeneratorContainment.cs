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
using Core.M2;
using Core.M3;
using Core.Notification.Partition.Emitter;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

internal class FeatureGeneratorContainment(Classifier classifier, Containment containment, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) : FeatureGeneratorLinkBase(classifier, containment, names, lionWebVersion, config)
{
    public IEnumerable<MemberDeclarationSyntax> RequiredSingleContainment() =>
        new List<MemberDeclarationSyntax>
        {
            SingleFeatureField(true),
            SingleRequiredFeatureProperty(AsType(containment.GetFeatureType(), writeable: true))
                .Xdoc(XdocThrowsIfSetToNull()),
            TryGet(writeable: true)
        }.Concat(RequiredFeatureSetter([
                AsureNotNullCall(),
                SingleContainmentEmitterVariable(),
                EmitterCollectOldDataCall(),
                SetParentNullCall(),
                AttachChildCall(),
                AssignFeatureField(),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ], true)
            .Select(s => s.Xdoc(XdocThrowsIfSetToNull()))
        );

    public IEnumerable<MemberDeclarationSyntax> OptionalSingleContainment() =>
        new List<MemberDeclarationSyntax>
            {
                SingleFeatureField(true),
                SingleOptionalFeatureProperty(writeable: true),
                TryGet(writeable: true)
            }
            .Concat(
                OptionalFeatureSetter([
                    SingleContainmentEmitterVariable(),
                    EmitterCollectOldDataCall(),
                    SetParentNullCall(),
                    AttachChildCall(),
                    AssignFeatureField(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ], true));

    private LocalDeclarationStatementSyntax SingleContainmentEmitterVariable() =>
        Variable(
            "emitter",
            AsType(typeof(ContainmentSingleNotificationEmitter<>), AsType(containment.GetFeatureType(), writeable: true)),
            NewCall([
                MetaProperty(containment), This(), IdentifierName("value"),
                FeatureField(containment), IdentifierName("notificationId")
            ])
        );

    public IEnumerable<MemberDeclarationSyntax> RequiredMultiContainment() =>
        new List<MemberDeclarationSyntax>
        {
            MultipleContainmentField(),
            MultipleContainmentProperty(AsNonEmptyReadOnlyCall())
                .Xdoc(XdocRequiredMultipleLink()),
            TryGetMultiple(InvocationExpression(MemberAccess(FeatureField(containment), IdentifierName("AsReadOnly"))))
        }.Concat(
            LinkAdder([
                    SafeNodesVariable(),
                    AssureNonEmptyCall(),
                    ReturnIfAddedNodesAreEqualToExistingNodes(),
                    LoopOverSafeNodes([
                        AddMultipleContainmentEmitterVariable(Null(), true),
                        EmitterCollectOldDataCall(),
                        RequiredAddRangeCall(containment, true),
                        EmitterNotifyCall()]),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(XdocRequiredAdder)
        ).Concat(
            LinkInserter([
                    AssureInRangeCall(),
                    SafeNodesVariable(),
                    AssureNonEmptyCall(),
                    AssureNoSelfMoveCall(),
                    AddMultipleContainmentEmitterVariable(IdentifierName("index")),
                    EmitterCollectOldDataCall(),
                    InsertRangeCall(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(i => XdocRequiredInserter(i))
        ).Concat(
            LinkRemover([
                    SafeNodesVariable(),
                    AssureNotNullCall(containment),
                    AssureNotClearingCall(),
                    RequiredRemoveSelfParentCall(),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(XdocRequiredRemover)
        );

    public IEnumerable<MemberDeclarationSyntax> OptionalMultiContainment() =>
        new List<MemberDeclarationSyntax>
        {
            MultipleContainmentField(),
            MultipleContainmentProperty(AsReadOnlyCall()),
            TryGetMultiple(InvocationExpression(MemberAccess(FeatureField(containment), IdentifierName("AsReadOnly"))))
        }.Concat(
            LinkAdder([
                SafeNodesVariable(),
                AssureNotNullCall(containment),
                AssureNotNullMembersCall(containment),
                ReturnIfAddedNodesAreEqualToExistingNodes(),
                LoopOverSafeNodes([
                    AddMultipleContainmentEmitterVariable(Null(), true),
                    EmitterCollectOldDataCall(),
                    OptionalAddRangeCall(containment, true),
                    EmitterNotifyCall()]),
                ReturnStatement(This())
            ], writeable: true)
        ).Concat(
            LinkInserter([
                AssureInRangeCall(),
                SafeNodesVariable(),
                AssureNotNullCall(containment),
                AssureNoSelfMoveCall(),
                AssureNotNullMembersCall(containment),
                AddMultipleContainmentEmitterVariable(IdentifierName("index")),
                EmitterCollectOldDataCall(),
                InsertRangeCall(),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ], writeable: true)
        ).Concat(
            LinkRemover([
                OptionalRemoveSelfParentCall(),
                ReturnStatement(This())
            ], writeable: true)
        );

    private ForEachStatementSyntax LoopOverSafeNodes(IEnumerable<StatementSyntax> loopBody) => 
        ForEachStatement(
            type: IdentifierName("var"),
            identifier: Identifier("safeNode"),
            expression: IdentifierName("safeNodes"),
            statement: Block(loopBody));
    
    private IfStatementSyntax ReturnIfAddedNodesAreEqualToExistingNodes() =>
        IfStatement(InvocationExpression(
            MemberAccess(FeatureField(containment), IdentifierName("SequenceEqual")),
            AsArguments([IdentifierName("safeNodes")])
        ), ReturnStatement(This()));

    private LocalDeclarationStatementSyntax AddMultipleContainmentEmitterVariable(ExpressionSyntax index, bool isAddedNodeSingle = false) =>
        Variable(
            "emitter",
            AsType(typeof(ContainmentAddMultipleNotificationEmitter<>),
                AsType(containment.GetFeatureType(), writeable: true)),
            NewCall([
                MetaProperty(containment), This(), isAddedNodeSingle ? IdentifierName("[safeNode]"): IdentifierName("safeNodes"),
                FeatureField(containment), index, IdentifierName("notificationId")
            ])
        );

    private ExpressionStatementSyntax AssureNoSelfMoveCall() =>
        ExpressionStatement(Call("AssureNoSelfMove",
            IdentifierName("index"),
            IdentifierName("safeNodes"),
            FeatureField(containment)
        ));

    private ExpressionStatementSyntax SetParentNullCall() =>
        ExpressionStatement(Call("SetParentNull", FeatureField(containment)));

    private ExpressionStatementSyntax RequiredRemoveSelfParentCall() =>
        ExpressionStatement(Call("RemoveSelfParent",
            IdentifierName("safeNodes"),
            FeatureField(containment),
            MetaProperty(containment),
            CallGeneric("ContainmentRemover", AsType(containment.Type, writeable: true),
                MetaProperty(containment))
        ));

    private ExpressionStatementSyntax OptionalRemoveSelfParentCall() =>
        ExpressionStatement(Call("RemoveSelfParent",
            OptionalNodesToList(),
            FeatureField(containment),
            MetaProperty(containment),
            CallGeneric("ContainmentRemover", AsType(containment.Type, writeable: true),
                MetaProperty(containment))
        ));

    private ExpressionStatementSyntax InsertRangeCall() =>
        ExpressionStatement(InvocationExpression(
            MemberAccess(FeatureField(containment), IdentifierName("InsertRange")),
            AsArguments([
                IdentifierName("index"),
                Call("SetSelfParent", IdentifierName("safeNodes"),
                    MetaProperty(containment)
                )
            ])
        ));

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