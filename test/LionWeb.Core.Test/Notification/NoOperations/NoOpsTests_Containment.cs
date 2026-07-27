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

namespace LionWeb.Core.Test.Notification.NoOperationsTests;

using Languages.Generated.V2026_1.TestLanguage;

[TestClass]
public class NoOpsTests_Containment: NotificationTestsBase
{
    [TestMethod]
    public void ChildReplaced_Single_sets_the_same_node()
    {
        var data = new DataTypeTestConcept("doc") { StringValue_0_1 = "a" };
        var originalPartition = new TestPartition("a")
        {
            Data = data
        };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.Data = data;

        Assert.AreEqual(0, notificationObserver.Count);
    }

    [TestMethod]
    public void ChildAdded_Multiple_adds_the_same_list_of_children()
    {
        var a = new LinkTestConcept("a");
        var b = new LinkTestConcept("b");
        var c = new LinkTestConcept("c");
        var d = new LinkTestConcept("d");
        var originalPartition = new TestPartition("geo")
        {
            Links = [a, b, c, d]
        };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.AddLinks([a, b, c, d]);

        Assert.AreEqual(0, notificationObserver.Count);
    }  
    
    [TestMethod]
    public void ChildAdded_Multiple_adds_empty_list()
    {
        var a = new LinkTestConcept("a");
        var b = new LinkTestConcept("b");
        var c = new LinkTestConcept("c");
        var d = new LinkTestConcept("d");
        var originalPartition = new TestPartition("geo")
        {
            Links = [a, b, c, d]
        };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.AddLinks([]);

        Assert.AreEqual(0, notificationObserver.Count);
    }
    
    [TestMethod]
    public void ChildMovedInSameContainment_adds_last_of_the_existing_children()
    {
        var a = new LinkTestConcept("a");
        var b = new LinkTestConcept("b");
        var c = new LinkTestConcept("c");
        var d = new LinkTestConcept("d");
        var originalPartition = new TestPartition("geo") { Links = [a, b, c, d] };
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        originalPartition.AddLinks([d]);

        Assert.AreEqual(0, notificationObserver.Count);
    }
}
