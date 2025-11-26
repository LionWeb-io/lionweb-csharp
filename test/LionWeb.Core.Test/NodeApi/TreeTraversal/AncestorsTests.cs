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
public class AncestorsTests
{
    #region noParent

    [TestMethod]
    public void NoParent()
    {
        var node = new Coord("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void NoParent_Self()
    {
        var node = new Coord("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Ancestors(true).ToList());
    }

    [TestMethod]
    public void NoParent_Annotation()
    {
        var node = new Documentation("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void NoParent_Annotation_Self()
    {
        var node = new Documentation("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Ancestors(true).ToList());
    }

    #endregion

    #region oneParent

    [TestMethod]
    public void OneParent()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        CollectionAssert.AreEqual(new List<INode> { parent }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void OneParent_Self()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        CollectionAssert.AreEqual(new List<INode> { node, parent }, node.Ancestors(true).ToList());
    }

    [TestMethod]
    public void OneParent_Annotation()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        CollectionAssert.AreEqual(new List<INode> { parent }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void OneParent_Annotation_Self()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        CollectionAssert.AreEqual(new List<INode> { node, parent }, node.Ancestors(true).ToList());
    }

    #endregion

    #region manyParents

    [TestMethod]
    public void ManyParents()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { parent, ancestor }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void ManyParents_Self()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { node, parent, ancestor }, node.Ancestors(true).ToList());
    }

    [TestMethod]
    public void ManyParents_Annotation()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { parent, ancestor }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void ManyParents_Annotation_Self()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { node, parent, ancestor }, node.Ancestors(true).ToList());
    }

    #endregion
}