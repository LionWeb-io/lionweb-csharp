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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

public class FeatureGeneratorReference(Classifier classifier, Reference reference, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) : FeatureGeneratorLinkBase(classifier, reference, names, lionWebVersion, config)
{
    public IEnumerable<MemberDeclarationSyntax> RequiredSingleReference()
    {
        TypeSyntax returnType;
        ExpressionSyntax getter;
        ExpressionSyntax setter;
        switch (config.UnresolvedReferenceHandling)
        {
            case UnresolvedReferenceHandling.Throw:
                returnType = AsType(reference.GetFeatureType());
                getter = NotNullOrThrow(ReferenceTargetNonNullTargetCall(),
                    NewCall([MetaProperty(reference)], AsType(typeof(UnsetFeatureException)))
                );
                setter = InvocationExpression(FeatureSet(), AsArguments([IdentifierName("value")]));
                break;

            case UnresolvedReferenceHandling.ReturnAsNull:
                returnType = NullableType(AsType(reference.GetFeatureType()));
                getter = CallGeneric("GetRequiredReference", AsType(reference.GetFeatureType()),
                    FeatureField(reference), MetaProperty(reference));
                setter = InvocationExpression(FeatureSet(), AsArguments([
                    NotNullOrThrow(IdentifierName("value"),
                        NewCall([MetaProperty(reference), IdentifierName("value")],
                            AsType(typeof(InvalidValueException))))
                ]));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(config.UnresolvedReferenceHandling),
                    config.UnresolvedReferenceHandling.ToString());
        }

        return new List<MemberDeclarationSyntax>
            {
                SingleReferenceField(),
                SingleRequiredFeatureProperty(returnType, getter, setter)
                    .Xdoc(XdocThrowsIfSetToNull()),
                TryGet(ReferenceTargetNullableTargetCall()),
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
    }

    private InvocationExpressionSyntax ReferenceTargetNullableTargetCall() =>
        CallGeneric("ReferenceTargetNullableTarget",
            AsType(reference.Type), FeatureField(reference), MetaProperty(reference));

    private InvocationExpressionSyntax ReferenceTargetNonNullTargetCall() =>
        CallGeneric("ReferenceTargetNonNullTarget",
            AsType(reference.Type), FeatureField(reference), MetaProperty(reference));

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

