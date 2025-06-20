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

namespace LionWeb.Core.Serialization.Delta.Command;

using System.Text;
using TargetNode = NodeId;
using CommandId = NodeId;
using ParticipationId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;
using EventSequenceNumber = long;

public interface IDeltaCommand : IDeltaContent
{
    CommandId? CommandId { get; }
}

public abstract record DeltaCommandBase(CommandId? CommandId, ProtocolMessage[] ProtocolMessages) : DeltaContentBase(ProtocolMessages), IDeltaCommand
{
    /// <inheritdoc />
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

        return base.Equals(other) && string.Equals(CommandId, other.CommandId, StringComparison.InvariantCulture);
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

public interface ISingleDeltaCommand : IDeltaCommand
{
    new CommandId CommandId { get; }
    CommandId? IDeltaCommand.CommandId => CommandId;
}

public record CommandResponse(CommandId CommandId, ProtocolMessage[] ProtocolMessages) : DeltaContentBase(ProtocolMessages)
{
    /// <inheritdoc />
    public override string Id => CommandId;
}

#region Partitions

public interface IPartitionCommand : ISingleDeltaCommand;

public record AddPartition(DeltaSerializationChunk NewPartition, CommandId CommandId, ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IPartitionCommand;

public record DeletePartition(TargetNode DeletedPartition, CommandId CommandId, ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IPartitionCommand;

#endregion

#region Nodes

public interface INodeCommand : ISingleDeltaCommand;

public record ChangeClassifier(
    TargetNode Node,
    MetaPointer NewClassifier,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), INodeCommand;

#endregion

public interface IFeatureCommand : ISingleDeltaCommand;

#region Properties

public interface IPropertyCommand : IFeatureCommand
{
    TargetNode Parent { get; }
    MetaPointer Property { get; }
};

public record AddProperty(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IPropertyCommand;

public record DeleteProperty(TargetNode Parent, MetaPointer Property, CommandId CommandId, ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IPropertyCommand;

public record ChangeProperty(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IPropertyCommand;

#endregion

#region Children

public interface IContainmentCommand : IFeatureCommand;

public record AddChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record DeleteChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record ReplaceChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveChildInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveAndReplaceChildInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

#endregion

#region Annotations

public interface IAnnotationCommand : ISingleDeltaCommand;

public record AddAnnotation(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record DeleteAnnotation(TargetNode Parent, Index Index, CommandId CommandId, ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record ReplaceAnnotation(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAnnotationInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAndReplaceAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAndReplaceAnnotationInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

#endregion

#region References

public interface IReferenceCommand : IFeatureCommand;

public record AddReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record DeleteReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record ChangeReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveEntryFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveEntryFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveEntryInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveAndReplaceEntryFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveAndReplaceEntryFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveAndReplaceEntryInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record AddReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record DeleteReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record ChangeReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record AddReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record DeleteReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record ChangeReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage[] ProtocolMessages) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

#endregion

public record CompositeCommand(ISingleDeltaCommand[] Commands, ProtocolMessage[] ProtocolMessages)
    : DeltaCommandBase(null, ProtocolMessages)
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

        return base.Equals(other) && Commands.SequenceEqual(other.Commands);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        foreach (var command in Commands)
        {
            hashCode.Add(command);
        }

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");
        builder.Append(nameof(Commands));
        builder.Append(" = [");
        bool first = true;
        foreach (var command in Commands)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            builder.Append(command);
        }

        builder.Append(']');

        return true;
    }
}