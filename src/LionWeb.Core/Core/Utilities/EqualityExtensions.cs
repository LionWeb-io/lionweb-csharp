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
    private static readonly FieldIdentityComparer _fieldComparer = new();
    private static readonly EnumerationLiteralIdentityComparer _enumerationLiteralComparer = new();
    private static readonly KeyedIdentityComparer _keyedIdentityComparer = new();

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
    /// <seealso cref="EqualsIdentity(Language,Language?)"/>
    public static bool EqualsIdentity(this LanguageEntity left, LanguageEntity? right) =>
        _entityComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="LanguageEntity"/> hash code by its <see cref="IKeyed.Key"/> and its <see cref="M2Extensions.GetLanguage"/>.
    /// </summary>
    public static int GetHashCodeIdentity(this LanguageEntity obj) =>
        _entityComparer.GetHashCode(obj);

    /// <inheritdoc cref="FeatureIdentityComparer"/>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/> and <see cref="M2Extensions.GetFeatureClassifier">hosting Classifier</see>; <c>false</c> otherwise.</returns>
    /// <seealso cref="EqualsIdentity(LanguageEntity,LanguageEntity?)"/>
    public static bool EqualsIdentity(this Feature left, Feature? right) =>
        _featureComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="Feature"/> hash code by its <see cref="IKeyed.Key"/> and its <see cref="M2Extensions.GetFeatureClassifier">hosting Classifier</see>.
    /// </summary>
    public static int GetHashCodeIdentity(this Feature obj) =>
        _featureComparer.GetHashCode(obj);
    
    /// <inheritdoc cref="FieldIdentityComparer"/>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/> and <see cref="M2Extensions.GetStructuredDataType">hosting StructuredDataType</see>; <c>false</c> otherwise.</returns>
    /// <seealso cref="EqualsIdentity(LanguageEntity,LanguageEntity?)"/>
    public static bool EqualsIdentity(this Field left, Field? right) =>
        _fieldComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="Field"/> hash code by its <see cref="IKeyed.Key"/> and its <see cref="M2Extensions.GetStructuredDataType">hosting StructuredDataType</see>.
    /// </summary>
    public static int GetHashCodeIdentity(this Field obj) =>
        _fieldComparer.GetHashCode(obj);
    
    /// <inheritdoc cref="EnumerationLiteralIdentityComparer"/>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/> and <see cref="M2Extensions.GetEnumeration">hosting Enumeration</see>; <c>false</c> otherwise.</returns>
    /// <seealso cref="EqualsIdentity(LanguageEntity,LanguageEntity?)"/>
    public static bool EqualsIdentity(this EnumerationLiteral left, EnumerationLiteral? right) =>
        _enumerationLiteralComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="EnumerationLiteral"/> hash code by its <see cref="IKeyed.Key"/> and its <see cref="M2Extensions.GetEnumeration">hosting Enumeration</see>.
    /// </summary>
    public static int GetHashCodeIdentity(this EnumerationLiteral obj) =>
        _enumerationLiteralComparer.GetHashCode(obj);

    /// <inheritdoc cref="KeyedIdentityComparer"/>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> are equal according to the applicable IdentityComparer; <c>false</c> otherwise.</returns>
    /// <seealso cref="KeyedIdentityComparer"/>
    public static bool EqualsIdentity(this IKeyed left, IKeyed? right) =>
        _keyedIdentityComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="IKeyed"/> hash code by forwarding to the applicable IdentityComparer.
    /// </summary>
    /// <seealso cref="KeyedIdentityComparer"/>
    public static int GetHashCodeIdentity(this IKeyed obj) =>
        _keyedIdentityComparer.GetHashCode(obj);
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
/// Compares <see cref="Feature">Features</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetFeatureClassifier">hosting Classifier</see>.
/// </summary>
/// <seealso cref="LanguageEntityIdentityComparer"/>
/// <seealso cref="LanguageIdentityComparer"/>
public class FeatureClassifierIdentityComparer : IEqualityComparer<Feature>
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
/// Compares <see cref="Feature">Features</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetLanguage">hosting Language</see>.
/// </summary>
/// <remarks>
/// We <i>don't include</i> the feature's classifier by default to align with <see cref="LionWeb.Core.Serialization.MetaPointer"/> semantics
/// -- keys should be unique per language.
/// Refer to <see cref="FeatureClassifierIdentityComparer"/> to also include the feature's classifier.
/// </remarks>
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
               x.GetLanguage().EqualsIdentity(y.GetLanguage());
    }

    /// <inheritdoc />
    public int GetHashCode(Feature obj) =>
        HashCode.Combine(obj.Key, obj.GetLanguage().GetHashCodeIdentity());
}

