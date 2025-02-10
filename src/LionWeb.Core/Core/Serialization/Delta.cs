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

using TargetNode = M1.CompressedId;
using CommandId = M1.CompressedId;
using QueryId = M1.CompressedId;
using FreeId = string;
using MessageKind = string;
using MessageDataKey = string;

public record CommandSource();

public record DeltaSerializationChunk(List<SerializedNode> Nodes);

public record ProtocolMessage(MessageKind Kind, string message, List<ProtocolMessageData> Data);

public record ProtocolMessageData(MessageDataKey Key, string Value);

public record DeltaProperty(TargetNode Parent, MetaPointer Property);

public record DeltaContainment(TargetNode Parent, MetaPointer Containment, Index Index);

public record DeltaAnnotation(TargetNode Parent, Index Index);

public record DeltaReference(TargetNode Parent, MetaPointer Reference, Index Index);

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

public record GetAvailableIdsResponse(List<FreeId> Ids, QueryId QueryId, ProtocolMessage? Message)
    : DeltaQueryBase(QueryId, Message), IDeltaQueryResponse;

#endregion

#region Command

public interface IDeltaCommand : IDeltaContent
{
    CommandId CommandId { get; }
}

#region Partitions

public interface IPartitionCommand : IDeltaCommand;

public record AddPartition(DeltaSerializationChunk NewPartition, CommandId CommandId, ProtocolMessage? Message)
    : IPartitionCommand;

public record DeletePartition(TargetNode DeletedPartition, CommandId CommandId, ProtocolMessage? Message)
    : IPartitionCommand;

#endregion

#region Nodes

public interface INodeCommand : IDeltaCommand;

public record ChangeClassifier(
    TargetNode Node,
    MetaPointer NewClassifier,
    CommandId CommandId,
    ProtocolMessage? Message) : INodeCommand;

#endregion

public interface IFeatureCommand : IDeltaCommand;

#region Properties

public interface IPropertyCommand : IFeatureCommand
{
    DeltaProperty Property { get; }

    TargetNode Parent => Property.Parent;
};

public record AddProperty(DeltaProperty Property, PropertyValue NewValue, CommandId CommandId, ProtocolMessage? Message)
    : IPropertyCommand;

public record DeleteProperty(DeltaProperty Property, CommandId CommandId, ProtocolMessage? Message) : IPropertyCommand;

public record ChangeProperty(
    DeltaProperty Property,
    PropertyValue NewValue,
    CommandId CommandId,
    ProtocolMessage? Message) : IPropertyCommand;

#endregion

#region Children

public interface IContainmentCommand : IFeatureCommand;

