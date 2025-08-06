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

using System.Text;
using Utilities;

public record struct NodeIdX
{
    private const byte _invalidChar = byte.MaxValue;

    private readonly byte[] _value;
    internal readonly string? _invalidId;

    public NodeIdX(string nodeId)
    {
        if (!Encode(nodeId, out _value))
            _invalidId = nodeId;
    }

    public override readonly int GetHashCode() =>
        _invalidId?.GetHashCode() ?? _value.GetHashCode();

    public override readonly string ToString() =>
        _invalidId ?? Decode(_value);

    public readonly bool Equals(NodeIdX other)
    {
        if (_invalidId != null || other._invalidId != null)
            return ToString().Equals(other.ToString(), StringComparison.InvariantCulture);

        return ByteExtensions.Equals(_invalidId, other._invalidId);
    }

    private readonly bool PrintMembers(StringBuilder builder)
    {
        builder.Append(ToString());
        return true;
    }

    #region Encode

    private static bool Encode(string nodeId, out byte[] result)
    {
        var maxLength = nodeId.Length * 6 / 8;
        if (maxLength == 0)
        {
            result = [];
            return false;
        }

        result = new byte[maxLength + 4];

        int currentStringIndex = 0;
        int currentByteIndex = 0;

        while (currentStringIndex < nodeId.Length)
        {
            if (!EncodeFourChars(nodeId.AsSpan(currentStringIndex, 4), result.AsSpan(currentByteIndex, 3)))
            {
                result = [];
                return false;
            }

            currentStringIndex += 4;
            currentByteIndex += 3;
        }

        return true;
    }

    internal static bool EncodeFourChars(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        Console.WriteLine($"chars: {new string(chars)}");
        
        if (
            !Encode(chars[0], out var b0) ||
            !Encode(chars[1], out var b1) ||
            !Encode(chars[2], out var b2) ||
            !Encode(chars[3], out var b3)
        )
            return false;

        Console.WriteLine($"b0:{b0:b8} b1:{b1:b8} b2:{b2:b8} b3:{b3:b8}");
        
        byte b0To0 = (byte)(b0 << 2);
        byte b1To0 = (byte)((b1 & 0b110000) >> 4);
        byte to0 = (byte)(b0To0 | b1To0);
        bytes[0] = to0;
        Console.WriteLine($"b0To0:{b0To0:b8} b1To0:{b1To0:b8} to0:{to0:b8}");
        
        byte b1To1 = (byte)((b1 & 0b1111) << 4);
        byte b2To1 = (byte)((b2 & 0b111100) >> 2);
        byte to1 = (byte)(b1To1 | b2To1);
        bytes[1] = to1;
        Console.WriteLine($"b1To1:{b1To1:b8} b2To1:{b2To1:b8} to1:{to1:b8}");
        
        byte b2To2 = (byte)(b2 << 6);
        byte b3To2 = (byte)(b3);
        byte to2 = (byte)(b2To2 | b3To2);
        bytes[2] = to2;
        Console.WriteLine($"b2To2:{b2To2:b8} b3To2:{b3To2:b8} to2:{to2:b8}");

        return true;
    }

    internal static bool Encode(char c, out byte value)
    {
        switch (c)
        {
            case >= '0' and <= '9':
                value = (byte)(c - '0');
                break;
            case >= 'A' and <= 'Z':
                value = (byte)(c - 'A' + 10);
                break;
            case >= 'a' and <= 'z':
                value = (byte)(c - 'a' + 36);
                break;
            case '-':
                value = 62;
                break;
            case '_':
                value = 63;
                break;
            default:
                value = _invalidChar;
                return false;
        }

        return true;
    }

    #endregion

    #region Decode

    private static string Decode(byte[] bytes)
    {
        var maxLength = bytes.Length * 8 / 6;

        char[] result = new char[maxLength];

        int currentStringIndex = 0;
        int currentByteIndex = 0;

        while (currentByteIndex < bytes.Length)
        {
            DecodeThreeBytes(bytes.AsSpan(currentByteIndex, 3), result.AsSpan(currentStringIndex, 4));

            currentStringIndex += 3;
            currentByteIndex += 4;
        }

        return new string(result);
    }

    internal static void DecodeThreeBytes(ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        byte b0 = bytes[0];
        byte b1 = bytes[1];
        byte b2 = bytes[2];

        Console.WriteLine($"b0:{b0:b8} b1:{b1:b8}  b2:{b2:b8}");


        byte c0 = (byte)(b0 >> 2);
        chars[0] = Decode(c0);
        Console.WriteLine($"c0:{c0:b8}");

        byte b0ToC1 = (byte)((b0 & 0b11) << 4);
        byte b1ToC1 = (byte)((b1 & 0b11110000) >> 4);
        var c1 = (byte)(b0ToC1 | b1ToC1);
        chars[1] = Decode(c1);
        Console.WriteLine($"b0ToC1:{b0ToC1:b8} b1ToC1:{b1ToC1:b8} c1:{c1:b8}");


        byte b1ToC2 = (byte)((b1 & 0b1111) << 2);
        byte b2ToC2 = (byte)((b2  & 0b11000000) >> 6);
        byte c2 = (byte)(b1ToC2 | b2ToC2);
        chars[2] = Decode(c2);
        Console.WriteLine($"b1ToC2:{b1ToC2:b8} b2ToC2:{b2ToC2:b8} c2:{c2:b8}");

        byte c3 = (byte)(b2 & 0b111111);
        chars[3] = Decode((byte)c3);
        Console.WriteLine($"c3:{c3:b8}");

        Console.WriteLine($"chars: {new string(chars)}");
    }

    internal static char Decode(byte b) =>
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