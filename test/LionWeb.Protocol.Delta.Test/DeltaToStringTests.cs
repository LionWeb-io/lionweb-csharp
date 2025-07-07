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

namespace LionWeb.Protocol.Delta.Test;

using Message.Command;
using Message.Event;
using Message.Query;

[TestClass]
public class DeltaToStringTests : JsonTestsBase
{
    [TestMethod]
    public void NoMessages()
    {
        var input = new CommandResponse(CommandId(), null);
        Assert.AreEqual(
            "CommandResponse { ProtocolMessages = null, CommandId = 1 }",
            input.ToString());
    }
    
    #region Command

    [TestMethod]
    public void CommandResponse()
    {
        var input = new CommandResponse(CommandId(), ProtocolMessages());
        Assert.AreEqual(
            "CommandResponse { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void AddProperty()
    {
        var input = new AddProperty(TargetNode(), MetaPointer(), PropertyValue(), CommandId(),
            ProtocolMessages());
        Assert.AreEqual(
            "AddProperty { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 1, Node = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 }, NewValue = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void CompositeCommand()
    {
        var input = new CompositeCommand([
            new DeleteProperty(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages()),
            new DeleteChild(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
                ProtocolMessages()),
        ], CommandId(), ProtocolMessages());
        Assert.AreEqual(
            "CompositeCommand { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 3, Parts = [DeleteProperty { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 1, Node = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 } }, DeleteChild { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 2, Parent = 2, Containment = MetaPointer { Language = myLang, Version = v0, Key = 2 }, Index = 1, DeletedChild = 3 }] }",
            input.ToString());
    }

    #endregion

    #region Event

    [TestMethod]
    public void PropertyAdded()
    {
        var input = new PropertyAdded(TargetNode(), MetaPointer(), PropertyValue(), Origin(), Sequence(), ProtocolMessages());
        Assert.AreEqual(
            "PropertyAdded { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], SequenceNumber = 0, OriginCommands = [CommandSource { ParticipationId = myParticipation, CommandId = 2 }, CommandSource { ParticipationId = myParticipation, CommandId = 3 }], Node = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 }, NewValue = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void CompositeEvent()
    {
        var input = new CompositeEvent(
        [
            new PropertyDeleted(TargetNode(), MetaPointer(), PropertyValue(), Origin(), 0,
                ProtocolMessages()),
            new ChildDeleted(TargetNode(), [], TargetNode(), MetaPointer(), Index(), Origin(), 0,
                ProtocolMessages()),
        ], Sequence(), ProtocolMessages());
        Assert.AreEqual(
            "CompositeEvent { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], SequenceNumber = 0, Parts = [PropertyDeleted { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], SequenceNumber = 0, OriginCommands = [CommandSource { ParticipationId = myParticipation, CommandId = 2 }, CommandSource { ParticipationId = myParticipation, CommandId = 3 }], Node = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 }, OldValue = 1 }, ChildDeleted { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], SequenceNumber = 0, OriginCommands = [CommandSource { ParticipationId = myParticipation, CommandId = 6 }, CommandSource { ParticipationId = myParticipation, CommandId = 7 }], DeletedChild = 4, DeletedDescendants = [], Parent = 5, Containment = MetaPointer { Language = myLang, Version = v0, Key = 2 }, Index = 1 }] }",
            input.ToString());
    }

    #endregion

    #region Query

    [TestMethod]
    public void SubscribeToPartitionContentsRequest()
    {
        var input = new SubscribeToPartitionContentsRequest(TargetNode(), QueryId(), ProtocolMessages());
        Assert.AreEqual(
            "SubscribeToPartitionContentsRequest { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], QueryId = 1, Partition = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void SubscribeToPartitionContentsResponse()
    {
        var input = new SubscribeToPartitionContentsResponse(Chunk(), QueryId(), ProtocolMessages());
        Assert.AreEqual(
            "SubscribeToPartitionContentsResponse { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], QueryId = 1, Contents = DeltaSerializationChunk { Nodes = [SerializedNode { Id = 1, Classifier = MetaPointer { Language = myLang, Version = v0, Key = 1 }, Properties = [], Containments = [], References = [], Annotations = [], Parent =  }, SerializedNode { Id = 2, Classifier = MetaPointer { Language = myLang, Version = v0, Key = 2 }, Properties = [], Containments = [], References = [], Annotations = [], Parent =  }] } }",
            input.ToString());
    }

    [TestMethod]
    public void GetAvailableIdsResponse()
    {
        var input = new GetAvailableIdsResponse([TargetNode(), TargetNode()], QueryId(),
            ProtocolMessages());
        Assert.AreEqual(
            "GetAvailableIdsResponse { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], QueryId = 1, Ids = [1, 2] }",
            input.ToString());
    }
    
    #endregion
}