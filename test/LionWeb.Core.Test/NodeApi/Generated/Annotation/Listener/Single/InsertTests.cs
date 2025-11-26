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
public class InsertTests
{
    [TestMethod]
    public void Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(0, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(0, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { bom, doc });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(1, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { doc, bom });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(0, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { bom, docA, docB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(1, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { docA, bom, docB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.InsertAnnotations(2, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(bom, args.NewAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB, bom });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        var oldParent = new Line("oldParent");
        oldParent.AddAnnotations([new Documentation("doc"), bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedFromOtherParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.NewParent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.InsertAnnotations(2, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        var oldParent = new Line("oldParent");
        oldParent.AddAnnotations([new Documentation("doc"), bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedFromOtherParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.NewParent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB, bom });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.InsertAnnotations(2, [bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { docA, docB, bom });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent_NoOp()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, _) => notifications++);

        line.InsertAnnotations(1, [bom]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void FromSameParent_NoOp_Reflective()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([docA, bom, docB]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, _) => notifications++);

        line.Set(null, new List<INode> { docA, bom, docB });

        Assert.AreEqual(0, notifications);
    }
}