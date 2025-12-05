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
public class MovedAndReplacedFromOtherParentTests : ReplicatorTestsBase
{
    [TestMethod]
    [Ignore("Should emit AnnotationMovedAndReplacedFromOtherParentNotification")]
    public void Multiple_Only()
    {
        var replaced = new TestAnnotation("replaced");
        var line = new LinkTestConcept("line");
        line.AddAnnotations([replaced]);
        
        var origin = new LinkTestConcept("origin");
        var moved = new TestAnnotation("moved");
        origin.AddAnnotations([moved]);
        var originalPartition = new TestPartition("a") { Links = [origin, line] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);
        
        replaced.ReplaceWith(moved);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<AnnotationMovedAndReplacedFromOtherParentNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    #region tests manually produce notifications

    [TestMethod]
    public void Multiple_Only_ProducesNotification()
    {
        var replaced = new TestAnnotation("replaced");
        var line = new LinkTestConcept("line");
        line.AddAnnotations([replaced]);
        
        var origin = new LinkTestConcept("origin");
        var moved = new TestAnnotation("moved");
        origin.AddAnnotations([moved]);
        var originalPartition = new TestPartition("a") { Links = [origin, line] };

        var clonedPartition = ClonePartition(originalPartition);
        
        var newIndex = 0;
        var oldIndex = 0;
        var notification = new AnnotationMovedAndReplacedFromOtherParentNotification(line, newIndex, moved, origin, oldIndex, replaced, 
            new NumericNotificationId("AnnotationMovedAndReplacedFromOtherParent", 0));
        
        CreatePartitionReplicator(clonedPartition, notification);
        
        Assert.AreEqual(1, clonedPartition.Links[1].GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Links[1].GetAnnotations()[0].GetId());
    }

    [TestMethod]
    public void Multiple_First_ProducesNotification()
    {
        var replaced = new TestAnnotation("replaced");
        var line = new LinkTestConcept("line");
        line.AddAnnotations([replaced, new TestAnnotation("bof")]);
        
        var origin = new LinkTestConcept("origin");
        var moved = new TestAnnotation("moved");
        origin.AddAnnotations([moved]);
        var originalPartition = new TestPartition("a") { Links = [origin, line] };

        var clonedPartition = ClonePartition(originalPartition);
        
        var newIndex = 0;
        var oldIndex = 0;
        var notification = new AnnotationMovedAndReplacedFromOtherParentNotification(line, newIndex, moved, origin, oldIndex, replaced, 
            new NumericNotificationId("AnnotationMovedAndReplacedFromOtherParent", 0));
        
        CreatePartitionReplicator(clonedPartition, notification);
       
        Assert.AreEqual(2, clonedPartition.Links[1].GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Links[1].GetAnnotations()[0].GetId());
    }
    
    [TestMethod]
    public void Multiple_Last_ProducesNotification()
    {
        var replaced = new TestAnnotation("replaced");
        var line = new LinkTestConcept("line");
        line.AddAnnotations([new TestAnnotation("bof"), replaced]);
        
        var origin = new LinkTestConcept("origin");
        var moved = new TestAnnotation("moved");
        origin.AddAnnotations([moved]);
        var originalPartition = new TestPartition("a") { Links = [origin, line] };

        var clonedPartition = ClonePartition(originalPartition);
        
        var newIndex = 1;
        var oldIndex = 0;
        var notification = new AnnotationMovedAndReplacedFromOtherParentNotification(line, newIndex, moved, origin, oldIndex, replaced, 
            new NumericNotificationId("AnnotationMovedAndReplacedFromOtherParent", 0));
        
        CreatePartitionReplicator(clonedPartition, notification);
       
        Assert.AreEqual(2, clonedPartition.Links[1].GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Links[1].GetAnnotations()[^1].GetId());
    }

    [TestMethod]
    public void not_matching_node_ids()
    {
        var replaced = new TestAnnotation("replaced");
        var nodeWithAnotherId = new TestAnnotation("node-with-another-id");
        var line = new LinkTestConcept("line");
        line.AddAnnotations([new TestAnnotation("bof"), replaced, nodeWithAnotherId]);
        
        var origin = new LinkTestConcept("origin");
        var moved = new TestAnnotation("moved");
        origin.AddAnnotations([moved]);
        var originalPartition = new TestPartition("a") { Links = [origin, line] };

        var clonedPartition = ClonePartition(originalPartition);
        
        var newIndex = 1;
        var oldIndex = 0;
        var notification = new AnnotationMovedAndReplacedFromOtherParentNotification(line, newIndex, moved, origin, oldIndex, nodeWithAnotherId, 
            new NumericNotificationId("AnnotationMovedAndReplacedFromOtherParent", 0));
        
        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }

    #endregion 
}