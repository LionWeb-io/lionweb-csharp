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
using Utilities;

/// Checks for duplicate node ids in an efficient manner.
public class DuplicateIdChecker
{
    private readonly HashSet<ICompressedId> _knownIds = new();

    /// Whether <paramref name="compressedId"/> has been seen before by this instance.
    public bool IsIdDuplicate(ICompressedId compressedId) =>
        !_knownIds.Add(compressedId);
}

/// <summary>
/// A potentially compressed id.
/// Implementation differ in their memory vs. computation tradeoffs.
/// </summary>
public interface ICompressedId
{
    /// The original node id, if available.
    public string? Original { get; }

    /// <summary>
    /// Creates either a new <see cref="CompressedId"/> or <see cref="UncompressedId"/>.
    /// </summary>
    /// <param name="id">Node id to compress.</param>
    /// <param name="config">Whether to store the uncompressed original. Uses more memory, but eases debugging.</param>
    /// <returns>The newly created compressed id.</returns>
    public static ICompressedId Create(string id, CompressedIdConfig config)
    {
        if (config.Compress)
        {
            var sha1 = SHA1.Create();
            var idHash = sha1.ComputeHash(ByteExtensions.AsAsciiBytes(id));
            return new CompressedId(idHash, config.KeepOriginal ? id : null);
        }

        return new UncompressedId(id);
    }

    /// <param name="id">Id to compress.</param>
    /// <param name="keepOriginal">Whether we keep the original around.</param>
    [Obsolete(message: "Use Create(string id, CompressedIdConfig config) instead.")]
    public static ICompressedId Create(string id, bool keepOriginal) =>
        Create(id, new CompressedIdConfig(KeepOriginal: keepOriginal));
}

/// <summary>
/// Configuration which optimizations to apply to (potentially compressed) ids.
/// </summary>
public record CompressedIdConfig
{
    /// <summary>
    /// Configuration which optimizations to apply to (potentially compressed) ids.
    /// </summary>
    /// <param name="Compress">
    /// Whether we compress ids at all; defaults to <c>false</c>.
    /// If set to <c>false</c>, <paramref name="KeepOriginal"/> MUST be <c>null</c> or <c>true</c>.
    /// </param>
    /// <param name="KeepOriginal">
    /// Whether we keep the original around for compressed ids; defaults to <c>false</c>.
    /// Uses more memory, but eases debugging.
    /// </param>
    public CompressedIdConfig(bool Compress = false, bool? KeepOriginal = null)
    {
        if (!Compress && KeepOriginal is false)
            throw new ArgumentException($"If we don't {nameof(Compress)}, we MUST {nameof(KeepOriginal)}");
        this.Compress = Compress;
        this.KeepOriginal = KeepOriginal ?? false;
    }

    /// <summary>Whether we compress ids at all; defaults to <c>false</c>.</summary>
    public bool Compress { get; init; }

    /// <summary>Whether we keep the original around for compressed ids; defaults to <c>false</c>. Uses more memory, but eases debugging.</summary>
    public bool KeepOriginal { get; init; }

    public void Deconstruct(out bool Compress, out bool KeepOriginal)
    {
        Compress = this.Compress;
        KeepOriginal = this.KeepOriginal;
    }
}

/// <summary>
/// An uncompressed id that always stores its original.
///
/// <para>
/// More memory efficient for models with typical ids longer than 20 characters,
/// but a bit faster.
/// </para>
/// </summary>
/// <param name="original">Original, uncompressed id.</param>
public readonly struct UncompressedId(string original) : ICompressedId, IEquatable<UncompressedId>
{
    /// <inheritdoc />
    public string Original => original;

    /// <inheritdoc />
    public override string ToString() =>
        Original;

    /// <inheritdoc />
    public override int GetHashCode() =>
        Original.GetHashCode();


    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is UncompressedId other && Equals(other);

    /// <inheritdoc />
    public bool Equals(UncompressedId other) =>
        Original.Equals(other.Original);

    /// <inheritdoc cref="Equals(UncompressedId)"/>
    public static bool operator ==(UncompressedId left, UncompressedId right) =>
        left.Equals(right);

    /// <inheritdoc cref="Equals(UncompressedId)"/>
    public static bool operator !=(UncompressedId left, UncompressedId right) =>
        !left.Equals(right);
}

/// <summary>
/// Stores a LionWeb node id in a compact format, optionally preserving the original.
///
/// <para>
/// If used without storing the original more memory efficient, but a bit slower.
/// </para>
/// </summary>
public readonly struct CompressedId : ICompressedId, IEquatable<CompressedId>
{
    internal CompressedId(byte[] identifier, string? original)
    {
        Identifier = identifier;
        Original = original;
    }

    private byte[] Identifier { get; }

    /// The original node id, if available.
    public string? Original { get; }

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
        ByteExtensions.Equals(Identifier, other.Identifier);

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
    private CompressedMetaPointer(ICompressedId language, ICompressedId version, ICompressedId key,
        MetaPointer? original)
    {
        Language = language;
        Version = version;
        Key = key;
        Original = original;
    }

    /// The MetaPointer's language in compressed format.
    public ICompressedId Language { get; }

    /// The MetaPointer's version in compressed format.
    public ICompressedId Version { get; }

    /// The MetaPointer's key in compressed format.
    public ICompressedId Key { get; }

    /// The original MetaPointer, if available.
    public MetaPointer? Original { get; }

    /// <summary>
    /// Creates a new <see cref="CompressedMetaPointer"/>.
    /// </summary>
    /// <param name="metaPointer">MetaPointer id to compress.</param>
    /// <param name="config">Whether to store the uncompressed original. Uses more memory, but eases debugging.</param>
    /// <returns>The newly created compressed MetaPointer.</returns>
    public static CompressedMetaPointer Create(MetaPointer metaPointer, CompressedIdConfig config) =>
        new(
            ICompressedId.Create(metaPointer.Language, config),
            ICompressedId.Create(metaPointer.Version, config),
            ICompressedId.Create(metaPointer.Key, config),
            config.KeepOriginal ? metaPointer : null
        );

    /// <summary>
    /// Creates a new <see cref="CompressedMetaPointer"/>.
    /// </summary>
    /// <param name="metaPointer">MetaPointer id to compress.</param>
    /// <param name="keepOriginal">Whether to store the uncompressed original. Uses more memory, but eases debugging.</param>
    /// <returns>The newly created compressed MetaPointer.</returns>
    [Obsolete(message: "Use Create(string id, CompressedIdConfig config) instead.")]
    public static CompressedMetaPointer Create(MetaPointer metaPointer, bool keepOriginal) =>
        Create(metaPointer, new CompressedIdConfig(KeepOriginal: keepOriginal));

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