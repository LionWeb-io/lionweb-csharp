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

namespace LionWeb.Protocol.Delta.Message.Command;

using Core.Serialization;
using System.Text;
using System.Text.Json.Serialization;

public interface IDeltaCommand : IDeltaContent
{
    CommandId? CommandId { get; }
}

public abstract record DeltaCommandBase(
    CommandId? CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaContentBase(ProtocolMessages), IDeltaCommand
{
    /// <inheritdoc />
    [JsonIgnore]
    public override string Id => CommandId;

    /// <inheritdoc />
    public virtual bool Equals(DeltaCommandBase? other)
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
               string.Equals(CommandId, other.CommandId, StringComparison.InvariantCulture);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(CommandId, StringComparer.InvariantCulture);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");

        builder.Append(nameof(CommandId));
        builder.Append(" = ");
        builder.Append(CommandId);

        return true;
    }
}

public record CompositeCommand(
    IDeltaCommand[] Parts,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IDeltaCommand
{
    /// <inheritdoc />
    public virtual bool Equals(CompositeCommand? other)
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
               Parts.ArrayEquals(other.Parts);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.ArrayHashCode(Parts);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");
        builder.Append(nameof(Parts));
        builder.Append(" = ");
        builder.ArrayPrintMembers(Parts);

        return true;
    }
}