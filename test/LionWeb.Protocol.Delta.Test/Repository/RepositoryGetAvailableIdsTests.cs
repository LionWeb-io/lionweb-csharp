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
using Core.Utilities;

[TestClass]
public class RepositoryGetAvailableIdsTests : RepositoryTestNoExceptionsBase
{
    [TestMethod]
    [Timeout(6000)]
    public async Task GetAvailableIds()
    {
        await _aClient.SignOn(RepoId);

        var availableIdsResponse = await _aClient.GetAvailableIds(11);

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
        await _aClient.SignOn(RepoId);

        var usedNodeId = "repoProvidedId-6";
        _aForest.AddPartitions([new TestPartition(usedNodeId)]);

        var availableIdsResponse = await _aClient.GetAvailableIds(11);

        Assert.AreEqual(11, availableIdsResponse.Ids.Length);
        foreach (var freeId in availableIdsResponse.Ids)
        {
            Assert.IsTrue(IdUtils.IsValid(freeId));
        }

        Assert.IsFalse(availableIdsResponse.Ids.Contains(usedNodeId));
    }
}