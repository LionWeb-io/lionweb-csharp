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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var valueA = new BillOfMaterials("sA");
        var valueB = new BillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        observer.AssertNone<AnnotationDeletedNotification>();
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        observer.AssertNone<AnnotationDeletedNotification>();
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(docA, notifications[0].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(docA, notifications[0].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> {  });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { doc });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, docB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, docB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(line, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }
}
