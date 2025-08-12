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
using Notification.Partition;

[TestClass]
public class PropertyTests_Listener
{
    [TestMethod]
    public void PropertyAdded_Optional()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("d");
        parent.Documentation = doc;

        int events = 0;
        parent.GetPublisher().Subscribe<PropertyAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(doc, args.Node);
            Assert.AreSame(ShapesLanguage.Instance.Documentation_text, args.Property);
            Assert.AreEqual("hello", args.NewValue);
        });

        doc.Text = "hello";

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void PropertyDeleted_Optional()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("d");
        parent.Documentation = doc;
        doc.Text = "hello";

        int events = 0;
        parent.GetPublisher().Subscribe<PropertyDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(doc, args.Node);
            Assert.AreSame(ShapesLanguage.Instance.Documentation_text, args.Property);
            Assert.AreEqual("hello", args.OldValue);
        });
        doc.Text = null;


        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void PropertyChanged_Optional()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("d");
        parent.Documentation = doc;
        doc.Text = "hello";

        int events = 0;
        parent.GetPublisher().Subscribe<PropertyChangedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(doc, args.Node);
            Assert.AreSame(ShapesLanguage.Instance.Documentation_text, args.Property);
            Assert.AreEqual("hello", args.OldValue);
            Assert.AreEqual("bye", args.NewValue);
        });

        int badEvents = 0;
        parent.GetPublisher().Subscribe<PropertyAddedNotification>((_, _) => badEvents++);
        parent.GetPublisher().Subscribe<PropertyDeletedNotification>((_, _) => badEvents++);

        doc.Text = "bye";

        Assert.AreEqual(1, events);
        Assert.AreEqual(0, badEvents);
    }
    
    [TestMethod]
    public void PropertyAdded_Required()
    {
        var parent = new Geometry("g");
        var circle = new Circle("d");
        parent.AddShapes([circle]);

        int events = 0;
        parent.GetPublisher().Subscribe<PropertyAddedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(circle, args.Node);
            Assert.AreSame(ShapesLanguage.Instance.IShape_uuid, args.Property);
            Assert.AreEqual("hello", args.NewValue);
        });

        circle.Uuid = "hello";

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void PropertyDeleted_Required()
    {
        var parent = new Geometry("g");
        var circle = new Circle("d");
        parent.AddShapes([circle]);
        circle.Uuid = "hello";

        int events = 0;
        parent.GetPublisher().Subscribe<PropertyDeletedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(circle, args.Node);
            Assert.AreSame(ShapesLanguage.Instance.IShape_uuid, args.Property);
            Assert.AreEqual("hello", args.OldValue);
        });
        Assert.ThrowsException<InvalidValueException>(() => circle.Uuid = null);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void PropertyChanged_Required()
    {
        var parent = new Geometry("g");
        var circle = new Circle("d");
        parent.AddShapes([circle]);
        circle.Uuid = "hello";

        int events = 0;
        parent.GetPublisher().Subscribe<PropertyChangedNotification>((_, args) =>
        {
            events++;
            Assert.AreSame(circle, args.Node);
            Assert.AreSame(ShapesLanguage.Instance.IShape_uuid, args.Property);
            Assert.AreEqual("hello", args.OldValue);
            Assert.AreEqual("bye", args.NewValue);
        });

        int badEvents = 0;
        parent.GetPublisher().Subscribe<PropertyAddedNotification>((_, _) => badEvents++);
        parent.GetPublisher().Subscribe<PropertyDeletedNotification>((_, _) => badEvents++);

        circle.Uuid = "bye";

        Assert.AreEqual(1, events);
        Assert.AreEqual(0, badEvents);
    }
}