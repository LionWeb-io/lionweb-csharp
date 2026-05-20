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

using Message;
using Message.Command;
using Message.Event;
using System.Text.RegularExpressions;

[TestClass]
public class JsonSerializationTests : JsonTestsBase
{
    private static IEnumerable<object[]> CollectAllDeltaMessages() => CollectAllMessages();

    [TestMethod]
    [DynamicData(nameof(CollectAllDeltaMessages), DynamicDataSourceType.Method)]
    public void Serialization(IDeltaContent delta)
    {
        var deltaSerializer = new DeltaSerializer();
        var serialized = deltaSerializer.Serialize(delta);
        var deserialized = deltaSerializer.Deserialize<IDeltaContent>(serialized);

        // see https://github.com/LionWeb-io/specification/issues/351
        if (delta is CompositeEvent ce)
            delta = ce with { SequenceNumber = IDeltaEvent.DefaultEventSequenceNumber };

        Assert.AreEqual(delta, deserialized);
    }

    private static IEnumerable<object[]> CollectMessagesInsideCompositeCommand() => CollectCommandMessages();

    [TestMethod]
    [DynamicData(nameof(CollectMessagesInsideCompositeCommand), DynamicDataSourceType.Method)]
    public void CompositeCommandSerialization(IDeltaCommand command)
    {
        var deltaSerializer = new DeltaSerializer();
        var compositeCommand = new CompositeCommand([command], "compositeCommandId", []);
        var serialized = deltaSerializer.Serialize(compositeCommand);
        var deserialized = deltaSerializer.Deserialize<CompositeCommand>(serialized);
        Assert.AreEqual(compositeCommand, deserialized);
    }

    private static IEnumerable<object[]> CollectMessagesInsideCompositeEvent() => CollectEventMessages();

    [TestMethod]
    [DynamicData(nameof(CollectMessagesInsideCompositeEvent), DynamicDataSourceType.Method)]
    public void CompositeEventSerialization(IDeltaEvent @event)
    {
        // see https://github.com/LionWeb-io/specification/issues/351
        if (@event is CompositeEvent ce)
            @event = ce with { SequenceNumber = IDeltaEvent.DefaultEventSequenceNumber };
        
        var deltaSerializer = new DeltaSerializer();
        var compositeEvent = new CompositeEvent([@event], []);
        var serialized = deltaSerializer.Serialize(compositeEvent);
        var deserialized = deltaSerializer.Deserialize<CompositeEvent>(serialized);
        deserialized.SequenceNumber = IDeltaEvent.DefaultEventSequenceNumber;

        Assert.AreEqual(compositeEvent, deserialized);
    }

    [TestMethod]
    public void NoPrettyPrinting()
    {
        var deltaSerializer = new DeltaSerializer();
        var serialized = deltaSerializer.Serialize(CreateAddChild());

        Assert.IsFalse(Regex.IsMatch(serialized, "\\s"), serialized);
    }
}