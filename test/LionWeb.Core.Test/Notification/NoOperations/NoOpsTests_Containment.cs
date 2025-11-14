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

using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class NoOpsTests_Containment
{
    [TestMethod]
    public void ChildReplaced_Single_sets_the_same_node()
    {
        var documentation = new Documentation("doc") { Text = "a" };
        var originalPartition = new Geometry("a")
        {
            Documentation = documentation
        };
        
        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);
        
        originalPartition.Documentation = documentation;

        Assert.AreEqual(0, notificationObserver.Count);
    }
}