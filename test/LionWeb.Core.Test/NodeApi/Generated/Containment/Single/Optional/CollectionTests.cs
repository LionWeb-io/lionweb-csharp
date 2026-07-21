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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Optional;

using Languages.Generated.V2024_1.TestLanguage;
using System.Collections;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new DataTypeTestConcept[0];
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new ArrayList();
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<DataTypeTestConcept>();
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new HashSet<DataTypeTestConcept>();
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<LinkTestConcept>();
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new DataTypeTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new ArrayList() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<DataTypeTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new List<LinkTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new TestPartition("g");
        var values = new HashSet<DataTypeTestConcept>() { null };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("s");
        var values = new DataTypeTestConcept[] { value };

        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("s");
        var values = new object[] { value };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("s");
        var values = new ArrayList() { value };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("s");
        var values = new List<DataTypeTestConcept>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new DataTypeTestConcept("s");
        var values = new HashSet<DataTypeTestConcept>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("c");
        var values = new List<LinkTestConcept>() { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("c");
        var values = new ArrayList() { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("c");
        var values = new object[] { value };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
    }

    #endregion
}