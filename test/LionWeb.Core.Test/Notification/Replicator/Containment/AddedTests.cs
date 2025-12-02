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

namespace LionWeb.Core.Test.Notification.Replicator.Containment;

using Core.Notification;
using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class AddedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var originalPartition = new TestPartition("a");
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        var added = new LinkTestConcept("added");
        
        Assert.IsFalse(sharedNodeMap.ContainsKey(added.GetId()));

        originalPartition.AddContents([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0]);
        
        Assert.IsTrue(sharedNodeMap.ContainsKey(added.GetId()));
    }

    [TestMethod]
    public void Multiple_adds_one_node_using_generic_api()
    {
        var originalPartition = new TestPartition("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new LinkTestConcept("added");
        originalPartition.Add(TestLanguageLanguage.Instance.TestPartition_contents, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0]);
    }

    [TestMethod]
    public void Multiple_adds_multiple_nodes_using_generic_api()
    {
        var originalPartition = new TestPartition("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added1 = new LinkTestConcept("added1");
        var added2 = new LinkTestConcept("added2");
        originalPartition.Add(TestLanguageLanguage.Instance.TestPartition_contents, [added1, added2]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added2, clonedPartition.Contents[0]);
    }

    [TestMethod]
    public void Multiple_sets_one_node_using_set()
    {
        var originalPartition = new TestPartition("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new LinkTestConcept("added");
        var values = new LinkTestConcept[] { added };

        originalPartition.Set(TestLanguageLanguage.Instance.TestPartition_contents, values);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0]);
    }

    [TestMethod]
    public void Multiple_sets_multiple_nodes_using_set()
    {
        var originalPartition = new TestPartition("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added1 = new LinkTestConcept("added1");
        var added2 = new LinkTestConcept("added2");
        var values = new LinkTestConcept[] { added1, added2 };

        originalPartition.Set(TestLanguageLanguage.Instance.TestPartition_contents, values);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added2, clonedPartition.Contents[0]);
    }

    [TestMethod]
    public void Multiple_First()
    {
        var originalPartition = new TestPartition("a") { Contents = [new LinkTestConcept("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new LinkTestConcept("added");
        originalPartition.InsertContents(0, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var originalPartition = new TestPartition("a") { Contents = [new LinkTestConcept("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new LinkTestConcept("added");
        originalPartition.InsertContents(1, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[1]);
    }

    [TestMethod]
    public void Single()
    {
        var parent = new LinkTestConcept("parent");
        var originalPartition = new TestPartition("a") {Contents = [parent]};
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new LinkTestConcept("added");
        parent.Containment_0_1 = added;

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0].Containment_0_1);
    }

    [TestMethod]
    public void Deep()
    {
        var originalPartition = new TestPartition("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new LinkTestConcept("added") { Containment_1 = new LinkTestConcept("coord") { Name = "1" } };
        originalPartition.AddContents([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0]);
    }
}