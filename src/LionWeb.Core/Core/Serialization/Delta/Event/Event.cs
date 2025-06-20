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

namespace LionWeb.Core.Serialization.Delta.Event;

using System.Text;
using TargetNode = NodeId;
using CommandId = NodeId;
using ParticipationId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;
using EventSequenceNumber = long;
using ErrorCode = NodeId;

public record CommandSource(ParticipationId ParticipationId, CommandId CommandId);

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
    ProtocolMessage[] ProtocolMessages)
    : DeltaContentBase(ProtocolMessages), ISingleDeltaEvent
{
    /// <inheritdoc />
    public override string Id => string.Join("__", OriginCommands.Select(x => x.ToString()));

    /// <inheritdoc />
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

        return base.Equals(other) && EventSequenceNumber == other.EventSequenceNumber && OriginCommands.SequenceEqual(other.OriginCommands);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(EventSequenceNumber);
        foreach (var command in OriginCommands)
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
        builder.Append(']');

        return true;
    }
}

#region Partitions

public interface IPartitionDeltaEvent : ISingleDeltaEvent;

public record PartitionAdded(
    DeltaSerializationChunk NewPartition,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IPartitionDeltaEvent;

public record PartitionDeleted(
    DeltaSerializationChunk DeletedPartition,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IPartitionDeltaEvent;

#endregion

#region Nodes

public interface INodeEvent : ISingleDeltaEvent;

public record ClassifierChanged(
    TargetNode Node,
    MetaPointer NewClassifier,
    MetaPointer OldClassifier,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), INodeEvent;

#endregion

public interface IFeatureEvent : ISingleDeltaEvent
{
    TargetNode Parent { get; }
    MetaPointer Feature { get; }
};

#region Properties

public interface IPropertyEvent : IFeatureEvent
{
    MetaPointer Property { get; }

    /// <inheritdoc />
    MetaPointer IFeatureEvent.Feature => Property;
};

public record PropertyAdded(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IPropertyEvent;

public record PropertyDeleted(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue OldValue,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IPropertyEvent;

public record PropertyChanged(
    TargetNode Parent,
    MetaPointer Property,
    PropertyValue NewValue,
    PropertyValue OldValue,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IPropertyEvent;

#endregion

#region Children

public interface IContainmentEvent : IFeatureEvent
{
    MetaPointer Containment { get; }

    /// <inheritdoc />
    MetaPointer IFeatureEvent.Feature => Containment;
};

public record ChildAdded(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IContainmentEvent;

public record ChildDeleted(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk DeletedChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IContainmentEvent;

public record ChildReplaced(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    DeltaSerializationChunk NewChild,
    DeltaSerializationChunk ReplacedChild,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IContainmentEvent;

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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
    MetaPointer IContainmentEvent.Containment => NewContainment;
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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    MetaPointer IContainmentEvent.Containment => NewContainment;
}

public record ChildMovedInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IContainmentEvent;

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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
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
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IContainmentEvent
{
    /// <inheritdoc />
    MetaPointer IContainmentEvent.Containment => NewContainment;
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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IContainmentEvent;

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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IAnnotationEvent;

public record AnnotationDeleted(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk DeletedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IAnnotationEvent;

public record AnnotationReplaced(
    TargetNode Parent,
    Index Index,
    DeltaSerializationChunk NewAnnotation,
    DeltaSerializationChunk ReplacedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IAnnotationEvent;

public record AnnotationMovedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IAnnotationEvent
{
    /// <inheritdoc />
    TargetNode IAnnotationEvent.Parent => NewParent;
}

public record AnnotationMovedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IAnnotationEvent;

public record AnnotationMovedAndReplacedFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode OldParent,
    Index OldIndex,
    DeltaSerializationChunk ReplacedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IAnnotationEvent
{
    /// <inheritdoc />
    TargetNode IAnnotationEvent.Parent => NewParent;
}

public record AnnotationMovedAndReplacedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    DeltaSerializationChunk ReplacedAnnotation,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages),
    IAnnotationEvent;

#endregion

#region References

public interface IReferenceEvent : IFeatureEvent
{
    MetaPointer Reference { get; }

    /// <inheritdoc />
    MetaPointer IFeatureEvent.Feature => Reference;
};

public record ReferenceAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget DeletedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    SerializedReferenceTarget NewTarget,
    SerializedReferenceTarget ReplacedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
    MetaPointer IReferenceEvent.Reference => NewReference;
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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    MetaPointer IReferenceEvent.Reference => NewReference;
}

public record EntryMovedInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

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
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    public TargetNode Parent => NewParent;

    /// <inheritdoc />
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
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages) : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent
{
    /// <inheritdoc />
    MetaPointer IReferenceEvent.Reference => NewReference;
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
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceResolveInfoAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode Target,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceResolveInfoDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode Target,
    ResolveInfo DeletedResolveInfo,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceResolveInfoChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    TargetNode? Target,
    ResolveInfo ReplacedResolveInfo,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceTargetAdded(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo ResolveInfo,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceTargetDeleted(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo ResolveInfo,
    TargetNode DeletedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

public record ReferenceTargetChanged(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    ResolveInfo? ResolveInfo,
    TargetNode ReplacedTarget,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages), IReferenceEvent;

#endregion

#region Miscellaneous

public record CompositeEvent(
    ISingleDeltaEvent[] Events,
    EventSequenceNumber EventSequenceNumber,
    ProtocolMessage[] ProtocolMessages)
    : DeltaContentBase(ProtocolMessages), IDeltaEvent
{
    /// <inheritdoc />
    public override string Id => string.Join("--", Events.Select(e => e.Id));

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

        return base.Equals(other) && Events.SequenceEqual(other.Events) && EventSequenceNumber == other.EventSequenceNumber;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        foreach (var @event in Events)
        {
            hashCode.Add(@event);
        }
        hashCode.Add(EventSequenceNumber);

        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", ");
        
        builder.Append(nameof(Events));
        builder.Append(" = [");
        bool firstEvent = true;
        foreach (var node in Events)
        {
            if (!firstEvent)
            {
                builder.Append(", ");
            }

            firstEvent = false;
            builder.Append(node);
        }

        builder.Append("], ");

        builder.Append(nameof(EventSequenceNumber));
        builder.Append(" = ");
        builder.Append(EventSequenceNumber);

        return true;
    }
}

public record NoOpEvent(
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages);

public record Error(
    ErrorCode ErrorCode,
    EventSequenceNumber EventSequenceNumber,
    CommandSource[] OriginCommands,
    ProtocolMessage[] ProtocolMessages)
    : SingleDeltaEventBase(EventSequenceNumber, OriginCommands, ProtocolMessages);

#endregion
