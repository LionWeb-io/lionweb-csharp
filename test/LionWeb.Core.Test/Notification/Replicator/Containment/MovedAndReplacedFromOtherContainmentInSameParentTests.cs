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
using Core.Notification.Partition;
using Languages.Generated.V2025_1.Shapes.M2;
using Languages.Generated.V2025_1.TestLanguage;
using M1;

[TestClass]
public class MovedAndReplacedFromOtherContainmentInSameParentTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Single()
    {
        var line = new Line("l")
        {
            Start = new Coord("moved"), End = new Coord("replaced")
        };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonedPartition = ClonePartition(originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        line.End = line.Start;

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(notificationObserver.Notifications[0]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single_ReplaceWith()
    {
        var line = new Line("l")
        {
            Start = new Coord("moved"), End = new Coord("replaced")
        };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        line.End.ReplaceWith(line.Start);

        Assert.AreEqual(1, notificationObserver.Count);
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(notificationObserver.Notifications[0]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single_not_matching_node_ids()
    {
        var moved = new Coord("moved");
        var replaced = new Coord("replaced");
        var line = new Line("l")
        {
            Start = moved, 
            End = replaced
        };
        
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonedPartition = ClonePartition(originalPartition);

        var nodeWithAnotherId = new Coord("node-with-another-id");
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(nodeWithAnotherId);

        var notification = new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(ShapesLanguage.Instance.Geometry_shapes, 0, 
            moved, originalPartition, ShapesLanguage.Instance.Geometry_shapes, 0, nodeWithAnotherId, 
            new NumericNotificationId("childMovedAndReplacedFromOtherContainmentInSameParentNotification", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        });
    }


    [TestMethod]
    public void Notification_not_matching_node_ids()
    {
        var moved = new LinkTestConcept("moved");
        var replaced = new LinkTestConcept("replaced");
        var nodeWithAnotherId = new LinkTestConcept("node-with-another-id");
        var originalPartition = new LinkTestConcept("a")
        {
            Containment_0_n = [moved],
            Containment_1_n = [replaced, nodeWithAnotherId]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 
            0, moved, originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, nodeWithAnotherId, 
            new NumericNotificationId("childMovedAndReplacedFromOtherContainmentInSameParentNotification", 0));

        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });
    }
}