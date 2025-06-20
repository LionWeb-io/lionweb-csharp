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

using Core.Serialization;
using Core.Serialization.Delta;
using Core.Serialization.Delta.Event;
using TargetNode = NodeId;
using CommandId = NodeId;
using QueryId = NodeId;

public abstract class JsonSerializationTestsBase
{
    private int _nextNodeId = 0;
    private int _nextKey = 0;
    private int _nextPropertyValue = 0;
    private int _nextResolveInfo = 0;
    private int _nextIndex = 0;

    protected void AssertSerialization(IDeltaContent input)
    {
        var deltaSerializer = new DeltaSerializer();
        var serialized = deltaSerializer.Serialize(input);
        var deserialized = deltaSerializer.Deserialize<IDeltaContent>(serialized);

        Assert.AreEqual(input, deserialized);
    }

    protected TargetNode CreateTargetNode() =>
        (++_nextNodeId).ToString();

    protected DeltaSerializationChunk CreateChunk() =>
        new DeltaSerializationChunk([
            CreateNode(),
            CreateNode()
        ]);

    private SerializedNode CreateNode() =>
        new SerializedNode
        {
            Id = CreateTargetNode(),
            Classifier = CreateMetaPointer(),
            Properties = [],
            Containments = [],
            References = [],
            Annotations = []
        };

    protected MetaPointer CreateMetaPointer() =>
        new MetaPointer("myLang", "v0", CreateKey());

    private string CreateKey() =>
        (++_nextKey).ToString();

    protected ProtocolMessage[] CreateProtocolMessages() =>
        [new ProtocolMessage("MyKind", "MyMessage",
            [new ProtocolMessageData("key0", "value0"), new ProtocolMessageData("key1", "value1")]
        )];

    protected String CreatePropertyValue() =>
        (++_nextPropertyValue).ToString();

    protected SerializedReferenceTarget CreateTarget() =>
        new SerializedReferenceTarget() { Reference = CreateTargetNode(), ResolveInfo = CreateResolveInfo() };

    protected String CreateResolveInfo() =>
        $"resolve{++_nextResolveInfo}";

    protected Int32 CreateIndex() =>
        ++_nextIndex;
    

    private int _nextCommandId = 0;

    protected CommandId CreateCommandId() =>
        (++_nextCommandId).ToString();
    
    private long _nextSequence = 0;

    protected long NextSequence() =>
        _nextSequence++;

    protected CommandSource[] CreateOrigin() =>
    [
        CreateCommandSource(),
        CreateCommandSource()
    ];

    private CommandSource CreateCommandSource() =>
        new CommandSource("myParticipation", CreateTargetNode());
    
    
    private int _nextQueryId = 0;

    protected QueryId CreateQueryId() =>
        (++_nextQueryId).ToString();
}