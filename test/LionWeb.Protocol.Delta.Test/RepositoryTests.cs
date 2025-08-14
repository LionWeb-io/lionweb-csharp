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
using Core.Test.Languages.Generated.V2023_1.Shapes.M2;
using Core.Utilities;
using Message;
using Message.Event;
using Repository;
using Repository.Forest;
using Repository.Partition;
using System.Text;

[TestClass]
public class RepositoryTests
{
    private readonly IForest _repositoryForest;
    private readonly LionWebTestRepository _repository;
    private readonly RepositoryConnector _repositoryConnector;

    private readonly IForest _aForest;
    private readonly LionWebTestClient _aClient;

    private readonly IForest _bForest;
    private readonly LionWebTestClient _bClient;
    private ClientConnector _aConnector;
    private ClientConnector _bConnector;
    private readonly List<Language> _languages;
    private readonly IVersion2023_1 _lionWebVersion;

    public RepositoryTests()
    {
        _lionWebVersion = LionWebVersions.v2023_1;
        _languages = [ShapesLanguage.Instance];

        _repositoryForest = new Forest();
        _repositoryConnector = new(_lionWebVersion);
        _repository = new LionWebTestRepository(_lionWebVersion, _languages, "repository", _repositoryForest,
            _repositoryConnector);

        _aClient = CreateClient("A", out _aForest, out _aConnector);
        _bClient = CreateClient("B", out _bForest, out _bConnector);
    }

    private LionWebTestClient CreateClient(string name, out IForest forest, out ClientConnector connector)
    {
        var participation = $"{name}Participation";
        forest = new Forest();
        connector = new ClientConnector(_lionWebVersion);
        var client = new LionWebTestClient(_lionWebVersion, _languages, name, forest, connector)
        {
            ParticipationId = participation
        };
        connector.Connect(participation, _repositoryConnector);
        return client;
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Partition()
    {
        // await _aClient.SignOn();
        // _aClient.WaitForReceived(1);
        //
        // await _bClient.SignOn();
        // _bClient.WaitForReceived(1);

        _aForest.AddPartitions([new Geometry("partition")]);
        _bClient.WaitForReceived(1);

        AssertEquals(_aForest.Partitions, _bForest.Partitions);
    }

    [TestMethod]
    // [Timeout(6000)]
    public async Task PartitionAnnotation()
    {
        // await _aClient.SignOn();
        // _aClient.WaitForReceived(1);
        //
        // await _bClient.SignOn();
        // _bClient.WaitForReceived(1);

        _aForest.AddPartitions([new Geometry("partition")]);
        _bClient.WaitForReceived(1);

        var bPartition = (Geometry)_bForest.Partitions.First();
        Assert.IsNotNull(bPartition);
        
        bPartition.AddAnnotations([new BillOfMaterials("bom")]);
        _aClient.WaitForReceived(1);

        AssertEquals(_aForest.Partitions, _bForest.Partitions);
    }

    protected void AssertEquals(IReadableNode? a, IReadableNode? b) =>
        AssertEquals([a], [b]);

    protected void AssertEquals(IEnumerable<IReadableNode?> a, IEnumerable<IReadableNode?> b)
    {
        List<IDifference> differences = new Comparer(a.ToList(), b.ToList()).Compare().ToList();
        Assert.IsTrue(differences.Count == 0,
            differences.DescribeAll(new() { LeftDescription = "a", RightDescription = "b" }));
    }

    public static void Run(Action action)
        // => Task.Run(() =>
    {
        action();
    }
    // );
}

class RepositoryConnector : IDeltaRepositoryConnector
{
    private readonly DeltaSerializer _deltaSerializer = new();
    private readonly NotificationToDeltaEventMapper _mapper;
    private readonly Dictionary<IClientInfo, ClientConnector> _clients = [];

