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

namespace LionWeb.Core.Test.NodeApi.TreeTraversal;

using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class AncestorTests
{
    [TestMethod]
    public void NoParent()
    {
        var node = new Circle("n");
        Assert.AreEqual(null, node.Ancestor<Shape>(false));
        Assert.IsNull(node.Ancestor<Shape>(false));
    }

    [TestMethod]
    public void NoParent_Self()
    {
        var node = new Circle("n");
        Assert.AreEqual(node, node.Ancestor<Shape>(true));
        Assert.IsNotNull(node.Ancestor<Shape>(true));
    }

    [TestMethod]
    public void ManyParents()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };

        Assert.IsNotNull(node.Ancestor<Shape>());
        Assert.AreEqual(parent, node.Ancestor<Shape>());

        Assert.IsNotNull(parent.Ancestor<Geometry>());
        Assert.AreEqual(ancestor, parent.Ancestor<Geometry>());
    }

    [TestMethod]
    public void ManyParents_Self()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };

        Assert.IsNotNull(node.Ancestor<Shape>(true));
        Assert.AreEqual(parent, node.Ancestor<Shape>(true));

        Assert.IsNotNull(parent.Ancestor<Geometry>(true));
        Assert.AreEqual(ancestor, parent.Ancestor<Geometry>(true));
    }
}