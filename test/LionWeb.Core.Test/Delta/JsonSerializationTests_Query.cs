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

using Core.Serialization.Delta.Query;

[TestClass]
public class JsonSerializationTests_Query : JsonSerializationTestsBase
{
    [TestMethod]
    public void SubscribeToChangingPartitionsRequest()
    {
        var input = new SubscribeToChangingPartitionsRequest(true, false, false, QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribeToChangingPartitionsResponse()
    {
        var input = new SubscribeToChangingPartitionsResponse(QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribeToPartitionContentsRequest()
    {
        var input = new SubscribeToPartitionContentsRequest(TargetNode(), QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribeToPartitionContentsResponse()
    {
        var input = new SubscribeToPartitionContentsResponse(Chunk(), QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void UnsubscribeFromPartitionContentsRequest()
    {
        var input = new UnsubscribeFromPartitionContentsRequest(TargetNode(), QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void UnsubscribeFromPartitionContentsResponse()
    {
        var input = new UnsubscribeFromPartitionContentsResponse(QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void GetAvailableIdsRequest()
    {
        var input = new GetAvailableIdsRequest(3, QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void GetAvailableIdsResponse()
    {
        var input = new GetAvailableIdsResponse([TargetNode(), TargetNode()], QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ListPartitionsRequest()
    {
        var input = new ListPartitionsRequest(QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ListPartitionsResponse()
    {
        var input = new ListPartitionsResponse(Chunk(), QueryId(), ProtocolMessages());
        AssertSerialization(input);
    }
}