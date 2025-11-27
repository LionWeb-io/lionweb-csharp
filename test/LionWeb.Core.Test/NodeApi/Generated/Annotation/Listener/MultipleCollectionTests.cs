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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.Listener;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewAnnotation);
            notifications++;
        });

        line.AddAnnotations(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewAnnotation);
            notifications++;
        });

        line.Set(null, values);

        Assert.AreEqual(2, notifications);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewAnnotation);
            notifications++;
        });

        line.InsertAnnotations(0, values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Insert_Empty_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewAnnotation);
            notifications++;
        });

        line.Set(null, values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1 + notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewAnnotation);
            notifications++;
        });

        line.InsertAnnotations(1, values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Insert_Two_Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1 + notifications, args.Index);
            Assert.AreEqual(values[notifications], args.NewAnnotation);
            notifications++;
        });

        line.Set(null, new List<INode> { docA, valueA, valueB, docB });

        Assert.AreEqual(2, notifications);
    }

    #endregion
}