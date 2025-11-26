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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Required;

using Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => parent.AddMaterials(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Constructor()
    {
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => new MaterialGroup("cs") { Materials = values });
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertMaterials(0, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveMaterials(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new ArrayList();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new List<IShape>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new List<Shape>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new HashSet<IShape>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Circle("myId");
        parent.AddMaterials([value]);
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        CollectionAssert.AreEqual(new List<IShape> { value }, parent.Materials.ToList());
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.AddMaterials(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void NullArray_Constructor()
    {
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => new MaterialGroup("cs") { Materials = values });
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertMaterials(0, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new MaterialGroup("cs");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveMaterials(values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new List<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new List<Shape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var values = new HashSet<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.AddMaterials(values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var circle = new Circle("cc");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(value.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { value }, parent.Materials.ToList());
    }

    [TestMethod]
    public void SingleArray_Constructor()
    {
        var value = new Line("s");
        var values = new IShape[] { value };
        var parent = new MaterialGroup("cs") { Materials = values };
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.InsertMaterials(0, values);
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
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveMaterials(values));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line] };
        var values = new IShape[] { line };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveMaterials(values));
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
        parent.RemoveMaterials(values);
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
        parent.RemoveMaterials(values);
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
        parent.RemoveMaterials(values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Materials.ToList());
    }

    #endregion

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new object[] { value };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new ArrayList() { value };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new List<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new List<Shape>() { value };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Line("s");
        var values = new HashSet<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(parent.Materials.Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Coord("c");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new MaterialGroup("cs");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, values));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Materials.Count == 0);
    }

    #endregion
}