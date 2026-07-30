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

namespace LionWeb.Protocol.Delta.Test.Repository;

using Core;
using Core.M1;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class RepositoryListPartitionsTests : RepositoryTestNoExceptionsBase
{
    private readonly TestPartition _partition;
    private readonly LinkTestConcept _linkLevel1B;
    private readonly TestAnnotation _annLevel2Ba;
    private readonly LinkTestConcept _linkLevel3Baa;
    private readonly TestAnnotation _annLevel4Baaa;
    private readonly LinkTestConcept _linkLevel1A;
    private readonly LinkTestConcept _linkLevel2Ac;
    private readonly LinkTestConcept _linkLevel2Ab;
    private readonly TestAnnotation _annLevel3Aba;
    private readonly LinkTestConcept _linkLevel2Aa;
    private readonly LinkTestConcept _linkLevel3Aab;
    private readonly LinkTestConcept _linkLevel3Aaa;
    private readonly TestAnnotation _annLevel4Aaaa;
    private readonly DataTypeTestConcept _dataLevel5Aaaaa;

    public RepositoryListPartitionsTests()
    {
        _dataLevel5Aaaaa = new DataTypeTestConcept("dataLevel5_A_A_A_A_A");
        _annLevel4Aaaa = new TestAnnotation("annLevel4_A_A_A_A") { Containment = _dataLevel5Aaaaa };
        _linkLevel3Aaa = new LinkTestConcept("linkLevel3_A_A_A").WithAnnotation(_annLevel4Aaaa);
        _linkLevel3Aab = new LinkTestConcept("linkLevel3_A_A_B");
        _linkLevel2Aa = new LinkTestConcept("linkLevel2_A_A")
        {
            Containment_0_n =
            [
                _linkLevel3Aaa,
                _linkLevel3Aab
            ]
        };
        _annLevel3Aba = new TestAnnotation("annLevel3_A_B_A");
        _linkLevel2Ab = new LinkTestConcept("linkLevel2_A_B").WithAnnotation(_annLevel3Aba);
        _linkLevel2Ac = new LinkTestConcept("linkLevel2_A_C");
        _linkLevel1A = new LinkTestConcept("linkLevel1_A")
        {
            Containment_0_1 = _linkLevel2Aa,
            Containment_1_n =
            [
                _linkLevel2Ab,
                _linkLevel2Ac,
            ]
        };
        _annLevel4Baaa = new TestAnnotation("annLevel4_B_A_A_A");
        _linkLevel3Baa = new LinkTestConcept("linkLevel3_B_A_A").WithAnnotation(_annLevel4Baaa);
        _annLevel2Ba = new TestAnnotation("annLevel2_B_A") { Containment = _linkLevel3Baa };
        _linkLevel1B = new LinkTestConcept("linkLevel1_B").WithAnnotation(_annLevel2Ba);
        _partition = new TestPartition("partition")
        {
            Links =
            [
                _linkLevel1A,
                _linkLevel1B
            ]
        };
    }
    
    [TestMethod]
    [Timeout(6000)]
    public async Task Empty()
    {
        await _aClient.SignOn(RepoId);
        
        var partitions = await _aClient.ListPartitions(DefaultDepthLimit);
        Assert.AreEqual(0, partitions.Count);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task One()
    {
        await _aClient.SignOn(RepoId);
        
        var part0 = new TestPartition("partition");
        _aForest.AddPartitions([part0]);
        _aClient.WaitForReceived(1);
        var partitions = await _aClient.ListPartitions(DefaultDepthLimit);
        Assert.HasCount(1, partitions);

        AssertEquals(part0, partitions[0]);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Two()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var part0 = new TestPartition("part0");
        _aForest.AddPartitions([part0]);
        await _bClient.SubscribeToPartitionContents(part0.GetId());
        WaitForReceived(1);

        var part1 = new TestPartition("part1");
        _bForest.AddPartitions([part1]);
        await _aClient.SubscribeToPartitionContents(part1.GetId());
        WaitForReceived(1);

        var partitions = await _aClient.ListPartitions(DefaultDepthLimit);
        Assert.HasCount(2, partitions);

        AssertEquals(part0, partitions[0]);
        AssertEquals(part1, partitions[1]);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task NoFeatures()
    {
        await _aClient.SignOn(RepoId);
        
        var containment01 = new LinkTestConcept("cont");
        var part0 = new TestPartition("partition")
        {
            Name = "my partition", 
            Links = [containment01]
        };
        part0.AddAnnotations([new TestAnnotation("ann")]);
        _aForest.AddPartitions([part0]);
        _aClient.WaitForReceived(1);

        var partitions = await _aClient.ListPartitions(0);
        Assert.HasCount(1, partitions);
        var actual = (TestPartition)partitions[0];

        Assert.AreEqual(part0.GetId(), actual.GetId());
        Assert.AreEqual(part0.GetConcept(), actual.GetConcept());
        Assert.AreEqual(part0.Name, actual.Name);
        Assert.IsEmpty(actual.Links);
    }
    
    [TestMethod]
    [Timeout(6000)]
    public async Task NotSubscribed()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var part0 = new TestPartition("part0");
        _aForest.AddPartitions([part0]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(DefaultDepthLimit);
        Assert.HasCount(1, partitions);

        Assert.AreEqual(part0.GetId(), partitions[0].GetId());

        part0.Name = "ChangedName";
        WaitForReceived(1);
        
        Assert.IsFalse(((TestPartition)partitions[0]).TryGetName(out _));
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth0()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(0);
        Assert.HasCount(1, partitions);

        CollectionAssert.AreEquivalent(
            new List<IReadableNode> { _partition }.Select(n => n.GetId()).ToList(),
            partitions.SelectMany(p => M1Extensions.Descendants(p, true, true)).Select(n => n.GetId()).ToList()
        );
    }

    [TestMethod]
    // [Timeout(6000)]
    public async Task Depth1()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(1);
        Assert.HasCount(1, partitions);

        CollectionAssert.AreEquivalent(
            new List<IReadableNode> { _partition, _linkLevel1A, _linkLevel1B }.Select(n => n.GetId()).ToList(),
            partitions.SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, true)).Select(n => n.GetId()).ToList()
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth2()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(2);
        Assert.HasCount(1, partitions);

        CollectionAssert.AreEquivalent(
            new List<IReadableNode>
            {
                _partition,
                _linkLevel1A,
                _linkLevel1B,
                _annLevel2Ba,
                _linkLevel2Aa,
                _linkLevel2Ab,
                _linkLevel2Ac
            }.Select(n => n.GetId()).ToList(),
            partitions.SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, true)).Select(n => n.GetId()).ToList()
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth3()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(3);
        Assert.HasCount(1, partitions);

        CollectionAssert.AreEquivalent(
            new List<IReadableNode>
            {
                _partition,
                _linkLevel1A,
                _linkLevel1B,
                _annLevel2Ba,
                _linkLevel2Aa,
                _linkLevel2Ab,
                _linkLevel2Ac,
                _annLevel3Aba,
                _linkLevel3Aaa,
                _linkLevel3Aab,
                _linkLevel3Baa
            }.Select(n => n.GetId()).ToList(),
            partitions.SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, true)).Select(n => n.GetId()).ToList()
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth4()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(4);
        Assert.HasCount(1, partitions);

        CollectionAssert.AreEquivalent(
            new List<IReadableNode>
            {
                _partition,
                _linkLevel1A,
                _linkLevel1B,
                _annLevel2Ba,
                _linkLevel2Aa,
                _linkLevel2Ab,
                _linkLevel2Ac,
                _annLevel3Aba,
                _linkLevel3Aaa,
                _linkLevel3Aab,
                _linkLevel3Baa,
                _annLevel4Aaaa,
                _annLevel4Baaa
            }.Select(n => n.GetId()).ToList(),
            partitions.SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, true)).Select(n => n.GetId()).ToList()
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth5()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(5);
        Assert.HasCount(1, partitions);

        CollectionAssert.AreEquivalent(
            new List<IReadableNode>
            {
                _partition,
                _linkLevel1A,
                _linkLevel1B,
                _annLevel2Ba,
                _linkLevel2Aa,
                _linkLevel2Ab,
                _linkLevel2Ac,
                _annLevel3Aba,
                _linkLevel3Aaa,
                _linkLevel3Aab,
                _linkLevel3Baa,
                _annLevel4Aaaa,
                _annLevel4Baaa,
                _dataLevel5Aaaaa
            }.Select(n => n.GetId()).ToList(),
            partitions.SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, true)).Select(n => n.GetId()).ToList()
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth6()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = await _bClient.ListPartitions(5);
        Assert.HasCount(1, partitions);

        CollectionAssert.AreEquivalent(
            new List<IReadableNode>
            {
                _partition,
                _linkLevel1A,
                _linkLevel1B,
                _annLevel2Ba,
                _linkLevel2Aa,
                _linkLevel2Ab,
                _linkLevel2Ac,
                _annLevel3Aba,
                _linkLevel3Aaa,
                _linkLevel3Aab,
                _linkLevel3Baa,
                _annLevel4Aaaa,
                _annLevel4Baaa,
                _dataLevel5Aaaaa
            }.Select(n => n.GetId()).ToList(),
            partitions.SelectMany(p => M1Extensions.Descendants<IReadableNode>(p, true, true)).Select(n => n.GetId()).ToList()
        );
    }
}