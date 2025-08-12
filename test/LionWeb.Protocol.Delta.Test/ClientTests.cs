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

using Client;
using Client.Forest;
using Client.Partition;
using Core;
using Core.M1;
using Core.M3;
using Core.Notification;
using Core.Notification.Processor;
using Core.Serialization;
using Core.Test.Languages.Generated.V2023_1.Shapes.M2;
using Core.Utilities;
using Message;
using Message.Event;
using Repository;

[TestClass]
public class ClientTests
{
    private readonly DeltaRepositoryConnector _repositoryConnector;
    private readonly IForest _repositoryForest;
    private readonly Geometry _repositoryPartition;
    private readonly LionWebTestRepository _repository;

    private readonly DeltaClientConnector _clientConnector;
    private readonly IForest _clientForest;
    private readonly Geometry _clientPartition;
    private readonly LionWebTestClient _client;
    private readonly IClientInfo _clientInfo;

    public ClientTests()
    {
        var lionWebVersion = LionWebVersions.v2023_1;
        List<Language> languages = [ShapesLanguage.Instance];

        _repositoryConnector = new DeltaRepositoryConnector(lionWebVersion);
        _clientInfo = new ClientInfo { ParticipationId = "clientParticipation" };
        _clientConnector = new(lionWebVersion,
            content => _repositoryConnector.ReceiveMessageFromClient(new DeltaMessageContext(_clientInfo, content)));
        _repositoryConnector.Sender = content => _clientConnector.ReceiveMessageFromRepository(content);

        _repositoryForest = new Forest();
        _repository = new LionWebTestRepository(lionWebVersion, languages, "server", _repositoryForest,
            _repositoryConnector);

        _clientForest = new Forest();
        _clientPartition = new Geometry("partition");
        _client = new LionWebTestClient(lionWebVersion, languages, "client", _clientForest, _clientConnector);
        _client.ParticipationId = _clientInfo.ParticipationId;
        
        _clientForest.AddPartitions([_clientPartition]);
        _repository.WaitForReceived(1);
        _repositoryPartition = (Geometry)_repositoryForest.Partitions.First();
    }

