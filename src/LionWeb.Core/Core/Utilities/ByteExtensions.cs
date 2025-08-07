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

namespace LionWeb.Core.Utilities;

using System.Text;

/// Convenience methods to deal with byte arrays.
public static class ByteExtensions
{
    /// Converts <paramref name="str"/> to bytes in <see cref="Encoding.ASCII"/> encoding.
    public static byte[] AsAsciiBytes(string str) =>
        Encoding.ASCII.GetBytes(str);

    /// Converts <paramref name="str"/> to bytes in <see cref="Encoding.UTF8"/> encoding.
    public static byte[] AsUtf8Bytes(string str) =>
        Encoding.UTF8.GetBytes(str);

    /// Checks whether byte arrays <paramref name="left"/> and <paramref name="right"/> are equal by length and content.
    public static bool Equals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) =>
        left.SequenceEqual(right);

    /// <inheritdoc cref="Equals(System.ReadOnlySpan{byte},System.ReadOnlySpan{byte})"/>
    public static bool Equals(byte[] left, byte[] right) =>
        Equals(left.AsSpan(), right.AsSpan());

    public static int GetHashCode(ReadOnlySpan<byte> bytes)
    {
        unchecked
        {
            var result = 0;
            foreach (byte b in bytes)
                result = (result * 31) ^ b;
            return result;
        }
    }
}