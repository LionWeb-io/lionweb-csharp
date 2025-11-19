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

using Languages.Generated.V2024_1.NullableReferencesTestLang;
using System.Collections;

[TestClass]
public class ReferenceTests_Unresolved_Null
{
    #region Single

    #region Optional

    #region Set

    [TestMethod]
    public void Single_Optional_Assign()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Set()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.SetReference_0_1(target);
        Assert.AreSame(target, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Set_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, target);
        Assert.AreSame(target, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Set_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1,
            new ReferenceTarget(null, "target", target));
        Assert.AreSame(target, parent.Reference_0_1);
    }

    #endregion

    #region Reset

    [TestMethod]
    public void Single_Optional_Assign_new()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        var reset = new LinkTestConcept("reset");
        parent.Reference_0_1 = reset;
        Assert.AreSame(reset, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Reset()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        var reset = new LinkTestConcept("reset");
        parent.SetReference_0_1(reset);
        Assert.AreSame(reset, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Reset_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        var reset = new LinkTestConcept("reset");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, reset);
        Assert.AreSame(reset, parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Reset_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        var reset = new LinkTestConcept("reset");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1,
            new ReferenceTarget(null, "reset", reset));
        Assert.AreSame(reset, parent.Reference_0_1);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Optional_Assign_null()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        parent.Reference_0_1 = null;
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Remove()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        parent.SetReference_0_1(null);
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Remove_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, null);
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Remove_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_0_1 = target;
        Assert.AreSame(target, parent.Reference_0_1);
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsNull(parent.Reference_0_1);
    }

    #endregion

    [TestMethod]
    public void Single_Optional_Get()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_Get_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsNull(parent.Reference_0_1);
    }

    [TestMethod]
    public void Single_Optional_TryGet()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsFalse(parent.TryGetReference_0_1(out var result));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Single_Optional_TryGet_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsFalse(parent.TryGetReference_0_1(out var result));
        Assert.IsNull(result);
    }

    #endregion

    #region Required

    #region Set

    [TestMethod]
    public void Single_Required_Assign()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Single_Required_Set()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.SetReference_1(target);
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Single_Required_Set_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1, target);
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Single_Required_Set_Reflective_Target()
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
    public void Single_Required_Assign_new()
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
    public void Single_Required_Reset()
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
    public void Single_Required_Reset_Reflective()
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
    public void Single_Required_Reset_Reflective_Target()
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
    public void Single_Required_Assign_null()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Reference_1 = null);
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Single_Required_Remove()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Reference_1 = target;
        Assert.AreSame(target, parent.Reference_1);
        Assert.ThrowsExactly<InvalidValueException>(() => parent.SetReference_1(null));
        Assert.AreSame(target, parent.Reference_1);
    }

    [TestMethod]
    public void Single_Required_Remove_Reflective()
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
    public void Single_Required_Remove_Reflective_Target()
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

    [TestMethod]
    public void Single_Required_Get()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsNull(parent.Reference_1);
    }

    [TestMethod]
    public void Single_Required_Get_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Reference_1);
    }

    [TestMethod]
    public void Single_Required_TryGet()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            new ReferenceTarget(null, "target", null));
        Assert.IsTrue(parent.TryGetReference_1(out var result));
        Assert.IsNull(result);
    }


    [TestMethod]
    public void Single_Required_TryGet_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsFalse(parent.TryGetReference_1(out var result));
        Assert.IsNull(result);
    }

    #endregion

    #endregion

    #region Multiple

    #region Optional

    #region Set

    [TestMethod]
    public void Multiple_Optional_Set_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<IReadableNode> { target });
        Assert.AreSame(target, parent.Reference_0_n.First());
    }

    [TestMethod]
    public void Multiple_Optional_Set_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", target) });
        Assert.AreSame(target, parent.Reference_0_n.First());
    }

    [TestMethod]
    public void Multiple_Optional_Add()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.AddReference_0_n(new List<LinkTestConcept> { target });
        Assert.AreSame(target, parent.Reference_0_n.First());
    }

    [TestMethod]
    public void Multiple_Optional_Add_Unresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        var target = new LinkTestConcept("target");
        parent.AddReference_0_n(new List<LinkTestConcept> { target });
        AssertEqual([null, target], parent.Reference_0_n);
    }

    [TestMethod]
    public void Multiple_Optional_Insert()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_0_n = [target] };
        Assert.AreSame(target, parent.Reference_0_n.First());
        var insert = new LinkTestConcept("insert");
        parent.InsertReference_0_n(0, [insert]);
        Assert.AreSame(insert, parent.Reference_0_n.First());
        Assert.AreSame(target, parent.Reference_0_n.Last());
    }

    [TestMethod]
    public void Multiple_Optional_Insert_Unresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        var insert = new LinkTestConcept("insert");
        parent.InsertReference_0_n(0, [insert]);
        AssertEqual([insert, null], parent.Reference_0_n);
    }

    [TestMethod]
    public void Multiple_Optional_Reset_Reflective()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_0_n = [target] };
        Assert.AreSame(target, parent.Reference_0_n.First());

        var reset = new LinkTestConcept("reset");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", reset) });
        Assert.AreSame(reset, parent.Reference_0_n.First());
    }

    [TestMethod]
    public void Multiple_Optional_Reset_Reflective_Unresolved()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_0_n = [target] };
        Assert.AreSame(target, parent.Reference_0_n.First());

        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        AssertEqual([null], parent.Reference_0_n);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Optional_Remove()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_0_n = [target] };
        Assert.AreSame(target, parent.Reference_0_n.First());
        parent.RemoveReference_0_n([target]);
        Assert.IsEmpty(parent.Reference_0_n);
    }

    [TestMethod]
    public void Multiple_Optional_Remove_Mixed()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> { new(null, "target", target), new(null, "unresolved", null) });
        parent.RemoveReference_0_n([target]);
        AssertEqual([null], parent.Reference_0_n);
    }

    [TestMethod]
    public void Multiple_Optional_Remove_Reflective()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_0_n = [target] };
        Assert.AreSame(target, parent.Reference_0_n.First());
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<IReadableNode> { });
        Assert.IsEmpty(parent.Reference_0_n);
    }

    [TestMethod]
    public void Multiple_Optional_Remove_Reflective_Target()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_0_n = [target] };
        Assert.AreSame(target, parent.Reference_0_n.First());
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<ReferenceTarget> { });
        Assert.IsEmpty(parent.Reference_0_n);
    }

    #endregion

    [TestMethod]
    public void Multiple_Optional_Get()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> {new ReferenceTarget(null, "target", null)});
        AssertEqual([null], parent.Reference_0_n);
    }

    [TestMethod]
    public void Multiple_Optional_Get_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsEmpty(parent.Reference_0_n);
    }

    [TestMethod]
    public void Multiple_Optional_TryGet()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            new List<ReferenceTarget> {new ReferenceTarget(null, "target", null)});
        Assert.IsTrue(parent.TryGetReference_0_n(out var result));
        AssertEqual([null], result);
    }

    [TestMethod]
    public void Multiple_Optional_TryGet_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsFalse(parent.TryGetReference_0_n(out var result));
        Assert.IsEmpty(result);
    }

    #endregion
    
    #region Required

    #region Set

    [TestMethod]
    public void Multiple_Required_Set_Reflective()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<IReadableNode> { target });
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Set_Reflective_Target()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", target) });
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Add()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.AddReference_1_n(new List<LinkTestConcept> { target });
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Add_Unresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        var target = new LinkTestConcept("target");
        parent.AddReference_1_n(new List<LinkTestConcept> { target });
        AssertEqual([null, target], parent.Reference_1_n);
    }

    [TestMethod]
    public void Multiple_Required_Insert()
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
    public void Multiple_Required_Insert_Unresolved()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        var insert = new LinkTestConcept("insert");
        parent.InsertReference_1_n(0, [insert]);
        AssertEqual([insert, null], parent.Reference_1_n);
    }

    [TestMethod]
    public void Multiple_Required_Reset_Reflective()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());

        var reset = new LinkTestConcept("reset");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", reset) });
        Assert.AreSame(reset, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Reset_Reflective_Unresolved()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());

        var reset = new LinkTestConcept("reset");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new ReferenceTarget(null, "target", null) });
        AssertEqual([null], parent.Reference_1_n);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Required_Remove()
    {
        var target = new LinkTestConcept("target");
        var second = new LinkTestConcept("second");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target, second] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        parent.RemoveReference_1_n([target]);
        Assert.AreSame(second, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Remove_Last()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveReference_1_n([target]));
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Remove_Mixed()
    {
        var parent = new LinkTestConcept("parent");
        var target = new LinkTestConcept("target");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> { new(null, "target", target), new(null, "unresolved", null) });
        parent.RemoveReference_1_n([target]);
        AssertEqual([null], parent.Reference_1_n);
    }

    [TestMethod]
    public void Multiple_Required_Remove_Reflective()
    {
        var target = new LinkTestConcept("target");
        var second = new LinkTestConcept("second");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target, second] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<IReadableNode> { second });
        Assert.AreSame(second, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Remove_Reflective_Last()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<IReadableNode> { }));
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Remove_Reflective_Target()
    {
        var target = new LinkTestConcept("target");
        var second = new LinkTestConcept("second");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target, second] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<ReferenceTarget> { new (null, "second", second)});
        Assert.AreSame(second, parent.Reference_1_n.First());
    }

    [TestMethod]
    public void Multiple_Required_Remove_Last_Reflective_Target()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent") { Reference_1_n = [target] };
        Assert.AreSame(target, parent.Reference_1_n.First());
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<ReferenceTarget> { }));
        Assert.AreSame(target, parent.Reference_1_n.First());
    }

    #endregion

    [TestMethod]
    public void Multiple_Required_Get()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> {new ReferenceTarget(null, "target", null)});
        AssertEqual([null], parent.Reference_1_n);
    }

    [TestMethod]
    public void Multiple_Required_Get_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Reference_1_n);
    }

    [TestMethod]
    public void Multiple_Required_TryGet()
    {
        var parent = new LinkTestConcept("parent");
        parent.Set(NullableReferencesTestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            new List<ReferenceTarget> {new ReferenceTarget(null, "target", null)});
        Assert.IsTrue(parent.TryGetReference_1_n(out var result));
        AssertEqual([null], result);
    }

    [TestMethod]
    public void Multiple_Required_TryGet_Unassigned()
    {
        var parent = new LinkTestConcept("parent");
        Assert.IsFalse(parent.TryGetReference_1_n(out var result));
        Assert.IsEmpty(result);
    }

    #endregion

    #endregion
    
    private void AssertEqual(IList<object?> expected, IEnumerable<object?> actual) => 
        Assert.IsTrue(expected.SequenceEqual(actual));
}