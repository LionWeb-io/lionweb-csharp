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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.NewTarget);
            notifications++;
        });

        parent.AddShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.NewTarget);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, values);

        Assert.AreEqual(2, notifications);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.NewTarget);
            notifications++;
        });

        parent.InsertShapes(0, values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1 + notifications, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.NewTarget);
            notifications++;
        });

        parent.InsertShapes(1, values);

        Assert.AreEqual(2, notifications);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        parent.AddShapes(values);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.DeletedTarget);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new ReferenceGeometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, _) => notifications++);

        parent.RemoveShapes(values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new ReferenceGeometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(circleA), args.DeletedTarget);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.DeletedTarget);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.DeletedTarget);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.DeletedTarget);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new ReferenceGeometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        int[] indexes = { 0, 1 };
        parent.GetNotificationSender().Subscribe<ReferenceDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(indexes[notifications], args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(values[notifications]), args.DeletedTarget);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    #endregion
}