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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional;

using Languages.Generated.V2024_1.TestLanguage;
using System.Collections;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[0];
        parent.AddReference_0_n(values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[0];
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Constructor()
    {
        var values = new LinkTestConcept[0];
        var parent = new LinkTestConcept("g") { Reference_0_n = values };
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[0];
        parent.InsertReference_0_n(0, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[0];
        parent.RemoveReference_0_n(values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new ArrayList();
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<LinkTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<LinkTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new HashSet<LinkTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<DataTypeTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new LinkTestConcept("g");
        parent.AddReference_0_n([new LinkTestConcept("myId")]);
        var values = new List<LinkTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.AddReference_0_n(values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void NullArray_Constructor()
    {
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => new LinkTestConcept("g") { Reference_0_n = values });
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.InsertReference_0_n(0, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveReference_0_n(values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new ArrayList() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<LinkTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<LinkTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<DataTypeTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new HashSet<LinkTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.AddReference_0_n(values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Constructor()
    {
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        var parent = new LinkTestConcept("g") { Reference_0_n = values };
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.InsertReference_0_n(0, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        var values = new LinkTestConcept[] { line };
        parent.RemoveReference_0_n(values);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Reference_0_n.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [line] };
        var values = new LinkTestConcept[] { line };
        parent.RemoveReference_0_n(values);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [line, circle] };
        var values = new LinkTestConcept[] { line };
        parent.RemoveReference_0_n(values);
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
        var values = new LinkTestConcept[] { line };
        parent.RemoveReference_0_n(values);
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
        var values = new LinkTestConcept[] { line };
        parent.RemoveReference_0_n(values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Reference_0_n.ToList());
    }

    #endregion

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new object[] { value };
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new ArrayList() { value };
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new List<LinkTestConcept>() { value };
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new List<LinkTestConcept>() { value };
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new HashSet<LinkTestConcept>() { value };
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new DataTypeTestConcept("c");
        var values = new List<DataTypeTestConcept>() { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new DataTypeTestConcept("c");
        var values = new ArrayList() { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new DataTypeTestConcept("c");
        var values = new object[] { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, values));
        Assert.IsTrue(parent.Reference_0_n.Count == 0);
    }

    #endregion
}