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

/// Helpers to properly deal with dictionaries in records
public static class RecordDictionaryExtensions
{
    /// Deep-equals of two dictionaries.
    public static bool DictionaryEquals<K,V>(this Dictionary<K,V>? left, Dictionary<K,V>? right) where K : notnull
    {
        if (left == null || right == null)
        {
            return left == right;
        }

        if (left.Count != right.Count)
            return false;

        return left.All(leftPair =>
            right.TryGetValue(leftPair.Key, out var rightValue)
            && EqualityComparer<V>.Default.Equals(leftPair.Value, rightValue)
        );
    }

    /// Deep hash code of a dictionary.
    public static void DictionaryHashCode<K,V>(this ref HashCode hashCode, Dictionary<K,V>? dict) where K : notnull
    {
        if (dict == null)
        {
            return;
        }

        foreach (var pair in dict)
        {
            hashCode.Add(pair.Key);
            hashCode.Add(pair.Value);
        }
    }

    /// Appends <paramref name="dict"/> to <paramref name="builder"/>.
    public static void DictionaryPrintMembers<K,V>(this StringBuilder builder, Dictionary<K,V>? dict) where K : notnull
    {
        if (dict == null)
        {
            builder.Append("null");
            return;
        }

        builder.Append("{ ");

        bool first = true;
        foreach (var pair in dict)
        {
            if (first)
            {
                first = false;
            } else
            {
                builder.Append(", ");
            }

            builder.Append(pair.Key);
            builder.Append(": ");
            if (pair.Value is not null)
                builder.Append(pair.Value);
            else
                builder.Append("null");
        }

        builder.Append(" }");
    }
}