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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.Listener.Single;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void Empty()
    {
        var source = new LinkTestConcept("src");
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_0_n([line]);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Empty_Reflective()
    {
        var source = new LinkTestConcept("src");
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<INode> {  });

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void NotContained()
    {
        var circle = new LinkTestConcept("myC");
        var source = new LinkTestConcept("src") { Reference_0_n = [circle] };
        var partition = new TestPartition("g") { Links = [source] };
        var line = new LinkTestConcept("myId");

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_0_n([line]);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Only()
    {
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [line] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_0_n([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Only_Reflective()
    {
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [line] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<INode> {  });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [line, circle] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_0_n([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void First_Reflective()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [line, circle] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<INode> { circle });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [circle, line] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_0_n([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Last_Reflective()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [circle, line] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<INode> { circle });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [circleA, line, circleB] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_0_n([line]);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Between_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Reference_0_n = [circleA, line, circleB] };
        var partition = new TestPartition("g") { Links = [source] };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<INode> { circleA, circleB });

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(line), notifications[0].DeletedTarget);
    }
}
