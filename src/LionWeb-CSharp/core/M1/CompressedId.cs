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

// ReSharper disable ArrangeMethodOrOperatorBody

namespace LionWeb.Core.M1;

using Serialization;
using System.Security.Cryptography;
using System.Text;

/// Checks for duplicate node ids in an efficient manner.
public class DuplicateIdChecker
{
    private readonly HashSet<CompressedId> _knownIds = new();

    /// Whether <paramref name="compressedId"/> has been seen before by this instance.
    public bool IsIdDuplicate(CompressedId compressedId) =>
        !_knownIds.Add(compressedId);
}

/// Stores a LionWeb node id in a compact format, optionally preserving the original.
public readonly struct CompressedId : IEquatable<CompressedId>
{
    private CompressedId(byte[] identifier, string? original)
    {
        Identifier = identifier;
        Original = original;
    }

    private byte[] Identifier { get; }

    /// The original node id, if available.
    public string? Original { get; }

    /// <summary>
    /// Creates a new <see cref="CompressedId"/>.
    /// </summary>
    /// <param name="id">Node id to compress.</param>
    /// <param name="keepOriginal">Whether to store the uncompressed original. Uses more memory, but eases debugging.</param>
    /// <returns>The newly created compressed id.</returns>
    public static CompressedId Create(string id, bool keepOriginal)
    {
        var sha1 = SHA1.Create();
        var idHash = sha1.ComputeHash(CompressedElement.AsBytes(id));
        return new CompressedId(idHash, keepOriginal ? id : null);
    }

    /// <inheritdoc />
    public override string ToString() =>
        Original ?? BitConverter.ToString(Identifier);

    /// <inheritdoc />
    public override int GetHashCode() =>
        BitConverter.ToInt32(Identifier, 0);


    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is CompressedId other && Equals(other);

    /// <inheritdoc />
    public bool Equals(CompressedId other) =>
        CompressedElement.Equals(Identifier, other.Identifier);

    /// <inheritdoc cref="Equals(CompressedId)"/>
    public static bool operator ==(CompressedId left, CompressedId right) =>
        left.Equals(right);

    /// <inheritdoc cref="Equals(CompressedId)"/>
    public static bool operator !=(CompressedId left, CompressedId right) =>
        !left.Equals(right);
}

/// Stores a LionWeb MetaPointer in a compact format, optionally preserving the original.
public readonly struct CompressedMetaPointer : IEquatable<CompressedMetaPointer>
{
    private CompressedMetaPointer(CompressedId language, CompressedId version, CompressedId key, MetaPointer? original)
    {
        Language = language;
        Version = version;
        Key = key;
        Original = original;
    }

    /// The MetaPointer's language in compressed format.
    public CompressedId Language { get; }

    /// The MetaPointer's version in compressed format.
    public CompressedId Version { get; }

    /// The MetaPointer's key in compressed format.
    public CompressedId Key { get; }

    /// The original MetaPointer, if available.
    public MetaPointer? Original { get; }

    /// <summary>
    /// Creates a new <see cref="CompressedMetaPointer"/>.
    /// </summary>
    /// <param name="metaPointer">MetaPointer id to compress.</param>
    /// <param name="keepOriginal">Whether to store the uncompressed original. Uses more memory, but eases debugging.</param>
    /// <returns>The newly created compressed MetaPointer.</returns>
    public static CompressedMetaPointer Create(MetaPointer metaPointer, bool keepOriginal) =>
        new(
            CompressedId.Create(metaPointer.Language, keepOriginal),
            CompressedId.Create(metaPointer.Version, keepOriginal),
            CompressedId.Create(metaPointer.Key, keepOriginal),
            keepOriginal ? metaPointer : null
        );

    /// <inheritdoc />
    public override string ToString() =>
        Original?.ToString() ??
        $"{{Language={Language}, Version={Version}, Key={Key}}}";

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(Key, Language, Version);

    /// <inheritdoc />
    public bool Equals(CompressedMetaPointer other) =>
        Key.Equals(other.Key) &&
        Language.Equals(other.Language) &&
        Version.Equals(other.Version);

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is CompressedMetaPointer pointer && Equals(pointer);

    /// <inheritdoc cref="Equals(LionWeb.Core.M1.CompressedMetaPointer)"/>
    public static bool operator ==(CompressedMetaPointer left, CompressedMetaPointer right) =>
        left.Equals(right);

    /// <inheritdoc cref="Equals(LionWeb.Core.M1.CompressedMetaPointer)"/>
    public static bool operator !=(CompressedMetaPointer left, CompressedMetaPointer right) =>
        !(left == right);
}

/// Convenience methods to deal with byte arrays.
internal static class CompressedElement
{
    internal static byte[] AsBytes(string str) =>
        Encoding.ASCII.GetBytes(str);

    internal static bool Equals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) =>
        left.SequenceEqual(right);
}