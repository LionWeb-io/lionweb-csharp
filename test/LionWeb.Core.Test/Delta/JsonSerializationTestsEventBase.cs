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

using Core.Serialization.Delta.Event;

public abstract class JsonSerializationTestsEventBase : JsonSerializationTestsBase
{
    #region Partitions

    protected PartitionAdded CreatePartitionAdded() =>
        new(Chunk(), Origin(), Sequence(), ProtocolMessages());

    protected PartitionDeleted CreatePartitionDeleted() =>
        new(TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());

    #endregion

    #region Nodes

    protected ClassifierChanged CreateClassifierChanged() =>
        new(TargetNode(), MetaPointer(), MetaPointer(), Origin(), Sequence(),
            ProtocolMessages());

    #endregion

    #region Properties

    protected PropertyAdded CreatePropertyAdded() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), Origin(), Sequence(),
            ProtocolMessages());

    protected PropertyDeleted CreatePropertyDeleted() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), Origin(), Sequence(),
            ProtocolMessages());

    protected PropertyChanged CreatePropertyChanged() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), PropertyValue(), Origin(),
            Sequence(), ProtocolMessages());

    #endregion

    #region Children

    protected ChildAdded CreateChildAdded() =>
        new(TargetNode(), Chunk(), MetaPointer(), Index(), Origin(), Sequence(),
            ProtocolMessages());

    protected ChildDeleted CreateChildDeleted() =>
        new(TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(), Origin(),
            Sequence(), ProtocolMessages());

    protected ChildReplaced CreateChildReplaced() =>
        new(Chunk(), TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(),
            Origin(), Sequence(), ProtocolMessages());

    protected ChildMovedFromOtherContainment CreateChildMovedFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), TargetNode(),
            MetaPointer(), Index(), Origin(), Sequence(), ProtocolMessages());

    protected ChildMovedFromOtherContainmentInSameParent CreateChildMovedFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), TargetNode(),
            MetaPointer(), Index(), Origin(), Sequence(), ProtocolMessages());

    protected ChildMovedInSameContainment CreateChildMovedInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), MetaPointer(), Index(),
            Origin(), Sequence(), ProtocolMessages());

    protected ChildMovedAndReplacedFromOtherContainment CreateChildMovedAndReplacedFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(), Origin(), Sequence(),
            ProtocolMessages());

    protected ChildMovedAndReplacedFromOtherContainmentInSameParent
        CreateChildMovedAndReplacedFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(),
            TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(), Origin(), Sequence(),
            ProtocolMessages());

    protected ChildMovedAndReplacedInSameContainment CreateChildMovedAndReplacedInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), MetaPointer(),
            Index(), TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());

    #endregion

    #region Annotations

    protected AnnotationAdded CreateAnnotationAdded() =>
        new(TargetNode(), Chunk(), Index(), Origin(), Sequence(), ProtocolMessages());

    protected AnnotationDeleted CreateAnnotationDeleted() =>
        new(TargetNode(), Descendants(), TargetNode(), Index(), Origin(), Sequence(),
            ProtocolMessages());

    protected AnnotationReplaced CreateAnnotationReplaced() =>
        new(Chunk(), TargetNode(), Descendants(), TargetNode(), Index(), Origin(),
            Sequence(), ProtocolMessages());

    protected AnnotationMovedFromOtherParent CreateAnnotationMovedFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(), Index(),
            Origin(), Sequence(), ProtocolMessages());

    protected AnnotationMovedInSameParent CreateAnnotationMovedInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), Index(), Origin(), Sequence(),
            ProtocolMessages());

    protected AnnotationMovedAndReplacedFromOtherParent CreateAnnotationMovedAndReplacedFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(),
            Index(), TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());

    protected AnnotationMovedAndReplacedInSameParent CreateAnnotationMovedAndReplacedInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), Index(),
            TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());

    #endregion

    #region References

    protected ReferenceAdded CreateReferenceAdded() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(),
            Sequence(), ProtocolMessages());

    protected ReferenceDeleted CreateReferenceDeleted() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(),
            Sequence(), ProtocolMessages());

    protected ReferenceChanged CreateReferenceChanged() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());

    protected EntryMovedFromOtherReference CreateEntryMovedFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());

    protected EntryMovedFromOtherReferenceInSameParent CreateEntryMovedFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());

    protected EntryMovedInSameReference CreateEntryMovedInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), Index(), TargetNode(),
            ResolveInfo(), Origin(), Sequence(), ProtocolMessages());

    protected EntryMovedAndReplacedFromOtherReference CreateEntryMovedAndReplacedFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(),
            ProtocolMessages());

    protected EntryMovedAndReplacedFromOtherReferenceInSameParent
        CreateEntryMovedAndReplacedFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(),
            TargetNode(), ResolveInfo(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(),
            ProtocolMessages());

    protected EntryMovedAndReplacedInSameReference CreateEntryMovedAndReplacedInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());

    protected ReferenceResolveInfoAdded CreateReferenceResolveInfoAdded() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            Origin(), Sequence(), ProtocolMessages());

    protected ReferenceResolveInfoDeleted CreateReferenceResolveInfoDeleted() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            Origin(), Sequence(), ProtocolMessages());

    protected ReferenceResolveInfoChanged CreateReferenceResolveInfoChanged() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            ResolveInfo(), Origin(), Sequence(), ProtocolMessages());

    protected ReferenceTargetAdded CreateReferenceTargetAdded() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            Origin(), Sequence(), ProtocolMessages());

    protected ReferenceTargetDeleted CreateReferenceTargetDeleted() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            Origin(), Sequence(), ProtocolMessages());

    protected ReferenceTargetChanged CreateReferenceTargetChanged() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            TargetNode(), Origin(), Sequence(), ProtocolMessages());

    #endregion

    #region Miscellaneous

    protected CompositeEvent CreateCompositeEvent() =>
        new(
        [
            CreatePropertyDeleted(),
            CreateChildDeleted(),
            CreateAnnotationDeleted(),
            CreateReferenceDeleted()
        ], Sequence(), ProtocolMessages());

    protected NoOpEvent CreateNoOpEvent() =>
        new(Origin(), Sequence(), ProtocolMessages());

    protected Error CreateError() =>
        new("myError", "very nice message", Origin(), Sequence(), ProtocolMessages());

    #endregion
}