public record AddChild(
    DeltaContainment Containment,
    DeltaSerializationChunk NewChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record DeleteChild(DeltaContainment Containment, CommandId CommandId, ProtocolMessage? Message)
    : IContainmentCommand;

public record ReplaceChild(
    DeltaContainment Containment,
    DeltaSerializationChunk NewChild,
    CommandId CommandId,
    ProtocolMessage? Message) : IContainmentCommand;

public record MoveChildFromOtherContainment(
    DeltaContainment NewContainment,
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
    DeltaContainment NewContainment,
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

public interface IAnnotationCommand : IDeltaCommand;

public record AddAnnotation(
    DeltaAnnotation Parent,
    DeltaSerializationChunk NewAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record DeleteAnnotation(DeltaAnnotation Parent, CommandId CommandId, ProtocolMessage? Message)
    : IAnnotationCommand;

public record ReplaceAnnotation(
    DeltaAnnotation Parent,
    DeltaSerializationChunk NewAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record MoveAnnotationFromOtherParent(
    DeltaAnnotation NewParent,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record MoveAnnotationInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage? Message) : IAnnotationCommand;

public record MoveAndReplaceAnnotationFromOtherParent(
    DeltaAnnotation NewParent,
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
    DeltaReference Reference,
    SerializedReferenceTarget NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record DeleteReference(DeltaReference Reference, CommandId CommandId, ProtocolMessage? Message)
    : IReferenceCommand;

public record ChangeReference(
    DeltaReference Reference,
    SerializedReferenceTarget NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record MoveEntryFromOtherReference(
    DeltaReference NewReference,
    DeltaReference OldReference,
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
    DeltaReference NewReference,
    DeltaReference OldReference,
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
    DeltaReference Reference,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record DeleteReferenceResolveInfo(DeltaReference Reference, CommandId CommandId, ProtocolMessage? Message)
    : IReferenceCommand;

public record ChangeReferenceResolveInfo(
    DeltaReference Reference,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record AddReferenceTarget(
    DeltaReference Reference,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

public record DeleteReferenceTarget(DeltaReference Reference, CommandId CommandId, ProtocolMessage? Message)
    : IReferenceCommand;

public record ChangeReferenceTarget(
    DeltaReference Reference,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage? Message) : IReferenceCommand;

#endregion

public record CompositeCommand(IDeltaCommand[] Commands, CommandId CommandId, ProtocolMessage? Message)
    : IReferenceCommand;

#endregion

#region Event

public interface IDeltaEvent : IDeltaContent
{
    CommandSource[] OriginCommands { get; }
};

#region Partitions

public interface IPartitionEvent : IDeltaEvent;

public record PartitionAdded(
    DeltaSerializationChunk NewPartition,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IPartitionEvent;

public record PartitionDeleted(
    DeltaSerializationChunk DeletedPartition,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IPartitionEvent;

#endregion

#region Nodes

public interface INodeEvent : IDeltaEvent;

public record ClassifierChanged(
    TargetNode Node,
    MetaPointer NewClassifier,
    MetaPointer OldClassifier,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : INodeEvent;

#endregion

public interface IFeatureEvent : IDeltaEvent
{
    TargetNode Parent { get; }
    MetaPointer Feature { get; }
};

#region Properties

public interface IPropertyEvent : IFeatureEvent
{
    DeltaProperty Property { get; }

    TargetNode IFeatureEvent.Parent => Property.Parent;

    MetaPointer IFeatureEvent.Feature => Property.Property;
};

public record PropertyAdded(
    DeltaProperty Property,
    PropertyValue NewValue,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IPropertyEvent;

public record PropertyDeleted(
    DeltaProperty Property,
    PropertyValue OldValue,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IPropertyEvent;

public record PropertyChanged(
    DeltaProperty Property,
    PropertyValue NewValue,
    PropertyValue OldValue,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IPropertyEvent;

#endregion

#region Children

public interface IContainmentEvent : IFeatureEvent
{
    MetaPointer Containment { get; }

    MetaPointer IFeatureEvent.Feature => Containment;
};

public record ChildAdded(
    DeltaContainment Containment,
    DeltaSerializationChunk NewChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IContainmentEvent
{
    public TargetNode Parent => Containment.Parent;

    MetaPointer IContainmentEvent.Containment => Containment.Containment;
}

public record ChildDeleted(
    DeltaContainment Containment,
    DeltaSerializationChunk DeletedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IContainmentEvent
{
    public TargetNode Parent => Containment.Parent;

    MetaPointer IContainmentEvent.Containment => Containment.Containment;
}

public record RhildReplaced(
    DeltaContainment Containment,
    DeltaSerializationChunk NewChild,
    DeltaSerializationChunk ReplacedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IContainmentEvent
{
    public TargetNode Parent => Containment.Parent;

    MetaPointer IContainmentEvent.Containment => Containment.Containment;
}

public record ChildMovedFromOtherContainment(
    DeltaContainment NewContainment,
    TargetNode MovedChild,
    DeltaContainment OldContainment,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IContainmentEvent
{
    public TargetNode Parent => NewContainment.Parent;

    MetaPointer IContainmentEvent.Containment => NewContainment.Containment;
}

public record ChildMovedFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    TargetNode Parent,
    MetaPointer OldContainment,
    Index OldIndex,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IContainmentEvent
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
    ProtocolMessage? Message) : IContainmentEvent;

public record ChildMovedAndReplacedFromOtherContainment(
    DeltaContainment NewContainment,
    TargetNode MovedChild,
    DeltaContainment OldContainment,
    DeltaSerializationChunk ReplacedChild,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IContainmentEvent
{
    public TargetNode Parent => NewContainment.Parent;

    MetaPointer IContainmentEvent.Containment => NewContainment.Containment;
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
    ProtocolMessage? Message) : IContainmentEvent
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
    ProtocolMessage? Message) : IContainmentEvent;

#endregion

#region Annotations

public interface IAnnotationEvent : IDeltaEvent
{
    TargetNode Parent { get; }
};

public record AnnotationAdded(
    DeltaAnnotation Parent,
    DeltaSerializationChunk NewAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => Parent.Parent;
}

public record AnnotationDeleted(
    DeltaAnnotation Parent,
    DeltaSerializationChunk DeletedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => Parent.Parent;
}

public record AnnotationReplaced(
    DeltaAnnotation Parent,
    DeltaSerializationChunk NewAnnotation,
    DeltaSerializationChunk ReplacedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => Parent.Parent;
}

public record AnnotationMovedFromOtherParent(
    DeltaAnnotation NewParent,
    TargetNode MovedAnnotation,
    DeltaAnnotation OldParent,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => NewParent.Parent;
}

public record AnnotationMovedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IAnnotationEvent;

public record AnnotationMovedAndReplacedFromOtherParent(
    DeltaAnnotation NewParent,
    TargetNode MovedAnnotation,
    DeltaAnnotation OldParent,
    DeltaSerializationChunk ReplacedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IAnnotationEvent
{
    TargetNode IAnnotationEvent.Parent => NewParent.Parent;
}

public record AnnotationMovedAndReplacedInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    TargetNode Parent,
    Index OldIndex,
    DeltaSerializationChunk ReplacedAnnotation,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IAnnotationEvent;

#endregion

#region References

public interface IReferenceEvent : IFeatureEvent
{
    MetaPointer Reference { get; }

    MetaPointer IFeatureEvent.Feature => Reference;
};

public record ReferenceAdded(
    DeltaReference Reference,
    SerializedReferenceTarget NewTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record ReferenceDeleted(
    DeltaReference Reference,
    SerializedReferenceTarget DeletedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record ReferenceChanged(
    DeltaReference Reference,
    SerializedReferenceTarget NewTarget,
    SerializedReferenceTarget ReplacedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record EntryMovedFromOtherReference(
    DeltaReference NewReference,
    DeltaReference OldReference,
    SerializedReferenceTarget MovedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => NewReference.Parent;

    MetaPointer IReferenceEvent.Reference => NewReference.Reference;
}

public record EntryMovedFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    SerializedReferenceTarget MovedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
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
    ProtocolMessage? Message) : IReferenceEvent;

public record EntryMovedAndReplacedFromOtherReference(
    DeltaReference NewReference,
    DeltaReference OldReference,
    SerializedReferenceTarget MovedEntry,
    SerializedReferenceTarget ReplacedEntry,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => NewReference.Parent;

    MetaPointer IReferenceEvent.Reference => NewReference.Reference;
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
    ProtocolMessage? Message) : IReferenceEvent
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
    ProtocolMessage? Message) : IReferenceEvent;

public record ReferenceResolveInfoAdded(
    DeltaReference Reference,
    ResolveInfo NewResolveInfo,
    TargetNode Target,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record ReferenceResolveInfoDeleted(
    DeltaReference Reference,
    TargetNode Target,
    ResolveInfo DeletedResolveInfo,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record ReferenceResolveInfoChanged(
    DeltaReference Reference,
    ResolveInfo NewResolveInfo,
    TargetNode? Target,
    ResolveInfo ReplacedResolveInfo,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record ReferenceTargetAdded(
    DeltaReference Reference,
    TargetNode NewTarget,
    ResolveInfo ResolveInfo,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record ReferenceTargetDeleted(
    DeltaReference Reference,
    ResolveInfo ResolveInfo,
    TargetNode DeletedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

public record ReferenceTargetChanged(
    DeltaReference Reference,
    TargetNode NewTarget,
    ResolveInfo? ResolveInfo,
    TargetNode ReplacedTarget,
    CommandSource[] OriginCommands,
    ProtocolMessage? Message) : IReferenceEvent
{
    public TargetNode Parent => Reference.Parent;

    MetaPointer IReferenceEvent.Reference => Reference.Reference;
}

#endregion

#region Miscellaneous

public record CompositeEvent(IDeltaEvent[] Events, CommandSource[] OriginCommands, ProtocolMessage? Message)
    : IDeltaEvent;

public record NoOpEvent(CommandSource[] OriginCommands, ProtocolMessage? Message) : IDeltaEvent;

public record Error(string ErrorCode, CommandSource[] OriginCommands, ProtocolMessage Message) : IDeltaEvent;

#endregion

#endregion