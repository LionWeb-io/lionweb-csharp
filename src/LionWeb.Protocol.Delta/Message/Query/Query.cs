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
namespace LionWeb.Protocol.Delta.Message.Query;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

public interface IDeltaQuery : IDeltaContent
{
    QueryId QueryId { get; }
}

public interface IDeltaQueryRequest : IDeltaQuery;

public interface IDeltaQueryResponse : IDeltaQuery;

public abstract record DeltaQueryBase(
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaContentBase(ProtocolMessages)
{
    /// <inheritdoc />
    [JsonIgnore]
    public override string Id => QueryId;

    /// <inheritdoc />
    public virtual bool Equals(DeltaQueryBase? other)
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
               string.Equals(QueryId, other.QueryId, StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(QueryId, StringComparer.InvariantCulture);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(QueryId));
        builder.Append(" = ");
        builder.Append(QueryId);

        return true;
    }
}

#region Subscription

public interface IDeltaQuerySubscription : IDeltaQuery;

#region SubscribeToChangingPartitions

public record SubscribeToChangingPartitionsRequest(
    bool Creation,
    bool Deletion,
    bool Partitions,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQuerySubscription, IDeltaQueryRequest;

public record SubscribeToChangingPartitionsResponse(
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQuerySubscription, IDeltaQueryResponse;

#endregion

#region SubscribeToPartitionContents

public record SubscribeToPartitionContentsRequest(
    TargetNode Partition,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQuerySubscription, IDeltaQueryRequest;

public record SubscribeToPartitionContentsResponse(
    DeltaSerializationChunk Contents,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQuerySubscription, IDeltaQueryResponse;

#endregion

#region UnsubscribeFromPartitionContents

public record UnsubscribeFromPartitionContentsRequest(
    TargetNode Partition,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQuerySubscription, IDeltaQueryRequest;

public record UnsubscribeFromPartitionContentsResponse(
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQuerySubscription, IDeltaQueryResponse;

#endregion

#endregion

#region Participation

public interface IDeltaQueryParticipation : IDeltaQuery;

#region SignOn

public record SignOnRequest(
    string DeltaProtocolVersion,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryParticipation, IDeltaQueryRequest
{
    public bool RequiresParticipationId => false;
}

public record SignOnResponse(
    ParticipationId ParticipationId,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryParticipation, IDeltaQueryResponse;

#endregion

#region SignOff

public record SignOffRequest(
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryParticipation, IDeltaQueryRequest;

public record SignOffResponse(
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryParticipation, IDeltaQueryResponse;

#endregion

#region Reconnect

public record ReconnectRequest(
    ParticipationId ParticipationId,
    EventSequenceNumber LastReceivedSequenceNumber,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryParticipation, IDeltaQueryRequest;

public record ReconnectResponse(
    EventSequenceNumber LastSentSequenceNumber,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryParticipation, IDeltaQueryResponse;

#endregion

#endregion

#region Miscellaneous

public interface IDeltaQueryMiscellaneous : IDeltaQuery;

#region GetAvailableIds

public record GetAvailableIdsRequest(
    int count,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryMiscellaneous, IDeltaQueryRequest;

public record GetAvailableIdsResponse(
    FreeId[] Ids,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryMiscellaneous, IDeltaQueryResponse
{
    /// <inheritdoc />
    public virtual bool Equals(GetAvailableIdsResponse? other)
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
               Ids.ArrayEquals(other.Ids);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.ArrayHashCode(Ids);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(Ids));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Ids);

        return true;
    }
}

#endregion

#region ListPartitions

public record ListPartitionsRequest(
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryMiscellaneous, IDeltaQueryRequest;

public record ListPartitionsResponse(
    DeltaSerializationChunk Partitions,
    QueryId QueryId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaQueryBase(QueryId, ProtocolMessages), IDeltaQueryMiscellaneous, IDeltaQueryResponse;

#endregion

#endregion