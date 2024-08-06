// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M1;

using Serialization;
using System.Security.Cryptography;
using System.Text;

public class DuplicateIdChecker
{
    private readonly HashSet<CompressedId> _knownIds = new();

    public bool IsIdDuplicate(CompressedId compressedId) =>
        !_knownIds.Add(compressedId);
}

public interface CompressedElement
{
    protected static byte[] AsBytes(string str) =>
        Encoding.ASCII.GetBytes(str);

    protected static bool Equals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) =>
        left.SequenceEqual(right);
}

public readonly struct CompressedId(byte[] identifier, string? original) : CompressedElement
{
    public byte[] Identifier { get; } = identifier;
    public string? Original { get; } = original;

    public static CompressedId Create(string id, bool keepOriginal)
    {
        var sha1 = SHA1.Create();
        var idHash = sha1.ComputeHash(CompressedElement.AsBytes(id));
        return new CompressedId(idHash, keepOriginal ? id : null);
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        BitConverter.ToInt32(Identifier, 0);

    /// <inheritdoc />
    public override bool Equals(object? other) =>
        other is CompressedId id && CompressedElement.Equals(Identifier, id.Identifier);

    /// <inheritdoc />
    public override string ToString() =>
        Original ?? BitConverter.ToString(Identifier);
}

public readonly struct CompressedMetaPointer(byte[] identifier, MetaPointer? original) : CompressedElement
{
    public byte[] Identifier { get; } = identifier;
    public MetaPointer? Original { get; } = original;

    public static CompressedMetaPointer Create(MetaPointer metaPointer, bool keepOriginal)
    {
        var sha1 = SHA1.Create();
        sha1.ComputeHash(CompressedElement.AsBytes(metaPointer.Language));
        sha1.ComputeHash(CompressedElement.AsBytes(metaPointer.Version));
        sha1.ComputeHash(CompressedElement.AsBytes(metaPointer.Key));
        return new CompressedMetaPointer(sha1.Hash!, keepOriginal ? metaPointer : null);
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        BitConverter.ToInt32(Identifier, 0);

    /// <inheritdoc />
    public override bool Equals(object? other) =>
        other is CompressedMetaPointer id && CompressedElement.Equals(Identifier, id.Identifier);

    /// <inheritdoc />
    public override string ToString() =>
        Original?.ToString() ?? BitConverter.ToString(Identifier);
}