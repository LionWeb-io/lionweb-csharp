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
using Core.Serialization.Delta.Event;
using Core.Serialization.Delta.Query;

[TestClass]
public class DeltaToStringTests : JsonSerializationTestsBase
{
    #region Command

    [TestMethod]
    public void CommandResponse()
    {
        var input = new CommandResponse(CreateCommandId(), CreateProtocolMessages());
        Assert.AreEqual(
            "CommandResponse { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void AddProperty()
    {
        var input = new AddProperty(CreateTargetNode(), CreateMetaPointer(), CreatePropertyValue(), CreateCommandId(),
            CreateProtocolMessages());
        Assert.AreEqual(
            "AddProperty { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 1, Parent = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 }, NewValue = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void CompositeCommand()
    {
        var input = new CompositeCommand([
            new DeleteProperty(CreateTargetNode(), CreateMetaPointer(), CreateCommandId(), CreateProtocolMessages()),
            new DeleteChild(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateCommandId(),
                CreateProtocolMessages()),
        ], CreateProtocolMessages());
        Assert.AreEqual(
            "CompositeCommand { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = , Commands = [DeleteProperty { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 1, Parent = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 } }, DeleteChild { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], CommandId = 2, Parent = 2, Containment = MetaPointer { Language = myLang, Version = v0, Key = 2 }, Index = 1 }] }",
            input.ToString());
    }

    #endregion

    #region Event

    [TestMethod]
    public void PropertyAdded()
    {
        var input = new PropertyAdded(CreateTargetNode(), CreateMetaPointer(), CreatePropertyValue(), NextSequence(),
            CreateOrigin(), CreateProtocolMessages());
        Assert.AreEqual(
            "PropertyAdded { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], EventSequenceNumber = 0, OriginCommands = [CommandSource { ParticipationId = myParticipation, CommandId = 2 }, CommandSource { ParticipationId = myParticipation, CommandId = 3 }], Parent = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 }, NewValue = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void CompositeEvent()
    {
        var input = new CompositeEvent(
        [
            new PropertyDeleted(CreateTargetNode(), CreateMetaPointer(), CreatePropertyValue(), 0, CreateOrigin(),
                CreateProtocolMessages()),
            new ChildDeleted(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(), 0, CreateOrigin(),
                CreateProtocolMessages()),
        ], NextSequence(), CreateProtocolMessages());
        Assert.AreEqual(
            "CompositeEvent { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], Events = [PropertyDeleted { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], EventSequenceNumber = 0, OriginCommands = [CommandSource { ParticipationId = myParticipation, CommandId = 2 }, CommandSource { ParticipationId = myParticipation, CommandId = 3 }], Parent = 1, Property = MetaPointer { Language = myLang, Version = v0, Key = 1 }, OldValue = 1 }, ChildDeleted { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], EventSequenceNumber = 0, OriginCommands = [CommandSource { ParticipationId = myParticipation, CommandId = 7 }, CommandSource { ParticipationId = myParticipation, CommandId = 8 }], Parent = 4, Containment = MetaPointer { Language = myLang, Version = v0, Key = 2 }, Index = 1, DeletedChild = DeltaSerializationChunk { Nodes = [SerializedNode { Id = 5, Classifier = MetaPointer { Language = myLang, Version = v0, Key = 3 }, Properties = [], Containments = [], References = [], Annotations = [], Parent =  }, SerializedNode { Id = 6, Classifier = MetaPointer { Language = myLang, Version = v0, Key = 4 }, Properties = [], Containments = [], References = [], Annotations = [], Parent =  }] } }], EventSequenceNumber = 0 }",
            input.ToString());
    }

    #endregion

    #region Query

    [TestMethod]
    public void SubscribePartitionRequest()
    {
        var input = new SubscribePartitionRequest(CreateTargetNode(), CreateQueryId(), CreateProtocolMessages());
        Assert.AreEqual(
            "SubscribePartitionRequest { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], QueryId = 1, Partition = 1 }",
            input.ToString());
    }

    [TestMethod]
    public void SubscribePartitionResponse()
    {
        var input = new SubscribePartitionResponse(CreateChunk(), CreateQueryId(), CreateProtocolMessages());
        Assert.AreEqual(
            "SubscribePartitionResponse { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], QueryId = 1, Contents = DeltaSerializationChunk { Nodes = [SerializedNode { Id = 1, Classifier = MetaPointer { Language = myLang, Version = v0, Key = 1 }, Properties = [], Containments = [], References = [], Annotations = [], Parent =  }, SerializedNode { Id = 2, Classifier = MetaPointer { Language = myLang, Version = v0, Key = 2 }, Properties = [], Containments = [], References = [], Annotations = [], Parent =  }] } }",
            input.ToString());
    }

    [TestMethod]
    public void GetAvailableIdsResponse()
    {
        var input = new GetAvailableIdsResponse([CreateTargetNode(), CreateTargetNode()], CreateQueryId(),
            CreateProtocolMessages());
        Assert.AreEqual(
            "GetAvailableIdsResponse { ProtocolMessages = [ProtocolMessage { Kind = MyKind, Message = MyMessage, Data = [ProtocolMessageData { Key = key0, Value = value0 }, ProtocolMessageData { Key = key1, Value = value1 }] }], QueryId = 1, Ids = [1, 2] }",
            input.ToString());
    }
    
    #endregion
}