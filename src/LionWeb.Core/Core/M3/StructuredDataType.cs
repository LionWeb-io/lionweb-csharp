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

/// Represents a collection of named instances of Data Types.
/// Meant to support a small composite of values that semantically form a unit. 
public interface StructuredDataType : Datatype
{
    /// All fields of this structured datatype. 
    public IReadOnlyList<Field> Fields { get; }

    /// <summary>
    /// Gets the <see cref="Fields"/>.
    /// </summary>
    /// <param name="fields">Value of <see cref="Fields"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Fields"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetFields([NotNullWhen(true)] out IReadOnlyList<Field>? fields)
    {
        fields = Fields;
        return fields is { Count: > 0 };
    }
}