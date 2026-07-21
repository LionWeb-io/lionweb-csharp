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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional;

using Languages.Generated.V2024_1.TestLanguage;
using System.Collections;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[0];
        parent.AddLinks(values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[0];
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Constructor()
    {
        var values = new LinkTestConcept[0];
        var parent = new TestPartition("g") { Links = values };
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[0];
        parent.InsertLinks(0, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[0];
        parent.RemoveLinks(values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new ArrayList();
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<LinkTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<LinkTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new HashSet<LinkTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<DataTypeTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new TestPartition("g");
        parent.AddLinks([new LinkTestConcept("myId")]);
        var values = new List<DataTypeTestConcept>();
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.AddLinks(values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void NullArray_Constructor()
    {
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => new TestPartition("g") { Links = values });
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.InsertLinks(0, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveLinks(values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new ArrayList() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<LinkTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<LinkTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<DataTypeTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new HashSet<LinkTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.AddLinks(values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var circle = new LinkTestConcept("cc");
        var parent = new TestPartition("g") { Links = [circle] };
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(circle.GetParent());
        Assert.AreSame(parent, value.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { value }, parent.Links.ToList());
    }

    [TestMethod]
    public void SingleArray_Constructor()
    {
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        var parent = new TestPartition("g") { Links = values };
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.InsertLinks(0, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        var values = new LinkTestConcept[] { line };
        parent.RemoveLinks(values);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Links.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line] };
        var values = new LinkTestConcept[] { line };
        parent.RemoveLinks(values);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Links.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line, circle] };
        var values = new LinkTestConcept[] { line };
        parent.RemoveLinks(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [circle, line] };
        var values = new LinkTestConcept[] { line };
        parent.RemoveLinks(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [circleA, line, circleB] };
        var values = new LinkTestConcept[] { line };
        parent.RemoveLinks(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Links.ToList());
    }

    #endregion

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new object[] { value };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new ArrayList() { value };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new List<LinkTestConcept>() { value };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new List<LinkTestConcept>() { value };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new HashSet<LinkTestConcept>() { value };
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("c");
        var values = new List<DataTypeTestConcept>() { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("c");
        var values = new ArrayList() { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("c");
        var values = new object[] { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    #endregion
}