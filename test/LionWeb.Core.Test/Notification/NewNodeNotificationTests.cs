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
public class NewNodeNotificationTests : NotificationTestsBase
{
    [TestMethod]
    public void AnnotationAdded()
    {
        var parent = new LinkTestConcept("parent");
        var ann = new TestAnnotation("ann");
        var notification = new AnnotationAddedNotification(parent, ann, 0, new NumericNotificationId("a", 0));
        Assert.AreSame(ann, notification.NewNode);
    }
    
    [TestMethod]
    public void AnnotationReplaced()
    {
        var parent = new LinkTestConcept("parent");
        var replaced = new TestAnnotation("replaced");
        var ann = new TestAnnotation("ann");
        var notification = new AnnotationReplacedNotification(ann, replaced, parent,0, new NumericNotificationId("a", 0));
        Assert.AreSame(ann, notification.NewNode);
    }
    
    [TestMethod]
    public void ChildAdded()
    {
        var parent = new LinkTestConcept("parent");
        var child = new LinkTestConcept("child");
        var notification = new ChildAddedNotification(parent, child, TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, 0, new NumericNotificationId("a", 0));
        Assert.AreSame(child, notification.NewNode);
    }
    
    [TestMethod]
    public void ChildReplaced()
    {
        var parent = new LinkTestConcept("parent");
        var replaced = new LinkTestConcept("replaced");
        var child = new LinkTestConcept("child");
        var notification = new ChildReplacedNotification(child, replaced, parent, TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, 0, new NumericNotificationId("a", 0));
        Assert.AreSame(child, notification.NewNode);
    }
    
    [TestMethod]
    public void PartitionAdded()
    {
        var part = new TestPartition("part");
        var notification = new PartitionAddedNotification(part, new NumericNotificationId("a", 0));
        Assert.AreSame(part, notification.NewNode);
    }
}