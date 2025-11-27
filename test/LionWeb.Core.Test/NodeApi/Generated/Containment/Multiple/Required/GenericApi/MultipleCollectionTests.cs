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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Required.GenericApi;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class MultipleCollectionTests
{
    [TestMethod]
    public void Array()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Add(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    #region Insert

    [TestMethod]
    public void Insert_ListMatchingType()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_ListSubtype()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Set()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_SingleEnumerable()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 1, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle, valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 1, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, valueA, valueB, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 2, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, valueA, valueB }, parent.Parts.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_ListMatchingType()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Remove_ListSubtype()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Set()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Remove_SingleEnumerable()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueA));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [valueA, valueB, circle] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    #endregion

    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.Add(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.Add(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.Add(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }


    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = new CompositeShape("cs");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape>() { valueA, valueB };
        parent.Add(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Parts.Contains(valueB));
    }
}