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

using LionWeb.Core.Notification.Partition;
using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.Listener;

[TestClass]
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.AddAnnotations(values);

        observer.AssertNone<AnnotationAddedNotification>();
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, values);

        observer.AssertNone<AnnotationAddedNotification>();
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(0, values);

        observer.AssertNone<AnnotationAddedNotification>();
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        observer.AssertNone<AnnotationDeletedNotification>();
    }


    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");
        line.AddAnnotations([bom]);
        var values = new List<BillOfMaterials>();

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
    }
}