    [TestMethod]
    [Timeout(6000)]
    public void ConnectionWorks()
    {
        _clientPartition.Documentation = new Documentation("doc");

         _repository.WaitForReceived(1);

        AssertEquals(_clientPartition, _repositoryPartition);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task SignOn()
    {
        var signOnResponse = await _client.SignOn();

        Assert.AreEqual("clientParticipation", signOnResponse.ParticipationId);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task GetAvailableIds()
    {
        var availableIdsResponse = await _client.GetAvailableIds(11);

        Assert.AreEqual(11, availableIdsResponse.Ids.Length);
        foreach (var freeId in availableIdsResponse.Ids)
        {
            Assert.IsTrue(IdUtils.IsValid(freeId));
        }
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task GetAvailableIds_SomeAlreadyUsed()
    {
        await _client.SignOn();

        var usedNodeId = "repoProvidedId-6";
        _clientPartition.Documentation = new Documentation(usedNodeId);

        var availableIdsResponse = await _client.GetAvailableIds(11);

        Assert.AreEqual(11, availableIdsResponse.Ids.Length);
        foreach (var freeId in availableIdsResponse.Ids)
        {
            Assert.IsTrue(IdUtils.IsValid(freeId));
        }

        Assert.IsFalse(availableIdsResponse.Ids.Contains(usedNodeId));
    }

    [TestMethod]
    [Timeout(6000)]
    public void InOrderEvents()
    {
        _repositoryConnector.SendToAllClients(ChildAdded(0));
        _repositoryConnector.SendToAllClients(PropertyAdded(1));
        _repositoryConnector.SendToAllClients(PropertyChanged(2));

        AssertEquals(new Geometry("partition") { Documentation = new Documentation("doc") { Text = "changed text" } },
            _clientPartition);
    }

    [TestMethod]
    [Timeout(6000)]
    public void OutOfOrderEvents()
    {
        _repositoryConnector.SendToAllClients(PropertyAdded(1));
        _repositoryConnector.SendToAllClients(PropertyChanged(2));
        _repositoryConnector.SendToAllClients(ChildAdded(0));

        AssertEquals(new Geometry("partition") { Documentation = new Documentation("doc") { Text = "changed text" } },
            _clientPartition);
    }

    private ChildAdded ChildAdded(EventSequenceNumber sequenceNumber) =>
        new("partition",
            new DeltaSerializationChunk([
                new SerializedNode()
                {
                    Id = "doc",
                    Classifier = ShapesLanguage.Instance.Documentation.ToMetaPointer(),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = _repositoryPartition.GetId()
                }
            ]),
            ShapesLanguage.Instance.Geometry_documentation.ToMetaPointer(),
            0,
            [new CommandSource(_clientInfo.ParticipationId, "cmdX")],
            null
        ) { SequenceNumber = sequenceNumber };

    private PropertyAdded PropertyAdded(EventSequenceNumber sequenceNumber) =>
        new("doc",
            ShapesLanguage.Instance.Documentation_text.ToMetaPointer(),
            "text",
            [new CommandSource(_clientInfo.ParticipationId, "cmdY")],
            null
        ) { SequenceNumber = sequenceNumber };

    private PropertyChanged PropertyChanged(EventSequenceNumber sequenceNumber) =>
        new("doc",
            ShapesLanguage.Instance.Documentation_text.ToMetaPointer(),
            "changed text",
            "text",
            [new CommandSource(_clientInfo.ParticipationId, "cmdZ")],
            null
        ) { SequenceNumber = sequenceNumber };

    protected Geometry Clone(Geometry node) =>
        (Geometry)new SameIdCloner([node]) { IncludingReferences = true }.Clone()[node];

    protected void AssertEquals(INode? a, INode? b) =>
        AssertEquals([a], [b]);

    protected void AssertEquals(IEnumerable<INode?> a, IEnumerable<INode?> b)
    {
        List<IDifference> differences = new Comparer(a.ToList(), b.ToList()).Compare().ToList();
        Assert.IsTrue(differences.Count == 0,
            differences.DescribeAll(new() { LeftDescription = "a", RightDescription = "b" }));
    }
}

internal class DeltaRepositoryConnector : IDeltaRepositoryConnector
{
    private readonly EventToDeltaCommandMapper _mapper;

    public DeltaRepositoryConnector(LionWebVersions lionWebVersion)
    {
        var commandIdProvider = new CommandIdProvider();
        _mapper = new EventToDeltaCommandMapper(
            new PartitionEventToDeltaCommandMapper(commandIdProvider, lionWebVersion),
            new ForestEventToDeltaCommandMapper(commandIdProvider, lionWebVersion)
        );
    }

    public Action<IDeltaContent> Sender { get; set; }

    public Task SendToClient(IClientInfo clientInfo, IDeltaContent content)
    {
        Sender?.Invoke(content);
        return Task.CompletedTask;
    }

    public Task SendToAllClients(IDeltaContent content)
    {
        Sender?.Invoke(content);
        return Task.CompletedTask;
    }

    public event EventHandler<IMessageContext<IDeltaContent>>? ReceiveFromClient;
    public void ReceiveMessageFromClient(IDeltaMessageContext context) => ReceiveFromClient?.Invoke(null, context);
    public IDeltaContent Convert(INotification notification) => _mapper.Map(notification);
}

internal class DeltaClientConnector : IDeltaClientConnector
{
    private readonly Action<IDeltaContent> _sender;
    private readonly EventToDeltaCommandMapper _mapper;

    public DeltaClientConnector(LionWebVersions lionWebVersion, Action<IDeltaContent> sender)
    {
        _sender = sender;
        var commandIdProvider = new CommandIdProvider();
        _mapper = new EventToDeltaCommandMapper(
            new PartitionEventToDeltaCommandMapper(commandIdProvider, lionWebVersion),
            new ForestEventToDeltaCommandMapper(commandIdProvider, lionWebVersion)
        );
    }

    public Task SendToRepository(IDeltaContent content)
    {
        _sender(content);
        return Task.CompletedTask;
    }

    public event EventHandler<IDeltaContent>? ReceiveFromRepository;

    public void ReceiveMessageFromRepository(IDeltaContent context) => ReceiveFromRepository?.Invoke(null, context);
    public IDeltaContent Convert(INotification notification) => _mapper.Map(notification);
}