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
/// Provides access to <see cref="IKeyedToAstCorrelation">correlations</see> between language elements and AST nodes
/// that have been recorded during <see cref="GeneratorFacade.Generate"/>.
///
/// <para/>
/// How to use correlations to postprocess generated classes:
/// <list type="number">
/// <item>
/// Set up <see cref="GeneratorFacade"/> with some <i>input language</i> and run <see cref="GeneratorFacade.Generate"/>.
/// Results in a <see cref="ICompilationUnitSyntax">compilation unit</see>.
/// </item>
/// <item>Retrieve <see cref="GeneratorFacade.Correlator"/>.</item>
/// <item>Filter the language elements of the <i>input language</i> according to your criteria.</item>
/// <item>For each language element, <see cref="FindAll">find</see> the correct <see cref="IKeyedToAstCorrelation">correlation</see>.</item>
/// <item>
/// For each correlation, <see cref="IKeyedToAstCorrelation.LookupIn">look up</see>
/// the <see cref="SyntaxNode">C# AST node</see> within the compilation unit.
/// </item>
/// <item>
/// <see cref="SyntaxNodeExtensions.ReplaceNode{T}(T, SyntaxNode, SyntaxNode)">Replace</see>
/// the looked up AST node with its modified version inside the compilation unit.
/// Remember to use the compilation unit <i>returned by Replace()</i> from now on (as SyntaxNodes are immutable). 
/// </item>
/// <item><see cref="GeneratorFacade.Persist(string, CompilationUnitSyntax)">Persist</see> the final compilation unit.</item>
/// </list>
///
/// <example>
/// Schematic of postprocessing:
/// <code>
/// Language inputLanguage;
/// var generator = new GeneratorFacade { ... };
///
/// var compilationUnit = generator.Generate();
/// var correlator = generator.Correlator;
///
/// var relevantLanguageElements = inputLanguage.Entities.Where( ... );
/// var correlations = relevantLanguageElements.SelectMany(e => correlator.FindAll(e));
///
/// foreach (var correlation in correlations)
/// {
///   var astNode = correlation.LookupIn(compilationUnit);
///   compilationUnit = compilationUnit.ReplaceNode(
///     astNode,
///     Modify(astNode)
///   );
/// }
///
/// generator.Persist(path, compilationUnit);
/// </code> 
/// </example>
/// </summary>
public class Correlator
{
    private readonly List<IKeyedToAstCorrelation> _correlations = [];

    /// <summary>
    /// All recorded <see cref="IKeyedToAstCorrelation">correlations</see>.
    /// </summary>
    public IReadOnlyList<IKeyedToAstCorrelation> Correlations => _correlations.AsReadOnly();

    /// <summary>
    /// Finds all <see cref="IKeyedToAstCorrelation">correlations</see> recorded for <paramref name="keyed"/>.
    /// </summary>
    public IEnumerable<IKeyedToAstCorrelation> FindAll(IKeyed keyed) =>
        _correlations.Where(c => c.Keyed == keyed);

    /// <summary>
    /// Finds all <see cref="IKeyedToAstCorrelation">correlations</see>
    /// of type <typeparamref name="T"/> recorded for <paramref name="keyed"/>.
    /// </summary>
    public IEnumerable<T> FindAll<T>(IKeyed keyed) where T : IKeyedToAstCorrelation =>
        _correlations.OfType<T>().Where(c => c.Keyed == keyed);

    /// <summary>
    /// Records <paramref name="correlation"/> and initializes it with <paramref name="syntaxNode"/>. 
    /// </summary>
    protected internal T Record<T>(IKeyedToAstCorrelation<T> correlation, T syntaxNode) where T : SyntaxNode
    {
        _correlations.Add(correlation);
        return correlation.Init(syntaxNode);
    }
}