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

namespace LionWeb.Protocol.Delta.Message.Command;

using Core.Serialization;

public interface IPartitionDeltaCommand : IDeltaCommand;

#region Nodes

public interface INodeCommand : IPartitionDeltaCommand;

public record ChangeClassifier(
    TargetNode Node,
    MetaPointer NewClassifier,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), INodeCommand;

#endregion

public interface IFeatureCommand : IPartitionDeltaCommand;

#region Properties

public interface IPropertyCommand : IFeatureCommand
{
    TargetNode Node { get; }

    MetaPointer Property { get; }
};

public record AddProperty(
    TargetNode Node,
    MetaPointer Property,
    PropertyValue NewValue,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IPropertyCommand;

public record DeleteProperty(
    TargetNode Node,
    MetaPointer Property,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IPropertyCommand;

public record ChangeProperty(
    TargetNode Node,
    MetaPointer Property,
    PropertyValue NewValue,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IPropertyCommand;

#endregion

#region Children

public interface IContainmentCommand : IFeatureCommand;

public record AddChild(
    TargetNode Parent,
    DeltaSerializationChunk NewChild,
    MetaPointer Containment,
    Index Index,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record DeleteChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    TargetNode DeletedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record ReplaceChild(
    DeltaSerializationChunk NewChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    TargetNode ReplacedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveChildInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode ReplacedChild,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode ReplacedChild,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

public record MoveAndReplaceChildInSameContainment(
    Index NewIndex,
    TargetNode ReplacedChild,
    TargetNode MovedChild,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IContainmentCommand;

#endregion

#region Annotations

public interface IAnnotationCommand : IPartitionDeltaCommand;

public record AddAnnotation(
    TargetNode Parent,
    DeltaSerializationChunk NewAnnotation,
    Index Index,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record DeleteAnnotation(
    TargetNode Parent,
    Index Index,
    TargetNode DeletedAnnotation,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record ReplaceAnnotation(
    DeltaSerializationChunk NewAnnotation,
    TargetNode Parent,
    Index Index,
    TargetNode ReplacedAnnotation,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAnnotationInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAndReplaceAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode ReplacedAnnotation,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

public record MoveAndReplaceAnnotationInSameParent(
    Index NewIndex,
    TargetNode ReplacedAnnotation,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IAnnotationCommand;

#endregion

#region References

public interface IReferenceCommand : IFeatureCommand;

public record AddReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? NewTarget,
    ResolveInfo? NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record DeleteReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? DeletedTarget,
    ResolveInfo? DeletedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record ChangeReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? OldTarget,
    ResolveInfo? OldResolveInfo,
    TargetNode? NewTarget,
    ResolveInfo? NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveEntryFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveEntryFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveEntryInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index NewIndex,
    Index OldIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveAndReplaceEntryFromOtherReference(
    TargetNode NewParent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode? ReplacedTarget,
    ResolveInfo? ReplacedResolveInfo,
    TargetNode OldParent,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveAndReplaceEntryFromOtherReferenceInSameParent(
    TargetNode Parent,
    MetaPointer NewReference,
    Index NewIndex,
    TargetNode? ReplacedTarget,
    ResolveInfo? ReplacedResolveInfo,
    MetaPointer OldReference,
    Index OldIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record MoveAndReplaceEntryInSameReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index OldIndex,
    TargetNode? MovedTarget,
    ResolveInfo? MovedResolveInfo,
    Index NewIndex,
    TargetNode? ReplacedTarget,
    ResolveInfo? ReplacedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record AddReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record DeleteReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo DeletedResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record ChangeReferenceResolveInfo(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    ResolveInfo OldResolveInfo,
    ResolveInfo NewResolveInfo,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record AddReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record DeleteReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode DeletedTarget,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

public record ChangeReferenceTarget(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode OldTarget,
    TargetNode NewTarget,
    CommandId CommandId,
    ProtocolMessage[]? ProtocolMessages
) : DeltaCommandBase(CommandId, ProtocolMessages), IReferenceCommand;

#endregion

