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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class MultipleCollectionRemoveTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.AddShapes(values);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.AddShapes(values);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        parent.RemoveShapes(values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void HalfContained_Reflective()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circle });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        int[] indices = [0, 1];
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(indices[notifications], args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.RemoveShapes(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Mixed_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        int[] indices = [0, 1];
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.Containment);
            Assert.AreEqual(indices[notifications], args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, notifications);
    }
}