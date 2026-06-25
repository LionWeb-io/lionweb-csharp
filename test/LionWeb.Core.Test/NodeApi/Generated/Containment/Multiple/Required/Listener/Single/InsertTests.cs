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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.Listener.Single;

using Core.Notification.Partition;
using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class InsertTests
{
    [TestMethod]
    public void Empty()
    {
        var compositeShape = new LinkTestConcept("cs");
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        }));

        compositeShape.InsertContainment_0_n(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circle] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        }));

        compositeShape.InsertContainment_0_n(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_Before_Reflective()
    {
        var circle = new LinkTestConcept("cId");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circle] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<INode> { line, circle });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new LinkTestConcept("cId");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circle] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        }));
        
        compositeShape.InsertContainment_0_n(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void One_After_Reflective()
    {
        var circle = new LinkTestConcept("cId");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circle] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<INode> { circle, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        }));
        
        compositeShape.InsertContainment_0_n(0, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Before_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            notifications++;
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(line, args.NewChild);
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<INode> { line, circleA, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        }));
        
        compositeShape.InsertContainment_0_n(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_Between_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(1, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        }));
        
        compositeShape.InsertContainment_0_n(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void Two_After_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };
        var line = new LinkTestConcept("myId");

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildAddedNotification>(args =>
        {
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(2, args.Index);
            Assert.AreEqual(line, args.NewChild);
            notifications++;
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [compositeShape, line] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedFromOtherContainmentNotification>(args =>
        {
            notifications++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        }));
        
        compositeShape.InsertContainment_0_n(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromOtherParent_Reflective()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [compositeShape, line] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedFromOtherContainmentNotification>(args =>
        {
            notifications++;
            Assert.AreSame(parent, args.OldParent);
            Assert.AreSame(TestLanguageLanguage.Instance.TestPartition_links, args.OldContainment);
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.NewParent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.NewContainment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent()
    {
        var line = new LinkTestConcept("myId");
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [line], Containment_1_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedFromOtherContainmentInSameParentNotification>(args =>
        {
            notifications++;
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, args.NewContainment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        }));
        
        compositeShape.InsertContainment_1_n(1, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameParent_Reflective()
    {
        var line = new LinkTestConcept("myId");
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [line], Containment_1_n = [circleA, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedFromOtherContainmentInSameParentNotification>(args =>
        {
            notifications++;
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, args.NewContainment);
            Assert.AreEqual(1, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<INode> { circleA, line, circleB });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameContainment()
    {
        var line = new LinkTestConcept("line");
        var circleA = new LinkTestConcept("circleA");
        var circleB = new LinkTestConcept("circleB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, line, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedInSameContainmentNotification>(args =>
        {
            notifications++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(+1, args.IndexOffset);
            Assert.AreEqual(line, args.MovedChild);
        }));

        compositeShape.InsertContainment_0_n(2, [line]);

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameContainment_Reflective()
    {
        var line = new LinkTestConcept("myId");
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, line, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedInSameContainmentNotification>(args =>
        {
            notifications++;
            Assert.AreEqual(1, args.OldIndex);
            Assert.AreSame(compositeShape, args.Parent);
            Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, args.Containment);
            Assert.AreEqual(2, args.NewIndex);
            Assert.AreEqual(line, args.MovedChild);
        }));
        
        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<INode> { circleA, circleB, line });

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void FromSameContainment_NoOp()
    {
        var lineA = new LinkTestConcept("myA");
        var lineB = new LinkTestConcept("myB");
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, lineA, lineB, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedInSameContainmentNotification>(_ => notifications++));
        
        compositeShape.InsertContainment_0_n(1, [lineA, lineB]);

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void FromSameContainment_NoOp_Reflective()
    {
        var lineA = new LinkTestConcept("myA");
        var lineB = new LinkTestConcept("myB");
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var compositeShape = new LinkTestConcept("cs") { Containment_0_n = [circleA, lineA, lineB, circleB] };
        var parent = new TestPartition("g") { Links = [compositeShape] };

        int notifications = 0;
        parent.GetNotificationSender()!.ConnectTo(new NotificationChecker<ChildMovedInSameContainmentNotification>(_ => notifications++));

        compositeShape.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n,
            new List<INode> { circleA, lineA, lineB, circleB });

        Assert.AreEqual(0, notifications);
    }
}