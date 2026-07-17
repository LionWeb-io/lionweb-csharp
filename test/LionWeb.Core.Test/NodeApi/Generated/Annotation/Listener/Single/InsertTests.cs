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
using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.Listener.Single;

[TestClass]
public class InsertTests
{
    [TestMethod]
    public void Empty()
    {
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        var bom = new BillOfMaterials("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(0, [bom]);

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_Before()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(0, [bom]);

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { bom, doc });

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_After()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(1, [bom]);

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var doc = new Documentation("cId");
        var line = new Line("g");
        var parent = new Geometry("parent") { Shapes = [line] };
        line.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { doc, bom });

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(0, [bom]);

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { bom, docA, docB });

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(1, [bom]);

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, bom, docB });

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(2, [bom]);

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, docB, bom });

        var notifications = observer.OfType<AnnotationAddedNotification>(1);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(bom, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var line = new LinkTestConcept("g");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new TestAnnotation("myId");
        var oldParent = new LinkTestConcept("oldParent");
        oldParent.AddAnnotations([new TestAnnotation("doc"), bom]);

        var partition = new TestPartition("partition") { Links = [parent, oldParent] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(2, [bom]);

        var notifications = observer.OfType<AnnotationMovedFromOtherParentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(line, notifications[0].NewParent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(bom, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var line = new LinkTestConcept("g");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [line] };
        line.AddAnnotations([docA, docB]);
        var bom = new TestAnnotation("myId");
        var oldParent = new LinkTestConcept("oldParent");
        oldParent.AddAnnotations([new TestAnnotation("doc"), bom]);

        var partition = new TestPartition("partition") { Links = [parent, oldParent] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, docB, bom });

        var notifications = observer.OfType<AnnotationMovedFromOtherParentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(line, notifications[0].NewParent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(bom, notifications[0].MovedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(2, [bom]);

        var notifications = observer.OfType<AnnotationMovedInSameParentNotification>(1);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(bom, notifications[0].MovedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, docB, bom });

        var notifications = observer.OfType<AnnotationMovedInSameParentNotification>(1);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(line, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(bom, notifications[0].MovedAnnotation);
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.InsertAnnotations(1, [bom]);

        observer.AssertNone<AnnotationMovedInSameParentNotification>();
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

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        line.Set(null, new List<INode> { docA, bom, docB });

        observer.AssertNone<AnnotationMovedInSameParentNotification>();
    }
}
