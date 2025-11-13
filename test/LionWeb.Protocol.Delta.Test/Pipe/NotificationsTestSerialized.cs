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

using Core.Notification.Partition;
using Core.Test.Languages.Generated.V2024_1.Shapes.M2;
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
}