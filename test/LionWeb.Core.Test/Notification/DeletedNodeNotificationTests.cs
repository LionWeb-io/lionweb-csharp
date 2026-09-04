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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Languages.Generated.V2023_1.TestLanguage;

[TestClass]
public class DeletedNodeNotificationTests : NotificationTestsBase
{
    [TestMethod]
    public void AnnotationDeleted()
    {
        var parent = new LinkTestConcept("parent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var ann = new TestAnnotation("ann") { Containment = new LinkTestConcept("container") { Containment_0_n = [link0, link1] }, Ref = refTarget };
        ((LinkTestConcept)ann.Containment).Containment_0_1 = link2;
        var notification = new AnnotationDeletedNotification(ann, parent, 0, new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            ann,
            (LinkTestConcept)ann.Containment,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void AnnotationReplaced()
    {
        var parent = new LinkTestConcept("parent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var ann = new TestAnnotation("ann") { Containment = new LinkTestConcept("container") { Containment_0_n = [link0, link1] }, Ref = refTarget };
        ((LinkTestConcept)ann.Containment).Containment_0_1 = link2;
        var replacement = new TestAnnotation("replacement") { Ref = refTarget };
        var notification =
            new AnnotationReplacedNotification(replacement, ann, parent, 0, new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            ann,
            (LinkTestConcept)ann.Containment,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedFromOtherParent()
    {
        var oldParent = new LinkTestConcept("oldParent");
        var newParent = new LinkTestConcept("newParent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var ann = new TestAnnotation("ann") { Containment = new LinkTestConcept("container") { Containment_0_n = [link0, link1] }, Ref = refTarget };
        ((LinkTestConcept)ann.Containment).Containment_0_1 = link2;
        var replacement = new TestAnnotation("replacement") { Ref = refTarget };
        var notification = new AnnotationMovedAndReplacedFromOtherParentNotification(
            newParent,
            1,
            replacement,
            oldParent,
            0,
            ann,
            new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            ann,
            (LinkTestConcept)ann.Containment,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent()
    {
        var parent = new LinkTestConcept("parent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var ann = new TestAnnotation("ann") { Containment = new LinkTestConcept("container") { Containment_0_n = [link0, link1] }, Ref = refTarget };
        ((LinkTestConcept)ann.Containment).Containment_0_1 = link2;
        var replacement = new TestAnnotation("replacement") { Ref = refTarget };
        var notification = new AnnotationMovedAndReplacedInSameParentNotification(
            1,
            replacement,
            parent,
            0,
            +1,
            ann,
            new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            ann,
            (LinkTestConcept)ann.Containment,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void ChildDeleted()
    {
        var parent = new LinkTestConcept("parent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var child = new LinkTestConcept("child") { Containment_0_n = [link0, link1], Containment_0_1 = link2, Reference_0_1 = refTarget };
        var notification = new ChildDeletedNotification(
            child,
            parent,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1,
            0,
            new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            child,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void ChildReplaced()
    {
        var parent = new LinkTestConcept("parent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var child = new LinkTestConcept("child") { Containment_0_n = [link0, link1], Containment_0_1 = link2, Reference_0_1 = refTarget };
        var replacement = new LinkTestConcept("replacement");
        var notification = new ChildReplacedNotification(
            replacement,
            child,
            parent,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1,
            0,
            new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            child,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent()
    {
        var parent = new LinkTestConcept("parent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var child = new LinkTestConcept("child") { Containment_0_n = [link0, link1], Containment_0_1 = link2, Reference_0_1 = refTarget };
        var replacement = new LinkTestConcept("replacement");
        var notification = new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n,
            1,
            replacement,
            parent,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1,
            0,
            child,
            new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            child,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromContainmentInOtherParent()
    {
        var oldParent = new LinkTestConcept("oldParent");
        var newParent = new LinkTestConcept("newParent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var child = new LinkTestConcept("child") { Containment_0_n = [link0, link1], Containment_0_1 = link2, Reference_0_1 = refTarget };
        var replacement = new LinkTestConcept("replacement");
        var notification = new ChildMovedAndReplacedFromContainmentInOtherParentNotification(
            newParent,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n,
            1,
            replacement,
            oldParent,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1,
            0,
            child,
            new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            child,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment()
    {
        var parent = new LinkTestConcept("parent");

        var refTarget = new LinkTestConcept("refTarget");

        var link0 = new LinkTestConcept("link0");
        var link3 = new LinkTestConcept("link3");
        var link1 = new LinkTestConcept("link1") { Containment_0_1 = link3 };
        var link2 = new LinkTestConcept("link2");
        var child = new LinkTestConcept("child") { Containment_0_n = [link0, link1], Containment_0_1 = link2, Reference_0_1 = refTarget };
        var replacement = new LinkTestConcept("replacement");
        var notification = new ChildMovedAndReplacedInSameContainmentNotification(
            1,
            replacement,
            parent,
            TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1,
            child,
            0,
            +1,
            new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode>
        {
            child,
            link0,
            link1,
            link2,
            link3
        }, notification.DeletedNodes.ToList());
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var data = new DataTypeTestConcept("data");
        var linkB = new LinkTestConcept("linkB");
        var linkA = new LinkTestConcept("linkA") { Containment_0_1 = linkB };
        var part = new TestPartition("part") { Data = data, Links = [linkA] };
        var notification = new PartitionDeletedNotification(part, new NumericNotificationId("a", 0));
        CollectionAssert.AreEquivalent(new List<IReadableNode> { part, data, linkA, linkB },
            notification.DeletedNodes.ToList());
    }
}
