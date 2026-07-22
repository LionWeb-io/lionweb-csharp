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
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.AddAnnotations(values);

        observer.AssertNone<AnnotationAddedNotification>();
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, values);

        observer.AssertNone<AnnotationAddedNotification>();
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.InsertAnnotations(0, values);

        observer.AssertNone<AnnotationAddedNotification>();
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var values = new INode[0];

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.RemoveAnnotations(values);

        observer.AssertNone<AnnotationDeletedNotification>();
    }


    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var child = new LinkTestConcept("g");
        var parent = new TestPartition("parent") { Links = [child] };
        var annotation = new TestAnnotation("myId");
        child.AddAnnotations([annotation]);
        var values = new List<TestAnnotation>();

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        child.Set(null, values);

        var notifications = observer.AssertOfType<AnnotationDeletedNotification>(1);
        Assert.AreSame(child, notifications[0].Parent);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(annotation, notifications[0].DeletedAnnotation);
    }
}
