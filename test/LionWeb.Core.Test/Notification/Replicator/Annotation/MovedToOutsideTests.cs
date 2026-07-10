// Copyright 2026 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.Test.Notification.Replicator.Annotation;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class MovedToOutsideTests : ReplicatorTestsBase
{
    [TestMethod]
    public void InsertBefore()
    {
        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(annotationA).WithAnnotation(annotationB);
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingAnnotation = new TestAnnotation("floatingAnnotation");
        var freeFloating = new LinkTestConcept("freeFloating").WithAnnotation(floatingAnnotation);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        floatingAnnotation.InsertBefore(annotationA);
        floatingAnnotation.InsertBefore(annotationB);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void InsertAfter()
    {
        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(annotationA).WithAnnotation(annotationB);
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingAnnotation = new TestAnnotation("floatingAnnotation");
        var freeFloating = new LinkTestConcept("freeFloating").WithAnnotation(floatingAnnotation);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        floatingAnnotation.InsertAfter(annotationA);
        floatingAnnotation.InsertAfter(annotationB);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Insert()
    {
        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(annotationA).WithAnnotation(annotationB);
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingAnnotation = new TestAnnotation("floatingAnnotation");
        var freeFloating = new LinkTestConcept("freeFloating").WithAnnotation(floatingAnnotation);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.InsertAnnotations(0, [annotationA, annotationB]);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Add()
    {
        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(annotationA).WithAnnotation(annotationB);
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingAnnotation = new TestAnnotation("floatingAnnotation");
        var freeFloating = new LinkTestConcept("freeFloating").WithAnnotation(floatingAnnotation);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.AddAnnotations([annotationA, annotationB]);

        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Replace()
    {
        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(annotationA).WithAnnotation(annotationB);
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingAnnotation = new TestAnnotation("floatingAnnotation");
        var freeFloating = new LinkTestConcept("freeFloating").WithAnnotation(floatingAnnotation);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        floatingAnnotation.ReplaceWith(annotationA);

        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Set_existing()
    {
        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(annotationA).WithAnnotation(annotationB);
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var floatingAnnotation = new TestAnnotation("floatingAnnotation");
        var freeFloating = new LinkTestConcept("freeFloating").WithAnnotation(floatingAnnotation);

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.Set(null, new List<IReadableNode> { annotationA });
        Assert.AreEqual(1, notificationObserver.Count);

        // TODO: Should be as below, but tracking the old position of moved elements is hard
        // freeFloating.Set(null, new List<IReadableNode> { annotationA, annotationB });
        // Assert.AreEqual(2, notificationObserver.Count);
        
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Set_new()
    {
        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(annotationA).WithAnnotation(annotationB);
        var originalPartition = new TestPartition("partition") { Links = [targetParent] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var freeFloating = new LinkTestConcept("freeFloating");

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        freeFloating.Set(null, new List<IReadableNode> { annotationA });
        Assert.AreEqual(1, notificationObserver.Count);

        // TODO: Should be as below, but tracking the old position of moved elements is hard
        // freeFloating.Set(null, new List<IReadableNode> { annotationA, annotationB });
        // Assert.AreEqual(2, notificationObserver.Count);
        
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationDeletedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
}