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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleRequiredTests
{
    #region SameInOtherInstance

    [TestMethod]
    public void SameInOtherInstance()
    {
        var child = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt");

        target.Containment_1 = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
    }

    [TestMethod]
    public void SameInOtherInstance_detach()
    {
        var child = new LinkTestConcept("myId");
        var orphan = new LinkTestConcept("o");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt") { Containment_1 = orphan };

        target.Containment_1 = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SameInOtherInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt");

        target.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
    }

    [TestMethod]
    public void SameInOtherInstance_detach_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var orphan = new LinkTestConcept("o");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt") { Containment_1 = orphan };

        target.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void Other()
    {
        var child = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt");

        target.Containment_1 = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
    }

    [TestMethod]
    public void Other_detach()
    {
        var child = new LinkTestConcept("myId");
        var orphan = new LinkTestConcept("o");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt") { Containment_1 = orphan };

        target.Containment_1 = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt");

        target.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
    }

    [TestMethod]
    public void Other_detach_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var orphan = new LinkTestConcept("o");
        var source = new LinkTestConcept("src") { Containment_1 = child };
        var target = new LinkTestConcept("tgt") { Containment_1 = orphan };

        target.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Containment_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => source.Containment_1);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SameInSameInstance()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_1 = child };

        parent.Containment_1 = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_1);
    }

    [TestMethod]
    public void SameInSameInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_1 = child };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_1);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_1 = child };

        parent.Containment_0_1 = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_0_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
    }

    [TestMethod]
    public void OtherInSameInstance_detach()
    {
        var child = new LinkTestConcept("myId");
        var orphan = new LinkTestConcept("o");
        var parent = new LinkTestConcept("src") { Containment_1 = child, Containment_0_1 = orphan };

        parent.Containment_0_1 = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_0_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("src") { Containment_1 = child };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_0_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
    }

    [TestMethod]
    public void OtherInSameInstance_detach_Reflective()
    {
        var child = new LinkTestConcept("myId");
        var orphan = new LinkTestConcept("o");
        var parent = new LinkTestConcept("src") { Containment_1 = child, Containment_0_1 = orphan };

        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Containment_0_1);
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion
}
