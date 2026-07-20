// Copyright 2025 TRUMPF Laser SE and other contributors
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

using LionWeb.Core.Notification.Partition;
using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;

namespace LionWeb.Core.Test.Notification.Replicator;

[TestClass]
public class MultiPartitionTests : ReplicatorTestsBase
{
    #region Children

    #region ChildMovedFromOtherContainment

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple_SamePartition()
    {
        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n = [moved] };
        var node = new TestPartition("a") { Links =  [origin] };

        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("origin") { Containment_1_n = [new LinkTestConcept("moved")] }] };

        CreatePartitionReplicator(clone, node);

        var observer = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(observer);

        node.AddLinks([moved]);

        var deletions = observer.Notifications.OfType<ChildDeletedNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.DeletedChild)).ToList();
        var moves = observer.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();

        AssertEquals([node], [clone]);
        Assert.IsFalse(deletions.Any());
        CollectionAssert.Contains(moves.Select(it => it.Item2).ToList(), moved);
    }

    [TestMethod]
    [Ignore("Requires implementation of rewriting logic")]
    public void ChildMovedFromOtherContainment_Multiple_DifferentPartition_Add()
    {
        // This change adds a new node to the original partition (node).
        // It should NOT be seen as ChildMovedFromOtherContainment but as ChildAdded, because parent of a moved child is not known.   
        // Therefore, rewriting of notification is needed.   

        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n = [moved] };
        var originPartition = new TestPartition("g") { Links =  [origin] };

        var node = new TestPartition("a") { Links =  [] };

        var clone = new TestPartition("a") { Links =  [] };

        CreatePartitionReplicator(clone, node);

        var originObserver = new NotificationObserver();
        originPartition.GetNotificationSender()!.ConnectTo(originObserver);
        var destinationObserver = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(destinationObserver);

        node.AddLinks([moved]);

        var originMoves = originObserver.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();
        var destinationMoves = destinationObserver.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    [TestMethod]
    [Ignore("Requires implementation of rewriting logic")]
    public void ChildMovedFromOtherContainment_Multiple_DifferentPartition_Insert()
    {
        // This change adds a new node to the original partition (node).
        // It should NOT be seen as ChildMovedFromOtherContainment but as ChildAdded, because parent of a moved child is not known.   
        // Therefore, rewriting of notification is needed.

        var moved = new LinkTestConcept("moved");
        var origin = new LinkTestConcept("origin") { Containment_1_n = [moved] };
        var originPartition = new TestPartition("g") { Links =  [origin] };

        var node = new TestPartition("a") { Links =  [] };

        var clone = new TestPartition("a") { Links =  [] };

        CreatePartitionReplicator(clone, node);

        var originObserver = new NotificationObserver();
        originPartition.GetNotificationSender()!.ConnectTo(originObserver);
        var destinationObserver = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(destinationObserver);

        node.InsertLinks(0, [moved]);

        var originMoves = originObserver.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();
        var destinationMoves = destinationObserver.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single_SamePartition()
    {
        var moved = new LinkTestConcept("moved");
        var node = new TestPartition("a") { Links =  [new LinkTestConcept("l") { Containment_1 = moved }, new LinkTestConcept("ll")] };

        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("l") { Containment_1 = new LinkTestConcept("moved") }, new LinkTestConcept("ll")] };

        CreatePartitionReplicator(clone, node);

        var observer = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(observer);

        node.Links[1].Containment_0_1 = moved;

        var deletions = observer.Notifications.OfType<ChildDeletedNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.DeletedChild)).ToList();
        var moves = observer.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();

        AssertEquals([node], [clone]);
        Assert.IsFalse(deletions.Any());
        CollectionAssert.Contains(moves.Select(it => it.Item2).ToList(), moved);
    }

    [TestMethod]
    [Ignore("Requires implementation of rewriting logic")]
    public void ChildMovedFromOtherContainment_Single_DifferentPartition()
    {
        // This change adds a new node to the original partition (node).
        // It should NOT be seen as ChildMovedFromOtherContainment but as ChildAdded, because parent of a moved child is not known.   
        // Therefore, rewriting of notification is needed.   

        var moved = new LinkTestConcept("moved");
        var originPartition = new TestPartition("g") { Links =  [new LinkTestConcept("l") { Containment_1 = moved }] };

        var node = new TestPartition("a") {Links = [new LinkTestConcept("b")]};

        var clone = new TestPartition("a") {Links = [new LinkTestConcept("b")] };

        CreatePartitionReplicator(clone, node);

        var originObserver = new NotificationObserver();
        originPartition.GetNotificationSender()!.ConnectTo(originObserver);
        var destinationObserver = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(destinationObserver);

        node.Links[0].Containment_0_1 = moved;

        var originMoves = originObserver.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();
        var destinationMoves = destinationObserver.Notifications.OfType<ChildMovedFromOtherContainmentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedChild)).ToList();

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    #endregion

    #endregion

    #region Annotations

    #region AnnotationMovedFromOtherParent

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple_SamePartition()
    {
        var moved = new TestAnnotation("moved");
        var origin = new LinkTestConcept("origin");
        origin.AddAnnotations([moved]);
        var node = new TestPartition("a") { Links =  [origin] };

        var cloneOrigin = new LinkTestConcept("origin");
        cloneOrigin.AddAnnotations([new TestAnnotation("moved")]);
        var clone = new TestPartition("a") { Links =  [cloneOrigin] };

        CreatePartitionReplicator(clone, node);

        var observer = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(observer);

        node.AddAnnotations([moved]);

        var deletions = observer.Notifications.OfType<AnnotationDeletedNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.DeletedAnnotation)).ToList();
        var moves = observer.Notifications.OfType<AnnotationMovedFromOtherParentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedAnnotation)).ToList();

        AssertEquals([node], [clone]);
        Assert.IsFalse(deletions.Any());
        CollectionAssert.Contains(moves.Select(it => it.Item2).ToList(), moved);
    }

    [TestMethod]
    [Ignore("Requires implementation of rewriting logic")]
    public void AnnotationMovedFromOtherParent_Multiple_DifferentPartition_Add()
    {
        // This change adds a new node to the original partition (node).
        // It should NOT be seen as ChildMovedFromOtherContainment but as ChildAdded, because parent of a moved child is not known.   
        // Therefore, rewriting of notification is needed.   

        var moved = new TestAnnotation("moved");
        var origin = new LinkTestConcept("origin");
        origin.AddAnnotations([moved]);
        var originPartition = new TestPartition("g") { Links =  [origin] };

        var node = new TestPartition("a") { };

        var clone = new TestPartition("a") { };

        CreatePartitionReplicator(clone, node);

        var originObserver = new NotificationObserver();
        originPartition.GetNotificationSender()!.ConnectTo(originObserver);
        var destinationObserver = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(destinationObserver);

        node.AddAnnotations([moved]);

        var originMoves = originObserver.Notifications.OfType<AnnotationMovedFromOtherParentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedAnnotation)).ToList();
        var destinationMoves = destinationObserver.Notifications.OfType<AnnotationMovedFromOtherParentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedAnnotation)).ToList();

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    [TestMethod]
    [Ignore("Requires implementation of rewriting logic")]
    public void AnnotationMovedFromOtherParent_Multiple_DifferentPartition_Insert()
    {
        // This change adds a new node to the original partition (node).
        // It should NOT be seen as ChildMovedFromOtherContainment but as ChildAdded, because parent of a moved child is not known.   
        // Therefore, rewriting of notification is needed.   

        var moved = new TestAnnotation("moved");
        var origin = new LinkTestConcept("origin");
        origin.AddAnnotations([moved]);
        var originPartition = new TestPartition("g") { Links =  [origin] };

        var node = new TestPartition("a") { };

        var clone = new TestPartition("a") { };

        CreatePartitionReplicator(clone, node);

        var originObserver = new NotificationObserver();
        originPartition.GetNotificationSender()!.ConnectTo(originObserver);
        var destinationObserver = new NotificationObserver();
        node.GetNotificationSender()!.ConnectTo(destinationObserver);

        node.InsertAnnotations(0, [moved]);

        var originMoves = originObserver.Notifications.OfType<AnnotationMovedFromOtherParentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedAnnotation)).ToList();
        var destinationMoves = destinationObserver.Notifications.OfType<AnnotationMovedFromOtherParentNotification>()
            .Select(e => (e.NotificationId.ToString(), (IReadableNode)e.MovedAnnotation)).ToList();

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    #endregion

    #endregion
}
