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

[TestClass]
public class ReferenceTests_Multiple_Required_GenericApi
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, -1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 1, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circle, line }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circleA, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 1, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, line, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 2, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Materials.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var line = new Line("myId");
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line] };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, [line]));
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { line }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line, circle] };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [circle, line] };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [circleA, line, circleB] };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Materials.ToList());
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new MaterialGroup("cs");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, null));
    }

    [TestMethod]
    public void Null_Insert_Empty()
    {
        var parent = new MaterialGroup("cs");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, null));
    }

    [TestMethod]
    public void Null_Insert_Empty_OutOfBounds()
    {
        var parent = new MaterialGroup("cs");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 1, [null]));
    }

    [TestMethod]
    public void Null_Remove_Empty()
    {
        var parent = new MaterialGroup("cs");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, null));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[0];
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[0];
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[0];
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Add(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }


    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Insert(ShapesLanguage.Instance.MaterialGroup_materials, 0, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        var values = new IShape[] { line };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line] };
        var values = new IShape[] { line };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { line }, parent.Materials.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line, circle] };
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [circle, line] };
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [circleA, line, circleB] };
        var values = new IShape[] { line };
        parent.Remove(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Materials.ToList());
    }

    #endregion

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
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
    public void Multiple_Insert_ListMatchingType()
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
    public void Multiple_Insert_ListSubtype()
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
    public void Multiple_Insert_Set()
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
    public void Multiple_Insert_SingleEnumerable()
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
    public void Multiple_Insert_Empty()
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
    public void Multiple_Insert_One_Before()
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
    public void Multiple_Insert_One_After()
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
    public void Multiple_Insert_Two_Before()
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
    public void Multiple_Insert_Two_Between()
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
    public void Multiple_Insert_Two_After()
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
    public void Multiple_Remove_ListMatchingType()
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
    public void Multiple_Remove_ListSubtype()
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
    public void Multiple_Remove_Set()
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
    public void Multiple_Remove_SingleEnumerable()
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
    public void Multiple_Remove_Empty()
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
    public void Multiple_Remove_Only()
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
    public void Multiple_Remove_NonContained()
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
    public void Multiple_Remove_HalfContained()
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
    public void Multiple_Remove_First()
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
    public void Multiple_Remove_Last()
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
    public void Multiple_Remove_Between()
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
    public void Multiple_Remove_Mixed()
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
    public void MultipleListMatchingType()
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
    public void MultipleListSubtype()
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
    public void MultipleSet()
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
    public void MultipleSingleEnumerable()
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

    #endregion
}