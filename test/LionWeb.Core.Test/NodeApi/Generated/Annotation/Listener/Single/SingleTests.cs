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
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.AddAnnotations([annotation]);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Add_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotation });

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Add_FromOtherParent()
    {
        var child = new LinkTestConcept("g");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [child] };
        var annotation = new TestAnnotation("myId");
        var oldParent = new LinkTestConcept("oldParent");
        oldParent.AddAnnotations([new TestAnnotation("doc"), annotation]);

        var partition = new TestPartition("partition") { Links = [parent, oldParent] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        child.AddAnnotations([annotation]);

        var notifications = observer.AssertOfType<AnnotationMovedFromOtherParentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].NewParent);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(annotation, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void Add_FromOtherParent_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [child] };
        var annotation = new TestAnnotation("myId");
        var oldParent = new LinkTestConcept("oldParent");
        oldParent.AddAnnotations([new TestAnnotation("doc"), annotation]);

        var partition = new TestPartition("partition") { Links = [parent, oldParent] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotation });

        var notifications = observer.AssertOfType<AnnotationMovedFromOtherParentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].NewParent);
        Assert.AreEqual(0, notifications[0].NewIndex);
        Assert.AreEqual(annotation, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void Add_FromSameParent()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotationA = new TestAnnotation("myId");
        child.AddAnnotations([annotationA, new TestAnnotation("doc")]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.AddAnnotations([annotationA]);

        var notifications = observer.AssertOfType<AnnotationMovedInSameParentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(annotationA, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void Add_FromSameParent_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotationA = new TestAnnotation("myId");
        var annotationB = new TestAnnotation("doc");
        child.AddAnnotations([annotationA, annotationB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotationB, annotationA });

        var notifications = observer.AssertOfType<AnnotationMovedInSameParentNotification>(1);
        Assert.AreEqual(0, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].NewIndex);
        Assert.AreEqual(annotationA, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void Add_FromSameParent_NoOp()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotationA = new TestAnnotation("myId");
        child.AddAnnotations([new TestAnnotation("doc"), annotationA]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.AddAnnotations([annotationA]);

        observer.AssertNone<AnnotationMovedInSameParentNotification>();
    }

    [TestMethod]
    public void Add_FromSameParent_NoOp_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotationA = new TestAnnotation("myId");
        var annotationB = new TestAnnotation("doc");
        child.AddAnnotations([annotationB, annotationA]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotationB, annotationA });

        observer.AssertNone<AnnotationMovedInSameParentNotification>();
    }
}
