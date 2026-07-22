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
using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;
using LionWeb.Core.Test.Notification;

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.Listener;

[TestClass]
public class MultipleCollectionRemoveTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        child.AddAnnotations([valueA, valueB]);
        var values = new List<INode>() { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        child.AddAnnotations([valueA, valueB]);
        var values = new List<INode>() { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Empty()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        observer.AssertNone<AnnotationDeletedNotification>();
    }

    [TestMethod]
    public void NonContained()
    {
        var existingA = new TestAnnotation("cA");
        var existingB = new TestAnnotation("cB");
        var child = new LinkTestConcept("cs");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        observer.AssertNone<AnnotationDeletedNotification>();
    }

    [TestMethod]
    public void HalfContained()
    {
        var existingA = new TestAnnotation("cA");
        var existingB = new TestAnnotation("cB");
        var child = new LinkTestConcept("cs");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var values = new INode[] { valueA, existingA };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(existingA, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void HalfContained_Reflective()
    {
        var existingA = new TestAnnotation("cA");
        var existingB = new TestAnnotation("cB");
        var child = new LinkTestConcept("cs");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var values = new INode[] { valueA, existingA };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(existingA, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Only()
    {
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> {  });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(0, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Last()
    {
        var existingA = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var existingA = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, valueA, valueB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Between()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, valueA, valueB, existingB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, valueA, valueB, existingB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, existingB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Mixed()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([valueA, existingA, valueB, existingB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }

    [TestMethod]
    public void Mixed_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([valueA, existingA, valueB, existingB]);
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, existingB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].DeletedAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].DeletedAnnotation);
    }
}
