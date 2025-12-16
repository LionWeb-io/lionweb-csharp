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

namespace LionWeb.Generator.GeneratorExtensions;

using Core.M3;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Interface for correlating LionWeb's <see cref="Classifier"/>s to their corresponding generated C# syntax nodes.
/// </summary>
public interface IKeyedToAstCorrelation
{
    IKeyed Keyed { get; }

    SyntaxNode ExtractFrom(SyntaxNode baseSyntax);
}

/// <inheritdoc />
public interface IKeyedToAstCorrelation<TAst> : IKeyedToAstCorrelation
    where TAst : SyntaxNode
{
    /// <remarks>
    /// We do <i>not</i> want to store the AST node, as it will be outdated very soon,
    /// and takes lots of memory (whole subtree).
    /// However, we want to pass it through <see cref="CorrelationManager.Record{T}"/>
    /// to simplify usage in the generator.
    /// Thus, we initialize outside the constructor. 
    /// </remarks>
    protected internal TAst Init(TAst ast);

    SyntaxNode IKeyedToAstCorrelation.ExtractFrom(SyntaxNode baseSyntax) =>
        ExtractFrom(baseSyntax);

    /// <inheritdoc cref="IKeyedToAstCorrelation.ExtractFrom"/>
    new TAst ExtractFrom(SyntaxNode baseSyntax);
}

/// <inheritdoc />
public interface IKeyedToAstCorrelation<out TKeyed, TAst> : IKeyedToAstCorrelation<TAst>
    where TKeyed : IKeyed
    where TAst : SyntaxNode
{
    IKeyed IKeyedToAstCorrelation.Keyed => Keyed;

    /// <inheritdoc cref="IKeyedToAstCorrelation.Keyed"/>
    new TKeyed Keyed { get; }
}

public abstract record CorrelationBase<TKeyed, TAst> : IKeyedToAstCorrelation<TKeyed, TAst>
    where TKeyed : IKeyed
    where TAst : SyntaxNode
{
    private readonly SyntaxAnnotation _syntaxAnnotation;
    private const string _syntaxAnnotationKind = nameof(IKeyedToAstCorrelation);

    /// <inheritdoc />
    public TKeyed Keyed { get; }

    public CorrelationBase(TKeyed keyed)
    {
        _syntaxAnnotation = new SyntaxAnnotation(_syntaxAnnotationKind);
        Keyed = keyed;
    }

    TAst IKeyedToAstCorrelation<TAst>.Init(TAst ast) =>
        ast.WithAdditionalAnnotations(_syntaxAnnotation);

    /// <inheritdoc />
    public TAst ExtractFrom(SyntaxNode baseSyntax) =>
        baseSyntax.DescendantNodesAndSelf().OfType<TAst>().First(n => n.HasAnnotation(_syntaxAnnotation));
}

public interface IClassifierToMainCorrelation : IKeyedToAstCorrelation
{
    Classifier Classifier { get; }

    /// <inheritdoc cref="IKeyedToAstCorrelation.ExtractFrom"/>
    new TypeDeclarationSyntax ExtractFrom(SyntaxNode baseSyntax);
}

/// <inheritdoc />
public interface IClassifierToMainCorrelation<out TKeyed, TAst> :
    IClassifierToMainCorrelation,
    IKeyedToAstCorrelation<TKeyed, TAst>
    where TKeyed : Classifier
    where TAst : TypeDeclarationSyntax
{
    Classifier IClassifierToMainCorrelation.Classifier =>
        Keyed;

    TypeDeclarationSyntax IClassifierToMainCorrelation.ExtractFrom(SyntaxNode baseSyntax) =>
        ((IKeyedToAstCorrelation<TKeyed, TAst>)this).ExtractFrom(baseSyntax);
}

/// <summary>
/// Record for holding <see cref="ClassDeclarationSyntax"/> of <see cref="Concept"/> and <see cref="Annotation"/> type of classifier.
/// </summary>
public record ClassifierToMainCorrelation(Classifier Classifier) :
    CorrelationBase<Classifier, ClassDeclarationSyntax>(Classifier),
    IClassifierToMainCorrelation<Classifier, ClassDeclarationSyntax>;

/// <summary>
/// Record for holding <see cref="InterfaceDeclarationSyntax"/> of <see cref="Interface"/> type of classifier.
/// </summary>
public record InterfaceToMainCorrelation(Interface Keyed) :
    CorrelationBase<Interface, InterfaceDeclarationSyntax>(Keyed),
    IClassifierToMainCorrelation<Interface, InterfaceDeclarationSyntax>;