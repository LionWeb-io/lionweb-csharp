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

namespace LionWeb.Core.M2.Lenient.Test;

using System.Collections;

[TestClass]
public class ContainmentTests_Single_Required : LenientNodeTestsBase
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
        parent.Set(OffsetDuplicate_offset, null);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new LenientNode[0];
        parent.Set(OffsetDuplicate_offset, values);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new ArrayList();
        parent.Set(OffsetDuplicate_offset, values);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<LenientNode>();
        parent.Set(OffsetDuplicate_offset, values);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new HashSet<LenientNode>();
        parent.Set(OffsetDuplicate_offset, values);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<string>();
        parent.Set(OffsetDuplicate_offset, values);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new LenientNode[] { null };
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
        var values = new List<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new HashSet<LenientNode>() { null };
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
        var values = new LenientNode[] { value };

        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new object[] { value };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new ArrayList() { value };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new List<LenientNode>() { value };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("s");
        var values = new HashSet<LenientNode>() { value };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newLine("c");
        var values = new List<LenientNode>() { value };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("c");
        var values = new ArrayList() { value };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("c");
        var values = new object[] { value };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { value }, parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, value.GetParent());
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new LenientNode[] { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("sA");
        var valueB = newCoord("sB");
        var values = new HashSet<LenientNode>() { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("cA");
        var valueB = newLine("cB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("cA");
        var valueB = newLine("cB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("cA");
        var valueB = newLine("cB");
        var values = new object[] { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        CollectionAssert.AreEqual(new List<INode> { valueA, valueB },
            parent.Get(OffsetDuplicate_offset) as List<INode>);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
    }

    #endregion
}