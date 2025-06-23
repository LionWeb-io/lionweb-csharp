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

[TestClass]
public class JsonSerializationTests_Query : JsonSerializationTestsQueryBase
{
    [TestMethod]
    public void SubscribeToChangingPartitionsRequest()
    {
        var input = CreateSubscribeToChangingPartitionsRequest();
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribeToChangingPartitionsResponse()
    {
        var input = CreateSubscribeToChangingPartitionsResponse();
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribeToPartitionContentsRequest()
    {
        var input = CreateSubscribeToPartitionContentsRequest();
        AssertSerialization(input);
    }

    [TestMethod]
    public void SubscribeToPartitionContentsResponse()
    {
        var input = CreateSubscribeToPartitionContentsResponse();
        AssertSerialization(input);
    }

    [TestMethod]
    public void UnsubscribeFromPartitionContentsRequest()
    {
        var input = CreateUnsubscribeFromPartitionContentsRequest();
        AssertSerialization(input);
    }

    [TestMethod]
    public void UnsubscribeFromPartitionContentsResponse()
    {
        var input = CreateUnsubscribeFromPartitionContentsResponse();
        AssertSerialization(input);
    }

    [TestMethod]
    public void GetAvailableIdsRequest()
    {
        var input = CreateGetAvailableIdsRequest();
        AssertSerialization(input);
    }

    [TestMethod]
    public void GetAvailableIdsResponse()
    {
        var input = CreateGetAvailableIdsResponse();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ListPartitionsRequest()
    {
        var input = CreateListPartitionsRequest();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ListPartitionsResponse()
    {
        var input = CreateListPartitionsResponse();
        AssertSerialization(input);
    }
}