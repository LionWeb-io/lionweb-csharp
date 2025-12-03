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

namespace LionWeb.Core.Test.Notification.Replicator.Annotation;

using Core.Notification;
using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class AddedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        var added = new TestAnnotation("added");

        Assert.IsFalse(sharedNodeMap.ContainsKey(added.GetId()));

        originalParent.AddAnnotations([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0].GetAnnotations()[0]);

        Assert.IsTrue(sharedNodeMap.ContainsKey(added.GetId()));
    }

    [TestMethod]
    public void Multiple_First()
    {
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("bof")]);
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new TestAnnotation("added");
        originalParent.InsertAnnotations(0, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0].GetAnnotations()[0]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("bof")]);
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new TestAnnotation("added");
        originalParent.InsertAnnotations(1, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Contents[0].GetAnnotations()[1]);
    }

    [TestMethod]
    [Ignore("Not possible with TestLanguage")]
    public void Deep()
    {
        var originalPartition = new TestPartition("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        // var added = new TestAnnotation("added") { AltGroups = [new MaterialGroup("mg") { MatterState = MatterState.gas }] };
        // originalPartition.AddAnnotations([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        // Assert.AreNotSame(added, clonedPartition.Contents[0].GetAnnotations()[0]);
    }
}