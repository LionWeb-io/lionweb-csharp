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
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<INode>();
        Assert.IsFalse(parent.AddRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsEmpty(parent.Containment_0_n);
    }

    [TestMethod]
    public void Insert_EmptyList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<INode>();
        Assert.IsFalse(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, values));
        Assert.IsTrue(parent.Containment_0_n.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<INode>();
        Assert.IsFalse(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsTrue(parent.Containment_0_n.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<INode> { null };
        Assert.IsFalse(parent.AddRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsEmpty(parent.Containment_0_n);
    }


    [TestMethod]
    public void Insert_NullList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<INode> { null };
        Assert.IsFalse(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, values));
        Assert.IsEmpty(parent.Containment_0_n);
    }

    [TestMethod]
    public void Remove_NullList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<INode> { null };
        Assert.IsFalse(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsEmpty(parent.Containment_0_n);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleList()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new List<INode> { value };
        Assert.IsTrue(parent.AddRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(value));
    }


    [TestMethod]
    public void Insert_SingleList()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new List<INode> { value };
        Assert.IsTrue(parent.InsertRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0, values));
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Containment_0_n.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleList_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        var values = new List<INode> { line };
        Assert.IsFalse(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Containment_0_n.Contains(line));
    }

    [TestMethod]
    public void SingleList_Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [line] };
        var values = new List<INode> { line };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SingleList_Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [line, circle] };
        var values = new List<INode> { line };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SingleList_Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circle, line] };
        var values = new List<INode> { line };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circle }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SingleList_Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Containment_0_n = [circleA, line, circleB] };
        var values = new List<INode> { line };
        Assert.IsTrue(parent.RemoveRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, values));
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB }, parent.Containment_0_n.ToList());
    }

    #endregion

    #endregion
}