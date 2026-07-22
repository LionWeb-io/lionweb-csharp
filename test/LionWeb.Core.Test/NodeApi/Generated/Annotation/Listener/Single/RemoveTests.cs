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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.Listener.Single;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations([annotation]);

        observer.AssertNone<AnnotationDeletedNotification>();
    }

    [TestMethod]
    public void NotContained()
    {
        var existing = new TestAnnotation("myC");
        var child = new LinkTestConcept("cs");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations([annotation]);

        observer.AssertNone<AnnotationDeletedNotification>();
    }

    [TestMethod]
    public void Only()
    {
        var annotation = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotation]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations([annotation]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var annotation = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotation]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> {  });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void First()
    {
        var annotationB = new TestAnnotation("cId");
        var annotationA = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotationA, annotationB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations([annotationA]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotationA, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var annotationB = new TestAnnotation("cId");
        var annotationA = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotationA, annotationB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotationB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotationA, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Last()
    {
        var annotationB = new TestAnnotation("cId");
        var annotationA = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotationB, annotationA]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations([annotationA]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotationA, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var annotationB = new TestAnnotation("cId");
        var annotationA = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotationB, annotationA]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotationB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotationA, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Between()
    {
        var annotationA = new TestAnnotation("cIdA");
        var annotationB = new TestAnnotation("cIdB");
        var annotationC = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotationA, annotationC, annotationB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations([annotationC]);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotationC, notifications[0].DeletedAnnotation);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var annotationA = new TestAnnotation("cIdA");
        var annotationB = new TestAnnotation("cIdB");
        var annotationC = new TestAnnotation("myId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([annotationA, annotationC, annotationB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotationA, annotationB });

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotationC, notifications[0].DeletedAnnotation);
    }
}
