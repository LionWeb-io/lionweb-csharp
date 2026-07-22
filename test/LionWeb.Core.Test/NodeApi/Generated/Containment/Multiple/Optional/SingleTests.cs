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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        parent.AddLinks([line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, line));
        Assert.AreSame(null, line.GetParent());
        Assert.IsFalse(parent.Links.Contains(line));
    }

    [TestMethod]
    public void Constructor()
    {
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line] };
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
    }

    [TestMethod]
    public void TryGet()
    {
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line] };
        Assert.IsTrue(parent.TryGetLinks(out var o));
        Assert.AreSame(line, o.FirstOrDefault());
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        parent.InsertLinks(0, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertLinks(-1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Links.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertLinks(1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Links.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [circle] };
        var line = new LinkTestConcept("myId");
        parent.InsertLinks(0, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { line, circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new TestPartition("g") { Links = [circle] };
        var line = new LinkTestConcept("myId");
        parent.InsertLinks(1, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, line }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertLinks(0, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { line, circleA, circleB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertLinks(1, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, line, circleB }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new TestPartition("g") { Links = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.InsertLinks(2, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Links.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB, line }, parent.Links.ToList());
    }

    [TestMethod]
    public void Insert_MoveForward()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n = 
            [
                childA,
                childB,
                childC
            ]
        };
        parent.InsertContainment_0_n(2, [childA]);

        Assert.AreEqual(parent, childA.GetParent());
        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childC.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB, childC, childA }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Insert_MoveBackward()
    {
        var childA = new LinkTestConcept("childA");
        var childB = new LinkTestConcept("childB");
        var childC = new LinkTestConcept("childC");

        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n = 
            [
                childA,
                childB,
                childC
            ]
        };
        parent.InsertContainment_0_n(1, [childC]);

        Assert.AreEqual(parent, childA.GetParent());
        Assert.AreEqual(parent, childB.GetParent());
        Assert.AreEqual(parent, childC.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA, childC, childB }, parent.Containment_0_n.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        parent.RemoveLinks([line]);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Links.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new LinkTestConcept("myC");
        var parent = new TestPartition("cs") { Links = [circle] };
        var line = new LinkTestConcept("myId");
        parent.RemoveLinks([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line] };
        parent.RemoveLinks([line]);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line, circle] };
        parent.RemoveLinks([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [circle, line] };
        parent.RemoveLinks([line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [circleA, line, circleB] };
        parent.RemoveLinks([line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Links.ToList());
    }

    #endregion
}