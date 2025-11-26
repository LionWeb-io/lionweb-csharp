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
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        parent.Add(ShapesLanguage.Instance.CompositeShape_parts, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, -1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 1, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circle, line }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 0, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { line, circleA, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 1, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, line, circleB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var line = new Line("myId");
        parent.Insert(ShapesLanguage.Instance.CompositeShape_parts, 2, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Parts.Contains(line));
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, line }, parent.Parts.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new CompositeShape("cs");
        var line = new Line("myId");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, [line]));
        Assert.IsNull(line.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Parts.Contains(line));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new CompositeShape("cs") { Parts = [circle] };
        var line = new Line("myId");
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [line] };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, [line]));
        Assert.AreSame(parent, line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { line }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [line, circle] };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [circle, line] };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, [line]);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Parts.ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        parent.Remove(ShapesLanguage.Instance.CompositeShape_parts, [line]);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Parts.ToList());
    }

    #endregion
}