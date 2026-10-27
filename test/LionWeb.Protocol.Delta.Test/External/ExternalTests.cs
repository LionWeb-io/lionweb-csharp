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

namespace LionWeb.Protocol.Delta.Test.External;

using Client;
using Core;
using Core.M1;
using Core.Serialization;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using Core.Test.Notification;
using Core.Utilities;
using Delta.Client;
using Delta.Repository;
using Message;
using Message.Command;
using Message.Event;

[TestClass]
public class ExternalTests : DeltaTestsBase
{
    private const string _repositoryId = "repo";
    private const string _clientBase = "../../../resources/external/client";
    private const string _repositoryBase = "../../../resources/external/repository";

    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(2);
    private static readonly LionWebVersions _lionWebVersion = TestLanguageLanguage.Instance.LionWebVersion;

    [TestMethod]
    [Ignore("Setup only")]
    public void Create00Model()
    {
        var linkA = new LinkTestConcept("linkA");
        Serialize(new TestPartition("partition")
            {
                Name = "My Partition",
                Links =
                [
                    linkA,
                    new LinkTestConcept("linkB") { Containment_0_1 = new LinkTestConcept("childB0"), Reference_0_1 = linkA }.WithAnnotation(new TestAnnotation("ann")
                    {
                        Containment = new DataTypeTestConcept("dataNode") { BooleanValue_0_1 = true, BooleanValue_1 = false }
                    })
                ]
            }, $"{_clientBase}/00model.json");
    }

    [TestMethod]
    [Ignore("Setup only")]
    public void Create01Event() =>
        Serialize(new PropertyChanged(
                "partition",
                _lionWebVersion.BuiltIns.INamed_name.ToMetaPointer(),
                "Changed partition",
                "My Partition",
                [new CommandSource("otherParticipation", "cmd01")],
                []
            ) { SequenceNumber = 1 }, $"{_clientBase}/01event.json");

    [TestMethod]
    [Ignore("Setup only")]
    public void Create02Event() =>
        Serialize(new ChildAdded(
                "partition",
                SerializeToDelta(new LinkTestConcept("newChild") { Containment_0_n = [new LinkTestConcept("newSubChild")] }),
                TestLanguageLanguage.Instance.TestPartition_links.ToMetaPointer(),
                2,
                [new CommandSource("otherParticipation", "cmd02")],
                []
            ) { SequenceNumber = 2 }, $"{_clientBase}/02event.json");

    [TestMethod]
    [Ignore("Setup only")]
    public void Create01Command() =>
        Serialize(new ChangeProperty(
                "partition",
                _lionWebVersion.BuiltIns.INamed_name.ToMetaPointer(),
                "Changed partition",
                "cmd01",
                []
            ), $"{_repositoryBase}/01command.json");

    [TestMethod]
    [Ignore("Setup only")]
    public void Create02Command() =>
        Serialize(new AddChild(
                "partition",
                SerializeToDelta(new LinkTestConcept("newChild") { Containment_0_n = [new LinkTestConcept("newSubChild")] }),
                TestLanguageLanguage.Instance.TestPartition_links.ToMetaPointer(),
                2,
                "cmd02",
                []
            ), $"{_repositoryBase}/02command.json");

    [TestMethod]
    public void TestClient()
    {
        var basePath = _clientBase;

        List<IGrouping<string, FileInfo>> pairs = ListFilePairs(basePath);

        var forest = DeserializeModel(pairs.First().Single());

        var connector = new TestDeltaClientConnector(_lionWebVersion);
        using var client = new LionWebTestClient(_lionWebVersion, [TestLanguageLanguage.Instance], "testClient", forest, connector, s => TestContext.WriteLine(s));
        client.ParticipationId = "dummy";

        foreach (var pair in pairs.Skip(1))
        {
            var evt = DeserializeDelta<IDeltaEvent>(pair.First(f => f.Name.Contains("event")));
            connector.SendToClient(evt).Wait(_timeout);

            var nextForest = DeserializeModel(pair.First(f => f.Name.Contains("model")));

            AssertEqual(nextForest, forest);
        }
    }

