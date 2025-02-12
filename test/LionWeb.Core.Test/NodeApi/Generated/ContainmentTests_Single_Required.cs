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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class ContainmentTests_Single_Required
{
    #region Single

    [TestMethod]
    public void Single()
    {
        var parent = new OffsetDuplicate("od");
        var coord = new Coord("myId");
        parent.Offset = coord;
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Single_Setter()
    {
        var parent = new OffsetDuplicate("od");
        var coord = new Coord("myId");
        parent.SetOffset(coord);
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var coord = new Coord("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Single_Constructor()
    {
        var coord = new Coord("myId");
        var parent = new OffsetDuplicate("od") { Offset = coord };
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var coord = new Coord("myId");
        var parent = new OffsetDuplicate("od") { Offset = coord };
        Assert.AreSame(coord, parent.Get(ShapesLanguage.Instance.OffsetDuplicate_offset));
    }

    [TestMethod]
    public void Single_TryGet()
    {
        var coord = new Coord("myId");
        var parent = new OffsetDuplicate("od") { Offset = coord };
        Assert.IsTrue(parent.TryGetOffset(out var o));
        Assert.AreSame(coord, o);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldCoord = new Coord("old");
        var parent = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        parent.Offset = coord;
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldCoord = new Coord("old");
        var parent = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        parent.SetOffset(coord);
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = new Coord("old");
        var parent = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new OffsetDuplicate("od");
        Assert.ThrowsException<InvalidValueException>(() => parent.Offset = null);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new OffsetDuplicate("od");
        Assert.ThrowsException<InvalidValueException>(() => parent.SetOffset(null));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, null));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
    }

    [TestMethod]
    public void Null_Constructor()
    {
        Assert.ThrowsException<InvalidValueException>(
            () => new OffsetDuplicate("od") { Offset = null });
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new OffsetDuplicate("od");
        Assert.IsFalse(parent.TryGetOffset(out var o));
        Assert.IsNull(o);
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new Coord[0];
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new ArrayList();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new HashSet<Coord>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new List<Line>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new Coord[] { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new List<Line>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var values = new HashSet<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Coord("s");
        var values = new Coord[] { value };

        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Coord("s");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Coord("s");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Coord("s");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Coord("s");
        var values = new HashSet<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Line("c");
        var values = new List<Line>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new Coord[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new HashSet<Coord>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Line("cA");
        var valueB = new Line("cB");
        var values = new List<Line>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Line("cA");
        var valueB = new Line("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Line("cA");
        var valueB = new Line("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    #endregion
}