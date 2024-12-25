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

namespace LionWeb.Core.Test.NodeApi.Dynamic;

using System.Collections;

[TestClass]
public class ContainmentTests_Single_Required : DynamicNodeTestsBase
{
    #region Single

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var coord = newCoord("myId");
        parent.Set(OffsetDuplicate_offset, coord);
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = newCoord("old");
        var parent = newOffsetDuplicate("g");
        parent.Set(OffsetDuplicate_offset, oldCoord);
        var coord = newCoord("myId");
        parent.Set(OffsetDuplicate_offset, coord);
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Get(OffsetDuplicate_offset));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, null));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new DynamicNode[0];
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new ArrayList();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<DynamicNode>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new HashSet<DynamicNode>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<string>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new DynamicNode[] { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<DynamicNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<DynamicNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new HashSet<DynamicNode>() { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new DynamicNode[] { value };

        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new List<DynamicNode>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new HashSet<DynamicNode>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newLine("c");
        var values = new List<DynamicNode>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new DynamicNode[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new List<DynamicNode>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new HashSet<DynamicNode>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("cA");
        var valueB = newLine("cB");
        var values = new List<DynamicNode>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("cA");
        var valueB = newLine("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("cA");
        var valueB = newLine("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    #endregion
}