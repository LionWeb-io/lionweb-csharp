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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Unresolved.Throw;

using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class MultipleRequiredTests
{
    #region Set

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<IReadableNode> { target });
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Set_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", target) });
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Add()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.AddReference_1_n(new List<LinkTestConcept> { target });
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Add_Unresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        var target = new LinkTestConcept("target");
        parent.AddReference_1_n(new List<LinkTestConcept> { target });
        Assert.ThrowsExactly<UnresolvedReferenceException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Insert()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        var insert = new LinkTestConcept("insert");
        parent.InsertReference_1_n(0, [insert]);
        Assert.AreSame(insert, parent.Reference_1_n.First());
        Assert.AreSame(target, parent.Reference_1_n.Last());
    }

    [TestMethod]
    public void Insert_Unresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        var insert = new LinkTestConcept("insert");
        parent.InsertReference_1_n(0, [insert]);
        Assert.ThrowsExactly<UnresolvedReferenceException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Reset_Reflective()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());

        var reset = new LinkTestConcept("reset");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", reset) });
        Assert.AreSame(reset, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Reset_Reflective_Unresolved()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());

        var reset = new LinkTestConcept("reset");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        Assert.ThrowsExactly<UnresolvedReferenceException>(() => parent.Reference_1_n);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove()
    {
        var target = new LinkTestConcept("target");
        var second = new LinkTestConcept("second");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target, second] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        parent.RemoveReference_1_n([target]);
        Assert.AreSame(second, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveReference_1_n([target]));
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new(null, "target", target), new(null, "unresolved", null) });
        parent.RemoveReference_1_n([target]);
        Assert.ThrowsExactly<UnresolvedReferenceException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Remove_Reflective()
    {
        var target = new LinkTestConcept("target");
        var second = new LinkTestConcept("second");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target, second] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<IReadableNode> { second });
        Assert.AreSame(second, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Remove_Reflective_Last()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<IReadableNode> { }));
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Remove_Reflective_Target()
    {
        var target = new LinkTestConcept("target");
        var second = new LinkTestConcept("second");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target, second] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<ReferenceTarget> { new (null, "second", second)});
        Assert.AreSame(second, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Remove_Last_Reflective_Target()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<ReferenceTarget> { }));
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    #endregion

    #region Get

    [TestMethod]
    public void Get_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Get_OneUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        Assert.ThrowsExactly<UnresolvedReferenceException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Get_OneResolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolved = new LinkTestConcept("resolved");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { ReferenceTarget.FromNode(resolved) });
        AssertEqual([resolved], parent.Reference_1_n);
    }

    [TestMethod]
    public void Get_AllUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget>
            {
                new ReferenceTarget(null, "targetA", null), new ReferenceTarget(null, "targetB", null),
            });
        Assert.ThrowsExactly<UnresolvedReferenceException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Get_PartialUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolved = new LinkTestConcept("resolved");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget>
            {
                new ReferenceTarget(null, "target", null), ReferenceTarget.FromNode(resolved),
            });
        Assert.ThrowsExactly<UnresolvedReferenceException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Get_AllResolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolvedA = new LinkTestConcept("resolvedA");
        var resolvedB = new LinkTestConcept("resolvedB");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { ReferenceTarget.FromNode(resolvedA), ReferenceTarget.FromNode(resolvedB) });
        AssertEqual([resolvedA, resolvedB], parent.Reference_1_n);
    }

    #endregion

    #region TryGet

    [TestMethod]
    public void TryGet_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsFalse(parent.TryGetReference_1_n(out var result));
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void TryGet_OneUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        Assert.IsFalse(parent.TryGetReference_1_n(out var result));
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void TryGet_OneResolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolved = new LinkTestConcept("resolved");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { ReferenceTarget.FromNode(resolved) });
        Assert.IsTrue(parent.TryGetReference_1_n(out var result));
        AssertEqual([resolved], result);
    }

    [TestMethod]
    public void TryGet_AllUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget>
            {
                new ReferenceTarget(null, "targetA", null), new ReferenceTarget(null, "targetB", null),
            });
        Assert.IsFalse(parent.TryGetReference_1_n(out var result));
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void TryGet_PartialUnresolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolved = new LinkTestConcept("resolved");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget>
            {
                new ReferenceTarget(null, "target", null), ReferenceTarget.FromNode(resolved),
            });
        Assert.IsFalse(parent.TryGetReference_1_n(out var result));
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void TryGet_AllResolved()
    {
        var parent = new LinkTestConcept("parent");
        var resolvedA = new LinkTestConcept("resolvedA");
        var resolvedB = new LinkTestConcept("resolvedB");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { ReferenceTarget.FromNode(resolvedA), ReferenceTarget.FromNode(resolvedB) });
        Assert.IsTrue(parent.TryGetReference_1_n(out var result));
        AssertEqual([resolvedA, resolvedB], result);
    }

    #endregion

    private void AssertEqual(IList<object?> expected, IEnumerable<object?> actual) =>
        Assert.IsTrue(expected.SequenceEqual(actual));
}