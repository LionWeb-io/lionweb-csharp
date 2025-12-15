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

namespace LionWeb.Protocol.Delta.Test.Client;

using Core;
using Core.M1;
using Core.M3;
using Core.Notification.Pipe;
using Core.Serialization;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Core.Utilities;
using Delta.Client;
using Delta.Repository;
using Message;
using Message.Event;

[TestClass]
public class ClientTests
{
    private const RepositoryId _repositoryId = "myRepo";

    private readonly TestDeltaRepositoryConnector _repositoryConnector;
    private readonly IForest _repositoryForest;
    private readonly TestPartition _repositoryPartition;
    private readonly LionWebTestRepository _repository;

    private readonly TestDeltaClientConnector _clientConnector;

    private readonly IForest _clientForest;
    private readonly TestPartition _clientPartition;
    private readonly LionWebTestClient _client;
    private readonly IClientInfo _clientInfo;

    public ClientTests()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        List<Language> languages = [TestLanguageLanguage.Instance];

        _repositoryConnector = new TestDeltaRepositoryConnector(lionWebVersion);
        _clientInfo = new ClientInfo { ParticipationId = "clientParticipation" };
        _clientConnector = new TestDeltaClientConnector(lionWebVersion);
        _clientConnector.Connect("clientId", _repositoryConnector);
        _repositoryConnector.Sender = content => _clientConnector.SendToClient(content);

        _repositoryForest = new Forest();
        _repository = new LionWebTestRepository(lionWebVersion, languages, "server", _repositoryForest,
            _repositoryConnector, Log);

        _clientForest = new Forest();
        _clientPartition = new TestPartition("partition");
        _client = new LionWebTestClient(lionWebVersion, languages, "client", _clientForest, _clientConnector, Log);
        _client.SignOn(_repositoryId);
        _client.ParticipationId = _clientInfo.ParticipationId;

        _clientForest.AddPartitions([_clientPartition]);
        _repository.WaitForReceived(1);
        _repositoryPartition = (TestPartition)_repositoryForest.Partitions.First();
    }

    [TestMethod]
    [Timeout(6000)]
    public void ConnectionWorks()
    {
        _clientPartition.Data = new DataTypeTestConcept("doc");

        _repository.WaitForReceived(1);

        AssertEquals(_clientPartition, _repositoryPartition);
    }

    [TestMethod]
    [Timeout(6000)]
    public void InOrderEvents()
    {
        _repositoryConnector.SendToAllClients(ChildAdded(0), []);
        _repositoryConnector.SendToAllClients(PropertyAdded(1), []);
        _repositoryConnector.SendToAllClients(PropertyChanged(2), []);

        AssertEquals(new TestPartition("partition") { Data = new DataTypeTestConcept("doc") { StringValue_0_1 = "changed text" } },
            _clientPartition);
    }

    [TestMethod]
    [Timeout(6000)]
    public void OutOfOrderEvents()
    {
        _repositoryConnector.SendToAllClients(PropertyAdded(1), []);
        _repositoryConnector.SendToAllClients(PropertyChanged(2), []);
        _repositoryConnector.SendToAllClients(ChildAdded(0), []);

        AssertEquals(new TestPartition("partition") { Data = new DataTypeTestConcept("doc") { StringValue_0_1 = "changed text" } },
            _clientPartition);
    }

    private ChildAdded ChildAdded(EventSequenceNumber sequenceNumber) =>
        new("partition",
            new DeltaSerializationChunk([
                new SerializedNode()
                {
                    Id = "doc",
                    Classifier = TestLanguageLanguage.Instance.DataTypeTestConcept.ToMetaPointer(),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = _repositoryPartition.GetId()
                }
            ]),
            TestLanguageLanguage.Instance.TestPartition_data.ToMetaPointer(),
            0,
            [new CommandSource(_clientInfo.ParticipationId, "cmdX")],
            null
        ) { SequenceNumber = sequenceNumber };

    private PropertyAdded PropertyAdded(EventSequenceNumber sequenceNumber) =>
        new("doc",
            TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1.ToMetaPointer(),
            "text",
            [new CommandSource(_clientInfo.ParticipationId, "cmdY")],
            null
        ) { SequenceNumber = sequenceNumber };

    private PropertyChanged PropertyChanged(EventSequenceNumber sequenceNumber) =>
        new("doc",
            TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1.ToMetaPointer(),
            "changed text",
            "text",
            [new CommandSource(_clientInfo.ParticipationId, "cmdZ")],
            null
        ) { SequenceNumber = sequenceNumber };

    protected TestPartition Clone(TestPartition node) =>
        (TestPartition)new SameIdCloner([node]) { IncludingReferences = true }.Clone()[node];

    protected void AssertEquals(INode? a, INode? b) =>
        AssertEquals([a], [b]);

    protected void AssertEquals(IEnumerable<INode?> a, IEnumerable<INode?> b)
    {
        List<IDifference> differences = new Comparer(a.ToList(), b.ToList()).Compare().ToList();
        Assert.IsTrue(differences.Count == 0,
            differences.DescribeAll(new() { LeftDescription = "a", RightDescription = "b" }));
    }

    private static void Log(string message) =>
        Console.WriteLine(message);
}