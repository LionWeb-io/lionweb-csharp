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

namespace LionWeb.Core;

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Utilities;

public readonly record struct NodeIdX
{
    private const byte _invalidChar = byte.MaxValue;

    private readonly byte[] _value;
    private readonly int _stringLength;

    private readonly string? _invalidId;

    public NodeIdX(string nodeId)
    {
        _stringLength = nodeId.Length;
        _value = Encode(nodeId);
        if (_value.Length == 0)
            _invalidId = nodeId;
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        _invalidId?.GetHashCode() ?? ByteExtensions.GetHashCode(_value);

    /// <inheritdoc />
    public override string ToString() =>
        _invalidId ?? Decode(_value).Substring(0, _stringLength);

    /// <inheritdoc />
    public bool Equals(NodeIdX other)
    {
        if (_invalidId != null && other._invalidId != null)
            return ToString().Equals(other.ToString(), StringComparison.InvariantCulture);

        return ByteExtensions.Equals(_value, other._value);
    }

    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append(ToString());
        return true;
    }

    #region Encode

    private delegate bool EncodeDelegate(ReadOnlySpan<char> chars, Span<byte> bytes);

    private static readonly EncodeDelegate _encode =
        BitConverter.IsLittleEndian ? EncodeFourChars : EncodeFourCharsBigEndian;

    private static int Ceiling(int number, int multipleOf) =>
        ((number + multipleOf - 1) / multipleOf) * multipleOf;

    private static byte[] Encode(string nodeId)
    {
        var stringLength = nodeId.Length;
        if (stringLength == 0)
            return [];

        var maxLength = (int)Math.Ceiling(stringLength * 6f / 8 + 3);

        var result = new byte[Ceiling(maxLength, 4)];

        int currentStringIndex = 0;
        int currentByteIndex = 0;

        var regularLimit = stringLength - 3;
        while (currentStringIndex < regularLimit)
        {
            if (!_encode(nodeId.AsSpan(currentStringIndex, 4), result.AsSpan(currentByteIndex, 4)))
                return [];

            currentStringIndex += 4;
            currentByteIndex += 3;
        }

        if (currentStringIndex < stringLength)
        {
            char[] buffer = new char[4];
            nodeId.AsSpan(currentStringIndex).CopyTo(buffer);
            if (!_encode(buffer.AsSpan(), result.AsSpan(currentByteIndex, 4)))
                return [];
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool EncodeFourChars(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        int c0 = Encode(chars[0]);
        int c1 = Encode(chars[1]);
        int c2 = Encode(chars[2]);
        int c3 = Encode(chars[3]);

        if (
            c0 == _invalidChar ||
            c1 == _invalidChar ||
            c2 == _invalidChar ||
            c3 == _invalidChar
        )
            return false;

        int i = (c0 << 2 | c1 >> 4) & 0xff |
                (((c1 << 4 | c2 >> 2) & 0xff) << 8) |
                (((c2 << 6 | c3) & 0xff) << 16);

        return BitConverter.TryWriteBytes(bytes, i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool EncodeFourCharsBigEndian(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        int c0 = Encode(chars[0]);
        int c1 = Encode(chars[1]);
        int c2 = Encode(chars[2]);
        int c3 = Encode(chars[3]);

        if (
            c0 == _invalidChar ||
            c1 == _invalidChar ||
            c2 == _invalidChar ||
            c3 == _invalidChar
        )
            return false;

        int i = (c3 << 2 | c2 >> 4) & 0xff |
                (((c2 << 4 | c1 >> 2) & 0xff) << 8) |
                (((c1 << 6 | c0) & 0xff) << 16);

        return BitConverter.TryWriteBytes(bytes, i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Encode(char c)
    {
        unchecked
        {
            return c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'A' and <= 'Z' => c - 'A' + 10,
                >= 'a' and <= 'z' => c - 'a' + 36,
                '-' => 62,
                '_' => 63,
                '\0' => 0,
                _ => _invalidChar
            };
        }
    }

    #endregion

    #region Decode

    private delegate void DecodeDelegate(ReadOnlySpan<byte> bytes, Span<char> chars);

    private static readonly DecodeDelegate _decode =
        BitConverter.IsLittleEndian ? DecodeThreeBytes : DecodeThreeBytesBigEndian;

    private static string Decode(ReadOnlySpan<byte> bytes)
    {
        var bytesLength = bytes.Length;
        var maxLength = (int)Math.Ceiling(bytesLength * 8f / 6);

        char[] result = new char[Ceiling(maxLength, 4)];

        int currentStringIndex = 0;
        int currentByteIndex = 0;

        var regularLimit = bytesLength - 3;
        while (currentByteIndex < regularLimit)
        {
            _decode(bytes.Slice(currentByteIndex, 3), result.AsSpan(currentStringIndex, 4));

            currentStringIndex += 4;
            currentByteIndex += 3;
        }

        return new string(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DecodeThreeBytes(ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        uint l = (uint)((bytes[0] << 16) | (bytes[1] << 8) | bytes[2]);

        var c = DecodeThreeBytes(l);

        BitConverter.TryWriteBytes(MemoryMarshal.Cast<char, byte>(chars), c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DecodeThreeBytesBigEndian(ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        uint l = (uint)((bytes[0] << 16) | (bytes[1] << 8) | bytes[2]);
        l = BinaryPrimitives.ReverseEndianness(l);

        var c = DecodeThreeBytes(l);
        c = BinaryPrimitives.ReverseEndianness(c);

        BitConverter.TryWriteBytes(MemoryMarshal.Cast<char, byte>(chars), c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong DecodeThreeBytes(uint bytes)
    {
        uint i0 = Decode(bytes >> 18);
        uint i1 = Decode((bytes >> 12) & 0x3F);
        uint i2 = Decode((bytes >> 6) & 0x3F);
        uint i3 = Decode(bytes & 0x3F);

        return ((ulong)i3 << 48) | ((ulong)i2 << 32) | (i1 << 16) | i0;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint Decode(uint b)
    {
        unchecked
        {
            return b switch
            {
                <= 9 => b + '0',
                <= 35 => b - 10 + 'A',
                <= 61 => b - 36 + 'a',
                62 => '-',
                63 => '_',
                _ => char.MaxValue
            };
        }
    }

    #endregion
}