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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.AddMaterials(values));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() =>         materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape>{}));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.InsertMaterials(0, values));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.RemoveMaterials(values));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var value = new Circle("myId");
        materialGroup.AddMaterials([value]);
        var values = new List<Coord>();

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(
            () => materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));

        Assert.AreEqual(0, notifications);
    }
}