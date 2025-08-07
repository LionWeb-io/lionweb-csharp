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

using System.Runtime.CompilerServices;
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
        if (!Encode(nodeId, out _value))
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
        if (_invalidId != null || other._invalidId != null)
            return ToString().Equals(other.ToString(), StringComparison.InvariantCulture);

        return ByteExtensions.Equals(_value, other._value);
    }

    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append(ToString());
        return true;
    }

    #region Encode

    private static bool Encode(string nodeId, out byte[] result)
    {
        var stringLength = nodeId.Length;
        if (stringLength == 0)
        {
            result = [];
            return false;
        }

        var maxLength = stringLength * 6 / 8 + 1;
        
        result = new byte[maxLength + 2];

        int currentStringIndex = 0;
        int currentByteIndex = 0;

        var regularLimit = stringLength - 3;
        while (currentStringIndex < regularLimit)
        {
            if (!EncodeFourChars(nodeId.AsSpan(currentStringIndex, 4), result.AsSpan(currentByteIndex, 3)))
            {
                result = [];
                return false;
            }

            currentStringIndex += 4;
            currentByteIndex += 3;
        }

        if (currentStringIndex < stringLength)
        {
            char[] buffer = new char[4];
            nodeId.AsSpan(currentStringIndex).CopyTo(buffer);
            if (!EncodeFourChars(buffer.AsSpan(), result.AsSpan(currentByteIndex, 3)))
            {
                result = [];
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool EncodeFourChars(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        if (
            !Encode(chars[0], out var b0) ||
            !Encode(chars[1], out var b1) ||
            !Encode(chars[2], out var b2) ||
            !Encode(chars[3], out var b3)
        )
            return false;

        bytes[0] = (byte)(b0 << 2 | b1 >> 4);
        bytes[1] = (byte)(b1 << 4 | b2 >> 2);
        bytes[2] = (byte)(b2 << 6 | b3);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool Encode(char c, out int value)
    {
        switch (c)
        {
            case >= '0' and <= '9':
                value = c - '0';
                return true;
            case >= 'A' and <= 'Z':
                value = c - 'A' + 10;
                return true;
            case >= 'a' and <= 'z':
                value = c - 'a' + 36;
                return true;
            case '-':
                value = 62;
                return true;
            case '_':
                value = 63;
                return true;
            case '\0':
                value = 0;
                return true;
            default:
                value = _invalidChar;
                return false;
        }
    }

    #endregion

    #region Decode

    private static string Decode(ReadOnlySpan<byte> bytes)
    {
        var bytesLength = bytes.Length;
        var maxLength = bytesLength * 8 / 6;

        char[] result = new char[maxLength + 4];

        int currentStringIndex = 0;
        int currentByteIndex = 0;

        var regularLimit = bytesLength - 2;
        while (currentByteIndex < regularLimit)
        {
            DecodeThreeBytes(bytes.Slice(currentByteIndex, 3), result.AsSpan(currentStringIndex, 4));

            currentStringIndex += 4;
            currentByteIndex += 3;
        }

        if (currentByteIndex < bytesLength)
        {
            byte[] buffer = new byte[3];
            bytes[currentByteIndex..].CopyTo(buffer);
            DecodeThreeBytes(buffer.AsSpan(), result.AsSpan(currentStringIndex, 4));
        }

        return new string(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DecodeThreeBytes(ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        byte b0 = bytes[0];
        byte b1 = bytes[1];
        byte b2 = bytes[2];

        chars[0] = Decode(b0 >> 2);
        chars[1] = Decode((b0 & 0b11) << 4 | b1 >> 4);
        chars[2] = Decode((b1 & 0b1111) << 2 | b2 >> 6);
        chars[3] = Decode(b2 & 0b111111);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static char Decode(int b) =>
        b switch
        {
            <= 9 => (char)(b + '0'),
            <= 35 => (char)(b - 10 + 'A'),
            <= 61 => (char)(b - 36 + 'a'),
            62 => '-',
            63 => '_',
            _ => char.MaxValue
        };

    #endregion
}