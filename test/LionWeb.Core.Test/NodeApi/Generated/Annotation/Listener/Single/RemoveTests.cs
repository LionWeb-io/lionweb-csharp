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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.Listener.Single;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.Shapes.M2;
using Notification;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, _) => notifications++);

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void NotContained()
    {
        var doc = new Documentation("myC");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, _) => notifications++);

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Only()
    {
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> {  });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void First()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom, doc]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom, doc]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> { doc });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Last()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> { doc });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, bom, docB]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.RemoveAnnotations([bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, bom, docB]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB });

        Assert.AreEqual(1, notifications);
    }
}