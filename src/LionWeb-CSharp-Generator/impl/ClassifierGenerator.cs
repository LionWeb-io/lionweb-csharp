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

namespace LionWeb.CSharp.Generator.Impl;

using Core;
using Core.M2;
using Core.M3;
using Io.Lionweb.Mps.Specific;
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
public class ClassifierGenerator(Classifier classifier, INames names)
    : ClassifierGeneratorBase(new UniqueFeatureNames(names))
{
    /// <inheritdoc cref="ClassifierGenerator"/>
    public TypeDeclarationSyntax ClassifierType() =>
        classifier switch
        {
            Concept or Annotation => ClassifierClass(),
            Interface i => ClassifierInterface(i),
            _ => throw new ArgumentException($"Unsupported classifier: {classifier}", nameof(classifier))
        };

    private ClassDeclarationSyntax ClassifierClass()
    {
        List<TypeSyntax> bases = [GetSuperclass()];
        bases.AddRange(Interfaces.Select(i => AsType(i)));

        List<SyntaxKind> modifiers = [SyntaxKind.PublicKeyword];
        if (classifier is Concept { Abstract: true })
            modifiers.Add(SyntaxKind.AbstractKeyword);
        modifiers.Add(SyntaxKind.PartialKeyword);

        var decl = ClassDeclaration(ClassifierName)
            .WithAttributeLists(AsAttributes([
                MetaPointerAttribute(classifier),
                ObsoleteAttribute(classifier)
            ]))
            .WithModifiers(AsModifiers(modifiers.ToArray()))
            .WithBaseList(AsBase(bases.ToArray()))
            .WithMembers(List(
                FeaturesToImplement(classifier).SelectMany(f => new FeatureGenerator(classifier, f, _names).Members())
                    .Concat(new List<MemberDeclarationSyntax> { GenConstructor(), GenGetClassifier() })
                    .Concat(new FeatureMethodsGenerator(classifier, _names).FeatureMethods())
                    .Concat(new ContainmentMethodsGenerator(classifier, _names).ContainmentMethods())
            ));

        return AttachConceptDescription(decl);
    }

    private T AttachConceptDescription<T>(T decl) where T : TypeDeclarationSyntax
    {
        var conceptDescriptions = classifier.GetAnnotations().OfType<ConceptDescription>()
            .Where(cd => cd.ConceptShortDescription != null);
        if (conceptDescriptions.Any())
        {
            return decl.Xdoc(XdocLine(conceptDescriptions.First().ConceptShortDescription, "summary"));
        }

        return decl;
    }

    private TypeSyntax GetSuperclass() =>
        classifier switch
        {
            Annotation { Extends: not null } a => AsType(a.Extends),
            Concept { Extends: not null } c => AsType(c.Extends),
            _ => null
        } ?? AsType(typeof(NodeBase));

    private IEnumerable<Interface> Interfaces =>
        classifier
            .DirectGeneralizations()
            .OfType<Interface>()
            .Ordered();


    private ConstructorDeclarationSyntax GenConstructor() =>
        Constructor(ClassifierName, Param("id", AsType(typeof(string))))
            .WithInitializer(Initializer("id"))
            .WithBody(AsStatements([]));

    private MethodDeclarationSyntax GenGetClassifier() =>
        Method("GetClassifier", AsType(typeof(Classifier)), exprBody: MetaProperty())
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword))
            .Xdoc(XdocInheritDoc());

    private MemberAccessExpressionSyntax MetaProperty() =>
        MemberAccess(MemberAccess(LanguageType, IdentifierName("Instance")), _names.AsProperty(classifier));

    private InterfaceDeclarationSyntax ClassifierInterface(Interface iface)
    {
        var bases = iface.Extends.Ordered().Select(e => AsType(e, false)).ToList();
        if (bases.Count == 0)
            bases = [AsType(typeof(INode))];

        var decl = InterfaceDeclaration(ClassifierName)
            .WithAttributeLists(AsAttributes(
            [
                MetaPointerAttribute(classifier),
                ObsoleteAttribute(classifier)
            ]))
            .WithModifiers(AsModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword))
            .WithBaseList(AsBase(bases.ToArray()))
            .WithMembers(List(iface.Features.Ordered().SelectMany(f =>
                new FeatureGenerator(classifier, f, _names).AbstractMembers())));
        return AttachConceptDescription(decl);
    }

    private string ClassifierName =>
        _names.AsType(classifier).ToString();
}