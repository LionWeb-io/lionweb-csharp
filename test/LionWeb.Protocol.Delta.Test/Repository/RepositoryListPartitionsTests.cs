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

using Core.Test.Languages.Generated.V2023_1.Shapes.M2;
using Core.Test.Languages.Generated.V2023_1.TestLanguage;

[TestClass]
public class RepositoryListPartitionsTests : RepositoryTestNoExceptionsBase
{
    [TestMethod]
    [Timeout(6000)]
    public async Task ListPartitions_empty()
    {
        await _aClient.SignOn(RepoId);
        
        var partitions = await _aClient.ListPartitions();
        Assert.AreEqual(0, partitions.Count);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task ListPartitions_one()
    {
        await _aClient.SignOn(RepoId);
        
        var part0 = new Geometry("partition");
        _aForest.AddPartitions([part0]);
        _aClient.WaitForReceived(1);
        var partitions = await _aClient.ListPartitions();
        Assert.HasCount(1, partitions);

        AssertEquals(part0, partitions[0]);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task ListPartitions_two()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var part0 = new Geometry("part0");
        _aForest.AddPartitions([part0]);
        await _bClient.SubscribeToPartitionContents(part0.GetId());
        WaitForReceived(1);

        var part1 = new Geometry("part1");
        _bForest.AddPartitions([part1]);
        await _aClient.SubscribeToPartitionContents(part1.GetId());
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
        await _aClient.SignOn(RepoId);
        
        var containment01 = new LinkTestConcept("cont");
        var part0 = new LinkTestConcept("partition")
        {
            Name = "my partition", Containment_0_1 = containment01, Reference_0_1 = containment01
        };
        part0.AddAnnotations([new TestAnnotation("ann")]);
        _aForest.AddPartitions([part0]);
        _aClient.WaitForReceived(1);

        var partitions = await _aClient.ListPartitions();
        Assert.HasCount(1, partitions);
        var actual = (LinkTestConcept)partitions[0];

        Assert.AreEqual(part0.GetId(), actual.GetId());
        Assert.AreEqual(part0.GetConcept(), actual.GetConcept());
        Assert.AreEqual(part0.Name, actual.Name);
        Assert.IsNull(actual.Containment_0_1);
        Assert.IsNull(actual.Reference_0_1);
    }
}