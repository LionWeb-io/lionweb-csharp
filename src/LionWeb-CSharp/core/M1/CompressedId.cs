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

    public bool IsIdDuplicate(string id) =>
        !_knownIds.Add(CompressedId.Create(id));
}

public interface CompressedElement
{
    protected static byte[] AsBytes(string str) =>
        Encoding.ASCII.GetBytes(str);

    protected static bool Equals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) =>
        left.SequenceEqual(right);
}

public readonly struct CompressedId(byte[] identifier) : CompressedElement
{
    public byte[] Identifier { get; } = identifier;

    public static CompressedId Create(string id)
    {
        var sha1 = SHA1.Create();
        var idHash = sha1.ComputeHash(CompressedElement.AsBytes(id));
        return new CompressedId(idHash);
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        BitConverter.ToInt32(Identifier, 0);

    /// <inheritdoc />
    public override bool Equals(object? other) =>
        other is CompressedId id && CompressedElement.Equals(Identifier, id.Identifier);
}

public readonly struct CompressedMetaPointer(byte[] identifier) : CompressedElement
{
    public byte[] Identifier { get; } = identifier;

    public static CompressedMetaPointer Create(MetaPointer metaPointer)
    {
        var sha1 = SHA1.Create();
        sha1.ComputeHash(CompressedElement.AsBytes(metaPointer.Language));
        sha1.ComputeHash(CompressedElement.AsBytes(metaPointer.Version));
        sha1.ComputeHash(CompressedElement.AsBytes(metaPointer.Key));
        return new CompressedMetaPointer(sha1.Hash!);
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        BitConverter.ToInt32(Identifier, 0);

    /// <inheritdoc />
    public override bool Equals(object? other) =>
        other is CompressedMetaPointer id && CompressedElement.Equals(Identifier, id.Identifier);
}