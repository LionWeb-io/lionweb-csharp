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
public class SingleTests
{
    [TestMethod]
    public void Add()
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

        line.AddAnnotations([bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_Reflective()
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

        line.Set(null, new List<INode> { bom });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromOtherParent()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
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
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.AddAnnotations([bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromOtherParent_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
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
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { bom });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromSameParent()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([bom, new Documentation("doc")]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.AddAnnotations([bom]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromSameParent_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        var doc = new Documentation("doc");
        line.AddAnnotations([bom, doc]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, args) =>
        {
            notifications++;
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(bom, args.MovedAnnotation);
        });

        line.Set(null, new List<INode> { doc, bom });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Add_FromSameParent_NoOp()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([new Documentation("doc"), bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, _) => notifications++);

        line.AddAnnotations([bom]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Add_FromSameParent_NoOp_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        var doc = new Documentation("doc");
        line.AddAnnotations([doc, bom]);

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationMovedInSameParentNotification>((_, _) => notifications++);

        line.Set(null, new List<INode> { doc, bom });

        Assert.AreEqual(0, notifications);
    }
}