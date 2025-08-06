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

namespace LionWeb.Protocol.Delta.Message.Event;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

public record CommandSource(ParticipationId ParticipationId, CommandId CommandId);

public interface IEvent : IDeltaContent
{
    public const EventSequenceNumber DefaultEventSequenceNumber = -1;
    
    EventSequenceNumber SequenceNumber { get; set; }

    CommandSource[]? OriginCommands { get; }
}

public abstract record DeltaEventBase(
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaContentBase(ProtocolMessages), IEvent
{
    /// <inheritdoc />
    [JsonIgnore]
    public override string Id => string.Join("__", OriginCommands.Select(x => x.ToString()));

    /// <inheritdoc />
    public virtual EventSequenceNumber SequenceNumber { get; set; } = IEvent.DefaultEventSequenceNumber;

    /// <inheritdoc />
    public virtual CommandSource[]? OriginCommands { get; init; } = OriginCommands;

    /// <inheritdoc />
    public virtual bool Equals(DeltaEventBase? other)
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
               OriginCommands.ArrayEquals(other.OriginCommands);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.ArrayHashCode(OriginCommands);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(SequenceNumber));
        builder.Append(" = ");
        builder.Append(SequenceNumber);
        builder.Append(", ");

        builder.Append(nameof(OriginCommands));
        builder.Append(" = ");
        builder.ArrayPrintMembers(OriginCommands);

        return true;
    }
}

#region Miscellaneous

public record CompositeEvent : DeltaEventBase, IEvent
{
    public CompositeEvent(IEvent[] Parts,
        ProtocolMessage[]? ProtocolMessages) : base(null, ProtocolMessages)
    {
        this.Parts = Parts;
    }

    /// <inheritdoc />
    [JsonIgnore]
    public override string Id => string.Join("--", Parts.Select(e => e.Id));

    public IEvent[] Parts { get; init; }

    /// <inheritdoc />
    [JsonIgnore]
    public override EventSequenceNumber SequenceNumber { get; set; } = IEvent.DefaultEventSequenceNumber;

    /// <inheritdoc />
    [JsonIgnore]
    public override CommandSource[]? OriginCommands => 
        Parts.SelectMany(e => e.OriginCommands ?? []).ToArray();

    /// <inheritdoc />
    public virtual bool Equals(CompositeEvent? other)
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
               Parts.ArrayEquals(other.Parts) &&
               SequenceNumber == other.SequenceNumber;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.ArrayHashCode(Parts);
        hashCode.Add(SequenceNumber);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        // do NOT call base.PrintMembers(), as we omit OriginCommands
        
        builder.Append(nameof(ProtocolMessages));
        builder.Append(" = ");
        builder.ArrayPrintMembers(ProtocolMessages);
        
        builder.Append(", ");

        builder.Append(nameof(SequenceNumber));
        builder.Append(" = ");
        builder.Append(SequenceNumber);
        builder.Append(", ");

        builder.Append(nameof(Parts));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Parts);

        return true;
    }
}

public record NoOpEvent(
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IEvent;

public record Error(
    ErrorCode ErrorCode,
    string Message,
    CommandSource[]? OriginCommands,
    ProtocolMessage[]? ProtocolMessages) : DeltaEventBase(OriginCommands, ProtocolMessages), IEvent;

#endregion