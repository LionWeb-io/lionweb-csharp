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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling.Multiple.Optional;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SomeEntriesTests
{
    #region SameInOtherInstance

    [TestMethod]
    public void Partial_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new Geometry("tgt");

        target.AddShapes([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void Partial_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void Partial_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    #endregion

    #region Other

    [TestMethod]
    public void Partial_Other_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.AddParts([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void Partial_Other_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.InsertParts(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void Partial_Other_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void Partial_OtherInSameInstance_Add()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.AddAltGroups([childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.AltGroups.ToList());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void Partial_OtherInSameInstance_Insert()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.InsertAltGroups(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.AltGroups.ToList());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void Partial_OtherInSameInstance_Reflective()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_altGroups, new List<MaterialGroup> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.AltGroups.ToList());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB }, parent.Groups.ToList());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void Partial_SameInSameInstance_Add()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.AddGroups([childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB, childA }, parent.Groups.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Insert_Start()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.InsertGroups(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA, childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Insert_Middle()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.InsertGroups(1, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB, childA }, parent.Groups.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Insert_End()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertGroups(2, [childA]));

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA, childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void Partial_SameInSameInstance_Reflective()
    {
        var childA = new MaterialGroup("myId");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_groups, new List<MaterialGroup> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.Groups.ToList());
    }

    #endregion
}