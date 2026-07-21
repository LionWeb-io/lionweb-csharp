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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Optional.Listener;

[TestClass]
public class NullTests
{
    [TestMethod]
    public void Null()
    {
        var parent = new TestPartition("g");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Data = null;

        observer.AssertEmpty();
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new TestPartition("g");

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, null);

        observer.AssertEmpty();
    }

    [TestMethod]
    public void Existing()
    {
        var oldChild = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldChild };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Data = null;

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_data, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(oldChild, notifications[0].DeletedChild);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldChild = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldChild };

        var observer = new NotificationObserver();
        parent.GetNotificationSender()!.ConnectTo(observer);

        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, null);

        var notifications = observer.AssertOfType<ChildDeletedNotification>(1);
        Assert.AreSame(parent, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_data, notifications[0].Containment);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(oldChild, notifications[0].DeletedChild);
    }
}
