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
public class InsertTests
{
    [TestMethod]
    public void One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circle] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_1_n(0, [line]);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var circle = new LinkTestConcept("cId");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circle] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { line, circle });

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new LinkTestConcept("cId");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circle] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_1_n(1, [line]);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var circle = new LinkTestConcept("cId");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circle] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { circle, line });

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_1_n(0, [line]);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { line, circleA, circleB });

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_1_n(1, [line]);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { circleA, line, circleB });

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_1_n(2, [line]);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { circleA, circleB, line });

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(2, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].NewTarget);
    }
}
