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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Single.Optional;

using Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new Line[0];
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new ArrayList();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new List<Shape>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new HashSet<Shape>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new Line[] { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new List<Shape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var values = new HashSet<Shape>() { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Line("s");
        var values = new Line[] { value };

        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Line("s");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Line("s");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Line("s");
        var values = new List<Shape>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Line("s");
        var values = new HashSet<Shape>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Coord("c");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new Line[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<Shape>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, values));
        Assert.IsNull(parent.AltSource);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    #endregion
}