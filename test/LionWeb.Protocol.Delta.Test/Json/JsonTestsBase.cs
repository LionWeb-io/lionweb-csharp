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
        new(true, false, false, QueryId(), ProtocolMessages());

    protected static SubscribeToChangingPartitionsResponse CreateSubscribeToChangingPartitionsResponse() =>
        new(QueryId(), ProtocolMessages());

    protected static SubscribeToPartitionContentsRequest CreateSubscribeToPartitionContentsRequest() =>
        new(TargetNode(), QueryId(), ProtocolMessages());

    protected static SubscribeToPartitionContentsResponse CreateSubscribeToPartitionContentsResponse() =>
        new(Chunk(), QueryId(), ProtocolMessages());

    protected static UnsubscribeFromPartitionContentsRequest CreateUnsubscribeFromPartitionContentsRequest() =>
        new(TargetNode(), QueryId(), ProtocolMessages());

    protected static UnsubscribeFromPartitionContentsResponse CreateUnsubscribeFromPartitionContentsResponse() =>
        new(QueryId(), ProtocolMessages());

    protected static GetAvailableIdsRequest CreateGetAvailableIdsRequest() => new(3, QueryId(), ProtocolMessages());

    protected static GetAvailableIdsResponse CreateGetAvailableIdsResponse() =>
        new([TargetNode(), TargetNode()], QueryId(), ProtocolMessages());

    protected static ListPartitionsRequest CreateListPartitionsRequest() => new(QueryId(), ProtocolMessages());

    protected static ListPartitionsResponse CreateListPartitionsResponse() =>
        new(Chunk(), QueryId(), ProtocolMessages());

    protected static SignOnRequest CreateSignOnRequest() =>
        new(LionWebVersions.v2025_1.VersionString, ClientId(), QueryId(), RepositoryId(), ProtocolMessages());

    protected static SignOnResponse CreateSignOnResponse() =>
        new(ParticipationId(), QueryId(), ProtocolMessages());

    protected static SignOffRequest CreateSignOffRequest() =>
        new(QueryId(), ProtocolMessages());

    protected static SignOffResponse CreateSignOffResponse() =>
        new(QueryId(), ProtocolMessages());

    protected static ReconnectRequest CreateReconnectRequest() =>
        new(ParticipationId(), Sequence(), QueryId(), ProtocolMessages());

    protected static ReconnectResponse CreateReconnectResponse() =>
        new(Sequence(), QueryId(), ProtocolMessages());
    
    protected static IEnumerable<object[]> CollectQueryMessages() =>
    [
        [CreateSubscribeToChangingPartitionsRequest()],
        [CreateSubscribeToChangingPartitionsResponse()],
        [CreateSubscribeToPartitionContentsRequest()],
        [CreateSubscribeToPartitionContentsResponse()],
        [CreateUnsubscribeFromPartitionContentsRequest()],
        [CreateUnsubscribeFromPartitionContentsResponse()],
        [CreateGetAvailableIdsRequest()],
        [CreateGetAvailableIdsResponse()],
        [CreateListPartitionsRequest()],
        [CreateListPartitionsResponse()],
        [CreateSignOnRequest()],
        [CreateSignOnResponse()],
        [CreateSignOffRequest()],
        [CreateSignOffResponse()],
        [CreateReconnectRequest()],
        [CreateReconnectResponse()]
    ];

    #endregion

    #region Command

    #region Partitions

    protected static AddPartition CreateAddPartition() =>
        new(Chunk(), CommandId(), ProtocolMessages());

    protected static DeletePartition CreateDeletePartition() =>
        new(TargetNode(), CommandId(), ProtocolMessages());

    #endregion

    #region Nodes

    protected static ChangeClassifier CreateChangeClassifier() =>
        new(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages());

    #endregion

    #region Properties

    protected static AddProperty CreateAddProperty() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), ProtocolMessages());

    protected static DeleteProperty CreateDeleteProperty() =>
        new(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages());

    protected static ChangeProperty CreateChangeProperty() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), ProtocolMessages());

    #endregion

    #region Children

    protected static AddChild CreateAddChild() =>
        new(TargetNode(), Chunk(), MetaPointer(), Index(), CommandId(), ProtocolMessages());

    protected static DeleteChild CreateDeleteChild() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected static ReplaceChild CreateReplaceChild() =>
        new(Chunk(), TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected static MoveChildFromOtherContainment CreateMoveChildFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected static MoveChildFromOtherContainmentInSameParent CreateMoveChildFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected static MoveChildInSameContainment CreateMoveChildInSameContainment() =>
        new(Index(), TargetNode(), CommandId(), ProtocolMessages());

    protected static MoveAndReplaceChildFromOtherContainment CreateMoveAndReplaceChildFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), ProtocolMessages());

    protected static MoveAndReplaceChildFromOtherContainmentInSameParent
        CreateMoveAndReplaceChildFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), ProtocolMessages());

    protected static MoveAndReplaceChildInSameContainment CreateMoveAndReplaceChildInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), CommandId(),
            ProtocolMessages());

    #endregion

    #region Annotations

    protected static AddAnnotation CreateAddAnnotation() =>
        new(TargetNode(), Chunk(), Index(), CommandId(), ProtocolMessages());

    protected static DeleteAnnotation CreateDeleteAnnotation() =>
        new(TargetNode(), Index(), TargetNode(), CommandId(), ProtocolMessages());

    protected static ReplaceAnnotation CreateReplaceAnnotation() =>
        new(Chunk(), TargetNode(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected static MoveAnnotationFromOtherParent CreateMoveAnnotationFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());

    protected static MoveAnnotationInSameParent CreateMoveAnnotationInSameParent() =>
        new(Index(), TargetNode(), CommandId(), ProtocolMessages());

    protected static MoveAndReplaceAnnotationFromOtherParent CreateMoveAndReplaceAnnotationFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(),
            CommandId(), ProtocolMessages());

    protected static MoveAndReplaceAnnotationInSameParent CreateMoveAndReplaceAnnotationInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), CommandId(),
            ProtocolMessages());

    #endregion

    #region References

    protected static AddReference CreateAddReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected static DeleteReference CreateDeleteReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected static ChangeReference CreateChangeReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(),
            ResolveInfo(), CommandId(), ProtocolMessages());

    protected static MoveEntryFromOtherReference CreateMoveEntryFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());

    protected static MoveEntryFromOtherReferenceInSameParent CreateMoveEntryFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());

    protected static MoveEntryInSameReference CreateMoveEntryInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), Index(), TargetNode(),
            ResolveInfo(), CommandId(), ProtocolMessages());

    protected static MoveAndReplaceEntryFromOtherReference CreateMoveAndReplaceEntryFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected static MoveAndReplaceEntryFromOtherReferenceInSameParent
        CreateMoveAndReplaceEntryFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(),
            TargetNode(), ResolveInfo(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());

    protected static MoveAndReplaceEntryInSameReference CreateMoveAndReplaceEntryInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());

    protected static AddReferenceResolveInfo CreateAddReferenceResolveInfo() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(),
            CommandId(), ProtocolMessages());

    protected static DeleteReferenceResolveInfo CreateDeleteReferenceResolveInfo() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(),
            CommandId(), ProtocolMessages());

    protected static ChangeReferenceResolveInfo CreateChangeReferenceResolveInfo() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(),
            ResolveInfo(), CommandId(), ProtocolMessages());

    protected static AddReferenceTarget CreateAddReferenceTarget() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            CommandId(), ProtocolMessages());

    protected static DeleteReferenceTarget CreateDeleteReferenceTarget() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            CommandId(), ProtocolMessages());

    protected static ChangeReferenceTarget CreateChangeReferenceTarget() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), ProtocolMessages());

    #endregion

    protected static CompositeCommand CreateCompositeCommand() =>
        new([
            CreateDeleteProperty(),
            CreateDeleteChild(),
            CreateDeleteAnnotation(),
            CreateDeleteReference()
        ], CommandId(), ProtocolMessages());


    protected static IEnumerable<object[]> CollectCommandMessages() =>
    [
        [CreateAddPartition()],
        [CreateDeletePartition()],
        [CreateChangeClassifier()],
        [CreateAddProperty()],
        [CreateDeleteProperty()],
        [CreateChangeProperty()],
        [CreateAddChild()],
        [CreateDeleteChild()],
        [CreateReplaceChild()],
        [CreateMoveChildFromOtherContainment()],
        [CreateMoveChildFromOtherContainmentInSameParent()],
        [CreateMoveChildInSameContainment()],
        [CreateMoveAndReplaceChildFromOtherContainment()],
        [CreateMoveAndReplaceChildFromOtherContainmentInSameParent()],
        [CreateMoveAndReplaceChildInSameContainment()],
        [CreateAddAnnotation()],
        [CreateDeleteAnnotation()],
        [CreateReplaceAnnotation()],
        [CreateMoveAnnotationFromOtherParent()],
        [CreateMoveAnnotationInSameParent()],
        [CreateMoveAndReplaceAnnotationFromOtherParent()],
        [CreateMoveAndReplaceAnnotationInSameParent()],
        [CreateAddReference()],
        [CreateDeleteReference()],
        [CreateChangeReference()],
        [CreateMoveEntryFromOtherReference()],
        [CreateMoveEntryFromOtherReferenceInSameParent()],
        [CreateMoveEntryInSameReference()],
        [CreateMoveAndReplaceEntryFromOtherReference()],
        [CreateMoveAndReplaceEntryFromOtherReferenceInSameParent()],
        [CreateMoveAndReplaceEntryInSameReference()],
        [CreateAddReferenceResolveInfo()],
        [CreateDeleteReferenceResolveInfo()],
        [CreateChangeReferenceResolveInfo()],
        [CreateAddReferenceTarget()],
        [CreateDeleteReferenceTarget()],
        [CreateChangeReferenceTarget()],
        [CreateCompositeCommand()]
    ];

    #endregion

    #region Event

    #region Partitions

    protected static PartitionAdded CreatePartitionAdded()
    {
        var chunk = Chunk();
        return new(chunk, chunk.Nodes.First().Id, Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };
    }

    protected static PartitionDeleted CreatePartitionDeleted() =>
        new(TargetNode(), Descendants(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    #endregion

    #region Nodes

    protected static ClassifierChanged CreateClassifierChanged() =>
        new(TargetNode(), MetaPointer(), MetaPointer(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    #endregion

    #region Properties

    protected static PropertyAdded CreatePropertyAdded() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static PropertyDeleted CreatePropertyDeleted() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static PropertyChanged CreatePropertyChanged() =>
        new(TargetNode(), MetaPointer(), PropertyValue(), PropertyValue(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    #endregion

    #region Children

    protected static ChildAdded CreateChildAdded() =>
        new(TargetNode(), Chunk(), MetaPointer(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildDeleted CreateChildDeleted() =>
        new(TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildReplaced CreateChildReplaced() =>
        new(Chunk(), TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildMovedFromOtherContainment CreateChildMovedFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), Origin(),
            ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static ChildMovedFromOtherContainmentInSameParent CreateChildMovedFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildMovedInSameContainment CreateChildMovedInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ChildMovedAndReplacedFromOtherContainment CreateChildMovedAndReplacedFromOtherContainment() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), TargetNode(),
            Descendants(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static ChildMovedAndReplacedFromOtherContainmentInSameParent
        CreateChildMovedAndReplacedFromOtherContainmentInSameParent() =>
        new(MetaPointer(), Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(),
            Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static ChildMovedAndReplacedInSameContainment CreateChildMovedAndReplacedInSameContainment() =>
        new(Index(), TargetNode(), TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(), Origin(),
            ProtocolMessages()) { SequenceNumber = Sequence() };

    #endregion

    #region Annotations

    protected static AnnotationAdded CreateAnnotationAdded() =>
        new(TargetNode(), Chunk(), Index(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static AnnotationDeleted CreateAnnotationDeleted() =>
        new(TargetNode(), Descendants(), TargetNode(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static AnnotationReplaced CreateAnnotationReplaced() =>
        new(Chunk(), TargetNode(), Descendants(), TargetNode(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static AnnotationMovedFromOtherParent CreateAnnotationMovedFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(), Index(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static AnnotationMovedInSameParent CreateAnnotationMovedInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), Index(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static AnnotationMovedAndReplacedFromOtherParent CreateAnnotationMovedAndReplacedFromOtherParent() =>
        new(TargetNode(), Index(), TargetNode(), TargetNode(), Index(), TargetNode(), Descendants(), Origin(),
            ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static AnnotationMovedAndReplacedInSameParent CreateAnnotationMovedAndReplacedInSameParent() =>
        new(Index(), TargetNode(), TargetNode(), Index(), TargetNode(), Descendants(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    #endregion

    #region References

    protected static ReferenceAdded CreateReferenceAdded() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceDeleted CreateReferenceDeleted() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceChanged CreateReferenceChanged() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(), ResolveInfo(), Origin(),
            ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static EntryMovedFromOtherReference CreateEntryMovedFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static EntryMovedFromOtherReferenceInSameParent CreateEntryMovedFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(),
            ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static EntryMovedInSameReference CreateEntryMovedInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), Index(), TargetNode(), ResolveInfo(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static EntryMovedAndReplacedFromOtherReference CreateEntryMovedAndReplacedFromOtherReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(), MetaPointer(), Index(),
            TargetNode(), ResolveInfo(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static EntryMovedAndReplacedFromOtherReferenceInSameParent
        CreateEntryMovedAndReplacedFromOtherReferenceInSameParent() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static EntryMovedAndReplacedInSameReference CreateEntryMovedAndReplacedInSameReference() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Index(), TargetNode(), ResolveInfo(),
            Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static ReferenceResolveInfoAdded CreateReferenceResolveInfoAdded() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceResolveInfoDeleted CreateReferenceResolveInfoDeleted() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceResolveInfoChanged CreateReferenceResolveInfoChanged() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(), ResolveInfo(), Origin(),
            ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static ReferenceTargetAdded CreateReferenceTargetAdded() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceTargetDeleted CreateReferenceTargetDeleted() =>
        new(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(), Origin(), ProtocolMessages())
        {
            SequenceNumber = Sequence()
        };

    protected static ReferenceTargetChanged CreateReferenceTargetChanged() =>
        new(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(), Origin(),
            ProtocolMessages()) { SequenceNumber = Sequence() };

    #endregion

    #region Miscellaneous

    protected static CompositeEvent CreateCompositeEvent() =>
        new(
        [
            CreatePropertyDeleted(),
            CreateChildDeleted(),
            CreateAnnotationDeleted(),
            CreateReferenceDeleted()
        ], ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static NoOpEvent CreateNoOpEvent() =>
        new(Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    protected static ErrorEvent CreateError() =>
        new("myError", "very nice message", Origin(), ProtocolMessages()) { SequenceNumber = Sequence() };

    #endregion

    protected static IEnumerable<object[]> CollectEventMessages() =>
    [
        [CreatePartitionAdded()],
        [CreatePartitionDeleted()],
        [CreateClassifierChanged()],
        [CreatePropertyAdded()],
        [CreatePropertyDeleted()],
        [CreatePropertyChanged()],
        [CreateChildAdded()],
        [CreateChildDeleted()],
        [CreateChildReplaced()],
        [CreateChildMovedFromOtherContainment()],
        [CreateChildMovedFromOtherContainmentInSameParent()],
        [CreateChildMovedInSameContainment()],
        [CreateChildMovedAndReplacedFromOtherContainment()],
        [CreateChildMovedAndReplacedFromOtherContainmentInSameParent()],
        [CreateChildMovedAndReplacedInSameContainment()],
        [CreateAnnotationAdded()],
        [CreateAnnotationDeleted()],
        [CreateAnnotationReplaced()],
        [CreateAnnotationMovedFromOtherParent()],
        [CreateAnnotationMovedInSameParent()],
        [CreateAnnotationMovedAndReplacedFromOtherParent()],
        [CreateAnnotationMovedAndReplacedInSameParent()],
        [CreateReferenceAdded()],
        [CreateReferenceDeleted()],
        [CreateReferenceChanged()],
        [CreateEntryMovedFromOtherReference()],
        [CreateEntryMovedFromOtherReferenceInSameParent()],
        [CreateEntryMovedInSameReference()],
        [CreateEntryMovedAndReplacedFromOtherReference()],
        [CreateEntryMovedAndReplacedFromOtherReferenceInSameParent()],
        [CreateEntryMovedAndReplacedInSameReference()],
        [CreateReferenceResolveInfoAdded()],
        [CreateReferenceResolveInfoDeleted()],
        [CreateReferenceResolveInfoChanged()],
        [CreateReferenceTargetAdded()],
        [CreateReferenceTargetDeleted()],
        [CreateReferenceTargetChanged()],
        [CreateCompositeEvent()],
        [CreateNoOpEvent()],
        [CreateError()]
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
            Properties = [],
            Containments = [],
            References = [],
            Annotations = []
        };

    protected static MetaPointer MetaPointer() =>
        new MetaPointer("myLang", "v0", CreateKey());

    private static NodeId CreateKey() =>
        (++_nextKey).ToString();

    protected static ProtocolMessage[] ProtocolMessages() =>
    [
        new ProtocolMessage("MyKind", "MyMessage",
            [new ProtocolMessageData("key0", "value0"), new ProtocolMessageData("key1", "value1")]
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