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

namespace LionWeb.Core.Serialization;

using System.Text;
using TargetNode = NodeId;
using CommandId = NodeId;
using ParticipationId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;
using EventSequenceNumber = long;

public record CommandSource(ParticipationId ParticipationId, CommandId CommandId);

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
    ProtocolMessage? Message { get; }
}

#region Query

public interface IDeltaQuery : IDeltaContent
{
    QueryId QueryId { get; }
}

public interface IDeltaQueryRequest : IDeltaQuery;

public interface IDeltaQueryResponse : IDeltaQuery;

public abstract record DeltaQueryBase(QueryId QueryId, ProtocolMessage? Message)
{
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(QueryId));
        builder.Append(" = ");
        builder.Append(QueryId);
        builder.Append(", ");

        builder.Append(nameof(Message));
        builder.Append(" = ");
        builder.Append(Message);

        return true;
    }
};

public record SubscribePartitionsRequest(
    bool Creation,
    bool Deletion,
    bool Partitions,
    QueryId QueryId,
    ProtocolMessage? Message) : DeltaQueryBase(QueryId, Message), IDeltaQuery;

public record SubscribePartitionsResponse(QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryResponse;

public record SubscribePartitionRequest(TargetNode Partition, QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryRequest;

public record SubscribePartitionResponse(DeltaSerializationChunk Contents, QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryResponse;

public record UnsubscribePartitionRequest(TargetNode Partition, QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryRequest;

public record UnsubscribePartitionResponse(QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryResponse;

public record GetAvailableIdsRequest(int count, QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryRequest;

public record GetAvailableIdsResponse(FreeId[] Ids, QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryResponse
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

        return base.Equals(other) && Ids.SequenceEqual(other.Ids);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var id in Ids)
        {
            hashCode.Add(id);
        }

        return hashCode.ToHashCode();
    }

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);

        builder.Append(", ");
        builder.Append(nameof(Ids));
        builder.Append(" = [");
        bool first = true;
        foreach (var id in Ids)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            builder.Append(id);
        }

        builder.Append(']');

        return true;
    }
}

#endregion

#region Command

public interface IDeltaCommand : IDeltaContent
{
    CommandId? CommandId { get; }
}

public interface ISingleDeltaCommand : IDeltaCommand
{
    CommandId CommandId { get; }
}

public record CommandResponse(CommandId CommandId, ProtocolMessage? Message) : IDeltaContent;

#region Partitions

public interface IPartitionCommand : ISingleDeltaCommand;

public record AddPartition(DeltaSerializationChunk NewPartition, CommandId CommandId, ProtocolMessage? Message)
    : IPartitionCommand;

public record DeletePartition(TargetNode DeletedPartition, CommandId CommandId, ProtocolMessage? Message)
    : IPartitionCommand;

#endregion

#region Nodes

public interface INodeCommand : ISingleDeltaCommand;

public record ChangeClassifier(
    TargetNode Node,
    MetaPointer NewClassifier,
    CommandId CommandId,
    ProtocolMessage? Message) : INodeCommand;

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
    ProtocolMessage? Message)
    : IPropertyCommand;

public record DeleteProperty(TargetNode Parent, MetaPointer Property, CommandId CommandId, ProtocolMessage? Message)
    : IPropertyCommand;

public record ChangeProperty(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    CommandId CommandId,
    ProtocolMessage? Message) : IPropertyCommand;

#endregion

#region Children

public interface IContainmentCommand : IFeatureCommand;

public record AddChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record DeleteChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    CommandId CommandId,
    ProtocolMessage? Message)
    : IContainmentCommand;

public record ReplaceChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record MoveChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record MoveChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record MoveChildInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record MoveAndReplaceChildInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

#endregion

#region Annotations

public interface IAnnotationCommand : ISingleDeltaCommand;

public record AddAnnotation(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record DeleteAnnotation(TargetNode Parent, Index Index, CommandId CommandId, ProtocolMessage? Message)
    : IAnnotationCommand;

public record ReplaceAnnotation(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record MoveAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record MoveAnnotationInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record MoveAndReplaceAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record MoveAndReplaceAnnotationInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

#endregion

#region References

public interface IReferenceCommand : IFeatureCommand;

public record AddReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record DeleteReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    CommandId CommandId,
    ProtocolMessage? Message)
    : IReferenceCommand;

public record ChangeReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record MoveEntryFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record MoveEntryFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record MoveEntryInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record MoveAndReplaceEntryFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record MoveAndReplaceEntryFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record MoveAndReplaceEntryInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record AddReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record DeleteReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    CommandId CommandId,
    ProtocolMessage? Message)
    : IReferenceCommand;

