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
using Core.Notification;
using Core.Notification.Partition.Emitter;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

public partial class FeatureGenerator
{
    private IEnumerable<MemberDeclarationSyntax> RequiredSingleReference(Reference reference) =>
        new List<MemberDeclarationSyntax>
            {
                SingleReferenceField(),
                SingleRequiredFeatureProperty(InvocationExpression(ReferenceTarget(reference)))
                    .Xdoc(XdocThrowsIfSetToNull()),
                SingleReferenceTargetMethod(reference),
                TryGet(InvocationExpression(ReferenceTarget(reference))),
                SingleReferenceSetter([
                        ExpressionStatement(CallGeneric("AssureNotNullInstance",
                            AsType(reference.GetFeatureType()),
                            IdentifierName("value"),
                            MetaProperty(feature)
                        )),
                        ReferenceEmitterVariable(),
                        EmitterCollectOldDataCall(),
                        AssignFeatureField(),
                        EmitterNotifyCall(),
                        ReturnStatement(This())
                    ])
            }
            .Concat(RequiredFeatureSetter([
                    SingleReferenceSetterForwarder()
                ])
                .Select(s => s.Xdoc(XdocThrowsIfSetToNull()))
            );

    private ReturnStatementSyntax SingleReferenceSetterForwarder() =>
        ReturnStatement(Call(FeatureSet().ToString(), FromNodeOptional(IdentifierName("value")),
            IdentifierName("notificationId")));

