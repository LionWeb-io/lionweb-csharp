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
public class SingleTests
{
    [TestMethod]
    public void Single()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        offsetDuplicate.Offset = coord;

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void Setter()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
        });

        offsetDuplicate.SetOffset(coord);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Reflective()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
        });

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        offsetDuplicate.Offset = coord;

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.IsInstanceOfType<ChildAddedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromOtherParent_Replace()
    {
        var replacedCoord = new Coord("replaced");
        var offsetDuplicate = new OffsetDuplicate("od") { Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        offsetDuplicate.Offset = coord;

        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromOtherParent_Replace_Reflective()
    {
        var replacedCoord = new Coord("replaced");
        var offsetDuplicate = new OffsetDuplicate("od") { Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        var notificationObserver = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(notificationObserver);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.IsInstanceOfType<ChildReplacedNotification>(notificationObserver.Notifications[0]);
        Assert.AreEqual(1, notificationObserver.Count);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord] };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        });

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord] };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        });

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent_Replace()
    {
        var replacedCoord = new Coord("replacedCoord");
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord], Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
            Assert.AreEqual(replacedCoord, args.ReplacedChild);
        });

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent_Replace_Reflective()
    {
        var replacedCoord = new Coord("replacedCoord");
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord], Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
            Assert.AreEqual(replacedCoord, args.ReplacedChild);
        });

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, notifications);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
            Assert.AreEqual(oldCoord, args.ReplacedChild);
        });

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
            Assert.AreEqual(oldCoord, args.ReplacedChild);
        });

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Existing_Same()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        offsetDuplicate.Offset = oldCoord;

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Existing_Same_Reflective()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildReplacedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedInSameContainmentNotification>((_, _) => notifications++);
        parent.GetNotificationSender().Subscribe<ChildMovedFromOtherContainmentInSameParentNotification>((_, _) => notifications++);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, oldCoord);

        Assert.AreEqual(0, notifications);
    }

    #endregion
}