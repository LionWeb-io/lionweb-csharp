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

using Core.M2;
using Core.M3;
using Core.Notification.Partition.Emitter;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

public partial class FeatureGenerator
{
    private IEnumerable<MemberDeclarationSyntax> RequiredSingleContainment(Containment containment) =>
        new List<MemberDeclarationSyntax>
        {
            SingleFeatureField(true),
            SingleRequiredFeatureProperty(writeable: true)
                .Xdoc(XdocThrowsIfSetToNull()),
            TryGet(writeable: true)
        }.Concat(RequiredFeatureSetter([
                AsureNotNullCall(),
                SingleContainmentEmitterVariable(),
                EmitterCollectOldDataCall(),
                SetParentNullCall(containment),
                AttachChildCall(),
                AssignFeatureField(),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ], true)
            .Select(s => s.Xdoc(XdocThrowsIfSetToNull()))
        );

    private IEnumerable<MemberDeclarationSyntax> OptionalSingleContainment(Containment containment) =>
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
                    SetParentNullCall(containment),
                    AttachChildCall(),
                    AssignFeatureField(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ], true));

    private LocalDeclarationStatementSyntax SingleContainmentEmitterVariable() =>
        Variable(
            "emitter",
            AsType(typeof(ContainmentSingleNotificationEmitter<>), AsType(feature.GetFeatureType(), writeable: true)),
            NewCall([
                MetaProperty(feature), This(), IdentifierName("value"),
                FeatureField(feature), IdentifierName("notificationId")
            ])
        );

    private IEnumerable<MemberDeclarationSyntax> RequiredMultiContainment(Containment containment) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleContainmentField(containment, writeable: true),
            MultipleContainmentProperty(containment, AsNonEmptyReadOnlyCall(containment), writeable: true)
                .Xdoc(XdocRequiredMultipleLink(containment)),
            TryGetMultiple()
        }.Concat(
            LinkAdder(containment, [
                    SafeNodesVariableContainment(),
                    AssureNonEmptyCall(containment),
                    AddMultipleContainmentEmitterVariable(Null()),
                    EmitterCollectOldDataCall(),
                    RequiredAddRangeCall(containment),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(a => XdocRequiredAdder(a, containment))
        ).Concat(
            LinkInserter(containment, [
                    AssureInRangeCall(containment),
                    SafeNodesVariableContainment(),
                    AssureNonEmptyCall(containment),
                    AssureNoSelfMoveCall(containment),
                    AddMultipleContainmentEmitterVariable(IdentifierName("index")),
                    EmitterCollectOldDataCall(),
                    InsertRangeCall(containment),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(i => XdocRequiredInserter(i, containment))
        ).Concat(
            LinkRemover(containment, [
                    SafeNodesVariableContainment(),
                    AssureNotNullCall(containment),
                    AssureNotClearingCall(containment),
                    RequiredRemoveSelfParentCall(containment),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(r => XdocRequiredRemover(r, containment))
        );

    private IEnumerable<MemberDeclarationSyntax> OptionalMultiContainment(Containment containment) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleContainmentField(containment, writeable: true),
            MultipleContainmentProperty(containment, AsReadOnlyCall(containment), writeable: true),
            TryGetMultiple()
        }.Concat(
            LinkAdder(containment, [
                SafeNodesVariableContainment(),
                AssureNotNullCall(containment),
                AssureNotNullMembersCall(containment),
                AddMultipleContainmentEmitterVariable(Null()),
                EmitterCollectOldDataCall(),
                OptionalAddRangeCall(containment),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ], writeable: true)
        ).Concat(
            LinkInserter(containment, [
                AssureInRangeCall(containment),
                SafeNodesVariableContainment(),
                AssureNotNullCall(containment),
                AssureNoSelfMoveCall(containment),
                AssureNotNullMembersCall(containment),
                AddMultipleContainmentEmitterVariable(IdentifierName("index")),
                EmitterCollectOldDataCall(),
                InsertRangeCall(containment),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ], writeable: true)
        ).Concat(
            LinkRemover(containment, [
                OptionalRemoveSelfParentCall(containment),
                ReturnStatement(This())
            ], writeable: true)
        );

    private LocalDeclarationStatementSyntax AddMultipleContainmentEmitterVariable(ExpressionSyntax index) =>
        Variable(
            "emitter",
            AsType(typeof(ContainmentAddMultipleNotificationEmitter<>),
                AsType(feature.GetFeatureType(), writeable: true)),
            NewCall([
                MetaProperty(feature), This(), IdentifierName("safeNodes"),
                FeatureField(feature), index, IdentifierName("notificationId")
            ])
        );

    private ExpressionStatementSyntax AssureNoSelfMoveCall(Containment containment) =>
        ExpressionStatement(Call("AssureNoSelfMove",
            IdentifierName("index"),
            IdentifierName("safeNodes"),
            FeatureField(containment)
        ));

    private ExpressionStatementSyntax SetParentNullCall(Containment containment) =>
        ExpressionStatement(Call("SetParentNull", FeatureField(containment)));

    private ExpressionStatementSyntax RequiredRemoveSelfParentCall(Containment containment) =>
        ExpressionStatement(Call("RemoveSelfParent",
            IdentifierName("safeNodes"),
            FeatureField(containment),
            MetaProperty(containment),
            CallGeneric("ContainmentRemover", AsType(containment.Type, writeable: true),
                MetaProperty(containment))
        ));

    private ExpressionStatementSyntax OptionalRemoveSelfParentCall(Containment containment) =>
        ExpressionStatement(Call("RemoveSelfParent",
            OptionalNodesToList(),
            FeatureField(containment),
            MetaProperty(containment),
            CallGeneric("ContainmentRemover", AsType(containment.Type, writeable: true),
                MetaProperty(containment))
        ));

    private ExpressionStatementSyntax InsertRangeCall(Containment containment) =>
        ExpressionStatement(InvocationExpression(
            MemberAccess(FeatureField(containment), IdentifierName("InsertRange")),
            AsArguments([
                IdentifierName("index"),
                Call("SetSelfParent", IdentifierName("safeNodes"),
                    MetaProperty(containment)
                )
            ])
        ));

    private FieldDeclarationSyntax MultipleContainmentField(Containment containment, bool writeable = false) =>
        Field(FeatureField(containment).ToString(),
                AsType(typeof(List<>), AsType(containment.Type, writeable: writeable)),
                Collection([]))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

    private PropertyDeclarationSyntax MultipleContainmentProperty(Containment containment,
        InvocationExpressionSyntax getter, bool writeable = false) =>
        PropertyDeclaration(
                AsType(typeof(IReadOnlyList<>), AsType(containment.Type, writeable: writeable)),
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
                MetaPointerAttribute(feature),
                FeatureAttribute(),
                ObsoleteAttribute(feature)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(containment));
    
    private LocalDeclarationStatementSyntax SafeNodesVariableContainment() => 
        SafeNodesVariable(OptionalNodesToList());
}