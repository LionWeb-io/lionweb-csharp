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

namespace LionWeb.Core.M2;

using M3;
using System.Collections.Immutable;

/// <summary>
/// Optional cache for M2 information, disabled by default.
/// Not filled implicitly, needs explicit call to <see cref="Register"/>.
/// <para/>
/// If available, used by <see cref="M2Extensions"/>.
/// </summary>
public interface IGlobalM2Cache
{
    /// <summary>
    /// Instance of the cache, if available.
    /// </summary>
    public static IGlobalM2Cache? Instance { get; protected set; }

    /// <summary>
    /// Enables the cache.
    /// No-op if already enabled. 
    /// </summary>
    static void Enable() =>
        Instance ??= new M2Cache();

    /// <summary>
    /// Disables the cache.
    /// </summary>
    static void Disable()
    {
        if (Instance is IDisposable disposable)
            disposable.Dispose();

        Instance = null;
    }

    /// <inheritdoc cref="M2Extensions.FindByKey{T}"/>
    /// <returns>The found element, or <see langword="null"/> if not found.</returns>
    T? FindByKey<T>(Language language, MetaPointerKey key) where T : class, IKeyed;

    /// <inheritdoc cref="M2Extensions.AllFeatures"/>
    /// <returns>All features of <paramref name="classifier"/> and all its generalizations,
    /// or <see langword="null"/> if <paramref name="classifier"/> unknown.</returns>
    IImmutableSet<Feature>? AllFeatures(Classifier classifier);

    /// <inheritdoc cref="M2Extensions.AllGeneralizations"/>
    /// <returns>All direct and indirect generalizations of <paramref name="classifier"/>,
    /// or <see langword="null"/> if <paramref name="classifier"/> unknown.</returns>
    IImmutableSet<Classifier>? AllGeneralizations(Classifier classifier);

    /// <inheritdoc cref="M2Extensions.AllSpecializations"/>
    /// <returns>All direct and indirect specializations of <paramref name="classifier"/>,
    /// or <see langword="null"/> if <paramref name="classifier"/> unknown.</returns>
    IImmutableSet<Classifier>? AllSpecializations(Classifier classifier);

    /// <inheritdoc cref="M2Extensions.DirectGeneralizations(Classifier)"/>
    /// <returns>All direct generalizations of <paramref name="classifier"/>,
    /// or <see langword="null"/> if <paramref name="classifier"/> unknown.</returns>
    IImmutableSet<Classifier>? DirectGeneralizations(Classifier classifier);

    /// <inheritdoc cref="M2Extensions.DirectSpecializations"/>
    /// <returns>All direct specializations of <paramref name="classifier"/>,
    /// or <see langword="null"/> if <paramref name="classifier"/> unknown.</returns>
    IImmutableSet<Classifier>? DirectSpecializations(Classifier classifier);

    /// <inheritdoc cref="M2Extensions.FeatureByKey"/>
    /// <returns>Feature with <paramref name="key"/> contained in <paramref name="classifier"/> or any of its generalizations,
    /// or <see langword="null"/> if <paramref name="classifier"/> unknown.</returns>
    Feature? FeatureByKey(Classifier classifier, MetaPointerKey key);

    /// <inheritdoc cref="M2Extensions.FieldByKey"/>
    /// <returns>Field with <paramref name="key"/> contained in <paramref name="structuredDataType"/>,
    /// or <see langword="null"/> if <paramref name="structuredDataType"/> unknown.</returns>
    Field? FieldByKey(StructuredDataType structuredDataType, MetaPointerKey key);

    /// <summary>
    /// Returns the literal with <paramref name="key"/> contained in <paramref name="enumeration"/>.
    /// </summary>
    /// <param name="enumeration">Enumeration to search through.</param>
    /// <param name="key">Key of requested literal.</param>
    /// <returns>Literal with <paramref name="key"/> contained in <paramref name="enumeration"/>,
    /// or <see langword="null"/> if <paramref name="enumeration"/> unknown.</returns>
    EnumerationLiteral? LiteralByKey(Enumeration enumeration, MetaPointerKey key);

    /// <summary>
    /// Clears the cache.
    /// </summary>
    void Clear();

    /// <summary>
    /// Registers all <paramref name="languages"/>, and all directly or indirectly referenced languages.
    /// Afterward, this cache holds all relations between elements of the registered languages. 
    /// </summary>
    void Register(IEnumerable<Language> languages);
}