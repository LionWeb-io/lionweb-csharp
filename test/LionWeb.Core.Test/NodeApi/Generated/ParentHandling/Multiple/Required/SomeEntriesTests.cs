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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling.Multiple.Required;

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
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.AddParts([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void Partial_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.InsertParts(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void Partial_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    #endregion

    #region Other

    [TestMethod]
    public void Other_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new Geometry("tgt");

        target.AddShapes([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void Other_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.AddDisabledParts([childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.DisabledParts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void OtherInSameInstance_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.InsertDisabledParts(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.DisabledParts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_disabledParts, new List<IShape> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.DisabledParts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, parent.Parts.ToList());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SameInSameInstance_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.AddParts([childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<IShape> { childB, childA }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Insert_Start()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.InsertParts(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA, childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Insert_Middle()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.InsertParts(1, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<IShape> { childB, childA }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Insert_End()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertParts(2, [childA]));

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA, childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void SameInSameInstance_Reflective()
    {
        var childA = new Line("myId");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.Parts.ToList());
    }

    #endregion
}