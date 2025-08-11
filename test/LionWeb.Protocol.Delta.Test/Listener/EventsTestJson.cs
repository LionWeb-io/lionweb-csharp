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

namespace LionWeb.Protocol.Delta.Test.Listener;

using Client;
using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Partition;
using Core.M3;
using Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using Core.Test.Listener;
using Message.Command;

[TestClass]
public class EventsTestJson : EventTestsBase
{
    protected override Geometry CreateReplicator(Geometry node)
    {
        var clone = Clone(node);

        var lionWebVersion = LionWebVersions.v2024_1;
        List<Language> languages = [ShapesLanguage.Instance, lionWebVersion.BuiltIns, lionWebVersion.LionCore];

        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap = DeltaUtils.BuildSharedKeyMap(languages);

        SharedNodeMap sharedNodeMap = new();

        var partitionEventHandler = new PartitionEventHandler(null);

        var deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
            ;

        var eventReceiver = new DeltaProtocolPartitionEventReceiver(
            partitionEventHandler,
            sharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder
        );

        var deltaSerializer = new DeltaSerializer();

        var commandToEventMapper = new DeltaCommandToDeltaEventMapper("myParticipation", sharedNodeMap);
        node.GetPublisher().Subscribe<IPartitionNotification>((sender, partitionEvent) =>
        {
            var command = new PartitionEventToDeltaCommandMapper(new CommandIdProvider(), lionWebVersion).Map(partitionEvent);
            var json = deltaSerializer.Serialize(command);
            var deserialized = deltaSerializer.Deserialize<IDeltaCommand>(json);
            eventReceiver.Receive(commandToEventMapper.Map(deserialized));
        });

        var replicator = new PartitionEventReplicator(clone, sharedNodeMap);
        replicator.ReplicateFrom(partitionEventHandler);
        return clone;
    }
}