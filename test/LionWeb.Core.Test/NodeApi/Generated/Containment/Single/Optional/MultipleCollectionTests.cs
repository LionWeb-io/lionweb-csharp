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
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("sA");
        var valueB = new DataTypeTestConcept("sB");
        var values = new DataTypeTestConcept[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedArray_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("sA");
        var valueB = new DataTypeTestConcept("sB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedList_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("sA");
        var valueB = new DataTypeTestConcept("sB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("sA");
        var valueB = new DataTypeTestConcept("sB");
        var values = new List<DataTypeTestConcept>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new DataTypeTestConcept("sA");
        var valueB = new DataTypeTestConcept("sB");
        var values = new HashSet<DataTypeTestConcept>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void ListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("cA");
        var valueB = new LinkTestConcept("cB");
        var values = new List<LinkTestConcept>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedListNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("cA");
        var valueB = new LinkTestConcept("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedArrayNonMatchingType_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("cA");
        var valueB = new LinkTestConcept("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.TestPartition_data, values));
        Assert.IsNull(parent.Data);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }
}