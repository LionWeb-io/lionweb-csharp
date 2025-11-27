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
public class FromSingleTests
{
    #region Other

    [TestMethod]
    public void Other_Add()
    {
        var child = new Line("myId");
        var source = new MaterialGroup("src") { DefaultShape = child };
        var target = new Geometry("tgt");

        target.AddShapes([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.IsNull(source.DefaultShape);
    }

    [TestMethod]
    public void Other_Insert()
    {
        var child = new Line("myId");
        var source = new MaterialGroup("src") { DefaultShape = child };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.IsNull(source.DefaultShape);
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new Line("myId");
        var source = new MaterialGroup("src") { DefaultShape = child };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.IsNull(source.DefaultShape);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance_Add()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { DefaultGroup = child };

        parent.AddAltGroups([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.IsNull(parent.DefaultGroup);
    }

    [TestMethod]
    public void OtherInSameInstance_Insert()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { DefaultGroup = child };

        parent.InsertAltGroups(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.IsNull(parent.DefaultGroup);
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { DefaultGroup = child };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_altGroups, new List<MaterialGroup> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.IsNull(parent.DefaultGroup);
    }

    #endregion
}