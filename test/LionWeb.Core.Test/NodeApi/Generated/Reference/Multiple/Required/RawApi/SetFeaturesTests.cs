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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.RawApi;

using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;
using System.Collections;

[TestClass]
public class SetFeaturesTests
{
    [TestMethod]
    public void ReferenceMultipleRequired_Add()
    {
        var parent = new LinkTestConcept("g");
        var other = new LinkTestConcept("myId");
        var target = ReferenceTarget.FromNode(other);
        Assert.IsTrue(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, target));
        Assert.IsTrue(parent.TryGetReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            out var result));
        CollectionAssert.AreEqual(new List<ReferenceTarget> { target }, (ICollection?)result);
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Insert()
    {
        var parent = new LinkTestConcept("g");
        var other = new LinkTestConcept("myId");
        var target = ReferenceTarget.FromNode(other);
        Assert.IsTrue(parent.InsertReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, 0,
            target));
        Assert.IsTrue(parent.TryGetReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            out var result));
        CollectionAssert.AreEqual(new List<ReferenceTarget> { target }, (ICollection?)result);
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var other = new LinkTestConcept("myId");
        var target = ReferenceTarget.FromNode(other);
        Assert.IsFalse(parent.SetReferenceRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, target));
        Assert.IsTrue(parent.TryGetReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            out var result));
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Remove()
    {
        var parent = new LinkTestConcept("g");
        var other = new LinkTestConcept("myId");
        var target = ReferenceTarget.FromNode(other);
        Assert.IsTrue(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, target));
        Assert.IsTrue(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            target));
        Assert.IsTrue(parent.TryGetReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            out var result));
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Remove_OtherTarget()
    {
        var parent = new LinkTestConcept("g");
        var other = new LinkTestConcept("myId");
        var target = ReferenceTarget.FromNode(other);
        Assert.IsTrue(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, target));
        Assert.IsTrue(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            ReferenceTarget.FromNode(other)));
        Assert.IsTrue(parent.TryGetReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            out var result));
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void ReferenceMultipleRequired_RemovePart()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("myA");
        var valueB = new LinkTestConcept("myB");
        parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            ReferenceTarget.FromNode(valueA));
        parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            ReferenceTarget.FromNode(valueB));
        parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            ReferenceTarget.FromNode(valueA));
        Assert.IsTrue(parent.TryGetReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            out var result));
        CollectionAssert.AreEqual(new List<ReferenceTarget> { ReferenceTarget.FromNode(valueB) },
            (ICollection?)result);
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Reset_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("myId");
        Assert.IsFalse(parent.SetReferenceRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            ReferenceTarget.FromNode(value)));
        Assert.IsTrue(parent.TryGetReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n,
            out var result));
        Assert.IsEmpty(result);
    }
}