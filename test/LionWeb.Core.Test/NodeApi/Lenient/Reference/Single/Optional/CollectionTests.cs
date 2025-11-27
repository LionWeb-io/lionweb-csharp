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

namespace LionWeb.Core.Test.NodeApi.Lenient.Reference.Single.Optional;

using System.Collections;

[TestClass]
public class CollectionTests : LenientNodeTestsBase
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new LenientNode[0];
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(parent.Get(OffsetDuplicate_altSource));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new ArrayList();
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(parent.Get(OffsetDuplicate_altSource));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new List<LenientNode>();
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(parent.Get(OffsetDuplicate_altSource));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new HashSet<LenientNode>();
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(parent.Get(OffsetDuplicate_altSource));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new List<string>();
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(parent.Get(OffsetDuplicate_altSource));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new LenientNode[] { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new List<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new List<string>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var values = new HashSet<LenientNode>() { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(OffsetDuplicate_altSource, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newLine("s");
        var values = new LenientNode[] { value };

        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newLine("s");
        var values = new object[] { value };
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newLine("s");
        var values = new ArrayList() { value };
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newLine("s");
        var values = new List<LenientNode>() { value };
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newLine("s");
        var values = new HashSet<LenientNode>() { value };
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newCoord("c");
        var values = new List<LenientNode>() { value };
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newCoord("c");
        var values = new ArrayList() { value };
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var value = newCoord("c");
        var values = new object[] { value };
        parent.Set(OffsetDuplicate_altSource, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_altSource) as IEnumerable<IReadableNode>).Contains(value));
    }

    #endregion
}