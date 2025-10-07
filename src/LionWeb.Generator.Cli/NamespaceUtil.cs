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

// ReSharper disable InconsistentNaming
namespace LionWeb.Generator.Cli;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

/// <summary>
/// According to <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#643-identifiers">C# spec</a>.
/// </summary>
public partial class NamespaceUtil
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Hex_Digit = "[a-fA-F0-9]";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Underscore_Character = @"(?:_|\\u005|\\U0000005)";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Unicode_Escape_Sequence = $@"(?:\\u{Hex_Digit}{{4}}|\\U{Hex_Digit}{{8}})";

    // Category Letter, all subcategories; category Number, subcategory letter.
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Letter_Character = $@"(?:\p{{L}}|\p{{Nl}}|{Unicode_Escape_Sequence})";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Identifier_Start_Character = $@"(?:{Letter_Character}|{Underscore_Character})";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Identifier_Part_Character =
        $"(?:{Letter_Character}|{Decimal_Digit_Character}|{Connecting_Character}|{Combining_Character}|{Formatting_Character})";

    // Category Number, subcategory decimal digit.
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Decimal_Digit_Character = @"[\p{Nd}]";

    // Category Other, subcategory format.
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Formatting_Character = @"[\p{Cf}]";

    // Category Mark, subcategories non-spacing and spacing combining.
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Combining_Character = @"[\p{Mn}\p{Mc}]";

    // Category Punctuation, subcategory connector.
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Connecting_Character = @"[\p{Pc}]";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Basic_Identifier = $"(?:{Identifier_Start_Character}{Identifier_Part_Character}*)";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    private const string Escaped_Identifier = $"(@?{Basic_Identifier})";


    [GeneratedRegex(@$"^{Escaped_Identifier}(?:\.{Escaped_Identifier})*$",
        RegexOptions.CultureInvariant | RegexOptions.Singleline)]
    public static partial Regex NamespaceRegex();
}