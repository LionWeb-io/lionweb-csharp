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

namespace LionWeb.Core.Test.Delta;

using Core.Serialization.Delta.Command;

public abstract class JsonSerializationTestsCommandBase : JsonSerializationTestsBase
{
    #region Partitions

    protected CommandResponse CreateCommandResponse() =>
        new(CommandId(), ProtocolMessages());

    protected AddPartition CreateAddPartition() =>
        new(Chunk(), CommandId(), ProtocolMessages());

    protected DeletePartition CreateDeletePartition() =>
        new(TargetNode(), CommandId(), ProtocolMessages());

    #endregion

    #region Nodes

    protected ChangeClassifier CreateChangeClassifier() =>
        new(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages());

    #endregion

    #region Properties

    protected AddProperty CreateAddProperty() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), ProtocolMessages());

    protected DeleteProperty CreateDeleteProperty() =>
        new(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages());

    protected ChangeProperty CreateChangeProperty() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), ProtocolMessages());

    #endregion

    #region Children

    protected AddChild CreateAddChild() =>
        new(TargetNode(), Chunk(), MetaPointer(), Index(), CommandId(), ProtocolMessages());

    protected DeleteChild CreateDeleteChild() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected ReplaceChild CreateReplaceChild() =>
        new(Chunk(), TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected MoveChildFromOtherContainment CreateMoveChildFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected MoveChildFromOtherContainmentInSameParent CreateMoveChildFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected MoveChildInSameContainment CreateMoveChildInSameContainment() =>
        new(Index(), TargetNode(), CommandId(), ProtocolMessages());

    protected MoveAndReplaceChildFromOtherContainment CreateMoveAndReplaceChildFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), ProtocolMessages());

    protected MoveAndReplaceChildFromOtherContainmentInSameParent
        CreateMoveAndReplaceChildFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), ProtocolMessages());

    protected MoveAndReplaceChildInSameContainment CreateMoveAndReplaceChildInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), CommandId(),
            ProtocolMessages());

    #endregion

    #region Annotations

    protected AddAnnotation CreateAddAnnotation() =>
        new(TargetNode(), Chunk(), Index(), CommandId(), ProtocolMessages());

    protected DeleteAnnotation CreateDeleteAnnotation() =>
        new(TargetNode(), Index(), TargetNode(), CommandId(), ProtocolMessages());

    protected ReplaceAnnotation CreateReplaceAnnotation() =>
        new(Chunk(), TargetNode(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected MoveAnnotationFromOtherParent CreateMoveAnnotationFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected MoveAnnotationInSameParent CreateMoveAnnotationInSameParent() =>
        new(Index(), TargetNode(), CommandId(), ProtocolMessages());

    protected MoveAndReplaceAnnotationFromOtherParent CreateMoveAndReplaceAnnotationFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(),
            CommandId(), ProtocolMessages());

    protected MoveAndReplaceAnnotationInSameParent CreateMoveAndReplaceAnnotationInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), CommandId(),
            ProtocolMessages());

    #endregion

    #region References

    protected AddReference CreateAddReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected DeleteReference CreateDeleteReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected ChangeReference CreateChangeReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(),
            ResolveInfo(), CommandId(), ProtocolMessages());

    protected MoveEntryFromOtherReference CreateMoveEntryFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());

    protected MoveEntryFromOtherReferenceInSameParent CreateMoveEntryFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());

    protected MoveEntryInSameReference CreateMoveEntryInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), Index(), TargetNode(),
            ResolveInfo(), CommandId(), ProtocolMessages());

    protected MoveAndReplaceEntryFromOtherReference CreateMoveAndReplaceEntryFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected MoveAndReplaceEntryFromOtherReferenceInSameParent
        CreateMoveAndReplaceEntryFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(),
            TargetNode(), ResolveInfo(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected MoveAndReplaceEntryInSameReference CreateMoveAndReplaceEntryInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());

    protected AddReferenceResolveInfo CreateAddReferenceResolveInfo() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            CommandId(), ProtocolMessages());

    protected DeleteReferenceResolveInfo CreateDeleteReferenceResolveInfo() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            CommandId(), ProtocolMessages());

    protected ChangeReferenceResolveInfo CreateChangeReferenceResolveInfo() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            ResolveInfo(), CommandId(), ProtocolMessages());

    protected AddReferenceTarget CreateAddReferenceTarget() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            CommandId(), ProtocolMessages());

    protected DeleteReferenceTarget CreateDeleteReferenceTarget() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            CommandId(), ProtocolMessages());

    protected ChangeReferenceTarget CreateChangeReferenceTarget() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            TargetNode(), CommandId(), ProtocolMessages());

    #endregion

    protected CompositeCommand CreateCompositeCommand() =>
        new([
            CreateDeleteProperty(),
            CreateDeleteChild(),
            CreateDeleteAnnotation(),
            CreateDeleteReference()
        ], ProtocolMessages());
}