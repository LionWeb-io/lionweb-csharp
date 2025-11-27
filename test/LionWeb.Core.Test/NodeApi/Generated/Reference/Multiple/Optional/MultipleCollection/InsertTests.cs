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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.MultipleCollection;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class InsertTests
{
    [TestMethod]
    public void ListMatchingType()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void ListSubtype()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Set()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleEnumerable()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Empty()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void One_Before()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void One_After()
    {
        var circle = new Circle("cId");
        var parent = new ReferenceGeometry("g") { Shapes = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(1, values);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle, valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(1, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, valueA, valueB, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new ReferenceGeometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(2, values);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, valueA, valueB }, parent.Shapes.ToList());
    }
}