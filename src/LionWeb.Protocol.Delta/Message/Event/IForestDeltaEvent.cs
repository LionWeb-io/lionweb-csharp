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

namespace LionWeb.Protocol.Delta.Message.Event;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

public interface IForestDeltaEvent : IDeltaEvent;

public record PartitionAdded(
    DeltaSerializationChunk NewPartition,
    TargetNode AffectedNode,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IForestDeltaEvent
{
    [JsonIgnore]
    public TargetNode AffectedNode { get; init; } = AffectedNode;

    /// <inheritdoc />
    [JsonIgnore]
    public override HashSet<TargetNode> AffectedNodes => [AffectedNode];

    /// <inheritdoc />
    public virtual bool Equals(PartitionAdded? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && NewPartition.Equals(other.NewPartition);
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(base.GetHashCode(), NewPartition);
}

public record PartitionDeleted(
    TargetNode DeletedPartition,
    TargetNode[] DeletedDescendants,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IForestDeltaEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public override HashSet<TargetNode> AffectedNodes => [DeletedPartition];

    /// <inheritdoc />
    public virtual bool Equals(PartitionDeleted? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) &&
               string.Equals(DeletedPartition, other.DeletedPartition, StringComparison.InvariantCulture) &&
               DeletedDescendants.ArrayEquals(other.DeletedDescendants);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(DeletedPartition, StringComparer.InvariantCulture);
        hashCode.ArrayHashCode(DeletedDescendants);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(DeletedPartition));
        builder.Append(" = ");
        builder.Append(DeletedPartition);
        builder.Append(", ");

        builder.Append(nameof(DeletedDescendants));
        builder.Append(" = ");
        builder.ArrayPrintMembers(DeletedDescendants);

        return true;
    }
}