    public IEnumerable<MemberDeclarationSyntax> OptionalSingleReference()
    {
        ExpressionSyntax getter = config.UnresolvedReferenceHandling switch
        {
            UnresolvedReferenceHandling.ReturnAsNull => ReferenceTargetNullableTargetCall(),
            UnresolvedReferenceHandling.Throw => ReferenceTargetNonNullTargetCall(),
            _ => throw new ArgumentOutOfRangeException(config.UnresolvedReferenceHandling.ToString())
        };
        return new List<MemberDeclarationSyntax>
            {
                SingleReferenceField(),
                SingleOptionalFeatureProperty(getter),
                TryGet(ReferenceTargetNullableTargetCall()),
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
    }

    private MethodDeclarationSyntax SingleReferenceSetter(List<StatementSyntax> body) =>
        Method(FeatureSet().ToString(), AsType(classifier),
                [
                    Param("value", NullableType(AsType(typeof(ReferenceTarget)))),
                    ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword))
            .WithBody(AsStatements(body));

    private static InvocationExpressionSyntax FromNodeOptional(ExpressionSyntax value) =>
        InvocationExpression(
                MemberAccess(
                    IdentifierName("ReferenceTarget"),
                    IdentifierName("FromNodeOptional")
                )
            )
            .WithArgumentList(AsArguments([
                    value
                ])
            );

    public IEnumerable<MemberDeclarationSyntax> RequiredMultiReference()
    {
        string getterMethod;
        MethodDeclarationSyntax tryGet;
        switch (config.UnresolvedReferenceHandling)
        {
            case UnresolvedReferenceHandling.ReturnAsNull:
                getterMethod = "GetRequiredNullableReferences";
                tryGet = TryGetMultiple(
                    ReferenceTargetNullableTargetsCall(),
                    NullableType(AsType(reference.GetFeatureType()))
                );
                break;

            case UnresolvedReferenceHandling.Throw:
                getterMethod = "GetRequiredNonNullReferences";
                tryGet = TryGetMultipleReferences();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(config.UnresolvedReferenceHandling),
                    config.UnresolvedReferenceHandling.ToString());
        }

        return new List<MemberDeclarationSyntax>
        {
            MultipleReferenceField(),
            MultipleReferenceProperty(CallGeneric(getterMethod, AsType(reference.GetFeatureType()),
                    FeatureField(reference), MetaProperty(reference)))
                .Xdoc(XdocRequiredMultipleLink()),
            tryGet
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
                        ReferenceTargetNullableTargetsCall(),
                        MetaProperty(reference)
                    )),
                    SimpleRemoveAllCall(),
                    ReturnStatement(This())
                ])
                .Select(XdocRequiredRemover)
        );
    }

    private LocalDeclarationStatementSyntax AddMultipleReferenceEmitterVariable(ExpressionSyntax index) =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceAddMultipleNotificationEmitter<>), AsType(reference.GetFeatureType())),
            NewCall([
                MetaProperty(reference), This(), IdentifierName("safeNodes"), index,
                IdentifierName("notificationId")
            ])
        );

    public IEnumerable<MemberDeclarationSyntax> OptionalMultiReference()
    {
        InvocationExpressionSyntax getter;
        MethodDeclarationSyntax tryGet;
        switch (config.UnresolvedReferenceHandling)
        {
            case UnresolvedReferenceHandling.ReturnAsNull:
                getter = ReferenceTargetNullableTargetsCall();
                tryGet = TryGetMultiple(
                    ReferenceTargetNullableTargetsCall(),
                    NullableType(AsType(reference.GetFeatureType()))
                );
                break;

            case UnresolvedReferenceHandling.Throw:
                getter = ReferenceTargetNonNullTargetsCall();
                tryGet = TryGetMultipleReferences();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(config.UnresolvedReferenceHandling),
                    config.UnresolvedReferenceHandling.ToString());
        }

        return new List<MemberDeclarationSyntax> { MultipleReferenceField(), MultipleReferenceProperty(getter), tryGet }
            .Concat(
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
    }

    private MethodDeclarationSyntax TryGetMultipleReferences() =>
        AbstractTryGetMultiple(AsType(reference.GetFeatureType()))
            .WithExpressionBody(ArrowExpressionClause(
                InvocationExpression(
                        GenericName(Identifier("TryGetReference"))
                            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(
                                AsType(reference.GetFeatureType())
                            )))
                    )
                    .WithArgumentList(ArgumentList(SeparatedList([
                        Argument(FeatureField(reference)),
                        Argument(IdentifierName(FeatureTryGetParam())).WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword))
                    ])))
            ))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

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

    private PropertyDeclarationSyntax MultipleReferenceProperty(InvocationExpressionSyntax getter)
    {
        TypeSyntax returnType;
        InvocationExpressionSyntax initer;

        switch (config.UnresolvedReferenceHandling)
        {
            case UnresolvedReferenceHandling.ReturnAsNull:
                returnType = NullableType(AsType(reference.Type));
                initer = InvocationExpression(
                    LinkAdd(reference),
                    AsArguments([PostfixUnaryExpression(
                        SyntaxKind.SuppressNullableWarningExpression,
                        IdentifierName("value")
                    )])
                );
                break;

            case UnresolvedReferenceHandling.Throw:
                returnType = AsType(reference.Type);
                initer = InvocationExpression(
                    LinkAdd(reference),
                    AsArguments([IdentifierName("value")])
                );
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(config.UnresolvedReferenceHandling), config.UnresolvedReferenceHandling.ToString());
        }

        return PropertyDeclaration(AsType(typeof(IReadOnlyList<>), returnType),
                Identifier(FeatureProperty(reference).ToString()))
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(getter))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(initer)
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
    }

    private InvocationExpressionSyntax ReferenceTargetNonNullTargetsCall() =>
        CallGeneric("ReferenceTargetNonNullTargets",
            AsType(reference.GetFeatureType()), FeatureField(reference), MetaProperty(reference));

    private InvocationExpressionSyntax ReferenceTargetNullableTargetsCall() =>
        CallGeneric("ReferenceTargetNullableTargets",
            AsType(reference.GetFeatureType()), FeatureField(reference), MetaProperty(reference));

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
                                        IdentifierName("ReferenceTarget"),
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