public record ChangeReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record AddReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record DeleteReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    CommandId CommandId,
    ProtocolMessage? Message)
    : IReferenceCommand;

public record ChangeReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

#endregion

public record CompositeCommand(ISingleDeltaCommand[] Commands, ProtocolMessage? Message)
    : IDeltaCommand
{
    public CommandId? CommandId => null;

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

        return Commands.SequenceEqual(other.Commands) && Equals(Message, other.Message);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var command in Commands)
        {
            hashCode.Add(command);
        }

        hashCode.Add(Message);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
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
        builder.Append("], ");

        builder.Append(nameof(Message));
        builder.Append(" = ");
        builder.Append(Message);
        builder.Append(", ");

        return true;
    }
}

#endregion

#region Event

public interface IDeltaEvent : IDeltaContent
{
    EventSequenceNumber EventSequenceNumber { get; }
}

public interface ISingleDeltaEvent : IDeltaEvent
{
    CommandSource[] OriginCommands { get; }
}

public abstract record SingleDeltaEventBase(
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message)
    : ISingleDeltaEvent
{
    public virtual bool Equals(SingleDeltaEventBase? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return OriginCommands.SequenceEqual(other.OriginCommands) &&
               Equals(Message, other.Message);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var command in OriginCommands)
        {
            hashCode.Add(command);
        }

        hashCode.Add(Message);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(EventSequenceNumber));
        builder.Append(" = ");
        builder.Append(EventSequenceNumber);
        builder.Append(", ");

        builder.Append(nameof(OriginCommands));
        builder.Append(" = [");
        bool first = true;
        foreach (var command in OriginCommands)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            builder.Append(command);
        }
        
        builder.Append("], ");
        builder.Append(nameof(Message));
        builder.Append(" = ");
        builder.Append(Message);

        return true;
    }
}

#region Partitions

public interface IPartitionEvent : ISingleDeltaEvent;

public record PartitionAdded(
    DeltaSerializationChunk NewPartition,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IPartitionEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewPartition));
        builder.Append(" = ");
        builder.Append(NewPartition);
        
        return true;
    }
}

public record PartitionDeleted(
    DeltaSerializationChunk DeletedPartition,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IPartitionEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(DeletedPartition));
        builder.Append(" = ");
        builder.Append(DeletedPartition);
        
        return true;
    }
}

#endregion

#region Nodes

public interface INodeEvent : ISingleDeltaEvent;

public record ClassifierChanged(
    TargetNode Node,
    MetaPointer NewClassifier,
    MetaPointer OldClassifier,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), INodeEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Node));
        builder.Append(" = ");
        builder.Append(Node);
        
        builder.Append(", ");
        builder.Append(nameof(NewClassifier));
        builder.Append(" = ");
        builder.Append(NewClassifier);
        
        builder.Append(", ");
        builder.Append(nameof(OldClassifier));
        builder.Append(" = ");
        builder.Append(OldClassifier);
        
        return true;
    }
    
}

#endregion

public interface IFeatureEvent : ISingleDeltaEvent
{
    TargetNode Parent { get; }
    MetaPointer Feature { get; }
};

#region Properties

public interface IPropertyEvent : IFeatureEvent
{
    TargetNode Parent { get; }
    MetaPointer Property { get; }

    MetaPointer IFeatureEvent.Feature => Property;
};

public record PropertyAdded(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IPropertyEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Property));
        builder.Append(" = ");
        builder.Append(Property);
        
        builder.Append(", ");
        builder.Append(nameof(NewValue));
        builder.Append(" = ");
        builder.Append(NewValue);
        
        return true;
    }
}

public record PropertyDeleted(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue OldValue,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IPropertyEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Property));
        builder.Append(" = ");
        builder.Append(Property);
        
        builder.Append(", ");
        builder.Append(nameof(OldValue));
        builder.Append(" = ");
        builder.Append(OldValue);
        
        return true;
    }
    
}

