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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling.Multiple.Optional;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SomeEntriesTests
{
    #region SameInOtherInstance

    [TestMethod]
    public void Partial_Add()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var source = new TestPartition("src") { Links = [childA, childB] };
        var target = new TestPartition("tgt");

        target.AddLinks([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, target.Links.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, source.Links.ToList());
    }

    [TestMethod]
    public void Partial_Insert()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var source = new TestPartition("src") { Links = [childA, childB] };
        var target = new TestPartition("tgt");

        target.InsertLinks(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, target.Links.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, source.Links.ToList());
    }

    [TestMethod]
    public void Partial_Reflective()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var source = new TestPartition("src") { Links = [childA, childB] };
        var target = new TestPartition("tgt");

        target.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, target.Links.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, source.Links.ToList());
    }

    #endregion

    #region Other

    [TestMethod]
    public void Partial_Other_Add()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var source = new TestPartition("src") { Links = [childA, childB] };
        var target = new LinkTestConcept("tgt");

        target.AddContainment_1_n([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, target.Containment_1_n.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, source.Links.ToList());
    }

    [TestMethod]
    public void Partial_Other_Insert()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var source = new TestPartition("src") { Links = [childA, childB] };
        var target = new LinkTestConcept("tgt");

        target.InsertContainment_1_n(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, target.Containment_1_n.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, source.Links.ToList());
    }

    [TestMethod]
    public void Partial_Other_Reflective()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var source = new TestPartition("src") { Links = [childA, childB] };
        var target = new LinkTestConcept("tgt");

        target.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<LinkTestConcept> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, target.Containment_1_n.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, source.Links.ToList());
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void Partial_OtherInSameInstance_Add()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        parent.AddContainment_1_n([childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, parent.Containment_1_n.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Partial_OtherInSameInstance_Insert()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        parent.InsertContainment_1_n(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, parent.Containment_1_n.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Partial_OtherInSameInstance_Reflective()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<LinkTestConcept> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, parent.Containment_1_n.ToList());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB }, parent.Containment_0_n.ToList());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void Partial_SameInSameInstance_Add()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        parent.AddContainment_0_n([childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB, childA }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Insert_Start()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        parent.InsertContainment_0_n(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA, childB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Insert_Middle()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        parent.InsertContainment_0_n(1, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childB, childA }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Insert_End()
    {
        var childA = new LinkTestConcept("a");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertContainment_0_n(2, [childA]));

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA, childB }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Reflective()
    {
        var childA = new LinkTestConcept("myId");
        var childB = new LinkTestConcept("b");
        var parent = new LinkTestConcept("src") { Containment_0_n = [childA, childB] };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<LinkTestConcept> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { childA }, parent.Containment_0_n.ToList());
    }

    #endregion
}
