// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Benchmark;

using BenchmarkDotNet.Attributes;
using M1;
using M3;
using Notification;
using Notification.Forest;
using Notification.Pipe;
using Protocol.Delta;
using Protocol.Delta.Client;
using Protocol.Delta.Message.Command;
using Protocol.Delta.Repository;
using Test.Languages.Generated.V2024_1.TestLanguage;
using Test.Notification.Replicator;
using Utilities;

[MemoryDiagnoser]
public class ReplicationBenchmark : BenchmarkBase
{
    private static readonly IVersion2024_1 _lionWebVersion = LionWebVersions.v2024_1;
    private const long _nodeCount = 5_900;

    [Benchmark]
    public void ReplicateDirectOneWay()
    {
        var originalForest = new Forest();
        var clonedForest = new Forest();

        var sharedNodeMap = new SharedNodeMap();
        var notificationMapper = new NotificationMapper(sharedNodeMap);
        originalForest.GetNotificationSender()!.ConnectTo(notificationMapper);

        var replicator = ForestReplicator.Create(clonedForest, sharedNodeMap, null);
        notificationMapper.ConnectTo(replicator);

        FillForest(originalForest, clonedForest);
    }

    [Benchmark]
    public void ReplicateDirectTwoWay()
    {
        var originalForest = new Forest();
        var clonedForest = new Forest();

        var replicatorMap = new SharedNodeMap();
        var cloneMap = new SharedNodeMap();
        var replicator = ForestReplicator.Create(originalForest, replicatorMap, "nodeReplicator");
        var cloneReplicator = ForestReplicator.Create(clonedForest, cloneMap, "cloneReplicator");

        var cloneMapper = new NotificationMapper(cloneMap);
        replicator.ConnectTo(cloneMapper);
        cloneMapper.ConnectTo(cloneReplicator);

        var mapper = new NotificationMapper(replicatorMap);
        cloneReplicator.ConnectTo(mapper);
        mapper.ConnectTo(replicator);

        var originalPartition = new TestPartition("partition");
        originalForest.AddPartitions([originalPartition]);
        var clonedPartition = (TestPartition)clonedForest.Partitions.First();

        List<LinkTestConcept> list = SerializerBenchmark.CreateNodes(_nodeCount).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            var partition = i % 2 == 0 ? originalPartition : clonedPartition; 
            LinkTestConcept node = list[i];

            if (node.GetParent() is null)
            {
                partition.AddLinks([node]);
            } else
            {
                partition.Links.LastOrDefault()?.AddContainment_0_n([node]);
            }

            if (i % 13 == 0)
            {
                partition.Links[i / 2].Reference_0_1 = node;
            }
        }

        CheckPartitionEqual(originalForest, clonedForest);
    }

    [Benchmark]
    public void ReplicateJsonOneWay()
    {
        var originalForest = new Forest();
        var clonedForest = new Forest();

        List<Language> languages = [TestLanguageLanguage.Instance, _lionWebVersion.BuiltIns, _lionWebVersion.LionCore];

        var sharedKeyedMap = SharedKeyedMapBuilder.BuildSharedKeyMap(languages);

        PartitionSharedNodeMap sharedNodeMap = new();

        var deserializerBuilder = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithLanguages(languages)  
            .WithHandler(new DeltaDeserializerHandler());

        var replicator = ForestReplicator.Create(clonedForest, sharedNodeMap, "cloneReplicator");

        var commandReceiver = new DeltaProtocolCommandReceiver(
            sharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder);
        
        commandReceiver.ConnectTo(replicator);

        var deltaNotificationReceiver = new DeltaNotificationReceiver(commandReceiver);
        
        originalForest.GetNotificationSender()!.ConnectTo(deltaNotificationReceiver);

        FillForest(originalForest, clonedForest);
    }

    private class DeltaNotificationReceiver(DeltaProtocolCommandReceiver commandReceiver) : INotificationReceiver
    {
        private readonly NotificationToDeltaCommandMapper _commandMapper = new(new CommandIdProvider(), _lionWebVersion);
        private readonly DeltaSerializer _deltaSerializer = new();
        
        public void Receive(INotificationSender correspondingSender, INotification notification)
        {
            var deltaCommand = _commandMapper.Map(notification);
            var json = _deltaSerializer.Serialize(deltaCommand);
            var deserializedCommand = _deltaSerializer.Deserialize<IDeltaCommand>(json);
            commandReceiver.Receive(deserializedCommand);
        }
    }

    private static void FillForest(Forest originalForest, Forest clonedForest)
    {
        var originalPartition = new TestPartition("partition");
        originalForest.AddPartitions([originalPartition]);

        List<LinkTestConcept> list = SerializerBenchmark.CreateNodes(_nodeCount).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            LinkTestConcept node = list[i];

            if (node.GetParent() is null)
            {
                originalPartition.AddLinks([node]);
            } else
            {
                originalPartition.Links.LastOrDefault()?.AddContainment_0_n([node]);
            }

            if (i % 13 == 0)
            {
                originalPartition.Links[i / 2].Reference_0_1 = node;
            }
        }

        CheckPartitionEqual(originalForest, clonedForest);
    }

    private static void CheckPartitionEqual(Forest originalForest, Forest clonedForest)
    {
        var differences = new Comparer(originalForest.Partitions.Cast<IReadableNode>().ToList(), clonedForest.Partitions.Cast<IReadableNode>().ToList()).Compare().ToList();

        if (differences.Count > 0)
        {
            Console.Error.WriteLine(differences.DescribeAll(new ComparerOutputConfig { LeftDescription = "Original", RightDescription = "Clone" }));

            throw new Exception("Clones not equal");
        }
    }
}