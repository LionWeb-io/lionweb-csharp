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

using M2;
using System.Diagnostics.CodeAnalysis;

/// A Language will provide the Concepts necessary to describe ideas
/// in a particular domain together with supporting elements necessary for the definition of those Concepts.
public interface Language : IKeyed, IPartitionInstance
{
    /// The version of this language. Can be any non-empty string.
    public string Version { get; }

    /// <summary>
    /// Gets the <see cref="Version"/>.
    /// </summary>
    /// <param name="version">Value of <see cref="Version"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Version"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetVersion([NotNullWhen(true)] out string? version);

    /// All <see cref="LanguageEntity">LanguageEntities</see> defined by this language.
    public IReadOnlyList<LanguageEntity> Entities { get; }

    /// <summary>
    /// Gets the <see cref="Entities"/>.
    /// </summary>
    /// <param name="entities">Value of <see cref="Entities"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Entities"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetEntities([NotNullWhen(true)] out IReadOnlyList<LanguageEntity>? entities)
    {
        entities = Entities;
        return entities is { Count: > 0 };
    }

    /// Other languages that define <see cref="LanguageEntity">LanguageEntities</see> that this language depends on.
    public IReadOnlyList<Language> DependsOn { get; }

    /// <summary>
    /// Gets the <see cref="DependsOn"/>.
    /// </summary>
    /// <param name="dependsOn">Value of <see cref="DependsOn"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="DependsOn"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetDependsOn([NotNullWhen(true)] out IReadOnlyList<Language>? dependsOn)
    {
        dependsOn = DependsOn;
        return dependsOn is { Count: > 0 };
    }

    /// <summary>
    /// Returns a node factory, capable of creating instances of this language's <see cref="Classifier">Classifiers</see>
    /// and <see cref="StructuredDataType">StructuredDataTypes</see>.
    /// We need a factory to establish the relation between the Language and generated C# types.  
    /// </summary>
    /// <returns>A node factory, capable of creating instances of this language's
    /// <see cref="Classifier">Classifiers</see> and <see cref="StructuredDataType">StructuredDataTypes</see>.</returns>
    public INodeFactory GetFactory();

    /// <summary>
    /// Sets a custom factory for this language.
    /// </summary>
    /// <param name="factory">New factory capable of creating instances of this language's
    /// <see cref="Classifier">Classifiers</see> and <see cref="StructuredDataType">StructuredDataTypes</see>.</param>
    public void SetFactory(INodeFactory factory);

    /// Version of LionWeb standard this language is based on.
    public LionWebVersions LionWebVersion { get; }
}

/// <inheritdoc/>
/// <typeparam name="T">Type of this language's factory.</typeparam>
public interface Language<T> : Language where T : INodeFactory
{
    /// <inheritdoc/>
    INodeFactory Language.GetFactory() => GetFactory();

    /// <inheritdoc cref="Language.GetFactory"/>
    public new T GetFactory();

    /// <inheritdoc />
    void Language.SetFactory(INodeFactory factory) => SetFactory((T)factory);

    /// <inheritdoc cref="Language.SetFactory"/>
    public void SetFactory(T factory);
}