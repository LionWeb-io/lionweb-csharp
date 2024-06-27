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

namespace LionWeb.Core.M2;

using M3;

/// <summary>
/// Common supertype of all LionCore attributes.
/// </summary>
public abstract class LionCoreAttribute : Attribute;

/// <summary>
/// Common supertype of all LionCore attributes that specify a Key.
/// </summary>
public abstract class LionCoreKeyAttribute : LionCoreAttribute
{
    /// <inheritdoc cref="IKeyed.Key"/>
    public required string Key { get; init; }
}

/// <summary>
/// Attribute to declare the LionWeb MetaPointer of <see cref="IKeyed"/> except <see cref="Language"/>.
/// </summary>
/// <seealso cref="LionCoreLanguage">For "metapointer" of a Language.</seealso>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Interface |
                AttributeTargets.Property)]
public class LionCoreMetaPointer : LionCoreKeyAttribute
{
    /// <summary>
    /// The <see cref="Language">LionWeb Language</see> that defines this element.
    /// </summary>
    public required Type Language { get; init; }
}

/// <summary>
/// Attribute to declare LionWeb key and version of a <see cref="Language"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LionCoreLanguage : LionCoreKeyAttribute
{
    /// <inheritdoc cref="Language.Version"/>
    public required string Version { get; init; }
}

/// <summary>
/// Attribute to declare the nature of a <see cref="Feature">LionWeb Feature</see>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class LionCoreFeature : LionCoreAttribute
{
    /// <inheritdoc cref="LionCoreFeatureKind"/>
    public required LionCoreFeatureKind Kind { get; init; }

    /// <inheritdoc cref="Feature.Optional"/>
    public required bool Optional { get; init; }

    /// <inheritdoc cref="Link.Multiple"/>
    /// <summary>
    /// <c>false</c> if feature is not a <see cref="Link"/>.
    /// </summary>
    public required bool Multiple { get; init; }
}

/// <summary>
/// Represents the subtype of <see cref="Feature"/> at hand.
/// </summary>
public enum LionCoreFeatureKind
{
    /// <summary>
    /// Feature is a <see cref="Property"/>.
    /// </summary>
    Property,

    /// <summary>
    /// Feature is a <see cref="Containment"/>.
    /// </summary>
    Containment,

    /// <summary>
    /// Feature is a <see cref="Reference"/>.
    /// </summary>
    Reference
}

/// <summary>
/// Extension methods related to <see cref="LionCoreMetaPointer"/>.
/// </summary>
public static class AttributeExtensions
{
    /// <summary>
    /// Gets the value an attribute on the given enumeration value.
    /// </summary>
    /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
    /// <param name="enumValue">The enumeration value</param>
    /// <returns>The attribute of type T that exists on the given object</returns>
    private static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
    {
        if (enumValue == null)
            return null;
        
        var type = enumValue.GetType();
        var memberInfos = type.GetMember(enumValue.ToString());
        var attributes = memberInfos[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 ? (T)attributes[0] : null;
    }

    /// <summary>
    /// Gets the LionCore key from the given enumeration value.
    /// </summary>
    /// <param name="enumValue">The enumeration value</param>
    /// <returns>The key declared on the given enumeration value</returns>
    public static string? LionCoreKey(this Enum enumValue)
        => enumValue.GetAttributeOfType<LionCoreMetaPointer>()?.Key;
}