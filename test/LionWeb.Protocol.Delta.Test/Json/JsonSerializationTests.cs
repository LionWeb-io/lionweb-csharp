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

using Core.Notification;
using Core.Notification.Partition;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Core.Utilities;
using Delta.Client;
using Delta.Repository;
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
    public void Serialization(IDeltaContent delta, Type messageType)
    {
        var deltaSerializer = new DeltaSerializer();
        var serialized = deltaSerializer.Serialize(delta);
        var deserialized = deltaSerializer.Deserialize<IDeltaContent>(serialized);

        Assert.AreEqual(delta, deserialized);
    }

    private static IEnumerable<object[]> CollectMessagesInsideCompositeCommand() => CollectCommandMessages();

    [TestMethod]
    [DynamicData(nameof(CollectMessagesInsideCompositeCommand), DynamicDataSourceType.Method)]
    public void CompositeCommandSerialization(IDeltaCommand command, Type commandType)
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
    public void CompositeEventSerialization(IDeltaEvent @event, Type eventType)
    {
        var deltaSerializer = new DeltaSerializer();
        var compositeEvent = new CompositeEvent([@event], null, []);
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

    [TestMethod]
    public void SerializeNumericNotificationId()
    {
        var notification = new PropertyAddedNotification(new DataTypeTestConcept("nodeId"), TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, true,
            new NumericNotificationId("myBase", 23));

        var lionWebVersion = TestLanguageLanguage.Instance.LionWebVersion;
        var deltaSerializer = new DeltaSerializer();

        var deltaCommand = new NotificationToDeltaCommandMapper(new CommandIdProvider(), lionWebVersion).Map(notification);
        Assert.IsTrue(IdUtils.IsValid(deltaCommand.Id));
        Assert.IsTrue(IdUtils.IsValid(deltaCommand.CommandId!));
        var serializedCommand = deltaSerializer.Serialize(deltaCommand);
        Assert.AreEqual(
            """{"messageKind":"AddProperty","node":"nodeId","property":{"language":"TestLanguage","version":"0","key":"DataTypeTestConcept-booleanValue_0_1"},"newValue":"true","commandId":"1","additionalInfos":[]}""",
            serializedCommand);

        var deltaEvent = new NotificationToDeltaEventMapper(new ParticipationIdProvider(), lionWebVersion).Map(notification);
        Assert.IsTrue(IdUtils.IsValid(deltaEvent.Id));
        Assert.IsTrue(IdUtils.IsValid(deltaEvent.OriginCommands![0].CommandId));
        Assert.IsTrue(IdUtils.IsValid(deltaEvent.OriginCommands![0].ParticipationId));
        var serializedEvent = deltaSerializer.Serialize(deltaEvent);
        Assert.AreEqual(
            """{"messageKind":"PropertyAdded","node":"nodeId","property":{"language":"TestLanguage","version":"0","key":"DataTypeTestConcept-booleanValue_0_1"},"newValue":"true","sequenceNumber":-1,"originCommands":[{"participationId":"participationId1","commandId":"myBase__23"}],"additionalInfos":[]}""",
            serializedEvent);
    }

    [TestMethod]
    public void SerializeParticipationNotificationId()
    {
        var notification = new PropertyAddedNotification(new DataTypeTestConcept("nodeId"), TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, true,
            new ParticipationNotificationId("myParticipation", "myCommand"));

        var lionWebVersion = TestLanguageLanguage.Instance.LionWebVersion;
        var deltaSerializer = new DeltaSerializer();

        var deltaCommand = new NotificationToDeltaCommandMapper(new CommandIdProvider(), lionWebVersion).Map(notification);
        Assert.IsTrue(IdUtils.IsValid(deltaCommand.Id));
        Assert.IsTrue(IdUtils.IsValid(deltaCommand.CommandId!));
        var serializedCommand = deltaSerializer.Serialize(deltaCommand);
        Assert.AreEqual(
            """{"messageKind":"AddProperty","node":"nodeId","property":{"language":"TestLanguage","version":"0","key":"DataTypeTestConcept-booleanValue_0_1"},"newValue":"true","commandId":"myCommand","additionalInfos":[]}""",
            serializedCommand);
        var deltaEvent = new NotificationToDeltaEventMapper(new ParticipationIdProvider(), lionWebVersion).Map(notification);
        Assert.IsTrue(IdUtils.IsValid(deltaEvent.Id));
        Assert.IsTrue(IdUtils.IsValid(deltaEvent.OriginCommands![0].CommandId));
        Assert.IsTrue(IdUtils.IsValid(deltaEvent.OriginCommands![0].ParticipationId));
        var serializedEvent = deltaSerializer.Serialize(deltaEvent);
        Assert.AreEqual(
            """{"messageKind":"PropertyAdded","node":"nodeId","property":{"language":"TestLanguage","version":"0","key":"DataTypeTestConcept-booleanValue_0_1"},"newValue":"true","sequenceNumber":-1,"originCommands":[{"participationId":"myParticipation","commandId":"myCommand"}],"additionalInfos":[]}""",
            serializedEvent);
    }
}