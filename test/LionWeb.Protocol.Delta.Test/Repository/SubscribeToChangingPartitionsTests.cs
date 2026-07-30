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
public class SubscribeToChangingPartitionsTests : RepositoryTestsBase
{
    [TestMethod]
    [Timeout(6000)]
    public async Task Create()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        await _bClient.SubscribeToChangingPartitions(true, false);

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
        Assert.AreEqual("changed", bPart.Name);
        
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
        
        await _bClient.SubscribeToChangingPartitions(false, true);

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

        await _bClient.SubscribeToChangingPartitions(true, true);

        var aPart = new TestPartition("partA") { Links = [new LinkTestConcept("cont")] };
        _aForest.AddPartitions([aPart]);
        WaitForReceived(1);

        Assert.HasCount(1, _bForest.Partitions);

        await _bClient.SubscribeToChangingPartitions(false, true);

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

        await _bClient.SubscribeToChangingPartitions(true, true);

        var aPartX = new TestPartition("partX");
        _aForest.AddPartitions([aPartX]);
        WaitForReceived(1);

        Assert.HasCount(1, _aForest.Partitions);
        Assert.HasCount(1, _bForest.Partitions);

        await _bClient.SubscribeToChangingPartitions(false, false);

        var aPartY = new TestPartition("partY");
        _aForest.AddPartitions([aPartY]);
        _aClient.WaitForReceived(1);

        Assert.HasCount(2, _aForest.Partitions);
        Assert.HasCount(1, _bForest.Partitions);

        // b still gets this event because it's subscribed to partX
        _aForest.RemovePartitions([aPartX]);
        WaitForReceived(1);

        Assert.HasCount(1, _aForest.Partitions);
        Assert.HasCount(0, _bForest.Partitions);

        _aForest.RemovePartitions([aPartY]);
        _aClient.WaitForReceived(1);

        Assert.HasCount(0, _aForest.Partitions);
        Assert.HasCount(0, _bForest.Partitions);

        AssertNoExceptions();
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task SubscribeAfterInfo()
    {
        await _aClient.SignOn(RepoId);

        await _aClient.InformAboutChangingPartitions(true, true, 2);
        var ex = await Assert.ThrowsExactlyAsync<DeltaException>(async () => await _aClient.SubscribeToChangingPartitions(true, false));

        Assert.AreEqual("Already subscribed to InformAboutChangingPartitions, but requesting SubscribeToChangingPartitions", ex.Message);

        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task SubscribeAfterInfoCleared()
    {
        await _aClient.SignOn(RepoId);

        await _aClient.InformAboutChangingPartitions(true, true, 2);
        await _aClient.InformAboutChangingPartitions(false, false, 2);
        await _aClient.SubscribeToChangingPartitions(true, false);

        AssertNoExceptions(_repository.Exceptions);
        AssertNoExceptions(_aClient.Exceptions);
    }
}