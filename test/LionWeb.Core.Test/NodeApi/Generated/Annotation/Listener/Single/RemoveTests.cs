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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.Listener.Single;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations([bom]);

        observer.AssertNone<AnnotationDeletedNotification>();
    }

    [TestMethod]
    public void NotContained()
    {
        var doc = new Documentation("myC");
        var line = new Line("cs");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations([bom]);

        observer.AssertNone<AnnotationDeletedNotification>();
    }

    [TestMethod]
    public void Only()
    {
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations([bom]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> {  });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void First()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom, doc]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations([bom]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([bom, doc]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { doc });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Last()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, bom]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations([bom]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc, bom]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { doc });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.RemoveAnnotations([bom]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, docB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].DeletedAnnotation);
    }
}
