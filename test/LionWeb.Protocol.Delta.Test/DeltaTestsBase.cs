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

using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using LionWeb.Core.Notification;
using LionWeb.Core.Notification.Forest;
using LionWeb.Core.Notification.Partition;
using LionWeb.Core.Notification.Pipe;
using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;
using LionWeb.Core.Test.Notification;
using LionWeb.Protocol.Delta.Client;
using LionWeb.Protocol.Delta.Message.Command;

namespace LionWeb.Protocol.Delta.Test;

public abstract class DeltaTestsBase: NotificationTestsBase
{
    /// <inheritdoc cref="CreateDeltaReplicator(IForest, INotificationSender)"/>
    protected T CreateDeltaReplicator<T>(T node) where T : IPartitionInstance, INode
    {
        var clone = ClonePartition(node);
        var cloneForest = new Forest();
        cloneForest.AddPartitions([clone]);

        CreateDeltaReplicator(cloneForest, node.GetNotificationSender()!);

        return clone;
    }

    /// <inheritdoc cref="CreateDeltaReplicator(IForest, INotificationSender)"/>
    protected IForest CreateDeltaReplicator(IForest originalForest)
    {
        var cloneForest = new Forest();
        cloneForest.AddPartitions(originalForest.Partitions.Select(p => (IPartitionInstance)ClonePartition((INode)p)));

        CreateDeltaReplicator(cloneForest, originalForest.GetNotificationSender()!);

        return cloneForest;
    }
    
    /// <summary>
    /// This replicator exercises the following path:
    /// 
    /// notification
    /// -> notificationToDeltaCommandMapper
    /// -> commandToEventMapper
    /// -> deltaProtocolEventReceiver
    /// -> deltaEventToNotificationMapper
    /// -> forestReplicator
    /// 
    /// ForestReplicator replicates notifications on clone partitions
    /// </summary>
    private void CreateDeltaReplicator(IForest cloneForest, INotificationSender originalSender)
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        List<Language> languages = [ShapesLanguage.Instance, TestLanguageLanguage.Instance, lionWebVersion.BuiltIns, lionWebVersion.LionCore];

        SharedKeyedMap sharedKeyedMap = SharedKeyedMapBuilder.BuildSharedKeyMap(languages);

        PartitionSharedNodeMap sharedNodeMap = new();

        var unresolvedReferencesManager = new UnresolvedReferencesManager();

        var deserializerBuilder = new DeserializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithLanguages(languages)
            .WithHandler(new DeltaDeserializerHandler(unresolvedReferencesManager));

        cloneForest.GetNotificationSender()!.ConnectTo(unresolvedReferencesManager);
        
        var eventReceiver = new DeltaProtocolEventReceiver(
            sharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder);

        var replicator = ForestReplicator.Create(cloneForest, sharedNodeMap, "cloneReplicator");
        
        eventReceiver.ConnectTo(replicator);

        var commandToEventMapper = new DeltaCommandToDeltaEventMapper("myParticipation", sharedNodeMap);
        var notificationToDeltaCommandMapper = new NotificationToDeltaCommandMapper(new CommandIdProvider(), lionWebVersion);
        
        originalSender.ConnectTo(new ForwardingReceiver(notification =>
        {
            var deltaCommand = notificationToDeltaCommandMapper.Map(notification);
            eventReceiver.Receive(commandToEventMapper.Map(deltaCommand));
        }));
    }
    
    /// <summary>
    /// This replicator exercises the following path:
    /// 
    /// notification
    /// -> notificationToDeltaCommandMapper
    /// -> deltaSerializer
    /// -> json
    /// -> deltaDeserializer
    /// -> commandToEventMapper
    /// -> deltaProtocolEventReceiver
    /// -> deltaEventToNotificationMapper
    /// -> forestReplicator
    /// 
    /// ForestReplicator replicates notifications on clone partitions.
    /// 
    /// Different from <see cref="CreateDeltaReplicator"/> this replicator serialize and deserialize delta commands.
    /// </summary>
    protected T CreateDeltaReplicatorWithJson<T>(T node) where T : INode
    {
        var clone = ClonePartition(node);

        var lionWebVersion = LionWebVersions.v2024_1;
        List<Language> languages = [ShapesLanguage.Instance, TestLanguageLanguage.Instance, lionWebVersion.BuiltIns, lionWebVersion.LionCore];

        SharedKeyedMap sharedKeyedMap = SharedKeyedMapBuilder.BuildSharedKeyMap(languages);

        PartitionSharedNodeMap sharedNodeMap = new();

        var deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)  
                .WithHandler(new DeltaDeserializerHandler());

        var cloneForest = new Forest();
        cloneForest.AddPartitions([(IPartitionInstance)clone]);
        
        var replicator = ForestReplicator.Create(cloneForest, sharedNodeMap, "cloneReplicator");

        var eventReceiver = new DeltaProtocolEventReceiver(
            sharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder);
        
        eventReceiver.ConnectTo(replicator);

        var deltaSerializer = new DeltaSerializer();

        var commandToEventMapper = new DeltaCommandToDeltaEventMapper("myParticipation", sharedNodeMap);
        var notificationToDeltaCommandMapper = new NotificationToDeltaCommandMapper(new CommandIdProvider(), lionWebVersion);
        
        node.GetPartition()!.GetNotificationSender()!.ConnectTo(new ForwardingReceiver(notification =>
        {
            if (notification is IPartitionNotification partitionNotification)
            {
                var command = notificationToDeltaCommandMapper.Map(partitionNotification);
                var json = deltaSerializer.Serialize(command);
                var deserialized = deltaSerializer.Deserialize<IDeltaCommand>(json);
                eventReceiver.Receive(commandToEventMapper.Map(deserialized));
            }
        }));

        return clone;
    }

    private class ForwardingReceiver(Action<INotification> handler) : INotificationReceiver
    {
        public void Receive(INotificationSender correspondingSender, INotification notification) =>
            handler(notification);
    }
}
