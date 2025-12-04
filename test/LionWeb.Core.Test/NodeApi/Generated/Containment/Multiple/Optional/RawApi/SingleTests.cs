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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    #region add

    [TestMethod]
    public void Add()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("line");
        parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line));
    }

    [TestMethod]
    public void Multiple_Add()
    {
        var parent = new LinkTestConcept("g");
        var line1 = new LinkTestConcept("line1");
        var line2 = new LinkTestConcept("line2");
        parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line1);
        parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line2);
        Assert.AreEqual(2, parent.Containment_0_n.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.AreSame(parent, line2.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line1));
        Assert.IsTrue(parent.Containment_0_n.Contains(line2));
    }

    #endregion

    #region insert

    [TestMethod]
    public void Insert()
    {
        var parent = new LinkTestConcept("g");
        var line1 = new LinkTestConcept("line1");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line1);
        Assert.AreEqual(1, parent.Containment_0_n.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line1));
    }


    [TestMethod]
    public void Multiple_Insert()
    {
        var parent = new LinkTestConcept("g");
        var line1 = new LinkTestConcept("line1");
        var line2 = new LinkTestConcept("line2");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line1);
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 1, line2);
        Assert.AreEqual(2, parent.Containment_0_n.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.AreSame(parent, line2.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line1));
        Assert.IsTrue(parent.Containment_0_n.Contains(line2));
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        Assert.IsFalse(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, -1, line));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        Assert.IsFalse(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 1, line));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { line, circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 1, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, line }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { line, circleA, circleB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 1, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, line, circleB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 2, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB, line }, parent.Containment_0_n.ToList());
    }

    #endregion

    #region remove

    [TestMethod]
    public void Remove()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line);
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(line));
    }

    [TestMethod]
    public void Multiple_Remove()
    {
        var parent = new LinkTestConcept("g");
        var line1 = new LinkTestConcept("line1");
        var line2 = new LinkTestConcept("line2");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line1);
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, line2);
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line1);
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line2);
        Assert.IsEmpty(parent.Containment_0_n);
        Assert.IsNull(line1.GetParent());
        Assert.IsNull(line2.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(line1));
        Assert.IsFalse(parent.Containment_0_n.Contains(line2));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new LinkTestConcept("myC");
        var parent = new LinkTestConcept("cs") { Containment_0_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [line] };
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line));
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [line, circle] };
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circle, line] };
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, line, circleB] };
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Containment_0_n.ToList());
    }

    #endregion
}