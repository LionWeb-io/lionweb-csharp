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
    /// <inheritdoc cref="LanguageIdentityComparer"/>
    public static readonly LanguageIdentityComparer LanguageComparer = new();

    /// <inheritdoc cref="LanguageEntityIdentityComparer"/>
    public static readonly LanguageEntityIdentityComparer EntityComparer = new();

    /// <inheritdoc cref="FeatureIdentityComparer"/>
    public static readonly FeatureIdentityComparer FeatureComparer = new();

    /// <inheritdoc cref="FieldIdentityComparer"/>
    public static readonly FieldIdentityComparer FieldComparer = new();

    /// <inheritdoc cref="EnumerationLiteralIdentityComparer"/>
    public static readonly EnumerationLiteralIdentityComparer EnumerationLiteralComparer = new();

    /// <inheritdoc cref="Utilities.KeyedIdentityComparer"/>
    public static readonly KeyedIdentityComparer KeyedIdentityComparer = new();

    /// <inheritdoc cref="LanguageIdentityComparer"/>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/>
    /// and <see cref="Language.Version"/>;
    /// <c>false</c> otherwise.
    /// </returns>
    public static bool EqualsIdentity(this Language left, Language? right) =>
        LanguageComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="Language"/> hash code by its <see cref="IKeyed.Key"/>
    /// and <see cref="Language.Version"/>.
    /// </summary>
    public static int GetHashCodeIdentity(this Language obj) =>
        LanguageComparer.GetHashCode(obj);

    /// <inheritdoc cref="LanguageIdentityComparer"/>
    public static int CompareIdentity(this Language left, Language? right) =>
        LanguageComparer.Compare(left, right);

    /// <inheritdoc cref="LanguageEntityIdentityComparer"/>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/>
    /// and <see cref="M2Extensions.GetLanguage">Language</see>;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="EqualsIdentity(Language,Language?)"/>
    public static bool EqualsIdentity(this LanguageEntity left, LanguageEntity? right) =>
        EntityComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="LanguageEntity"/> hash code by its <see cref="IKeyed.Key"/>
    /// and its <see cref="M2Extensions.GetLanguage"/>.
    /// </summary>
    public static int GetHashCodeIdentity(this LanguageEntity obj) =>
        EntityComparer.GetHashCode(obj);

    /// <inheritdoc cref="LanguageEntityIdentityComparer"/>
    public static int CompareIdentity(this LanguageEntity left, LanguageEntity? right) =>
        EntityComparer.Compare(left, right);

    /// <inheritdoc cref="FeatureIdentityComparer"/>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/>
    /// and <see cref="M2Extensions.GetFeatureClassifier">hosting Classifier</see>;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="EqualsIdentity(LanguageEntity,LanguageEntity?)"/>
    public static bool EqualsIdentity(this Feature left, Feature? right) =>
        FeatureComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="Feature"/> hash code by its <see cref="IKeyed.Key"/>
    /// and its <see cref="M2Extensions.GetFeatureClassifier">hosting Classifier</see>.
    /// </summary>
    public static int GetHashCodeIdentity(this Feature obj) =>
        FeatureComparer.GetHashCode(obj);

    /// <inheritdoc cref="FeatureIdentityComparer"/>
    public static int CompareIdentity(this Feature left, Feature? right) =>
        FeatureComparer.Compare(left, right);

    /// <inheritdoc cref="FieldIdentityComparer"/>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/>
    /// and <see cref="M2Extensions.GetStructuredDataType">hosting StructuredDataType</see>;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="EqualsIdentity(LanguageEntity,LanguageEntity?)"/>
    public static bool EqualsIdentity(this Field left, Field? right) =>
        FieldComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="Field"/> hash code by its <see cref="IKeyed.Key"/>
    /// and its <see cref="M2Extensions.GetStructuredDataType">hosting StructuredDataType</see>.
    /// </summary>
    public static int GetHashCodeIdentity(this Field obj) =>
        FieldComparer.GetHashCode(obj);

    /// <inheritdoc cref="FieldIdentityComparer"/>
    public static int CompareIdentity(this Field left, Field? right) =>
        FieldComparer.Compare(left, right);

    /// <inheritdoc cref="EnumerationLiteralIdentityComparer"/>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same <see cref="IKeyed.Key"/>
    /// and <see cref="M2Extensions.GetEnumeration">hosting Enumeration</see>;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="EqualsIdentity(LanguageEntity,LanguageEntity?)"/>
    public static bool EqualsIdentity(this EnumerationLiteral left, EnumerationLiteral? right) =>
        EnumerationLiteralComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="EnumerationLiteral"/> hash code by its <see cref="IKeyed.Key"/>
    /// and its <see cref="M2Extensions.GetEnumeration">hosting Enumeration</see>.
    /// </summary>
    public static int GetHashCodeIdentity(this EnumerationLiteral obj) =>
        EnumerationLiteralComparer.GetHashCode(obj);

    /// <inheritdoc cref="EnumerationLiteralIdentityComparer"/>
    public static int CompareIdentity(this EnumerationLiteral left, EnumerationLiteral? right) =>
        EnumerationLiteralComparer.Compare(left, right);

    /// <inheritdoc cref="Utilities.KeyedIdentityComparer"/>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> are equal according to the applicable IdentityComparer;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="Utilities.KeyedIdentityComparer"/>
    public static bool EqualsIdentity(this IKeyed left, IKeyed? right) =>
        KeyedIdentityComparer.Equals(left, right);

    /// <summary>
    /// Calculates <see cref="IKeyed"/> hash code by forwarding to the applicable IdentityComparer.
    /// </summary>
    /// <seealso cref="Utilities.KeyedIdentityComparer"/>
    public static int GetHashCodeIdentity(this IKeyed obj) =>
        KeyedIdentityComparer.GetHashCode(obj);

    /// <inheritdoc cref="Utilities.KeyedIdentityComparer"/>
    public static int CompareIdentity(this IKeyed left, IKeyed? right) =>
        KeyedIdentityComparer.Compare(left, right);
}

