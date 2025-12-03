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
using Core.Notification.Partition;
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class MovedAndReplacedInSameParentTests : ReplicatorTestsBase
{
    #region tests uses ReplaceWith method

    [TestMethod]
    public void Forward_uses_ReplaceWith()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([moved, replaced]);

        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        Assert.IsTrue(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(moved.GetId()));
        
        replaced.ReplaceWith(moved);

        AssertEquals([originalPartition], [clonedPartition]);

        Assert.IsFalse(sharedNodeMap.ContainsKey(replaced.GetId()));
        Assert.IsTrue(sharedNodeMap.ContainsKey(moved.GetId()));
    }

    [TestMethod]
    public void Backward_uses_ReplaceWith()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([replaced, moved]);

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(moved);

        AssertEquals([originalPartition], [clonedPartition]);
    }


    [TestMethod]
    public void Backward_MoreThanThreeNodes_uses_ReplaceWith()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("A"), replaced, new TestAnnotation("B"), moved, new TestAnnotation("C")]);

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(moved);

        AssertEquals([originalPartition], [clonedPartition]);
    }


    [TestMethod]
    public void Forward_MoreThanThreeNodes_uses_ReplaceWith()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("A"), moved, new TestAnnotation("B"), replaced, new TestAnnotation("C")]);

        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(moved);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion

    #region tests manually produce notifications

    [TestMethod]
    public void Forward()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([moved, replaced]);

        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalParent, oldIndex,
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));

        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(1, clonedPartition.Contents[0].GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Contents[0].GetAnnotations()[0].GetId());
    }

    [TestMethod]
    public void Backward()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([replaced, moved]);

        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 0;
        var oldIndex = 1;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalParent, oldIndex,
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));

        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(1, clonedPartition.Contents[0].GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Contents[0].GetAnnotations()[0].GetId());
    }

    [TestMethod]
    public void Backward_MoreThanThreeNodes()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("A"), replaced, new TestAnnotation("B"), moved, new TestAnnotation("C")]);

        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 3;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalParent, oldIndex,
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));

        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(4, clonedPartition.Contents[0].GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Contents[0].GetAnnotations()[1].GetId());
    }


    [TestMethod]
    public void Forward_MoreThanThreeNodes()
    {
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("A"), moved, new TestAnnotation("B"), replaced, new TestAnnotation("C")]);

        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 3;
        var oldIndex = 1;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalParent, oldIndex,
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));

        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(4, clonedPartition.Contents[0].GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Contents[0].GetAnnotations()[2].GetId());
    }
    
    [TestMethod]
    public void not_matching_node_ids()
    {
        var replaced = new TestAnnotation("replaced");
        var nodeWithAnotherId = new TestAnnotation("node-with-another-id");
        var moved = new TestAnnotation("moved");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Contents = [originalParent] };
        originalParent.AddAnnotations([moved, replaced, nodeWithAnotherId]);

        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var notification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalParent, oldIndex,
            nodeWithAnotherId, new NumericNotificationId("annotationMovedAndReplaced", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }

    #endregion
}