    private FieldDeclarationSyntax SingleReferenceField() =>
        Field(FeatureField(feature).ToString(),
                NullableType(AsType(typeof(ReferenceDescriptor))),
                Null())
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private LocalDeclarationStatementSyntax ReferenceEmitterVariable() =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceSingleNotificationEmitter<>), AsType(feature.GetFeatureType())),
            NewCall([
                MetaProperty(feature), This(), IdentifierName("value"),
                FeatureField(feature), IdentifierName("notificationId")
            ])
        );

    private IEnumerable<MemberDeclarationSyntax> OptionalSingleReference(Reference reference) =>
        new List<MemberDeclarationSyntax>
            {
                SingleReferenceField(),
                SingleOptionalFeatureProperty(InvocationExpression(ReferenceTarget(reference))),
                SingleReferenceTargetMethod(reference),
                TryGet(InvocationExpression(ReferenceTarget(reference))),
                SingleReferenceSetter([
                    ExpressionStatement(CallGeneric("AssureNullableInstance",
                        AsType(reference.GetFeatureType()),
                        IdentifierName("value"),
                        MetaProperty(feature)
                    )),
                    ReferenceEmitterVariable(),
                    EmitterCollectOldDataCall(),
                    AssignFeatureField(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
            }
            .Concat(OptionalFeatureSetter([
                SingleReferenceSetterForwarder()
            ]));

    private MethodDeclarationSyntax SingleReferenceSetter(List<StatementSyntax> body) =>
        Method(FeatureSet().ToString(), AsType(classifier),
                [
                    Param("value", NullableType(AsType(typeof(ReferenceDescriptor)))),
                    ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword))
            .WithBody(AsStatements(body));

    private static InvocationExpressionSyntax FromNodeOptionalValue() =>
        FromNodeOptional(IdentifierName("value"));

    private static InvocationExpressionSyntax FromNodeOptional(ExpressionSyntax value) =>
        InvocationExpression(
                MemberAccess(
                    IdentifierName("ReferenceDescriptorExtensions"),
                    IdentifierName("FromNodeOptional")
                )
            )
            .WithArgumentList(AsArguments([
                    value
                ])
            );

    private IEnumerable<MemberDeclarationSyntax> RequiredMultiReference(Reference reference) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleReferenceField(reference),
            MultipleReferenceProperty(reference,
                    CallGeneric("AsNonEmptyReadOnly", AsType(reference.Type),
                        Call(ReferenceTargets(reference).ToString()),
                        MetaProperty(reference)
                    ))
                .Xdoc(XdocRequiredMultipleLink(reference)),
            MultipleReferenceTargetsMethod(reference),
            TryGetMultiple(InvocationExpression(ReferenceTargets(reference)))
        }.Concat(
            LinkAdder(reference, [
                    SafeNodesVariableReference(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(reference),
                    AddMultipleReferenceEmitterVariable(MemberAccess(FeatureField(reference),
                        IdentifierName("Count"))),
                    EmitterCollectOldDataCall(),
                    SimpleAddRangeCall(reference),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
                .Select(a => XdocRequiredAdder(a, reference))
        ).Concat(
            LinkInserter(reference, [
                    AssureInRangeCall(reference),
                    SafeNodesVariableReference(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(reference),
                    AddMultipleReferenceEmitterVariable(IdentifierName("index")),
                    EmitterCollectOldDataCall(),
                    SimpleInsertRangeCall(reference),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
                .Select(i => XdocRequiredInserter(i, reference))
        ).Concat(
            LinkRemover(reference, [
                    SafeNodesVariableContainment(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(reference),
                    ExpressionStatement(Call("AssureNotClearing",
                        IdentifierName("safeNodes"),
                        InvocationExpression(ReferenceTargets(reference)),
                        MetaProperty(reference)
                    )),
                    SimpleRemoveAllCall(reference),
                    ReturnStatement(This())
                ])
                .Select(r => XdocRequiredRemover(r, reference))
        );

    private LocalDeclarationStatementSyntax AddMultipleReferenceEmitterVariable(ExpressionSyntax index) =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceAddMultipleNotificationEmitter<>), AsType(feature.GetFeatureType())),
            NewCall([
                MetaProperty(feature), This(), IdentifierName("safeNodes"), index,
                IdentifierName("notificationId")
            ])
        );

    private IEnumerable<MemberDeclarationSyntax> OptionalMultiReference(Reference reference) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleReferenceField(reference),
            MultipleReferenceProperty(reference, Call(ReferenceTargets(reference).ToString())),
            MultipleReferenceTargetsMethod(reference),
            TryGetMultiple(InvocationExpression(ReferenceTargets(reference)))
        }.Concat(
            LinkAdder(reference, [
                SafeNodesVariableReference(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(reference),
                AddMultipleReferenceEmitterVariable(MemberAccess(FeatureField(reference),
                    IdentifierName("Count"))),
                EmitterCollectOldDataCall(),
                SimpleAddRangeCall(reference),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ])
        ).Concat(
            LinkInserter(reference, [
                AssureInRangeCall(reference),
                SafeNodesVariableReference(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(reference),
                AddMultipleReferenceEmitterVariable(IdentifierName("index")),
                EmitterCollectOldDataCall(),
                SimpleInsertRangeCall(reference),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ])
        ).Concat(
            LinkRemover(reference, [
                SafeNodesVariableContainment(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(reference),
                SimpleRemoveAllCall(reference),
                ReturnStatement(This())
            ])
        );

    private ExpressionStatementSyntax AssureNotNullMembersCall(Reference reference) =>
        ExpressionStatement(Call("AssureNotNullMembers",
            IdentifierName("safeNodes"),
            MetaProperty(reference)
        ));

    private ExpressionStatementSyntax SimpleAddRangeCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(
            MemberAccess(FeatureField(reference), IdentifierName("AddRange")),
            AsArguments([IdentifierName("safeNodes")])
        ));

    private ExpressionStatementSyntax SimpleRemoveAllCall(Reference reference) =>
        ExpressionStatement(Call(
            "RemoveAll",
            IdentifierName("safeNodes"),
            FeatureField(reference),
            CallGeneric("ReferenceRemover", AsType(reference.Type), MetaProperty(reference))
        ));

    private ExpressionStatementSyntax SimpleInsertRangeCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(
            MemberAccess(FeatureField(reference), IdentifierName("InsertRange")),
            AsArguments(
                [IdentifierName("index"), IdentifierName("safeNodes")])
        ));

    private FieldDeclarationSyntax MultipleReferenceField(Reference reference) =>
        Field(FeatureField(reference).ToString(),
                AsType(typeof(List<>),
                    AsType(typeof(ReferenceDescriptor))),
                Collection([]))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

    private PropertyDeclarationSyntax
        MultipleReferenceProperty(Reference reference, InvocationExpressionSyntax getter) =>
        PropertyDeclaration(AsType(typeof(IReadOnlyList<>), AsType(reference.Type)),
                Identifier(FeatureProperty(reference).ToString()))
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(getter))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(InvocationExpression(
                        LinkAdd(reference),
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
            .Xdoc(XdocDefault(reference));

    private MethodDeclarationSyntax SingleReferenceTargetMethod(Reference reference) =>
        Method(ReferenceTarget(reference).ToString(),
                NullableType(AsType(reference.Type)))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword))
            .WithExpressionBody(ArrowExpressionClause(CallGeneric("ReferenceDescriptorNullableTarget",
                AsType(reference.Type), FeatureField(reference))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    private ExpressionSyntax ReferenceTarget(Reference reference) =>
        IdentifierName($"{reference.Name.ToFirstUpper()}Target");

    private MethodDeclarationSyntax MultipleReferenceTargetsMethod(Reference reference) =>
        Method(ReferenceTargets(reference).ToString(),
                AsType(typeof(IImmutableList<>), AsType(reference.Type)))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword))
            .WithExpressionBody(ArrowExpressionClause(CallGeneric("ReferenceDescriptorNullableTargets",
                AsType(reference.GetFeatureType()), FeatureField(reference))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    private ExpressionSyntax ReferenceTargets(Reference reference) =>
        IdentifierName($"{reference.Name.ToFirstUpper()}Targets");

    private LocalDeclarationStatementSyntax SafeNodesVariableReference() =>
        SafeNodesVariable(
            ConditionalAccessExpression(
                IdentifierName("nodes"),
                InvocationExpression(
                    MemberAccess(
                        InvocationExpression(
                                MemberBindingExpression(
                                    IdentifierName("Select")
                                )
                            )
                            .WithArgumentList(
                                AsArguments([
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("ReferenceDescriptorExtensions"),
                                        IdentifierName("FromNode")
                                    )
                                ])
                            ),
                        IdentifierName("ToList")
                    )
                )
            )
        );
}