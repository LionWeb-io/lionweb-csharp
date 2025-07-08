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
using System.Diagnostics.CodeAnalysis;

/// The definition of the LionCore language containing the built-ins:
/// primitive types, <see cref="INamed"/> and <see cref="Node"/>.
public interface IBuiltInsLanguage : Language
{
    /// Key of all LionWeb BuiltIns language implementations.
    public const MetaPointerKey LanguageKey = "LionCore-builtins";

    /// Name of all LionWeb BuiltIns language implementations.
    protected const string LanguageName = "LionCore_builtins";

    internal const string ResolveInfoPrefix = "LionWeb.LionCore_builtins.";

    /// The built-in primitive type Boolean.
    PrimitiveType Boolean { get; }

    /// The built-in primitive type Integer.
    PrimitiveType Integer { get; }

    /// The built-in primitive type String.
    PrimitiveType String { get; }

    /// Generic concept for "named things".
    Interface INamed { get; }

    /// The "name" <see cref="Property"/> of <c>INamed</c>.
    Property INamed_name { get; }

    /// Generic concept that corresponds to a model node, i.e. <see cref="INode"/>.
    Concept Node { get; }
}

/// <inheritdoc cref="IBuiltInsLanguage"/>
[Obsolete("Use IBuiltInsLanguage instead")]
public sealed class BuiltInsLanguage
{
    [Obsolete("Use IBuiltInsLanguage instead")]
    public static readonly IBuiltInsLanguage Instance = LionWebVersions.Current.BuiltIns;
}

/// <summary>
/// An interface for "named things".
/// It corresponds nominally to <see cref="IBuiltInsLanguage.INamed"/>.
/// </summary>
public interface INamed : IReadableNode
{
    /// The human-readable name.
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="Name"/>.
    /// </summary>
    /// <param name="name">Value of <see cref="Name"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Name"/> is set; <c>false</c> otherwise.</returns>
    public bool TryGetName([NotNullWhen(true)] out string? name);
}

/// Writable variant of <see cref="INamed"/>.
public interface INamedWritable : INamed, IWritableNode
{
    string INamed.Name => Name;

    /// <inheritdoc cref="INamed.Name"/>
    public new string Name { get; set; }

    /// <inheritdoc cref="INamed.Name"/>
    public INamedWritable SetName(string value);
}