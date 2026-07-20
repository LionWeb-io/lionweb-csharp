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

using LionWeb.Core.Notification.Partition;
using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required.Listener;

[TestClass]
public class SingleTests: NotificationTestsBase
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.SetOffset(coord);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(coord, notifications[0].NewChild);
    }

    [TestMethod]
    public void Reflective()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        var notifications = observer.AssertOfType<ChildAddedNotification>(1);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(coord, notifications[0].NewChild);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Offset = coord;

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(coord, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord] };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        var notifications = observer.AssertOfType<ChildMovedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(coord, notifications[0].MovedChild);
    }

    [TestMethod]
    public void FromSameParent_Replace()
    {
        var replacedCoord = new Coord("replacedCoord");
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord], Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Offset = coord;

        var notifications = observer.AssertOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(coord, notifications[0].MovedChild);
        Assert.AreEqual(replacedCoord, notifications[0].ReplacedChild);
    }

    [TestMethod]
    public void FromSameParent_Replace_Reflective()
    {
        var replacedCoord = new Coord("replacedCoord");
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord], Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        var notifications = observer.AssertOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(1);
        Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, notifications[0].OldContainment);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].NewContainment);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(coord, notifications[0].MovedChild);
        Assert.AreEqual(replacedCoord, notifications[0].ReplacedChild);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Offset = coord;

        var notifications = observer.AssertOfType<ChildReplacedNotification>(1);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(coord, notifications[0].NewChild);
        Assert.AreEqual(oldCoord, notifications[0].ReplacedChild);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        var notifications = observer.AssertOfType<ChildReplacedNotification>(1);
        Assert.AreSame(offsetDuplicate, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(coord, notifications[0].NewChild);
        Assert.AreEqual(oldCoord, notifications[0].ReplacedChild);
    }

    [TestMethod]
    public void Existing_Same()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Offset = oldCoord;

        observer.AssertEmpty();
    }

    [TestMethod]
    public void Existing_Same_Reflective()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, oldCoord);

        observer.AssertEmpty();
    }

    #endregion
}
