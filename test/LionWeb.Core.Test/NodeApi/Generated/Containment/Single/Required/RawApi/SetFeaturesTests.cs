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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SetFeaturesTests
{
    [TestMethod]
    public void ContainmentSingleRequired_Init()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.TryGetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, out var result));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ContainmentSingleRequired_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var coord = new LinkTestConcept("myId");
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, coord));
        Assert.IsTrue(parent.TryGetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, out var result));
        Assert.AreSame(coord, result);
    }

    [TestMethod]
    public void ContainmentSingleRequired_Unset_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var doc = new LinkTestConcept("myId");
        parent.Containment_1 = doc;
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
        Assert.IsTrue(parent.TryGetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, out var result));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = new LinkTestConcept("old");
        var parent = new LinkTestConcept("g") { Containment_1 = oldCoord };
        var coord = new LinkTestConcept("myId");
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, coord));
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.IsTrue(parent.TryGetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, out var result));
        Assert.AreSame(coord, result);
    }
}