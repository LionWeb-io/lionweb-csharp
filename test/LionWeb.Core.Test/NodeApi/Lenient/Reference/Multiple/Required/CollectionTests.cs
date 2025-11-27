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

namespace LionWeb.Core.Test.NodeApi.Lenient.Reference.Multiple.Required;

using System.Collections;

[TestClass]
public class CollectionTests : LenientNodeTestsBase
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new LenientNode[0];
        parent.Set(MaterialGroup_materials, values);
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new ArrayList();
        parent.Set(MaterialGroup_materials, values);
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new List<LenientNode>();
        parent.Set(MaterialGroup_materials, values);
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new List<LenientNode>();
        parent.Set(MaterialGroup_materials, values);
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new HashSet<LenientNode>();
        parent.Set(MaterialGroup_materials, values);
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new List<string>();
        parent.Set(MaterialGroup_materials, values);
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newCircle("myId");
        parent.Set(MaterialGroup_materials, new List<LenientNode> { value });
        var values = new List<LenientNode>();
        parent.Set(MaterialGroup_materials, values);
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new LenientNode[] { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new ArrayList() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new List<LenientNode>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new List<LenientNode>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new List<string>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var values = new HashSet<LenientNode>() { null };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() =>
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Count() == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newLine("s");
        var values = new LenientNode[] { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var circle = newCircle("cc");
        var parent = newMaterialGroup("cs");
        parent.Set(MaterialGroup_materials, new LenientNode[] { circle });
        var value = newLine("s");
        var values = new LenientNode[] { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(value.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { value },
            (parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).ToList());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newLine("s");
        var values = new object[] { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newLine("s");
        var values = new ArrayList() { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newLine("s");
        var values = new List<LenientNode>() { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newLine("s");
        var values = new List<LenientNode>() { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newLine("s");
        var values = new HashSet<LenientNode>() { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newCoord("c");
        var values = new List<LenientNode>() { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newCoord("c");
        var values = new ArrayList() { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = newMaterialGroup("cs");
        var value = newCoord("c");
        var values = new object[] { value };
        parent.Set(MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(MaterialGroup_materials) as IEnumerable<IReadableNode>).Contains(value));
    }

    #endregion
}