/// <summary>
/// Compares <see cref="Field">Fields</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetStructuredDataType">hosting StructuredDataType</see>.
/// </summary>
/// <seealso cref="LanguageEntityIdentityComparer"/>
/// <seealso cref="LanguageIdentityComparer"/>
public class FieldIdentityComparer : IEqualityComparer<Field>
{
    /// <inheritdoc />
    public bool Equals(Field? x, Field? y)
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
               x.GetStructuredDataType().EqualsIdentity(y.GetStructuredDataType());
    }

    /// <inheritdoc />
    public int GetHashCode(Field obj) =>
        HashCode.Combine(obj.Key, obj.GetStructuredDataType().GetHashCodeIdentity());
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
    public int GetHashCode(LanguageEntity obj)
    {
        if(obj.GetParent() != null)
            return HashCode.Combine(obj.Key, obj.GetLanguage().GetHashCodeIdentity());
        return HashCode.Combine(obj.Key);
    }
}

/// <summary>
/// Compares <see cref="EnumerationLiteral">EnumerationLiterals</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetEnumeration">hosting Enumeration.</see>.
/// </summary>
/// <seealso cref="LanguageEntityIdentityComparer"/>
public class EnumerationLiteralIdentityComparer : IEqualityComparer<EnumerationLiteral>
{
    /// <inheritdoc />
    public bool Equals(EnumerationLiteral? x, EnumerationLiteral? y)
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
               x.GetEnumeration().EqualsIdentity(y.GetEnumeration());
    }

    /// <inheritdoc />
    public int GetHashCode(EnumerationLiteral obj)
    {
        if(obj.GetParent() != null)
            return HashCode.Combine(obj.Key, obj.GetEnumeration().GetHashCodeIdentity());
        return HashCode.Combine(obj.Key);
    }
}

/// <summary>
/// Compares <see cref="IKeyed">Keyed language parts</see> by forwarding to the applicable IdentityComparer.
/// </summary>
/// <seealso cref="LanguageIdentityComparer"/>
/// <seealso cref="LanguageEntityIdentityComparer"/>
/// <seealso cref="FeatureIdentityComparer"/>
/// <seealso cref="FieldIdentityComparer"/>
/// <seealso cref="EnumerationLiteralIdentityComparer"/>
public class KeyedIdentityComparer : IEqualityComparer<IKeyed>
{
    /// <inheritdoc />
    public bool Equals(IKeyed? x, IKeyed? y) => (x, y) switch
    {
        (Language a, Language b) => a.EqualsIdentity(b),
        (LanguageEntity a, LanguageEntity b) => a.EqualsIdentity(b),
        (Feature a, Feature b) => a.EqualsIdentity(b),
        (Field a, Field b) => a.EqualsIdentity(b),
        (EnumerationLiteral a, EnumerationLiteral b) => a.EqualsIdentity(b),
        (null, null) => true,
        _ => false
    };

    /// <inheritdoc />
    public int GetHashCode(IKeyed obj) => obj switch
    {
        Language l => l.GetHashCodeIdentity(),
        LanguageEntity l => l.GetHashCodeIdentity(),
        Feature l => l.GetHashCodeIdentity(),
        Field l => l.GetHashCodeIdentity(),
        EnumerationLiteral l => l.GetHashCodeIdentity(),
        _ => 0
    };
}

/// Compares nodes by id.
public class NodeIdComparer<T> : IEqualityComparer<T> where T : IReadableNode
{
    /// <inheritdoc />
    public bool Equals(T? x, T? y) =>
        x?.GetId() == y?.GetId();

    /// <inheritdoc />
    public int GetHashCode(T obj) =>
        obj.GetId().GetHashCode();
}