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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Handler;
using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class NotificationApiTests
{
    #region get informed about changes

    [TestMethod]
    public void ReceiveNotifications_from_partition_via_subscribe()
    {
        var node = new Geometry("partition");

        int notificationCount = 0;
        var sender = node.GetNotificationHandler();
        sender?.Subscribe<IPartitionNotification>((sender, notification) =>
        {
            notificationCount++;
            Console.WriteLine(notification);
        });

        node.Documentation = new Documentation("added");

        Assert.AreEqual(1, notificationCount);
    }

    [TestMethod]
    public void ReceiveNotifications_from_partition_via_connected_handlers()
    {
        var node = new Geometry("partition");

        var sender = node.GetNotificationHandler();
        var receiver = new Observer();
        INotificationHandler.Connect(from: sender, to: receiver);

        node.Documentation = new Documentation("added");

        Assert.AreEqual(1, receiver.NotificationCount);
    }

    [TestMethod]
    public void ReceiveNotifications_from_forest()
    {
        var node = new Geometry("partition");
        var forest = new Forest();

        var sender = forest.GetNotificationHandler();
        var receiver = new Observer();
        INotificationHandler.Connect(from: sender, to: receiver);
        
        forest.AddPartitions([node]); 

        Assert.AreEqual(1, receiver.NotificationCount);
    }

    [TestMethod]
    public void ReceiveNotifications_from_forest_and_partition()
    {
        var node = new Geometry("partition");
        var forest = new Forest();

        var forestHandler = forest.GetNotificationHandler();
        var partitionHandler = node.GetNotificationHandler();
        
        var forestReceiver = new Observer();
        var partitionReceiver = new Observer();
        
        INotificationHandler.Connect(from: forestHandler, to: forestReceiver);
        INotificationHandler.Connect(from: partitionHandler, to: partitionReceiver);
        
        forest.AddPartitions([node]); 
        node.Documentation = new Documentation("added");

        Assert.AreEqual(1, forestReceiver.NotificationCount);
        Assert.AreEqual(1, partitionReceiver.NotificationCount);
    }

    #endregion

    #region replicate changes

    [TestMethod]
    public void ReplicateChanges_Partition()
    {
    }

    #endregion
}

internal class Observer : IReceivingNotificationHandler
{
    public int NotificationCount { get; private set; }
    public void Dispose() => throw new NotImplementedException();

    public bool Handles(params Type[] notificationTypes) => true;

    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        NotificationCount++;
        Console.WriteLine(notification);
    }
}