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

namespace LionWeb.Core.VersionSpecific.V2023_1;

using M1;
using M3;

internal class SerializerVersionSpecifics_2023_1 : SerializerVersionSpecificsBase
{
    public override LionWebVersions Version => LionWebVersions.v2023_1;

    public override PropertyValue? ConvertDatatype(IReadableNode node, Feature property, object? value) =>
        value switch
        {
            null => null,
            Enum e => ConvertEnumeration(e),
            int or bool or string => ConvertPrimitiveType(value),
            _ => _handler?.UnknownDatatype(node, property, value)
        };
}