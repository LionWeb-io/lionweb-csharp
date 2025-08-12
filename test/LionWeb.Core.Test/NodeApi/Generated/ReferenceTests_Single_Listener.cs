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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;
using M1.Event.Partition;
using Notification.Partition;

[TestClass]
public class ReferenceTests_Single_Listener
{
    [TestMethod]
    public void ReferenceAdded_Optional()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, reference), args.NewTarget);
        });

        source.AltSource = reference;

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void ReferenceAdded_Optional_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, reference), args.NewTarget);
        });

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, reference);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void ReferenceDeleted_Optional()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.AltSource = reference;

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, reference), args.DeletedTarget);
        });

        source.AltSource = null;

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void ReferenceDeleted_Optional_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.AltSource = reference;

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, reference), args.DeletedTarget);
        });

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, null);

        Assert.AreEqual(1, events);
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

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceChangedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, oldTarget), args.OldTarget);
            Assert.AreEqual(new ReferenceTarget(null, newTarget), args.NewTarget);
        });

        int badEvents = 0;
        partition.GetProcessor().Subscribe<ReferenceAddedNotification>((_, _) => badEvents++);
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, _) => badEvents++);

        source.AltSource = newTarget;

        Assert.AreEqual(1, events);
        Assert.AreEqual(0, badEvents);
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

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceChangedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_altSource, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, oldTarget), args.OldTarget);
            Assert.AreEqual(new ReferenceTarget(null, newTarget), args.NewTarget);
        });

        int badEvents = 0;
        partition.GetProcessor().Subscribe<ReferenceAddedNotification>((_, _) => badEvents++);
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, _) => badEvents++);

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, newTarget);

        Assert.AreEqual(1, events);
        Assert.AreEqual(0, badEvents);
    }

    [TestMethod]
    public void ReferenceAdded_Required_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, reference), args.NewTarget);
        });

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_source, reference);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void ReferenceDeleted_Required()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.Source = reference;

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, reference), args.DeletedTarget);
        });

        Assert.ThrowsException<InvalidValueException>(() => source.Source = null);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void ReferenceDeleted_Required_Reflective()
    {
        var partition = new Geometry("g");
        var source = new OffsetDuplicate("g");
        partition.AddShapes([source]);
        var reference = new Line("myId");
        source.Source = reference;

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, reference), args.DeletedTarget);
        });

        Assert.ThrowsException<InvalidValueException>(() =>
            source.Set(ShapesLanguage.Instance.OffsetDuplicate_source, null));

        Assert.AreEqual(0, events);
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

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceChangedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, oldTarget), args.OldTarget);
            Assert.AreEqual(new ReferenceTarget(null, newTarget), args.NewTarget);
        });

        int badEvents = 0;
        partition.GetProcessor().Subscribe<ReferenceAddedNotification>((_, _) => badEvents++);
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, _) => badEvents++);

        source.Source = newTarget;

        Assert.AreEqual(1, events);
        Assert.AreEqual(0, badEvents);
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

        int events = 0;
        partition.GetProcessor().Subscribe<ReferenceChangedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(source, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_source, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(new ReferenceTarget(null, oldTarget), args.OldTarget);
            Assert.AreEqual(new ReferenceTarget(null, newTarget), args.NewTarget);
        });

        int badEvents = 0;
        partition.GetProcessor().Subscribe<ReferenceAddedNotification>((_, _) => badEvents++);
        partition.GetProcessor().Subscribe<ReferenceDeletedNotification>((_, _) => badEvents++);

        source.Set(ShapesLanguage.Instance.OffsetDuplicate_source, newTarget);

        Assert.AreEqual(1, events);
        Assert.AreEqual(0, badEvents);
    }
}