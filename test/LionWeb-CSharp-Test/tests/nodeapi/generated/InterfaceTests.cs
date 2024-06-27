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

namespace LionWeb.Core.M2.Generated.Test;

using Examples.Shapes.M2;
using M3;

[TestClass]
public class InterfaceTests
{
    [TestMethod]
    public void GetClassifier()
    {
        IShape node = new Circle("c");
        Assert.AreSame(ShapesLanguage.Instance.Circle, node.GetClassifier());
    }

    [TestMethod]
    public void GetProperty_Reflective()
    {
        IShape node = new Circle("c") { Uuid = "abc" };
        Assert.AreEqual("abc", node.Get(ShapesLanguage.Instance.IShape_uuid));
    }

    [TestMethod]
    public void GetContainment_Reflective()
    {
        Coord child = new Coord("coord");
        IShape node = new Circle("c") { Fixpoints = [child] };
        CollectionAssert.AreEqual(new List<Coord> { child },
            ((IReadOnlyList<Coord>)node.Get(ShapesLanguage.Instance.IShape_fixpoints)).ToList());
    }

    [TestMethod]
    public void SetProperty()
    {
        IShape node = new Circle("c");
        node.Uuid = "abc";
        Assert.AreEqual("abc", node.Uuid);
    }

    [TestMethod]
    public void SetProperty_Reflective()
    {
        IShape node = new Circle("c");
        node.Set(ShapesLanguage.Instance.IShape_uuid, "abc");
        Assert.AreEqual("abc", node.Uuid);
    }

    [TestMethod]
    public void AddContainment()
    {
        Coord child = new Coord("coord");
        IShape prevParent = new Line("l") { Fixpoints = [child] };
        IShape node = new Circle("c");
        node.AddFixpoints([child]);
        CollectionAssert.AreEqual(new List<Coord> { child }, node.Fixpoints.ToList());
        Assert.IsFalse(prevParent.Fixpoints.Any());
        Assert.AreSame(node, child.GetParent());
    }

    [TestMethod]
    public void InsertContainment()
    {
        Coord child = new Coord("coord");
        IShape prevParent = new Line("l") { Fixpoints = [child] };
        IShape node = new Circle("c");
        node.InsertFixpoints(0, [child]);
        CollectionAssert.AreEqual(new List<Coord> { child }, node.Fixpoints.ToList());
        Assert.IsFalse(prevParent.Fixpoints.Any());
        Assert.AreSame(node, child.GetParent());
    }

    [TestMethod]
    public void RemoveContainment()
    {
        Coord child = new Coord("coord");
        IShape node = new Circle("c") { Fixpoints = [child] };
        node.RemoveFixpoints([child]);
        Assert.IsFalse(node.Fixpoints.Any());
        Assert.IsNull(child.GetParent());
    }

    [TestMethod]
    public void SetContainment_Reflective()
    {
        Coord child = new Coord("coord");
        IShape prevParent = new Line("l") { Fixpoints = [child] };
        IShape node = new Circle("c");
        node.Set(ShapesLanguage.Instance.IShape_fixpoints, new List<Coord> { child });
        CollectionAssert.AreEqual(new List<Coord> { child }, node.Fixpoints.ToList());
        Assert.IsFalse(prevParent.Fixpoints.Any());
        Assert.AreSame(node, child.GetParent());
    }

    [TestMethod]
    public void CollectAllSetFeatures()
    {
        Coord child = new Coord("coord");
        IShape node = new Circle("c") { Fixpoints = [child], Uuid = "abc", R = 1 };
        CollectionAssert.AreEquivalent(
            new List<Feature>
            {
                ShapesLanguage.Instance.IShape_uuid,
                ShapesLanguage.Instance.IShape_fixpoints,
                ShapesLanguage.Instance.Circle_r
            }, node.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void GetContainmentOf()
    {
        Coord child = new Coord("coord");
        IShape node = new Circle("c") { Fixpoints = [child] };
        Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, node.GetContainmentOf(child));
    }
}