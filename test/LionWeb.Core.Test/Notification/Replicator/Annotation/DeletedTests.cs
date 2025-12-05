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

[TestClass]
public class DeletedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var deleted = new TestAnnotation("deleted");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Links = [originalParent] };
        originalParent.AddAnnotations([deleted]);

        var clonedPartition = ClonePartition(originalPartition);

        var sharedNodeMap = new SharedNodeMap();
        
        CreatePartitionReplicator(clonedPartition, originalPartition, sharedNodeMap);

        Assert.IsTrue(sharedNodeMap.ContainsKey(deleted.GetId()));

        originalParent.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);

        Assert.IsFalse(sharedNodeMap.ContainsKey(deleted.GetId()));
    }

    [TestMethod]
    public void Multiple_First()
    {
        var deleted = new TestAnnotation("deleted");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Links = [originalParent] };
        originalParent.AddAnnotations([deleted, new TestAnnotation("bof")]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalParent.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var deleted = new TestAnnotation("deleted");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Links = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("bof"), deleted]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalParent.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_not_matching_node_ids()
    {
        var deleted = new TestAnnotation("deleted");
        var nodeWithAnotherId = new TestAnnotation("node-with-another-id");
        var originalParent = new LinkTestConcept("a");
        var originalPartition = new TestPartition("partition") { Links = [originalParent] };
        originalParent.AddAnnotations([new TestAnnotation("bof"), deleted, nodeWithAnotherId]);

        var clonedPartition = ClonePartition(originalPartition);

        var notification = new AnnotationDeletedNotification(nodeWithAnotherId, originalParent, 1,
            new NumericNotificationId("annotationDeletedNotification", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }
    
    /// <summary>
    /// This test confirms that no notification is generated from DetachFromParent method
    /// TODO: This is a known bug, we want to have a notification emitted.
    /// </summary>
    [TestMethod]
    public void uses_detach_from_parent()
    {
        var deleted = new TestAnnotation("deleted");
        var originalPartition = new TestPartition("a");
        originalPartition.AddAnnotations([new TestAnnotation("bof"), deleted]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);
        
        deleted.DetachFromParent();

        Assert.AreEqual(0, notificationObserver.Notifications.Count);
    }
}