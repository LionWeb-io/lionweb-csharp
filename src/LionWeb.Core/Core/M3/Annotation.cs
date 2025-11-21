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

/// An Annotation is an additional piece of information attached to potentially any node, sharing the node’s lifecycle.
public interface Annotation : Classifier
{
    /// An annotation can only be attached to a specific <see cref="Classifier"/> (and all its specializations, aka subtypes).
    public Classifier Annotates { get; }

    /// <summary>
    /// Gets the <see cref="Annotates"/>.
    /// </summary>
    /// <param name="annotates">Value of <see cref="Annotates"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Annotates"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetAnnotates([NotNullWhen(true)] out Classifier? annotates)
    {
        annotates = Annotates;
        return annotates != null;
    }

    /// An annotation can extend zero or one other annotations, the same way a C# class can extend another class.
    public Annotation? Extends { get; }

    /// <summary>
    /// Gets the <see cref="Extends"/>.
    /// </summary>
    /// <param name="extends">Value of <see cref="Extends"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Extends"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetExtends([NotNullWhen(true)] out Annotation? extends)
    {
        extends = Extends;
        return extends != null;
    }

    /// An annotation can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# class can implement C# interfaces.
    public IReadOnlyList<Interface> Implements { get; }

    /// <summary>
    /// Gets the <see cref="Implements"/>.
    /// </summary>
    /// <param name="implements">Value of <see cref="Implements"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Implements"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetImplements([NotNullWhen(true)] out IReadOnlyList<Interface>? implements)
    {
        implements = Implements;
        return implements is { Count: > 0 };
    }
}