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

namespace LionWeb.Core.Test.Serialization.Protobuf.Streaming;

using Counter = uint;

public enum PredefinedString : StringIndex
{
    Null = 1,
    Empty = 2,
    True = 3,
    False = 4,
    Zero = 5,
    One = 6
}

public static class PredefinedStringExtensions
{
    public static readonly StringIndex Max = Enum.GetValues(typeof(PredefinedString)).Cast<StringIndex>().Max();

    public static bool TryGet(string? value, out StringIndex result)
    {
        switch (value)
        {
            case null:
                result = (StringIndex)PredefinedString.Null;
                return true;
            case "":
                result = (StringIndex)PredefinedString.Empty;
                return true;
            case "true":
                result = (StringIndex)PredefinedString.True;
                return true;
            case "false":
                result = (StringIndex)PredefinedString.False;
                return true;
            case "0":
                result = (StringIndex)PredefinedString.Zero;
                return true;
            case "1":
                result = (StringIndex)PredefinedString.One;
                return true;
            default:
                result = 0;
                return false;
        }
    }

    public static bool TryGet(this PredefinedString predefinedString, out string? result)
    {
        switch (predefinedString)
        {
            case PredefinedString.Null:
                result = null;
                return true;
            case PredefinedString.Empty:
                result = string.Empty;
                return true;
            case PredefinedString.True:
                result = bool.TrueString;
                return true;
            case PredefinedString.False:
                result = bool.FalseString;
                return true;
            case PredefinedString.Zero:
                result = "0";
                return true;
            case PredefinedString.One:
                result = "1";
                return true;
            default:
                result = null;
                return false;
        }
    }
}

class StringIndexer(Action<string, StringIndex> adder) : IndexCounterBase<string>(PredefinedStringExtensions.Max + 1, adder)
{
    protected override bool TryGet(string? candidate, out StringIndex result) =>
        PredefinedStringExtensions.TryGet(candidate, out result);
}

class StringLookup() : IndexLookupBase<string>(PredefinedStringExtensions.Max + 1)
{
    protected override bool TryGet(StringIndex idx, out string? candidate) =>
        ((PredefinedString)idx).TryGet(out candidate);
}