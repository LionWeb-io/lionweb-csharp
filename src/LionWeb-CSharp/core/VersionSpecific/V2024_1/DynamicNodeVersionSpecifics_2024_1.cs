﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.VersionSpecific.V2024_1;

using M2;
using M3;
using Utilities;
using V2023_1;

/// <see cref="DynamicNode"/> parts specific to LionWeb <see cref="IVersion2024_1_Compatible"/>.  
internal class DynamicNodeVersionSpecifics_2024_1_Compatible : IDynamicNodeVersionSpecifics
{
    public LionWebVersions Version => LionWebVersions.v2024_1_Compatible;

    public object PrepareSetProperty(Property property, object? value)
    {
        switch (value)
        {
            case string when property.Type.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.String):
            case int when property.Type.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Integer):
            case bool when property.Type.EqualsIdentity(BuiltInsLanguage_2023_1.Instance.Boolean):
            case string when property.Type.EqualsIdentity(BuiltInsLanguage_2024_1.Instance.String):
            case int when property.Type.EqualsIdentity(BuiltInsLanguage_2024_1.Instance.Integer):
            case bool when property.Type.EqualsIdentity(BuiltInsLanguage_2024_1.Instance.Boolean):
                return value;

            case Enum when property.Type is Enumeration e:
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

                break;
        }

        throw new InvalidValueException(property, value);
    }
}