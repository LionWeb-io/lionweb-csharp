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
public class RepositoryPartitionTests : RepositoryTestsBase
{
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
}