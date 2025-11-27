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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required.GenericApi;

using Languages.Generated.V2024_1.Shapes.M2;

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
        parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }


    #region Insert

    [TestMethod]
    public void Insert_ListMatchingType()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_ListSubtype()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Set()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_SingleEnumerable()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 1, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle, valueA, valueB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circleA, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 1, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, valueA, valueB, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 2, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, valueA, valueB }, parent.Materials.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Remove_ListSubtype()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Set()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Remove_SingleEnumerable()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new MaterialGroup("cs") { Materials = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new MaterialGroup("cs") { Materials = [valueA, valueB, circle] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new MaterialGroup("cs") { Materials = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new MaterialGroup("cs") { Materials = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Materials.ToList());
    }

    #endregion

    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new MaterialGroup("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values);
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
        parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values);
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
        parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values);
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
        parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueA));
        Assert.IsNull(valueB.GetParent());
        Assert.IsTrue(parent.Materials.Contains(valueB));
    }
}