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

[TestClass]
public class RepositorySubscribeToChangingPartitionsTests : RepositoryTestsBase
{
    [TestMethod]
    [Timeout(6000)]
    public async Task Create()
    {
        await _bClient.SubscribeToChangingPartitions(true, false, false);

        var aPart = new LinkTestConcept("part")
        {
            Containment_0_1 = new LinkTestConcept("cont")
        };
        _aForest.AddPartitions([aPart]);
        WaitForReceived(1);

        var bPart = (LinkTestConcept)_bForest.Partitions.First();
        Assert.IsNotNull(bPart);
        Assert.IsNotNull(bPart.Containment_0_1);

        aPart.Name = "changed";
        _aClient.WaitForReceived(1);
        Assert.IsFalse(bPart.TryGetName(out _));
        
        _aForest.RemovePartitions([aPart]);
        _aClient.WaitForReceived(1);
        
        Assert.HasCount(1, _bForest.Partitions);
    }
    
    [TestMethod]
    [Timeout(6000)]
    public async Task Delete()
    {
        await _bClient.SubscribeToChangingPartitions(false, true, false);

        var aPart = new LinkTestConcept("part")
        {
            Containment_0_1 = new LinkTestConcept("cont")
        };
        _aForest.AddPartitions([aPart]);
        WaitForReceived(1);

        Assert.HasCount(0, _bForest.Partitions);

        _aForest.RemovePartitions([aPart]);
        _aClient.WaitForReceived(1);
        
        Assert.HasCount(0, _bForest.Partitions);
    }
    
    [TestMethod]
    [Timeout(6000)]
    public async Task Partitions()
    {
        await _bClient.SubscribeToChangingPartitions(true, false, true);

        var aPart = new LinkTestConcept("part")
        {
            Containment_0_1 = new LinkTestConcept("cont")
        };
        _aForest.AddPartitions([aPart]);
        WaitForReceived(1);

        var bPart = (LinkTestConcept)_bForest.Partitions.First();
        Assert.IsNotNull(bPart);
        Assert.IsNotNull(bPart.Containment_0_1);

        aPart.Name = "changed";
        WaitForReceived(1);
        Assert.AreEqual("changed", bPart.Name);
        
        _aForest.RemovePartitions([aPart]);
        WaitForReceived(1);
        
        Assert.HasCount(0, _bForest.Partitions);
    }
    
}