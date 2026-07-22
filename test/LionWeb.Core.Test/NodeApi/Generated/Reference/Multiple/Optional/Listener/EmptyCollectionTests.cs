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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.Listener;

[TestClass]
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var source = new LinkTestConcept("src");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.AddReference_0_n(values);

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var source = new LinkTestConcept("src");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);

        observer.AssertEmpty();
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var source = new LinkTestConcept("src");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_0_n(0, values);

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var source = new LinkTestConcept("src");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_0_n(values);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var source = new LinkTestConcept("src");
        var partition = new TestPartition("g") { Links = [source] };
        var circle = new LinkTestConcept("myId");
        source.AddReference_0_n([circle]);
        var values = new List<LinkTestConcept>();

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(circle), notifications[0].DeletedTarget);
    }
}
