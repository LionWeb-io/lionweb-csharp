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
public class ReferenceTests_Single_Required : LenientNodeTestsBase
{
    #region Single

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var reference = newLine("myId");
        parent.Set(OffsetDuplicate_source, reference);
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Get(OffsetDuplicate_source));
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldReference = newLine("old");
        var parent = newOffsetDuplicate("od");
        parent.Set(OffsetDuplicate_source, oldReference);
        var reference = newLine("myId");
        parent.Set(OffsetDuplicate_source, reference);
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Get(OffsetDuplicate_source));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        parent.Set(OffsetDuplicate_source, null);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_source));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new LenientNode[0];
        parent.Set(OffsetDuplicate_source, values);
        Assert.ThrowsException<UnsetFeatureException>(() =>
            parent.Get(OffsetDuplicate_source));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new ArrayList();
        parent.Set(OffsetDuplicate_source, values);
        Assert.ThrowsException<UnsetFeatureException>(() =>
            parent.Get(OffsetDuplicate_source));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<LenientNode>();
        parent.Set(OffsetDuplicate_source, values);
        Assert.ThrowsException<UnsetFeatureException>(() =>
            parent.Get(OffsetDuplicate_source));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new HashSet<LenientNode>();
        parent.Set(OffsetDuplicate_source, values);
        Assert.ThrowsException<UnsetFeatureException>(() =>
            parent.Get(OffsetDuplicate_source));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<string>();
        parent.Set(OffsetDuplicate_source, values);
        Assert.ThrowsException<UnsetFeatureException>(() =>
            parent.Get(OffsetDuplicate_source));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new LenientNode[] { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_source, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_source, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_source, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new List<string>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_source, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var values = new HashSet<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_source, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newLine("s");
        var values = new LenientNode[] { value };

        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newLine("s");
        var values = new object[] { value };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newLine("s");
        var values = new ArrayList() { value };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newLine("s");
        var values = new List<LenientNode>() { value };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newLine("s");
        var values = new HashSet<LenientNode>() { value };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("c");
        var values = new List<LenientNode>() { value };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("c");
        var values = new ArrayList() { value };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = newCoord("c");
        var values = new object[] { value };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(value));
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new LenientNode[] { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new HashSet<LenientNode>() { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new object[] { valueA, valueB };
        parent.Set(OffsetDuplicate_source, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_source) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    #endregion

    #region metamodelViolation

    [TestMethod]
    public void String_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = "a";
        parent.Set(OffsetDuplicate_source, value);
        Assert.AreEqual("a", parent.Get(OffsetDuplicate_source));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var value = -10;
        parent.Set(OffsetDuplicate_source, value);
        Assert.AreEqual(-10, parent.Get(OffsetDuplicate_source));
    }

    #endregion
}