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

namespace LionWeb.Core.Serialization;

using System.Text;

public static class ArrayEqualsExtensions
{
    public static bool ArrayEquals(this object[]? left, object[]? right)
    {
        if (left == null || right == null)
        {
            return left == right;
        }

        return left.SequenceEqual(right);
    }

    public static void ArrayHashCode(this ref HashCode hashCode, object[]? arr)
    {
        if (arr == null)
        {
            return;
        }

        foreach (var item in arr)
        {
            hashCode.Add(item);
        }
    }

    public static void ArrayPrintMembers(this StringBuilder builder, object[]? arr)
    {
        if (arr == null)
        {
            builder.Append("null");
            return;
        }

        builder.Append('[');

        bool first = true;
        foreach (var item in arr)
        {
            if (first)
            {
                first = false;
            } else
            {
                builder.Append(", ");
            }

            builder.Append(item);
        }

        builder.Append(']');
    }
}