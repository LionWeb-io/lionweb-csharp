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
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.AddReference_1_n(values);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(2);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(source, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept>{valueA,valueB});

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(2);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(source, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_1_n(0, values);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(2);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(source, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.InsertReference_1_n(1, values);

        var notifications = observer.AssertOfType<ReferenceAddedNotification>(2);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].NewTarget);
        Assert.AreSame(source, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[1].Reference);
        Assert.AreEqual(2, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].NewTarget);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var source = new LinkTestConcept("mg");
        var partition = new TestPartition("g") { Links = [source] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new List<LinkTestConcept> { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.RemoveReference_1_n(values));

        observer.AssertNone<ReferenceDeletedNotification>();
    }


    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [valueA, valueB] };
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        Assert.ThrowsExactly<InvalidValueException>(() => source.RemoveReference_1_n(values));

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_1_n(values);

        observer.AssertNone<ReferenceDeletedNotification>();
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new LinkTestConcept("cA");
        var circleB = new LinkTestConcept("cB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var valueA = new LinkTestConcept("sA");
        var values = new LinkTestConcept[] { valueA, circleA };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_1_n(values);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(1);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(circleA), notifications[0].DeletedTarget);
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circle, valueA, valueB] };
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_1_n(values);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(2);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].DeletedTarget);
        Assert.AreSame(source, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].DeletedTarget);
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [circleA, valueA, valueB, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_1_n(values);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(2);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(1, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].DeletedTarget);
        Assert.AreSame(source, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].DeletedTarget);
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var valueA = new LinkTestConcept("sA");
        var valueB = new LinkTestConcept("sB");
        var source = new LinkTestConcept("mg") { Reference_1_n = [valueA, circleA, valueB, circleB] };
        var partition = new TestPartition("g") { Links = [source] };
        var values = new LinkTestConcept[] { valueA, valueB };

        var observer = new NotificationObserver();
        partition.GetNotificationSender()!.ConnectTo(observer);

        source.RemoveReference_1_n(values);

        var notifications = observer.AssertOfType<ReferenceDeletedNotification>(2);
        Assert.AreSame(source, notifications[0].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[0].Reference);
        Assert.AreEqual(0, notifications[0].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueA), notifications[0].DeletedTarget);
        Assert.AreSame(source, notifications[1].Parent);
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, notifications[1].Reference);
        Assert.AreEqual(1, notifications[1].Index);
        Assert.AreEqual(ReferenceTarget.FromNode(valueB), notifications[1].DeletedTarget);
    }

    #endregion
}
