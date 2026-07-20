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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Single.Listener;

[TestClass]
public class ListenerTests
{
    [TestMethod]
    public void ReferenceAdded_Optional()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.AltSource = reference;

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(reference), notifications[0].NewTarget);
    }

    [TestMethod]
    public void ReferenceAdded_Optional_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, reference);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(reference), notifications[0].NewTarget);
    }

    [TestMethod]
    public void ReferenceDeleted_Optional()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.AltSource = reference;

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.AltSource = null;

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(reference), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void ReferenceDeleted_Optional_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.AltSource = reference;

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, null);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(reference), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void ReferenceChanged_Optional()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var oldTarget = new Line("oldTarget");
        source.AltSource = oldTarget;
        var newTarget = new Line("newTarget");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.AltSource = newTarget;

        var notifications = observer.AssertOfType<ReferenceChangedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(oldTarget), notifications[0].OldTarget);
        Assert.AreEqual(ReferenceTarget.FromNode(newTarget), notifications[0].NewTarget);
        observer.AssertNone<ReferenceAddedNotification>();
        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void ReferenceChanged_Optional_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var oldTarget = new Line("oldTarget");
        source.AltSource = oldTarget;
        var newTarget = new Line("newTarget");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, newTarget);

        var notifications = observer.AssertOfType<ReferenceChangedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(oldTarget), notifications[0].OldTarget);
        Assert.AreEqual(ReferenceTarget.FromNode(newTarget), notifications[0].NewTarget);
        observer.AssertNone<ReferenceAddedNotification>();
        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void ReferenceAdded_Required_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_source, reference);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(reference), notifications[0].NewTarget);
    }

    [TestMethod]
    public void ReferenceDeleted_Required()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.Source = reference;

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.Source = null);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void ReferenceDeleted_Required_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.Source = reference;

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() =>
            source.Set(ShapesLanguage.Instance.OffsetDuplicate_source, null));

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void ReferenceChanged_Required()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var oldTarget = new Line("oldTarget");
        source.Source = oldTarget;
        var newTarget = new Line("newTarget");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Source = newTarget;

        var notifications = observer.AssertOfType<ReferenceChangedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(oldTarget), notifications[0].OldTarget);
        Assert.AreEqual(ReferenceTarget.FromNode(newTarget), notifications[0].NewTarget);
        observer.AssertNone<ReferenceAddedNotification>();
        observer.AssertNone<ReferenceDeletedNotification>();
    }
    
    [TestMethod]
    public void ReferenceChanged_Required_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var oldTarget = new Line("oldTarget");
        source.Source = oldTarget;
        var newTarget = new Line("newTarget");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_source, newTarget);

        var notifications = observer.AssertOfType<ReferenceChangedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(oldTarget), notifications[0].OldTarget);
        Assert.AreEqual(ReferenceTarget.FromNode(newTarget), notifications[0].NewTarget);
        observer.AssertNone<ReferenceAddedNotification>();
        observer.AssertNone<ReferenceDeletedNotification>();
    }
}
