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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Optional.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class NullTests
{
    [TestMethod]
    public void Null()
    {
        var parent = new Geometry("g");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        parent.Documentation = null;

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new Geometry("g");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, null);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Existing()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(oldDoc, args.DeletedChild);
        });

        parent.Documentation = null;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_documentation, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(oldDoc, args.DeletedChild);
        });

        parent.Set(ShapesLanguage.Instance.Geometry_documentation, null);

        Assert.AreEqual(1, notifications);
    }
}