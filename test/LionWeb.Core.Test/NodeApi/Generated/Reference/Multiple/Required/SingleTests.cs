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

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        parent.AddMaterials([line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, line));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Constructor()
    {
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line] };
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
    }

    [TestMethod]
    public void TryGet()
    {
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line] };
        Assert.IsTrue(parent.TryGetMaterials(out var o));
        Assert.AreSame(line, o.FirstOrDefault());
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        parent.InsertMaterials(0, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertMaterials(-1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertMaterials(1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var line = new Line("myId");
        parent.InsertMaterials(0, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var line = new Line("myId");
        parent.InsertMaterials(1, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circle, line }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertMaterials(0, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circleA, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertMaterials(1, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, line, circleB }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new MaterialGroup("cs") { Materials = [circleA, circleB] };
        var line = new Line("myId");
        parent.InsertMaterials(2, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Materials.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Materials.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new MaterialGroup("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveMaterials([line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Materials.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new MaterialGroup("cs") { Materials = [circle] };
        var line = new Line("myId");
        parent.RemoveMaterials([line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line] };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveMaterials([line]));
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { line }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [line, circle] };
        parent.RemoveMaterials([line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [circle, line] };
        parent.RemoveMaterials([line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Materials.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new MaterialGroup("cs") { Materials = [circleA, line, circleB] };
        parent.RemoveMaterials([line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Materials.ToList());
    }

    #endregion
}