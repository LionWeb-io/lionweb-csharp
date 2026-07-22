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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Core.Utilities;
using Languages.Generated.V2024_1.TestLanguage;
using M2;
using M3;

[TestClass]
public class ReflectionTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void GetClassifier()
    {
        var node = new LinkTestConcept("id");
        Assert.AreEqual(TestLanguageLanguage.Instance.LinkTestConcept, node.GetClassifier(), new LanguageEntityIdentityComparer());
    }

    #region inherited

    #region property

    [TestMethod]
    public void GetInheritedProperty()
    {
        var node = new LinkTestConcept("id") { Name = "hi" };
        Assert.AreEqual("hi", node.Get(_builtIns.INamed_name));
    }

    [TestMethod]
    public void GetInheritedProperty_Unset()
    {
        var node = new LinkTestConcept("id");
        Assert.ThrowsExactly<UnsetFeatureException>(() => node.Get(_builtIns.INamed_name));
    }

    [TestMethod]
    public void SetInheritedProperty()
    {
        var node = new LinkTestConcept("id");
        node.Name = "hi";
        Assert.AreEqual("hi", node.Name);
    }

    [TestMethod]
    public void SetInheritedProperty_Setter()
    {
        var node = new LinkTestConcept("id");
        node.SetName("hi");
        Assert.AreEqual("hi", node.Name);
    }

    [TestMethod]
    public void SetInheritedProperty_Constructor()
    {
        var node = new LinkTestConcept("id") { Name = "hi" };
        Assert.AreEqual("hi", node.Name);
    }

    [TestMethod]
    public void SetInheritedProperty_Reflective()
    {
        var node = new LinkTestConcept("id");
        node.Set(_builtIns.INamed_name, "hi");
        Assert.AreEqual("hi", node.Name);
    }

    #endregion

    #region containment

    [TestMethod]
    public void GetInheritedContainment()
    {
        var child = new LinkTestConcept("c");
        var parent = new LinkTestConcept("id") { Containment_0_1 = child };
        Assert.AreSame(child, parent.Get(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1));
    }

    [TestMethod]
    public void GetInheritedContainment_Unset()
    {
        var parent = new LinkTestConcept("id");
        Assert.AreSame(null, parent.Get(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1));
    }

    [TestMethod]
    public void SetInheritedContainment()
    {
        var parent = new LinkTestConcept("id");
        var child = new LinkTestConcept("c");
        parent.Containment_0_1 = child;
        Assert.AreSame(child, parent.Containment_0_1);
    }

    [TestMethod]
    public void SetInheritedContainment_Setter()
    {
        var parent = new LinkTestConcept("id");
        var child = new LinkTestConcept("c");
        parent.SetContainment_0_1(child);
        Assert.AreSame(child, parent.Containment_0_1);
    }

    [TestMethod]
    public void SetInheritedContainment_Constructor()
    {
        var child = new LinkTestConcept("c");
        var parent = new LinkTestConcept("id") { Containment_0_1 = child };
        Assert.AreSame(child, parent.Containment_0_1);
    }

    [TestMethod]
    public void SetInheritedContainment_Reflective()
    {
        var parent = new LinkTestConcept("id");
        var child = new LinkTestConcept("c");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, child);
        Assert.AreSame(child, parent.Containment_0_1);
    }

    [TestMethod]
    public void InheritedContainment_DetachChild()
    {
        var child = new LinkTestConcept("c");
        var parent = new LinkTestConcept("id") { Containment_0_1 = child };
        child.DetachFromParent();
        Assert.IsNull(child.GetParent());
        Assert.IsNull(parent.Containment_0_1);
    }

    [TestMethod]
    public void InheritedContainment_GetContainmentOf()
    {
        var child = new LinkTestConcept("c");
        var parent = new LinkTestConcept("id") { Containment_0_1 = child };
        Assert.AreEqual(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1, parent.GetContainmentOf(child));
    }

    [TestMethod]
    public void InheritedContainment_CollectAllSetFeatures()
    {
        var child = new LinkTestConcept("c");
        var parent = new LinkTestConcept("id") { Containment_0_1 = child };
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}
