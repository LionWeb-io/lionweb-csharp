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
using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class DeletedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([deleted]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_First()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([deleted, new BillOfMaterials("bof")]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), deleted]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_not_matching_node_ids()
    {
        var deleted = new BillOfMaterials("deleted");
        var nodeWithAnotherId = new BillOfMaterials("node-with-another-id");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), deleted, nodeWithAnotherId]);

        var clonedPartition = ClonePartition(originalPartition);

        var notification = new AnnotationDeletedNotification(nodeWithAnotherId, originalPartition, 1,
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
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), deleted]);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);
        
        deleted.DetachFromParent();

        Assert.AreEqual(0, notificationObserver.Notifications.Count);
    }
}