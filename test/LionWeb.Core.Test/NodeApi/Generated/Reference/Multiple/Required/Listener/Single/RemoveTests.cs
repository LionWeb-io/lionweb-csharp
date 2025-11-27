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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.Listener.Single;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.RemoveMaterials([line]));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Empty_Reflective()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() =>
            materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { }));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void NotContained()
    {
        var circle = new Circle("myC");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, _) => notifications++);

        materialGroup.RemoveMaterials([line]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Only()
    {
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [line] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.RemoveMaterials([line]));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [line] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape>{}));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [line, circle] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.DeletedTarget);
        });

        materialGroup.RemoveMaterials([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [line, circle] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.DeletedTarget);
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape>{circle});

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle, line] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.DeletedTarget);
        });

        materialGroup.RemoveMaterials([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle, line] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.DeletedTarget);
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape>{circle});

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, line, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.DeletedTarget);
        });

        materialGroup.RemoveMaterials([line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, line, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.DeletedTarget);
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape>{circleA, circleB});

        Assert.AreEqual(1, notifications);
    }
}