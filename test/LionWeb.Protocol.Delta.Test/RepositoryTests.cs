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
using Core;
using Core.M1;
using Core.M3;
using Core.Test.Languages.Generated.V2023_1.Shapes.M2;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using Core.Utilities;
using Repository;

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
        _languages = [ShapesLanguage.Instance, TestLanguageLanguage.Instance];

        _repositoryForest = new Forest();
        _repositoryConnector = new(_lionWebVersion);
        _repository = new LionWebTestRepository(_lionWebVersion, _languages, "repository", _repositoryForest,
            _repositoryConnector);

        _aClient = CreateClient("A", out _aForest, out _aConnector);
        _bClient = CreateClient("B", out _bForest, out _bConnector);
    }

    [TestCleanup]
    public void AssertNoExceptions()
    {
        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
        AssertNoExceptions(_bClient.Exceptions);
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
    public void NewPartition()
    {
        _aForest.AddPartitions([new Geometry("partition")]);
        WaitForReceived(1);

        AssertEquals(_aForest.Partitions, _bForest.Partitions);
    }

    [TestMethod]
    [Timeout(6000)]
    public void NewPartitions()
    {
        _aForest.AddPartitions([new Geometry("partitionA"), new LinkTestConcept("partitionB")]);
        WaitForReceived(2);

        AssertEquals(_aForest.Partitions, _bForest.Partitions);
    }

    [TestMethod]
    [Timeout(6000)]
    public void RemovePartition()
    {
        _aForest.AddPartitions([new Geometry("geo")]);
        WaitForReceived(1);

        var bLink = new LinkTestConcept("link");
        _bForest.AddPartitions([bLink]);
        WaitForReceived(1);

        var aLink = (LinkTestConcept)_aForest.Partitions.Last();
        _aForest.RemovePartitions([aLink]);
        WaitForReceived(1);

        bLink.Name = "hello";
        Assert.IsFalse(aLink.TryGetName(out _));

        aLink.Name = "bye";
        Assert.AreEqual("hello", bLink.Name);

        Assert.AreEqual("geo", _bForest.Partitions.First().GetId());
        AssertEquals(_aForest.Partitions, _bForest.Partitions);
    }

    [TestMethod]
    [Timeout(6000)]
    public void PartitionAnnotation()
    {
        _aForest.AddPartitions([new Geometry("partition")]);
        WaitForReceived(1);

        var bPartition = (Geometry)_bForest.Partitions.First();
        Assert.IsNotNull(bPartition);

        bPartition.AddAnnotations([new BillOfMaterials("bom")]);
        WaitForReceived(1);

        AssertEquals(_aForest.Partitions, _bForest.Partitions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task ListPartitions_empty()
    {
        var partitions = await _aClient.ListPartitions();
        Assert.AreEqual(0, partitions.Count);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task ListPartitions_one()
    {
        await _aClient.SignOn("myRepo");
        var part0 = new Geometry("partition");
        _aForest.AddPartitions([part0]);
        WaitForReceived(1);
        var partitions = await _aClient.ListPartitions();
        Assert.HasCount(1, partitions);
        
        AssertEquals(part0, partitions[0]);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task ListPartitions_two()
    {
        var part0 = new Geometry("part0");
        _aForest.AddPartitions([part0]);
        WaitForReceived(1);

        var part1 = new Geometry("part1");
        _bForest.AddPartitions([part1]);
        WaitForReceived(1);
        
        var partitions = await _aClient.ListPartitions();
        Assert.HasCount(2, partitions);
        
        AssertEquals(part0, partitions[0]);
        AssertEquals(part1, partitions[1]);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task ListPartitions_noFeatures()
    {
        await _aClient.SignOn("myRepo");
        var containment01 = new LinkTestConcept("cont");
        var part0 = new LinkTestConcept("partition")
        {
            Name = "my partition", Containment_0_1 = containment01, Reference_0_1 = containment01
        };
        part0.AddAnnotations([new TestAnnotation("ann")]);
        _aForest.AddPartitions([part0]);
        WaitForReceived(1);
        
        var partitions = await _aClient.ListPartitions();
        Assert.HasCount(1, partitions);
        var actual = (LinkTestConcept)partitions[0];
        
        Assert.AreEqual(part0.GetId(), actual.GetId());
        Assert.AreEqual(part0.GetConcept(), actual.GetConcept());
        Assert.AreEqual(part0.Name, actual.Name);
        Assert.IsNull(actual.Containment_0_1);
        Assert.IsNull(actual.Reference_0_1);
    }

    protected void AssertEquals(IReadableNode? a, IReadableNode? b) =>
        AssertEquals([a], [b]);

    protected void AssertEquals(IEnumerable<IReadableNode?> a, IEnumerable<IReadableNode?> b)
    {
        List<IDifference> differences = new Comparer(a.ToList(), b.ToList()).Compare().ToList();
        Assert.IsTrue(differences.Count == 0,
            differences.DescribeAll(new() { LeftDescription = "a", RightDescription = "b" }));
    }

    protected void AssertNoExceptions(List<Exception> exceptions) =>
        Assert.AreEqual(0, exceptions.Count, string.Join(Environment.NewLine, exceptions));

    private void WaitForReceived(int numberOfMessages)
    {
        _aClient.WaitForReceived(numberOfMessages);
        _bClient.WaitForReceived(numberOfMessages);
    }
}