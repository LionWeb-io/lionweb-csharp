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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Unresolved.Null;

using Languages.Generated.V2024_1.NullableReferencesTestLang;

[TestClass]
public class SingleRequiredTests
{
    #region Set

    [TestMethod]
    public void Assign()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Set()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.SetReference_1(target);
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1, target);
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Set_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            new ReferenceTarget(null, "target", target));
        Assert.AreSame(target, parent.Reference_1);
    }

    #endregion

    #region Reset

    [TestMethod]
    public void Assign_new()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        var reset = new LinkTestConcept("reset");
        parent.Reference_1 = reset;
        Assert.AreSame(reset, parent.Reference_1);
    }

    [TestMethod]
    public void Reset()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        var reset = new LinkTestConcept("reset");
        parent.SetReference_1(reset);
        Assert.AreSame(reset, parent.Reference_1);
    }

    [TestMethod]
    public void Reset_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        var reset = new LinkTestConcept("reset");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1, reset);
        Assert.AreSame(reset, parent.Reference_1);
    }

    [TestMethod]
    public void Reset_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        var reset = new LinkTestConcept("reset");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            new ReferenceTarget(null, "reset", reset));
        Assert.AreSame(reset, parent.Reference_1);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Assign_null()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Reference_1 = null);
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Remove()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        Assert.ThrowsExactly<InvalidValueException>(() => parent.SetReference_1(null));
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Remove_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1, null));
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Remove_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsNull(parent.Reference_1);
    }

    #endregion

    #region Get

    [TestMethod]
    public void Get_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Reference_1);
    }

    [TestMethod]
    public void Get_OneUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsNull(parent.Reference_1);
    }

    [TestMethod]
    public void Get_OneResolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolved = new LinkTestConcept("resolved");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            ReferenceTarget.FromNode(resolved));
        Assert.AreSame(resolved, parent.Reference_1);
    }

    #endregion

    #region TryGet

    [TestMethod]
    public void TryGet_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsFalse(parent.TryGetReference_1(out var result));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryGet_OneUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsTrue(parent.TryGetReference_1(out var result));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryGet_OneResolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolved = new LinkTestConcept("resolved");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            ReferenceTarget.FromNode(resolved));
        Assert.IsTrue(parent.TryGetReference_1(out var result));
        Assert.AreSame(resolved, result);
    }

    #endregion
}