    public RepositoryConnector(LionWebVersions lionWebVersion)
    {
        var exceptionParticipationIdProvider = new ExceptionParticipationIdProvider();
        _mapper = new(
            new PartitionNotificationToDeltaEventMapper(exceptionParticipationIdProvider, lionWebVersion),
            new ForestNotificationToDeltaEventMapper(exceptionParticipationIdProvider, lionWebVersion)
        );
    }

    public void AddClient(IClientInfo clientInfo, ClientConnector clientConnector) =>
        _clients[clientInfo] = clientConnector;

    public async Task SendToClient(IClientInfo clientInfo, IDeltaContent content)
    {
        if (_clients.TryGetValue(clientInfo, out var clientConnector))
        {
            RepositoryTests.Run(() =>
            {
                var encoded = Encode(clientInfo, content);
                clientConnector.MessageFromRepository(encoded);
            });
        }
    }

    public async Task SendToAllClients(IDeltaContent content)
    {
        foreach ((var clientInfo, var clientConnector) in _clients)
        {
            RepositoryTests.Run(() =>
            {
                var encoded = Encode(clientInfo, content);
                clientConnector.MessageFromRepository(encoded);
            });
        }
    }

    private byte[] Encode(IClientInfo clientInfo, IDeltaContent content) =>
        Encode(_deltaSerializer.Serialize(UpdateSequenceNumber(content, clientInfo)));

    private static byte[] Encode(string msg) =>
        Encoding.UTF8.GetBytes(msg);

    private static IDeltaContent UpdateSequenceNumber(IDeltaContent content, IClientInfo clientInfo)
    {
        if (content is IDeltaEvent ev)
        {
            ev.SequenceNumber = clientInfo.IncrementAndGetSequenceNumber();
        }

        return content;
    }

    public event EventHandler<IMessageContext<IDeltaContent>>? ReceiveFromClient;

    public IDeltaContent Convert(INotification notification) =>
        _mapper.Map(notification);

    public void MessageFromClient(ClientInfo clientInfo, byte[] encoded) =>
        RepositoryTests.Run(() =>
        {
            var deltaContent = _deltaSerializer.Deserialize<IDeltaContent>(Encoding.UTF8.GetString(encoded));
            ReceiveFromClient?.Invoke(this, new DeltaMessageContext(clientInfo, deltaContent));
        });
}

class ClientConnector : IDeltaClientConnector
{
    private readonly DeltaSerializer _deltaSerializer = new();
    private readonly NotificationToDeltaCommandMapper _mapper;
    private RepositoryConnector _repositoryConnector;
    private ClientInfo _clientInfo;


    public ClientConnector(LionWebVersions lionWebVersion)
    {
        var commandIdProvider = new CommandIdProvider();
        _mapper = new(
            new PartitionNotificationToDeltaCommandMapper(commandIdProvider, lionWebVersion),
            new ForestNotificationToDeltaCommandMapper(commandIdProvider, lionWebVersion)
        );
    }

    public void Connect(ParticipationId participationId, RepositoryConnector repositoryConnector)
    {
        _clientInfo = new ClientInfo { ParticipationId = participationId };
        repositoryConnector.AddClient(_clientInfo, this);
        _repositoryConnector = repositoryConnector;
    }

    public Task SendToRepository(IDeltaContent content)
    {
        RepositoryTests.Run(() =>
        {
            var encoded = Encode(content);
            _repositoryConnector.MessageFromClient(_clientInfo, encoded);
        });

        return Task.CompletedTask;
    }

    private byte[] Encode(IDeltaContent content) =>
        Encode(_deltaSerializer.Serialize(content));

    private static byte[] Encode(string msg) =>
        Encoding.UTF8.GetBytes(msg);

    public event EventHandler<IDeltaContent>? ReceiveFromRepository;

    public IDeltaContent Convert(INotification notification) =>
        _mapper.Map(notification);

    public void MessageFromRepository(byte[] encoded) =>
        RepositoryTests.Run(() =>
        {
            var deltaContent = _deltaSerializer.Deserialize<IDeltaContent>(Encoding.UTF8.GetString(encoded));
            ReceiveFromRepository?.Invoke(this, deltaContent);
        });
}