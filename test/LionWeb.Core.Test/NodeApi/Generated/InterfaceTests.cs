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

using Languages.Generated.V2024_1.TestLanguage;
using M2;
using M3;

[TestClass]
public class InterfaceTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void GetClassifier()
    {
        INamedWritable node = new LinkTestConcept("c");
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept, node.GetClassifier());
    }

    [TestMethod]
    public void GetProperty_Reflective()
    {
        INamedWritable node = new LinkTestConcept("c") { Name = "abc" };
        Assert.AreEqual("abc", node.Get(_builtIns.INamed_name));
    }

    [TestMethod]
    public void GetContainment_Reflective()
    {
        LinkTestConcept child = new LinkTestConcept("coord");
        INamedWritable node = new LinkTestConcept("c") { Containment_0_n = [child] };
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child },
            ((IReadOnlyList<LinkTestConcept>)node.Get(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n)).ToList());
    }

    [TestMethod]
    public void SetProperty()
    {
        INamedWritable node = new LinkTestConcept("c");
        node.SetName("abc");
        Assert.AreEqual("abc", ((LinkTestConcept)node).Name);
    }

    [TestMethod]
    public void SetProperty_Reflective()
    {
        INamedWritable node = new LinkTestConcept("c");
        node.Set(_builtIns.INamed_name, "abc");
        Assert.AreEqual("abc", ((LinkTestConcept)node).Name);
    }

    [TestMethod]
    public void AddContainment()
    {
        LinkTestConcept child = new LinkTestConcept("coord");
        LinkTestConcept prevParent = new LinkTestConcept("l") { Containment_0_n = [child] };
        LinkTestConcept node = new LinkTestConcept("c");
        node.AddContainment_0_n([child]);
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, node.Containment_0_n.ToList());
        Assert.IsFalse(prevParent.Containment_0_n.Any());
        Assert.AreSame(node, child.GetParent());
    }

    [TestMethod]
    public void InsertContainment()
    {
        LinkTestConcept child = new LinkTestConcept("coord");
        LinkTestConcept prevParent = new LinkTestConcept("l") { Containment_0_n = [child] };
        LinkTestConcept node = new LinkTestConcept("c");
        node.InsertContainment_0_n(0, [child]);
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, node.Containment_0_n.ToList());
        Assert.IsFalse(prevParent.Containment_0_n.Any());
        Assert.AreSame(node, child.GetParent());
    }

    [TestMethod]
    public void RemoveContainment()
    {
        LinkTestConcept child = new LinkTestConcept("coord");
        LinkTestConcept node = new LinkTestConcept("c") { Containment_0_n = [child] };
        node.RemoveContainment_0_n([child]);
        Assert.IsFalse(node.Containment_0_n.Any());
        Assert.IsNull(child.GetParent());
    }

    [TestMethod]
    public void SetContainment_Reflective()
    {
        LinkTestConcept child = new LinkTestConcept("coord");
        LinkTestConcept prevParent = new LinkTestConcept("l") { Containment_0_n = [child] };
        INamedWritable node = new LinkTestConcept("c");
        node.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, new List<LinkTestConcept> { child });
        CollectionAssert.AreEqual(new List<LinkTestConcept> { child }, ((LinkTestConcept)node).Containment_0_n.ToList());
        Assert.IsFalse(prevParent.Containment_0_n.Any());
        Assert.AreSame((INode)node, child.GetParent());
    }

    [TestMethod]
    public void CollectAllSetFeatures()
    {
        LinkTestConcept child = new LinkTestConcept("coord");
        LinkTestConcept node = new LinkTestConcept("c") { Containment_0_n = [child], Name = "abc" };
        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                _builtIns.INamed_name,
                TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n,
            }, node.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void GetContainmentOf()
    {
        LinkTestConcept child = new LinkTestConcept("coord");
        LinkTestConcept node = new LinkTestConcept("c") { Containment_0_n = [child] };
        Assert.AreSame(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, node.GetContainmentOf(child));
    }
}
