// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core;

using M2;
using M3;
using VersionSpecific.V2023_1;
using VersionSpecific.V2024_1;

/// <summary>
/// A supported version of LionWeb standard.
///
/// <para>
/// We distinguish <i>pure</i> versions and <i>mixed</i> versions.
/// Pure versions support exactly one version of LionWeb standard.
/// </para>
///
/// <para>
/// Mixed versions still define one main version of LionWeb standard,
/// but keep compatibility with some defined versions as good as possible.
/// We should use them only in migration scenarios, as mixing standards can have unpredictable results. 
/// </para> 
/// </summary>
public interface LionWebVersions
{
    /// Version of this LionWeb standard, as used in
    /// <see cref="LionWeb.Core.Serialization.SerializationChunk.SerializationFormatVersion">SerializationChunk.SerializationFormatVersion</see>
    /// and <see cref="IBuiltInsLanguage"/>/<see cref="ILionCoreLanguage"/>'s <see cref="Language.Version"/>.
    public string VersionString { get; }

    /// The BuiltIns language adhering to this LionWeb standard.
    public IBuiltInsLanguage BuiltIns { get; }

    /// The LionCore M3 language adhering to this LionWeb standard.
    public ILionCoreLanguage LionCore { get; }

    public static LionWebVersions Current => v2023_1;

    public static IReadOnlyList<LionWebVersions> AllVersions { get => [v2023_1, v2024_1]; }

    public static LionWebVersions GetByVersionString(string versionString) =>
        AllVersions.FirstOrDefault(v => v.VersionString == versionString) ??
        throw new UnsupportedVersionException(versionString);

    /// <inheritdoc cref="IVersion2023_1"/>
    public static IVersion2023_1 v2023_1 => Version2023_1.Instance;

    /// <inheritdoc cref="IVersion2024_1"/>
    public static IVersion2024_1 v2024_1 => Version2024_1.Instance;
}

public interface IVersion2023_1 : LionWebVersions;

/// LionWeb standard 2024.1, defined at https://lionweb.io/specification/2024.1
public interface IVersion2024_1 : LionWebVersions;

/// LionWeb standard 2024.1, backwards-compatible with 2023.1
public interface IVersion2024_1_Compatible : LionWebVersions;

internal abstract class VersionBase<TBuiltIns, TLionCore> : LionWebVersions where TBuiltIns : IBuiltInsLanguage
    where TLionCore : ILionCoreLanguage
{
    public abstract string VersionString { get; }

    IBuiltInsLanguage LionWebVersions.BuiltIns { get => BuiltIns; }
    public abstract TBuiltIns BuiltIns { get; }

    ILionCoreLanguage LionWebVersions.LionCore { get => LionCore; }
    public abstract TLionCore LionCore { get; }
}