public record PropertyChanged(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    PropertyValue OldValue,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IPropertyEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Property));
        builder.Append(" = ");
        builder.Append(Property);
        
        builder.Append(", ");
        builder.Append(nameof(NewValue));
        builder.Append(" = ");
        builder.Append(NewValue);
        
        builder.Append(", ");
        builder.Append(nameof(OldValue));
        builder.Append(" = ");
        builder.Append(OldValue);
        
        return true;
    }
}

#endregion

#region Children

public interface IContainmentEvent : IFeatureEvent
{
    MetaPointer Containment { get; }

    MetaPointer IFeatureEvent.Feature => Containment;
};

public record ChildAdded(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewChild));
        builder.Append(" = ");
        builder.Append(NewChild);
        
        return true;
    }
}

public record ChildDeleted(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk DeletedChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(DeletedChild));
        builder.Append(" = ");
        builder.Append(DeletedChild);
        
        return true;
    }
}

public record ChildReplaced(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    DeltaSerializationChunk ReplacedChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewChild));
        builder.Append(" = ");
        builder.Append(NewChild);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        
        return true;
    }
}

public record ChildMovedFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode OldParent,
    MetaPointer OldContainment,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IContainmentEvent.Containment => NewContainment;
    
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        
        builder.Append(", ");
        builder.Append(nameof(NewContainment));
        builder.Append(" = ");
        builder.Append(NewContainment);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        
        builder.Append(", ");
        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        
        builder.Append(", ");
        builder.Append(nameof(OldContainment));
        builder.Append(" = ");
        builder.Append(OldContainment);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        return true;
    }
}

public record ChildMovedFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer OldContainment,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    MetaPointer IContainmentEvent.Containment => NewContainment;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewContainment));
        builder.Append(" = ");
        builder.Append(NewContainment);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(OldContainment));
        builder.Append(" = ");
        builder.Append(OldContainment);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        return true;
    }
}

public record ChildMovedInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        return true;
    }
}

public record ChildMovedAndReplacedFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode OldParent,
    MetaPointer OldContainment,
    Index OldIndex,
    DeltaSerializationChunk ReplacedChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IContainmentEvent.Containment => NewContainment;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        
        builder.Append(", ");
        builder.Append(nameof(NewContainment));
        builder.Append(" = ");
        builder.Append(NewContainment);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        
        builder.Append(", ");
        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        
        builder.Append(", ");
        builder.Append(nameof(OldContainment));
        builder.Append(" = ");
        builder.Append(OldContainment);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        
        return true;
    }
}

public record ChildMovedAndReplacedFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer OldContainment,
    Index OldIndex,
    DeltaSerializationChunk ReplacedChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    MetaPointer IContainmentEvent.Containment => NewContainment;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewContainment));
        builder.Append(" = ");
        builder.Append(NewContainment);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(OldContainment));
        builder.Append(" = ");
        builder.Append(OldContainment);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        
        return true;
    }
}

public record ChildMovedAndReplacedInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index OldIndex,
    DeltaSerializationChunk ReplacedChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IContainmentEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedChild));
        builder.Append(" = ");
        builder.Append(MovedChild);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Containment));
        builder.Append(" = ");
        builder.Append(Containment);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedChild));
        builder.Append(" = ");
        builder.Append(ReplacedChild);
        
        return true;
    }
}

#endregion

#region Annotations

public interface IAnnotationEvent : ISingleDeltaEvent
{
    TargetNode Parent { get; }
};

public record AnnotationAdded(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IAnnotationEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewAnnotation));
        builder.Append(" = ");
        builder.Append(NewAnnotation);
        
        return true;
    }
}

public record AnnotationDeleted(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk DeletedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IAnnotationEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(DeletedAnnotation));
        builder.Append(" = ");
        builder.Append(DeletedAnnotation);
        
        return true;
    }
}

public record AnnotationReplaced(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    DeltaSerializationChunk ReplacedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IAnnotationEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewAnnotation));
        builder.Append(" = ");
        builder.Append(NewAnnotation);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedAnnotation));
        builder.Append(" = ");
        builder.Append(ReplacedAnnotation);
        
        return true;
    }
}

