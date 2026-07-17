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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.Listener;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.AddMaterials(values);

        var notifications = observer.OfType<ReferenceAddedNotification>(2);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(materialGroup, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape>{valueA,valueB});

        var notifications = observer.OfType<ReferenceAddedNotification>(2);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(materialGroup, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.InsertMaterials(0, values);

        var notifications = observer.OfType<ReferenceAddedNotification>(2);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(materialGroup, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.InsertMaterials(1, values);

        var notifications = observer.OfType<ReferenceAddedNotification>(2);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(materialGroup, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[1].Reference);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs");
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.RemoveMaterials(values));

        observer.AssertNone<ReferenceDeletedNotification>();
    }


    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [valueA, valueB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => materialGroup.RemoveMaterials(values));

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.RemoveMaterials(values);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.RemoveMaterials(values);

        var notifications = observer.OfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(circleA), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circle, valueA, valueB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.RemoveMaterials(values);

        var notifications = observer.OfType<ReferenceDeletedNotification>(2);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].DeletedTarget);
        Assert.AreSame(materialGroup, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].DeletedTarget);
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [circleA, valueA, valueB, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.RemoveMaterials(values);

        var notifications = observer.OfType<ReferenceDeletedNotification>(2);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].DeletedTarget);
        Assert.AreSame(materialGroup, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].DeletedTarget);
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g");
        var materialGroup = new MaterialGroup("cs") { Materials = [valueA, circleA, valueB, circleB] };
        parent.AddAnnotations([new BillOfMaterials("bom") { DefaultGroup = materialGroup }]);
        var values = new IShape[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        materialGroup.RemoveMaterials(values);

        var notifications = observer.OfType<ReferenceDeletedNotification>(2);
        Assert.AreSame(materialGroup, notifications[0].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].DeletedTarget);
        Assert.AreSame(materialGroup, notifications[1].Parent);
        Assert.AreSame(ShapesLanguage.Instance.MaterialGroup_materials, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].DeletedTarget);
    }

    #endregion
}
