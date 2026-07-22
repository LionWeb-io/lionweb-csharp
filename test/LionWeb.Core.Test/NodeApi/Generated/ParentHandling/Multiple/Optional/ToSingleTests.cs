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
public class ToSingleTests
{
    #region Other

    [TestMethod]
    public void Other()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new LinkTestConcept("tgt");

        target.Containment_0_1 = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_0_1);
        Assert.AreEqual(0, source.Links.Count);
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var source = new TestPartition("src") { Links = [child] };
        var target = new LinkTestConcept("tgt");

        target.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_0_1);
        Assert.AreEqual(0, source.Links.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.Containment_0_1 = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_0_1);
        Assert.AreEqual(0, parent.Containment_0_n.Count);
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_0_n = [child] };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_0_1);
        Assert.AreEqual(0, parent.Containment_0_n.Count);
    }

    #endregion
}
