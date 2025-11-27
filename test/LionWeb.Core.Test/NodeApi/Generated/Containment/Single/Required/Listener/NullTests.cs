// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class NullTests
{
    [TestMethod]
    public void Null()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        Assert.ThrowsException<InvalidValueException>(() => offsetDuplicate.Offset = null);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Existing()
    {
        var offsetDuplicate = new OffsetDuplicate("od") { Offset = new Coord("myId") };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        Assert.ThrowsException<InvalidValueException>(() => offsetDuplicate.Offset = null);

        Assert.AreEqual(0, notifications);
    }
}