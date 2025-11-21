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

namespace LionWeb.Core.M3;

using System.Diagnostics.CodeAnalysis;

/// A primitive value with finite, pre-defined, known set of possible values.
public interface Enumeration : Datatype
{
    /// All possible values of this enumeration.
    public IReadOnlyList<EnumerationLiteral> Literals { get; }

    /// <summary>
    /// Gets the <see cref="Literals"/>.
    /// </summary>
    /// <param name="literals">Value of <see cref="Literals"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Literals"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetLiterals([NotNullWhen(true)] out IReadOnlyList<EnumerationLiteral>? literals)
    {
        literals = Literals;
        return literals is { Count: > 0 };
    }
}