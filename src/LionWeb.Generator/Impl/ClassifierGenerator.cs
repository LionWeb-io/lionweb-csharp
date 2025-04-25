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
using Core.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Names;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;

/// <summary>
/// Generates concept and annotation classes, and interface interfaces.
/// Covers members:
/// - Constructor()
/// - GetClassifier()
/// </summary>
/// <seealso cref="FeatureMethodsGenerator"/>
/// <seealso cref="ContainmentMethodsGenerator"/>
/// <seealso cref="FeatureGenerator"/>
public class ClassifierGenerator(
    Classifier classifier,
    INames names,
    LionWebVersions lionWebVersion,
    GeneratorConfig config)
    : ClassifierGeneratorBase(new UniqueFeatureNames(names), lionWebVersion, config)
{
    /// <inheritdoc cref="ClassifierGenerator"/>
    public TypeDeclarationSyntax ClassifierType() =>
        classifier switch
        {
            Annotation a => ClassifierAnnotation(a),
            Concept c => ClassifierConcept(c),
            Interface i => ClassifierInterface(i),
            _ => throw new ArgumentException($"Unsupported classifier: {classifier}", nameof(classifier))
        };

    private ClassDeclarationSyntax ClassifierAnnotation(Annotation annotation)
    {
        List<TypeSyntax> bases = [];
        if (annotation.Extends is not null && !annotation.Extends.EqualsIdentity(_builtIns.Node))
        {
            bases.Add(AsType(annotation.Extends));
        } else
        {
            bases.Add(AsType(typeof(AnnotationInstanceBase)));
        }

        bases.AddRange(Interfaces.Select(i => AsType(i, writeable: true)));
        bases.AddRange(AddINamedWritableInterface(annotation.Extends));

        return ClassifierClass(bases, GenGetClassifier("GetAnnotation", typeof(Annotation)));
    }

    private ClassDeclarationSyntax ClassifierConcept(Concept concept)
    {
        List<TypeSyntax> bases = [];

        if (concept.Extends is not null && !concept.Extends.EqualsIdentity(_builtIns.Node))
        {
            bases.Add(AsType(concept.Extends));
        } else
        {
            bases.Add(AsType(typeof(ConceptInstanceBase)));
        }

        bases.AddRange(Interfaces.Select(i => AsType(i, writeable: true)));
        bases.AddRange(AddINamedWritableInterface(concept.Extends));

        if (concept.Partition)
            bases.Add(AsType(typeof(IPartitionInstance<INode>)));

        return ClassifierClass(bases, GenGetClassifier("GetConcept", typeof(Concept)));
    }

    private IEnumerable<TypeSyntax> AddINamedWritableInterface(Classifier? extends)
    {
        if (
            // We are an INamed
            classifier.AllGeneralizations().Contains(_builtIns.INamed) &&
            // INamedWritable gets added directly
            !Interfaces.Contains(_builtIns.INamed) &&
            (
                extends == null ||
                // INamedWritable gets added to supertype
                !extends.AllGeneralizations().Contains(_builtIns.INamed)
            )
        )
            return [AsType(typeof(INamedWritable))];

        return [];
    }

    private ClassDeclarationSyntax ClassifierClass(List<TypeSyntax> bases, MethodDeclarationSyntax genGetClassifier)
    {
        List<SyntaxKind> modifiers = [SyntaxKind.PublicKeyword];
        if (classifier is Concept { Abstract: true })
            modifiers.Add(SyntaxKind.AbstractKeyword);
        modifiers.Add(SyntaxKind.PartialKeyword);

        return ClassDeclaration(ClassifierName)
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(classifier),
                ObsoleteAttribute(classifier)
            ]))
            .WithModifiers(AsModifiers(modifiers.ToArray()))
            .WithBaseList(AsBase(bases.ToArray()))
            .WithMembers(List(
                FeaturesToImplement(classifier)
                    .SelectMany(f => new FeatureGenerator(classifier, f, _names, _lionWebVersion, _config).Members())
                    .Concat(new List<MemberDeclarationSyntax> { GenConstructor(), genGetClassifier })
                    .Concat(new FeatureMethodsGenerator(classifier, _names, _lionWebVersion, _config).FeatureMethods())
                    .Concat(new ContainmentMethodsGenerator(classifier, _names, _lionWebVersion, _config)
                        .ContainmentMethods())
            ))
            .Xdoc(XdocDefault());
    }

    private IEnumerable<XmlNodeSyntax> XdocDefault()
    {
        List<XmlNodeSyntax> result = [];

        var conceptShortDescription = VersionSpecifics.GetConceptShortDescription(classifier);
        if (conceptShortDescription != null)
            result.AddRange(XdocLine(conceptShortDescription, "summary"));

        var conceptHelpUrl = VersionSpecifics.GetConceptHelpUrl(classifier);
        if (conceptHelpUrl != null)
            result.AddRange(XdocSeeAlso(conceptHelpUrl));

        result.AddRange(XdocKeyed(classifier));

        return result;
    }

    private IEnumerable<Interface> Interfaces =>
        classifier
            .DirectGeneralizations()
            .OfType<Interface>()
            .Ordered();


    private ConstructorDeclarationSyntax GenConstructor() =>
        Constructor(ClassifierName, Param("id", AsType(typeof(string))))
            .WithInitializer(Initializer("id"))
            .WithBody(AsStatements([]));

    private MethodDeclarationSyntax GenGetClassifier(string methodName, Type returnType) =>
        Method(methodName, AsType(returnType), exprBody: MetaProperty())
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc());

    private MemberAccessExpressionSyntax MetaProperty() =>
        MemberAccess(MemberAccess(LanguageType, IdentifierName("Instance")), _names.AsProperty(classifier));

    private InterfaceDeclarationSyntax ClassifierInterface(Interface iface)
    {
        var bases = iface.Extends.Ordered().Select(e => AsType(e, false)).ToList();
        if (bases.Count == 0)
            bases = [AsType(typeof(INode))];

        return InterfaceDeclaration(ClassifierName)
            .WithAttributeLists(AsAttributes(
            [
                MetaPointerAttribute(classifier),
                ObsoleteAttribute(classifier)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword))
            .WithBaseList(AsBase(bases.ToArray()))
            .WithMembers(List(iface.Features.Ordered().SelectMany(f =>
                new FeatureGenerator(classifier, f, _names, _lionWebVersion, _config).AbstractMembers())))
            .Xdoc(XdocDefault());
    }

    private string ClassifierName =>
        _names.AsType(classifier).ToString();
}