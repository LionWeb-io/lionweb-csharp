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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.MultipleCollection;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class RemoveTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void Set()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void Empty()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void First()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB, circle] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }
}