    [TestMethod]
    public async Task TestRepository()
    {
        var basePath = _repositoryBase;

        List<IGrouping<string, FileInfo>> pairs = ListFilePairs(basePath);

        var repositoryForest = new Forest();
        var repositoryConnector = new DeltaRepositoryConnector(_lionWebVersion);
        using var repository = new LionWebTestRepository(_lionWebVersion, [TestLanguageLanguage.Instance], "testRepository", repositoryForest, repositoryConnector, s => TestContext.WriteLine(s));

        var receivingClientForest = new Forest();
        var receivingClientConnector = new TestDeltaClientConnector(_lionWebVersion);
        using var receivingClient = new LionWebTestClient(_lionWebVersion, [TestLanguageLanguage.Instance], "receivingClient", receivingClientForest, receivingClientConnector,
            s => TestContext.WriteLine(s));
        receivingClientConnector.Connect("receivingClient", repositoryConnector);
        await receivingClient.SignOn(_repositoryId);
        await receivingClient.SubscribeToChangingPartitions(true, true);


        var sendingClientForest = new Forest();
        var sendingClientConnector = new TestDeltaClientConnector(_lionWebVersion);
        using var sendingClient = new LionWebTestClient(_lionWebVersion, [TestLanguageLanguage.Instance], "sendingClient", sendingClientForest, sendingClientConnector, s => TestContext.WriteLine(s));
        sendingClientConnector.Connect("sendingClient", repositoryConnector);
        await sendingClient.SignOn(_repositoryId);
        await sendingClient.SubscribeToChangingPartitions(true, true);

        sendingClientForest.AddPartitions(DeserializeModel(pairs.First().Single()).Partitions);

        foreach (var pair in pairs.Skip(1))
        {
            var cmd = DeserializeDelta<IDeltaCommand>(pair.First(f => f.Name.Contains("command")));
            await sendingClientConnector.SendToRepository(cmd);

            var nextForest = DeserializeModel(pair.First(f => f.Name.Contains("model")));

            AssertEqual(nextForest, repositoryForest);
            AssertEqual(nextForest, receivingClientForest);
        }
    }

    private static List<IGrouping<string, FileInfo>> ListFilePairs(string basePath)
    {
        var pairs = new DirectoryInfo(basePath)
            .EnumerateFiles()
            .GroupBy(f => f.Name[..2])
            .OrderBy(g => g.Key)
            .ToList();
        return pairs;
    }

    private static void AssertEqual(IForest expected, IForest actual)
    {
        var differences = new IdComparer(expected.Partitions.Cast<IReadableNode?>().ToList(), actual.Partitions.Cast<IReadableNode?>().ToList()).Compare().ToList();

        Assert.IsEmpty(differences, differences.DescribeAll(new ComparerOutputConfig { LeftDescription = "expected", RightDescription = "actual" }));
    }

    private static IForest DeserializeModel(FileInfo fileInfo)
    {
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build();

        var result = new Forest();

        result.AddPartitions(deserializer.Deserialize(JsonUtils.ReadJsonFromString<SerializationChunk>(File.ReadAllText(fileInfo.FullName))).Cast<TestPartition>());

        return result;
    }

    private static void Serialize(IReadableNode node, string fileName)
    {
        var serializer = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build();

        JsonUtils.WriteJsonToFile(fileName, serializer.SerializeToChunk([node]));
    }

    private static DeltaSerializationChunk SerializeToDelta(IReadableNode node)
    {
        var serializer = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build();

        var chunk = serializer.SerializeToChunk([node]);

        return new DeltaSerializationChunk(chunk.Nodes);
    }

    private static T DeserializeDelta<T>(FileInfo fileInfo) where T : class, IDeltaContent =>
        new DeltaSerializer().Deserialize<T>(File.ReadAllText(fileInfo.FullName));

    private static void Serialize(IDeltaContent deltaContent, string fileName)
    {
        var serializer = new DeltaSerializer();
        File.WriteAllText(fileName, serializer.Serialize(deltaContent));
    }

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext { get; set; }
}