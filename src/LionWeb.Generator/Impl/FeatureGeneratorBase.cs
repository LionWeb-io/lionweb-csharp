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
internal abstract class FeatureGeneratorBase(Classifier classifier, Feature feature, INames names, LionWebVersions lionWebVersion, GeneratorConfig config)
    : ClassifierGeneratorBase(names, lionWebVersion, config)
{
    /// <inheritdoc cref="FeatureGeneratorBase"/>
    public static IEnumerable<MemberDeclarationSyntax> Members(Classifier classifier, Feature feature, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) =>
        feature switch
        {
            Property { Optional: false } p => new FeatureGeneratorProperty(classifier, p, names, lionWebVersion, config).RequiredProperty(),
            Property { Optional: true } p => new FeatureGeneratorProperty(classifier, p, names, lionWebVersion, config).OptionalProperty(),
            Containment { Optional: false, Multiple: false } c => new FeatureGeneratorContainment(classifier, c, names, lionWebVersion, config).RequiredSingleContainment(),
            Containment { Optional: true, Multiple: false } c => new FeatureGeneratorContainment(classifier, c, names, lionWebVersion, config).OptionalSingleContainment(),
            Containment { Optional: false, Multiple: true } c => new FeatureGeneratorContainment(classifier, c, names, lionWebVersion, config).RequiredMultiContainment(),
            Containment { Optional: true, Multiple: true } c => new FeatureGeneratorContainment(classifier, c, names, lionWebVersion, config).OptionalMultiContainment(),
            Reference { Optional: false, Multiple: false } r => new FeatureGeneratorReference(classifier, r, names, lionWebVersion, config).RequiredSingleReference(),
            Reference { Optional: true, Multiple: false } r => new FeatureGeneratorReference(classifier, r, names, lionWebVersion, config).OptionalSingleReference(),
            Reference { Optional: false, Multiple: true } r => new FeatureGeneratorReference(classifier, r, names, lionWebVersion, config).RequiredMultiReference(),
            Reference { Optional: true, Multiple: true } r => new FeatureGeneratorReference(classifier, r, names, lionWebVersion, config).OptionalMultiReference(),
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

    /// <inheritdoc cref="FeatureGeneratorBase"/>
    public static IEnumerable<MemberDeclarationSyntax> AbstractMembers(Classifier classifier, Feature feature, INames names, LionWebVersions lionWebVersion, GeneratorConfig config) =>
        feature switch
        {
            Property { Optional: false } p => new FeatureGeneratorProperty(classifier, p, names, lionWebVersion, config).AbstractSingleRequiredMembers(false),
            Property { Optional: true } p => new FeatureGeneratorProperty(classifier, p, names, lionWebVersion, config).AbstractSingleOptionalMembers(false),
            Containment { Multiple: true } c => new FeatureGeneratorContainment(classifier, c, names, lionWebVersion, config).AbstractLinkMembers(writeable: true),
            Containment { Optional: false } c => new FeatureGeneratorContainment(classifier, c, names, lionWebVersion, config).AbstractSingleRequiredMembers(true),
            Containment { Optional: true } c => new FeatureGeneratorContainment(classifier, c, names, lionWebVersion, config).AbstractSingleOptionalMembers(writeable: true),
            Reference { Multiple: true } r => new FeatureGeneratorReference(classifier, r, names, lionWebVersion, config).AbstractLinkMembers(writeable: false),
            Reference { Optional: false } r => new FeatureGeneratorReference(classifier, r, names, lionWebVersion, config).AbstractSingleRequiredMembers(false),
            Reference { Optional: true } r => new FeatureGeneratorReference(classifier, r, names, lionWebVersion, config).AbstractSingleOptionalMembers(false),
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };

    public IEnumerable<MemberDeclarationSyntax> AbstractSingleRequiredMembers(bool writeable) =>
    [
        AbstractSingleRequiredFeatureProperty(writeable: writeable),
        AbstractRequiredFeatureSetter(writeable: writeable).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
    ];

    public IEnumerable<MemberDeclarationSyntax> AbstractSingleOptionalMembers(bool writeable) =>
    [
        AbstractSingleOptionalFeatureProperty(),
        AbstractOptionalFeatureSetter(writeable: writeable).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
    ];

    protected MethodDeclarationSyntax TryGet(ExpressionSyntax? storage = null, ExpressionSyntax? result = null,
        bool writeable = false) =>
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
                ReturnStatement(result ?? NotEquals(IdentifierName(FeatureTryGetParam()), Null()))
            ]));

    protected MethodDeclarationSyntax TryGetMultiple(ExpressionSyntax? storage = null, TypeSyntax? outType = null) =>
        AbstractTryGetMultiple(outType)
            .WithBody(AsStatements([
                Assignment(FeatureTryGetParam(), storage ?? FeatureField(feature)),
                ReturnStatement(
                    NotEquals(
                        MemberAccess(IdentifierName(FeatureTryGetParam()), IdentifierName("Count")),
                        0.AsLiteral()
                    )
                )
            ]));

    protected MethodDeclarationSyntax AbstractTryGetMultiple(TypeSyntax? outType) =>
        Method(FeatureTryGet(feature), AsType(typeof(bool)),
                [
                    OutParam(Param(
                            FeatureTryGetParam(),
                            AsType(typeof(IReadOnlyList<>), outType ?? AsType(feature.GetFeatureType()))
                        ))
                        .WithModifiers(AsModifiers(SyntaxKind.OutKeyword))
                ]
            )
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword))
            .WithAttributeLists(AsAttributes([ObsoleteAttribute(feature)]))
            .Xdoc(XdocDefault(feature));

    private ParameterSyntax OutParam(ParameterSyntax parameterSyntax) =>
        parameterSyntax
            .WithAttributeLists(AsAttributes([
                Attribute(IdentifierName(AsType(typeof(NotNullWhenAttribute)).ToString()))
                    .WithArgumentList(
                        AttributeArgumentList(SingletonSeparatedList(AttributeArgument(True())))
                    )
            ]));


    protected IEnumerable<XmlNodeSyntax> XdocThrowsIfSetToNull() =>
        XdocThrows("If set to null", AsType(typeof(InvalidValueException)));


    protected ExpressionStatementSyntax AsureNotNullCall() =>
        ExpressionStatement(Call("AssureNotNull",
            IdentifierName("value"),
            MetaProperty(feature)
        ));

    protected static LocalDeclarationStatementSyntax SafeNodesVariable(ExpressionSyntax init) =>
        Variable("safeNodes", Var(), init);

    protected static ConditionalAccessExpressionSyntax OptionalNodesToList() =>
        ConditionalAccessExpression(IdentifierName("nodes"),
            InvocationExpression(MemberBindingExpression(IdentifierName("ToList")))
        );

    protected ExpressionStatementSyntax AttachChildCall() =>
        ExpressionStatement(Call("AttachChild", IdentifierName("value")));

    protected ExpressionStatementSyntax AssignFeatureField(ExpressionSyntax? value = null) =>
        Assignment(FeatureField(feature).ToString(), value ?? IdentifierName("value"));

    protected FieldDeclarationSyntax SingleFeatureField(bool writable = false) =>
        Field(FeatureField(feature).ToString(), NullableType(AsType(feature.GetFeatureType(), writeable: writable)),
                Null())
            .WithModifiers(AsModifiers(SyntaxKind.PrivateKeyword));

    protected PropertyDeclarationSyntax SingleRequiredFeatureProperty(TypeSyntax returnType,
        ExpressionSyntax? getter = null, ExpressionSyntax? setter = null) =>
        Property(FeatureProperty(feature).ToString(), returnType,
                getter ?? NotNullOrThrow(
                    FeatureField(feature),
                    NewCall([MetaProperty(feature)], AsType(typeof(UnsetFeatureException)))
                ),
                setter ?? InvocationExpression(FeatureSet(), AsArguments([IdentifierName("value")]))
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

    protected AttributeSyntax FeatureAttribute() =>
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

    protected PropertyDeclarationSyntax SingleOptionalFeatureProperty(ExpressionSyntax? getter = null, bool writeable = false) =>
        Property(FeatureProperty(feature).ToString(),
                NullableType(AsType(feature.GetFeatureType(), writeable: writeable)),
                getter ?? FeatureField(feature),
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

    protected List<MethodDeclarationSyntax> RequiredFeatureSetter(List<StatementSyntax> body, bool writeable = false)
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

    protected IEnumerable<MethodDeclarationSyntax> OptionalFeatureSetter(List<StatementSyntax> body,
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

    protected IEnumerable<XmlNodeSyntax> XdocDefault(Feature ffeature) =>
        XdocKeyed(ffeature)
            .Concat(XdocRemarks(ffeature));

    private IEnumerable<XmlNodeSyntax> XdocRemarks(Feature feature)
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

    protected IEnumerable<XmlNodeSyntax> XdocThrows(string text, TypeSyntax exception) =>
    [
        XdocSlashes(),
        XmlElement("exception", new SyntaxList<XmlNodeSyntax>(XdocText(text)))
            .AddStartTagAttributes(XmlCrefAttribute(NameMemberCref(exception))),
        XdocNewline()
    ];

    protected bool InheritedFromInterface() =>
        !classifier.EqualsIdentity(feature.GetFeatureClassifier());

    protected MethodDeclarationSyntax InterfaceDelegator(MethodDeclarationSyntax impl,
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

    protected ExpressionSyntax FeatureSet() =>
        IdentifierName($"Set{feature.Name.ToFirstUpper()}");

    protected string FeatureTryGetParam() =>
        _names.FeatureParam(feature);
}