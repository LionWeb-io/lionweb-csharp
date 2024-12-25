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

namespace LionWeb.Core.Generator.Impl;

using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics.CodeAnalysis;

/// String mangling helpers.
public static class StringExtensions
{
    /// Converts the first character of <paramref name="str"/> to uppercase.
    /// <returns><c>null</c> if <paramref name="str"/> is <c>null</c></returns>
    [return: NotNullIfNotNull(nameof(str))]
    public static string? ToFirstUpper(this string? str)
    {
        if (str == null)
            return null;

        if (str.Length == 0)
            return str;

        char[] a = str.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    /// Converts the first character of <paramref name="str"/> to lowercase.
    /// <returns><c>null</c> if <paramref name="str"/> is <c>null</c></returns>
    [return: NotNullIfNotNull(nameof(str))]
    public static string? ToFirstLower(this string? str)
    {
        if (str == null)
            return null;

        if (str.Length == 0)
            return str;

        char[] a = str.ToCharArray();
        a[0] = char.ToLower(a[0]);
        return new string(a);
    }
    
    /// Prefixes <paramref name="candidate"/> with <c>@</c> iff <paramref name="candidate"/> is a C# keyword.
    public static string PrefixKeyword(this string candidate) =>
        SyntaxFacts.GetKeywordKind(candidate) == SyntaxKind.None &&
        SyntaxFacts.GetContextualKeywordKind(candidate) == SyntaxKind.None
            ? candidate
            : $"@{candidate}";

}