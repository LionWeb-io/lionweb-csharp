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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.Listener.Single;

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
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { line, circle });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
        });

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circle, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { line, circleA, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        parent.InsertShapes(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ReferenceAddedNotification>((_, args) =>
        {
            Assert.AreSame(parent, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.ReferenceGeometry_shapes, args.Reference);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(ReferenceTarget.FromNode(line), args.NewTarget);
            notifications++;
        });

        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }
}