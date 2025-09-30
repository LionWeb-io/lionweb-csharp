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

using System.Text.RegularExpressions;

/// <summary>
/// According to <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#643-identifiers">C# spec</a>.
/// </summary>
public partial class NamespaceUtil
{
    // private const string Simple_Identifier =
    //     "(?:" +
    //     Available_Identifier +
    //     "|" + Escaped_Identifier +
    //     ")";
    //
    // private const string Available_Identifier =
    //     // excluding keywords or contextual keywords, see note below
    //     Basic_Identifier;
    //
    // private const string Escaped_Identifier =
    //     // Includes keywords and contextual keywords prefixed by '@'.
    //     // See note below.
    //     "@" + Basic_Identifier;
    //
    // private const string Basic_Identifier =
    //     Identifier_Start_Character + Identifier_Part_Character + "*";
    //     
    // private const string Identifier_Start_Character =
    //     Letter_Character +
    //     "|" + Underscore_Character;
    //
    // private const string Underscore_Character =
    //         "_" + // underscore
    //         @"|\\u005" + // Unicode_Escape_Sequence for underscore
    //         @"|\\U0000005" // Unicode_Escape_Sequence for underscore
    //     ;
    //
    // private const string Identifier_Part_Character =
    //     "(?:" +
    //     Letter_Character +
    //     "|" + Decimal_Digit_Character +
    //     "|" + Connecting_Character +
    //     "|" + Combining_Character +
    //     "|" + Formatting_Character +
    //     ")";
    //
    // private const string Decimal_Digit_Character =
    //         // Category Number, subcategory decimal digit.
    //         @"[\p{Nd}]"
    //     // Only escapes for category Nd allowed. See note below.
    //     // + "|" + Unicode_Escape_Sequence
    //     ;
    //
    // private const string Connecting_Character =
    //         // Category Punctuation, subcategory connector.
    //         @"[\p{Pc}]"
    //     // Only escapes for category Pc allowed. See note below.
    //     // + "|" + Unicode_Escape_Sequence
    //     ;
    //
    // private const string Combining_Character =
    //     // Category Mark, subcategories non-spacing and spacing combining.
    //     @"[\p{Mn}\p{Mc}]"
    //     // Only escapes for categories Mn & Mc allowed. See note below.
    //     // +"|" + Unicode_Escape_Sequence
    //     ;
    //
    // private const string Formatting_Character =
    //     // Category Other, subcategory format.
    //     @"[\p{Cf}]"
    //     // Only escapes for category Cf allowed, see note below.
    //     // + "|" + Unicode_Escape_Sequence
    //     ;
    //
    // private const string Letter_Character =
    //         "(?:" +
    //         // Category Letter, all subcategories; category Number, subcategory letter.
    //         @"[\p{L}\p{Nl}]"
    //         // Only escapes for categories L & Nl allowed. See note below.
    //         + "|" + Unicode_Escape_Sequence
    //         + ")"
    //     ;
    //
    // private const string Unicode_Escape_Sequence =
    //     @$"(?:\\u{Hex_Digit}{{4}})" +
    //     @$"|(?:\\U{Hex_Digit}{{8}})";

    private const string Hex_Digit = "[a-fA-F0-9]";

    private const string _namespacePart = $@"(?:\p{{L}}|\p{{Nl}}|\\u{Hex_Digit}{{4}}|\\U{Hex_Digit}{{8}})+";
    private const string _atNamespacePart = $"@?{_namespacePart}";

    [GeneratedRegex(@$"^{_atNamespacePart}(?:\.{_atNamespacePart})*$")]
    public static partial Regex NamespaceRegex();
}