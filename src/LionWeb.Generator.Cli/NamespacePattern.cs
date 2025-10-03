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

namespace LionWeb.Generator.Cli;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter<NamespacePattern>))]
public enum NamespacePattern
{
    DotSeparated = 1,
    DotSeparatedFirstUppercase
}

public static class NamespacePatternExtensions
{
    public static bool IsSet(this NamespacePattern? candidate) =>
        candidate is not null && candidate is not default(NamespacePattern);

    public static bool FirstToUpper(this NamespacePattern pattern) => pattern switch
    {
        NamespacePattern.DotSeparated => false,
        NamespacePattern.DotSeparatedFirstUppercase => true,
        _ => throw new UnknownEnumValueException<NamespacePattern>(pattern)
    };
}