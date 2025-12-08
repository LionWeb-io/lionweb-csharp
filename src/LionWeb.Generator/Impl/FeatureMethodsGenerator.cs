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
/// - AddInternal()
/// - InsertInternal()
/// - RemoveInternal()
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

        IEnumerable<MemberDeclarationSyntax> featureMethods = new List<MemberDeclarationSyntax>
        {
            GenGetInternal(),
            GenTryGetPropertyRaw(),
            GenTryGetContainmentRaw(),
            GenTryGetContainmentsRaw(),
            GenTryGetReferenceRaw(),
            GenTryGetReferencesRaw(),
            GenSetInternal(),
            GenSetPropertyRaw(),
            GenSetContainmentRaw(),
            GenSetReferenceRaw(),
            GenCollectAllSetFeatures(),
            GenAddContainmentsRaw(),
            GenAddReferencesRaw(),
            GenInsertContainmentsRaw(),
            GenInsertReferencesRaw(),
            GenRemoveContainmentsRaw(),
            GenRemoveReferencesRaw()
        }.Where(m => m is not null);

        if (!FeaturesToImplement(classifier).OfType<Link>().Any(link => link.Multiple))
        {
            return featureMethods;
        }

        var addInternal =  GenAddInternal();
        var insertInternal = GenInsertInternal();
        var removeInternal = GenRemoveInternal();

        featureMethods = featureMethods
            .Append(addInternal)
            .Append(insertInternal)
            .Append(removeInternal);

        return featureMethods;
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

    private MethodDeclarationSyntax? GenTryGetPropertyRaw()
    {
        var properties = FeaturesToImplement(classifier).OfType<Property>().ToList();
        if (properties.Count == 0)
            return null;
        
        return Method("TryGetPropertyRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Property))),
                Param("result", NullableType(AsType(typeof(object))))
                    .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword, SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.TryGetPropertyRaw(feature, out result)"), ReturnTrue())
                }
                .Concat(properties.Select(GenGetInternalRaw))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenTryGetContainmentRaw()
    {
        var containments = FeaturesToImplement(classifier)
            .OfType<Containment>()
            .Where(c => !c.Multiple)
            .ToList();
        if (containments.Count == 0)
            return null;
        
        return Method("TryGetContainmentRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Containment))),
                Param("result", NullableType(AsType(typeof(IReadableNode))))
                    .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword, SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.TryGetContainmentRaw(feature, out result)"), ReturnTrue())
                }
                .Concat(containments.Select(GenGetInternalRaw))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenTryGetContainmentsRaw()
    {
        var containments = FeaturesToImplement(classifier)
            .OfType<Containment>()
            .Where(c => c.Multiple)
            .ToList();
        if (containments.Count == 0)
            return null;
        
        return Method("TryGetContainmentsRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Containment))),
                Param("result", AsType(typeof(IReadOnlyList<IReadableNode>)))
                    .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword, SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.TryGetContainmentsRaw(feature, out result)"), ReturnTrue())
                }
                .Concat(containments.Select(GenGetInternalRaw))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenTryGetReferenceRaw()
    {
        var references = FeaturesToImplement(classifier)
            .OfType<Reference>()
            .Where(r => !r.Multiple)
            .ToList();
        if (references.Count == 0)
            return null;
        
        return Method("TryGetReferenceRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Reference))),
                Param("result", NullableType(AsType(typeof(IReferenceTarget))))
                    .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword, SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.TryGetReferenceRaw(feature, out result)"), ReturnTrue())
                }
                .Concat(references.Select(GenGetInternalRaw))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenTryGetReferencesRaw()
    {
        var references = FeaturesToImplement(classifier)
            .OfType<Reference>()
            .Where(r => r.Multiple)
            .ToList();
        if (references.Count == 0)
            return null;
        
        return Method("TryGetReferencesRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Reference))),
                Param("result", AsType(typeof(IReadOnlyList<IReferenceTarget>)))
                    .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword, SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.TryGetReferencesRaw(feature, out result)"), ReturnTrue())
                }
                .Concat(references.Select(GenGetInternalRaw))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private StatementSyntax GenGetInternalRaw(Feature feature) =>
        IfStatement(GenEqualsIdentityFeature(feature),
            AsStatements([
                Assignment("result", FeatureField(feature)),
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
            Containment { Optional: false, Multiple: false } singleReqContainment => GenSetInternalSingleRequiredContainment(singleReqContainment),
            Containment { Optional: true, Multiple: false } singleOptContainment => GenSetInternalSingleOptionalContainment(singleOptContainment),
            Reference { Optional: false, Multiple: false } singleReqReference => GenSetInternalSingleRequiredReference(singleReqReference),
            Reference { Optional: true, Multiple: false } singleOptReference => GenSetInternalSingleOptionalReference(singleOptReference),
            Containment { Optional: true, Multiple: true } multiCont => GenSetInternalMultiOptionalContainment(multiCont),
            Containment { Optional: false, Multiple: true } multiCont => GenSetInternalMultiRequiredContainment(multiCont),
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
                        IdentifierName(FeatureSet(property)), AsArguments([IdentifierName("v")]))),
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
                        IdentifierName(FeatureSet(property)), AsArguments([CastValueType(property)]))),
                ReturnTrue()
            ])
        ),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalSingleRequiredContainment(Containment containment) =>
    [
        GenSetInternalSingleRequiredLink(containment, true),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalSingleRequiredReference(Reference reference) =>
    [
        GenSetInternalSingleRequiredLink(reference, false),
        GenSetInternalSingleReference(reference),
        GenThrowInvalidValueException()
    ];

    private IfStatementSyntax GenSetInternalSingleRequiredLink(Link link, bool writeable) =>
        IfStatement(
            IsPatternExpression(IdentifierName("value"), AsTypePattern(link, writeable: writeable)),
            AsStatements([
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(FeatureSet(link)),
                        AsArguments([IdentifierName("v")]))),
                ReturnTrue()
            ])
        );

    private IfStatementSyntax GenSetInternalSingleReference(Reference reference) =>
        IfStatement(
            IsPatternExpression(IdentifierName("value"), DeclarationPattern(
                    AsType(typeof(ReferenceTarget)),
                    SingleVariableDesignation(Identifier("target"))
                )
            ),
            AsStatements([
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(FeatureSet(reference)),
                        AsArguments([IdentifierName("target")]))),
                ReturnTrue()
            ])
        );

    private List<StatementSyntax> GenSetInternalSingleOptionalContainment(Containment containment) =>
    [
        GenSetInternalSingleOptionalLink(containment, true),
        GenThrowInvalidValueException()
    ];

    private List<StatementSyntax> GenSetInternalSingleOptionalReference(Reference reference) =>
    [
        GenSetInternalSingleOptionalLink(reference, false),
        GenSetInternalSingleReference(reference),
        GenThrowInvalidValueException()
    ];

    private IfStatementSyntax GenSetInternalSingleOptionalLink(Link link, bool writeable) =>
        IfStatement(
            IsPatternExpression(IdentifierName("value"), NullOrTypePattern(link, writeable: writeable)),
            AsStatements([
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(FeatureSet(link)),
                        AsArguments([CastValueType(link, writeable: writeable)]))),
                ReturnTrue()
            ])
        );

    private List<StatementSyntax> GenSetInternalMultiOptionalContainment(Containment containment) =>
    [
        SafeNodesVar(containment, writeable: true),
        SetContainmentEmitterVariable(containment),
        EmitterCollectOldDataCall(),
        IfStatement(
            InvocationExpression(FeatureSetRaw(containment), AsArguments([IdentifierName("safeNodes")])),
            EmitterNotifyCall()
        ),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiRequiredContainment(Containment containment) =>
    [
        SafeNodesVar(containment, writeable: true),
        AssureNonEmptyCall(containment),
        SetContainmentEmitterVariable(containment),
        EmitterCollectOldDataCall(),
        IfStatement(
            InvocationExpression(FeatureSetRaw(containment), AsArguments([IdentifierName("safeNodes")])),
            EmitterNotifyCall()
        ),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenSetInternalMultiOptionalReference(Reference reference) =>
    [
        SafeNodesVarReference(reference),
        AssureNotNullCall(reference),
        AssureNotNullMembersCall(reference),
        SetReferenceEmitterVariable(reference),
        EmitterCollectOldDataCall(),
        IfStatement(
            InvocationExpression(FeatureSetRaw(reference), AsArguments([IdentifierName("safeNodes")])),
            EmitterNotifyCall()
        ),
        ReturnTrue()
    ];

    private LocalDeclarationStatementSyntax SafeNodesVarReference(Reference reference) =>
        Variable("safeNodes", Var(),
            InvocationExpression(MemberAccess(InvocationExpression(
                MemberAccess(MetaProperty(reference),
                    Generic("AsReferenceTargets", AsType(reference.Type, true))
                ),
                AsArguments([IdentifierName("value")])
            ), IdentifierName("ToList")))
        );

    private List<StatementSyntax> GenSetInternalMultiRequiredReference(Reference reference) =>
    [
        SafeNodesVarReference(reference),
        AssureNonEmptyCall(reference),
        SetReferenceEmitterVariable(reference),
        EmitterCollectOldDataCall(),
        IfStatement(
            InvocationExpression(FeatureSetRaw(reference), AsArguments([IdentifierName("safeNodes")])),
            EmitterNotifyCall()
        ),
        ReturnTrue()
    ];

    private ExpressionStatementSyntax ClearFieldCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(MemberAccess(FeatureField(reference), IdentifierName("Clear"))));

    private ExpressionStatementSyntax ReferenceAddRangeCall(Reference reference) =>
        ExpressionStatement(InvocationExpression(MemberAccess(FeatureField(reference), IdentifierName("AddRange")),
            AsArguments([IdentifierName("safeNodes")])));

    private MethodDeclarationSyntax? GenSetPropertyRaw()
    {
        var properties = FeaturesToImplement(classifier).OfType<Property>().ToList();
        if (properties.Count == 0)
            return null;
        
        return Method("SetPropertyRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Property))),
                Param("value", NullableType(AsType(typeof(object))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.SetPropertyRaw(feature, value)"),
                        ReturnTrue())
                }
                .Concat(properties.Select(p => GenSetSingleFeatureRaw(p)))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenSetContainmentRaw()
    {
        var containments = FeaturesToImplement(classifier)
            .OfType<Containment>()
            .Where(c => !c.Multiple)
            .ToList();
        if (containments.Count == 0)
            return null;
        
        return Method("SetContainmentRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Containment))),
                Param("value", NullableType(AsType(typeof(IWritableNode))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.SetContainmentRaw(feature, value)"),
                        ReturnTrue())
                }
                .Concat(containments.Select(c => GenSetSingleFeatureRaw(c, writeable:true)))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenSetReferenceRaw()
    {
        var references = FeaturesToImplement(classifier)
            .OfType<Reference>()
            .Where(r => !r.Multiple)
            .ToList();
        if (references.Count == 0)
            return null;
        
        return Method("SetReferenceRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Reference))),
                Param("value", NullableType(AsType(typeof(ReferenceTarget))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.SetReferenceRaw(feature, value)"),
                        ReturnTrue())
                }
                .Concat(references.Select(feature => IfStatement(
                        GenEqualsIdentityFeature(feature),
                    ReturnStatement(Call(FeatureSetRaw(feature).ToString(), IdentifierName("value")))
                )))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private IfStatementSyntax GenSetSingleFeatureRaw(Feature feature, bool writeable = false) =>
        IfStatement(
            And(
                GenEqualsIdentityFeature(feature),
                IsPatternExpression(IdentifierName("value"), NullOrTypePattern(feature, writeable:writeable))
            ),
            ReturnStatement(Call(FeatureSetRaw(feature).ToString(), CastValueType(feature, writeable: writeable)))
        );

    private MethodDeclarationSyntax? GenAddContainmentsRaw()
    {
        var containments = FeaturesToImplement(classifier)
            .OfType<Containment>()
            .Where(r => r.Multiple)
            .ToList();
        if (containments.Count == 0)
            return null;
        
        return Method("AddContainmentsRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Containment))),
                Param("value", NullableType(AsType(typeof(IWritableNode))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.AddContainmentsRaw(feature, value)"),
                        ReturnTrue())
                }
                .Concat(containments.Select(feature => IfStatement(
                    And(
                        GenEqualsIdentityFeature(feature),
                        IsPatternExpression(IdentifierName("value"), NullOrTypePattern(feature, writeable:true))
                        
                    ),
                    ReturnStatement(Call(LinkAddRaw(feature).ToString(), CastValueType(feature, writeable:true)))
                )))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenAddReferencesRaw()
    {
        var references = FeaturesToImplement(classifier)
            .OfType<Reference>()
            .Where(r => r.Multiple)
            .ToList();
        if (references.Count == 0)
            return null;
        
        return Method("AddReferencesRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Reference))),
                Param("value", NullableType(AsType(typeof(ReferenceTarget))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.AddReferencesRaw(feature, value)"),
                        ReturnTrue())
                }
                .Concat(references.Select(feature => IfStatement(
                        GenEqualsIdentityFeature(feature),
                    ReturnStatement(Call(LinkAddRaw(feature).ToString(), IdentifierName("value")))
                )))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenInsertContainmentsRaw()
    {
        var containments = FeaturesToImplement(classifier)
            .OfType<Containment>()
            .Where(r => r.Multiple)
            .ToList();
        if (containments.Count == 0)
            return null;
        
        return Method("InsertContainmentsRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Containment))),
                Param("index", AsType(typeof(int))),
                Param("value", NullableType(AsType(typeof(IWritableNode))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.InsertContainmentsRaw(feature, index, value)"),
                        ReturnTrue())
                }
                .Concat(containments.Select(feature => IfStatement(
                    And(
                        GenEqualsIdentityFeature(feature),
                        IsPatternExpression(IdentifierName("value"), NullOrTypePattern(feature, writeable:true))
                        
                    ),
                    ReturnStatement(Call(LinkInsertRaw(feature).ToString(), IdentifierName("index"), CastValueType(feature, writeable:true)))
                )))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenInsertReferencesRaw()
    {
        var references = FeaturesToImplement(classifier)
            .OfType<Reference>()
            .Where(r => r.Multiple)
            .ToList();
        if (references.Count == 0)
            return null;
        
        return Method("InsertReferencesRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Reference))),
                Param("index", AsType(typeof(int))),
                Param("value", NullableType(AsType(typeof(ReferenceTarget))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.InsertReferencesRaw(feature, index, value)"),
                        ReturnTrue())
                }
                .Concat(references.Select(feature => IfStatement(
                        GenEqualsIdentityFeature(feature),
                    ReturnStatement(Call(LinkInsertRaw(feature).ToString(), IdentifierName("index"), IdentifierName("value")))
                )))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

        private MethodDeclarationSyntax? GenRemoveContainmentsRaw()
    {
        var containments = FeaturesToImplement(classifier)
            .OfType<Containment>()
            .Where(r => r.Multiple)
            .ToList();
        if (containments.Count == 0)
            return null;
        
        return Method("RemoveContainmentsRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Containment))),
                Param("value", NullableType(AsType(typeof(IWritableNode))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.RemoveContainmentsRaw(feature, value)"),
                        ReturnTrue())
                }
                .Concat(containments.Select(feature => IfStatement(
                    And(
                        GenEqualsIdentityFeature(feature),
                        IsPatternExpression(IdentifierName("value"), NullOrTypePattern(feature, writeable:true))
                        
                    ),
                    ReturnStatement(Call(LinkRemoveRaw(feature).ToString(), CastValueType(feature, writeable:true)))
                )))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private MethodDeclarationSyntax? GenRemoveReferencesRaw()
    {
        var references = FeaturesToImplement(classifier)
            .OfType<Reference>()
            .Where(r => r.Multiple)
            .ToList();
        if (references.Count == 0)
            return null;
        
        return Method("RemoveReferencesRaw", AsType(typeof(bool)), [
                Param("feature", AsType(typeof(Reference))),
                Param("value", NullableType(AsType(typeof(ReferenceTarget))))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
                SyntaxKind.OverrideKeyword))
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.RemoveReferencesRaw(feature, value)"),
                        ReturnTrue())
                }
                .Concat(references.Select(feature => IfStatement(
                        GenEqualsIdentityFeature(feature),
                    ReturnStatement(Call(LinkRemoveRaw(feature).ToString(), IdentifierName("value")))
                )))
                .Append(ReturnStatement(false.AsLiteral()))
            ));
    }

    private LocalDeclarationStatementSyntax SetContainmentEmitterVariable(Containment containment) =>
        Variable(
            "emitter",
            AsType(typeof(ContainmentSetNotificationEmitter<>), AsType(containment.GetFeatureType(), writeable: true)),
            NewCall([
                MetaProperty(containment),
                This(),
                IdentifierName("safeNodes"),
                FeatureField(containment)
            ])
        );

    private LocalDeclarationStatementSyntax SetReferenceEmitterVariable(Reference reference) =>
        Variable(
            "emitter",
            AsType(typeof(ReferenceSetNotificationEmitter<>), AsType(reference.GetFeatureType())),
            NewCall([
                MetaProperty(reference),
                This(),
                IdentifierName("safeNodes"),
                FeatureField(reference)
            ])
        );

    private BinaryPatternSyntax NullOrTypePattern(Feature feature, bool writeable = false) =>
        BinaryPattern(
            SyntaxKind.OrPattern,
            ConstantPattern(Null()),
            TypePattern(AsType(feature.GetFeatureType(), true, writeable: writeable))
        );

    private DeclarationPatternSyntax AsTypePattern(Feature feature, bool writeable = false) =>
        DeclarationPattern(
            AsType(feature.GetFeatureType(), true, writeable: writeable),
            SingleVariableDesignation(Identifier("v"))
        );

    private ExpressionStatementSyntax AssignToFeatureProperty(Feature feature, ExpressionSyntax expression) =>
        Assignment(FeatureProperty(feature).ToString(), expression);

    private ThrowStatementSyntax GenThrowInvalidValueException() =>
        ThrowStatement(
            NewCall([IdentifierName("feature"), IdentifierName("value")], AsType(typeof(InvalidValueException)))
        );

    private LocalDeclarationStatementSyntax EnumerableVar(Link link, bool writeable = false) =>
        Variable("enumerable", Var(), AsNodesCall(link, writeable: writeable));

    private LocalDeclarationStatementSyntax EnumerableToListVar(Link link, bool writeable = false) =>
        Variable("enumerable", Var(),
            InvocationExpression(MemberAccess(AsNodesCall(link, writeable: writeable), IdentifierName("ToList")))
        );

    private LocalDeclarationStatementSyntax SafeNodesVar(Link link, bool writeable = false) =>
        Variable("safeNodes", Var(),
            InvocationExpression(MemberAccess(AsNodesCall(link, writeable: writeable), IdentifierName("ToList")))
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
                Generic("AsNodes", AsType(link.Type, true, writeable: writeable))
            ),
            AsArguments([IdentifierName("value")])
        );

        return invocationExpressionSyntax;
    }

    #endregion

    #region AddInternal

    private MethodDeclarationSyntax GenAddInternal() =>
        Method("AddInternal", AsType(typeof(bool)), [
                Param("link", NullableType(AsType(typeof(Link)))),
                Param("value", AsType(typeof(IEnumerable<IReadableNode>)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.AddInternal(link, value)"),
                        ReturnTrue())
                }
                .Concat(FeaturesToImplement(classifier).OfType<Link>().Where(l => l.Multiple).Select(GenAddInternal))
                .Append(ReturnStatement(false.AsLiteral()))
            ));

    private StatementSyntax GenAddInternal(Link link)
    {
        List<StatementSyntax> body = link switch
        {
            Containment containment => GenAddInternalMultipleContainment(containment, writeable: true),
            Reference reference => GenAddInternalMultipleReference(reference),
            _ => throw new ArgumentException($"unsupported link: {link}", nameof(link))
        };

        return IfStatement(
            GenEqualsIdentityLink(link),
            AsStatements(body)
        );
    }

    private List<StatementSyntax> GenAddInternalMultipleReference(Reference reference) =>
    [
        ExpressionStatement(
            InvocationExpression(
                IdentifierName(AddLink(reference)), AsArguments([AsNodesCall(reference)]))),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenAddInternalMultipleContainment(Containment containment, bool writeable) =>
    [
        ExpressionStatement(
            InvocationExpression(
                IdentifierName(AddLink(containment)), AsArguments([AsNodesCall(containment, writeable)]))),
        ReturnTrue()
    ];
    
    #endregion

    #region InsertInternal

    private MethodDeclarationSyntax GenInsertInternal() =>
        Method("InsertInternal", AsType(typeof(bool)), [
                Param("link", NullableType(AsType(typeof(Link)))),
                Param("index", AsType(typeof(int))),
                Param("value", AsType(typeof(IEnumerable<IReadableNode>)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.InsertInternal(link, index, value)"),
                        ReturnTrue())
                }
                .Concat(FeaturesToImplement(classifier).OfType<Link>().Where(l => l.Multiple).Select(GenInsertInternal))
                .Append(ReturnStatement(false.AsLiteral()))
            ));

    
    private StatementSyntax GenInsertInternal(Link link)
    {
        List<StatementSyntax> body = link switch
        {
            Containment containment => GenInsertInternalMultipleContainment(containment, writeable: true),
            Reference reference => GenInsertInternalMultipleReference(reference),
            _ => throw new ArgumentException($"unsupported link: {link}", nameof(link))
        };

        return IfStatement(
            GenEqualsIdentityLink(link),
            AsStatements(body)
        );
    }

    private List<StatementSyntax> GenInsertInternalMultipleReference(Reference reference) =>
    [
        ExpressionStatement(
            InvocationExpression(
                IdentifierName(InsertLink(reference)), AsArguments([IdentifierName("index"), AsNodesCall(reference)]))),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenInsertInternalMultipleContainment(Containment containment, bool writeable) =>
    [
        ExpressionStatement(
            InvocationExpression(
                IdentifierName(InsertLink(containment)),
                AsArguments([IdentifierName("index"), AsNodesCall(containment, writeable)]))),
        ReturnTrue()
    ];
    
    #endregion

    #region RemoveInternal

    private MethodDeclarationSyntax GenRemoveInternal() =>
        Method("RemoveInternal", AsType(typeof(bool)), [
                Param("link", NullableType(AsType(typeof(Link)))),
                Param("value", AsType(typeof(IEnumerable<IReadableNode>)))
            ])
            .WithModifiers(AsModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc())
            .WithBody(AsStatements(new List<StatementSyntax>
                {
                    IfStatement(ParseExpression("base.RemoveInternal(link, value)"),
                        ReturnTrue())
                }
                .Concat(FeaturesToImplement(classifier).OfType<Link>().Where(l => l.Multiple).Select(GenRemoveInternal))
                .Append(ReturnStatement(false.AsLiteral()))
            ));

    #endregion
    
    private StatementSyntax GenRemoveInternal(Link link)
    {
        List<StatementSyntax> body = link switch
        {
            Containment containment => GenRemoveInternalMultipleContainment(containment, writeable: true),
            Reference reference => GenRemoveInternalMultipleReference(reference),
            _ => throw new ArgumentException($"unsupported link: {link}", nameof(link))
        };

        return IfStatement(
            GenEqualsIdentityLink(link),
            AsStatements(body)
        );
    }

    private List<StatementSyntax> GenRemoveInternalMultipleReference(Reference reference) =>
    [
        ExpressionStatement(
            InvocationExpression(
                IdentifierName(RemoveLink(reference)), AsArguments([AsNodesCall(reference)]))),
        ReturnTrue()
    ];

    private List<StatementSyntax> GenRemoveInternalMultipleContainment(Containment containment, bool writeable) =>
    [
        ExpressionStatement(
            InvocationExpression(
                IdentifierName(RemoveLink(containment)),
                AsArguments([AsNodesCall(containment, writeable)]))),
        ReturnTrue()
    ];
    
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
    private InvocationExpressionSyntax GenEqualsIdentityLink(Link link) =>
        InvocationExpression(MemberAccess(MetaProperty(link), IdentifierName("EqualsIdentity")),
            AsArguments([IdentifierName("link")])
        );
}