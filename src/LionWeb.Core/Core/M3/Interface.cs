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

/// An Interface represents a category of entities sharing some similar characteristics.
public interface Interface : Classifier
{
    /// A LionWeb interface can extend zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# interface can extend other C# interfaces.
    public IReadOnlyList<Interface> Extends { get; }

    /// <summary>
    /// Gets the <see cref="Extends"/>.
    /// </summary>
    /// <param name="extends">Value of <see cref="Extends"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Extends"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetExtends([NotNullWhen(true)] out IReadOnlyList<Interface>? extends)
    {
        extends = Extends;
        return extends is { Count: > 0 };
    }
}