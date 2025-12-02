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

using Core.Test.Languages.Generated.V2023_1.TestLanguage;
using Delta.Client;

[TestClass]
public class RepositorySignOnTests : RepositoryTestsBase
{
    [TestMethod]
    [Timeout(6000)]
    public void NoSignOn()
    {
        _aForest.AddPartitions([new TestPartition("part")]);

        Assert.AreEqual(0, _aClient.MessageCount);
        Assert.HasCount(0, _bForest.Partitions);

        Assert.ContainsSingle(e => e is InvalidOperationException, _aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public void NoSignOn_Repo()
    {
        _aClient.ParticipationId = "xxx";

        _aForest.AddPartitions([new TestPartition("part")]);
        _aClient.WaitForReceived(1);

        Assert.AreEqual(1, _aClient.MessageCount);
        Assert.HasCount(0, _bForest.Partitions);

        Assert.ContainsSingle(e => e is DeltaException, _aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task SignOff_NoSignOn()
    {
        await Assert.ThrowsAsync<DeltaException>(async () => await _aClient.SignOff());

        Assert.AreEqual(0, _aClient.MessageCount);
        Assert.HasCount(0, _bForest.Partitions);

        Assert.IsEmpty(_aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task SignOff_NoSignOn_Repo()
    {
        _aClient.ParticipationId = "xxx";

        await Assert.ThrowsAsync<DeltaException>(async () => await _aClient.SignOff());

        Assert.AreEqual(1, _aClient.MessageCount);
        Assert.HasCount(0, _bForest.Partitions);

        Assert.IsEmpty(_aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Reconnect()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        await _bClient.SubscribeToChangingPartitions(true, true, true);
        
        var participationId = _aClient.ParticipationId;

        _aForest.AddPartitions([new TestPartition("part")]);
        WaitForReceived(1);
        
        await _aClient.SignOff();

        var onlyA = new TestPartition("onlyA");
        _aForest.AddPartitions([onlyA]);
        _aForest.RemovePartitions([onlyA]);
        
        Assert.HasCount(2, _aClient.Exceptions);
        
        _bForest.AddPartitions([new TestPartition("onlyB")]);
        _bClient.WaitForReceived(1);
        
        await _aClient.Reconnect(participationId);
        _aForest.AddPartitions([new TestPartition("p2")]);
        WaitForReceived(1);
        
        Assert.HasCount(2, _aForest.Partitions);
        Assert.HasCount(3, _bForest.Partitions);

        Assert.HasCount(2, _aClient.Exceptions);
        Assert.IsEmpty(_bClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Reconnect_WrongCount()
    {
        await _aClient.SignOn(RepoId);
        
        var participationId = _aClient.ParticipationId;

        _aForest.AddPartitions([new TestPartition("part0")]);
        _aForest.AddPartitions([new TestPartition("part1")]);
        _aClient.WaitForReceived(2);
        
        await _aClient.SignOff();
        _aClient.SetEventSequenceNumber(0);

        await Assert.ThrowsAsync<DeltaException>(async () => await _aClient.Reconnect(participationId));

        Assert.HasCount(0, _aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Reconnect_SignedOn()
    {
        await _aClient.SignOn(RepoId);
        await Assert.ThrowsAsync<DeltaException>(async () => await _aClient.Reconnect("unknown"));

        Assert.AreEqual(1, _aClient.MessageCount);

        Assert.IsEmpty(_aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Reconnect_SignedOn_Repo()
    {
        await _aClient.SignOn(RepoId);
        _aClient.SetSignedIn(false);
        
        await Assert.ThrowsAsync<DeltaException>(async () => await _aClient.Reconnect("unknown"));

        Assert.AreEqual(2, _aClient.MessageCount);

        Assert.IsEmpty(_aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Reconnect_NeverConnected()
    {
        await _aClient.Reconnect("unknown");

        Assert.AreEqual(1, _aClient.MessageCount);
        Assert.HasCount(0, _bForest.Partitions);

        Assert.IsEmpty(_aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Reconnect_NeverConnected_Repo()
    {
        _aClient.ParticipationId = "xxx";
        _aClient.SetSignedIn(false);

        // TODO: Should this succeed? This way we can force our own participationId (not given by repo)
        await _aClient.Reconnect("xxx");

        Assert.AreEqual(1, _aClient.MessageCount);
        Assert.HasCount(0, _bForest.Partitions);

        Assert.HasCount(0, _aClient.Exceptions);
        Assert.IsEmpty(_repository.Exceptions);
    }
}