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
using Names;
using System.Collections.Immutable;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

public class FeatureGeneratorReference(Classifier classifier, Reference reference, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) : FeatureGeneratorLinkBase(classifier, reference, names, lionWebVersion, config)
{
    public IEnumerable<MemberDeclarationSyntax> RequiredSingleReference() =>
        new List<MemberDeclarationSyntax>
            {
                SingleReferenceField(),
                SingleRequiredFeatureProperty(InvocationExpression(ReferenceTarget()))
                    .Xdoc(XdocThrowsIfSetToNull()),
                SingleReferenceTargetMethod(),
                TryGet(InvocationExpression(ReferenceTarget())),
                SingleReferenceSetter([
                        ExpressionStatement(CallGeneric("AssureNotNullInstance",
                            AsType(reference.GetFeatureType()),
                            IdentifierName("value"),
                            MetaProperty(reference)
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
        Field(FeatureField(reference).ToString(),
                NullableType(AsType(typeof(ReferenceTarget))),
                Null())
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private LocalDeclarationStatementSyntax ReferenceEmitterVariable() =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceSingleNotificationEmitter<>), AsType(reference.GetFeatureType())),
            NewCall([
                MetaProperty(reference), This(), IdentifierName("value"),
                FeatureField(reference), IdentifierName("notificationId")
            ])
        );

    public IEnumerable<MemberDeclarationSyntax> OptionalSingleReference() =>
        new List<MemberDeclarationSyntax>
            {
                SingleReferenceField(),
                SingleOptionalFeatureProperty(InvocationExpression(ReferenceTarget())),
                SingleReferenceTargetMethod(),
                TryGet(InvocationExpression(ReferenceTarget())),
                SingleReferenceSetter([
                    ExpressionStatement(CallGeneric("AssureNullableInstance",
                        AsType(reference.GetFeatureType()),
                        IdentifierName("value"),
                        MetaProperty(reference)
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
                    Param("value", NullableType(AsType(typeof(ReferenceTarget)))),
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

    public IEnumerable<MemberDeclarationSyntax> RequiredMultiReference() =>
        new List<MemberDeclarationSyntax>
        {
            MultipleReferenceField(),
            MultipleReferenceProperty(CallGeneric("AsNonEmptyReadOnly", AsType(reference.Type),
                        Call(ReferenceTargets().ToString()),
                        MetaProperty(reference)
                    ))
                .Xdoc(XdocRequiredMultipleLink()),
            MultipleReferenceTargetsMethod(),
            TryGetMultiple(InvocationExpression(ReferenceTargets()))
        }.Concat(
            LinkAdder([
                    SafeNodesVariableReference(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(),
                    AddMultipleReferenceEmitterVariable(MemberAccess(FeatureField(reference),
                        IdentifierName("Count"))),
                    EmitterCollectOldDataCall(),
                    SimpleAddRangeCall(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
                .Select(XdocRequiredAdder)
        ).Concat(
            LinkInserter([
                    AssureInRangeCall(),
                    SafeNodesVariableReference(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(),
                    AddMultipleReferenceEmitterVariable(IdentifierName("index")),
                    EmitterCollectOldDataCall(),
                    SimpleInsertRangeCall(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
                .Select(XdocRequiredInserter)
        ).Concat(
            LinkRemover([
                    SafeNodesVariable(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(),
                    ExpressionStatement(Call("AssureNotClearing",
                        IdentifierName("safeNodes"),
                        InvocationExpression(ReferenceTargets()),
                        MetaProperty(reference)
                    )),
                    SimpleRemoveAllCall(),
                    ReturnStatement(This())
                ])
                .Select(XdocRequiredRemover)
        );

    private LocalDeclarationStatementSyntax AddMultipleReferenceEmitterVariable(ExpressionSyntax index) =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceAddMultipleNotificationEmitter<>), AsType(reference.GetFeatureType())),
            NewCall([
                MetaProperty(reference), This(), IdentifierName("safeNodes"), index,
                IdentifierName("notificationId")
            ])
        );

    public IEnumerable<MemberDeclarationSyntax> OptionalMultiReference() =>
        new List<MemberDeclarationSyntax>
        {
            MultipleReferenceField(),
            MultipleReferenceProperty(Call(ReferenceTargets().ToString())),
            MultipleReferenceTargetsMethod(),
            TryGetMultiple(InvocationExpression(ReferenceTargets()))
        }.Concat(
            LinkAdder([
                SafeNodesVariableReference(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(),
                AddMultipleReferenceEmitterVariable(MemberAccess(FeatureField(reference),
                    IdentifierName("Count"))),
                EmitterCollectOldDataCall(),
                SimpleAddRangeCall(),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ])
        ).Concat(
            LinkInserter([
                AssureInRangeCall(),
                SafeNodesVariableReference(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(),
                AddMultipleReferenceEmitterVariable(IdentifierName("index")),
                EmitterCollectOldDataCall(),
                SimpleInsertRangeCall(),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ])
        ).Concat(
            LinkRemover([
                SafeNodesVariable(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(),
                SimpleRemoveAllCall(),
                ReturnStatement(This())
            ])
        );

    private ExpressionStatementSyntax AssureNotNullMembersCall() =>
        ExpressionStatement(Call("AssureNotNullMembers",
            IdentifierName("safeNodes"),
            MetaProperty(reference)
        ));

    private ExpressionStatementSyntax SimpleAddRangeCall() =>
        ExpressionStatement(InvocationExpression(
            MemberAccess(FeatureField(reference), IdentifierName("AddRange")),
            AsArguments([IdentifierName("safeNodes")])
        ));

    private ExpressionStatementSyntax SimpleRemoveAllCall() =>
        ExpressionStatement(Call(
            "RemoveAll",
            IdentifierName("safeNodes"),
            FeatureField(reference),
            CallGeneric("ReferenceRemover", AsType(reference.Type), MetaProperty(reference))
        ));

    private ExpressionStatementSyntax SimpleInsertRangeCall() =>
        ExpressionStatement(InvocationExpression(
            MemberAccess(FeatureField(reference), IdentifierName("InsertRange")),
            AsArguments(
                [IdentifierName("index"), IdentifierName("safeNodes")])
        ));

    private FieldDeclarationSyntax MultipleReferenceField() =>
        Field(FeatureField(reference).ToString(),
                AsType(typeof(List<>),
                    AsType(typeof(ReferenceTarget))),
                Collection([]))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

    private PropertyDeclarationSyntax
        MultipleReferenceProperty(InvocationExpressionSyntax getter) =>
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
                MetaPointerAttribute(reference),
                FeatureAttribute(),
                ObsoleteAttribute(reference)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(reference));

    private MethodDeclarationSyntax SingleReferenceTargetMethod() =>
        Method(ReferenceTarget().ToString(),
                NullableType(AsType(reference.Type)))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword))
            .WithExpressionBody(ArrowExpressionClause(CallGeneric("ReferenceDescriptorNullableTarget",
                AsType(reference.Type), FeatureField(reference))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    private ExpressionSyntax ReferenceTarget() =>
        IdentifierName($"{reference.Name.ToFirstUpper()}Target");

    private MethodDeclarationSyntax MultipleReferenceTargetsMethod() =>
        Method(ReferenceTargets().ToString(),
                AsType(typeof(IImmutableList<>), AsType(reference.Type)))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword))
            .WithExpressionBody(ArrowExpressionClause(CallGeneric("ReferenceDescriptorNullableTargets",
                AsType(reference.GetFeatureType()), FeatureField(reference))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    private ExpressionSyntax ReferenceTargets() =>
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