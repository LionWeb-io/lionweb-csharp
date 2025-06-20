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

    protected TargetNode TargetNode() =>
        (++_nextNodeId).ToString();

    protected TargetNode[] Descendants() =>
        [TargetNode(), TargetNode()];

    protected DeltaSerializationChunk Chunk() =>
        new DeltaSerializationChunk([
            Node(),
            Node()
        ]);

    private SerializedNode Node() =>
        new SerializedNode
        {
            Id = TargetNode(),
            Classifier = MetaPointer(),
            Properties = [],
            Containments = [],
            References = [],
            Annotations = []
        };

    protected MetaPointer MetaPointer() =>
        new MetaPointer("myLang", "v0", CreateKey());

    private string CreateKey() =>
        (++_nextKey).ToString();

    protected ProtocolMessage[] ProtocolMessages() =>
    [
        new ProtocolMessage("MyKind", "MyMessage",
            [new ProtocolMessageData("key0", "value0"), new ProtocolMessageData("key1", "value1")]
        )
    ];

    protected String PropertyValue() =>
        (++_nextPropertyValue).ToString();

    protected String ResolveInfo() =>
        $"resolve{++_nextResolveInfo}";

    protected Int32 Index() =>
        ++_nextIndex;


    private int _nextCommandId = 0;

    protected CommandId CommandId() =>
        (++_nextCommandId).ToString();

    private long _nextSequence = 0;

    protected long Sequence() =>
        _nextSequence++;

    protected CommandSource[] Origin() =>
    [
        CreateCommandSource(),
        CreateCommandSource()
    ];

    private CommandSource CreateCommandSource() =>
        new CommandSource("myParticipation", TargetNode());


    private int _nextQueryId = 0;

    protected QueryId QueryId() =>
        (++_nextQueryId).ToString();
}