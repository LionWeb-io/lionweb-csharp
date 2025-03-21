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

using TargetNode = NodeId;
using CommandId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;

public record CommandSource(NodeId Source);

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

public abstract record DeltaQueryBase(QueryId QueryId, ProtocolMessage? Message);

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
}

#endregion

#region Command

public interface IDeltaCommand : IDeltaContent;

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
}

#endregion

#region Event

public interface IDeltaEvent : IDeltaContent;

public interface ISingleDeltaEvent : IDeltaEvent
{
    CommandSource[] OriginCommands { get; }
}

public abstract record SingleDeltaEventBase(CommandSource[] OriginCommands, ProtocolMessage? Message)
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
}

#region Partitions

public interface IPartitionEvent : ISingleDeltaEvent;

public record PartitionAdded(
    DeltaSerializationChunk NewPartition,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IPartitionEvent;

public record PartitionDeleted(
    DeltaSerializationChunk DeletedPartition,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IPartitionEvent;

#endregion

#region Nodes

public interface INodeEvent : ISingleDeltaEvent;

public record ClassifierChanged(
    TargetNode Node,
    MetaPointer NewClassifier,
    MetaPointer OldClassifier,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), INodeEvent;

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
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IPropertyEvent;

public record PropertyDeleted(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue OldValue,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IPropertyEvent;

public record PropertyChanged(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    PropertyValue OldValue,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IPropertyEvent;

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
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent;

public record ChildDeleted(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk DeletedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent;

public record ChildReplaced(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    DeltaSerializationChunk ReplacedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent;

public record ChildMovedFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode OldParent,
    MetaPointer OldContainment,
    Index OldIndex,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IContainmentEvent.Containment => NewContainment;
}

public record ChildMovedFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer OldContainment,
    Index OldIndex,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent
{
    MetaPointer IContainmentEvent.Containment => NewContainment;
}

public record ChildMovedInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index OldIndex,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent;

public record ChildMovedAndReplacedFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode OldParent,
    MetaPointer OldContainment,
    Index OldIndex,
    DeltaSerializationChunk ReplacedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IContainmentEvent.Containment => NewContainment;
}

public record ChildMovedAndReplacedFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer OldContainment,
    Index OldIndex,
    DeltaSerializationChunk ReplacedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent
{
    MetaPointer IContainmentEvent.Containment => NewContainment;
}

public record ChildMovedAndReplacedInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index OldIndex,
    DeltaSerializationChunk ReplacedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IContainmentEvent;

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
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IAnnotationEvent;

public record AnnotationDeleted(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk DeletedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IAnnotationEvent;

public record AnnotationReplaced(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    DeltaSerializationChunk ReplacedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IAnnotationEvent;

public record AnnotationMovedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => NewParent;
}

public record AnnotationMovedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IAnnotationEvent;

public record AnnotationMovedAndReplacedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    DeltaSerializationChunk ReplacedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => NewParent;
}

public record AnnotationMovedAndReplacedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    DeltaSerializationChunk ReplacedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IAnnotationEvent;

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
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget DeletedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    SerializedReferenceTarget ReplacedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record EntryMovedFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent
{
    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record EntryMovedAndReplacedFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    SerializedReferenceTarget ReplacedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent
{
    public TargetNode Parent => NewParent;

    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedAndReplacedFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    SerializedReferenceTarget ReplacedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent
{
    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedAndReplacedInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    SerializedReferenceTarget ReplacedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceResolveInfoAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode Target,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceResolveInfoDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode Target,
    ResolveInfo DeletedResolveInfo,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceResolveInfoChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode? Target,
    ResolveInfo ReplacedResolveInfo,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceTargetAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo ResolveInfo,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceTargetDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo ResolveInfo,
    TargetNode DeletedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

public record ReferenceTargetChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo? ResolveInfo,
    TargetNode ReplacedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : SingleDeltaEventBase(OriginCommands, Message), IReferenceEvent;

#endregion

#region Miscellaneous

public record CompositeEvent(ISingleDeltaEvent[] Events, ProtocolMessage? Message)
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
}

public record NoOpEvent(CommandSource[] OriginCommands, ProtocolMessage? Message)
    : SingleDeltaEventBase(OriginCommands, Message);

public record Error(string ErrorCode, CommandSource[] OriginCommands, ProtocolMessage Message)
    : SingleDeltaEventBase(OriginCommands, Message);

#endregion

#endregion