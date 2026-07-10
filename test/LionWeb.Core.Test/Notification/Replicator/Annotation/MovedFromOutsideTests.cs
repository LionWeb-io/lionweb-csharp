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
public class MovedFromOutsideTests : ReplicatorTestsBase
{
    [TestMethod]
    public void InsertBefore()
    {
        var existingAnnotation = new TestAnnotation("existingAnnotation");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(existingAnnotation);
        var originalPartition = new TestPartition("partition") {Links = [targetParent]};

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        targetParent.AddAnnotations([annotationA, annotationB]);
        var cloneTargetParent = clonedPartition.Links.First();
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddAnnotations([annotationA, annotationB]);
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        existingAnnotation.InsertBefore(annotationA);
        existingAnnotation.InsertBefore(annotationB);
        
        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void InsertAfter()
    {
        var existingAnnotation = new TestAnnotation("existingAnnotation");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(existingAnnotation);
        var originalPartition = new TestPartition("partition") {Links = [targetParent]};

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        targetParent.AddAnnotations([annotationA, annotationB]);
        var cloneTargetParent = clonedPartition.Links.First();
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddAnnotations([annotationA, annotationB]);
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        existingAnnotation.InsertAfter(annotationA);
        existingAnnotation.InsertAfter(annotationB);
        
        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void Insert()
    {
        var existingAnnotation = new TestAnnotation("existingAnnotation");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(existingAnnotation);
        var originalPartition = new TestPartition("partition") {Links = [targetParent]};

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        targetParent.AddAnnotations([annotationA, annotationB]);
        var cloneTargetParent = clonedPartition.Links.First();
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddAnnotations([annotationA, annotationB]);
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.InsertAnnotations(0, [annotationA, annotationB]);
        
        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void Add()
    {
        var existingAnnotation = new TestAnnotation("existingAnnotation");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(existingAnnotation);
        var originalPartition = new TestPartition("partition") {Links = [targetParent]};

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        targetParent.AddAnnotations([annotationA, annotationB]);
        var cloneTargetParent = clonedPartition.Links.First();
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddAnnotations([annotationA, annotationB]);
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.AddAnnotations([annotationA, annotationB]);
        
        Assert.AreEqual(2, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationAddedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void Replace()
    {
        var existingAnnotation = new TestAnnotation("existingAnnotation");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(existingAnnotation);
        var originalPartition = new TestPartition("partition") {Links = [targetParent]};

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        targetParent.AddAnnotations([annotationA, annotationB]);
        var cloneTargetParent = clonedPartition.Links.First();
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddAnnotations([annotationA, annotationB]);
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        existingAnnotation.ReplaceWith(annotationA);
        
        Assert.AreEqual(1, notificationObserver.Count);
        CollectionAssert.AllItemsAreInstancesOfType(notificationObserver.Notifications, typeof(AnnotationReplacedNotification));

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void Set_existing()
    {
        var existingAnnotation = new TestAnnotation("existingAnnotation");
        var targetParent = new LinkTestConcept("targetParent").WithAnnotation(existingAnnotation);
        var originalPartition = new TestPartition("partition") {Links = [targetParent]};

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        targetParent.AddAnnotations([annotationA, annotationB]);
        var cloneTargetParent = clonedPartition.Links.First();
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddAnnotations([annotationA, annotationB]);
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.Set(null, new List<IReadableNode>{annotationA, annotationB});
        
        Assert.AreEqual(3, notificationObserver.Count);
        Assert.IsInstanceOfType<AnnotationDeletedNotification>(notificationObserver.Notifications[0]);
        Assert.HasCount(2, notificationObserver.Notifications.OfType<AnnotationAddedNotification>());

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void Set_new()
    {
        var targetParent = new LinkTestConcept("targetParent");
        var originalPartition = new TestPartition("partition") {Links = [targetParent]};

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        var annotationA = new TestAnnotation("annotationA");
        var annotationB = new TestAnnotation("annotationB");
        targetParent.AddAnnotations([annotationA, annotationB]);
        var cloneTargetParent = clonedPartition.Links.First();
        var freeFloating = new LinkTestConcept("freeFloating");
        freeFloating.AddAnnotations([annotationA, annotationB]);
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        targetParent.Set(null, new List<IReadableNode>{annotationA, annotationB});
        
        Assert.AreEqual(2, notificationObserver.Count);
        Assert.HasCount(2, notificationObserver.Notifications.OfType<AnnotationAddedNotification>());

        AssertEquals([originalPartition], [clonedPartition]);
    }
}