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
public class NoOpsTests_Property : NotificationTestsBase
{
    [TestMethod]
    public void PropertyAdded_with_null_value()
    {
        var data = new DataTypeTestConcept("c");
        var originalPartition = new TestPartition("a") { Data = data };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        data.StringValue_0_1 = null;

        Assert.AreEqual(0, notificationObserver.Count);
    }

    [TestMethod]
    public void PropertyChanged_to_same_value()
    {
        var data = new DataTypeTestConcept("c") { StringValue_0_1 = "originalText" };
        var originalPartition = new TestPartition("a") { Data = data };

        var notificationObserver = new NotificationObserver();
        originalPartition.GetNotificationSender()!.ConnectTo(notificationObserver);

        data.StringValue_0_1 = "originalText";

        Assert.AreEqual(0, notificationObserver.Count);
    }
}
