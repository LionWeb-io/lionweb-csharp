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
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, _) => notifications++);

        line.AddAnnotations(values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, _) => notifications++);

        line.Set(null, values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationAddedNotification>((_, _) => notifications++);

        line.InsertAnnotations(0, values);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, _) => notifications++);

        line.RemoveAnnotations(values);

        Assert.AreEqual(0, notifications);
    }


    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([bom]);
        var values = new List<BillOfMaterials>();

        int notifications = 0;
        parent.GetNotificationSender().Subscribe<AnnotationDeletedNotification>((_, args) =>
        {
            notifications++;
            Assert.AreSame(line, args.Parent);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(bom, args.DeletedAnnotation);
        });

        line.Set(null, values);

        Assert.AreEqual(1, notifications);
    }
}