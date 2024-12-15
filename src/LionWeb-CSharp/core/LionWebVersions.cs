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

    /// Whether this LionWeb standard is compatible with <paramref name="other"/>.
    /// <remarks>
    /// The implementation MUST be symmetric!
    /// </remarks>
    public bool IsCompatibleWith(LionWebVersions other);

    /// The current default version.
    public static LionWebVersions Current => v2023_1;

    /// All supported <i>pure</i> versions of LionWeb standard.
    public static IReadOnlyList<LionWebVersions> AllPureVersions { get => [v2023_1, v2024_1]; }

    /// All supported <i>mixed</i> versions of LionWeb standard.
    public static IReadOnlyList<LionWebVersions> AllMixedVersions { get => [v2024_1_Compatible]; }

    /// Finds the <i>pure</i> version of LionWeb standard defined by <paramref name="versionString"/>.
    /// <exception cref="UnsupportedVersionException">If LionWeb standard <paramref name="versionString"/> is not supported.</exception>
    public static LionWebVersions GetPureByVersionString(string versionString) =>
        AllPureVersions.FirstOrDefault(v => v.VersionString == versionString) ??
        throw new UnsupportedVersionException(versionString);

    /// Finds the version of LionWeb standard defined by <paramref name="versionInterface"/>.
    /// <param name="versionInterface">Interface defining the LionWeb version. Must be a specialization of <see cref="LionWebVersions"/>.</param>
    /// <exception cref="UnsupportedVersionException">If LionWeb standard <paramref name="versionInterface"/> is not supported.</exception>
    public static LionWebVersions GetByInterface(Type versionInterface) => versionInterface switch
    {
        _ when versionInterface == typeof(IVersion2023_1) => v2023_1,
        _ when versionInterface == typeof(IVersion2024_1) => v2024_1,
        _ when versionInterface == typeof(IVersion2024_1_Compatible) => v2024_1_Compatible,
        _ => throw new UnsupportedVersionException(versionInterface.ToString())
    };

    /// <inheritdoc cref="IVersion2023_1"/>
    public static IVersion2023_1 v2023_1 => Version2023_1.Instance;

    /// <inheritdoc cref="IVersion2024_1"/>
    public static IVersion2024_1 v2024_1 => Version2024_1.Instance;

    /// <inheritdoc cref="IVersion2024_1_Compatible"/>
    public static IVersion2024_1_Compatible v2024_1_Compatible => Version2024_1_Compatible.Instance;
}

/// Extensions for <see cref="LionWebVersions"/>.
public static class LionWebVersionsExtensions
{
    /// Assures <paramref name="self"/> and <paramref name="other"/> are compatible.
    /// <exception cref="VersionMismatchException">If <paramref name="self"/> and <paramref name="other"/> are NOT compatible.</exception>
    public static void AssureCompatible(this LionWebVersions self, LionWebVersions other, string? message = null)
    {
        if (!self.IsCompatibleWith(other))
            throw new VersionMismatchException(self, other, message);
    }

    /// Assures <paramref name="self"/> and <paramref name="otherVersionString"/> are compatible.
    /// <exception cref="UnsupportedVersionException">If LionWeb standard <paramref name="otherVersionString"/> is not supported.</exception>
    /// <exception cref="VersionMismatchException">If <paramref name="self"/> and <paramref name="otherVersionString"/> are NOT compatible.</exception>
    public static void AssureCompatible(this LionWebVersions self, string otherVersionString, string? message = null) =>
        AssureCompatible(self, LionWebVersions.GetPureByVersionString(otherVersionString), message);

    /// Assures <paramref name="self"/> and <paramref name="language"/>'s <see cref="Language.LionWebVersion"/> are compatible.
    /// <exception cref="VersionMismatchException">If <paramref name="self"/> and <paramref name="language"/>'s <see cref="Language.LionWebVersion"/> are NOT compatible.</exception>
    public static void AssureCompatible(this LionWebVersions self, Language language) =>
        AssureCompatible(language.LionWebVersion, self, $"[{language.Key}, {language.Version}]");
}

/// LionWeb standard 2023.1, defined at https://lionweb.io/specification/2023.1
public interface IVersion2023_1 : LionWebVersions;

/// LionWeb standard 2024.1, defined at https://lionweb.io/specification/2024.1
public interface IVersion2024_1 : LionWebVersions;

/// LionWeb standard 2024.1, backwards-compatible with 2023.1
public interface IVersion2024_1_Compatible : LionWebVersions;

internal interface IVersionBase : LionWebVersions
{
    bool IsCompatibleWithInternal(LionWebVersions other);
}

internal abstract class VersionBase<TBuiltIns, TLionCore> : IVersionBase where TBuiltIns : IBuiltInsLanguage
    where TLionCore : ILionCoreLanguage
{
    public abstract string VersionString { get; }

    IBuiltInsLanguage LionWebVersions.BuiltIns { get => BuiltIns; }
    public abstract TBuiltIns BuiltIns { get; }

    ILionCoreLanguage LionWebVersions.LionCore { get => LionCore; }
    public abstract TLionCore LionCore { get; }

    bool LionWebVersions.IsCompatibleWith(LionWebVersions other) =>
        this.IsCompatibleWithInternal(other) || ((IVersionBase)other).IsCompatibleWithInternal(this);

    public virtual bool IsCompatibleWithInternal(LionWebVersions other) =>
        ReferenceEquals(this, other);
}