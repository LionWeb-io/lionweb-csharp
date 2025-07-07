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

using Event;

[TestClass]
public class JsonSerializationTests : JsonTestsBase
{
    private static IEnumerable<object[]> GetTestData() => CollectAllMessages();

    [TestMethod]
    [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
    public void Serialization(IDeltaContent delta)
    {
        var deltaSerializer = new DeltaSerializer();
        var serialized = deltaSerializer.Serialize(delta);
        var deserialized = deltaSerializer.Deserialize<IDeltaContent>(serialized);

        // see https://github.com/LionWeb-io/specification/issues/351
        if (delta is CompositeEvent ce)
            delta = ce with { SequenceNumber = 0 };
        
        Assert.AreEqual(delta, deserialized);
    }
}