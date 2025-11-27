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

namespace LionWeb.Core.Test.NodeApi.Dynamic.Annotation.MultipleCollection;

using System.Collections;

[TestClass]
public class MultipleCollectionTests : DynamicNodeTestsBase
{
    [TestMethod]
    public void Array()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void UntypedArray_Reflective()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void UntypedList_Reflective()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void ListMatchingType()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new List<INode>() { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new HashSet<INode>() { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new HashSet<INode>() { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new SingleEnumerable<INode> { valueA, valueB };
        parent.AddAnnotations(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable_Reflective()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new SingleEnumerable<INode> { valueA, valueB };
        parent.Set(null, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void ListNonMatchingType_Reflective()
    {
        var parent = newLine("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new List<INode>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void UntypedListNonMatchingType_Reflective()
    {
        var parent = newLine("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void UntypedArrayNonMatchingType_Reflective()
    {
        var parent = newLine("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void SingleList_NotAnnotating()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsException<InvalidValueException>(() => parent.AddAnnotations(values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Reflective()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(null, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Insert()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertAnnotations(0, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Remove()
    {
        var parent = newGeometry("g");
        var value = newDocumentation("sA");
        var values = new List<INode>() { value };
        parent.RemoveAnnotations(values);
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Set(null, values);
        var result = parent.Get(null);
        CollectionAssert.AreEqual(new List<INode>() {valueA, valueB}, (result as IList<INode>).ToList());
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = newLine("g");
        var valueA = newBillOfMaterials("sA");
        var valueB = newBillOfMaterials("sB");
        var values = new INode[] { valueA, valueB };
        parent.Set(null, values);
        var result = parent.Get(null);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = newLine("g");
        var result = parent.Get(null);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }
}