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
public class InsertTests
{
    [TestMethod]
    public void One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        materialGroup.InsertMaterials(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { line, circle });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        materialGroup.InsertMaterials(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { circle, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        materialGroup.InsertMaterials(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { line, circleA, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        materialGroup.InsertMaterials(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { circleA, line, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        materialGroup.InsertMaterials(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(materialGroup, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, args.Reference);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }
}