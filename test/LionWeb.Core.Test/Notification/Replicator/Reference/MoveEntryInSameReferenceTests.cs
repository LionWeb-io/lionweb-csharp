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

namespace LionWeb.Core.Test.Notification.Replicator.Reference;

using Core.Notification;
using Core.Notification.Partition;
using Languages.Generated.V2025_1.TestLanguage;

[TestClass]
public class MoveEntryInSameReferenceTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Backward()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, moved]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 2;
        var newIndex = 0;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, ReferenceTarget.FromNode(moved), new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(ref1);
        sharedNodeMap.RegisterNode(ref2);
        sharedNodeMap.RegisterNode(moved);
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(3, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[0].GetId());
    }

    [TestMethod]
    public void Forward()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [moved, ref1, ref2]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 0;
        var newIndex = 2;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, ReferenceTarget.FromNode(moved), new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(ref1);
        sharedNodeMap.RegisterNode(ref2);
        sharedNodeMap.RegisterNode(moved);
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(3, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[^1].GetId());
    }

    [TestMethod]
    public void Forward_MoreThanThreeNodes()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, moved, ref2, ref3, ref4]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 1;
        var newIndex = 3;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, ReferenceTarget.FromNode(moved), new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        
        foreach (var reference in originalPartition.Reference_0_n)
        {
            sharedNodeMap.RegisterNode(reference);
        }
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(5, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[3].GetId());
    }

    [TestMethod]
    public void Backward_MoreThanThreeNodes()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, ref3, moved, ref4]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 3;
        var newIndex = 1;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, ReferenceTarget.FromNode(moved), new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        
        foreach (var reference in originalPartition.Reference_0_n)
        {
            sharedNodeMap.RegisterNode(reference);
        }
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(5, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[1].GetId());
    }
}