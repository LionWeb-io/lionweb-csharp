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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget>();
        Assert.IsFalse(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsEmpty(parent.Reference_0_n);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget>();
        Assert.IsFalse(parent.InsertReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 0,
            values));
        Assert.IsEmpty(parent.Reference_0_n);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget>();
        Assert.IsFalse(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsEmpty(parent.Reference_0_n);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget> { null };
        Assert.IsFalse(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsEmpty(parent.Reference_0_n);
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget> { null };
        Assert.IsFalse(parent.InsertReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 0, values));
        Assert.IsEmpty(parent.Reference_0_n);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget> { null };
        Assert.IsFalse(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsEmpty(parent.Reference_0_n);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new List<ReferenceTarget> { ReferenceTarget.FromNode(value) };
        Assert.IsTrue(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new List<ReferenceTarget> { ReferenceTarget.FromNode(value) };
        Assert.IsTrue(
            parent.InsertReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 0, values));
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        var values = new List<ReferenceTarget> { ReferenceTarget.FromNode(line) };
        Assert.IsFalse(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Reference_0_n.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [line] };
        var values = new List<ReferenceTarget> { ReferenceTarget.FromNode(line) };
        Assert.IsTrue(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [line, circle] };
        var values = new List<ReferenceTarget> { ReferenceTarget.FromNode(line) };
        Assert.IsTrue(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circle, line] };
        var values = new List<ReferenceTarget> { ReferenceTarget.FromNode(line) };
        Assert.IsTrue(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circleA, line, circleB] };
        var values = new List<ReferenceTarget> { ReferenceTarget.FromNode(line) };
        Assert.IsTrue(parent.RemoveReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Reference_0_n.ToList());
    }

    #endregion

    #endregion
}