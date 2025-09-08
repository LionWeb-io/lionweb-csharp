// MIT License
//
// Copyright (c) 2025 LionWeb Project and other contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// SPDX-FileCopyrightText: 2025 LionWeb Project and other contributors
// SPDX-License-Identifier: MIT

namespace LionWeb.Core.Serialization;

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json;

// Adjusted from https://github.com/evil-dr-nick/utf8jsonstreamreader
// Taken from https://stackoverflow.com/questions/54983533/parsing-a-json-file-with-net-core-3-0-system-text-json
// and fixed a few bugs with that

/// <inheritdoc cref="Utf8JsonReader"/>
public ref struct Utf8JsonStreamReader
{
    private readonly Stream _utf8JsonStream;

    // note: buffers will often be bigger than this - do not ever use this number for calculations.
    private readonly int _initialBufferSize;

    private SequenceSegment? _firstSegment;
    private int _firstSegmentStartIndex;
    private SequenceSegment? _lastSegment;
    private int _lastSegmentEndIndex;

    private Utf8JsonReader _jsonReader;
    private bool _keepBuffers;
    private bool _isFinalBlock;

    /// <inheritdoc cref="Utf8JsonReader()"/>
    public Utf8JsonStreamReader(Stream utf8JsonStream, int initialBufferSize)
    {
        _utf8JsonStream = utf8JsonStream;
        _initialBufferSize = initialBufferSize;

        _firstSegment = null;
        _firstSegmentStartIndex = 0;
        _lastSegment = null;
        _lastSegmentEndIndex = -1;

        _jsonReader = default;
        _keepBuffers = false;
        _isFinalBlock = false;
    }

    /// <inheritdoc cref="Utf8JsonReader.Read"/>
    public bool Read()
    {
        // read could be unsuccessful due to insufficient bufer size, retrying in loop with additional buffer segments
        while (!_jsonReader.Read())
        {
            if (_isFinalBlock)
                return false;

            MoveNext();
        }

        return true;
    }

    private void MoveNext()
    {
        _firstSegmentStartIndex += (int)_jsonReader.BytesConsumed;

        // release previous segments if possible
        while (_firstSegmentStartIndex > 0 && _firstSegment?.Memory.Length <= _firstSegmentStartIndex)
        {
            var currFirstSegment = _firstSegment;
            _firstSegmentStartIndex -= _firstSegment.Memory.Length;
            _firstSegment = (SequenceSegment?)_firstSegment.Next;
            if (!_keepBuffers)
            {
                currFirstSegment.Dispose();
            }
        }

        // create new segment
        var newSegment = new SequenceSegment(_initialBufferSize, _lastSegment);
        _lastSegment?.SetNext(newSegment);
        _lastSegment = newSegment;

        if (_firstSegment == null)
        {
            _firstSegment = newSegment;
            _firstSegmentStartIndex = 0;
        }

        // read data from stream
        _lastSegmentEndIndex = 0;
        int bytesRead;
        do
        {
            bytesRead = _utf8JsonStream.Read(newSegment.Buffer.Memory.Span.Slice(_lastSegmentEndIndex));
            _lastSegmentEndIndex += bytesRead;
        } while (bytesRead > 0 && _lastSegmentEndIndex < newSegment.Buffer.Memory.Length);

        _isFinalBlock = _lastSegmentEndIndex < newSegment.Buffer.Memory.Length;
        var data = new ReadOnlySequence<byte>(_firstSegment, _firstSegmentStartIndex, _lastSegment,
            _lastSegmentEndIndex);
        _jsonReader =
            new Utf8JsonReader(data, _isFinalBlock, _jsonReader.CurrentState);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DeserializePost()
    {
        // release memory if possible
        var firstSegment = _firstSegment;
        var firstSegmentStartIndex = _firstSegmentStartIndex + (int)_jsonReader.BytesConsumed;

        while (firstSegment?.Memory.Length < firstSegmentStartIndex)
        {
            firstSegmentStartIndex -= firstSegment.Memory.Length;
            firstSegment.Dispose();
            firstSegment = (SequenceSegment?)firstSegment.Next;
        }

        if (firstSegment != _firstSegment)
        {
            _firstSegment = firstSegment;
            _firstSegmentStartIndex = firstSegmentStartIndex;
            var data = new ReadOnlySequence<byte>(_firstSegment!, _firstSegmentStartIndex, _lastSegment!,
                _lastSegmentEndIndex);
            _jsonReader =
                new Utf8JsonReader(data, _isFinalBlock, _jsonReader.CurrentState);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long DeserializePre(out SequenceSegment? firstSegment, out int firstSegmentStartIndex)
    {
        // JsonSerializer.Deserialize can read only a single object. We have to extract
        // object to be deserialized into separate Utf8JsonReader. This incurs one additional
        // pass through data (but data is only passed, not parsed).
        var tokenStartIndex = _jsonReader.TokenStartIndex;
        firstSegment = _firstSegment;
        firstSegmentStartIndex = _firstSegmentStartIndex;

        // loop through data until end of object is found
        _keepBuffers = true;
        int depth = 0;

        if (TokenType == JsonTokenType.StartObject || TokenType == JsonTokenType.StartArray)
            depth++;

        while (depth > 0 && Read())
        {
            if (TokenType == JsonTokenType.StartObject || TokenType == JsonTokenType.StartArray)
                depth++;
            else if (TokenType == JsonTokenType.EndObject || TokenType == JsonTokenType.EndArray)
                depth--;
        }

        _keepBuffers = false;
        return tokenStartIndex;
    }

    /// <inheritdoc cref="JsonSerializer.Deserialize{T}(Stream, JsonSerializerOptions)"/>
    public T? Deserialize<T>(JsonSerializerOptions? options = null)
    {
        var tokenStartIndex = DeserializePre(out var firstSegment, out var firstSegmentStartIndex);

        var seq = new ReadOnlySequence<byte>(firstSegment!, firstSegmentStartIndex, _lastSegment!,
            _lastSegmentEndIndex).Slice(tokenStartIndex, _jsonReader.Position);
        var newJsonReader = new Utf8JsonReader(seq, true, default);

        // deserialize value
        var result = JsonSerializer.Deserialize<T>(ref newJsonReader, options);

        DeserializePost();
        return result;
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose() => _lastSegment?.Dispose();

    /// <inheritdoc cref="Utf8JsonReader.CurrentDepth"/>
    public int CurrentDepth => _jsonReader.CurrentDepth;

    /// <inheritdoc cref="Utf8JsonReader.HasValueSequence"/>
    public bool HasValueSequence => _jsonReader.HasValueSequence;

    /// <inheritdoc cref="Utf8JsonReader.TokenStartIndex"/>
    public long TokenStartIndex => _jsonReader.TokenStartIndex;

    /// <inheritdoc cref="Utf8JsonReader.TokenType"/>
    public JsonTokenType TokenType => _jsonReader.TokenType;

    /// <inheritdoc cref="Utf8JsonReader.ValueSequence"/>
    public ReadOnlySequence<byte> ValueSequence => _jsonReader.ValueSequence;

    /// <inheritdoc cref="Utf8JsonReader.ValueSpan"/>
    public ReadOnlySpan<byte> ValueSpan => _jsonReader.ValueSpan;

    /// <inheritdoc cref="Utf8JsonReader.GetBoolean"/>
    public bool GetBoolean() => _jsonReader.GetBoolean();

    /// <inheritdoc cref="Utf8JsonReader.GetByte"/>
    public byte GetByte() => _jsonReader.GetByte();

    /// <inheritdoc cref="Utf8JsonReader.GetBytesFromBase64"/>
    public byte[] GetBytesFromBase64() => _jsonReader.GetBytesFromBase64();

    /// <inheritdoc cref="Utf8JsonReader.GetComment"/>
    public string GetComment() => _jsonReader.GetComment();

    /// <inheritdoc cref="Utf8JsonReader.GetDateTime"/>
    public DateTime GetDateTime() => _jsonReader.GetDateTime();

    /// <inheritdoc cref="Utf8JsonReader.GetDateTimeOffset"/>
    public DateTimeOffset GetDateTimeOffset() => _jsonReader.GetDateTimeOffset();

    /// <inheritdoc cref="Utf8JsonReader.GetDecimal"/>
    public decimal GetDecimal() => _jsonReader.GetDecimal();

    /// <inheritdoc cref="Utf8JsonReader.GetDouble"/>
    public double GetDouble() => _jsonReader.GetDouble();

    /// <inheritdoc cref="Utf8JsonReader.GetGuid"/>
    public Guid GetGuid() => _jsonReader.GetGuid();

    /// <inheritdoc cref="Utf8JsonReader.GetInt16"/>
    public short GetInt16() => _jsonReader.GetInt16();

    /// <inheritdoc cref="Utf8JsonReader.GetInt32"/>
    public int GetInt32() => _jsonReader.GetInt32();

    /// <inheritdoc cref="Utf8JsonReader.GetInt64"/>
    public long GetInt64() => _jsonReader.GetInt64();

    /// <inheritdoc cref="Utf8JsonReader.GetSByte"/>
    public sbyte GetSByte() => _jsonReader.GetSByte();

    /// <inheritdoc cref="Utf8JsonReader.GetSingle"/>
    public float GetSingle() => _jsonReader.GetSingle();

    /// <inheritdoc cref="Utf8JsonReader.GetString"/>
    public string? GetString() => _jsonReader.GetString();

    /// <inheritdoc cref="Utf8JsonReader.GetUInt32"/>
    public uint GetUInt32() => _jsonReader.GetUInt32();

    /// <inheritdoc cref="Utf8JsonReader.GetUInt64"/>
    public ulong GetUInt64() => _jsonReader.GetUInt64();

    /// <inheritdoc cref="Utf8JsonReader.TryGetByte"/>
    public bool TryGetByte(out byte value) => _jsonReader.TryGetByte(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetBytesFromBase64"/>
    public bool TryGetBytesFromBase64(out byte[]? value) => _jsonReader.TryGetBytesFromBase64(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetDateTime"/>
    public bool TryGetDateTime(out DateTime value) => _jsonReader.TryGetDateTime(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetDateTimeOffset"/>
    public bool TryGetDateTimeOffset(out DateTimeOffset value) => _jsonReader.TryGetDateTimeOffset(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetDecimal"/>
    public bool TryGetDecimal(out decimal value) => _jsonReader.TryGetDecimal(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetDouble"/>
    public bool TryGetDouble(out double value) => _jsonReader.TryGetDouble(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetGuid"/>
    public bool TryGetGuid(out Guid value) => _jsonReader.TryGetGuid(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetInt16"/>
    public bool TryGetInt16(out short value) => _jsonReader.TryGetInt16(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetInt32"/>
    public bool TryGetInt32(out int value) => _jsonReader.TryGetInt32(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetInt64"/>
    public bool TryGetInt64(out long value) => _jsonReader.TryGetInt64(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetSByte"/>
    public bool TryGetSByte(out sbyte value) => _jsonReader.TryGetSByte(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetSingle"/>
    public bool TryGetSingle(out float value) => _jsonReader.TryGetSingle(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetUInt16"/>
    public bool TryGetUInt16(out ushort value) => _jsonReader.TryGetUInt16(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetUInt32"/>
    public bool TryGetUInt32(out uint value) => _jsonReader.TryGetUInt32(out value);

    /// <inheritdoc cref="Utf8JsonReader.TryGetUInt64"/>
    public bool TryGetUInt64(out ulong value) => _jsonReader.TryGetUInt64(out value);

    private sealed class SequenceSegment : ReadOnlySequenceSegment<byte>, IDisposable
    {
        internal IMemoryOwner<byte> Buffer { get; }
        private SequenceSegment? Previous { get; set; }
        private bool _disposed;

        public SequenceSegment(int size, SequenceSegment? previous)
        {
            Buffer = MemoryPool<byte>.Shared.Rent(size);
            Previous = previous;

            Memory = Buffer.Memory;
            RunningIndex = previous?.RunningIndex + previous?.Memory.Length ?? 0;
        }

        public void SetNext(SequenceSegment next) => Next = next;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Buffer.Dispose();
            Previous?.Dispose();
        }
    }
}