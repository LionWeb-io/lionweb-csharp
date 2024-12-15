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

/// Externalized logic of <see cref="DynamicNode"/>, specific to one version of LionWeb standard.
internal interface IDynamicNodeVersionSpecifics : IVersionSpecifics
{
    /// <summary>
    /// Creates an instance of <see cref="IDynamicNodeVersionSpecifics"/> that implements <paramref name="lionWebVersion"/>.
    /// </summary>
    /// <exception cref="UnsupportedVersionException"></exception>
    public static IDynamicNodeVersionSpecifics Create(LionWebVersions lionWebVersion) => lionWebVersion switch
    {
        IVersion2023_1 => new DynamicNodeVersionSpecifics_2023_1(),
        IVersion2024_1 => new DynamicNodeVersionSpecifics_2024_1(),
        IVersion2024_1_Compatible => new DynamicNodeVersionSpecifics_2024_1_Compatible(),
        _ => throw new UnsupportedVersionException(lionWebVersion)
    };

    /// Prepares <paramref name="value"/> to be set as value of <paramref name="property"/>.
    /// <exception cref="InvalidValueException">If <paramref name="value"/> cannot be set as value for <paramref name="property"/>.</exception>
    object PrepareSetProperty(Property property, object? value);
}

internal abstract class DynamicNodeVersionSpecificsBase : IDynamicNodeVersionSpecifics
{
    public abstract LionWebVersions Version { get; }

    public abstract object PrepareSetProperty(Property property, object? value);

    protected object? PrepareSetEnum(object value, Enumeration e)
    {
        try
        {
            var factory = e.GetLanguage().GetFactory();
            var enumerationLiteral = e.Literals[0];
            Enum literal = factory.GetEnumerationLiteral(enumerationLiteral);
            if (literal.GetType().IsEnumDefined(value))
            {
                return value;
            }
        } catch (ArgumentException)
        {
            // fall-through
        }

        return null;
    }
}