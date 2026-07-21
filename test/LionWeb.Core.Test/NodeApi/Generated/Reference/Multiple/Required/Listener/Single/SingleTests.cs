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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.Listener.Single;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.AddReference_1_n([line]);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void Add_Reflective()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { line });

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }
}
