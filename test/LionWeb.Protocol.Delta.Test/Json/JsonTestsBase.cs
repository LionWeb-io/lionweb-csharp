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

namespace LionWeb.Protocol.Delta.Test.Json;

using Core;
using Core.Serialization;
using Message;
using Message.Command;
using Message.Event;
using Message.Query;

public abstract class JsonTestsBase
{
    private const int _defaultDepthLimit = 9;
    
    private static int _nextNodeId;
    private static int _nextKey;
    private static int _nextPropertyValue;
    private static int _nextResolveInfo;
    private static Index _nextIndex;
    private static int _nextQueryId;
    private static EventSequenceNumber _nextSequence;
    private static int _nextCommandId;

    protected JsonTestsBase()
    {
        _nextKey = 0;
        _nextPropertyValue = 0;
        _nextResolveInfo = 0;
        _nextIndex = 0;
        _nextQueryId = 0;
        _nextSequence = 0;
        _nextCommandId = 0;
        _nextNodeId = 0;
    }

    #region Query

    protected static SubscribeToChangingPartitionsRequest CreateSubscribeToChangingPartitionsRequest() =>
        new(true, false, QueryId(), AdditionalInfos());

    protected static SubscribeToChangingPartitionsResponse CreateSubscribeToChangingPartitionsResponse() =>
        new(QueryId(), AdditionalInfos());

    protected static SubscribeToPartitionContentsRequest CreateSubscribeToPartitionContentsRequest() =>
        new(TargetNode(), QueryId(), AdditionalInfos());

    protected static SubscribeToPartitionContentsResponse CreateSubscribeToPartitionContentsResponse() =>
        new(Chunk(), QueryId(), AdditionalInfos());

    protected static UnsubscribeFromPartitionContentsRequest CreateUnsubscribeFromPartitionContentsRequest() =>
        new(TargetNode(), QueryId(), AdditionalInfos());

    protected static UnsubscribeFromPartitionContentsResponse CreateUnsubscribeFromPartitionContentsResponse() =>
        new(QueryId(), AdditionalInfos());

    protected static GetAvailableIdsRequest CreateGetAvailableIdsRequest() => new(3, QueryId(), AdditionalInfos());

    protected static GetAvailableIdsResponse CreateGetAvailableIdsResponse() =>
        new([TargetNode(), TargetNode()], QueryId(), AdditionalInfos());

    protected static ListPartitionsRequest CreateListPartitionsRequest() => new(_defaultDepthLimit, QueryId(), AdditionalInfos());

    protected static ListPartitionsResponse CreateListPartitionsResponse() =>
        new(Chunk(), false, QueryId(), AdditionalInfos());

    protected static SignOnRequest CreateSignOnRequest() =>
        new(LionWebVersions.v2026_1.VersionString, ClientId(), QueryId(), RepositoryId(), AdditionalInfos());

    protected static SignOnResponse CreateSignOnResponse() =>
        new(ParticipationId(), QueryId(), AdditionalInfos());

    protected static SignOffRequest CreateSignOffRequest() =>
        new(QueryId(), AdditionalInfos());

    protected static SignOffResponse CreateSignOffResponse() =>
        new(QueryId(), AdditionalInfos());

    protected static ReconnectRequest CreateReconnectRequest() =>
        new(LionWebVersions.v2026_1.VersionString, ClientId(), RepositoryId(), ParticipationId(), Sequence(), QueryId(), AdditionalInfos());

    protected static ReconnectResponse CreateReconnectResponse() =>
        new(Sequence(), QueryId(), AdditionalInfos());
    
    protected static IEnumerable<object[]> CollectQueryMessages() =>
    [
        [CreateSubscribeToChangingPartitionsRequest(), typeof(SubscribeToChangingPartitionsRequest)],
        [CreateSubscribeToChangingPartitionsResponse(), typeof(SubscribeToChangingPartitionsResponse)],
        [CreateSubscribeToPartitionContentsRequest(), typeof(SubscribeToPartitionContentsRequest)],
        [CreateSubscribeToPartitionContentsResponse(), typeof(SubscribeToPartitionContentsResponse)],
        [CreateUnsubscribeFromPartitionContentsRequest(), typeof(UnsubscribeFromPartitionContentsRequest)],
        [CreateUnsubscribeFromPartitionContentsResponse(), typeof(UnsubscribeFromPartitionContentsResponse)],
        [CreateGetAvailableIdsRequest(), typeof(GetAvailableIdsRequest)],
        [CreateGetAvailableIdsResponse(), typeof(GetAvailableIdsResponse)],
        [CreateListPartitionsRequest(), typeof(ListPartitionsRequest)],
        [CreateListPartitionsResponse(), typeof(ListPartitionsResponse)],
        [CreateSignOnRequest(), typeof(SignOnRequest)],
        [CreateSignOnResponse(), typeof(SignOnResponse)],
        [CreateSignOffRequest(), typeof(SignOffRequest)],
        [CreateSignOffResponse(), typeof(SignOffResponse)],
        [CreateReconnectRequest(), typeof(ReconnectRequest)],
        [CreateReconnectResponse(), typeof(ReconnectResponse)]
    ];

