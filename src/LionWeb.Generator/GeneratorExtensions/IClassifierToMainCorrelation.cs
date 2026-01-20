// Copyright 2026 TRUMPF Laser SE and other contributors
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
/// Correlates a LionWeb <see cref="Classifier"/> to the generated C# <see cref="TypeDeclarationSyntax">main class</see>.
/// </summary>
///
/// <example>
/// LionWeb classifier <i>MyClassifier</i> ->
/// <code>
/// public partial class MyClassifier : ConceptInstanceBase
/// {
///   ...
/// }
/// </code>
/// </example>
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