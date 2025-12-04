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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new LinkTestConcept("cs");
        var line = new LinkTestConcept("myId");
        parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, line);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(line));
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new LinkTestConcept("cs");
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0, line);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new LinkTestConcept("cs");
        var line = new LinkTestConcept("myId");
        Assert.IsFalse(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, -1, line));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new LinkTestConcept("cs");
        var line = new LinkTestConcept("myId");
        Assert.IsFalse(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, line));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(line));
        CollectionAssert.AreEqual(new List<INode> { line, circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(line));
        CollectionAssert.AreEqual(new List<INode> { circle, line }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 0, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(line));
        CollectionAssert.AreEqual(new List<INode> { line, circleA, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 1, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(line));
        CollectionAssert.AreEqual(new List<INode> { circleA, line, circleB }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, 2, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Containment_1_n.Contains(line));
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB, line }, parent.Containment_1_n.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new LinkTestConcept("cs");
        var line = new LinkTestConcept("myId");
        Assert.IsFalse(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, line));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new LinkTestConcept("myC");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [line] };
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, line));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1_n.Contains(line));
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [line, circle] };
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circle, line] };
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, line);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle }, parent.Containment_1_n.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("cs") { Containment_1_n = [circleA, line, circleB] };
        parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, line);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB }, parent.Containment_1_n.ToList());
    }

    #endregion
}