/// <summary>
/// Compares <see cref="Language">Languages</see> by their <see cref="IKeyed.Key"/> and <see cref="Language.Version"/>.
/// </summary>
public class LanguageIdentityComparer : IEqualityComparer<Language>, IComparer<Language>
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

    /// <inheritdoc />
    public int Compare(Language? x, Language? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (y is null)
            return 1;

        if (x is null)
            return -1;

        var keyComparison = string.Compare(x.Key, y.Key, StringComparison.InvariantCulture);
        if (keyComparison != 0)
            return keyComparison;

        return string.Compare(x.Version, y.Version, StringComparison.InvariantCulture);
    }
}

/// <summary>
/// Compares <see cref="Feature">Features</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetFeatureClassifier">hosting Classifier</see>.
/// </summary>
/// <seealso cref="LanguageEntityIdentityComparer"/>
/// <seealso cref="LanguageIdentityComparer"/>
public class FeatureClassifierIdentityComparer : IEqualityComparer<Feature>, IComparer<Feature>
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

    /// <inheritdoc />
    public int Compare(Feature? x, Feature? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (y is null)
            return 1;

        if (x is null)
            return -1;

        var keyComparison = string.Compare(x.Key, y.Key, StringComparison.InvariantCulture);
        if (keyComparison != 0)
            return keyComparison;

        return x.GetFeatureClassifier().CompareIdentity(y.GetFeatureClassifier());
    }
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
public class FeatureIdentityComparer : IEqualityComparer<Feature>, IComparer<Feature>
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

    /// <inheritdoc />
    public int Compare(Feature? x, Feature? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (y is null)
            return 1;

        if (x is null)
            return -1;

        var keyComparison = string.Compare(x.Key, y.Key, StringComparison.InvariantCulture);
        if (keyComparison != 0)
            return keyComparison;

        return x.GetLanguage().CompareIdentity(y.GetLanguage());
    }
}

