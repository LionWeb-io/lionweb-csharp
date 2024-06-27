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

namespace LionWeb.Core.Utilities;

using M2;
using M3;

/// <summary>
/// Compares LionWeb M2 <see cref="IKeyed"/> by their key / MetaPointer.
/// </summary>
public static class EqualityExtensions
{
    private static readonly LanguageIdentityComparer _languageComparer = new();
    private static readonly LanguageEntityIdentityComparer _entityComparer = new();
    private static readonly FeatureIdentityComparer _featureComparer = new();

    /// <inheritdoc cref="LanguageIdentityComparer"/>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/> and <see cref="Language.Version"/>; <c>false</c> otherwise.</returns>
    public static bool EqualsIdentity(this Language left, Language? right) =>
        _languageComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="Language"/> hash code by its <see cref="IKeyed.Key"/> and <see cref="Language.Version"/>.
    /// </summary>
    public static int GetHashCodeIdentity(this Language obj) =>
        _languageComparer.GetHashCode(obj);

    /// <inheritdoc cref="LanguageEntityIdentityComparer"/>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/> and <see cref="M2Extensions.GetLanguage">Language</see>; <c>false</c> otherwise.</returns>
    /// <seealso cref="EqualsIdentity(LionWeb.Core.M3.Language,LionWeb.Core.M3.Language?)"/>
    public static bool EqualsIdentity(this LanguageEntity left, LanguageEntity? right) =>
        _entityComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="LanguageEntity"/> hash code by its <see cref="IKeyed.Key"/> and its <see cref="M2Extensions.GetLanguage"/>.
    /// </summary>
    public static int GetHashCodeIdentity(this LanguageEntity obj) =>
        _entityComparer.GetHashCode(obj);

    /// <inheritdoc cref="FeatureIdentityComparer"/>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/> and <see cref="M2Extensions.GetLanguage">Language</see>; <c>false</c> otherwise.</returns>
    /// <seealso cref="EqualsIdentity(LionWeb.Core.M3.Language,LionWeb.Core.M3.Language?)"/>
    public static bool EqualsIdentity(this Feature left, Feature? right) =>
        _featureComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="Feature"/> hash code by its <see cref="IKeyed.Key"/> and its <see cref="M2Extensions.GetLanguage"/>.
    /// </summary>
    public static int GetHashCodeIdentity(this Feature obj) =>
        _featureComparer.GetHashCode(obj);
}

/// <summary>
/// Compares <see cref="Language">Languages</see> by their <see cref="IKeyed.Key"/> and <see cref="Language.Version"/>.
/// </summary>
public class LanguageIdentityComparer : IEqualityComparer<Language>
{
    /// <inheritdoc />
    public bool Equals(Language? x, Language? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Key == y.Key &&
               x.Version == y.Version;
    }

    /// <inheritdoc />
    public int GetHashCode(Language obj) =>
        HashCode.Combine(obj.Key, obj.Version);
}

/// <summary>
/// Compares <see cref="Feature">Features</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetLanguage">Languages</see>.
/// </summary>
/// <seealso cref="LanguageEntityIdentityComparer"/>
/// <seealso cref="LanguageIdentityComparer"/>
public class FeatureIdentityComparer : IEqualityComparer<Feature>
{
    /// <inheritdoc />
    public bool Equals(Feature? x, Feature? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Key == y.Key &&
               x.GetFeatureClassifier().EqualsIdentity(y.GetFeatureClassifier());
    }

    /// <inheritdoc />
    public int GetHashCode(Feature obj) =>
        HashCode.Combine(obj.Key, obj.GetFeatureClassifier().GetHashCodeIdentity());
}

/// <summary>
/// Compares <see cref="LanguageEntity">LanguageEntities</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetLanguage">Languages</see>.
/// </summary>
/// <seealso cref="LanguageIdentityComparer"/>
public class LanguageEntityIdentityComparer : IEqualityComparer<LanguageEntity>
{
    /// <inheritdoc />
    public bool Equals(LanguageEntity? x, LanguageEntity? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Key == y.Key &&
               x.GetLanguage().EqualsIdentity(y.GetLanguage());
    }

    /// <inheritdoc />
    public int GetHashCode(LanguageEntity obj) =>
        HashCode.Combine(obj.Key, obj.GetLanguage().GetHashCodeIdentity());
}