// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.M2;

using M3;

/// <summary>
/// The definition of the LionCore language containing the built-ins: primitive types, <see cref="INamed"/> and <see cref="Node"/>.
/// </summary>
public interface IBuiltInsLanguage : Language
{
    /// <summary>
    /// The definition of the LionCore built-ins language.
    /// </summary>
    public static IBuiltInsLanguage GetInstance(LionWebVersions version) => version.BuiltIns;

    /// <summary>
    /// The built-in primitive type Boolean.
    /// </summary>
    PrimitiveType Boolean { get; }

    /// <summary>
    /// The built-in primitive type Integer.
    /// </summary>
    PrimitiveType Integer { get; }

    /// <summary>
    /// The built-in primitive type Json.
    /// </summary>
    PrimitiveType Json { get; }

    /// <summary>
    /// The built-in primitive type String.
    /// </summary>
    PrimitiveType String { get; }

    /// <summary>
    /// Generic concept for "named things".
    /// </summary>
    Interface INamed { get; }

    /// <summary>
    /// The "name" <see cref="Property"/> of <c>INamed</c>.
    /// </summary>
    Property INamed_name { get; }

    /// <summary>
    /// Generic concept that corresponds to a model node, i.e. <see cref="INode"/>.
    /// </summary>
    Concept Node { get; }
}

/// <inheritdoc cref="IBuiltInsLanguage"/>
[Obsolete("Use IBuiltInsLanguage instead")]
public sealed class BuiltInsLanguage
{
    /// <inheritdoc cref="IBuiltInsLanguage.GetInstance"/>
    [Obsolete("Use IBuiltInsLanguage instead")]
    public static readonly IBuiltInsLanguage Instance = LionWebVersions.Current.BuiltIns;
}

/// <summary>
/// An interface for "named things".
/// It corresponds nominally to the <c>INamed</c> Concept defined above.
/// </summary>
public interface INamed : IReadableNode
{
    /// <summary>
    /// The human-readable name.
    /// </summary>
    public string Name { get; }
}

/// Writable variant of <see cref="INamed"/>.
public interface INamedWritable : INamed, IWritableNode
{
    /// <inheritdoc cref="INamed.Name"/>
    public new string Name { get; set; }

    /// <inheritdoc cref="INamed.Name"/>
    public INamedWritable SetName(string value);
}