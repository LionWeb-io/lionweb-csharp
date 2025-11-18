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

namespace LionWeb.Protocol.Delta.Test.Pipe;

using Core.M1;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Core.Test.Notification;

[TestClass]
public class NotificationsTestSerialized : DeltaTestsBase
{
    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new Circle("c");
        var originalPartition = new Geometry("a") { Shapes = [circle] };
        var clonedPartition = CreateDeltaReplicator(originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        circle.Name = "Hello";

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<PropertyAddedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildReplaced_Single_with_same_node_id()
    {
        var documentation = new Documentation("doc") { Text = "a" };
        var originalPartition = new Geometry("a") { Documentation = documentation };

        var clonedPartition = CreateDeltaReplicator(originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        var documentation2 = new Documentation("doc") { Text = "b" };
        originalPartition.Documentation = documentation2;

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ChildAdded_ForwardReference()
    {
        var originalPartition = new LinkTestConcept("partition");

        var clonedPartition = CreateDeltaReplicator(originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        var referencedChildA = new LinkTestConcept("referencedChildA");
        var referencedChildB = new LinkTestConcept("referencedChildB");
        var childWithForwardReference = new LinkTestConcept("childWithForwardReference")
        {
            Reference_0_1 = referencedChildA,
            Reference_1 = referencedChildB,
            Reference_0_n = [referencedChildA, referencedChildB],
            Reference_1_n = [referencedChildB, referencedChildB]
        };

        originalPartition.Containment_0_1 = childWithForwardReference;
        originalPartition.AddContainment_0_n([referencedChildA, referencedChildB]);

        Assert.AreEqual(3, notificationObserver.Count);
        foreach (var notification in notificationObserver.Notifications)
        {
            Assert.IsInstanceOfType<ChildAddedNotification>(notification);
        }

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void ChildReplaced_ForwardReference()
    {
        var originalPartition = new LinkTestConcept("partition")
        {
            Containment_1 = new LinkTestConcept("replacedChild")
        };

        var clonedPartition = CreateDeltaReplicator(originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        var referencedChild = new LinkTestConcept("referencedChild");
        var childWithForwardReference = new LinkTestConcept("childWithForwardReference")
        {
            Reference_0_1 = referencedChild
        };

        originalPartition.Containment_0_1 = childWithForwardReference;
        originalPartition.Containment_1 = referencedChild;

        Assert.AreEqual(2, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[1]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void PartitionAdded_ForwardReference()
    {
        var originalPartition = new LinkTestConcept("partition");
        var originalForest = new Forest();
        originalForest.AddPartitions([originalPartition]);

        var clonedForest = CreateDeltaReplicator(originalForest);

        var notificationObserver = new NotificationObserver();
        originalForest.GetNotificationSender()!.ConnectTo(notificationObserver);

        var referencedPartition = new LinkTestConcept("referencedPartition");
        var childWithForwardReference = new LinkTestConcept("childWithForwardReference")
        {
            Reference_0_1 = referencedPartition
        };

        originalPartition.Containment_0_1 = childWithForwardReference;
        originalForest.AddPartitions([referencedPartition]);

        Assert.HasCount(2, notificationObserver.Notifications);
        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.IsInstanceOfType<PartitionAddedNotification>(notificationObserver.Notifications[1]);

        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }

    // TODO: Not yet possible: https://github.com/LionWeb-io/lionweb-integration-testing/issues/85
    // [TestMethod]
    // public void AnnotationAdded_ForwardReference()
    // {
    //     var originalPartition = new LinkTestConcept("partition");
    //
    //     var clonedPartition = CreateDeltaReplicator(originalPartition);
    //
    //     var notificationObserver = new NotificationObserver();
    //     originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);
    //
    //     var referencedAnnotation = new TestAnnotation("referencedAnnotation");
    //     var childWithForwardReference = new LinkTestConcept("childWithForwardReference")
    //     {
    //         AnnotationReference =  referencedAnnotation
    //     };
    //
    // originalPartition.Containment_0_1 = childWithForwardReference;
    //     originalPartition.AddAnnotations([referencedAnnotation]);
    //
    //     Assert.AreEqual(2, notificationObserver.Count);
    //     Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
    //     Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[1]);
    //
    //     AssertEquals([originalPartition], [clonedPartition]);
    // }
}