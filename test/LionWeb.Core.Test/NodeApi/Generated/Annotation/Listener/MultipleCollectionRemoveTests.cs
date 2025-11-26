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
public class MultipleCollectionRemoveTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        line.AddAnnotations([valueA, valueB]);
        var values = new List<INode>() { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        line.AddAnnotations([valueA, valueB]);
        var values = new List<INode>() { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.Set(null, new List<INode> { });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, _) => notifications++);

        line.RemoveAnnotations(values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void NonContained()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, _) => notifications++);

        line.RemoveAnnotations(values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void HalfContained()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var values = new INode[] { valueA, docA };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(docA, args.DeletedAnnotation);
            notifications++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void HalfContained_Reflective()
    {
        var docA = new Documentation("cA");
        var docB = new Documentation("cB");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var valueA = new BillOfMaterials("sA");
        var values = new INode[] { valueA, docA };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(docA, args.DeletedAnnotation);
            notifications++;
        });

        line.Set(null, new List<INode> { docB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Only()
    {
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.Set(null, new List<INode> {  });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Last()
    {
        var doc = new Documentation("cId");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var doc = new Documentation("cId");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.Set(null, new List<INode> { doc });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, valueA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, valueA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.Set(null, new List<INode> { docA, docB });

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Mixed()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, docA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        int[] indices = [0, 1];
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(indices[notifications], args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.RemoveAnnotations(values);

        Assert.AreEqual(2, notifications);
    }

    [TestMethod]
    public void Mixed_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([valueA, docA, valueB, docB]);
        var values = new INode[] { valueA, valueB };

        int notifications = 0;
        int[] indices = [0, 1];
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(indices[notifications], args.Index);
            Assert.AreEqual(values[notifications], args.DeletedAnnotation);
            notifications++;
        });

        line.Set(null, new List<INode> { docA, docB });

        Assert.AreEqual(2, notifications);
    }
}