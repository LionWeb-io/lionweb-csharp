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
public class CollectionTests
{
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
}