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
/// Correlates LionWeb's <see cref="IKeyed">language elements</see>
/// to their corresponding generated <see cref="SyntaxNode">C# syntax nodes</see>.
/// </summary>
public interface IKeyedToAstCorrelation
{
    /// <summary>
    /// The correlated LionWeb <see cref="IKeyed">language element</see>.
    /// </summary>
    IKeyed Keyed { get; }

    /// <summary>
    /// Looks up the corresponding generated <see cref="SyntaxNode">C# syntax node</see> in <paramref name="baseSyntax"/>.
    /// </summary>
    /// <param name="baseSyntax"></param>
    /// <returns></returns>
    SyntaxNode LookupIn(SyntaxNode baseSyntax);
}

/// <inheritdoc />
/// <typeparam name="TAst">Type of the correlated <see cref="SyntaxNode">AST node</see>.</typeparam>
public interface IKeyedToAstCorrelation<TAst> : IKeyedToAstCorrelation
    where TAst : SyntaxNode
{
    /// <remarks>
    /// We do <i>not</i> want to store the AST node, as it will be outdated very soon,
    /// and takes lots of memory (whole subtree).
    /// However, we want to pass it through <see cref="Correlator.Record{T}"/>
    /// to simplify usage in the generator.
    /// Thus, we initialize outside the constructor. 
    /// </remarks>
    protected internal TAst Init(TAst ast);

    SyntaxNode IKeyedToAstCorrelation.LookupIn(SyntaxNode baseSyntax) =>
        LookupIn(baseSyntax);

    /// <inheritdoc cref="IKeyedToAstCorrelation.LookupIn"/>
    new TAst LookupIn(SyntaxNode baseSyntax);
}

/// <inheritdoc />
/// <typeparam name="TKeyed">Type of the correlated <see cref="IKeyed">language element</see>.</typeparam>
public interface IKeyedToAstCorrelation<out TKeyed, TAst> : IKeyedToAstCorrelation<TAst>
    where TKeyed : IKeyed
    where TAst : SyntaxNode
{
    IKeyed IKeyedToAstCorrelation.Keyed => Keyed;

    /// <inheritdoc cref="IKeyedToAstCorrelation.Keyed"/>
    new TKeyed Keyed { get; }
}

/// <summary>
/// Base implementation for <see cref="IKeyedToAstCorrelation{TKeyed, TAst}"/>
/// </summary>
public abstract record CorrelationBase<TKeyed, TAst> : IKeyedToAstCorrelation<TKeyed, TAst>
    where TKeyed : IKeyed
    where TAst : SyntaxNode
{
    private readonly SyntaxAnnotation _syntaxAnnotation;
    private const string _syntaxAnnotationKind = nameof(IKeyedToAstCorrelation);

    /// <inheritdoc />
    public TKeyed Keyed { get; }

    /// <summary>
    /// Stores <paramref name="keyed"/> and initializes the <see cref="_syntaxAnnotation"/>
    /// to find the correlated <see cref="TAst">AST node</see>.
    /// </summary>
    protected CorrelationBase(TKeyed keyed)
    {
        _syntaxAnnotation = new SyntaxAnnotation(_syntaxAnnotationKind);
        Keyed = keyed;
    }

    TAst IKeyedToAstCorrelation<TAst>.Init(TAst ast) =>
        ast.WithAdditionalAnnotations(_syntaxAnnotation);

    /// <inheritdoc />
    public TAst LookupIn(SyntaxNode baseSyntax) =>
        baseSyntax.DescendantNodesAndSelf().OfType<TAst>().First(n => n.HasAnnotation(_syntaxAnnotation));
}

/// <summary>
/// Correlates a LionWeb <see cref="Classifier"/> to the generated C# <see cref="TypeDeclarationSyntax">main class</see>.
/// </summary>
public interface IClassifierToMainCorrelation : IKeyedToAstCorrelation
{
    /// <inheritdoc cref="IKeyedToAstCorrelation.Keyed"/>
    Classifier Classifier { get; }

    /// <inheritdoc cref="IKeyedToAstCorrelation.LookupIn"/>
    new TypeDeclarationSyntax LookupIn(SyntaxNode baseSyntax);
}

/// <inheritdoc cref="IClassifierToMainCorrelation" />
public interface IClassifierToMainCorrelation<out TKeyed, TAst> :
    IClassifierToMainCorrelation,
    IKeyedToAstCorrelation<TKeyed, TAst>
    where TKeyed : Classifier
    where TAst : TypeDeclarationSyntax
{
    Classifier IClassifierToMainCorrelation.Classifier =>
        Keyed;

    TypeDeclarationSyntax IClassifierToMainCorrelation.LookupIn(SyntaxNode baseSyntax) =>
        ((IKeyedToAstCorrelation<TKeyed, TAst>)this).LookupIn(baseSyntax);
}

/// <summary>
/// Correlates a LionWeb <see cref="Concept"/> or <see cref="Annotation"/> to the generated C# <see cref="ClassDeclarationSyntax">main class</see>.
/// </summary>
public record ClassifierToMainCorrelation(Classifier Classifier) :
    CorrelationBase<Classifier, ClassDeclarationSyntax>(Classifier),
    IClassifierToMainCorrelation<Classifier, ClassDeclarationSyntax>;

/// <summary>
/// Correlates a LionWeb <see cref="Interface"/> to the generated C# <see cref="InterfaceDeclarationSyntax"/>.
/// </summary>
public record InterfaceToMainCorrelation(Interface Keyed) :
    CorrelationBase<Interface, InterfaceDeclarationSyntax>(Keyed),
    IClassifierToMainCorrelation<Interface, InterfaceDeclarationSyntax>;