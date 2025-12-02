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

namespace LionWeb.Core.Test.Notification.Replicator.Reference;

using Core.Notification;
using Core.Notification.Partition;
using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class TargetTests : ReplicatorTestsBase
{
    [TestMethod]
    public void ReferenceAdded_referencetarget_refers_to_cloned_node()
    {
        var circle = new LinkTestConcept("circle");
        var od = new LinkTestConcept("od");
        var originalPartition = new TestPartition("a") { Contents =  [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(clonedPartition);

        var notificationMapper = new NotificationToNotificationMapper(sharedNodeMap);

        var referenceAddedNotification = new ReferenceAddedNotification(originalPartition,
            TestLanguageLanguage.Instance.LinkTestConcept_reference_1, 0,
            ReferenceTarget.FromNode(circle), new NumericNotificationId("refAddedNotification", 0));

        var notification = notificationMapper.Map(referenceAddedNotification);

        Assert.AreNotSame(circle, ((ReferenceAddedNotification)notification).NewTarget.Target);
    }

    [TestMethod]
    public void ReferenceChanged_referencetarget_refers_to_cloned_node()
    {
        var circle = new LinkTestConcept("circle");
        var line = new LinkTestConcept("line");
        var od = new LinkTestConcept("od") { Reference_1 = circle };
        var originalPartition = new TestPartition("a") { Contents =  [od, circle, line] };

        var clonedPartition = ClonePartition(originalPartition);
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(clonedPartition);

        var notificationMapper = new NotificationToNotificationMapper(sharedNodeMap);
        var referenceChangedNotification = new ReferenceChangedNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_1, 0,
            ReferenceTarget.FromNode(line), ReferenceTarget.FromNode(circle), new NumericNotificationId("refChangedNotification", 0));

        var notification = notificationMapper.Map(referenceChangedNotification);

        Assert.AreNotSame(line, ((ReferenceChangedNotification)notification).NewTarget.Target);
        Assert.AreNotSame(circle, ((ReferenceChangedNotification)notification).OldTarget.Target);
    }

    [TestMethod]
    public void ReferenceDeleted_referencetarget_refers_to_cloned_node()
    {
        var circle = new LinkTestConcept("circle");
        var od = new LinkTestConcept("od") { Reference_1 = circle };
        var originalPartition = new TestPartition("a") { Contents =  [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(clonedPartition);

        var notificationMapper = new NotificationToNotificationMapper(sharedNodeMap);
        var referenceDeletedNotification = new ReferenceDeletedNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_1, 0,
            ReferenceTarget.FromNode(circle),
            new NumericNotificationId("refChangedNotification", 0));

        var notification = notificationMapper.Map(referenceDeletedNotification);

        Assert.AreNotSame(circle, ((ReferenceDeletedNotification)notification).DeletedTarget.Target);
    }
}