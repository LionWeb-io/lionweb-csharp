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
public class InsertTests
{
    [TestMethod]
    public void Empty()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(0, [annotation]);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_Before()
    {
        var existing = new TestAnnotation("cId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(0, [annotation]);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var existing = new TestAnnotation("cId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotation, existing });

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_After()
    {
        var existing = new TestAnnotation("cId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(1, [annotation]);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var existing = new TestAnnotation("cId");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existing, annotation });

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Two_Before()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(0, [annotation]);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { annotation, existingA, existingB });

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Two_Between()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(1, [annotation]);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, annotation, existingB });

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Two_After()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(2, [annotation]);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, existingB, annotation });

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].NewAnnotation);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");
        var oldParent = new LinkTestConcept("oldParent");
        oldParent.AddAnnotations([new TestAnnotation("doc"), annotation]);

        var partition = new TestPartition("partition") { Links = [parent, oldParent] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(2, [annotation]);

        var notifications = observer.AssertOfType<AnnotationMovedFromOtherParentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].NewParent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(annotation, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [child] };
        child.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");
        var oldParent = new LinkTestConcept("oldParent");
        oldParent.AddAnnotations([new TestAnnotation("doc"), annotation]);

        var partition = new TestPartition("partition") { Links = [parent, oldParent] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, existingB, annotation });

        var notifications = observer.AssertOfType<AnnotationMovedFromOtherParentNotification>(1);
        Assert.AreSame(oldParent, notifications[0].OldParent);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].NewParent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(annotation, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");
        child.AddAnnotations([existingA, annotation, existingB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(2, [annotation]);

        var notifications = observer.AssertOfType<AnnotationMovedInSameParentNotification>(1);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(annotation, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");
        child.AddAnnotations([existingA, annotation, existingB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, existingB, annotation });

        var notifications = observer.AssertOfType<AnnotationMovedInSameParentNotification>(1);
        Assert.AreEqual(1, notifications[0].OldIndex);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(2, notifications[0].NewIndex);
        Assert.AreEqual(annotation, notifications[0].MovedAnnotation);
    }

    [TestMethod]
    public void FromSameParent_NoOp()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");
        child.AddAnnotations([existingA, annotation, existingB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(1, [annotation]);

        observer.AssertNone<AnnotationMovedInSameParentNotification>();
    }

    [TestMethod]
    public void FromSameParent_NoOp_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");
        child.AddAnnotations([existingA, annotation, existingB]);

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, annotation, existingB });

        observer.AssertNone<AnnotationMovedInSameParentNotification>();
    }
}