public record AnnotationMovedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => NewParent;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedAnnotation));
        builder.Append(" = ");
        builder.Append(MovedAnnotation);
        
        builder.Append(", ");
        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        return true;
    }
}

public record AnnotationMovedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IAnnotationEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedAnnotation));
        builder.Append(" = ");
        builder.Append(MovedAnnotation);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        return true;
    }
}

public record AnnotationMovedAndReplacedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    DeltaSerializationChunk ReplacedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => NewParent;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedAnnotation));
        builder.Append(" = ");
        builder.Append(MovedAnnotation);
        
        builder.Append(", ");
        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedAnnotation));
        builder.Append(" = ");
        builder.Append(ReplacedAnnotation);
        
        return true;
    }
}

public record AnnotationMovedAndReplacedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    DeltaSerializationChunk ReplacedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IAnnotationEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedAnnotation));
        builder.Append(" = ");
        builder.Append(MovedAnnotation);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedAnnotation));
        builder.Append(" = ");
        builder.Append(ReplacedAnnotation);
        
        return true;
    }
}

#endregion

#region References

public interface IReferenceEvent : IFeatureEvent
{
    MetaPointer Reference { get; }

    MetaPointer IFeatureEvent.Feature => Reference;
};

public record ReferenceAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewTarget));
        builder.Append(" = ");
        builder.Append(NewTarget);
        
        return true;
    }
}

public record ReferenceDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget DeletedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(DeletedTarget));
        builder.Append(" = ");
        builder.Append(DeletedTarget);
        
        return true;
    }
}

public record ReferenceChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    SerializedReferenceTarget ReplacedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewTarget));
        builder.Append(" = ");
        builder.Append(NewTarget);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedTarget));
        builder.Append(" = ");
        builder.Append(ReplacedTarget);
        
        return true;
    }
}

public record EntryMovedFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IReferenceEvent.Reference => NewReference;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        
        builder.Append(", ");
        builder.Append(nameof(NewReference));
        builder.Append(" = ");
        builder.Append(NewReference);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        
        builder.Append(", ");
        builder.Append(nameof(OldReference));
        builder.Append(" = ");
        builder.Append(OldReference);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedEntry));
        builder.Append(" = ");
        builder.Append(MovedEntry);
        
        return true;
    }
}

public record EntryMovedFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    MetaPointer IReferenceEvent.Reference => NewReference;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(NewReference));
        builder.Append(" = ");
        builder.Append(NewReference);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(OldReference));
        builder.Append(" = ");
        builder.Append(OldReference);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedEntry));
        builder.Append(" = ");
        builder.Append(MovedEntry);
        
        return true;
    }
}

public record EntryMovedInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedEntry));
        builder.Append(" = ");
        builder.Append(MovedEntry);
        
        return true;
    }
}

public record EntryMovedAndReplacedFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    SerializedReferenceTarget ReplacedEntry,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IReferenceEvent.Reference => NewReference;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(NewParent));
        builder.Append(" = ");
        builder.Append(NewParent);
        
        builder.Append(", ");
        builder.Append(nameof(NewReference));
        builder.Append(" = ");
        builder.Append(NewReference);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(OldParent));
        builder.Append(" = ");
        builder.Append(OldParent);
        
        builder.Append(", ");
        builder.Append(nameof(OldReference));
        builder.Append(" = ");
        builder.Append(OldReference);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedEntry));
        builder.Append(" = ");
        builder.Append(MovedEntry);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedEntry));
        builder.Append(" = ");
        builder.Append(ReplacedEntry);
        
        return true;
    }
}

public record EntryMovedAndReplacedFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    SerializedReferenceTarget ReplacedEntry,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    MetaPointer IReferenceEvent.Reference => NewReference;

    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(NewReference));
        builder.Append(" = ");
        builder.Append(NewReference);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(OldReference));
        builder.Append(" = ");
        builder.Append(OldReference);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedEntry));
        builder.Append(" = ");
        builder.Append(MovedEntry);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedEntry));
        builder.Append(" = ");
        builder.Append(ReplacedEntry);
        
        return true;
    }
}

public record EntryMovedAndReplacedInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    SerializedReferenceTarget ReplacedEntry,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(NewIndex));
        builder.Append(" = ");
        builder.Append(NewIndex);
        
        builder.Append(", ");
        builder.Append(nameof(OldIndex));
        builder.Append(" = ");
        builder.Append(OldIndex);
        
        builder.Append(", ");
        builder.Append(nameof(MovedEntry));
        builder.Append(" = ");
        builder.Append(MovedEntry);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedEntry));
        builder.Append(" = ");
        builder.Append(ReplacedEntry);
        
        return true;
    }
}

public record ReferenceResolveInfoAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode Target,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewResolveInfo));
        builder.Append(" = ");
        builder.Append(NewResolveInfo);
        
        builder.Append(", ");
        builder.Append(nameof(Target));
        builder.Append(" = ");
        builder.Append(Target);
        
        return true;
    }
}

public record ReferenceResolveInfoDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode Target,
    ResolveInfo DeletedResolveInfo,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(Target));
        builder.Append(" = ");
        builder.Append(Target);
        
        builder.Append(", ");
        builder.Append(nameof(DeletedResolveInfo));
        builder.Append(" = ");
        builder.Append(DeletedResolveInfo);
        
        return true;
    }
}

public record ReferenceResolveInfoChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode? Target,
    ResolveInfo ReplacedResolveInfo,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewResolveInfo));
        builder.Append(" = ");
        builder.Append(NewResolveInfo);
        
        builder.Append(", ");
        builder.Append(nameof(Target));
        builder.Append(" = ");
        builder.Append(Target);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedResolveInfo));
        builder.Append(" = ");
        builder.Append(ReplacedResolveInfo);
        
        return true;
    }
}

public record ReferenceTargetAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo ResolveInfo,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewTarget));
        builder.Append(" = ");
        builder.Append(NewTarget);
        
        builder.Append(", ");
        builder.Append(nameof(ResolveInfo));
        builder.Append(" = ");
        builder.Append(ResolveInfo);
        
        return true;
    }
}

public record ReferenceTargetDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo ResolveInfo,
    TargetNode DeletedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(ResolveInfo));
        builder.Append(" = ");
        builder.Append(ResolveInfo);
        
        builder.Append(", ");
        builder.Append(nameof(DeletedTarget));
        builder.Append(" = ");
        builder.Append(DeletedTarget);
        
        return true;
    }
}

public record ReferenceTargetChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo? ResolveInfo,
    TargetNode ReplacedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message), IReferenceEvent
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(Parent));
        builder.Append(" = ");
        builder.Append(Parent);
        
        builder.Append(", ");
        builder.Append(nameof(Reference));
        builder.Append(" = ");
        builder.Append(Reference);
        
        builder.Append(", ");
        builder.Append(nameof(Index));
        builder.Append(" = ");
        builder.Append(Index);
        
        builder.Append(", ");
        builder.Append(nameof(NewTarget));
        builder.Append(" = ");
        builder.Append(NewTarget);
        
        builder.Append(", ");
        builder.Append(nameof(ResolveInfo));
        builder.Append(" = ");
        builder.Append(ResolveInfo);
        
        builder.Append(", ");
        builder.Append(nameof(ReplacedTarget));
        builder.Append(" = ");
        builder.Append(ReplacedTarget);
        
        return true;
    }
}

#endregion

#region Miscellaneous

public record CompositeEvent(
    ISingleDeltaEvent[] Events,
    EventSequenceNumber EventSequenceNumber,
    ProtocolMessage? Message)
    : IDeltaEvent
{
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

        return Events.SequenceEqual(other.Events) && Equals(Message, other.Message);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var @event in Events)
        {
            hashCode.Add(@event);
        }

        hashCode.Add(Message);
        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Events));
        builder.Append(" = [");
        bool first = true;
        foreach (var node in Events)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            builder.Append(node);
        }
        builder.Append("], ");
        
        builder.Append(nameof(EventSequenceNumber));
        builder.Append(" = ");
        builder.Append(EventSequenceNumber);
        builder.Append(", ");
        
        builder.Append(nameof(Message));
        builder.Append(" = ");
        builder.Append(Message);
        
        return true;
    }
}

public record NoOpEvent(
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message)
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        return true;
    }
}

public record Error(
    string ErrorCode,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage Message)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, Message)
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        
        builder.Append(", ");
        builder.Append(nameof(ErrorCode));
        builder.Append(" = ");
        builder.Append(ErrorCode);
        
        return true;
    }
}

#endregion

#endregion