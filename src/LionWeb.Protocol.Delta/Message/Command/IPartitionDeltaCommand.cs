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
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), INodeCommand;

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
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IPropertyCommand;

public record DeleteProperty(
    TargetNode Node,
    MetaPointer Property,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IPropertyCommand;

public record ChangeProperty(
    TargetNode Node,
    MetaPointer Property,
    PropertyValue NewValue,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IPropertyCommand;

#endregion

#region Children

public interface IContainmentCommand : IFeatureCommand;

public record AddChild(
    TargetNode Parent,
    DeltaSerializationChunk NewChild,
    MetaPointer Containment,
    Index Index,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record DeleteChild(
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    TargetNode DeletedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record ReplaceChild(
    DeltaSerializationChunk NewChild,
    TargetNode Parent,
    MetaPointer Containment,
    Index Index,
    TargetNode ReplacedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record MoveChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record MoveChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record MoveChildInSameContainment(
    Index NewIndex,
    TargetNode MovedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainment(
    TargetNode NewParent,
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode ReplacedChild,
    TargetNode MovedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record MoveAndReplaceChildFromOtherContainmentInSameParent(
    MetaPointer NewContainment,
    Index NewIndex,
    TargetNode ReplacedChild,
    TargetNode MovedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

public record MoveAndReplaceChildInSameContainment(
    Index NewIndex,
    TargetNode ReplacedChild,
    TargetNode MovedChild,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IContainmentCommand;

#endregion

#region Annotations

public interface IAnnotationCommand : IPartitionDeltaCommand;

public record AddAnnotation(
    TargetNode Parent,
    DeltaSerializationChunk NewAnnotation,
    Index Index,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IAnnotationCommand;

public record DeleteAnnotation(
    TargetNode Parent,
    Index Index,
    TargetNode DeletedAnnotation,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IAnnotationCommand;

public record ReplaceAnnotation(
    DeltaSerializationChunk NewAnnotation,
    TargetNode Parent,
    Index Index,
    TargetNode ReplacedAnnotation,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IAnnotationCommand;

public record MoveAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IAnnotationCommand;

public record MoveAnnotationInSameParent(
    Index NewIndex,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IAnnotationCommand;

public record MoveAndReplaceAnnotationFromOtherParent(
    TargetNode NewParent,
    Index NewIndex,
    TargetNode ReplacedAnnotation,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IAnnotationCommand;

public record MoveAndReplaceAnnotationInSameParent(
    Index NewIndex,
    TargetNode ReplacedAnnotation,
    TargetNode MovedAnnotation,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IAnnotationCommand;

#endregion

#region References

public interface IReferenceCommand : IFeatureCommand;

public record AddReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? NewReference,
    ResolveInfo? NewResolveInfo,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IReferenceCommand;

public record DeleteReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? DeletedReference,
    ResolveInfo? DeletedResolveInfo,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IReferenceCommand;

public record ChangeReference(
    TargetNode Parent,
    MetaPointer Reference,
    Index Index,
    TargetNode? OldReference,
    ResolveInfo? OldResolveInfo,
    TargetNode? NewReference,
    ResolveInfo? NewResolveInfo,
    CommandId CommandId,
    AdditionalInfo[]? AdditionalInfos
) : DeltaCommandBase(CommandId, AdditionalInfos), IReferenceCommand;

#endregion

