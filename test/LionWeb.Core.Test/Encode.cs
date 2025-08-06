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

namespace LionWeb.Core.Test;

using System.Collections;
using System.Reflection;

[TestClass]
public class Encode
{
    private static readonly Dictionary<byte, char> _mapping = new Dictionary<byte, char>()
    {
        { 0b000000, '0' },
        { 0b000001, '1' },
        { 0b000010, '2' },
        { 0b000011, '3' },
        { 0b000100, '4' },
        { 0b000101, '5' },
        { 0b000110, '6' },
        { 0b000111, '7' },
        { 0b001000, '8' },
        { 0b001001, '9' },
        { 0b001010, 'A' },
        { 0b001011, 'B' },
        { 0b001100, 'C' },
        { 0b001101, 'D' },
        { 0b001110, 'E' },
        { 0b001111, 'F' },
        { 0b010000, 'G' },
        { 0b010001, 'H' },
        { 0b010010, 'I' },
        { 0b010011, 'J' },
        { 0b010100, 'K' },
        { 0b010101, 'L' },
        { 0b010110, 'M' },
        { 0b010111, 'N' },
        { 0b011000, 'O' },
        { 0b011001, 'P' },
        { 0b011010, 'Q' },
        { 0b011011, 'R' },
        { 0b011100, 'S' },
        { 0b011101, 'T' },
        { 0b011110, 'U' },
        { 0b011111, 'V' },
        { 0b100000, 'W' },
        { 0b100001, 'X' },
        { 0b100010, 'Y' },
        { 0b100011, 'Z' },
        { 0b100100, 'a' },
        { 0b100101, 'b' },
        { 0b100110, 'c' },
        { 0b100111, 'd' },
        { 0b101000, 'e' },
        { 0b101001, 'f' },
        { 0b101010, 'g' },
        { 0b101011, 'h' },
        { 0b101100, 'i' },
        { 0b101101, 'j' },
        { 0b101110, 'k' },
        { 0b101111, 'l' },
        { 0b110000, 'm' },
        { 0b110001, 'n' },
        { 0b110010, 'o' },
        { 0b110011, 'p' },
        { 0b110100, 'q' },
        { 0b110101, 'r' },
        { 0b110110, 's' },
        { 0b110111, 't' },
        { 0b111000, 'u' },
        { 0b111001, 'v' },
        { 0b111010, 'w' },
        { 0b111011, 'x' },
        { 0b111100, 'y' },
        { 0b111101, 'z' },
        { 0b111110, '-' },
        { 0b111111, '_' }
    };

    public static IEnumerable<object[]> TestPair =>
        _mapping.Select(pair => new object[] { pair.Key, pair.Value });

    [TestMethod]
    [DynamicData(nameof(TestPair))]
    public void EncodeChar(byte b, char c)
    {
        NodeIdX.Encode(c, out var actual);
        Assert.AreEqual(b, actual);
    }

    [TestMethod]
    [DynamicData(nameof(TestPair))]
    public void DecodeByte(byte b, char c)
    {
        var actual = NodeIdX.Decode(b);
        Assert.AreEqual(c, actual);
    }
    
    public static IEnumerable<object[]> FourChars =>
    [
        [new byte[] { 0, 0, 0 }, new[] { '0', '0', '0', '0' }],
        [new byte[] { 0b000000_00, 0b1010_1001, 0b00_111111 }, new[] { '0', 'A', 'a', '_' }],
        [new byte[] { 0b111111_11, 0b1110_1111, 0b11_111110 }, new[] { '_', '-', '_', '-' }],
        [new byte[] { 0b111111_11, 0b1111_1111, 0b11_111111 }, new[] { '_', '_', '_', '_' }],
        [new byte[] { 0b000001_00, 0b0010_0000, 0b11_000100 }, new[] { '1', '2', '3', '4' }],
    ];
    
    public static string DisplayFourChars(MethodInfo methodInfo, object[] values)
    {
        var bytes = (byte[])values[0];
        var chars = (char[])values[1];
    
        return new string(chars);
    }

    [TestMethod]
    [DynamicData(nameof(FourChars), DynamicDataDisplayName = nameof(DisplayFourChars))]
    public void EncodeFourChars(byte[] bytes, char[] chars)
    {
        var actual = new byte[3];
        NodeIdX.EncodeFourChars(chars.AsSpan(), actual.AsSpan());
        Assert.AreEqual(bytes[0], actual[0]);
        Assert.AreEqual(bytes[1], actual[1]);
        Assert.AreEqual(bytes[2], actual[2]);
    }
    
    [TestMethod]
    [DynamicData(nameof(FourChars), DynamicDataDisplayName = nameof(DisplayFourChars))]
    public void DecodeThreeBytes(byte[] bytes, char[] chars)
    {
        char[] actual = new char[4];
        NodeIdX.DecodeThreeBytes(bytes.AsSpan(), actual.AsSpan());
        Assert.AreEqual(chars[0], actual[0]);
        Assert.AreEqual(chars[1], actual[1]);
        Assert.AreEqual(chars[2], actual[2]);
        Assert.AreEqual(chars[3], actual[3]);
    }
    
    public static IEnumerable<object[]> FourCharsRandom
    {
        get
        {
            var random = new Random();
            return Enumerable.Range(0, 50)
                .Select(_ => new object[] { new[] { RandomChar(), RandomChar(), RandomChar(), RandomChar() } });

            char RandomChar()
            {
                return _mapping[(byte)random.Next(0, 64)];
            }
        }
    }

    public static string DisplayFourCharsRandom(MethodInfo methodInfo, object[] values) =>
        new((char[])values[0]);

    [TestMethod]
    [DynamicData(nameof(FourCharsRandom), DynamicDataDisplayName = nameof(DisplayFourCharsRandom))]
    public void EncodeFourCharsRandom(char[] chars)
    {
        byte[] bytes = new byte[3];
        NodeIdX.EncodeFourChars(chars.AsSpan(), bytes.AsSpan());
        Console.WriteLine();
        char[] actual = new char[4];
        NodeIdX.DecodeThreeBytes(bytes.AsSpan(),  actual.AsSpan());
        
        Assert.AreEqual(chars[0], actual[0]);
        Assert.AreEqual(chars[1], actual[1]);
        Assert.AreEqual(chars[2], actual[2]);
        Assert.AreEqual(chars[3], actual[3]);
    }
}