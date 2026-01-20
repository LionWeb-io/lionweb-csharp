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
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Correlates a LionWeb <see cref="IKeyed">language element</see>
/// to the generated C# Language constructor <see cref="AssignmentExpressionSyntax">field initialization</see>
/// </summary>
/// 
/// <example>
/// LionWeb classifier <i>MyClassifier</i> in Language <i>MyLanguage</i> ->
/// <code>
/// public partial class MyLanguageLanguage : LanguageBase&lt;IMyLanguageFactory&gt;
/// {
///   public MyLanguageLanguage(string id) : base(id, LionWebVersions.v2024_1)
///   {
///     _myClassifier = new(() => new ConceptBase&lt;MyLanguageLanguage&gt;(...) { ... });
///     ...
///   }
///
///   ...
/// }
/// </code>
/// </example>
public record KeyedToLanguageConstructorCorrelation : CorrelationBase<IKeyed, AssignmentExpressionSyntax>
{
    /// <inheritdoc cref="KeyedToLanguageConstructorCorrelation"/>
    public KeyedToLanguageConstructorCorrelation(IKeyed keyed) : base(keyed)
    {
    }
}