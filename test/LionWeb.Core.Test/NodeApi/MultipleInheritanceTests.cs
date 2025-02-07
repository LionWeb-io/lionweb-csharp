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

namespace LionWeb.Core.Test.NodeApi;

using Languages.Generated.V2023_1.MultiInheritLang;

[TestClass]
public class MultipleInheritanceTests
{
    [TestMethod]
    public void SetCombinedConcept()
    {
        CombinedConcept node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.IfaceContainment = child;

        AssertSame(child, node);
    }

    [TestMethod]
    public void SetCombinedConcept_Reflective()
    {
        CombinedConcept node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.Set(MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment, child);

        AssertSame(child, node);
    }

    [TestMethod]
    public void SetIntermediateConcept()
    {
        IntermediateConcept node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.IfaceContainment = child;

        AssertSame(child, node);
    }

    [TestMethod]
    public void SetIntermediateConcept_Reflective()
    {
        IntermediateConcept node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.Set(MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment, child);

        AssertSame(child, node);
    }

    [TestMethod]
    public void SetAbstractConcept()
    {
        AbstractConcept node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.IfaceContainment = child;

        AssertSame(child, node);
    }

    [TestMethod]
    public void SetAbstractConcept_Reflective()
    {
        AbstractConcept node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.Set(MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment, child);

        AssertSame(child, node);
    }

    [TestMethod]
    public void SetBaseIface()
    {
        BaseIface node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.IfaceContainment = child;

        AssertSame(child, node);
    }

    [TestMethod]
    public void SetBaseIface_Reflective()
    {
        BaseIface node = new CombinedConcept("cc");
        var child = new CombinedConcept("child");

        node.Set(MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment, child);

        AssertSame(child, node);
    }

    private static void AssertSame(BaseIface child, BaseIface node)
    {
        Assert.AreSame<INode>(child, ((CombinedConcept)node).IfaceContainment);
        Assert.AreSame<INode>(child, ((IntermediateConcept)node).IfaceContainment);
        Assert.AreSame<INode>(child, ((AbstractConcept)node).IfaceContainment);
        Assert.AreSame<INode>(child, ((BaseIface)node).IfaceContainment);

        var containment = MultiInheritLangLanguage.Instance.BaseIface_ifaceContainment;
        Assert.AreSame(child, ((CombinedConcept)node).Get(containment));
        Assert.AreSame(child, ((IntermediateConcept)node).Get(containment));
        Assert.AreSame(child, ((AbstractConcept)node).Get(containment));
        Assert.AreSame(child, ((BaseIface)node).Get(containment));
    }
}