/// <summary>
/// Compares <see cref="Field">Fields</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetStructuredDataType">hosting StructuredDataType</see>.
/// </summary>
/// <seealso cref="LanguageEntityIdentityComparer"/>
/// <seealso cref="LanguageIdentityComparer"/>
public class FieldIdentityComparer : IEqualityComparer<Field>, IComparer<Field>
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

    /// <inheritdoc />
    public int Compare(Field? x, Field? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (y is null)
            return 1;

        if (x is null)
            return -1;

        var keyComparison = string.Compare(x.Key, y.Key, StringComparison.InvariantCulture);
        if (keyComparison != 0)
            return keyComparison;

        return x.GetStructuredDataType().CompareIdentity(y.GetStructuredDataType());
    }
}

/// <summary>
/// Compares <see cref="LanguageEntity">LanguageEntities</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetLanguage">Languages</see>.
/// </summary>
/// <seealso cref="LanguageIdentityComparer"/>
public class LanguageEntityIdentityComparer : IEqualityComparer<LanguageEntity>, IComparer<LanguageEntity>
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
        if (obj.GetParent() != null)
            return HashCode.Combine(obj.Key, obj.GetLanguage().GetHashCodeIdentity());
        return HashCode.Combine(obj.Key);
    }

    /// <inheritdoc />
    public int Compare(LanguageEntity? x, LanguageEntity? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (y is null)
            return 1;

        if (x is null)
            return -1;

        var keyComparison = string.Compare(x.Key, y.Key, StringComparison.InvariantCulture);
        if (keyComparison != 0)
            return keyComparison;

        return x.GetLanguage().CompareIdentity(y.GetLanguage());
    }
}

/// <summary>
/// Compares <see cref="EnumerationLiteral">EnumerationLiterals</see> by their <see cref="IKeyed.Key"/>, and their <see cref="M2Extensions.GetEnumeration">hosting Enumeration.</see>.
/// </summary>
/// <seealso cref="LanguageEntityIdentityComparer"/>
public class EnumerationLiteralIdentityComparer : IEqualityComparer<EnumerationLiteral>, IComparer<EnumerationLiteral>
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
        if (obj.GetParent() != null)
            return HashCode.Combine(obj.Key, obj.GetEnumeration().GetHashCodeIdentity());
        return HashCode.Combine(obj.Key);
    }

    /// <inheritdoc />
    public int Compare(EnumerationLiteral? x, EnumerationLiteral? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (y is null)
            return 1;

        if (x is null)
            return -1;

        var keyComparison = string.Compare(x.Key, y.Key, StringComparison.InvariantCulture);
        if (keyComparison != 0)
            return keyComparison;

        return x.GetEnumeration().CompareIdentity(y.GetEnumeration());
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
public class KeyedIdentityComparer : IEqualityComparer<IKeyed>, IComparer<IKeyed>
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

    /// <inheritdoc />
    public int Compare(IKeyed? x, IKeyed? y) => (x, y) switch
    {
        (Language a, Language b) => a.CompareIdentity(b),
        (LanguageEntity a, LanguageEntity b) => a.CompareIdentity(b),
        (Feature a, Feature b) => a.CompareIdentity(b),
        (Field a, Field b) => a.CompareIdentity(b),
        (EnumerationLiteral a, EnumerationLiteral b) => a.CompareIdentity(b),
        ({ } a, { } b) => string.Compare(a.Key, b.Key, StringComparison.InvariantCulture),
        (null, null) => 0,
        (_, null) => 1,
        (null, _) => -1
    };
}

/// Compares nodes by id.
public class NodeIdComparer<T> : IEqualityComparer<T>, IComparer<T> where T : IReadableNode
{
    /// <inheritdoc />
    public bool Equals(T? x, T? y) =>
        x?.GetId() == y?.GetId();

    /// <inheritdoc />
    public int GetHashCode(T obj) =>
        obj.GetId().GetHashCode();

    /// <inheritdoc />
    public int Compare(T? x, T? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (y is null)
            return 1;

        if (x is null)
            return -1;

        return string.Compare(x.GetId(), y.GetId(), StringComparison.InvariantCulture);
    }
}