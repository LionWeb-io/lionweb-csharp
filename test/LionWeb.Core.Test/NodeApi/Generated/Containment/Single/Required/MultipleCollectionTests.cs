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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required;

using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new Coord[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedArray_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedList_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Coord("sA");
        var valueB = new Coord("sB");
        var values = new HashSet<Coord>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void ListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Line("cA");
        var valueB = new Line("cB");
        var values = new List<Line>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedListNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Line("cA");
        var valueB = new Line("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void UntypedArrayNonMatchingType_Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var valueA = new Line("cA");
        var valueB = new Line("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }
}