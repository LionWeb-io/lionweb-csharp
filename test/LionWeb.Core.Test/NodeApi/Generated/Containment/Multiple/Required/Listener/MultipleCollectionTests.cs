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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewChild);
            notifications++;
        });

        compositeShape.AddParts(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values);

        Assert.AreEqual(2, notifications);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewChild);
            notifications++;
        });

        compositeShape.InsertParts(0, values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1 + notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewChild);
            notifications++;
        });

        compositeShape.InsertParts(1, values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Insert_Two_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildAddedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1 + notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts,
            new List<INode> { circleA, valueA, valueB, circleB });

        Assert.AreEqual(2, notifications);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => compositeShape.RemoveParts(values));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        Assert.ThrowsExactly<InvalidValueException>(() => compositeShape.RemoveParts(values));

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, _) => notifications++);

        compositeShape.RemoveParts(values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            notifications++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Remove_HalfContained_Reflective()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(circleA, args.DeletedChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Last_Reflective()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circle });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Between_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        int[] indexes = { 0, 1 };
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indexes[notifications], args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        compositeShape.RemoveParts(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Remove_Mixed_Reflective()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };

        int notifications = 0;
        int[] indexes = { 0, 1 };
        parent.GetNotificationSender().Subscribe<ChildDeletedNotification>((_, args) =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.Containment);
            Assert.AreEqual(indexes[notifications], args.Index);
            Assert.AreEqual(values[notifications], args.DeletedChild);
            notifications++;
        });

        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { circleA, circleB });

        Assert.AreEqual(2, notifications);
    }

    #endregion
}