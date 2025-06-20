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

namespace LionWeb.Core.Serialization.Delta;

using System.Text;
using TargetNode = NodeId;
using CommandId = NodeId;
using ParticipationId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;
using EventSequenceNumber = long;

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

        return Nodes.SequenceEqual(other.Nodes);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var node in Nodes)
        {
            hashCode.Add(node);
        }

        return HashCode.Combine(Nodes);
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Nodes));
        builder.Append(" = [");
        bool first = true;
        foreach (var node in Nodes)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            builder.Append(node);
        }

        builder.Append(']');

        return true;
    }
}

public record ProtocolMessage(MessageKind Kind, string Message, ProtocolMessageData[] Data)
{
    /// <inheritdoc />
    public virtual bool Equals(ProtocolMessage? other)
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
               Data.SequenceEqual(other.Data);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Kind, StringComparer.InvariantCulture);
        hashCode.Add(Message, StringComparer.InvariantCulture);
        foreach (var data in Data)
        {
            hashCode.Add(data);
        }

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
        builder.Append(" = [");
        bool first = true;
        foreach (var data in Data)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            builder.Append(data);
        }

        builder.Append(']');

        return true;
    }
}

public record ProtocolMessageData(MessageDataKey Key, string Value);

public interface IDeltaContent
{
    ProtocolMessage[] ProtocolMessages { get; }
    ParticipationId InternalParticipationId { get; set; }
    public bool RequiresParticipationId => true;

    string Id { get; }
}

public abstract record DeltaContentBase(ProtocolMessage[] ProtocolMessages) : IDeltaContent
{
    public ParticipationId InternalParticipationId { get; set; }
    public abstract string Id { get; }
    
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

        return ProtocolMessages.SequenceEqual(other.ProtocolMessages);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var message in ProtocolMessages)
        {
            hashCode.Add(message);
        }

        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(ProtocolMessages));
        builder.Append(" = [");
        bool first = true;
        foreach (var message in ProtocolMessages)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            builder.Append(message);
        }

        builder.Append(']');

        return true;
    }
}