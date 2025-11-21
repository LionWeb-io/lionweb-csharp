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

/// A Concept represents a category of entities sharing the same structure.
public interface Concept : Classifier
{
    /// An <i>abstract</i> concept cannot be instantiated.
    /// A non-abstract, i.e. <i>concrete</i> concept can be instantiated.
    public bool Abstract { get; }

    /// <summary>
    /// Gets the <see cref="Abstract"/>.
    /// </summary>
    /// <param name="abstract">Value of <see cref="Abstract"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Abstract"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetAbstract([NotNullWhen(true)] out bool? @abstract)
    {
        @abstract = Abstract;
        return @abstract != null;
    }

    /// A <i>partition</i> concept MUST NOT have a <see cref="IReadableNode.GetParent">parent</see>.
    /// It is the root of a node tree.
    /// A non-partition, i.e. <i>regular</i> concept MUST have a <see cref="IReadableNode.GetParent">parent</see>.
    public bool Partition { get; }

    /// <summary>
    /// Gets the <see cref="Partition"/>.
    /// </summary>
    /// <param name="partition">Value of <see cref="Partition"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Partition"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetPartition([NotNullWhen(true)] out bool? partition)
    {
        partition = Partition;
        return partition != null;
    }

    /// A concept can extend zero or one other concepts, the same way a C# class can extend another class.
    public Concept? Extends { get; }

    /// <summary>
    /// Gets the <see cref="Extends"/>.
    /// </summary>
    /// <param name="extends">Value of <see cref="Extends"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Extends"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetExtends([NotNullWhen(true)] out Concept? extends)
    {
        extends = Extends;
        return extends != null;
    }

    /// A concept can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
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