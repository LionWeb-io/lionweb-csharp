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

// ReSharper disable CoVariantArrayConversion

namespace LionWeb.Protocol.Delta.Message;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

public sealed class DeltaProtocolVersion
{
    private readonly string _version;

    private DeltaProtocolVersion(string version)
    {
        _version = version;
    }

    public static DeltaProtocolVersion? v2025_1 = new("2025.1");

    public string Version => _version;

    public override string ToString() => _version;
}

public record DeltaSerializationChunk(SerializedNode[] Nodes)
{
    /// <inheritdoc />
    public virtual bool Equals(DeltaSerializationChunk? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Nodes.ArrayEquals(other.Nodes);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.ArrayHashCode(Nodes);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Nodes));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Nodes);

        return true;
    }
}

public record AdditionalInfo(MessageKind Kind, string Message, AdditionalInfoData[]? Data)
{
    /// <inheritdoc />
    public virtual bool Equals(AdditionalInfo? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Kind, other.Kind, StringComparison.InvariantCulture) &&
               string.Equals(Message, other.Message, StringComparison.InvariantCulture) &&
               Data.ArrayEquals(other.Data);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Kind, StringComparer.InvariantCulture);
        hashCode.Add(Message, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(Data);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Kind));
        builder.Append(" = ");
        builder.Append(Kind);
        builder.Append(", ");

        builder.Append(nameof(Message));
        builder.Append(" = ");
        builder.Append(Message);
        builder.Append(", ");

        builder.Append(nameof(Data));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Data);

        return true;
    }
}

public record AdditionalInfoData(AdditionalInfoDataKey Key, string Value)
{
    /// <inheritdoc />
    public virtual bool Equals(AdditionalInfoData? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Key, other.Key, StringComparison.InvariantCulture) &&
               string.Equals(Value, other.Value, StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Key, StringComparer.InvariantCulture);
        hashCode.Add(Value, StringComparer.InvariantCulture);
        return hashCode.ToHashCode();
    }
}

public interface IDeltaError
{
    ErrorCode ErrorCode { get; }
    string Message { get; }
}

public interface IDeltaContent
{
    AdditionalInfo[]? AdditionalInfos { get; }

    [JsonIgnore]
    ParticipationId InternalParticipationId { get; set; }

    [JsonIgnore]
    public bool RequiresParticipationId => true;

    [JsonIgnore]
    string Id { get; }
}

public abstract record DeltaContentBase(AdditionalInfo[]? AdditionalInfos) : IDeltaContent
{
    [JsonIgnore]
    public ParticipationId InternalParticipationId { get; set; }

    [JsonIgnore]
    public abstract string Id { get; }

    public virtual AdditionalInfo[]? AdditionalInfos { get; init; } = AdditionalInfos;

    /// <inheritdoc />
    public virtual bool Equals(DeltaContentBase? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return AdditionalInfos.ArrayEquals(other.AdditionalInfos);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.ArrayHashCode(AdditionalInfos);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(AdditionalInfos));
        builder.Append(" = ");
        builder.ArrayPrintMembers(AdditionalInfos);

        return true;
    }
}