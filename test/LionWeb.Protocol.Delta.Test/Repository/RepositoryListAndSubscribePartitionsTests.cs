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
public class RepositoryListAndSubscribePartitionsTests : RepositoryTestNoExceptionsBase
{
    [TestMethod]
    [Timeout(6000)]
    public async Task Empty()
    {
        await _aClient.SignOn(RepoId);
        
        var partitions = await _aClient.ListAndSubscribePartitions();
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
        var partitions = await _aClient.ListAndSubscribePartitions();
        
        Assert.HasCount(1, partitions);
        AssertEquals(part0, partitions[0]);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task ListExisting()
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

        var partitions = await _aClient.ListAndSubscribePartitions();
        Assert.HasCount(2, partitions);

        Assert.AreSame(part0, partitions[0]);
        AssertEquals(part1, partitions[1]);
        Assert.AreNotSame(part1, partitions[1]);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task AddedToForest()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var part0 = new TestPartition("part0");
        _aForest.AddPartitions([part0]);
        WaitForReceived(1);

        var partitions = await _bClient.ListAndSubscribePartitions();
        Assert.HasCount(1, partitions);
        AssertEquals(part0, partitions[0]);
        Assert.AreNotSame(part0, partitions[0]);
        
        Assert.HasCount(1, _bForest.Partitions);
        Assert.AreSame(partitions[0], _bForest.Partitions.First());
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task IncludesFeatures()
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

        var partitions = await _aClient.ListAndSubscribePartitions();
        Assert.HasCount(1, partitions);
        var actual = (TestPartition)partitions[0];

        Assert.AreEqual(part0.GetId(), actual.GetId());
        Assert.AreEqual(part0.GetConcept(), actual.GetConcept());
        Assert.AreEqual(part0.Name, actual.Name);
        Assert.IsNotEmpty(actual.Links);
    }
    
    [TestMethod]
    [Timeout(6000)]
    public async Task Subscribed()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var part0 = new TestPartition("part0");
        _aForest.AddPartitions([part0]);
        WaitForReceived(1);

        var partitions = await _bClient.ListAndSubscribePartitions();
        Assert.HasCount(1, partitions);

        Assert.AreEqual(part0.GetId(), partitions[0].GetId());

        part0.Name = "ChangedName";
        WaitForReceived(1);
        
        Assert.AreEqual("ChangedName", ((TestPartition)partitions[0]).Name);
    }
}