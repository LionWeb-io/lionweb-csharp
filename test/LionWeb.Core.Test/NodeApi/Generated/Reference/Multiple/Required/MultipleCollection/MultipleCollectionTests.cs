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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.MultipleCollection;

using Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.AddMaterials(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Array_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Array_Constructor()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        var parent = new MaterialGroup("cs") { Materials = values };
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void UntypedArray_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void UntypedList_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.AddMaterials(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.AddMaterials(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.AddMaterials(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape>() { valueA, valueB };
        parent.AddMaterials(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void ListNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void UntypedListNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void UntypedArrayNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var parent = new MaterialGroup("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        var result = parent.Get(ShapesLanguage.Instance.MaterialGroup_materials);
        CollectionAssert.AreEqual(new List<INode>() { valueA, valueB }, (result as IEnumerable<INode>).ToList());
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = new MaterialGroup("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        var result = parent.Get(ShapesLanguage.Instance.MaterialGroup_materials);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = new MaterialGroup("g");
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Get(ShapesLanguage.Instance.MaterialGroup_materials));
    }
}