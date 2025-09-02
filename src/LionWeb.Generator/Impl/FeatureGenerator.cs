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

using Core;
using Core.M2;
using Core.M3;
using Core.Notification;
using Core.Notification.Partition.Emitter;
using Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;
using Property = Core.M3.Property;

/// <summary>
/// Generates concept/annotation members for one feature.
/// Covers members:
/// - feature field
/// - feature property
/// - SetFeature()
/// - TryGetFeature()
/// - AddFeature()
/// - InsertFeature()
/// - RemoveFeature()
/// </summary>
public class FeatureGenerator(Classifier classifier, Feature feature, INames names, LionWebVersions lionWebVersion, GeneratorConfig config)
    : ClassifierGeneratorBase(names, lionWebVersion, config)
{
    /// <inheritdoc cref="FeatureGenerator"/>
    public IEnumerable<MemberDeclarationSyntax> Members() =>
        feature switch
        {
            Property { Optional: false } p => RequiredProperty(p),
            Property { Optional: true } p => OptionalProperty(p),
            Containment { Optional: false, Multiple: false } c => RequiredSingleContainment(c),
            Containment { Optional: true, Multiple: false } c => OptionalSingleContainment(c),
            Reference { Optional: false, Multiple: false } r => RequiredSingleReference(r),
            Reference { Optional: true, Multiple: false } r => OptionalSingleReference(r),
            Containment { Optional: false, Multiple: true } c => RequiredMultiContainment(c),
            Containment { Optional: true, Multiple: true } c => OptionalMultiContainment(c),
            Reference { Optional: false, Multiple: true } r => RequiredMultiReference(r),
            Reference { Optional: true, Multiple: true } r => OptionalMultiReference(r),
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

    /// <inheritdoc cref="FeatureGenerator"/>
    public IEnumerable<MemberDeclarationSyntax> AbstractMembers() =>
        feature switch
        {
            Containment { Multiple: true } c =>
                AbstractLinkMembers(c, writeable: true),

            Reference { Multiple: true } r =>
                AbstractLinkMembers(r),

            { Optional: false } =>
            [
                AbstractSingleRequiredFeatureProperty(feature is Containment),
                AbstractRequiredFeatureSetter(feature is Containment)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ],
            { Optional: true } =>
            [
                AbstractSingleOptionalFeatureProperty(),
                AbstractOptionalFeatureSetter(feature is Containment)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ],
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

    private IEnumerable<MemberDeclarationSyntax> AbstractLinkMembers(Link link, bool writeable = false) =>
    [
        AbstractMultipleLinkProperty(link),
        AbstractLinkAdder(link, writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
        AbstractLinkInserter(link, writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
        AbstractLinkRemover(link, writeable: writeable)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
    ];

    private IEnumerable<MemberDeclarationSyntax> RequiredProperty(Property property)
    {
        List<StatementSyntax> setterBody =
        [
            PropertyEmitterVariable(),
            EmitterCollectOldDataCall(),
            AssignFeatureField(),
            EmitterNotifyCall(),
            ReturnStatement(This())
        ];
        if (IsReferenceType(property))
            setterBody.Insert(0, AsureNotNullCall());

        var prop = SingleRequiredFeatureProperty();
        var setter = RequiredFeatureSetter(setterBody);

        var members = new List<MemberDeclarationSyntax> { prop, TryGet() }.Concat(setter);
        if (IsReferenceType(property))
            members = members.Select(member => member.Xdoc(XdocThrowsIfSetToNull()));

        return new List<MemberDeclarationSyntax> { SingleFeatureField() }.Concat(members);
    }

    private LocalDeclarationStatementSyntax PropertyEmitterVariable() =>
        Variable(
            "emitter",
            AsType(typeof(PropertyNotificationEmitter)),
            NewCall([MetaProperty(feature), This(), IdentifierName("value"), FeatureField(feature), IdentifierName("notificationId")])
        );

    private MethodDeclarationSyntax TryGet(bool writeable = false) =>
        Method(FeatureTryGet(feature), AsType(typeof(bool)),
                [
                    OutParam(Param(FeatureTryGetParam(),
                            NullableType(AsType(feature.GetFeatureType(), writeable: writeable))))
                        .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(feature))
            .WithBody(AsStatements([
                Assignment(FeatureTryGetParam(), FeatureField(feature)),
                ReturnStatement(NotEquals(FeatureField(feature), Null()))
            ]));


    private MethodDeclarationSyntax TryGetMultiple() =>
        Method(FeatureTryGet(feature), AsType(typeof(bool)),
                [
                    OutParam(Param(
                            FeatureTryGetParam(),
                            AsType(typeof(IReadOnlyList<>), AsType(feature.GetFeatureType()))
                        ))
                        .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(feature))
            .WithBody(AsStatements([
                Assignment(FeatureTryGetParam(), FeatureField(feature)),
                ReturnStatement(
                    NotEquals(
                        MemberAccess(FeatureField(feature), IdentifierName("Count")),
                        0.AsLiteral()
                    )
                )
            ]));

    private ParameterSyntax OutParam(ParameterSyntax parameterSyntax) =>
        parameterSyntax
            .WithAttributeLists(AsAttributes([
                Attribute(IdentifierName(AsType(typeof(NotNullWhenAttribute)).ToString()))
                    .WithArgumentList(
                        AttributeArgumentList(SingletonSeparatedList(AttributeArgument(True())))
                    )
            ]));


    private IEnumerable<MemberDeclarationSyntax> OptionalProperty(Property property) =>
        new List<MemberDeclarationSyntax> { SingleFeatureField(), SingleOptionalFeatureProperty(), TryGet() }
            .Concat(
                OptionalFeatureSetter([
                    PropertyEmitterVariable(),
                    EmitterCollectOldDataCall(),
                    AssignFeatureField(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
            );

    private IEnumerable<MemberDeclarationSyntax> RequiredSingleContainment(Containment containment) =>
        new List<MemberDeclarationSyntax>
        {
            SingleFeatureField(true),
            SingleRequiredFeatureProperty(true)
                .Xdoc(XdocThrowsIfSetToNull()),
            TryGet(true)
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

    private IEnumerable<XmlNodeSyntax> XdocThrowsIfSetToNull() =>
        XdocThrows("If set to null", AsType(typeof(InvalidValueException)));

    private IEnumerable<MemberDeclarationSyntax> OptionalSingleContainment(Containment containment) =>
        new List<MemberDeclarationSyntax>
            {
                SingleFeatureField(true), SingleOptionalFeatureProperty(true), TryGet(true)
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
            NewCall([MetaProperty(feature), This(), IdentifierName("value"), FeatureField(feature), IdentifierName("notificationId")])
        );

    private IEnumerable<MemberDeclarationSyntax> RequiredSingleReference(Reference reference) =>
        new List<MemberDeclarationSyntax>
            {
                SingleFeatureField(),
                SingleRequiredFeatureProperty()
                    .Xdoc(XdocThrowsIfSetToNull()),
                TryGet()
            }
            .Concat(RequiredFeatureSetter([
                    AsureNotNullCall(),
                    ReferenceEmitterVariable(),
                    EmitterCollectOldDataCall(),
                    AssignFeatureField(),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
                .Select(s => s.Xdoc(XdocThrowsIfSetToNull()))
            );

    private LocalDeclarationStatementSyntax ReferenceEmitterVariable() =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceSingleNotificationEmitter)),
            NewCall([MetaProperty(feature), This(), IdentifierName("value"), FeatureField(feature), IdentifierName("notificationId")])
        );

    private IEnumerable<MemberDeclarationSyntax> OptionalSingleReference(Reference reference) =>
        new List<MemberDeclarationSyntax> { SingleFeatureField(), SingleOptionalFeatureProperty(), TryGet() }
            .Concat(OptionalFeatureSetter([
                ReferenceEmitterVariable(),
                EmitterCollectOldDataCall(),
                AssignFeatureField(),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ]));

    private IEnumerable<MemberDeclarationSyntax> RequiredMultiContainment(Containment containment) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleLinkField(containment, writeable: true),
            MultipleLinkProperty(containment, AsNonEmptyReadOnlyCall(containment), writeable: true)
                .Xdoc(XdocRequiredMultipleLink(containment)),
            TryGetMultiple()
        }.Concat(
            LinkAdder(containment, [
                    SafeNodesVariable(),
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
                    SafeNodesVariable(),
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
                    SafeNodesVariable(),
                    AssureNotNullCall(containment),
                    AssureNotClearingCall(containment),
                    RequiredRemoveSelfParentCall(containment),
                    ReturnStatement(This())
                ], writeable: true)
                .Select(r => XdocRequiredRemover(r, containment))
        );

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

    private IEnumerable<MemberDeclarationSyntax> OptionalMultiContainment(Containment containment) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleLinkField(containment, writeable: true),
            MultipleLinkProperty(containment, AsReadOnlyCall(containment), writeable: true),
            TryGetMultiple()
        }.Concat(
            LinkAdder(containment, [
                SafeNodesVariable(),
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
                SafeNodesVariable(),
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
            AsType(typeof(ContainmentAddMultipleNotificationEmitter<>), AsType(feature.GetFeatureType(), writeable: true)),
            NewCall([
                MetaProperty(feature), This(), IdentifierName("safeNodes"), FeatureField(feature), index, IdentifierName("notificationId")
            ])
        );

    private IEnumerable<MemberDeclarationSyntax> RequiredMultiReference(Reference reference) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleLinkField(reference),
            MultipleLinkProperty(reference, AsNonEmptyReadOnlyCall(reference))
                .Xdoc(XdocRequiredMultipleLink(reference)),
            TryGetMultiple()
        }.Concat(
            LinkAdder(reference, [
                    SafeNodesVariable(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(reference),
                    AddMultipleReferenceEmitterVariable(MemberAccess(FeatureField(reference), IdentifierName("Count"))),
                    EmitterCollectOldDataCall(),
                    SimpleAddRangeCall(reference),
                    EmitterNotifyCall(),
                    ReturnStatement(This())
                ])
                .Select(a => XdocRequiredAdder(a, reference))
        ).Concat(
            LinkInserter(reference, [
                    AssureInRangeCall(reference),
                    SafeNodesVariable(),
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
                    SafeNodesVariable(),
                    AssureNotNullCall(reference),
                    AssureNonEmptyCall(reference),
                    AssureNotClearingCall(reference),
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
                MetaProperty(feature), This(), IdentifierName("safeNodes"), index, IdentifierName("notificationId")
            ])
        );

    private IEnumerable<MemberDeclarationSyntax> OptionalMultiReference(Reference reference) =>
        new List<MemberDeclarationSyntax>
        {
            MultipleLinkField(reference),
            MultipleLinkProperty(reference, AsReadOnlyCall(reference)), 
            TryGetMultiple()
        }.Concat(
            LinkAdder(reference, [
                SafeNodesVariable(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(reference),
                AddMultipleReferenceEmitterVariable(MemberAccess(FeatureField(reference), IdentifierName("Count"))),
                EmitterCollectOldDataCall(),
                SimpleAddRangeCall(reference),
                EmitterNotifyCall(),
                ReturnStatement(This())
            ])
        ).Concat(
            LinkInserter(reference, [
                AssureInRangeCall(reference),
                SafeNodesVariable(),
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
                SafeNodesVariable(),
                AssureNotNullCall(reference),
                AssureNotNullMembersCall(reference),
                SimpleRemoveAllCall(reference),
                ReturnStatement(This())
            ])
        );

    private bool IsReferenceType(Property property) =>
        !(_builtIns.Boolean.EqualsIdentity(property.Type) ||
          _builtIns.Integer.EqualsIdentity(property.Type));

    private ExpressionStatementSyntax AssureNoSelfMoveCall(Containment containment) =>
        ExpressionStatement(Call("AssureNoSelfMove",
            IdentifierName("index"),
            IdentifierName("safeNodes"),
            FeatureField(containment)
        ));


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

    private ExpressionStatementSyntax AssureNotNullMembersCall(Reference reference) =>
        ExpressionStatement(Call("AssureNotNullMembers",
            IdentifierName("safeNodes"),
            MetaProperty(reference)
        ));

    private ExpressionStatementSyntax AsureNotNullCall() =>
        ExpressionStatement(Call("AssureNotNull",
            IdentifierName("value"),
            MetaProperty(feature)
        ));

    private static LocalDeclarationStatementSyntax SafeNodesVariable() =>
        Variable("safeNodes", Var(), OptionalNodesToList());

    private static ConditionalAccessExpressionSyntax OptionalNodesToList() =>
        ConditionalAccessExpression(IdentifierName("nodes"),
            InvocationExpression(MemberBindingExpression(IdentifierName("ToList")))
        );

    private ExpressionStatementSyntax SetParentNullCall(Containment containment) =>
        ExpressionStatement(Call("SetParentNull", FeatureField(containment)));

    private ExpressionStatementSyntax AttachChildCall() =>
        ExpressionStatement(Call("AttachChild", IdentifierName("value")));

    private InvocationExpressionSyntax AsReadOnlyCall(Link link) =>
        InvocationExpression(MemberAccess(FeatureField(link), IdentifierName("AsReadOnly")));

    private InvocationExpressionSyntax AsNonEmptyReadOnlyCall(Link link) =>
        Call("AsNonEmptyReadOnly", FeatureField(link), MetaProperty(link));

    private ExpressionStatementSyntax RequiredRemoveSelfParentCall(Containment containment) =>
        ExpressionStatement(Call("RemoveSelfParent",
            IdentifierName("safeNodes"),
            FeatureField(containment),
            MetaProperty(containment),
            CallGeneric("ContainmentRemover", AsType(containment.Type, writeable: true), MetaProperty(containment))
        ));

    private ExpressionStatementSyntax OptionalRemoveSelfParentCall(Containment containment) =>
        ExpressionStatement(Call("RemoveSelfParent",
            OptionalNodesToList(),
            FeatureField(containment),
            MetaProperty(containment),
            CallGeneric("ContainmentRemover", AsType(containment.Type, writeable: true), MetaProperty(containment))
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
            AsArguments([IdentifierName("index"), IdentifierName("safeNodes")])
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

    private ExpressionStatementSyntax AssignFeatureField() =>
        Assignment(FeatureField(feature).ToString(), IdentifierName("value"));

    private FieldDeclarationSyntax SingleFeatureField(bool writable = false) =>
        Field(FeatureField(feature).ToString(), NullableType(AsType(feature.GetFeatureType(), writeable: writable)),
                Null())
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private FieldDeclarationSyntax MultipleLinkField(Link link, bool writeable = false) =>
        Field(FeatureField(link).ToString(), AsType(typeof(List<>), AsType(link.Type, writeable: writeable)),
                Collection([]))
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword));

    private PropertyDeclarationSyntax SingleRequiredFeatureProperty(bool writeable = false) =>
        Property(FeatureProperty(feature).ToString(), AsType(feature.GetFeatureType(), writeable: writeable),
                BinaryExpression(
                    SyntaxKind.CoalesceExpression,
                    FeatureField(feature),
                    ThrowExpression(NewCall([MetaProperty(feature)], AsType(typeof(UnsetFeatureException))))
                ),
                InvocationExpression(FeatureSet(), AsArguments([IdentifierName("value")]))
            )
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(feature),
                FeatureAttribute(),
                ObsoleteAttribute(feature)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(feature)
                .Concat(XdocThrows($"If {FeatureProperty(feature)} has not been set",
                    AsType(typeof(UnsetFeatureException))))
            );

    private AttributeSyntax FeatureAttribute() =>
        AsAttribute(AsType(typeof(LionCoreFeature)),
        [
            ("Kind",
                MemberAccess(
                    IdentifierName(AsType(typeof(LionCoreFeatureKind)).ToString()),
                    IdentifierName(feature switch
                    {
                        Property => "Property",
                        Containment => "Containment",
                        Reference => "Reference",
                        _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
                    })
                )
            ),
            ("Optional", feature.Optional.AsLiteral()),
            ("Multiple", (feature is Link { Multiple: true }).AsLiteral())
        ]);

    private PropertyDeclarationSyntax AbstractSingleRequiredFeatureProperty(bool writeable = false) =>
        PropertyDeclaration(AsType(feature.GetFeatureType(), writeable: writeable),
                Identifier(FeatureProperty(feature).ToString()))
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ])))
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(feature),
                FeatureAttribute(),
                ObsoleteAttribute(feature)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(feature));

    private PropertyDeclarationSyntax SingleOptionalFeatureProperty(bool writeable = false) =>
        Property(FeatureProperty(feature).ToString(),
                NullableType(AsType(feature.GetFeatureType(), writeable: writeable)),
                FeatureField(feature),
                InvocationExpression(FeatureSet(), AsArguments([IdentifierName("value")]))
            )
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(feature),
                FeatureAttribute(),
                ObsoleteAttribute(feature)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(feature));

    private PropertyDeclarationSyntax AbstractSingleOptionalFeatureProperty() =>
        PropertyDeclaration(NullableType(AsType(feature.GetFeatureType())),
                Identifier(FeatureProperty(feature).ToString()))
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ])))
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(feature),
                FeatureAttribute(),
                ObsoleteAttribute(feature)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .Xdoc(XdocDefault(feature));

    private PropertyDeclarationSyntax MultipleLinkProperty(Link link, InvocationExpressionSyntax getter, bool writeable = false) =>
        PropertyDeclaration(AsType(typeof(IReadOnlyList<>), AsType(link.Type, writeable: writeable)),
                Identifier(FeatureProperty(link).ToString()))
            .WithAccessorList(AccessorList(List([
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(getter))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(InvocationExpression(LinkAdd(link),
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
            .Xdoc(XdocDefault(link));

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

    private List<MethodDeclarationSyntax> RequiredFeatureSetter(List<StatementSyntax> body, bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractRequiredFeatureSetter(writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractRequiredFeatureSetter(writeable),
                    SetCall())
            );

        return result;
    }

    private InvocationExpressionSyntax SetCall() =>
        Call(FeatureSet().ToString(), IdentifierName("value"));

    private MethodDeclarationSyntax AbstractRequiredFeatureSetter(bool writeable = false) =>
        Method(FeatureSet().ToString(), AsType(classifier),
                [
                    Param("value", AsType(feature.GetFeatureType(), writeable: writeable)),
                    ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(feature));

    private IEnumerable<MethodDeclarationSyntax> OptionalFeatureSetter(List<StatementSyntax> body,
        bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractOptionalFeatureSetter(writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractOptionalFeatureSetter(writeable), SetCall())
            );

        return result;
    }

    private IEnumerable<XmlNodeSyntax> XdocDefault(Feature ffeature) =>
        XdocKeyed(ffeature)
            .Concat(XdocRemarks(ffeature));

    private static IEnumerable<XmlNodeSyntax> XdocRemarks(Feature feature)
    {
        var builder = new StringBuilder();
        builder.Append(feature.Optional ? "Optional " : "Required ");
        if (feature is Link l)
        {
            builder.Append(l.Multiple ? "Multiple " : "Single ");
        }

        builder.Append(feature switch
        {
            Property => "Property",
            Containment => "Containment",
            Reference => "Reference",
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        });
        return XdocLine(builder.ToString(), "remarks");
    }

    private static IEnumerable<XmlNodeSyntax> XdocThrows(string text, TypeSyntax exception) =>
    [
        XdocSlashes(),
        XmlElement("exception", new SyntaxList<XmlNodeSyntax>(XdocText(text)))
            .AddStartTagAttributes(XmlCrefAttribute(NameMemberCref(exception))),
        XdocNewline()
    ];

    private bool InheritedFromInterface() =>
        !classifier.EqualsIdentity(feature.GetFeatureClassifier());

    private MethodDeclarationSyntax InterfaceDelegator(MethodDeclarationSyntax impl,
        ExpressionSyntax expression)
    {
        var modifiers = impl.Modifiers;
        return impl
            .WithReturnType(AsType(feature.GetFeatureClassifier(), writeable: true))
            .WithModifiers(AsModifiers())
            .AppendXdoc(modifiers, SyntaxKind.None, [])
            .WithExplicitInterfaceSpecifier(
                ExplicitInterfaceSpecifier(IdentifierName(AsType(feature.GetFeatureClassifier(), writeable: true)
                    .ToString()))
            )
            .WithExpressionBody(ArrowExpressionClause(expression))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private MethodDeclarationSyntax AbstractOptionalFeatureSetter(bool writeable = false) =>
        Method(FeatureSet().ToString(), AsType(classifier), [
                Param("value", NullableType(AsType(feature.GetFeatureType(), writeable: writeable))),
                ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(feature));

    private IEnumerable<MethodDeclarationSyntax> LinkRemover(Link link, List<StatementSyntax> body, bool writeable = false)
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
                    Param("nodes", AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable))),
                    ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(link));

    private IEnumerable<MethodDeclarationSyntax> LinkInserter(Link link, List<StatementSyntax> body, bool writeable = false)
    {
        List<MethodDeclarationSyntax> result =
        [
            AbstractLinkInserter(link, writeable: writeable)
                .WithBody(AsStatements(body))
        ];

        if (InheritedFromInterface())
            result.Insert(0,
                InterfaceDelegator(AbstractLinkInserter(link, writeable: writeable),
                    Call(LinkInsert(link).ToString(), IdentifierName("index"), IdentifierName("nodes"))
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

    private IEnumerable<MethodDeclarationSyntax> LinkAdder(Link link, List<StatementSyntax> body, bool writeable = false)
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
                    Param("nodes", AsType(typeof(IEnumerable<>), AsType(link.Type, writeable: writeable))),
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

    private ExpressionSyntax FeatureSet() =>
        IdentifierName($"Set{feature.Name.ToFirstUpper()}");

    private string FeatureTryGetParam() =>
        _names.FeatureParam(feature);
}