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

public class CorrelationManager
{
    private readonly List<IKeyedToAstCorrelation> _correlations = [];

    public IReadOnlyList<IKeyedToAstCorrelation> Correlations => _correlations.AsReadOnly();

    public IEnumerable<IKeyedToAstCorrelation> FindAll(IKeyed keyed) =>
        _correlations.Where(c => c.Keyed == keyed);

    /// <inheritdoc cref="FindAll"/>
    public IEnumerable<T> FindAll<T>(IKeyed keyed) where T : IKeyedToAstCorrelation =>
        _correlations.OfType<T>().Where(c => c.Keyed == keyed);

    protected internal T Record<T>(IKeyedToAstCorrelation<T> correlation, T syntaxNode) where T : SyntaxNode
    {
        _correlations.Add(correlation);
        return correlation.Init(syntaxNode);
    }
}