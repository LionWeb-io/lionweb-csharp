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

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class TwowayTests : TwowayReplicatorTestsBase
{
    #region ChildAdded

    [TestMethod]
    public void ChildAdded_Multiple_Only()
    {
        var node = new TestPartition("a");

        var clone = new TestPartition("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);

        var added = new LinkTestConcept("added");
        node.AddContents([added]);
        var added2 = new LinkTestConcept("added2");
        clone.AddContents([added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Contents[0]);
        Assert.AreNotSame(added2, node.Contents[1]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_First()
    {
        var node = new TestPartition("a") { Contents =  [new LinkTestConcept("l")] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new LinkTestConcept("added");
        node.InsertContents(0, [added]);
        var added2 = new LinkTestConcept("added2");
        clone.InsertContents(1, [added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Contents[0]);
        Assert.AreNotSame(added2, node.Contents[2]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_Last()
    {
        var node = new TestPartition("a") { Contents =  [new LinkTestConcept("l")] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new LinkTestConcept("added");
        node.InsertContents(1, [added]);
        var added2 = new LinkTestConcept("added2");
        clone.InsertContents(0, [added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Contents[2]);
        Assert.AreNotSame(added2, node.Contents[0]);
    }

    [TestMethod]
    [Ignore("issue with replaceWith")]
    public void ChildAdded_Single()
    {
        var node = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [node] };

        var clone = new LinkTestConcept("a");
        var clonePartition = new TestPartition("partition") { Contents = [clone] };

        var (replicator, cloneReplicator) = CreateReplicators(originalPartition, clonePartition);


        var added = new LinkTestConcept("added");
        node.Containment_0_1 = added;
        var added2 = new LinkTestConcept("added2");
        clone.Containment_0_1 = added2;

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Containment_0_1);
        Assert.AreNotSame(added2, node.Containment_0_1);
    }

    [TestMethod]
    public void ChildAdded_Deep()
    {
        var node = new TestPartition("a");

        var clone = new TestPartition("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new LinkTestConcept("added") { Containment_1 = new LinkTestConcept("coord") { Name = "1" } };
        node.AddContents([added]);
        var added2 = new LinkTestConcept("added2") { Containment_1 = new LinkTestConcept("coord2") { Name = "11" } };
        clone.AddContents([added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Contents[0]);
        Assert.AreNotSame(added2, node.Contents[1]);
    }

    [TestMethod]
    public void ChildAdded_Deep_DuplicateNodeId()
    {
        var node = new TestPartition("a");

        var clone = new TestPartition("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new LinkTestConcept("added") { Containment_1 = new LinkTestConcept("coord") { Name = "1" } };
        node.AddContents([added]);
        var added2 = new LinkTestConcept("added2") { Containment_1 = new LinkTestConcept("coord") { Name = "1" } };

        Assert.ThrowsExactly<DuplicateNodeIdException>(() => node.AddContents([added2]));
    }

    #endregion

    #region ChildDeleted

    [TestMethod]
    public void ChildDeleted_Multiple_Only()
    {
        var deleted = new LinkTestConcept("deleted");
        var node = new TestPartition("a") { Contents =  [deleted] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("deleted")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveContents([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_First()
    {
        var deleted = new LinkTestConcept("deleted");
        var node = new TestPartition("a") { Contents =  [deleted, new LinkTestConcept("l")] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("deleted"), new LinkTestConcept("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveContents([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_Last()
    {
        var deleted = new LinkTestConcept("deleted");
        var node = new TestPartition("a") { Contents =  [new LinkTestConcept("l"), deleted] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("l"), new LinkTestConcept("deleted")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveContents([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Single()
    {
        var deleted = new LinkTestConcept("deleted");
        var node = new LinkTestConcept("a") { Containment_0_1 = deleted };
        var originalPartition = new TestPartition("partition") { Contents = [node] };

        var clone = new LinkTestConcept("a") { Containment_0_1 = new LinkTestConcept("deleted") };
        var clonePartition = new TestPartition("partition") { Contents = [clone] };

        var (replicator, cloneReplicator) = CreateReplicators(originalPartition, clonePartition);


        node.Containment_0_1 = null;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildReplaced

    [TestMethod]
    [Ignore("issue with replaceWith")]
    public void ChildReplaced_Single()
    {
        var node = new LinkTestConcept("a") { Containment_0_1 = new LinkTestConcept("replaced") { Name = "a" } };
        var originalPartition = new TestPartition("partition") { Contents = [node] };

        var clone = new LinkTestConcept("a") { Containment_0_1 = new LinkTestConcept("replaced") { Name = "a" } };
        var clonePartition = new TestPartition("partition") { Contents = [clone] };

        var (replicator, cloneReplicator) = CreateReplicators(originalPartition, clonePartition);


        var added = new LinkTestConcept("added") { Name = "added" };
        node.Containment_0_1 = added;

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    [Ignore("issue with replaceWith")]
    public void ChildReplaced_Deep()
    {
        var node = new TestPartition("a");
        var bof = new LinkTestConcept("bof")
        {
            Containment_1 = new LinkTestConcept("mg") { Name = "liquid" }
        };
        node.AddContents([bof]);

        var clone = new TestPartition("a");
        clone.AddContents([
            new LinkTestConcept("bof") { Containment_1 = new LinkTestConcept("mg") { Name = "liquid" } }
        ]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.Containment_1 = new LinkTestConcept("replaced") { Name = "gas" };

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildMovedFromOtherContainment

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple()
    {
        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n = [moved] };
        var node = new TestPartition("a") { Contents =  [origin] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("origin") { Containment_1_n = [new LinkTestConcept("moved")] }] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.AddContents([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single()
    {
        var moved = new LinkTestConcept("moved");
        var node = new TestPartition("a") { Contents =  [new LinkTestConcept("l") { Containment_1 = moved }, new LinkTestConcept("b")] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("l") { Containment_1 = new LinkTestConcept("moved") }, new LinkTestConcept("b")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.Contents[1].Containment_0_1 = moved;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildMovedFromOtherContainmentInSameParent

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Multiple()
    {
        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n = [moved] };
        var node = new TestPartition("a") { Contents =  [origin] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("origin") { Containment_1_n = [new LinkTestConcept("moved")] }] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        origin.AddContainment_0_n([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Single()
    {
        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n = [moved] };
        var node = new TestPartition("a") { Contents =  [origin] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("origin") { Containment_1_n = [new LinkTestConcept("moved")] }] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        origin.Containment_0_1 = moved;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildMovedInSameContainment

    [TestMethod]
    public void ChildMovedInSameContainment_Forward()
    {
        var moved = new LinkTestConcept("moved");
        var node = new TestPartition("a") { Contents =  [moved, new LinkTestConcept("l")] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("moved"), new LinkTestConcept("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.AddContents([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_Backward()
    {
        var moved = new LinkTestConcept("moved");
        var node = new TestPartition("a") { Contents =  [new LinkTestConcept("l"), moved] };

        var clone = new TestPartition("a") { Contents =  [new LinkTestConcept("l"), new LinkTestConcept("moved")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.InsertContents(0, [moved]);

        AssertEquals([node], [clone]);
    }

    #endregion
}