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
public partial class FeatureGenerator(Classifier classifier, Feature feature, INames names, LionWebVersions lionWebVersion, GeneratorConfig config)
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
            Containment { Multiple: true } c => AbstractLinkMembers(c, writeable: true),
            Reference { Multiple: true } r => AbstractLinkMembers(r, writeable: false),
            Containment { Optional: false } => AbstractSingleRequiredMembers(true),
            Reference { Optional: false } or Property { Optional: false } => AbstractSingleRequiredMembers(false),
            Containment { Optional: true } => AbstractSingleOptionalMembers(writeable: true),
            Reference { Optional: true } or Property { Optional: true } => AbstractSingleOptionalMembers(false),
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

    private IEnumerable<MemberDeclarationSyntax> AbstractSingleRequiredMembers(bool writeable) =>
    [
        AbstractSingleRequiredFeatureProperty(writeable: writeable),
        AbstractRequiredFeatureSetter(writeable: writeable).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
    ];

    private IEnumerable<MemberDeclarationSyntax> AbstractSingleOptionalMembers(bool writeable) =>
    [
        AbstractSingleOptionalFeatureProperty(),
        AbstractOptionalFeatureSetter(writeable: writeable).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
    ];

    private MethodDeclarationSyntax TryGet(ExpressionSyntax? storage = null, bool writeable = false) =>
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
                Assignment(FeatureTryGetParam(), storage ?? FeatureField(feature)),
                ReturnStatement(NotEquals(IdentifierName(FeatureTryGetParam()), Null()))
            ]));


    private MethodDeclarationSyntax TryGetMultiple(ExpressionSyntax? storage = null) =>
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
                Assignment(FeatureTryGetParam(), storage ?? FeatureField(feature)),
                ReturnStatement(
                    NotEquals(
                        MemberAccess(IdentifierName(FeatureTryGetParam()), IdentifierName("Count")),
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


    private IEnumerable<XmlNodeSyntax> XdocThrowsIfSetToNull() =>
        XdocThrows("If set to null", AsType(typeof(InvalidValueException)));


    private ExpressionStatementSyntax AsureNotNullCall() =>
        ExpressionStatement(Call("AssureNotNull",
            IdentifierName("value"),
            MetaProperty(feature)
        ));

    private static LocalDeclarationStatementSyntax SafeNodesVariable(ExpressionSyntax init) =>
        Variable("safeNodes", Var(), init);

    private static ConditionalAccessExpressionSyntax OptionalNodesToList() =>
        ConditionalAccessExpression(IdentifierName("nodes"),
            InvocationExpression(MemberBindingExpression(IdentifierName("ToList")))
        );

    private ExpressionStatementSyntax AttachChildCall() =>
        ExpressionStatement(Call("AttachChild", IdentifierName("value")));

    private ExpressionStatementSyntax AssignFeatureField(ExpressionSyntax? value = null) =>
        Assignment(FeatureField(feature).ToString(), value ?? IdentifierName("value"));

    private FieldDeclarationSyntax SingleFeatureField(bool writable = false) =>
        Field(FeatureField(feature).ToString(), NullableType(AsType(feature.GetFeatureType(), writeable: writable)),
                Null())
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    private PropertyDeclarationSyntax SingleRequiredFeatureProperty(ExpressionSyntax? storage = null, bool writeable = false) =>
        Property(FeatureProperty(feature).ToString(), AsType(feature.GetFeatureType(), writeable: writeable),
                BinaryExpression(
                    SyntaxKind.CoalesceExpression,
                    storage ?? FeatureField(feature),
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

    private PropertyDeclarationSyntax SingleOptionalFeatureProperty(ExpressionSyntax? storage = null, bool writeable = false) =>
        Property(FeatureProperty(feature).ToString(),
                NullableType(AsType(feature.GetFeatureType(), writeable: writeable)),
                storage ?? FeatureField(feature),
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

    private ExpressionSyntax FeatureSet() =>
        IdentifierName($"Set{feature.Name.ToFirstUpper()}");

    private string FeatureTryGetParam() =>
        _names.FeatureParam(feature);
}