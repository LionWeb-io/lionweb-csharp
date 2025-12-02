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
public class SubscribeToPartitionContentsTests : RepositoryTestNoExceptionsBase
{
    [TestMethod]
    [Timeout(6000)]
    public async Task NoSubscription()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var part = new TestPartition("part");
        _aForest.AddPartitions([part]);

        Assert.HasCount(0, _bForest.Partitions);
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task WithSubscription()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var part = new TestPartition("part");

        _aForest.AddPartitions([part]);

        var actual = await _bClient.SubscribeToPartitionContents("part");

        Assert.HasCount(1, _bForest.Partitions);
        AssertEquals(part, _bForest.Partitions.First());
        Assert.AreSame(actual, _bForest.Partitions.First());
    }

    [TestMethod]
    [Timeout(6000)]
    public async Task Unsubscribe()
    {
        await _aClient.SignOn(RepoId);
        await _bClient.SignOn(RepoId);
        
        var aPart = new TestPartition("part");
        _aForest.AddPartitions([aPart]);

        var actual = await _bClient.SubscribeToPartitionContents("part");
        await _aClient.UnsubscribeFromPartitionContents("part");

        Assert.HasCount(1, _bForest.Partitions);
        var bPart = (TestPartition)_bForest.Partitions.First();
        bPart.Name = "my name";

        Assert.IsFalse(aPart.TryGetName(out _));
    }
}