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

namespace LionWeb.Core.Test.Utilities;

public static class StringRandomizer
{
    // Constant seed for reproducible tests
    private static readonly Random _defaultRandom = new Random(0x1EE7);
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";

    public static string RandomLength() =>
        Random(_defaultRandom.Next(500));

    public static string Random(int length, string? chars = _chars) =>
        new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_defaultRandom.Next(s.Length)]).ToArray());
    
    public static char RandomChar(string? chars = _chars) => chars[_defaultRandom.Next(chars.Length)];
}