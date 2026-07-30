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

using Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class InformAboutChangingPartitionsTests : RepositoryTestsBase
{
    [TestMethod]
    [Timeout(6000)]
    public async Task Create()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        await _bClient.InformAboutChangingPartitions(true, false, DefaultDepthLimit);

        var aPart = new TestPartition("part")
        {
            Links = [new LinkTestConcept("cont")]
        };
        _aForest.AddPartitions([aPart]);
        WaitForReceived(1);

        var bPart = (TestPartition)_bForest.Partitions.First();
        Assert.IsNotNull(bPart);
        Assert.IsNotEmpty(bPart.Links);

        aPart.Name = "changed";
        _aClient.WaitForReceived(1);
        Assert.IsFalse(bPart.TryGetName(out _));
        
        _aForest.RemovePartitions([aPart]);
        _aClient.WaitForReceived(1);
        
        Assert.HasCount(1, _bForest.Partitions);
        AssertNoExceptions();
    }
    
    [TestMethod]
    [Timeout(6000)]
    public async Task DeleteUnknown()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        await _bClient.InformAboutChangingPartitions(false, true, DefaultDepthLimit);

        var aPart = new TestPartition("part")
        {
            Links = [new LinkTestConcept("cont")]
        };
        _aForest.AddPartitions([aPart]);
        WaitForReceived(1);

        Assert.HasCount(0, _bForest.Partitions);

        _aForest.RemovePartitions([aPart]);
        _aClient.WaitForReceived(1);
        
        Assert.HasCount(0, _bForest.Partitions);
        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
        Assert.HasCount(1, _bClient.Exceptions);
        Assert.IsInstanceOfType<DeltaException>(_bClient.Exceptions[0]);
        var bClientException = (DeltaException)_bClient.Exceptions[0];
        Assert.AreEqual("Unknown node id part", bClientException.Message);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task DeleteKnown()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, DefaultDepthLimit);

        var aPart = new TestPartition("partA") { Links = [new LinkTestConcept("cont")] };
        _aForest.AddPartitions([aPart]);
        WaitForReceived(1);

        Assert.HasCount(1, _bForest.Partitions);

        await _bClient.InformAboutChangingPartitions(false, true, DefaultDepthLimit);

        _aForest.RemovePartitions([aPart]);
        WaitForReceived(1);

        Assert.HasCount(0, _bForest.Partitions);

        _aForest.AddPartitions([new TestPartition("partB")]);

        Assert.HasCount(1, _aForest.Partitions);
        Assert.HasCount(0, _bForest.Partitions);

        AssertNoExceptions();
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Unsubscribe()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, DefaultDepthLimit);

        var aPartX = new TestPartition("partX");
        _aForest.AddPartitions([aPartX]);
        WaitForReceived(1);

        Assert.HasCount(1, _aForest.Partitions);
        Assert.HasCount(1, _bForest.Partitions);

        await _bClient.InformAboutChangingPartitions(false, false, DefaultDepthLimit);

        var aPartY = new TestPartition("partY");
        _aForest.AddPartitions([aPartY]);
        _aClient.WaitForReceived(1);

        Assert.HasCount(2, _aForest.Partitions);
        Assert.HasCount(1, _bForest.Partitions);

        // b does NOT get this event because it's NOT subscribed to partX
        _aForest.RemovePartitions([aPartX]);
        _aClient.WaitForReceived(1);

        Assert.HasCount(1, _aForest.Partitions);
        Assert.HasCount(1, _bForest.Partitions);

        _aForest.RemovePartitions([aPartY]);
        _aClient.WaitForReceived(1);

        Assert.HasCount(0, _aForest.Partitions);
        Assert.HasCount(1, _bForest.Partitions);

        AssertNoExceptions();
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task InfoAfterSubscribe()
    {
        await _aClient.SignOn(RepoId);

        await _aClient.SubscribeToChangingPartitions(true, true);
        var ex = await Assert.ThrowsExactlyAsync<DeltaException>(async () => await _aClient.InformAboutChangingPartitions(true, false, DefaultDepthLimit));

        Assert.AreEqual("Already subscribed to SubscribeToChangingPartitions, but requesting InformAboutChangingPartitions", ex.Message);

        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task InfoAfterSubscribeCleared()
    {
        await _aClient.SignOn(RepoId);

        await _aClient.SubscribeToChangingPartitions(true, true);
        await _aClient.SubscribeToChangingPartitions(false, false);
        await _aClient.InformAboutChangingPartitions(true, false, DefaultDepthLimit);

        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
    }
    
    [TestMethod]
    [Timeout(6000)]
    public async Task Depth0()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, 0);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = _bForest.Partitions.ToList();
        Assert.HasCount(1, partitions);

        AssertNodesPresent([_partition], partitions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth1()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, 1);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = _bForest.Partitions.ToList();
        Assert.HasCount(1, partitions);

        AssertNodesPresent([_partition, _linkLevel1A, _linkLevel1B], partitions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth2()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, 2);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = _bForest.Partitions.ToList();
        Assert.HasCount(1, partitions);

        AssertNodesPresent(
            [
                _partition,
                _linkLevel1A,
                _linkLevel1B,
                _annLevel2Ba,
                _linkLevel2Aa,
                _linkLevel2Ab,
                _linkLevel2Ac
            ],
            partitions
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth3()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, 3);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = _bForest.Partitions.ToList();
        Assert.HasCount(1, partitions);

        AssertNodesPresent(
            [
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
            ],
            partitions
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth4()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, 4);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = _bForest.Partitions.ToList();
        Assert.HasCount(1, partitions);

        AssertNodesPresent(
            [
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
            ],
            partitions
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth5()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, 5);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = _bForest.Partitions.ToList();
        Assert.HasCount(1, partitions);

        AssertNodesPresent(
            [
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
            ],
            partitions
        );
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Depth6()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);

        await _bClient.InformAboutChangingPartitions(true, true, 6);

        _aForest.AddPartitions([_partition]);
        WaitForReceived(1);

        var partitions = _bForest.Partitions.ToList();
        Assert.HasCount(1, partitions);

        AssertNodesPresent(
            [
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
            ],
            partitions
        );
    }
}