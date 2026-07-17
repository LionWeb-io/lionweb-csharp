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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.AddAnnotations(values);

        var notifications = observer.OfType<AnnotationAddedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, values);

        var notifications = observer.OfType<AnnotationAddedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(0, values);

        var notifications = observer.OfType<AnnotationAddedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    [TestMethod]
    public void Insert_Empty_Reflective()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, values);

        var notifications = observer.OfType<AnnotationAddedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(1, values);

        var notifications = observer.OfType<AnnotationAddedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, valueA, valueB, docB });

        var notifications = observer.OfType<AnnotationAddedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    #endregion
}
