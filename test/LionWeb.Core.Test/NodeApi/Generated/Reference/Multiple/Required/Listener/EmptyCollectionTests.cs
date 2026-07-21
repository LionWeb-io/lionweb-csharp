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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.Listener;

[TestClass]
public class EmptyCollectionTests
{
    [TestMethod]
    public void EmptyArray()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.AddReference_1_n(values));

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { }));

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.InsertReference_1_n(0, values));

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[0];

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.RemoveReference_1_n(values));

        observer.AssertNone<ReferenceAddedNotification>();
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var value = new LinkTestConcept("myId");
        source.AddReference_1_n([value]);
        var values = new List<LinkTestConcept>();

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, values));

        observer.AssertNone<ReferenceDeletedNotification>();
    }
}
