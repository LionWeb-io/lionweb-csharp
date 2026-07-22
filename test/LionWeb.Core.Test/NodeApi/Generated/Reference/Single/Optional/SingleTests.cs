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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Single.Optional;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    #region Single

    [TestMethod]
    public void Single()
    {
        var parent = new LinkTestConcept("g");
        var reference = new LinkTestConcept("myId");
        parent.Reference_0_1 = reference;
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Setter()
    {
        var parent = new LinkTestConcept("g");
        var reference = new LinkTestConcept("myId");
        parent.SetReference_0_1(reference);
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var reference = new LinkTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, reference);
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Constructor()
    {
        var reference = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_1 = reference };
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Reference_0_1);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var reference = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_1 = reference };
        Assert.AreSame(reference, parent.Get(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1));
    }

    [TestMethod]
    public void Single_TryGet()
    {
        var reference = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_1 = reference };
        Assert.IsTrue(parent.TryGetReference_0_1(out var o));
        Assert.AreSame(reference, o);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldReference = new LinkTestConcept("old");
        var parent = new LinkTestConcept("g") { Reference_0_1 = oldReference };
        var reference = new LinkTestConcept("myId");
        parent.Reference_0_1 = reference;
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Reference_0_1);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldReference = new LinkTestConcept("old");
        var parent = new LinkTestConcept("g") { Reference_0_1 = oldReference };
        var reference = new LinkTestConcept("myId");
        parent.SetReference_0_1(reference);
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Reference_0_1);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldReference = new LinkTestConcept("old");
        var parent = new LinkTestConcept("g") { Reference_0_1 = oldReference };
        var reference = new LinkTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, reference);
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Reference_0_1);
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new LinkTestConcept("g");
        parent.Reference_0_1 = null;
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new LinkTestConcept("g");
        parent.SetReference_0_1(null);
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new LinkTestConcept("g");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, null);
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Null_Constructor()
    {
        var parent = new LinkTestConcept("g") { Reference_0_1 = null };
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new LinkTestConcept("g") { Reference_0_1 = null };
        Assert.IsFalse(parent.TryGetReference_0_1(out var o));
        Assert.IsNull(o);
    }

    #endregion
}