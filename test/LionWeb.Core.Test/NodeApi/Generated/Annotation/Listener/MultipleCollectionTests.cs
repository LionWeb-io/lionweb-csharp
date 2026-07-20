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
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.AddAnnotations(values);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, values);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(0, values);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    [TestMethod]
    public void Insert_Empty_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, values);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(1, values);

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    [TestMethod]
    public void Insert_Two_Between_Reflective()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        child.AddAnnotations([existingA, existingB]);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new INode[] { valueA, valueB };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, new List<INode> { existingA, valueA, valueB, existingB });

        var notifications = observer.AssertOfType<AnnotationAddedNotification>(2);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(valueA, notifications[0].NewAnnotation);
        Assert.AreSame(child, notifications[1].Parent);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(valueB, notifications[1].NewAnnotation);
    }

    #endregion
}
