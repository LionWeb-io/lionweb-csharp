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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;
using Property = Core.M3.Property;

/// <summary>
/// Generates non-feature-specific concept/annotation members.
/// Covers members:
/// - GetInternal()
/// - SetInternal()
/// - CollectAllSetFeatures()
/// </summary>
public class FeatureMethodsGenerator(Classifier classifier, INames names, LionWebVersions lionWebVersion, GeneratorConfig config)
    : ClassifierGeneratorBase(names, lionWebVersion, config)
{
    /// <inheritdoc cref="FeatureMethodsGenerator"/>
    public IEnumerable<MemberDeclarationSyntax> FeatureMethods()
    {
        if (!FeaturesToImplement(classifier).Any())
            return [];

        return
        [
            GenGetInternal(),
            GenSetInternal(),
            GenCollectAllSetFeatures()
        ];
    }

    #region GetInternal

    private MethodDeclarationSyntax GenGetInternal() =>
        Method("GetInternal", AsType(typeof(bool)), [
                Param("feature", NullableType(AsType(typeof(Feature)))),
                Param("result", NullableType(AsType(typeof(object))))
                    .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.GetInternal(feature, out result)"), ReturnTrue())
                }
                .Concat(FeaturesToImplement(classifier).Select(GenGetInternal))
                .Append(ReturnStatement(false.AsLiteral()))
            ));

    private StatementSyntax GenGetInternal(Feature feature) =>
        IfStatement(GenEqualsIdentityFeature(feature),
            AsStatements([
                Assignment("result", FeatureProperty(feature)),
                ReturnTrue()
            ])
        );

    #endregion

    #region SetInternal

    private MethodDeclarationSyntax GenSetInternal() =>
        Method("SetInternal", AsType(typeof(bool)), [
                Param("feature", NullableType(AsType(typeof(Feature)))),
                Param("value", NullableType(AsType(typeof(object)))),
                ParamWithDefaultNullValue("notificationId", AsType(typeof(INotificationId)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.SetInternal(feature, value, notificationId)"),
                        ReturnTrue())
                }
                .Concat(FeaturesToImplement(classifier).Select(GenSetInternal))
                .Append(ReturnStatement(false.AsLiteral()))
            ));

    private StatementSyntax GenSetInternal(Feature feature)
    {
        List<StatementSyntax> body = feature switch
        {
            Property { Optional: false } reqProp => GenSetInternalRequiredProperty(reqProp),
            Property { Optional: true } reqProp => GenSetInternalOptionalProperty(reqProp),
            Link { Optional: false, Multiple: false } singleReqLink => GenSetInternalSingleRequiredLink(singleReqLink),
            Link { Optional: true, Multiple: false } singleOptLink => GenSetInternalSingleOptionalLink(singleOptLink),
            Containment { Optional: true, Multiple: true } multiCont => GenSetInternalMultiOptionalContainment(
                multiCont),
            Containment { Optional: false, Multiple: true } multiCont => GenSetInternalMultiRequiredContainment(
                multiCont),
            Reference { Optional: true, Multiple: true } multiRef => GenSetInternalMultiOptionalReference(multiRef),
            Reference { Optional: false, Multiple: true } multiRef => GenSetInternalMultiRequiredReference(multiRef),
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

        return IfStatement(
            GenEqualsIdentityFeature(feature),
            AsStatements(body)
        );
    }

    private List<StatementSyntax> GenSetInternalRequiredProperty(Property property) =>
    [
        IfStatement(
            IsPatternExpression(IdentifierName("value"), AsTypePattern(property)),
            AsStatements([
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(FeatureSet(property)), AsArguments([IdentifierName("v"), IdentifierName("notificationId")]))),
                ReturnTrue()
            ])
        ),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalOptionalProperty(Property property) =>
    [
        IfStatement(
            IsPatternExpression(IdentifierName("value"), NullOrTypePattern(property)),
            AsStatements([
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(FeatureSet(property)), AsArguments([CastValueType(property), IdentifierName("notificationId")]))),
                ReturnTrue()
            ])
        ),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalSingleRequiredLink(Link link) =>
    [
        IfStatement(
            IsPatternExpression(IdentifierName("value"), AsTypePattern(link)),
            AsStatements([
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(FeatureSet(link)), AsArguments([IdentifierName("v"), IdentifierName("notificationId")]))),
                ReturnTrue()
            ])
        ),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalSingleOptionalLink(Link link) =>
    [
        IfStatement(
            IsPatternExpression(IdentifierName("value"), NullOrTypePattern(link, writeable: link is Containment)),
            AsStatements([
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(FeatureSet(link)), AsArguments([CastValueType(link, writeable: link is Containment), IdentifierName("notificationId")]))),
                ReturnTrue()
            ])
        ),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalMultiOptionalContainment(Containment containment) =>
    [
        SafeNodesVar(containment),
        SetContainmentEmitterVariable(containment),
        EmitterCollectOldDataCall(),
        RemoveSelfParentCall(containment),
        OptionalAddRangeCall(containment),
        EmitterNotifyCall(),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiRequiredContainment(Containment containment) =>
    [
        SafeNodesVar(containment),
        AssureNonEmptyCall(containment),
        SetContainmentEmitterVariable(containment),
        EmitterCollectOldDataCall(),
        RemoveSelfParentCall(containment),
        RequiredAddRangeCall(containment),
        EmitterNotifyCall(),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiOptionalReference(Reference reference) =>
    [
        SafeNodesVar(reference),
        AssureNotNullCall(reference),
        AssureNotNullMembersCall(reference),
        SetReferenceEmitterVariable(reference),
        EmitterCollectOldDataCall(),
        ClearFieldCall(reference),
        ReferenceAddRangeCall(reference),
        EmitterNotifyCall(),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiRequiredReference(Reference reference) =>
    [
        SafeNodesVar(reference),
        AssureNonEmptyCall(reference),
        SetReferenceEmitterVariable(reference),
        EmitterCollectOldDataCall(),
        ClearFieldCall(reference),
        ReferenceAddRangeCall(reference),
        EmitterNotifyCall(),
        ReturnTrue()
    ];

    private ExpressionStatementSyntax ClearFieldCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(MemberAccess(FeatureField(reference), IdentifierName("Clear"))));

    private ExpressionStatementSyntax ReferenceAddRangeCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(MemberAccess(FeatureField(reference), IdentifierName("AddRange")),
            AsArguments([IdentifierName("safeNodes")])));

    private LocalDeclarationStatementSyntax SetContainmentEmitterVariable(Containment containment) =>
        Variable(
            "emitter",
            AsType(typeof(ContainmentSetNotificationEmitter<>), AsType(containment.GetFeatureType(), writeable:true)),
            NewCall([
                MetaProperty(containment), This(), IdentifierName("safeNodes"), FeatureField(containment), IdentifierName("notificationId")
            ])
        );

    private LocalDeclarationStatementSyntax SetReferenceEmitterVariable(Reference reference) =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceSetNotificationEmitter<>), AsType(reference.GetFeatureType())),
            NewCall([
                MetaProperty(reference), This(), IdentifierName("safeNodes"), FeatureField(reference), IdentifierName("notificationId")
            ])
        );

    private BinaryPatternSyntax NullOrTypePattern(Feature feature, bool writeable = false) =>
        BinaryPattern(
            SyntaxKind.OrPattern,
            ConstantPattern(Null()),
            TypePattern(AsType(feature.GetFeatureType(), true, writeable: writeable))
        );

    private DeclarationPatternSyntax AsTypePattern(Feature feature) =>
        DeclarationPattern(
            AsType(feature.GetFeatureType(), true, feature is Containment),
            SingleVariableDesignation(Identifier("v"))
        );

    private ExpressionStatementSyntax AssignToFeatureProperty(Feature feature, ExpressionSyntax expression) =>
        Assignment(FeatureProperty(feature).ToString(), expression);

    private ThrowStatementSyntax GenThrowInvalidValueException() =>
        ThrowStatement(
            NewCall([IdentifierName("feature"), IdentifierName("value")], AsType(typeof(InvalidValueException)))
        );

    private LocalDeclarationStatementSyntax EnumerableVar(Link link) =>
        Variable("enumerable", Var(), AsNodesCall(link, writeable: link is Containment));

    private LocalDeclarationStatementSyntax EnumerableToListVar(Link link) =>
        Variable("enumerable", Var(),
            InvocationExpression(MemberAccess(AsNodesCall(link, writeable: link is Containment), IdentifierName("ToList")))
        );

    private LocalDeclarationStatementSyntax SafeNodesVar(Link link) =>
        Variable("safeNodes", Var(),
            InvocationExpression(MemberAccess(AsNodesCall(link, writeable: link is Containment), IdentifierName("ToList")))
        );

    private ExpressionStatementSyntax AssureNonEmptyCall(Link link) =>
        ExpressionStatement(Call("AssureNonEmpty", IdentifierName("safeNodes"), MetaProperty(link)));

    private ExpressionStatementSyntax RemoveSelfParentCall(Containment containment) =>
        ExpressionStatement(Call("RemoveSelfParent",
            InvocationExpression(MemberAccess(FeatureField(containment), IdentifierName("ToList"))),
            FeatureField(containment),
            MetaProperty(containment)
        ));

    private ExpressionStatementSyntax LinkAddCall(Link link) =>
        ExpressionStatement(InvocationExpression(LinkAdd(link),
            AsArguments([IdentifierName("enumerable")])
        ));

    private ExpressionStatementSyntax ClearCall(Reference reference) =>
        ExpressionStatement(
            InvocationExpression(MemberAccess(FeatureField(reference), IdentifierName("Clear")))
        );

    private CastExpressionSyntax CastValueType(Feature feature, bool writeable = false) => 
        CastExpression(NullableType(AsType(feature.GetFeatureType(), true, writeable: writeable)), IdentifierName("value"));

    private InvocationExpressionSyntax AsNodesCall(Link link, bool writeable = false)
    {
        var invocationExpressionSyntax = InvocationExpression(
            MemberAccess(MetaProperty(link),
                Generic("AsNodes",  AsType(link.Type, true, writeable: writeable) )
            ),
            AsArguments([IdentifierName("value")])
        );

        return invocationExpressionSyntax;
    }

    #endregion

    #region CollectAllSetFeatures

    private MethodDeclarationSyntax GenCollectAllSetFeatures() =>
        Method("CollectAllSetFeatures", AsType(typeof(IEnumerable<Feature>)))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(
                new List<StatementSyntax>
                    {
                        ParseStatement("List<Feature> result = base.CollectAllSetFeatures().ToList();")
                    }
                    .Concat(FeaturesToImplement(classifier).Select(GenCollectAllSetFeatures))
                    .Append(ReturnStatement(IdentifierName("result")))
            ));

    private StatementSyntax GenCollectAllSetFeatures(Feature feature) =>
        IfStatement(
            InvocationExpression(IdentifierName(FeatureTryGet(feature)))
                .WithArgumentList(ArgumentList(SingletonSeparatedList(
                    Argument(Underscore())
                        .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword))
                ))),
            ExpressionStatement(InvocationExpression(
            MemberAccess(IdentifierName("result"), IdentifierName("Add")),
            AsArguments([MetaProperty(feature)])
            ))
        );

    #endregion

    private ReturnStatementSyntax ReturnTrue() =>
        ReturnStatement(true.AsLiteral());

    private InvocationExpressionSyntax GenEqualsIdentityFeature(Feature feature) =>
        InvocationExpression(MemberAccess(MetaProperty(feature), IdentifierName("EqualsIdentity")),
            AsArguments([IdentifierName("feature")])
        );
}