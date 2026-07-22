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
public class SingleEntryTests
{
    #region SameInOtherInstance

    [TestMethod]
    public void SameInOtherInstance_Add()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new TestPartition("tgt");

        target.AddLinks([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Links.ToList());
        Assert.AreEqual(0, source.Links.Count);
    }

    [TestMethod]
    public void SameInOtherInstance_Insert()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new TestPartition("tgt");

        target.InsertLinks(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Links.ToList());
        Assert.AreEqual(0, source.Links.Count);
    }

    [TestMethod]
    public void SameInOtherInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new TestPartition("tgt");

        target.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Links.ToList());
        Assert.AreEqual(0, source.Links.Count);
    }

    #endregion

    #region Other

    [TestMethod]
    public void Other_Add()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new LinkTestConcept("tgt");

        target.AddContainment_1_n([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Containment_1_n.ToList());
        Assert.AreEqual(0, source.Links.Count);
    }

    [TestMethod]
    public void Other_Insert()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new LinkTestConcept("tgt");

        target.InsertContainment_1_n(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Containment_1_n.ToList());
        Assert.AreEqual(0, source.Links.Count);
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new LinkTestConcept("tgt");

        target.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<LinkTestConcept> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Containment_1_n.ToList());
        Assert.AreEqual(0, source.Links.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance_Add()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.AddContainment_1_n([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_1_n.ToList());
        Assert.AreEqual(0, parent.Containment_0_n.Count);
    }

    [TestMethod]
    public void OtherInSameInstance_Insert()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.InsertContainment_1_n(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_1_n.ToList());
        Assert.AreEqual(0, parent.Containment_0_n.Count);
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<LinkTestConcept> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_1_n.ToList());
        Assert.AreEqual(0, parent.Containment_0_n.Count);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SameInSameInstance_Add()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.AddContainment_0_n([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Insert_Start()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.InsertContainment_0_n(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Insert_End()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertContainment_0_n(1, [child]));

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_0_n.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<LinkTestConcept> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_0_n.ToList());
    }

    #endregion
}
