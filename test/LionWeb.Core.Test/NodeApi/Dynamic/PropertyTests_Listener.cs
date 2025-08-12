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

namespace LionWeb.Core.Test.NodeApi.Dynamic;

using Notification.Partition;

[TestClass]
public class PropertyTests_Listener : DynamicNodeTestsBase
{
    [TestMethod]
    public void PropertyAdded()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("d");
        parent.Set(Geometry_documentation, doc);

        int notifications = 0;
        parent.GetProcessor().Subscribe<PropertyAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(doc, args.Node);
            Assert.AreSame(Documentation_text, args.Property);
            Assert.AreEqual("hello", args.NewValue);
        });

        doc.Set(Documentation_text, "hello");

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("d");
        parent.Set(Geometry_documentation, doc);
        doc.Set(Documentation_text, "hello");

        int notifications = 0;
        parent.GetProcessor().Subscribe<PropertyDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(doc, args.Node);
            Assert.AreSame(Documentation_text, args.Property);
            Assert.AreEqual("hello", args.OldValue);
        });
        doc.Set(Documentation_text, null);


        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("d");
        parent.Set(Geometry_documentation, doc);
        doc.Set(Documentation_text, "hello");

        int notifications = 0;
        parent.GetProcessor().Subscribe<PropertyChangedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(doc, args.Node);
            Assert.AreSame(Documentation_text, args.Property);
            Assert.AreEqual("hello", args.OldValue);
            Assert.AreEqual("bye", args.NewValue);
        });

        int badNotifications = 0;
        parent.GetProcessor().Subscribe<PropertyAddedNotification>((_, _) => badNotifications++);
        parent.GetProcessor().Subscribe<PropertyDeletedNotification>((_, _) => badNotifications++);

        doc.Set(Documentation_text, "bye");

        Assert.AreEqual(1, notifications);
        Assert.AreEqual(0, badNotifications);
    }
}