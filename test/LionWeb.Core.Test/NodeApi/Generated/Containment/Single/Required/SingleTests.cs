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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Single()
    {
        var parent = new LinkTestConcept("od");
        var coord = new LinkTestConcept("myId");
        parent.Containment_1 = coord;
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Containment_1);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new LinkTestConcept("od");
        var coord = new LinkTestConcept("myId");
        parent.SetContainment_1(coord);
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Containment_1);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new LinkTestConcept("od");
        var coord = new LinkTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, coord);
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Containment_1);
    }

    [TestMethod]
    public void Constructor()
    {
        var coord = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("od") { Containment_1 = coord };
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Containment_1);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var coord = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("od") { Containment_1 = coord };
        Assert.AreSame(coord, parent.Get(TestLanguageLanguage.Instance.LinkTestConcept_containment_1));
    }

    [TestMethod]
    public void TryGet()
    {
        var coord = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("od") { Containment_1 = coord };
        Assert.IsTrue(parent.TryGetContainment_1(out var o));
        Assert.AreSame(coord, o);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldCoord = new LinkTestConcept("old");
        var parent = new LinkTestConcept("g") { Containment_1 = oldCoord };
        var coord = new LinkTestConcept("myId");
        parent.Containment_1 = coord;
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Containment_1);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldCoord = new LinkTestConcept("old");
        var parent = new LinkTestConcept("g") { Containment_1 = oldCoord };
        var coord = new LinkTestConcept("myId");
        parent.SetContainment_1(coord);
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Containment_1);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = new LinkTestConcept("old");
        var parent = new LinkTestConcept("g") { Containment_1 = oldCoord };
        var coord = new LinkTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, coord);
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Containment_1);
    }

    #endregion
}