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
public class SingleEntryTests
{
    #region SameInOtherInstance

    [TestMethod]
    public void SameInOtherInstance_Add()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new Geometry("tgt");

        target.AddShapes([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void SameInOtherInstance_Insert()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void SameInOtherInstance_Reflective()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    #endregion

    #region Other

    [TestMethod]
    public void Other_Add()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.AddParts([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void Other_Insert()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.InsertParts(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance_Add()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.AddAltGroups([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.AreEqual(0, parent.Groups.Count);
    }

    [TestMethod]
    public void OtherInSameInstance_Insert()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.InsertAltGroups(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.AreEqual(0, parent.Groups.Count);
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_altGroups, new List<MaterialGroup> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.AreEqual(0, parent.Groups.Count);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SameInSameInstance_Add()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.AddGroups([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Insert_Start()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.InsertGroups(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Insert_End()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertGroups(1, [child]));

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Reflective()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_groups, new List<MaterialGroup> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    #endregion
}