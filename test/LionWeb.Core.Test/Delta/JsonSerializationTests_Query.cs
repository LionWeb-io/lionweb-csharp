﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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
using TargetNode = NodeId;
using CommandId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;

[TestClass]
public class JsonSerializationTests_Query : JsonSerializationTestsBase
{
    [TestMethod]
    public void SubscribePartitionsRequest()
    {
        var input = new SubscribePartitionsRequest(true, false, false, CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribePartitionsResponse()
    {
        var input = new SubscribePartitionsResponse(CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribePartitionRequest()
    {
        var input = new SubscribePartitionRequest(CreateTargetNode(), CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribePartitionResponse()
    {
        var input = new SubscribePartitionResponse(CreateChunk(), CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void UnsubscribePartitionRequest()
    {
        var input = new UnsubscribePartitionRequest(CreateTargetNode(), CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void UnsubscribePartitionResponse()
    {
        var input = new UnsubscribePartitionResponse(CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void GetAvailableIdsRequest()
    {
        var input = new GetAvailableIdsRequest(3, CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void GetAvailableIdsResponse()
    {
        var input = new GetAvailableIdsResponse([CreateTargetNode(), CreateTargetNode()], CreateQueryId(), CreateProtocolMessage());
        AssertSerialization(input);
    }
    
    private int _nextQueryId = 0;

    private QueryId CreateQueryId() =>
        (++_nextQueryId).ToString();
}