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
using Core.M1.Event.Partition.Emitter;
using Core.M2;
using Core.M3;
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
public class FeatureMethodsGenerator(Classifier classifier, INames names, LionWebVersions lionWebVersion)
    : ClassifierGeneratorBase(names, lionWebVersion)
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
                Param("value", NullableType(AsType(typeof(object))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.SetInternal(feature, value)"),
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
                AssignToFeatureProperty(property, IdentifierName("v")),
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
                AssignToFeatureProperty(property, CastValueType(property)),
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
                AssignToFeatureProperty(link, IdentifierName("v")),
                ReturnTrue()
            ])
        ),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalSingleOptionalLink(Link link) =>
    [
        IfStatement(
            IsPatternExpression(IdentifierName("value"), NullOrTypePattern(link)),
            AsStatements([
                AssignToFeatureProperty(link, CastValueType(link)),
                ReturnTrue()
            ])
        ),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalMultiOptionalContainment(Containment containment) =>
    [
        SafeNodesVar(containment),
        SetContainmentEventVariable(containment),
        EventCollectOldDataCall(),
        RemoveSelfParentCall(containment),
        OptionalAddRangeCall(containment),
        EventRaiseEventCall(),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiRequiredContainment(Containment containment) =>
    [
        SafeNodesVar(containment),
        AssureNonEmptyCall(containment),
        SetContainmentEventVariable(containment),
        EventCollectOldDataCall(),
        RemoveSelfParentCall(containment),
        RequiredAddRangeCall(containment),
        EventRaiseEventCall(),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiOptionalReference(Reference reference) =>
    [
        SafeNodesVar(reference),
        AssureNotNullCall(reference),
        AssureNotNullMembersCall(reference),
        SetReferenceEventVariable(reference),
        EventCollectOldDataCall(),
        ClearFieldCall(reference),
        ReferenceAddRangeCall(reference),
        EventRaiseEventCall(),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiRequiredReference(Reference reference) =>
    [
        SafeNodesVar(reference),
        AssureNonEmptyCall(reference),
        SetReferenceEventVariable(reference),
        EventCollectOldDataCall(),
        ClearFieldCall(reference),
        ReferenceAddRangeCall(reference),
        EventRaiseEventCall(),
        ReturnTrue()
    ];

    private ExpressionStatementSyntax ClearFieldCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(MemberAccess(FeatureField(reference), IdentifierName("Clear"))));

    private ExpressionStatementSyntax ReferenceAddRangeCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(MemberAccess(FeatureField(reference), IdentifierName("AddRange")),
            AsArguments([IdentifierName("safeNodes")])));

    private LocalDeclarationStatementSyntax SetContainmentEventVariable(Containment containment) =>
        Variable(
            "evt",
            AsType(typeof(ContainmentSetEventEmitter<>), AsType(containment.GetFeatureType())),
            NewCall([
                MetaProperty(containment), This(), IdentifierName("safeNodes"), FeatureField(containment)
            ])
        );

    private LocalDeclarationStatementSyntax SetReferenceEventVariable(Reference reference) =>
        Variable(
            "evt",
            AsType(typeof(ReferenceSetEventEmitter<>), AsType(reference.GetFeatureType())),
            NewCall([
                MetaProperty(reference), This(), IdentifierName("safeNodes"), FeatureField(reference)
            ])
        );

    private BinaryPatternSyntax NullOrTypePattern(Feature feature) =>
        BinaryPattern(
            SyntaxKind.OrPattern,
            ConstantPattern(Null()),
            TypePattern(AsType(feature.GetFeatureType(), true))
        );

    private DeclarationPatternSyntax AsTypePattern(Feature feature) =>
        DeclarationPattern(
            AsType(feature.GetFeatureType(), true),
            SingleVariableDesignation(Identifier("v"))
        );

    private ExpressionStatementSyntax AssignToFeatureProperty(Feature feature, ExpressionSyntax expression) =>
        Assignment(FeatureProperty(feature).ToString(), expression);

    private ThrowStatementSyntax GenThrowInvalidValueException() =>
        ThrowStatement(
            NewCall([IdentifierName("feature"), IdentifierName("value")], AsType(typeof(InvalidValueException)))
        );

    private LocalDeclarationStatementSyntax SafeNodesVar(Link link) =>
        Variable("safeNodes", Var(),
            InvocationExpression(MemberAccess(AsNodesCall(link), IdentifierName("ToList")))
        );

    private ExpressionStatementSyntax AssureNonEmptyCall(Link link) =>
        ExpressionStatement(Call("AssureNonEmpty", IdentifierName("safeNodes"), MetaProperty(link)));

    private ExpressionStatementSyntax RemoveSelfParentCall(Containment containment) =>
        ExpressionStatement(Call("RemoveSelfParent",
            InvocationExpression(MemberAccess(FeatureField(containment), IdentifierName("ToList"))),
            FeatureField(containment),
            MetaProperty(containment)
        ));

    private CastExpressionSyntax CastValueType(Feature feature) =>
        CastExpression(NullableType(AsType(feature.GetFeatureType(), true)), IdentifierName("value"));

    private InvocationExpressionSyntax AsNodesCall(Link link) =>
        InvocationExpression(
            MemberAccess(MetaProperty(link),
                Generic("AsNodes", AsType(link.Type, true))
            ),
            AsArguments([IdentifierName("value")])
        );

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

    private StatementSyntax GenCollectAllSetFeatures(Feature feature)
    {
        ExpressionSyntax cond = feature switch
        {
            Property p => NotEquals(
                FeatureField(p),
                Default()
            ),
            Link { Multiple: false } singleLink => NotEquals(
                FeatureField(singleLink),
                Default()
            ),
            Link { Multiple: true } multiLink => NotEquals(
                MemberAccess(FeatureField(multiLink), IdentifierName("Count")),
                0.AsLiteral()
            ),
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

        return IfStatement(cond, ExpressionStatement(InvocationExpression(
            MemberAccess(IdentifierName("result"), IdentifierName("Add")),
            AsArguments([MetaProperty(feature)])
        )));
    }

    #endregion

    private ReturnStatementSyntax ReturnTrue() =>
        ReturnStatement(true.AsLiteral());

    private InvocationExpressionSyntax GenEqualsIdentityFeature(Feature feature) =>
        InvocationExpression(MemberAccess(MetaProperty(feature), IdentifierName("EqualsIdentity")),
            AsArguments([IdentifierName("feature")])
        );
}