    #endregion

    #region Command

    #region Partitions

    protected static AddPartition CreateAddPartition() =>
        new(Chunk(), CommandId(), AdditionalInfos());

    protected static DeletePartition CreateDeletePartition() =>
        new(TargetNode(), CommandId(), AdditionalInfos());

    #endregion

    #region Nodes

    protected static ChangeClassifier CreateChangeClassifier() =>
        new(TargetNode(), MetaPointer(), CommandId(), AdditionalInfos());

    #endregion

    #region Properties

    protected static AddProperty CreateAddProperty() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), AdditionalInfos());

    protected static DeleteProperty CreateDeleteProperty() =>
        new(TargetNode(), MetaPointer(), CommandId(), AdditionalInfos());

    protected static ChangeProperty CreateChangeProperty() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), AdditionalInfos());

    #endregion

    #region Children

    protected static AddChild CreateAddChild() =>
        new(TargetNode(), Chunk(), MetaPointer(), Index(), CommandId(), AdditionalInfos());

    protected static DeleteChild CreateDeleteChild() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            AdditionalInfos());

    protected static ReplaceChild CreateReplaceChild() =>
        new(Chunk(), TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            AdditionalInfos());

    protected static MoveChildFromOtherContainment CreateMoveChildFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            AdditionalInfos());

    protected static MoveChildFromOtherContainmentInSameParent CreateMoveChildFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), CommandId(),
            AdditionalInfos());

    protected static MoveChildInSameContainment CreateMoveChildInSameContainment() =>
        new(Index(), TargetNode(), CommandId(), AdditionalInfos());

    protected static MoveAndReplaceChildFromOtherContainment CreateMoveAndReplaceChildFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), AdditionalInfos());

    protected static MoveAndReplaceChildFromOtherContainmentInSameParent
        CreateMoveAndReplaceChildFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), AdditionalInfos());

    protected static MoveAndReplaceChildInSameContainment CreateMoveAndReplaceChildInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), CommandId(),
            AdditionalInfos());

    #endregion

    #region Annotations

    protected static AddAnnotation CreateAddAnnotation() =>
        new(TargetNode(), Chunk(), Index(), CommandId(), AdditionalInfos());

    protected static DeleteAnnotation CreateDeleteAnnotation() =>
        new(TargetNode(), Index(), TargetNode(), CommandId(), AdditionalInfos());

    protected static ReplaceAnnotation CreateReplaceAnnotation() =>
        new(Chunk(), TargetNode(), Index(), TargetNode(), CommandId(),
            AdditionalInfos());

    protected static MoveAnnotationFromOtherParent CreateMoveAnnotationFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), CommandId(),
            AdditionalInfos());

    protected static MoveAnnotationInSameParent CreateMoveAnnotationInSameParent() =>
        new(Index(), TargetNode(), CommandId(), AdditionalInfos());

    protected static MoveAndReplaceAnnotationFromOtherParent CreateMoveAndReplaceAnnotationFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(),
            CommandId(), AdditionalInfos());

    protected static MoveAndReplaceAnnotationInSameParent CreateMoveAndReplaceAnnotationInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), CommandId(),
            AdditionalInfos());

    #endregion

    #region References

    protected static AddReference CreateAddReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            AdditionalInfos());

    protected static DeleteReference CreateDeleteReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            AdditionalInfos());

    protected static ChangeReference CreateChangeReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(),
            ResolveInfo(), CommandId(), AdditionalInfos());

    #endregion

    protected static CompositeCommand CreateCompositeCommand() =>
        new([
            CreateDeleteProperty(),
            CreateDeleteChild(),
            CreateDeleteAnnotation(),
            CreateDeleteReference()
        ], CommandId(), AdditionalInfos());


    protected static IEnumerable<object[]> CollectCommandMessages() =>
    [
        [CreateAddPartition(), typeof(AddPartition)],
        [CreateDeletePartition(), typeof(DeletePartition)],
        [CreateChangeClassifier(), typeof(ChangeClassifier)],
        [CreateAddProperty(), typeof(AddProperty)],
        [CreateDeleteProperty(), typeof(DeleteProperty)],
        [CreateChangeProperty(), typeof(ChangeProperty)],
        [CreateAddChild(), typeof(AddChild)],
        [CreateDeleteChild(), typeof(DeleteChild)],
        [CreateReplaceChild(), typeof(ReplaceChild)],
        [CreateMoveChildFromOtherContainment(), typeof(MoveChildFromOtherContainment)],
        [CreateMoveChildFromOtherContainmentInSameParent(), typeof(MoveChildFromOtherContainmentInSameParent)],
        [CreateMoveChildInSameContainment(), typeof(MoveChildInSameContainment)],
        [CreateMoveAndReplaceChildFromOtherContainment(), typeof(MoveAndReplaceChildFromOtherContainment)],
        [CreateMoveAndReplaceChildFromOtherContainmentInSameParent(), typeof(MoveAndReplaceChildFromOtherContainmentInSameParent)],
        [CreateMoveAndReplaceChildInSameContainment(), typeof(MoveAndReplaceChildInSameContainment)],
        [CreateAddAnnotation(), typeof(AddAnnotation)],
        [CreateDeleteAnnotation(), typeof(DeleteAnnotation)],
        [CreateReplaceAnnotation(), typeof(ReplaceAnnotation)],
        [CreateMoveAnnotationFromOtherParent(), typeof(MoveAnnotationFromOtherParent)],
        [CreateMoveAnnotationInSameParent(), typeof(MoveAnnotationInSameParent)],
        [CreateMoveAndReplaceAnnotationFromOtherParent(), typeof(MoveAndReplaceAnnotationFromOtherParent)],
        [CreateMoveAndReplaceAnnotationInSameParent(), typeof(MoveAndReplaceAnnotationInSameParent)],
        [CreateAddReference(), typeof(AddReference)],
        [CreateDeleteReference(), typeof(DeleteReference)],
        [CreateChangeReference(), typeof(ChangeReference)],
        [CreateCompositeCommand(), typeof(CompositeCommand)]
    ];

    #endregion

    #region Event

    #region Partitions

    protected static PartitionAdded CreatePartitionAdded()
    {
        var chunk = Chunk();
        return new(chunk, chunk.Nodes.First().Id, Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };
    }

    protected static PartitionDeleted CreatePartitionDeleted() =>
        new(TargetNode(), Descendants(), Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    #endregion

    #region Nodes

    protected static ClassifierChanged CreateClassifierChanged() =>
        new(TargetNode(), MetaPointer(), MetaPointer(), Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    #endregion

    #region Properties

    protected static PropertyAdded CreatePropertyAdded() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static PropertyDeleted CreatePropertyDeleted() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static PropertyChanged CreatePropertyChanged() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), PropertyValue(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    #endregion

    #region Children

    protected static ChildAdded CreateChildAdded() =>
        new(TargetNode(), Chunk(), MetaPointer(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildDeleted CreateChildDeleted() =>
        new(TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildReplaced CreateChildReplaced() =>
        new(Chunk(), TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildMovedFromOtherContainment CreateChildMovedFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), Origin(),
            AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static ChildMovedFromOtherContainmentInSameParent CreateChildMovedFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildMovedInSameContainment CreateChildMovedInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildMovedAndReplacedFromOtherContainment CreateChildMovedAndReplacedFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), TargetNode(),
            Descendants(), Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static ChildMovedAndReplacedFromOtherContainmentInSameParent
        CreateChildMovedAndReplacedFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(),
            Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static ChildMovedAndReplacedInSameContainment CreateChildMovedAndReplacedInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(), Origin(),
            AdditionalInfos()) { SequenceNumber = Sequence() };

    #endregion

    #region Annotations

    protected static AnnotationAdded CreateAnnotationAdded() =>
        new(TargetNode(), Chunk(), Index(), Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static AnnotationDeleted CreateAnnotationDeleted() =>
        new(TargetNode(), Descendants(), TargetNode(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static AnnotationReplaced CreateAnnotationReplaced() =>
        new(Chunk(), TargetNode(), Descendants(), TargetNode(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static AnnotationMovedFromOtherParent CreateAnnotationMovedFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(), Index(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static AnnotationMovedInSameParent CreateAnnotationMovedInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), Index(), Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static AnnotationMovedAndReplacedFromOtherParent CreateAnnotationMovedAndReplacedFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(), Index(), TargetNode(), Descendants(), Origin(),
            AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static AnnotationMovedAndReplacedInSameParent CreateAnnotationMovedAndReplacedInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), Index(), TargetNode(), Descendants(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    #endregion

    #region References

    protected static ReferenceAdded CreateReferenceAdded() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceDeleted CreateReferenceDeleted() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), AdditionalInfos())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceChanged CreateReferenceChanged() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(), ResolveInfo(), Origin(),
            AdditionalInfos()) { SequenceNumber = Sequence() };

    #endregion

    #region Miscellaneous

    protected static CompositeEvent CreateCompositeEvent() =>
        new(
        [
            CreatePropertyDeleted(),
            CreateChildDeleted(),
            CreateAnnotationDeleted(),
            CreateReferenceDeleted()
        ], Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static NoOpEvent CreateNoOpEvent() =>
        new(Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    protected static ErrorEvent CreateErrorEvent() =>
        new("myError", "very nice message", Origin(), AdditionalInfos()) { SequenceNumber = Sequence() };

    #endregion

    protected static IEnumerable<object[]> CollectEventMessages() =>
    [
        [CreatePartitionAdded(), typeof(PartitionAdded)],
        [CreatePartitionDeleted(), typeof(PartitionDeleted)],
        [CreateClassifierChanged(), typeof(ClassifierChanged)],
        [CreatePropertyAdded(), typeof(PropertyAdded)],
        [CreatePropertyDeleted(), typeof(PropertyDeleted)],
        [CreatePropertyChanged(), typeof(PropertyChanged)],
        [CreateChildAdded(), typeof(ChildAdded)],
        [CreateChildDeleted(), typeof(ChildDeleted)],
        [CreateChildReplaced(), typeof(ChildReplaced)],
        [CreateChildMovedFromOtherContainment(), typeof(ChildMovedFromOtherContainment)],
        [CreateChildMovedFromOtherContainmentInSameParent(), typeof(ChildMovedFromOtherContainmentInSameParent)],
        [CreateChildMovedInSameContainment(), typeof(ChildMovedInSameContainment)],
        [CreateChildMovedAndReplacedFromOtherContainment(), typeof(ChildMovedAndReplacedFromOtherContainment)],
        [CreateChildMovedAndReplacedFromOtherContainmentInSameParent(), typeof(ChildMovedAndReplacedFromOtherContainmentInSameParent)],
        [CreateChildMovedAndReplacedInSameContainment(), typeof(ChildMovedAndReplacedInSameContainment)],
        [CreateAnnotationAdded(), typeof(AnnotationAdded)],
        [CreateAnnotationDeleted(), typeof(AnnotationDeleted)],
        [CreateAnnotationReplaced(), typeof(AnnotationReplaced)],
        [CreateAnnotationMovedFromOtherParent(), typeof(AnnotationMovedFromOtherParent)],
        [CreateAnnotationMovedInSameParent(), typeof(AnnotationMovedInSameParent)],
        [CreateAnnotationMovedAndReplacedFromOtherParent(), typeof(AnnotationMovedAndReplacedFromOtherParent)],
        [CreateAnnotationMovedAndReplacedInSameParent(), typeof(AnnotationMovedAndReplacedInSameParent)],
        [CreateReferenceAdded(), typeof(ReferenceAdded)],
        [CreateReferenceDeleted(), typeof(ReferenceDeleted)],
        [CreateReferenceChanged(), typeof(ReferenceChanged)],
        [CreateCompositeEvent(), typeof(CompositeEvent)],
        [CreateNoOpEvent(), typeof(NoOpEvent)],
        [CreateErrorEvent(), typeof(ErrorEvent)]
    ];

    #endregion

    protected static IEnumerable<object[]> CollectAllMessages() =>
        CollectQueryMessages()
            .Concat(CollectCommandMessages())
            .Concat(CollectEventMessages());

    protected static TargetNode TargetNode() =>
        (++_nextNodeId).ToString();

    protected static TargetNode[] Descendants() =>
        [TargetNode(), TargetNode()];

    protected static DeltaSerializationChunk Chunk() =>
        new DeltaSerializationChunk([
            Node(),
            Node()
        ]);

    private static SerializedNode Node() =>
        new SerializedNode
        {
            Id = TargetNode(),
            Classifier = MetaPointer(),
        };

    protected static MetaPointer MetaPointer() =>
        new MetaPointer("myLang", "v0", CreateKey());

    private static NodeId CreateKey() =>
        (++_nextKey).ToString();

    protected static AdditionalInfo[] AdditionalInfos() =>
    [
        new AdditionalInfo("MyKind", "MyMessage",
            new() { { "key0", "value0" }, { "key1", "value1" } }
        )
    ];

    protected static PropertyValue PropertyValue() =>
        (++_nextPropertyValue).ToString();

    protected static ResolveInfo ResolveInfo() =>
        $"resolve{++_nextResolveInfo}";

    protected static Index Index() =>
        ++_nextIndex;


    protected static CommandId CommandId() =>
        (++_nextCommandId).ToString();

    protected static EventSequenceNumber Sequence() =>
        _nextSequence++;

    protected static CommandSource[] Origin() =>
    [
        CreateCommandSource(),
        CreateCommandSource()
    ];

    private static CommandSource CreateCommandSource() =>
        new CommandSource(ParticipationId(), TargetNode());

    private static ParticipationId ParticipationId() =>
        "myParticipation";

    private static ClientId ClientId() =>
        "iAmTheClient";

    protected static QueryId QueryId() =>
        (++_nextQueryId).ToString();

    protected static RepositoryId RepositoryId() =>
        "myRepo";

    private TestContext testContextInstance;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}