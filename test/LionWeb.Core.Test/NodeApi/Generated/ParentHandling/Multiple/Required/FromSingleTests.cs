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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling.Multiple.Required;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class FromSingleTests
{
    #region Other

    [TestMethod]
    public void Other_Add()
    {
        var child = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new TestPartition("tgt");

        target.AddLinks([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Links.ToList());
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
    }

    [TestMethod]
    public void Other_Insert()
    {
        var child = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new TestPartition("tgt");

        target.InsertLinks(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Links.ToList());
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new TestPartition("tgt");

        target.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, target.Links.ToList());
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance_Add()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_1 = child };

        parent.AddContainment_1_n([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_1_n.ToList());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
    }

    [TestMethod]
    public void OtherInSameInstance_Insert()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_1 = child };

        parent.InsertContainment_1_n(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_1_n.ToList());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_1 = child };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1_n, new List<LinkTestConcept> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, parent.Containment_1_n.ToList());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
    }

    #endregion
}
